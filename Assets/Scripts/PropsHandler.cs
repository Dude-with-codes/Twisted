using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsHandler : MonoBehaviour
{
    public bool isActivated = false, isPrimary = false;
    public Transform prop;
    public float progress;
    public int[] envId;
    [Range(0f, 5f)]
    public float maxScale;
    
    Vector3 targetScale;

    // Start is called before the first frame update
    void Start()
    {
        StartOver();

        if (GameController.instance && !isPrimary)
            GameController.instance.gameEnded += InitiateStartOver;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActivated && progress <= 1f)
        {
            progress += Time.deltaTime;

            ChangeScale(progress);
        }
    }

    public void ChangeScale(float value)
    {
        prop.localScale = Vector3.Lerp(Vector3.zero, targetScale, value);
    }

    public void StartOver()
    {
        isActivated = false;
        progress = 0f;
        prop.localScale = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("DaTrigger") && IsSelectedEnv())
        {
            progress = 0f;
            isActivated = true;

            targetScale = Vector3.one * maxScale;
        }
    }

    void InitiateStartOver()
    {
        Invoke("StartOver", 1f);
    }

    bool IsSelectedEnv()
    {
        bool value = false;

        foreach (int id in envId)
        {
            if(id == GameController.instance.store.CurrentEnv)
            {
                value = true;
                break;
            }
        }

        return value;
    }
}
