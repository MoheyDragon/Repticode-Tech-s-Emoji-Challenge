using UnityEngine;
using UnityEngine.UI;

public class SlidesContainer:MonoBehaviour
{
    Material material;
    [HideInInspector] public SlideAnimator animator;
    private void Start()
    {
        animator = GetComponent<SlideAnimator>();
        material = animator.GetSkinnedMeshRenderer.materials[1];
    }
    public void SetTextue(Texture texture)
    {
        material.mainTexture = texture;
    }
    public SkinnedMeshRenderer GetPanelSkinnedMeshRenderer()
    {
        return animator.GetSkinnedMeshRenderer;
    }

    public void ResetSlider()
    {
        animator.Reset();
        LeanTween.reset();
    }
}

