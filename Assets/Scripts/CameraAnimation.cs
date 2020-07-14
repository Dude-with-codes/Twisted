using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraAnimation : MonoBehaviour
{
    public enum CameraModes
    {
        backingToMenu,
        menu,
        storeEnv,
        storeBall
    }

    public Image curtains;
    public Color curtainStart, curtainTarget;
    public Transform cameraObj, kimble;
    public Vector3 cameraStarting, cameraEnding, cameraGameMode, kimbleMenuRotation, cameraStartingRotation;
    public Vector3 cameraStoreEnvPos, cameraStoreBallPos, kimbleStoreEnv, kimbleStoreBall, cameraStoreBallRot;
    public bool preGameplay = false;
    public bool inStore = false;
    public float zoomTime = 50f;
    public float storeTransitionTime;
    public CameraModes currentMode = CameraModes.menu;
    
    float curtainTransitionTime;
    float storePressTimestamp = 0f;
    Vector3 currentCamPos, lastPosInMenu, cameraRotInMenu, lastRotation, lastKimbleRotation;
    [SerializeField]
    float updateProgress, x, y;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.instance)
        {
            GameController.instance.gameStarted += Action;
            GameController.instance.gameEnded += GameEnd;
        }
        else
        {
            Debug.LogError("No Instance");
        }

        curtainTransitionTime = GameController.instance.curtainTransitionTime;
        StartCoroutine(Curtains(curtainStart, curtainTarget));
        StartAnimation();
    }

    void Update()
    {
        if(currentMode != CameraModes.menu)
        {
            if(updateProgress < 1f)
            {
                updateProgress += Time.deltaTime;

                if(currentMode == CameraModes.storeEnv)
                {
                    cameraObj.localPosition = Vector3.Lerp(currentCamPos, cameraStoreEnvPos, updateProgress);
                    cameraObj.localEulerAngles = Vector3.Lerp(lastRotation, cameraStartingRotation, updateProgress);

                    x = Mathf.Lerp(lastKimbleRotation.x, kimbleStoreEnv.x, updateProgress);
                    y = Mathf.Lerp(lastKimbleRotation.y, kimbleStoreEnv.y, updateProgress);

                    kimble.localEulerAngles = (Vector3.right * x) + (Vector3.up * y) + (Vector3.forward * kimbleStoreEnv.z);
                }

                if (currentMode == CameraModes.storeBall)
                {
                    cameraObj.localPosition = Vector3.Lerp(currentCamPos, cameraStoreBallPos, updateProgress);
                    cameraObj.localEulerAngles = Vector3.Lerp(lastRotation, cameraStoreBallRot, updateProgress);

                    x = Mathf.Lerp(lastKimbleRotation.x, kimbleStoreBall.x, updateProgress);
                    y = Mathf.Lerp(lastKimbleRotation.y, kimbleStoreBall.y, updateProgress);

                    kimble.localEulerAngles = (Vector3.right * x) + (Vector3.up * y) + (Vector3.forward * kimbleStoreBall.z);
                }

                if (currentMode == CameraModes.backingToMenu)
                {
                    cameraObj.localPosition = Vector3.Lerp(currentCamPos, lastPosInMenu, updateProgress);
                    cameraObj.localEulerAngles = Vector3.Lerp(lastRotation, cameraRotInMenu, updateProgress);

                    x = Mathf.Lerp(lastKimbleRotation.x, kimbleMenuRotation.x, updateProgress);
                    y = Mathf.Lerp(lastKimbleRotation.y, kimbleMenuRotation.y, updateProgress);

                    kimble.localEulerAngles = (Vector3.right * x) + (Vector3.up * y) + (Vector3.forward * kimbleMenuRotation.z);

                    if (updateProgress > 1f)
                    {
                        currentMode = CameraModes.menu; 
                    }
                }
            }
        }
    }

    void BackToMenu()
    {
        currentCamPos = cameraObj.localPosition;
        lastRotation = cameraObj.localEulerAngles;
        lastKimbleRotation = kimble.localEulerAngles;
        currentMode = CameraModes.backingToMenu;
        updateProgress = 0f;

        if (lastRotation.x > 180f)
            lastRotation.x -= 360f;

        if (lastRotation.y > 180f)
            lastRotation.y -= 360f;

        if (lastKimbleRotation.y > 180f)
            lastKimbleRotation.y -= 360f;
    }

    void TakeToEnv()
    {
        currentCamPos = cameraObj.localPosition;
        lastRotation = cameraObj.localEulerAngles;
        lastKimbleRotation = kimble.localEulerAngles;
        currentMode = CameraModes.storeEnv;
        updateProgress = 0f;
        GameController.instance.uiController.AssignListener(TakeToBall);

        if (lastRotation.x > 180f)
            lastRotation.x -= 360f;

        if (lastRotation.y > 180f)
            lastRotation.y -= 360f;

        if (lastKimbleRotation.y > 180f)
            lastKimbleRotation.y -= 360f;

        GameController.instance.uiController.ChangeBetweenBallAndEnvironment();
    }

    void TakeToBall()
    {
        currentCamPos = cameraObj.localPosition;
        lastRotation = cameraObj.localEulerAngles;
        lastKimbleRotation = kimble.localEulerAngles;
        currentMode = CameraModes.storeBall;
        updateProgress = 0f;
        GameController.instance.uiController.AssignListener(TakeToEnv);

        if (lastRotation.x > 180f)
            lastRotation.x -= 360f;

        if (lastRotation.y > 180f)
            lastRotation.y -= 360f;

        if (lastKimbleRotation.y > 180f)
            lastKimbleRotation.y -= 360f;

        GameController.instance.uiController.ChangeBetweenBallAndEnvironment();
    }

    public void Action()
    {
        preGameplay = false;
    }

    public void TakeToStore()
    {
        if(Time.time > storePressTimestamp + storeTransitionTime)
        {
            inStore = !inStore;

            if (currentMode == CameraModes.menu)
            {
                //StartCoroutine();
                TakeToEnv();
                lastPosInMenu = cameraObj.localPosition;
                cameraRotInMenu = cameraObj.localEulerAngles;

                if (cameraRotInMenu.x > 180f)
                    cameraRotInMenu.x -= 360f;

                if (cameraRotInMenu.y > 180f)
                    cameraRotInMenu.y -= 360f;
            }
            else
            {
                BackToMenu();
            }

            GameController.instance.uiController.AssignListener(TakeToBall);
            storePressTimestamp = Time.time;
        }
    }

    void GameEnd()
    {
        preGameplay = true;
        StartCoroutine(Curtains(curtainTarget, curtainStart));
        StartAnimation(curtainTransitionTime);
        StartCoroutine(Curtains(curtainStart, curtainTarget, curtainTransitionTime));
    }

    public void StartAnimation(float waitTime = 0f)
    {
        StartCoroutine(Animate(waitTime));
    }

    IEnumerator Animate(float wait)
    {
        yield return new WaitForSeconds(wait);

        kimble.localEulerAngles = kimbleMenuRotation;

        float progress = 0f;

        while(preGameplay)
        {
            progress += Time.deltaTime / zoomTime;
            cameraObj.localPosition = Vector3.Lerp(cameraStarting, cameraEnding, progress);
            cameraObj.localEulerAngles = Vector3.Lerp(cameraStartingRotation, Vector3.zero, progress);
            yield return null;
            yield return new WaitUntil(() => currentMode == CameraModes.menu);
        }

        progress = 0f;
        Vector3 current = cameraObj.localPosition;
        Vector3 currentRotation = cameraObj.localEulerAngles;

        if (currentRotation.x > 180f)
            currentRotation.x -= 360f;

        while(progress <= 1f)
        {
            progress += Time.deltaTime / 1.5f;
            cameraObj.localPosition = Vector3.Lerp(current, cameraGameMode, progress);
            cameraObj.localEulerAngles = Vector3.Lerp(currentRotation, Vector3.right * 22f, progress);
            kimble.localEulerAngles = Vector3.Lerp(kimbleMenuRotation, Vector3.zero, progress);

            yield return null;
        }

        StopCoroutine(Animate(wait));
    }

    IEnumerator Curtains(Color from, Color to, float wait = 0f)
    {
        yield return new WaitForSeconds(wait);

        float progress = 0f;

        while(progress < 1f)
        {
            progress += Time.deltaTime / curtainTransitionTime;

            curtains.color = Color.Lerp(from, to, progress);

            yield return null;
        }

        StopCoroutine(Curtains(from, to));
    }
}
