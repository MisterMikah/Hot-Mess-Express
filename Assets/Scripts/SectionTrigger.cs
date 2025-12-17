using System.Collections.Generic;
using UnityEngine;
using System;   // for Array.Sort

public class SectionTrigger : MonoBehaviour
{
    [Header("Road Spawning")]
    public GameObject roadSection;    // road section PREFAB to spawn
    public float sectionLength = 100f;
    public int maxSections = 3;

    // oldest -> newest
    private readonly Queue<GameObject> sections = new Queue<GameObject>();

    void Start()
    {
        // Find all existing road sections in the scene (start pieces)
        GameObject[] startSections = GameObject.FindGameObjectsWithTag("RoadSection");
        if (startSections.Length == 0)
        {
            Debug.LogWarning("SectionTrigger: No objects tagged 'RoadSection' found in scene.");
            return;
        }

        // sort them by z so they’re in correct order
        Array.Sort(startSections, (a, b) =>
            a.transform.position.z.CompareTo(b.transform.position.z));

        foreach (var s in startSections)
            sections.Enqueue(s);

        // if you forgot to assign the prefab, use the last section as the prefab
        if (roadSection == null)
        {
            roadSection = startSections[startSections.Length - 1];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // only respond to the ROAD trigger volumes
        if (!other.CompareTag("RoadTrigger")) return;
        if (sections.Count == 0) return;

        // last spawned section is the one at the back of the queue
        GameObject last = null;
        foreach (var s in sections)
            last = s;

        if (last == null) return;

        // spawn NEW section in front of the last one
        Vector3 spawnPos = last.transform.position + last.transform.forward * sectionLength;
        GameObject newSection = Instantiate(roadSection, spawnPos, last.transform.rotation);

        sections.Enqueue(newSection);

        // now safely clean up *behind* the player
        CleanupOldSections();
    }

    private void CleanupOldSections()
    {
        // don't go crazy if count is low
        if (sections.Count <= maxSections) return;

        // only delete sections that are clearly behind the player
        // tweak the buffer if needed
        float safeBehindZ = transform.position.z - sectionLength * 0.5f;

        while (sections.Count > maxSections)
        {
            GameObject oldest = sections.Peek();
            if (oldest == null)
            {
                sections.Dequeue();
                continue;
            }

            // if the oldest section is still under / slightly ahead of the player, stop
            if (oldest.transform.position.z + sectionLength * 0.5f > safeBehindZ)
            {
                // it's not safely behind us yet, don't delete it
                break;
            }

            // it's safely behind, kill it
            sections.Dequeue();
            Destroy(oldest);
        }
    }
}
