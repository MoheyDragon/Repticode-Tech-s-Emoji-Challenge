using System;
using System.Collections;
using UnityEngine;
using RTLTMPro;

public class DialogueManager : MonoBehaviour
{
                     protected int currentDialogue;
    [SerializeField] float panelEntryDuration;
    [SerializeField] float showDialoguesDuration;
    [SerializeField] private LocalizedGroupsNames localizedDialogueGroup;
    protected RTLTextMeshPro textMesh;
    private static RTLTextMeshPro rtlPlaceHolder;
    protected CanvasGroup textCanvasGroup;
    private Row currentRow;

    protected virtual void Start()
    {
        currentDialogue = -1;
        SaveDialougeIndex();
        textMesh = transform.GetChild(0).GetComponent<RTLTextMeshPro>();
        textCanvasGroup= transform.GetChild(0).GetComponent<CanvasGroup>();
        rtlPlaceHolder = GameObject.Find("RTLPlaceHodler").GetComponent<RTLTextMeshPro>();
    }
    public void BringDialogueInsideScene(Action callBack=null)
    {
        LeanTween.moveLocalX(gameObject, 0, panelEntryDuration).setEaseInOutSine().setOnComplete(()=>
        {
            currentDialogue = -1;
            if (callBack!=null)
            callBack.Invoke();
        });
    }
    public virtual void ShowNextDialogue(int pendingDialogues=0,Action callBack=null)
    {
        HidePreviousDialogue();
        currentDialogue++;
        currentRow = LocalizationManager.Instance.GetRow(localizedDialogueGroup, currentDialogue);
        GetFontSettings();
        _ChangeTextDialouge();
        FadeInDialouge();
        StartCoroutine(CO_PlayNextDialogueAfterTime(pendingDialogues,callBack));
    }
    public void ShowSpecificText(int textIndex)
    {
        currentDialogue = textIndex-1;
        ShowNextDialogue();
    }
    string _customContent = "customContent";
    public void ReplaceCustomContent(string[] customContent,bool isInTestMode=false)
    {
        _ChangeTextDialouge();
        for (int i = 0; i < customContent.Length; i++)
        {
            string newText;
            if (LanguageManager.Instance.GetCurrentLanguage==LanguageManager.Language.Arabic&&!isInTestMode)
            {
                string replaceingText= rtlPlaceHolder.GetFixedText(customContent[i]);
                newText = textMesh.text.Replace(_customContent + i,replaceingText);
            }
            else
            {
                newText = textMesh.text.Replace(_customContent + i, customContent[i]);
            }
            textMesh.text = newText;
            textMesh.SetText(newText);
        }
        _customContent = "customContent";
    }
    public void ReplaceUserTestGuideText(string oldText, string replacedText)
    {
        string newText;
        newText = textMesh.text.Replace(oldText, replacedText);
        textMesh.text = newText;
    }
    public void AddFacialExpressionInUserTest()
    {
        textMesh.text += "\nfacial expression=\"u+---\"/>";
    }

    const string tutorialText = "here is the tutorial";
    const string arabicTutorialText = "إليك الدرس";
    public void TutorialText()
    {
        if (LanguageManager.Instance.GetCurrentLanguage == LanguageManager.Language.Arabic)
        {
            string replaceingText = rtlPlaceHolder.GetFixedText(arabicTutorialText + "\n");
            replaceingText += textMesh.text;
            textMesh.SetText(replaceingText);
        }
        else
            textMesh.text = tutorialText + "\n" + textMesh.text;
    }
    public void ColorizeText(string targetText, Color targetColor)
    {
        string richText = $"<color=#{ColorUtility.ToHtmlStringRGBA(targetColor)}>{targetText}</color>";
        string colorizedText = textMesh.text.Replace(targetText, richText);
        textMesh.text = colorizedText;
    }
    #region MinorFunctions
    public virtual void HidePreviousDialogue()
    {
        if (currentDialogue >= 0)
            LeanTween.alphaCanvas(textCanvasGroup, 0, 0);
    }
    private void _ChangeTextDialouge()
    {
        string text= currentRow.language[LanguageManager.Instance.GetCurrentLanguageIndex];
        textMesh.text = text.Replace("\"\"", "\"");
    }
    private void GetFontSettings()
    {
        FontSettings fontSettings=LanguageManager.Instance.GetFontSettingsBasedOnLanguageSelected();
        textMesh.font = fontSettings.font;
        if (textMesh.alignment!=TMPro.TextAlignmentOptions.Center)
        {
            textMesh.alignment = fontSettings.alignment;
        }
    }
    public void OverRideFontSize(float fontSize)
    {
        textMesh.fontSize = fontSize;
    }
    public void ControlTextWrapping(bool enabled)
    {
        textMesh.enableWordWrapping = enabled;
    }
    public virtual void FadeInDialouge()
    {
        LeanTween.alphaCanvas(textCanvasGroup, 1, showDialoguesDuration).setEaseInOutSine();
    }
    #endregion
    IEnumerator CO_PlayNextDialogueAfterTime(int pendingDialogue,Action callBack=null)
    {
        yield return Seconds.GetCachedWaitForSeconds(currentRow.duration);
        pendingDialogue--;
        if (pendingDialogue> -1)
            ShowNextDialogue(pendingDialogue,callBack);
        else
            if (callBack!=null) callBack.Invoke();
    }
    #region ForceDialougeIndexChanges
    int savedCurrentDialogue;
    public void RepetedTaskDialouge(bool hasEnteredBefore,string[] customContent)
    {
        if (hasEnteredBefore)
            LoadDialougeIndex();
        else
            SaveDialougeIndex();
        ShowNextDialogue();
        ReplaceCustomContent(customContent);
    }
    public void BreakRepetedDialogue(int repetedDialogueCount)
    {
        currentDialogue = savedCurrentDialogue + repetedDialogueCount;
    }
    public void SaveDialougeIndex() => savedCurrentDialogue = currentDialogue;
    public void LoadDialougeIndex() => currentDialogue = savedCurrentDialogue;
    #endregion
}
