using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject[] clouds;
    public Transform cloudsParent;
    public int initialCount = 100;

    List<GameObject> inGameClouds = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCloud());
    }

    IEnumerator SpawnCloud()
    {
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < initialCount; i++)
        {
            GameObject cloud = Instantiate(clouds[Random.Range(0, clouds.Length)], cloudsParent) as GameObject;
            cloud.SetActive(false);
            inGameClouds.Add(cloud);

            if (i % 20 == 0 && i > 0)
                yield return null;
        }
    }
}
