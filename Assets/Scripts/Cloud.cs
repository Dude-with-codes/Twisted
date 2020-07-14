using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public bool isInView;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.right * Time.deltaTime * GameController.instance.cloudSpeed * GameController.instance.gameSpeed);
    }
}
