using UnityEngine;
using UnityEngine.UI;
using System;

public class PerformanceBar:MonoBehaviour
{
    Image barImage;
    CanvasGroup canvasGroup;
    BarNameFading barName;
    private void Awake()
    {
        barImage = GetComponent<Image>();
        barName = transform.GetChild(0).GetComponent<BarNameFading>();
        canvasGroup = GetComponent<CanvasGroup>();
        transform.localScale = new Vector3(0, 1, 1);
    }
    public void ShowBar(float duration, float yMoveAmount, Color color, float scaleAmount,Action callBack=null)
    {
        barImage.color = color;
        LeanTween.alphaCanvas(canvasGroup, 1, duration/2).setEaseInOutSine();
        LeanTween.moveLocalY(gameObject, yMoveAmount, 0).setEaseInOutSine();
        LeanTween.scaleX(barName.gameObject, 1 / scaleAmount, duration).setEaseInOutSine();
        LeanTween.scaleX(gameObject,scaleAmount, duration).setEaseInOutSine().setOnComplete(()=>
        {
            barName.enabled = true;
            callBack?.Invoke();
        });
        hasShownBar = true;
    }
    bool hasShownBar;
    public void HideBar(float duration)
    {
        if (!hasShownBar) return;
        LeanTween.alphaCanvas(canvasGroup, 0, duration/5).setEaseInOutSine();
        barName.enabled = true;
        barName.FadeOut();
    }
    public void SetBarNameText(string text)
    {
        barName.SetText(text);
    }
    
}
