using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject hurdle;
    public Transform hurdleParent;
    public int initialCount;
    public float floorGap, angleGap, ballGap, deltaSpawnTime;
    public Transform spawnPositionKeeper, nextSpawner;
    public bool waiting = false;

    List<GameObject> hurdles = new List<GameObject>();
    int spawnedHurdles;
    Vector3 spawnPosition;
    float randomAngle = 0f, currentSpawnTime;
    int direction = 1;
    Vector3 scale;
    int randomScale;

    void Start()
    {
        if(GameController.instance)
        {
            GameController.instance.gameStarted += StartSpawning;
            GameController.instance.gameEnded += HideAll;
        }    
    }

    void Update()
    {
        if(PoolCondition && waiting)
        {
            ResetHurdle();
        }
    }

    void HideAll()
    {
        Invoke("DelayedHide", GameController.instance.curtainTransitionTime);
    }

    void DelayedHide()
    {
        foreach (GameObject item in hurdles)
        {
            item.SetActive(false);
        }
    }

    void SetSpawnPosition(bool reset, Vector3 pos)
    {
        if (reset)
            spawnPosition = pos;
        else
            spawnPosition += pos;

        spawnPositionKeeper.localPosition = spawnPosition;
    }

    void StartSpawning()
    {
        foreach(GameObject obj in hurdles)
        {
            obj.GetComponent<HurdleController>().isUsed = true;
            obj.SetActive(false);
        }

        randomAngle = 0f;
        direction = 0;
        SetSpawnPosition(true, Vector3.zero);
        spawnedHurdles = 0;

        if (hurdles.Count < initialCount)
        {
            StartCoroutine(SpawnHurdles());
        }
        else
        {
            StartCoroutine(PoolInitial());
        }
    }

    public void SetHurdleAhead()
    {
        ResetHurdle();
    }

    IEnumerator SpawnHurdles()
    {
        HurdleController hurdleObj;

        //while(GameController.instance.State == GameController.GameState.live)
        while(hurdles.Count < initialCount)
        {
            GameObject obj = Instantiate(hurdle, hurdleParent) as GameObject;
            hurdles.Add(obj);
            SetProperties(obj.transform);
            spawnedHurdles++;
            hurdleObj = obj.GetComponent<HurdleController>();
            currentSpawnTime = Time.time;

            yield return new WaitUntil(() => ShouldSpawn);

            SetAngleForNext();
            SetPositionForNext();
            hurdleObj.directionOfSpin = direction;
        }

        waiting = true;
        StopCoroutine(SpawnHurdles());
    }

    IEnumerator PoolInitial()
    {
        while (spawnedHurdles < initialCount)
        {
            ResetHurdle();
            spawnedHurdles++;
            currentSpawnTime = Time.time;

            yield return new WaitUntil(() => ItIsTime);
        }

        waiting = true;
        StopCoroutine(PoolInitial());
    }

    void ResetHurdle()
    {
        bool isPooled = false;
        HurdleController hurdleObj = null;

        foreach (GameObject obj in hurdles)
        {
            if (obj.GetComponent<HurdleController>().isUsed)
            {
                SetProperties(obj.transform);
                obj.GetComponent<HurdleController>().StartOver();
                isPooled = true;
                hurdleObj = obj.GetComponent<HurdleController>();
                obj.SetActive(true);
                break;
            }
        }

        if(!isPooled)
        {
            GameObject obj = Instantiate(hurdle, hurdleParent) as GameObject;
            hurdles.Add(obj);
            SetProperties(obj.transform);
            spawnedHurdles++;
            hurdleObj = obj.GetComponent<HurdleController>();
        }

        print("Pooled: " + isPooled);

        SetAngleForNext();
        SetPositionForNext();

        if (hurdleObj)
            hurdleObj.directionOfSpin = direction;

        // Shit Pool -_-
        hurdles.RemoveAt(0);
        hurdles.Add(hurdleObj.gameObject);
    }

    void SetProperties(Transform hurdleInstance)
    {
        SetScale();
        hurdleInstance.localScale = scale;
        hurdleInstance.localPosition = spawnPosition;
        hurdleInstance.localEulerAngles = Vector3.forward * randomAngle;
    }

    void SetScale()
    {
        randomScale = Random.Range(5, 15);
        scale = Vector3.one + (Vector3.forward * (randomScale - 1f));
    }

    void SetPositionForNext()
    {
        SetSpawnPosition(false, Vector3.forward * (randomScale + floorGap));
    }

    void SetAngleForNext()
    {
        direction = Random.Range(-1, 2);

        randomAngle += (angleGap * direction);
    }

    bool ShouldSpawn
    {
        get
        {
            bool value = spawnedHurdles <= initialCount && ItIsTime;
            return value;
        }
    }

    bool ItIsTime
    {
        get
        {
            bool value = Time.time >= currentSpawnTime + deltaSpawnTime;
            return value;
        }
    }

    bool PoolCondition
    {
        get
        {
            bool value = spawnPosition.z - GameController.instance.player.ball.transform.position.z < ballGap;
            return value;
        }
    }
}
