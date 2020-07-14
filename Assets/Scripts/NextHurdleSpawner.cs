using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextHurdleSpawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SpawnedPosition"))
        {
            GameController.instance.spawner.SetHurdleAhead();
        }
    }
}
