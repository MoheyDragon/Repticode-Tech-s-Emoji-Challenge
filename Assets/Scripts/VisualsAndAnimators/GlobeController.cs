using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeController : SceneObject
{
    [SerializeField] float Xdestination = 16.57f;
    [SerializeField] float Ydestination=0;
    [SerializeField] float entryDuration=1.5f;
    [SerializeField] float departureDuration = 2.5f;
    [SerializeField] float sizeOnEntry = 0.5f;
    [SerializeField] float sizeAfterEntry=1.5f;
    private void Start()
    {
        LeanTween.scale(gameObject, Vector3.one*sizeOnEntry, entryDuration);
        _canvasGroup = GameObject.Find("GlobeCanvas").GetComponent<CanvasGroup>();
    }
    public void EnterScene()
    {
        LeanTween.alphaCanvas(canvasGroup, 1, entryDuration/2).setEaseInOutSine();
        LeanTween.moveX(gameObject, Xdestination, entryDuration).setEaseInOutSine();
        LeanTween.moveY(gameObject, Ydestination, entryDuration).setEaseInOutSine();
        LeanTween.scale(gameObject, Vector3.one*sizeAfterEntry,entryDuration).setEaseInOutSine();
    }
    public void LeaveScene()
    {
        LeanTween.alphaCanvas(canvasGroup, 0, departureDuration).setEaseInOutSine();
    }
}
