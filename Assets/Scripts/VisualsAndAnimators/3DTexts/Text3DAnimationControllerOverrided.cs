using UnityEngine;
using System.Collections;

public class Text3DAnimationControllerOverrided : Text3DAnimationController
{
    [Space]
    [SerializeField] float midY;
    [SerializeField] float endY;
    Vector3 midRotate, endRotate;
    protected override void Start()
    {
        base.Start();
        midRotate = new Vector3(0, midY, 0);
        endRotate = new Vector3(0, endY, 0);
    }
    public override void StartMove()
    {
        // if stop time is larger than zero, the text has to stop in the middle, so the only difference here is ease method,
        // if there is a stop in the middle it is easeInOut else it's only ease in
        if (stopTime <= 0)
            LeanTween.rotate(gameObject, midRotate, textGroup.MidRotateDuration).setEaseInSine().setOnComplete(OnMidRotationFinished);
        else
            LeanTween.rotate(gameObject, midRotate, textGroup.MidRotateDuration).setEaseInOutSine().setOnComplete(OnMidRotationFinished);
    }
    public override void OnMidRotationFinished()
    {
        // Same as Start move, if there is no stop time only ease out else ease both ways after waiting for stop time
        if (stopTime <= 0)
            LeanTween.rotate(gameObject, endRotate, textGroup.MidRotateDuration).setEaseOutSine().
            setOnComplete(OnAnimationEnd);
        else
            textGroup.PauseSingleText(transform.GetSiblingIndex(), stopTime);
    }
    public override void ContinueAnimation()
    {
        LeanTween.rotate(gameObject, endRotate, textGroup.MidRotateDuration).setEaseInOutSine().
            setOnComplete(OnAnimationEnd);
    }
}