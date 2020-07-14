using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentTransition : MonoBehaviour
{
    float progress = 0f;
    bool startTransition = false;
    Vector3 current, target;

    public void StartOver()
    {
        progress = 0f;
        startTransition = true;
        current = target = transform.localPosition;
        print(current);
        target.y = -3.83f;
    }

    // Update is called once per frame
    void Update()
    {
        if(startTransition)
        {
            progress += Time.deltaTime / GameController.instance.terrainSpawner.transitionTime;

            transform.localPosition = Vector3.Lerp(current, target, progress);

            if(progress >= 1f)
            {
                startTransition = false;
            }
        }
    }
}
