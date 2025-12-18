using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SectionTrigger : MonoBehaviour
{
    [Header("Road Spawning")]
    public GameObject roadSection;        // normal road section PREFAB
    public float sectionLength = 100f;
    public int maxSections = 3;

    [Header("Level Length")]
    public int minExtraSections = 5;      // min normal sections to spawn
    public int maxExtraSections = 10;     // max normal sections to spawn
    public GameObject endSectionPrefab;   // special final section prefab

    [Header("Spawning Control")]
    public ObstacleSpawner obstacleSpawner;   // set this in Inspector

    // oldest -> newest
    private readonly Queue<GameObject> sections = new Queue<GameObject>();

    private int extraSectionsToSpawn;     // how many normal pieces this run gets
    private int extraSpawned = 0;         // how many we've spawned so far
    private bool endSpawned = false;      // once true, no more spawns

    void Start()
    {
        GameObject[] startSections = GameObject.FindGameObjectsWithTag("RoadSection");
        if (startSections.Length == 0)
        {
            Debug.LogWarning("SectionTrigger: No objects tagged 'RoadSection' found in scene.");
            return;
        }

        Array.Sort(startSections, (a, b) =>
            a.transform.position.z.CompareTo(b.transform.position.z));

        foreach (var s in startSections)
            sections.Enqueue(s);

        if (roadSection == null)
        {
            roadSection = startSections[startSections.Length - 1];
        }

        extraSectionsToSpawn = Random.Range(minExtraSections, maxExtraSections + 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("RoadTrigger")) return;
        if (sections.Count == 0) return;

        // prevent same trigger from firing multiple times
        other.enabled = false;

        if (endSpawned) return;

        GameObject last = null;
        foreach (var s in sections)
            last = s;

        if (last == null) return;

        GameObject prefabToSpawn;

        if (extraSpawned < extraSectionsToSpawn)
        {
            prefabToSpawn = roadSection;
            extraSpawned++;
        }
        else
        {
            prefabToSpawn = endSectionPrefab != null ? endSectionPrefab : roadSection;
            endSpawned = true;
        }

        Vector3 spawnPos = last.transform.position + last.transform.forward * sectionLength;
        GameObject newSection = Instantiate(prefabToSpawn, spawnPos, last.transform.rotation);
        sections.Enqueue(newSection);

        // tell spawner where the end section is, so it stops a bit before it
        if (endSpawned && obstacleSpawner != null)
        {
            obstacleSpawner.SetEndLimit(spawnPos.z);
        }

        CleanupOldSections();
    }

    private void CleanupOldSections()
    {
        if (sections.Count <= maxSections) return;

        float safeBehindZ = transform.position.z - sectionLength * 0.5f;

        while (sections.Count > maxSections)
        {
            GameObject oldest = sections.Peek();
            if (oldest == null)
            {
                sections.Dequeue();
                continue;
            }

            if (oldest.transform.position.z + sectionLength * 0.5f > safeBehindZ)
            {
                break;
            }

            sections.Dequeue();
            Destroy(oldest);
        }
    }
}
