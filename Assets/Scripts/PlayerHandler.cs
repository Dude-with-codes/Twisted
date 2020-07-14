using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject ball;
    public AudioSource jumpSource;
    public float timeToCompleteJump;
    public Vector3 startingPosition;
    //[Range(0f, 50f)]
    //public float jumpForce;
    //[Range(0f, 10f)]
    //public float downDrag;
    [Range(0.1f, 10f)]
    public float motionSpeed;
    public bool isJumping = false;
    public float jumpStartTime = 0f;

    Rigidbody ballBody;//, selfBody;
    [Range(0f, 5f)]
    public float jumpTime;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.instance)
        {
            GameController.instance.gameEnded += InitiateStart;
            //GameController.instance.gameStarted += REFACTOR HERE!
        }

        ballBody = ball.GetComponent<Rigidbody>();
        //selfBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isJumping)
        {
            jumpTime = (Time.time - jumpStartTime) * Mathf.PI * GameController.instance.gameSpeed;

            if (jumpTime >= Mathf.PI)
            {
                isJumping = false;
                ballBody.useGravity = true;
                jumpTime = Mathf.PI;
            }

            ball.transform.localPosition = Vector3.up * (Mathf.Abs(Mathf.Sin(jumpTime) * -6f) - 6f);
        }

        // MOVING FORWARD...
        if (GameController.instance.State == GameController.GameState.live)
        {
            Vector3 temp = transform.position;
            temp.z += (Time.deltaTime * (motionSpeed * GameController.instance.gameSpeed));
            transform.position = temp;

            if (AboveSlabs)
            {
                // CORRECTION in POSITION...
                temp = ball.transform.localPosition;
                temp.z = 0f;
                ball.transform.localPosition = temp;
            }
        }
    }

    void FixedUpdate()
    {
        //if (GameController.instance.State == GameController.GameState.live)
        //{
        //    selfBody.velocity = (Vector3.forward * GameController.instance.gameSpeed * 3f);
        //}
    }

    public void Jump()
    {
        //StartCoroutine(JumpSequence());

        //rigidbody.velocity = Vector3.up * jumpForce;

        if (!isJumping && AboveSlabs)
        {
            isJumping = true;
            ballBody.useGravity = false;
            jumpStartTime = Time.time;
            GameController.instance.RotateEnvironment();

            if (jumpSource)
                jumpSource.Play();
        }
    }

    public void InitiateStart()
    {
        Invoke("DelayedStart", GameController.instance.curtainTransitionTime);
    }

    public void DelayedStart()
    {
        transform.position = startingPosition;
        ball.transform.localPosition = Vector3.up * -6f;
        ballBody.velocity = Vector3.zero;
    }


    bool AboveSlabs
    {
        get
        {
            return ball.transform.position.y >= -6.05f;
        }
    }

    //IEnumerator JumpSequence()
    //{
    //    float progress = 0f;
    //    bool condition = false;
    //    int direction = 1;

    //    float phaseTime = timeToCompleteJump * 0.4f;
                
    //    while (transform.localPosition.y < -0.1f)
    //    {
    //        //progress += (Time.deltaTime / timeToCompleteJump);
    //        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime);

    //        yield return null;
    //    }

    //    while (transform.localPosition.y > -5.9f)
    //    {
    //        //progress += (Time.deltaTime / timeToCompleteJump);
    //        transform.localPosition = Vector3.Lerp(Vector3.zero, transform.localPosition, Time.deltaTime);

    //        yield return null;
    //    }

    //    StopCoroutine(JumpSequence());
    //}
}
