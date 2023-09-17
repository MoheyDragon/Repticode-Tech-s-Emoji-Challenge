using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [SerializeField] Animator cameraAnimator;
    [SerializeField] Camera emojisCamera;
    string zoomIn = "ZoomIn",zoomOut="ZoomOut";
    public void ZoomIn()
    {
        cameraAnimator.SetTrigger(zoomIn);
    }
    public void ZoomOut()
    {
        cameraAnimator.SetTrigger(zoomOut);
    }
    public void DisableEmojisOverlayCamera()
    {
        emojisCamera.enabled = false;
    }
}
