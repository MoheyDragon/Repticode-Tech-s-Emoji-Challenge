using UnityEngine;
using System;

public class TeacherController : MonoBehaviour
{
    Animator animator;
    readonly string isTalkingParameter = "IsTalking";
    readonly string walkParameter = "Walk";
    readonly string waveParameter = "Wave";
    [SerializeField] float wavingFrequency =20;
    private void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        Invoke(nameof(Wave), wavingFrequency);
    }
    private void Wave()
    {
        if (GameManager.Instance.GetCurrentState==GameStates.State.Idle)
        {
            animator.SetTrigger(waveParameter);
            Invoke(nameof(Wave), wavingFrequency);
        }
    }
    public void StartTalking()
    {
        animator.SetBool(isTalkingParameter, true);
        LeanTween.moveLocal(teacherCamera, tutorialCameraPosition, swipingDuration).setEaseInOutSine();
    }
    public void StopTalking()
    {
        animator.SetBool(isTalkingParameter, false);
        LeanTween.moveLocal(teacherCamera, emojisCameraPosition, swipingDuration).setEaseInOutSine();
    }
    [SerializeField] Vector3 emojisDisplaySwipingDestination;
    [SerializeField] Vector3 emojisDisplaySwipingRotation;
    [Space]
    [SerializeField] Vector3 lastTextSwipingDestination;
    [SerializeField] Vector3 lastTextRotation;

    [SerializeField] float swipingDuration;
    [Space]
    [Header("Shadow")]
    [SerializeField] Light teacherLight;
    [SerializeField] float midPositionShadowStrength, rightPositionShadowStrength;
    public void DisplayEmojis(Action callBack)
    {
        ToggleWalkAnimation();
        callBack += ToggleWalkAnimation;
        LeanTween.value(gameObject, midPositionShadowStrength, rightPositionShadowStrength, swipingDuration).setEaseInOutSine().setOnUpdate((float value) => teacherLight.shadowStrength=value);
        LeanTween.rotate(gameObject, emojisDisplaySwipingRotation, swipingDuration).setEaseInOutSine();
        LeanTween.moveLocal(teacherCamera, emojisCameraPosition, swipingDuration).setEaseInOutSine();
        LeanTween.moveLocal(gameObject, emojisDisplaySwipingDestination, swipingDuration).setEaseInOutSine()
            .setOnComplete(callBack); 
    }
    [Space]
    [SerializeField] GameObject teacherCamera;
    [SerializeField] Vector3 emojisCameraPosition = new Vector3(0.119999997f, 2.25999999f, 1.19000006f);
    [SerializeField] Vector3 tutorialCameraPosition = new Vector3(0.0320000015f, 2.28999996f, 0.646000028f);
    public void DisplayLastTexts()
    {
        LeanTween.rotate(gameObject, lastTextRotation, swipingDuration).setEaseInOutSine();
        LeanTween.moveLocal(gameObject, lastTextSwipingDestination, swipingDuration).setEaseInOutSine();
    }
    private void ToggleWalkAnimation()
    {
        animator.SetTrigger(walkParameter);
    }
}
