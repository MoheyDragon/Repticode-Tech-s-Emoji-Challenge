using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public partial class ShelvesAnimator : MonoBehaviour
{
    private Animator shelvesAnimator;
    private Action currentCallBack;
    public Dictionary<ShelvesAnimatorParameters,string> animatorParametes;

    #region AnimatorParameters
    readonly string enterParameter = "enter";
    readonly string performanceParameter= "performance";
    readonly string midRotateParameter = "midRotate";
    readonly string backParameter = "back";
    #endregion
    private void Awake()
    {
        shelvesAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        animatorParametes = new Dictionary<ShelvesAnimatorParameters, string>
        { {ShelvesAnimatorParameters.enter,enterParameter },
          {ShelvesAnimatorParameters.midRotate,midRotateParameter },
          {ShelvesAnimatorParameters.back,backParameter},
          {ShelvesAnimatorParameters.performance,performanceParameter }
        };
    }
    public void PlayAnimation(ShelvesAnimatorParameters parameter,Action callBack=null)
    {
        shelvesAnimator.SetTrigger(animatorParametes[parameter]);
        currentCallBack=callBack;
    }
    public void OnAnimationEnd()
    {
        currentCallBack?.Invoke();
        currentCallBack = null;
    }
}
