using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class Template
{
    public Vector3 starting;
    public Vector3 ending;
    public Vector3 gameplay;
    public Vector3 gameEnd;
    public RectTransform element;
    public float transitionTime;

    protected float progress = 0f;


    public void StartAnimation()
    {
        GameController.instance.StartCoroutine(AnimationMagic());
    }

    public void InvertAnimation()
    {
        GameController.instance.StartCoroutine(InvertAnimationMagic());
    }

    public void GameplayAnimation()
    {
        GameController.instance.StartCoroutine(GameplayAnimationMagic());
    }

    public void GameEndAnimation()
    {
        GameController.instance.StartCoroutine(GameEndAnimationMagic());
    }

    protected virtual IEnumerator AnimationMagic()
    {
        yield return null;
    }

    protected virtual IEnumerator InvertAnimationMagic()
    {
        yield return null;
    }

    protected virtual IEnumerator GameplayAnimationMagic()
    {
        yield return null;
    }

    protected virtual IEnumerator GameEndAnimationMagic()
    {
        yield return null;
    }
}

[System.Serializable]
public class PositionAnimationTemplate : Template
{
    protected override IEnumerator AnimationMagic()
    {
        while (progress < 1f)
        {
            progress += Time.deltaTime / transitionTime;
            element.localPosition = Vector3.Lerp(starting, ending, progress);
            yield return null;
        }
    }

    protected override IEnumerator InvertAnimationMagic()
    {
        while (progress > 0f)
        {
            progress -= Time.deltaTime / transitionTime;
            element.localPosition = Vector3.Lerp(starting, ending, progress);
            yield return null;
        }
    }

    protected override IEnumerator GameplayAnimationMagic()
    {
        Vector3 current = element.localPosition;
        float localProgress = 0f;

        while (localProgress < 1f)
        {
            localProgress += Time.deltaTime / transitionTime;
            element.localPosition = Vector3.Lerp(current, gameplay, localProgress);
            yield return null;
        }
    }

    protected override IEnumerator GameEndAnimationMagic()
    {
        Vector3 current = element.localPosition;
        float localProgress = 0f;

        while (localProgress < 1f)
        {
            localProgress += Time.deltaTime / transitionTime;
            element.localPosition = Vector3.Lerp(current, gameEnd, localProgress);
            yield return null;
        }
    }
}

[System.Serializable]
public class RotationAnimationTemplate : Template
{
    protected override IEnumerator AnimationMagic()
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime / transitionTime;
            element.localEulerAngles = Vector3.Lerp(starting, ending, progress);
            yield return null;
        }
    }

    protected override IEnumerator InvertAnimationMagic()
    {
        while (progress > 0f)
        {
            progress -= Time.deltaTime / transitionTime;
            element.localEulerAngles = Vector3.Lerp(starting, ending, progress);
            yield return null;
        }
    }

    protected override IEnumerator GameplayAnimationMagic()
    {
        Vector3 current = element.localEulerAngles;

        while (progress > 0f)
        {
            progress -= Time.deltaTime / transitionTime;
            element.localEulerAngles = Vector3.Lerp(current, gameplay, progress);
            yield return null;
        }
    }
}

[System.Serializable]
public class ScaleAnimationTemplate : Template
{
    protected override IEnumerator AnimationMagic()
    {
        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime / transitionTime;
            element.localScale = Vector3.Lerp(starting, ending, progress);
            yield return null;
        }
    }

    protected override IEnumerator InvertAnimationMagic()
    {
        while (progress > 0f)
        {
            progress -= Time.deltaTime / transitionTime;
            element.localScale = Vector3.Lerp(starting, ending, progress);
            yield return null;
        }
    }

    protected override IEnumerator GameplayAnimationMagic()
    {
        Vector3 current = element.localScale;
        float localProgress = 0f;

        while (localProgress < 1f)
        {
            localProgress += Time.deltaTime / transitionTime;
            element.localScale = Vector3.Lerp(current, gameplay, localProgress);
            yield return null;
        }
    }
}

