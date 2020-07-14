using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameEvent onStart;
    public GameEvent onGameStart;
    public GameEvent onGameOver;
    public GameEvent onEnvironmentView;
    public GameEvent onBallView;
    public GameEvent onBackingToMenu;

    [Space]
    //public PositionAnimationTemplate tapTextAnimation;
    //public PositionAnimationTemplate soundButtonAnimation;
    //public PositionAnimationTemplate storeButtonAnimation;
    //public PositionAnimationTemplate scoreAnimation;
    //public PositionAnimationTemplate highscoreAnimation;
    //public PositionAnimationTemplate scoreDividerAnimation;
    //public PositionAnimationTemplate titleAnimation;
    public ScaleAnimationTemplate countdownTextAnimation;
    public ScaleAnimationTemplate titleScaleAnimation;

    [Space]
    public Animator soundAnimator;
    public Animator storeAnimator;
    public Animator ballEnvAnimator;
    public Animator ballAnimator;
    public Text scoreText, highscoreText, countdownText;
    public Button ballEnvButton;

    int isMute = 0, countdown;
    bool isInStore = false, seeingEnvironment = false;

    private void Start()
    {
        Invoke("BringInPlayText", 3f);

        if(!PlayerPrefs.HasKey("IsMute"))
        {
            PlayerPrefs.SetInt("IsMute", 2);
        }

        isMute = PlayerPrefs.GetInt("IsMute");

        GameController.instance.gameStarted += GameStarted;
        GameController.instance.gameEnded += GameEnded;

        StartCoroutine(RandomizeBallAnimation());
        onStart.CallEvent();
    }

    public void BringInPlayText()
    {
        //tapTextAnimation.StartAnimation();
        Invoke("BringInSound", 1f);
    }

    void BringInSound()
    {
        //soundButtonAnimation.StartAnimation();
        //storeButtonAnimation.StartAnimation();
        //highscoreAnimation.StartAnimation();
        soundAnimator.SetInteger("IsMute", isMute);
    }

    public void GameStarted()
    {
        //tapTextAnimation.InvertAnimation();
        //highscoreAnimation.GameplayAnimation();
        //storeButtonAnimation.GameplayAnimation();
        //scoreAnimation.GameplayAnimation();
        //scoreDividerAnimation.GameplayAnimation();

        onGameStart.CallEvent();
    }

    void Countdown()
    {
        countdownText.text = countdown.ToString();
        countdownTextAnimation.StartAnimation();
        countdown--;
    }

    public void GameEnded()
    {
        Invoke("DelayedCalls", 1f);

        onGameOver.CallEvent();
    }

    void DelayedCalls()
    {
        BringInPlayText();
        //tapTextAnimation.StartAnimation();
        //highscoreAnimation.GameEndAnimation();
        //storeButtonAnimation.GameEndAnimation();
        //scoreAnimation.GameEndAnimation();
        //scoreDividerAnimation.GameEndAnimation();
    }

    public void AddScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void ShowHighscore(int highscore)
    {
        highscoreText.text = highscore.ToString();
        //highscoreAnimation.StartAnimation();
    }

    public void MuteUnmute()
    {
        soundAnimator.SetTrigger("Change");

        if (isMute == 1)
            isMute = 2;
        else
            isMute = 1;

        PlayerPrefs.SetInt("IsMute", isMute);
    }

    public void StoreButton()
    {
        storeAnimator.SetTrigger("Change");
        GameController.instance.TitleTransition();
        GameController.instance.store.ActivateStore();

        isInStore = !isInStore;
        GameController.instance.SetStateToStore(isInStore);

        if (isInStore)
            onEnvironmentView.CallEvent();
        else
            onBackingToMenu.CallEvent();
    }

    public void AssignListener(UnityAction function)
    {
        ballEnvButton.onClick.RemoveAllListeners();
        ballEnvButton.onClick.AddListener(function);
    }

    public void ChangeBetweenBallAndEnvironment()
    {
        ballEnvAnimator.SetTrigger("Change");

        seeingEnvironment = !seeingEnvironment;

        if (seeingEnvironment)
        {
            onEnvironmentView.CallEvent();
        }
        else
        {
            onBallView.CallEvent();
        }

        GameController.instance.store.ChangeBetweenBallAndEnv();
    }

    IEnumerator RandomizeBallAnimation()
    {
        float waitTime;
        int animNumber;

        while(true)
        {
            waitTime = Random.Range(3f, 5f);
            animNumber = Random.Range(1, 3);

            ballAnimator.SetInteger("JumpOrRoll", animNumber);

            yield return new WaitForSeconds(0.5f);

            ballAnimator.SetInteger("JumpOrRoll", 0);

            yield return new WaitForSeconds(waitTime - 0.5f);
        }
    }

    IEnumerator TitleAnimationSequence()
    {
        yield return new WaitForSeconds(2f);

        // Title animations pliss!

        countdown = 3;

        while (countdown > 0)
        {
            Countdown();
            yield return new WaitForSeconds(1f);
        }

        countdownTextAnimation.GameplayAnimation();
    }
}

[System.Serializable]
public class MoveableElement
{
    // Private Variables
    // Serialized
    [SerializeField]
    private RectTransform element;
    [SerializeField]
    private float delay;
    [SerializeField]
    private float transitionTime;
    [SerializeField]
    private Vector3 startingPosition;
    [SerializeField]
    private Vector3 endingPosition;
    [SerializeField]
    private bool useCurrentPosition = false;

    // Non-serialized
    private float progress = 0f;

    public IEnumerator MoveElement()
    {
        yield return new WaitForSeconds(delay);
        Vector3 currentPosition;
        progress = 0f;

        if (useCurrentPosition)
            currentPosition = element.localPosition;
        else
            currentPosition = startingPosition;

        while(progress < 1f)
        {
            progress += Time.deltaTime / transitionTime;
            element.localPosition = Vector3.Lerp(currentPosition, endingPosition, progress);

            yield return null;
        }
    }
}

[System.Serializable]
public class GameEvent
{
    [SerializeField]
    private MoveableElement[] elements;

    public void CallEvent()
    {
        foreach (var element in elements)
        {
            GameController.instance.StartCoroutine(element.MoveElement());
        }
    }
}