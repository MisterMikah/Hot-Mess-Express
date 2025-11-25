using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;

    [Tooltip("Middle lane obstacles (cones only, can burst).")]
    public GameObject[] conePrefabs;

    [Tooltip("Side lane heavy obstacles that move toward the player.")]
    public GameObject[] truckPrefabs;

    [Tooltip("Side lane static obstacles that can be jumped or slid under.")]
    public GameObject[] fencePrefabs;

    [Header("Layout")]
    public float laneWidth = 2.5f;     // lanes: -laneWidth, 0, +laneWidth
    public float rowSpacing = 9f;      // distance between rows in Z
    public float spawnAhead = 80f;     // how far ahead to keep obstacles
    public float despawnBehind = 20f;  // how far behind to destroy them

    [Header("Trucks")]
    [Range(0f, 1f)] public float truckChance = 0.3f;   // chance side lane gets a truck instead of fence
    public Vector2Int truckRowsMinMax = new Vector2Int(2, 3); // how many rows a truck occupies
    public float truckMoveSpeed = 5f;  // toward player

    [Header("Cone Bursts (middle lane only)")]
    [Range(0f, 1f)] public float coneBurstChance = 0.25f;
    public Vector2Int coneBurstLen = new Vector2Int(2, 4);

    private float nextSpawnZ;
    private readonly List<GameObject> active = new List<GameObject>();

    // multi-row trucks per lane (only lanes 0 and 2 used)
    private int[] laneTruckRows = new int[3]; // [0]=left, [1]=middle(not used), [2]=right

    // middle cone burst
    private int coneBurstRowsLeft = 0;

    Vector3 LanePos(int laneIndex, float z)
    {
        // laneIndex: 0 = left, 1 = middle, 2 = right
        float x = (laneIndex - 1) * laneWidth;
        return new Vector3(x, 0f, z);
    }

    void Start()
    {
        nextSpawnZ = Mathf.Floor(player.position.z / rowSpacing) * rowSpacing + rowSpacing * 4f;
    }

    void Update()
    {
        float targetZ = player.position.z + spawnAhead;

        // spawn forward rows
        while (nextSpawnZ <= targetZ)
        {
            SpawnRow(nextSpawnZ);
            nextSpawnZ += rowSpacing;
        }

        // despawn behind player
        for (int i = active.Count - 1; i >= 0; i--)
        {
            if (active[i] == null)
            {
                active.RemoveAt(i);
                continue;
            }

            if (active[i].transform.position.z < player.position.z - despawnBehind)
            {
                Destroy(active[i]);
                active.RemoveAt(i);
            }
        }
    }

    void SpawnRow(float z)
    {
        bool[] blocked = new bool[3]; // which lanes have obstacles this row

        // 1) Continue any trucks already in progress (side lanes only)
        for (int lane = 0; lane < 3; lane++)
        {
            if (laneTruckRows[lane] > 0)
            {
                blocked[lane] = true;
                laneTruckRows[lane]--;
            }
        }

        // 2) Middle-lane cone bursts
        if (coneBurstRowsLeft > 0)
        {
            blocked[1] = true;        // middle lane blocked by cone
            coneBurstRowsLeft--;
        }
        else if (Random.value < coneBurstChance)
        {
            coneBurstRowsLeft = Random.Range(coneBurstLen.x, coneBurstLen.y + 1);
            blocked[1] = true;
            coneBurstRowsLeft--; // consume first row now
        }

        // 3) Random extra blocks, but always leave at least one free lane
        List<int> candidates = new List<int>();
        for (int lane = 0; lane < 3; lane++)
            if (!blocked[lane])
                candidates.Add(lane);

        int extraBlocks = candidates.Count > 0
            ? Random.Range(0, Mathf.Min(2, candidates.Count))  // at most 2 extra lanes blocked
            : 0;

        // shuffle candidates
        for (int i = 0; i < candidates.Count; i++)
        {
            int j = Random.Range(i, candidates.Count);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        int blockedNow = 0;
        for (int i = 0; i < extraBlocks; i++)
        {
            blocked[candidates[i]] = true;
            blockedNow++;
        }

        // ensure at least one lane is free
        if (blocked[0] && blocked[1] && blocked[2])
            blocked[candidates[candidates.Count - 1]] = false;

        // 4) Actually spawn obstacles per lane according to your rules
        for (int lane = 0; lane < 3; lane++)
        {
            if (!blocked[lane]) continue;

            // ----- middle lane: CONES ONLY -----
            if (lane == 1)
            {
                GameObject conePrefab = PickRandom(conePrefabs);
                if (conePrefab)
                    Spawn(conePrefab, LanePos(lane, z), lane, isTruck: false);
                continue;
            }

            // ----- side lanes (0 & 2): trucks or fences -----
            bool continuingTruck = laneTruckRows[lane] > 0;

            if (continuingTruck)
            {
                // middle of a multi-row truck
                GameObject truckPrefab = PickRandom(truckPrefabs);
                if (truckPrefab)
                    Spawn(truckPrefab, LanePos(lane, z), lane, isTruck: true);
            }
            else
            {
                // starting a new obstacle: truck or fence
                bool spawnTruck = truckPrefabs.Length > 0 && Random.value < truckChance;

                if (spawnTruck)
                {
                    int rows = Random.Range(truckRowsMinMax.x, truckRowsMinMax.y + 1);
                    laneTruckRows[lane] = rows - 1; // this row is first
                    GameObject truckPrefab = PickRandom(truckPrefabs);
                    if (truckPrefab)
                        Spawn(truckPrefab, LanePos(lane, z), lane, isTruck: true);
                }
                else
                {
                    GameObject fencePrefab = PickRandom(fencePrefabs);
                    if (fencePrefab)
                        Spawn(fencePrefab, LanePos(lane, z), lane, isTruck: false);
                }
            }
        }
    }

    GameObject PickRandom(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) return null;
        int idx = Random.Range(0, prefabs.Length);
        return prefabs[idx];
    }

    void Spawn(GameObject prefab, Vector3 pos, int laneIndex, bool isTruck)
    {
        Quaternion rot = Quaternion.identity;

        if (isTruck)
        {
            // trucks face toward player (if +Z is player direction, truck faces -Z)
            rot = Quaternion.Euler(0f, 180f, 0f);
        }

        GameObject go = Instantiate(prefab, pos, rot);
        active.Add(go);

        // give trucks movement toward player
        if (isTruck)
        {
            var mover = go.AddComponent<TruckMover>();
            mover.speed = truckMoveSpeed;
        }
    }
}
