using UnityEngine;
using System.Collections;
using System;

public class SlidesController : MonoBehaviour
{
    [SerializeField] SlidesContainer[]slidesContainers;
    [SerializeField] GroupOfSlides[] slides;
    [SerializeField] GroupOfSlides[] arabicSlides;
                     GroupOfSlides[] SlidesGroup;
    [Space]
    [SerializeField] Vector3 [] focusedPostions;
    [SerializeField] Vector3 [] minimizedPostions;
    [Space]
    [SerializeField] float startSize;
    [SerializeField] Vector3 focusedVector;
                     Vector3 startVector;
    [Space]
    [SerializeField] float fadeDuration=1;

    int slidesCount;
    int currentSlideIndex;
    int currentSlideContainerIndex;
    Action currentCallBack;
    readonly string enterAnimatorParameter = "Enter";
    readonly string exitAnimatorParameter = "Exit";

    private void Start()
    {
        LanguageManager.Instance.OnLangaugeSelected += OnLangaugeSelected;
        startVector = Vector3.one * startSize;
        panelsMeshRenderers[0] = slidesContainers[0].GetPanelSkinnedMeshRenderer();
        panelsMeshRenderers[1] = slidesContainers[1].GetPanelSkinnedMeshRenderer();
    }
    public void OnLangaugeSelected()
    {
        switch (LanguageManager.Instance.GetCurrentLanguage)
        {
            case LanguageManager.Language.English:
                SlidesGroup = slides;
                break;
            case LanguageManager.Language.Arabic:
                SlidesGroup = arabicSlides;
                break;
            default:
                break;
        }
    }
    public void StartSlides(Action callBack)
    {
        isStopped = false;
        currentCallBack = callBack;
        slidesCount = SlidesGroup[BenefitsManager3D.Instance.CurrentTopicIndex].Slides.Length;
        currentSlideIndex = -1;
        currentSlideContainerIndex = 0;
        NextSlide();
    }
    public void SetPanelTexture(Texture texture)
    {
        slidesContainers[currentSlideContainerIndex].SetTextue(texture);
    }
    private void PrepareNextSlide()
    {
        if (currentSlideIndex == -1)
        {
            PrepareFirstSlide();
            return;
        }
        AlterCurrentSlide();
        SetPanelTexture(SlidesGroup[BenefitsManager3D.Instance.CurrentTopicIndex].Slides[currentSlideIndex + 1].texture);
        LeanTween.value(gameObject, 0, 1, fadeDuration).setEaseInOutSine().setOnUpdate((float value) => panelsMeshRenderers[currentSlideContainerIndex].material.SetFloat("_alpha", value));
        AlterCurrentSlide();
    }
    private void PrepareFirstSlide()
    {
        panelsMeshRenderers[currentSlideContainerIndex].material.SetFloat("_alpha", 1);
        slidesContainers[0].SetTextue(SlidesGroup[BenefitsManager3D.Instance.CurrentTopicIndex].Slides[0].texture);
    }
    private void AlterCurrentSlide()
    {
        currentSlideContainerIndex = currentSlideContainerIndex == 0 ? 1: 0;
    }
    public void FadeOutCurrentSlide(Action callBack = null)
    {
        int slider = currentSlideContainerIndex;
        if (currentSlideIndex < 0) return;
        LeanTween.moveLocal(slidesContainers[slider].gameObject, minimizedPostions[currentSlideContainerIndex], fadeDuration).setEaseInOutSine();
        LeanTween.scale(slidesContainers[slider].gameObject, startVector, fadeDuration).setEaseInOutSine().setOnComplete(() =>
        {
            callBack += () => LeanTween.value(gameObject, 1, 0, fadeDuration).setEaseInOutSine().setOnUpdate((float value) => panelsMeshRenderers[slider].material.SetFloat("_alpha", value));
            slidesContainers[slider].animator.Exit(exitAnimatorParameter, callBack);
        }
        );
    }
    private void EnterNextSlide()
    {
        if (currentSlideIndex>0)
            AlterCurrentSlide();
        slidesContainers[currentSlideContainerIndex].animator.Enter(enterAnimatorParameter,ScaleUpSlide);
        float startingAlpha= panelsMeshRenderers[currentSlideContainerIndex].material.GetFloat("_alpha");
        LeanTween.value(gameObject, startingAlpha, 1, fadeDuration).setEaseInOutSine().setOnUpdate((float value) => panelsMeshRenderers[currentSlideContainerIndex].material.SetFloat("_alpha", value));
    }
    private void ScaleUpSlide()
    {
        LeanTween.moveLocal(slidesContainers[currentSlideContainerIndex].gameObject, focusedPostions[currentSlideContainerIndex], fadeDuration).setEaseInOutSine();
        LeanTween.scale(slidesContainers[currentSlideContainerIndex].gameObject, focusedVector, fadeDuration).setEaseInOutSine().setOnComplete(() =>
        {
            StartCoroutine(CO_WaitForSlideShowDuration(() =>
            {
                HandleSingleSlideFinshes();
            }));
        });
    }
    public void NextSlide()
    {
        if (isStopped) return;
        PrepareNextSlide();
        FadeOutCurrentSlide();
        currentSlideIndex++;
        EnterNextSlide();
    }
    IEnumerator CO_WaitForSlideShowDuration(Action callBack)
    {
        yield return Seconds.GetCachedWaitForSeconds(SlidesGroup[BenefitsManager3D.Instance.CurrentTopicIndex].Slides[currentSlideIndex].duration);
        callBack?.Invoke();
    }
    private void HandleSingleSlideFinshes()
    {
        
        if (currentSlideIndex + 1 == slidesCount)
            HandleAllSlidesFinished();
        else
            NextSlide();
    }
    SkinnedMeshRenderer []panelsMeshRenderers=new SkinnedMeshRenderer[2];
    [SerializeField] float shadowDisablingDelay;
    private void FadeOutLastPanel(int panelIndex)
    {
        LeanTween.value(gameObject, 1, 0, fadeDuration).setEaseInOutSine().setOnUpdate((float value) => panelsMeshRenderers[panelIndex].material.SetFloat("_alpha", value));
    }
    public void HandleAllSlidesFinished()
    {
        if(BenefitsManager3D.Instance.CurrentTopicIndex==0)
        {
            FadeOutCurrentSlide(currentCallBack);
            FadeOutLastPanel(currentSlideContainerIndex);
        }
        else
        {
            currentCallBack.Invoke();
        }
    }
    public void OnVideoFinish(Action callBack)
    {
        FadeOutCurrentSlide(callBack);
        FadeOutLastPanel(currentSlideContainerIndex);
    }
    private void ResetSlide(int currentSlideContainerIndex)
    {
        LeanTween.moveLocal(slidesContainers[currentSlideContainerIndex].gameObject, minimizedPostions[currentSlideContainerIndex], 1);
        LeanTween.scale(slidesContainers[currentSlideContainerIndex].gameObject, startVector, 1);
        panelsMeshRenderers[currentSlideContainerIndex].material.SetFloat("_alpha", 1);
    }
    bool isStopped;
    public void Reset()
    {
        isStopped = true;
        LeanTween.reset();
        for (int i = 0; i < 2; i++)
        {
            slidesContainers[i].animator.Reset();
            ResetSlide(i);
        }
    }
}

