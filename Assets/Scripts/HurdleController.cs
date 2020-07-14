using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdleController : MonoBehaviour
{
    public Transform childObject;
    public float timeToComplete;
    public Vector3 startPosition, endPosition;
    public bool isUsed = false;
    public int directionOfSpin;

    float progress = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartOver()
    {
        progress = 0f;
        isUsed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(progress < 1f)
        {
            progress += Time.deltaTime / timeToComplete;

            childObject.localPosition = Vector3.Lerp(startPosition, endPosition, progress);
        }
    }

    public void RegisterDirection()
    {
        isUsed = false;
        GameController.instance.Direction = directionOfSpin;
    }

    public void SetUsed()
    {
        isUsed = true;
    }
}
