using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionsTeller : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (collision.gameObject.GetComponentInParent<HurdleController>())
                collision.gameObject.GetComponentInParent<HurdleController>().RegisterDirection();

            GameController.instance.AddToHighscore();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            if (collision.gameObject.GetComponentInParent<HurdleController>())
                collision.gameObject.GetComponentInParent<HurdleController>().SetUsed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("FallDetector"))
        {
            GameController.instance.InitateStop();
        }
    }
}
