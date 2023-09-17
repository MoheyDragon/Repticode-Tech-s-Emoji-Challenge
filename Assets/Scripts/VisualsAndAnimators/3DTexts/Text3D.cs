using UnityEngine;
public struct Text3D
{
    public Text3DAnimationController textAnimatorController;
    public GameObject textGameObject;
    public float delay;

    public Text3D(Text3DAnimationController animator, GameObject textGameObject, float delay)
    {
        this.textAnimatorController = animator;
        this.textGameObject = textGameObject;
        this.delay = delay;
        this.textGameObject.SetActive(false);
    }
}