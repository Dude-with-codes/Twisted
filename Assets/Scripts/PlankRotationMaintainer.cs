using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankRotationMaintainer : MonoBehaviour
{
    float x, y, z;

    void Start()
    {
        x = transform.localEulerAngles.x;
        y = transform.localEulerAngles.y;
        z = transform.localEulerAngles.z;
    }

    void LateUpdate()
    {
        transform.localEulerAngles = Vector3.right * x + Vector3.up * y + Vector3.forward * z;
    }
}
