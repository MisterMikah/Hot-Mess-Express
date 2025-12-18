using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Refs")]
    public Transform player;

    [Tooltip("Middle lane obstacles (dividers only, spawn in bursts).")]
    public GameObject[] dividerPrefabs;   // middle lane only

    [Tooltip("Side lane heavy obstacles that move toward the player.")]
    public GameObject[] truckPrefabs;     // left/right lanes

    [Tooltip("Side lane static hurdles that can be jumped or slid under.")]
    public GameObject[] fencePrefabs;     // left/right lanes

    [Header("Layout")]
    public float laneWidth = 3f;      // lanes: -laneWidth, 0, +laneWidth
    public float rowSpacing = 9f;     // distance between rows in Z
    public float spawnAhead = 80f;    // how far ahead to keep obstacles
    public float despawnBehind = 20f; // how far behind player to destroy

    [Header("Trucks")]
    public int maxTrucks = 2;
    [Range(0f, 1f)] public float truckChance = 0.5f;
    public Vector2Int truckRowsMinMax = new Vector2Int(2, 3);
    public float truckMoveSpeed = 5f;

    [Header("Divider Bursts (middle lane only)")]
    [Range(0f, 1f)] public float dividerBurstChance = 0.25f;
    public Vector2Int dividerBurstLen = new Vector2Int(3, 6);

    [Header("Side Lane Variety")]
    [Range(0f, 1f)] public float sideLaneObstacleChance = 0.6f;
    [Range(0f, 1f)] public float fenceChance = 0.5f;

    [Header("End Limit")]
    public float endObstacleBuffer = 15f;   // space before end road with no new obstacles

    private float nextSpawnZ;
    private readonly List<GameObject> active = new List<GameObject>();

    private bool spawningEnabled = true;
    private bool useEndLimit = false;
    private float endLimitZ = float.PositiveInfinity;

    private int[] laneTruckRows = new int[3];
    private int dividerBurstRowsLeft = 0;

    private int[] truckCountPerLane = new int[3];
    private int[] fenceCountPerLane = new int[3];
    private int currentTrucks = 0;

    private enum ObstacleType { None, Divider, Fence, Truck }

    void Start()
    {
        float baseZ = Mathf.Floor(player.position.z / rowSpacing) * rowSpacing;
        nextSpawnZ = baseZ + rowSpacing * 4f;
    }

    void Update()
    {
        if (!spawningEnabled) return;

        float targetZ = player.position.z + spawnAhead;

        if (useEndLimit)
        {
            float maxZBeforeEnd = endLimitZ - endObstacleBuffer;
            targetZ = Mathf.Min(targetZ, maxZBeforeEnd);
        }

        while (nextSpawnZ <= targetZ)
        {
            SpawnRow(nextSpawnZ);
            nextSpawnZ += rowSpacing;
        }

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

        // 1) continue existing multi-row trucks
        for (int lane = 0; lane < 3; lane++)
        {
            if (laneTruckRows[lane] > 0)
            {
                blocked[lane] = true;
                type[lane] = ObstacleType.Truck;
                laneTruckRows[lane]--;
            }
        }

        // 2) middle-lane divider bursts
        if (dividerBurstRowsLeft > 0)
        {
            blocked[1] = true;
            type[1] = ObstacleType.Divider;
            dividerBurstRowsLeft--;
        }
        else if (Random.value < dividerBurstChance)
        {
            dividerBurstRowsLeft = Random.Range(dividerBurstLen.x, dividerBurstLen.y + 1);
            blocked[1] = true;
            type[1] = ObstacleType.Divider;
            dividerBurstRowsLeft--;
        }

        // 3) extra obstacles, keeping at least one lane free-ish
        List<int> freeLanes = new List<int>();
        for (int lane = 0; lane < 3; lane++)
        {
            if (!blocked[lane])
                freeLanes.Add(lane);
        }

        int maxNew = Mathf.Max(0, freeLanes.Count - 1);
        int newCount = (freeLanes.Count > 0) ? Random.Range(0, maxNew + 1) : 0;

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
                if (type[1] == ObstacleType.None)
                {
                    blocked[1] = true;
                    type[1] = ObstacleType.Divider;
                }
            }
            else
            {
                bool laneHasTruck = truckCountPerLane[lane] > 0;
                bool laneHasFence = fenceCountPerLane[lane] > 0;

                if (Random.value > sideLaneObstacleChance)
                    continue;

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
                    spawnTruck = Random.value < truckChance;
                }
                else if (canTruck)
                {
                    spawnTruck = true;
                }
                else
                {
                    spawnTruck = false;
                }

                if (spawnTruck)
                {
                    blocked[lane] = true;
                    type[lane] = ObstacleType.Truck;

                    int rows = Random.Range(truckRowsMinMax.x, truckRowsMinMax.y + 1);
                    laneTruckRows[lane] = rows - 1;
                }
                else
                {
                    blocked[lane] = true;
                    type[lane] = ObstacleType.Fence;
                }
            }
        }

        // 3.5) guarantee at least one "safe" lane
        int hardCount = 0;
        for (int lane = 0; lane < 3; lane++)
        {
            if (type[lane] == ObstacleType.Truck || type[lane] == ObstacleType.Divider)
                hardCount++;
        }

        if (hardCount == 3)
        {
            int laneToSoften = (Random.value < 0.5f) ? 0 : 2;

            if (type[laneToSoften] == ObstacleType.Truck || type[laneToSoften] == ObstacleType.Divider)
            {
                if (fencePrefabs.Length > 0)
                    type[laneToSoften] = ObstacleType.Fence;
                else
                    type[laneToSoften] = ObstacleType.None;
            }
        }

        for (int lane = 0; lane < 3; lane++)
        {
            Vector3 pos = LanePos(lane, z);

            switch (type[lane])
            {
                case ObstacleType.Divider:
                    SpawnDivider(pos);
                    break;
                case ObstacleType.Fence:
                    SpawnFence(pos, lane);
                    break;
                case ObstacleType.Truck:
                    SpawnTruck(pos, lane);
                    break;
            }
        }
    }

    Vector3 LanePos(int laneIndex, float z)
    {
        float x = (laneIndex - 1) * laneWidth;
        return new Vector3(x, 0f, z);
    }

    GameObject PickRandom(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) return null;
        int idx = Random.Range(0, prefabs.Length);
        return prefabs[idx];
    }

    void SpawnDivider(Vector3 pos)
    {
        GameObject prefab = PickRandom(dividerPrefabs);
        if (!prefab) return;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        active.Add(go);
    }

    void SpawnFence(Vector3 pos, int laneIndex)
    {
        GameObject prefab = PickRandom(fencePrefabs);
        if (!prefab) return;

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        active.Add(go);

        if (laneIndex == 0 || laneIndex == 2)
            fenceCountPerLane[laneIndex]++;
    }

    void SpawnTruck(Vector3 pos, int laneIndex)
    {
        GameObject prefab = PickRandom(truckPrefabs);
        if (!prefab) return;

        Quaternion rot = Quaternion.Euler(0f, 180f, 0f);
        GameObject go = Instantiate(prefab, pos, rot);
        active.Add(go);

        var mover = go.AddComponent<TruckMover>();
        mover.speed = truckMoveSpeed;

        if (laneIndex == 0 || laneIndex == 2)
            truckCountPerLane[laneIndex]++;

        currentTrucks = Mathf.Clamp(currentTrucks + 1, 0, maxTrucks);
    }

    void HandleDespawn(GameObject go)
    {
        var mover = go.GetComponent<TruckMover>();
        int laneIndex = LaneIndexFromX(go.transform.position.x);

        if (mover != null)
        {
            currentTrucks = Mathf.Max(0, currentTrucks - 1);

            if (laneIndex >= 0 && laneIndex < 3)
            {
                truckCountPerLane[laneIndex] = Mathf.Max(0, truckCountPerLane[laneIndex] - 1);
            }
        }
        else
        {
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

    public void SetEndLimit(float endZ)
    {
        useEndLimit = true;
        endLimitZ = endZ;
    }

    public void StopSpawning()
    {
        spawningEnabled = false;
    }
}
