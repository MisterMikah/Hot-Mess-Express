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

    [Tooltip("Side lane static hurdles that can be jumped or slid under.")]
    public GameObject[] fencePrefabs;

    [Header("Layout")]
    public float laneWidth = 2.5f;     // lanes: -laneWidth, 0, +laneWidth
    public float rowSpacing = 9f;      // distance between rows in Z
    public float spawnAhead = 80f;     // how far ahead to keep obstacles
    public float despawnBehind = 20f;  // how far behind player to destroy

    [Header("Trucks")]
    public int maxTrucks = 2;          // max trucks alive at once
    [Range(0f, 1f)] public float truckChance = 0.3f;
    public Vector2Int truckRowsMinMax = new Vector2Int(2, 3);
    public float truckMoveSpeed = 5f;

    [Header("Cone Bursts (middle lane only)")]
    [Range(0f, 1f)] public float coneBurstChance = 0.25f;
    public Vector2Int coneBurstLen = new Vector2Int(2, 4);

    [Header("Side Lane Frequency")]
    [Range(0f, 1f)] public float sideLaneObstacleChance = 0.6f; // chance lane gets *anything*
    [Range(0f, 1f)] public float fenceChance = 0.5f;            // chance of fence when not truck


    private float nextSpawnZ;
    private readonly List<GameObject> active = new List<GameObject>();

    // multi-row trucks per lane (0 = left, 1 = middle, 2 = right)
    private int[] laneTruckRows = new int[3];

    // middle-lane cone bursts
    private int coneBurstRowsLeft = 0;

    // counts of active trucks/fences per lane so they never share a lane
    private int[] truckCountPerLane = new int[3];
    private int[] fenceCountPerLane = new int[3];
    private int currentTrucks = 0;

    private enum ObstacleType { None, Cone, Fence, Truck }

    void Start()
    {
        // start a bit ahead so nothing spawns on top of the player
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
            GameObject go = active[i];
            if (go == null)
            {
                active.RemoveAt(i);
                continue;
            }

            if (go.transform.position.z < player.position.z - despawnBehind)
            {
                HandleDespawn(go);
                Destroy(go);
                active.RemoveAt(i);
            }
        }
    }

    void SpawnRow(float z)
    {
        bool[] blocked = new bool[3];
        ObstacleType[] type = new ObstacleType[3];

        // 1) continue existing multi-row trucks (side lanes only)
        for (int lane = 0; lane < 3; lane++)
        {
            if (laneTruckRows[lane] > 0)
            {
                blocked[lane] = true;
                type[lane] = ObstacleType.Truck;
                laneTruckRows[lane]--; // one row consumed
            }
        }

        // check if both side lanes are already blocked this row by trucks
        bool sideLanesBlocked = blocked[0] && blocked[2];

        // 2) middle-lane cone bursts (cones only in lane 1)
        if (!sideLanesBlocked) // never block all three lanes
        {
            if (coneBurstRowsLeft > 0)
            {
                blocked[1] = true;
                type[1] = ObstacleType.Cone;
                coneBurstRowsLeft--;
            }
            else if (Random.value < coneBurstChance)
            {
                coneBurstRowsLeft = Random.Range(coneBurstLen.x, coneBurstLen.y + 1);
                blocked[1] = true;
                type[1] = ObstacleType.Cone;
                coneBurstRowsLeft--;
            }
        }

        // 3) decide extra obstacles for this row, but keep at least one lane free
        List<int> freeLanes = new List<int>();
        for (int lane = 0; lane < 3; lane++)
        {
            if (!blocked[lane])
                freeLanes.Add(lane);
        }

        int maxNew = Mathf.Max(0, freeLanes.Count - 1); // keep at least 1 lane free
        int newCount = (freeLanes.Count > 0) ? Random.Range(0, maxNew + 1) : 0;

        // shuffle free lanes
        for (int i = 0; i < freeLanes.Count; i++)
        {
            int j = Random.Range(i, freeLanes.Count);
            (freeLanes[i], freeLanes[j]) = (freeLanes[j], freeLanes[i]);
        }

        for (int i = 0; i < newCount; i++)
        {
            int lane = freeLanes[i];

            if (lane == 1)
            {
                // middle lane: cones only (if not already part of a burst)
                if (type[1] == ObstacleType.None)
                {
                    blocked[1] = true;
                    type[1] = ObstacleType.Cone;
                }
            }
            else
            {
                // side lanes: fences OR trucks (never both in same lane)
                bool laneHasTruck = truckCountPerLane[lane] > 0;
                bool laneHasFence = fenceCountPerLane[lane] > 0;

                // First, decide if we even place *anything* in this lane this row
                if (Random.value > sideLaneObstacleChance)
                {
                    // leave lane free this row
                    continue;
                }

                bool canTruck = truckPrefabs.Length > 0 &&
                                !laneHasFence &&
                                currentTrucks < maxTrucks;

                bool canFence = fencePrefabs.Length > 0 &&
                                !laneHasTruck;

                if (!canTruck && !canFence)
                    continue;

                bool spawnTruck = false;
                if (canTruck && canFence)
                {
                    // First decide if it should be a truck at all
                    if (Random.value < truckChance)
                    {
                        spawnTruck = true;
                    }
                    else
                    {
                        // If RNG says “not truck”, still only spawn fence sometimes
                        if (Random.value > fenceChance)
                            continue; // nothing in this lane this row
                    }
                }
                else if (canTruck)
                {
                    spawnTruck = true;
                }
                else
                {
                    // Only fences allowed, but still chance to skip
                    if (Random.value > fenceChance)
                        continue;
                }

                if (spawnTruck)
                {
                    blocked[lane] = true;
                    type[lane] = ObstacleType.Truck;

                    int rows = Random.Range(truckRowsMinMax.x, truckRowsMinMax.y + 1);
                    laneTruckRows[lane] = rows - 1; // current row is first
                }
                else
                {
                    blocked[lane] = true;
                    type[lane] = ObstacleType.Fence;
                }
            }
        }

        // 4) actually spawn the obstacles for this row
        for (int lane = 0; lane < 3; lane++)
        {
            Vector3 pos = LanePos(lane, z);

            switch (type[lane])
            {
                case ObstacleType.Cone:
                    SpawnCone(pos);
                    break;
                case ObstacleType.Fence:
                    SpawnFence(pos, lane);
                    break;
                case ObstacleType.Truck:
                    SpawnTruck(pos, lane);
                    break;
                case ObstacleType.None:
                    // free lane
                    break;
            }
        }
    }

    Vector3 LanePos(int laneIndex, float z)
    {
        // laneIndex: 0 = left, 1 = middle, 2 = right
        float x = (laneIndex - 1) * laneWidth;
        return new Vector3(x, 0f, z);
    }

    GameObject PickRandom(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) return null;
        int idx = Random.Range(0, prefabs.Length);
        return prefabs[idx];
    }

    void SpawnCone(Vector3 pos)
    {
        GameObject prefab = PickRandom(conePrefabs);
        if (!prefab) return;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        active.Add(go);
        // cones only in middle lane; they don't block trucks in side lanes
    }

    void SpawnFence(Vector3 pos, int laneIndex)
    {
        GameObject prefab = PickRandom(fencePrefabs);
        if (!prefab) return;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        active.Add(go);

        // track fences per side lane to prevent trucks in the same lane
        if (laneIndex == 0 || laneIndex == 2)
        {
            fenceCountPerLane[laneIndex]++;
        }
    }

    void SpawnTruck(Vector3 pos, int laneIndex)
    {
        GameObject prefab = PickRandom(truckPrefabs);
        if (!prefab) return;

        // face toward the player (assuming player faces +Z)
        Quaternion rot = Quaternion.Euler(0f, 180f, 0f);
        GameObject go = Instantiate(prefab, pos, rot);
        active.Add(go);

        // add movement
        var mover = go.AddComponent<TruckMover>();
        mover.speed = truckMoveSpeed;

        // track trucks per lane
        truckCountPerLane[laneIndex]++;
        currentTrucks = Mathf.Clamp(currentTrucks + 1, 0, maxTrucks);
    }

    void HandleDespawn(GameObject go)
    {
        // Called just before Destroy(go)
        var mover = go.GetComponent<TruckMover>();
        if (mover != null)
        {
            // truck despawn
            currentTrucks = Mathf.Max(0, currentTrucks - 1);

            int laneIndex = LaneIndexFromX(go.transform.position.x);
            if (laneIndex >= 0 && laneIndex < 3)
            {
                truckCountPerLane[laneIndex] = Mathf.Max(0, truckCountPerLane[laneIndex] - 1);
            }
        }
        else
        {
            // fence or cone
            int laneIndex = LaneIndexFromX(go.transform.position.x);
            if (laneIndex == 0 || laneIndex == 2)
            {
                fenceCountPerLane[laneIndex] = Mathf.Max(0, fenceCountPerLane[laneIndex] - 1);
            }
        }
    }

    int LaneIndexFromX(float x)
    {
        if (Mathf.Approximately(laneWidth, 0f)) return 1;

        int idx = Mathf.RoundToInt(x / laneWidth) + 1;
        return Mathf.Clamp(idx, 0, 2);
    }
}
