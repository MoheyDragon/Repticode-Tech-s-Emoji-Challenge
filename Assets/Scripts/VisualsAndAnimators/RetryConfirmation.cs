using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using System;

public class RetryConfirmation :MonoBehaviour
{
    [SerializeField] float fadeDuration;
    readonly float displayPanelX = 0;
    readonly float hidePanelX = 2500;

    Image backgroundImage;
    [SerializeField] CanvasGroup background;
    [SerializeField] GameObject panel;
    [SerializeField] ActivationEscButton[] buttonsFunctions;
    [SerializeField] GameObject []arabicTexts;
    [SerializeField] GameObject []englishTexts;

    IRetryReceiver currentRetryReceiver;
    private void Start()
    {
        backgroundImage = background.GetComponent<Image>();
        for (int i = 0; i < 1; i++)
        {
            arabicTexts[i].SetActive(false);
            englishTexts[i].SetActive(false);
        }
        LanguageManager.Instance.OnLangaugeSelected += OnLangaugeSelected;
    }
    public void OnLangaugeSelected()
    {
        if (LanguageManager.Instance.GetCurrentLanguage==LanguageManager.Language.English)
        {
            foreach (GameObject text in englishTexts)
                text.SetActive(true);
        }
        else
            foreach (GameObject text in arabicTexts)
                text.SetActive(true);

    }
    [SerializeField] RTLTextMeshPro inputPlaceHolder; 
    public void EnterPanel(IRetryReceiver retryReceiver,bool haveBG)
    {
        currentRetryReceiver = retryReceiver;
        backgroundImage.enabled= haveBG;
        inputPlaceHolder.text = "-";
        LeanTween.moveLocalX(panel, displayPanelX, fadeDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(background, 1, fadeDuration).setEaseInOutSine().setOnComplete(EnableButtons);
    }
    public void EnableButtons()
    {
        Invoke(nameof(EnableButtonsAfterDelay),fadeDuration);
    }
    private void EnableButtonsAfterDelay()
    {
        KeyboardManager.Instance.enabled = false;
        for (int i = 0; i < 2; i++)
        {
            buttonsFunctions[i].enabled = true;
        }
    }
    public void DisableButtons()
    {
        for (int i = 0; i < 2; i++)
        {
            buttonsFunctions[i].enabled = false;
        }
    }
    public void ExitPanel(Action callBack)
    {
        DisableButtons();
        LeanTween.moveLocalX(panel, hidePanelX, fadeDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(background, 0, fadeDuration).setEaseInOutSine().setOnComplete(callBack);
    }
    public void RetryConformed()
    {
        ExitPanel(currentRetryReceiver.RetryConformed);
    }
    public void RetryDenied()
    {
        ExitPanel(currentRetryReceiver.RetryDenied);
    }
}
