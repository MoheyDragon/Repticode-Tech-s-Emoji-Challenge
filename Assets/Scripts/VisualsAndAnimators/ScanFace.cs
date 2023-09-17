using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanFace : SceneObject
{
    string scanParameter = "Scan",
           fadeOutParameter = "FadeOut",
           resetParameter = "Reset";
    public void Scan()
    {
        animator.SetTrigger(scanParameter);
    }
    public void FadeOut()
    {
        animator.SetTrigger(fadeOutParameter);
    }
    public void ResetAnimator()
    {
        animator.SetTrigger(resetParameter);
    }
}
