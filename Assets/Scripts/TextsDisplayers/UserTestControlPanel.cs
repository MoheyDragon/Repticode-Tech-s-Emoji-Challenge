using System;
using UnityEngine;

public class UserTestControlPanel : MonoBehaviour
{
    DialogueManager dialogueManager;
    CanvasGroup canvasGroup;
    [SerializeField] ActivationEscButton[] ActivationButtons;
    [Space]
    [Header("Texts Rect Transforms")]
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform[] fixedRectTransforms;
    [SerializeField] float[] fontsSizes;
    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void BringInCTA(float fadeDuration)
    {
        AssignRectTransform(0);
        LeanTween.alphaCanvas(canvasGroup, 1, fadeDuration).setEaseInOutSine();
        dialogueManager.BringDialogueInsideScene(()=>
        {
            dialogueManager.ShowNextDialogue(0,()=>
            {
                EnableButtons(0, true);
            }
          );
        });
    }
    public void OpenHtmlBox()
    {
        AssignRectTransform(1);
        EnableButtons(0, false);
    }
    public void OnTaskFinish()
    {
        dialogueManager.ShowSpecificText(5);
        AssignRectTransform(2);
        EnableButtons(1, true);
        EnableButtons(2, true);
    }
    public void ReplaceRadiusText(string[] radius)
    {
        if (LanguageManager.Instance.GetCurrentLanguage==LanguageManager.Language.Arabic)
        {
            for (int i = 0; i < 2; i++)
            {
                radius[i] = ReverseText(radius[i]);
            }
            string ReverseText(string input)
            {
                char[] charArray = input.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }
        }
        dialogueManager.ReplaceCustomContent(radius,true);
    }
    public void TaskFinishCTA(bool exit)
    {
        EnableButtons(1, false);
        EnableButtons(2, false);
        UserTestWebSimulatorManager.Instance.TaskWindowFade(exit);
    }
    private void EnableButtons(int buttonIndex,bool enabled)
    {
        ActivationButtons[buttonIndex].enabled=enabled;
    }
    public void FadeOut(float fadeDuration)
    {
        LeanTween.alphaCanvas(canvasGroup, 0, fadeDuration).setEaseInOutSine();
        dialogueManager.HidePreviousDialogue();
    }
    public void HidePreviousDialogue()
    {
        dialogueManager.HidePreviousDialogue();
    }
    public void ShowNextDialogue()
    {
        dialogueManager.ShowNextDialogue();
    }
    public void LoadDialougeIndex()
    {
        dialogueManager.LoadDialougeIndex();
    }
    public void AssignRectTransform(int index)
    {
        dialogueManager.OverRideFontSize(fontsSizes[index]);
        RectTransform target = fixedRectTransforms[index];
        rectTransform.anchoredPosition = target.anchoredPosition;
        rectTransform.sizeDelta = target.sizeDelta;
        rectTransform.anchorMin = target.anchorMin;
        rectTransform.anchorMax = target.anchorMax;
        rectTransform.pivot = target.pivot;
        rectTransform.offsetMin = target.offsetMin;
        rectTransform.offsetMax = target.offsetMax;
        rectTransform.rotation = target.rotation;
    }
}
