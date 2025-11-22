using System.Collections.Generic;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection;       // prefab to spawn
    public float sectionLength = 100f;    // length of one section
    public int maxSections = 3;          // how many to keep before destroying old ones

    private List<GameObject> spawnedSections = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("RoadTrigger")) return;

        // Get section root that owns this trigger
        Transform sectionRoot = other.transform.root;

        // Spawn next section ahead
        Vector3 spawnPos = sectionRoot.position + sectionRoot.forward * sectionLength;
        GameObject newSection = Instantiate(roadSection, spawnPos, sectionRoot.rotation);

        // Track the new section
        spawnedSections.Add(newSection);

        if (spawnedSections.Count > maxSections)
        {
            Destroy(spawnedSections[0]);
            spawnedSections.RemoveAt(0);
        }
        else if (spawnedSections.Count == 2)
        {
            // optional: destroy the initial starting piece after the first new section spawns
            GameObject startSection = GameObject.FindWithTag("StartSection");
            if (startSection != null)
                Destroy(startSection);
        }

    }
}
