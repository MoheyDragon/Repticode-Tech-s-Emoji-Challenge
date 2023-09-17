using UnityEngine;
using UnityEngine.Video;
using System;

public class VideoSlideController : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] float startSize,ViewSize;
                     Vector3 startSizeVector, ViewSizeVector;
    [Space]
    [SerializeField] float entryDuration;
    CanvasGroup canvasGroup;
    Action currentCallBack;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        videoPlayer.loopPointReached += OnVideoFinshes;
        startSizeVector = Vector3.one * startSize;
        ViewSizeVector = Vector3.one * ViewSize;
        LeanTween.scale(gameObject, startSizeVector, 0);
    }
    public void PlayVideo(Action callBack)
    {
        videoPlayer.enabled = true;
        videoPlayer.Play();
        LeanTween.scale(gameObject, ViewSizeVector, entryDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(canvasGroup, 1, entryDuration).setEaseInOutSine();
        currentCallBack = callBack;
    }
    private void OnVideoFinshes(VideoPlayer videoPlayer)
    {
        videoPlayer.Stop();
        videoPlayer.enabled = false;
        LeanTween.scale(gameObject, startSizeVector, entryDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(canvasGroup, 0, entryDuration).setEaseInOutSine().setOnComplete(() => currentCallBack.Invoke());
    }
}

