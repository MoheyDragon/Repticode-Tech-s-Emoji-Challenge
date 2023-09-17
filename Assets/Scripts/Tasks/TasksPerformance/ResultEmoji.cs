using UnityEngine;
using UnityEngine.UI;
using System;
public class ResultEmoji : EmojiObject
{
    [SerializeField] CanvasGroup performanceBarsCanvasGroup;
    [SerializeField] PerformanceBar speedBar;
    [SerializeField] PerformanceBar logicBar;
                     PivotTracker pivotTracker;
    [SerializeField] Vector2 tiling, offset;
    private void Start()
    {
        pivotTracker = GetComponent<PivotTracker>();
        pivotTracker.enabled = false;
        performanceBarsCanvasGroup.alpha = 0;
    }
    float barYpositionCorrection;
    const float yStartSize = 0.0044f;
    public void ShowResult(int index,float duration, string texturePropertyName, string fadePropertyName,string tillingPropertyName,string offsetPropertyName,Vector3 displayPosition, Action callBack = null)
    {
        pivotTracker.enabled = true;
        Vector3 targetSize = UserTestWebSimulatorManager.Instance.GetSizeByTaskIndex(index);
        barYpositionCorrection = yStartSize * (targetSize.x-1) * 0.5f+yStartSize;
        performanceBarsCanvasGroup.gameObject.SetActive(true);
        LeanTween.moveLocal(gameObject,displayPosition, duration);
        LeanTween.scale(performanceBarsCanvasGroup.gameObject, GeneralMethods.GetInverseVector(targetSize), duration).setEaseInOutSine();
        LeanTween.scale(gameObject, targetSize, duration).setEaseInOutSine().setOnComplete(() =>
            {
                meshRenderer.material.SetTexture(texturePropertyName, UserTestWebSimulatorManager.Instance.GetEmojiTextureByTaskIndex(index));
                meshRenderer.material.SetVector(tillingPropertyName, tiling);
                meshRenderer.material.SetVector(offsetPropertyName, offset);
                meshRenderer.material.SetFloat(fadePropertyName, 0);
                callBack?.Invoke();
            });
    }
    public void FadeOut(string fadePropertyName, float duration)
    {
        FadeOutEmojiFace(fadePropertyName, duration);
        LeanTween.moveLocal(gameObject, Vector3.zero, duration);
        pivotTracker.enabled = false;
        speedBar.HideBar(duration);
        logicBar.HideBar(duration);
        LeanTween.scale(gameObject, Vector3.one, duration);
    }
    public void ShowPerfromanceBars(int index,float duration, Color color,Action callBack)
    {
        if (name== "Emoji (2)")
            barYpositionCorrection = 0.00619f;
        TaskPerformanceRecorder taskPerformanceRecorder = PerformanceManager.Instance.tasksPerformanceRecorders[index];
            speedBar.ShowBar(duration,barYpositionCorrection, color, taskPerformanceRecorder.speed, () =>
            {
                if (name == "Emoji (2)")
                    barYpositionCorrection = 0.00246f;
                logicBar.ShowBar(duration,-barYpositionCorrection, color, taskPerformanceRecorder.logic, callBack);
            });
            LeanTween.alphaCanvas(performanceBarsCanvasGroup, 1, duration * 2).setEaseInOutSine();
    }
    public void SetBarsNamesTexts()
    {
            string logicText, speedText;
            logicText = "المنطق";
            speedText = "السرعة";
            speedBar.SetBarNameText(speedText);
            logicBar.SetBarNameText(logicText);
    }
}
