using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public GameObject[] terrains;
    public Transform terrainParent;
    public Vector3 startingSpawn;
    public Vector3 targetPosition;
    public float transitionTime;
    public int initialSpawns;

    List<GameObject> available = new List<GameObject>();
    List<GameObject> inUse = new List<GameObject>();
    List<float> positions = new List<float>();

    Vector3 spawnedPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.instance)
        {
            GameController.instance.gameStarted += StartSpawning;
            GameController.instance.gameEnded += EndSpawning;
        }

        spawnedPosition = startingSpawn;

        StartCoroutine(SpawnTerrain());
    }

    // Update is called once per frame
    void Update()
    {
        if(positions.Count > 0 && ReuseNext(positions[0]))
        {
            positions.RemoveAt(0);
            ReuseTerrain();
        }
    }

    IEnumerator SpawnTerrain()
    {
        int count = 0;

        while (count < terrains.Length)
        {
            for (int i = 0; i < initialSpawns; i++)
            {
                GameObject paharhh = Instantiate(terrains[count], terrainParent) as GameObject;
                available.Add(paharhh);
                paharhh.SetActive(false);
                paharhh.transform.localPosition = startingSpawn;
            }

            count++;
            yield return null;
        }
    }

    IEnumerator TerrainMaintainer()
    {
        foreach (GameObject obj in available)
        {
            obj.SetActive(false);
        }

        spawnedPosition = startingSpawn;
        float timeStore = 0f;
        int randomIndex = 0;
        GameObject plain;
        print(spawnedPosition);

        while (GameController.instance.State == GameController.GameState.live)
        {
            timeStore = Time.time;
            yield return new WaitUntil(() => (SpawnNext() && (Time.time - timeStore >= 0.1f)));

            for (int i = 0; i < 2; i++)
            {
                // Random Index calculator
                randomIndex = Random.Range(0, available.Count);

                plain = available[randomIndex];
                plain.SetActive(true);
                plain.transform.localPosition = spawnedPosition;
                plain.GetComponent<EnvironmentTransition>().StartOver();

                Vector3 scale = Vector3.one * 2f;
                scale.x = i == 0 ? -2 : 2;
                plain.transform.localScale = scale;

                inUse.Add(plain);
                available.RemoveAt(randomIndex);
            }

            spawnedPosition.z += 12f;
            positions.Add(spawnedPosition.z - 2f);
        }
    }

    void ReuseTerrain()
    {
        GameObject concernedPlain = inUse[0];
        GameObject otherConcernedPlain = inUse[1];

        for (int i = 0; i < 2; i++)
        {
            inUse.RemoveAt(0);
        }

        available.Add(concernedPlain);
        available.Add(otherConcernedPlain);
    }

    bool SpawnNext()
    {
        bool value = ((GameController.instance.player.transform.position.z) / (20f * 0.35f)) + 30f >= spawnedPosition.z;
        return value;
    }

    bool ReuseNext(float compareValue)
    {
        bool value = (GameController.instance.player.transform.position.z / (20f * 0.35f)) - 10f >= compareValue;
        return value;
    }

    public void StartSpawning()
    {
        StartCoroutine(TerrainMaintainer());
    }

    public void EndSpawning()
    {
        foreach (GameObject land in inUse)
        {
            available.Add(land);
        }

        inUse.Clear();
    }
}
