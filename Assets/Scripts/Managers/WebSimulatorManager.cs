using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using RTLTMPro;
public class WebSimulatorManager : DialogueManager
{
    CanvasGroup webSimulatorCG;

    [Space]

    [SerializeField] CanvasGroup htmlBoxCG;
    [SerializeField] CanvasGroup[] htmlBoxSequenceItems;
    int currentHtmlStepItemIndex;
    
    [Space]
    [SerializeField] float fadeInDuration;
    const float fadeOutDuration = 0.5f;

    protected override void Start()
    {
        base.Start();
        currentHtmlStepItemIndex = -1;
        htmlBoxImage = htmlBoxCG.GetComponent<Image>();
        webSimulatorCG = GetComponent<CanvasGroup>();
    }
    public virtual void ShowCanvas()
    {
        LeanTween.alphaCanvas(webSimulatorCG, 1, fadeInDuration);
    }
    Image htmlBoxImage;
    float highlightDuration=0.75f;
    int cycleCount = 2;
    public void ShowHtmlBox()
    {
        LeanTween.alphaCanvas(htmlBoxCG, 1, fadeInDuration);
        StartCoroutine(HighlightHtmlBox());
    }
    IEnumerator HighlightHtmlBox()
    {
        for (int cycle = 0; cycle < cycleCount; cycle++)
        {
            yield return StartCoroutine(TransitionColor(Color.white, ProjectCustomColors.Instance.Green));
            yield return StartCoroutine(TransitionColor(ProjectCustomColors.Instance.Green, Color.white));
        }
    }
    IEnumerator TransitionColor(Color startColor, Color endColor)
    {
        float elapsedTime = 0;

        while (htmlBoxImage.color != endColor)
        {
            htmlBoxImage.color = Color.Lerp(startColor, endColor, elapsedTime / highlightDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void NextHtmlSequenceItem()
    {
        currentHtmlStepItemIndex++;
        LeanTween.alphaCanvas(htmlBoxSequenceItems[currentHtmlStepItemIndex], 1, fadeInDuration);
    }
    public void FadeOutHtmlBox()
    {
        LeanTween.alphaCanvas(htmlBoxCG, 0, fadeOutDuration);
    }
    public void FadeOutWebSimulator(Action callBack)
    {
        LeanTween.alphaCanvas(webSimulatorCG, 0, fadeOutDuration).setOnComplete(callBack);
    }
    readonly float largeEmojiSize=2;
    readonly float secondEmojiSize=1.3f;
    readonly float resizeDuration=3;
    public float ResizeEmoji()
    {
        Vector3 emojiScale = htmlBoxSequenceItems[0].transform.lossyScale;
        textMesh.text=textMesh.text.Replace("\"--\"","\"50\"");
        previousNumber = "50";
        ColorizeText("50", ProjectCustomColors.Instance.Green);
        LeanTween.value(gameObject, 50, 199, resizeDuration).setEaseInOutSine().setOnUpdate((float value) => ReplaceTutorialSize(value,true));
        LeanTween.scale(htmlBoxSequenceItems[0].gameObject, emojiScale*largeEmojiSize, resizeDuration).setEaseInOutSine().setOnComplete(() =>
        {
              LeanTween.value(gameObject, 199, 100, resizeDuration).setEaseInOutSine().setOnUpdate((float value) => ReplaceTutorialSize(value,false));
              LeanTween.scale(htmlBoxSequenceItems[0].gameObject, emojiScale*secondEmojiSize, resizeDuration).setEaseInOutSine();
        });
        return resizeDuration*2;
    }
    string previousNumber;
    private void ReplaceTutorialSize(float emojiSize, bool isIncreasing)
    {
        int newNumber = (int)Math.Round(emojiSize);
        if (isIncreasing)
        {
            if (newNumber == 20)
                newNumber = 21;
        }
        else
        {
            if (newNumber == 20)
                newNumber = 19;
            else if (newNumber == 3|| newNumber == 2)
                newNumber = 1;
        }
        string newNumberString = newNumber.ToString();
        textMesh.text = textMesh.text.Replace(previousNumber, newNumberString);
        previousNumber = newNumberString;
    }
    public void ReplaceTutorialColor()
    {
        textMesh.text = textMesh.text.Replace("---","yellow" );
    }
    [SerializeField] RTLTextMeshPro facialExpressionTextMesh;
    public void TypeInFacialExpression()
    {
        LeanTween.alphaCanvas(facialExpressionTextMesh.GetComponent<CanvasGroup>(), 1, 1).setOnComplete(()=>
        {
            StartCoroutine(CO_TypeInFacialExpression());
        });
    }
    float typeSpeed = 0.5f;
    IEnumerator CO_TypeInFacialExpression()
    {
        WaitForSeconds typeSpeedWait = Seconds.GetCachedWaitForSeconds(typeSpeed);
        int startIndex = 0;
        string[] facialExpressionChars = { "U", "+", "1", "F", "6", "0", "0"};
        for (int i = 0; i <7; i++)
        {
            yield return typeSpeedWait;
            facialExpressionTextMesh.text = facialExpressionTextMesh.text.Remove(startIndex + i, 1);
            facialExpressionTextMesh.text = facialExpressionTextMesh.text.Insert(startIndex + i, facialExpressionChars[i]);
            EmojiFacePartFillGradually(i);
        }
        facialExpressionTextMesh.color = Color.white;
    }

    [SerializeField] Image leftEye, rightEye, mouth;
    public void EmojiFacePartFillGradually(int partIndex)
    {
        float fillDuration=1;
        switch (partIndex)
        {
            case 2:
                LeanTween.value(gameObject, 0, 1, fillDuration).setOnUpdate((float value) => leftEye.fillAmount = value).setEaseInOutSine();
                break;
            case 4:
                LeanTween.value(gameObject, 0, 1, fillDuration).setOnUpdate((float value) => rightEye.fillAmount = value).setEaseInOutSine();
                break;
            case 6:
                LeanTween.value(gameObject, 0, 1, fillDuration).setOnUpdate((float value) => mouth.fillAmount = value).setEaseInOutSine();
                break;
            default:
                break;
        }
    }
}
