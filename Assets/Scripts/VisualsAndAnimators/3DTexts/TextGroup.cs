using UnityEngine;
using System.Collections;
using System;
using System.Timers;
[Serializable]
public class TextGroup :MonoBehaviour
{
    [SerializeField] float[] delays;
    [Space]
    [SerializeField] float midRotateDuration = 2;
    [Space]
    [SerializeField] bool loop;
    [SerializeField] int minimumLoopsToProceed;
    [SerializeField] GameStates.State targetState;

    public float MidRotateDuration => midRotateDuration;
    int loopsCounter;
    Text3D[] text3Ds;
    private void Awake()
    {
        int childCount = transform.childCount;
        text3Ds = new Text3D[childCount];
        for (int i = 0; i < childCount; i++)
        {
            Text3DAnimationController _textAnimatorController= transform.GetChild(i).GetComponent<Text3DAnimationController>();
            GameObject _textGameObject = _textAnimatorController.transform.GetChild(1).gameObject;
            text3Ds[i] = new Text3D(_textAnimatorController, _textGameObject, delays[i]);
        }
    }
    public void EnterPlay(Action callBack=null)
    {
        ResetValues();
        Play(callBack);
    }
    private void ResetValues()
    {
        loopsCounter = 0;
    }
    public void Play(Action callBack=null)
    {
        for (int i = 0; i < text3Ds.Length; i++)
            StartCoroutine(CO_moveTextAfterDelay(text3Ds[i]));
        if (callBack != null)
            this.callBack = callBack;
    }
    public void Stop()
    {
        for (int i = 0; i < text3Ds.Length; i++)
        {
            text3Ds[i].textAnimatorController.Stop();
        }
    }
    public void PauseSingleText(int index,float autoResumeTime)
    {
        StartCoroutine(CO_ResumeSingleTextAnimation(index,autoResumeTime));
    }
    IEnumerator CO_ResumeSingleTextAnimation(int index,float autoResumeTime)
    {
        yield return Seconds.GetCachedWaitForSeconds(autoResumeTime);
        text3Ds[index].textAnimatorController.ContinueAnimation();
    }
    Action callBack;
    IEnumerator CO_moveTextAfterDelay(Text3D text3D)
    {
        yield return Seconds.GetCachedWaitForSeconds(text3D.delay);
        
        text3D.textGameObject.SetActive(true);
        text3D.textAnimatorController.StartMove();
    }
    public void AnimationsReachedLoopPoint()
    {
        loopsCounter++;
        if (loopsCounter >= minimumLoopsToProceed)
        {
            if ((int)GameManager.Instance.GetCurrentState >= (int)targetState)
            {
                Stop();
                if (callBack != null)
                    callBack.Invoke();
            }
            else if(loop) Play();

        }
        else if (loop) Play();
    }
    public int GetTextsCount => text3Ds.Length;
}
