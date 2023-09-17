using UnityEngine;
using System.Collections.Generic;
using System;
public class HintsKeyboard:Singleton<HintsKeyboard>
{
    CanvasGroup canvasGroup;
    [SerializeField] float highlightSpeed;
    [SerializeField] float fadeDuration;
    public float GetHighlightSpeed => highlightSpeed;

    Dictionary<string,KeyboardButton> keyboardButtons;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        keyboardButtons = new Dictionary<string, KeyboardButton>();

        Transform buttonsParent = transform.GetChild(1);
        foreach (Transform child in buttonsParent)
            keyboardButtons.Add(child.name.ToLower(), child.GetComponent<KeyboardButton>());
    }
    public void ShowYesNo()
    {
        string[] buttonsToHighlight = { "y", "n" };
        HighlightButtons(buttonsToHighlight);
    }
    public void HighlightButtons(string[] buttonsToHighlight,Action callBack=null)
    {
        ShowKeyboard(buttonsToHighlight,callBack);
    }
    public void StopHighlightingAllButtons()
    {
        foreach (var item in keyboardButtons)
        {
            item.Value.StopHighlighting();
        }
    }
    public void StopHighlightingButtons(string[] buttonsToHighlight)
    {
        foreach (string button in buttonsToHighlight)
            keyboardButtons[button].StopHighlighting();
    }
    private void ShowKeyboard(string[] buttonsToHighlight,Action callBack=null)
    {
        foreach (KeyboardButton button in keyboardButtons.Values)
            button.StopHighlighting();
        LeanTween.alphaCanvas(canvasGroup, 1, fadeDuration).setEaseInOutSine().setOnComplete(() =>
                {
            foreach (string letter in buttonsToHighlight)
                keyboardButtons[letter].HilightButton();
                    callBack?.Invoke();
        });
    }
    public void FadeInKeyboard()
    {
        LeanTween.alphaCanvas(canvasGroup, 1, fadeDuration).setEaseInOutSine();
    }
    public void FadeOutKeyboard(float duration=-1)
    {
        if (duration == -1) duration = fadeDuration;
        LeanTween.alphaCanvas(canvasGroup, 0, duration).setEaseInOutSine();
    }
}
