using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    [SerializeField]
    private GameObject[] environments;
    [SerializeField]
    private Transform environmentsParent;
    [SerializeField]
    private Transform storeChainBase, storeChainParent;
    [SerializeField]
    private Vector3 storeChainUpPosition, storeChainDownPosition;
    [SerializeField]
    private SpriteRenderer storeMode;
    [SerializeField]
    private Sprite area, ball;
    [SerializeField]
    private Text watchVidMainText, watchVidShadowText;

    [SerializeField]
    private bool rotateThePlank, moveThePlank;
    private float plankRotationProgress, plankPositionProgress;
    private int currentEnv;
    private bool isInStore = false;
    private bool inTransit = false;
    private int rotationDirection = 1;

    public int CurrentEnv { get { return currentEnv; } }

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("Current Env"))
        {
            currentEnv = PlayerPrefs.GetInt("Current Env");
        }
        else
        {
            PlayerPrefs.SetInt("Current Env", 0);
            currentEnv = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Animation
        if (rotateThePlank/* && plankRotationProgress <= 1f*/)
        {
            plankRotationProgress += Time.deltaTime * rotationDirection;
            float currentY = Mathf.Lerp(-180f, 180f, plankRotationProgress);
            storeChainParent.localEulerAngles = Vector3.up * currentY;
        }

        if (moveThePlank)
        {
            // Bring Plank down
            if (!isInStore && plankPositionProgress <= 1f)
            {
                plankPositionProgress += Time.deltaTime;
                storeChainBase.localPosition = Vector3.Lerp(storeChainUpPosition, storeChainDownPosition, plankPositionProgress);
            }

            // Take Plank up
            if (isInStore && plankPositionProgress > 0f)
            {
                plankPositionProgress -= Time.deltaTime;
                storeChainBase.localPosition = Vector3.Lerp(storeChainUpPosition, storeChainDownPosition, plankPositionProgress);
            }
        }
    }

    public void ChangeBetweenBallAndEnv()
    {
        StartCoroutine(RotatePlank());
    }

    public void ActivateStore()
    {
        StartCoroutine(MovePlank());
    }

    IEnumerator RotatePlank()
    {
        rotateThePlank = true;
        yield return new WaitForSeconds(.5f);

        storeMode.sprite = rotationDirection == 1 ? area : ball;

        yield return new WaitForSeconds(.5f);
        rotationDirection *= -1;
        rotateThePlank = false;
    }

    IEnumerator MovePlank()
    {
        yield return new WaitForSeconds(!isInStore ? GameController.instance.titleInOutTime + GameController.instance.titleDelay : 0f);
        moveThePlank = true;
        yield return new WaitForSeconds(1.1f);
        isInStore = !isInStore;
        moveThePlank = false;
    }

    public void NextEnvironment()
    {
        if (!inTransit)
        {
            inTransit = true;

            if (currentEnv < environments.Length - 1)
            {
                currentEnv++;
                StartCoroutine(ChangeEnvironment(currentEnv - 1));
            }
            else
            {
                StartCoroutine(DenyRotation(-1));
            }
        }
    }

    public void PreviousEnvironment()
    {
        if (!inTransit)
        {
            inTransit = true;

            if (currentEnv > 0)
            {
                currentEnv--;
                StartCoroutine(ChangeEnvironment(currentEnv + 1));
            }
            else
            {
                StartCoroutine(DenyRotation(1));
            }
        }
    }

    IEnumerator ChangeEnvironment(int previousEnv)
    {
        float localProgress = 0f;

        environments[currentEnv].SetActive(true);
        float currentZ;

        while(localProgress <= 1f)
        {
            localProgress += Time.deltaTime / .75f;

            //currentZ = Mathf.Lerp(pastZ, targetZ, localProgress);
            //environmentsParent.localEulerAngles = Vector3.forward * currentZ;

            currentZ = 180 * (Time.deltaTime / .75f) * (previousEnv - currentEnv);
            environmentsParent.Rotate(Vector3.forward * currentZ);

            yield return null;
        }

        environmentsParent.localEulerAngles = Vector3.forward * (180 * (currentEnv % 2));
        environments[previousEnv].SetActive(false);
        inTransit = false;

        StopCoroutine(ChangeEnvironment(previousEnv));
    }

    IEnumerator DenyRotation(int direction)
    {
        float localProgress = 0f;

        float pastZ, currentZ, targetZ;
        pastZ = environmentsParent.localEulerAngles.z;
        targetZ = pastZ + (5f * direction);

        while (localProgress <= 2f)
        {
            localProgress += Time.deltaTime / .15f;

            currentZ = Mathf.Lerp(pastZ, targetZ, localProgress < 1 ? localProgress : (2f - localProgress));
            environmentsParent.localEulerAngles = Vector3.forward * currentZ;

            yield return null;
        }
        
        inTransit = false;

        StopCoroutine(DenyRotation(direction));
    }
}

public class EnvironmentMeta
{

    //public void Watched
}
