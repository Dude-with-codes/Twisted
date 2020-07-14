using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixerSnapshot mainMenu, gameplay, mute;
    public float transitionTime = 2f;

    bool isMuted = false, isGameplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        if(GameController.instance)
        {
            GameController.instance.gameStarted += GameStarted;
            GameController.instance.gameEnded += GameEnded;
        }

        isMuted = PlayerPrefs.GetInt("IsMute") == 1;

        if(isMuted)
        {
            mute.TransitionTo(0.5f);
        }
    }

    void GameStarted()
    {
        gameplay.TransitionTo(transitionTime);

        isGameplayed = true;
    }

    void GameEnded()
    {
        mainMenu.TransitionTo(GameController.instance.curtainTransitionTime);

        isGameplayed = false;
    }

    public void Mute()
    {
        if (isMuted)
        {
            if (isGameplayed)
                gameplay.TransitionTo(0.5f);
            else
                mainMenu.TransitionTo(0.5f);
        }
        else
            mute.TransitionTo(0.5f);

        isMuted = !isMuted;
    }
}
