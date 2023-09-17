using System;
using UnityEngine;
using System.Collections;

public class SlideAnimator : MonoBehaviour
{
    Action currentCallBack;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    public SkinnedMeshRenderer GetSkinnedMeshRenderer => meshRenderer;
    Animator animator;

    #region BendingAnimation

    readonly string bendParameter = "bend";
    readonly string unbendParameter = "unbend";
    Animator bendAnimator;
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
        bendAnimator=transform.GetChild(0).GetComponent<Animator>();
    }
    float bendingDelay=1.5F;
    IEnumerator CO_DelayBending()
    {
        yield return Seconds.GetCachedWaitForSeconds(bendingDelay);
        bendAnimator.SetTrigger(unbendParameter);
    }
    public void Enter(string parameter, Action callBack = null)
    {
        meshRenderer.enabled = true;
        animator.SetTrigger(parameter);
        currentCallBack = callBack;
        StartCoroutine(CO_DelayBending());
    }

    public void OnAnimationEnd()
    {
        currentCallBack?.Invoke();
    }
    public void Exit(string parameter, Action callBack = null)
    {
        animator.SetTrigger(parameter);
        bendAnimator.SetTrigger(bendParameter);
        currentCallBack = callBack;
    }
    private void Hide()
    {
        meshRenderer.enabled = false;
    }
    public void Reset()
    {
        animator.ResetTrigger("Enter");
        animator.ResetTrigger("Exit");
        animator.SetTrigger("Reset");
        Hide();
    }
}
