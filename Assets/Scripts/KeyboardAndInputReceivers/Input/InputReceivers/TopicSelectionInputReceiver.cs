using TMPro;
using UnityEngine;

public class TopicSelectionInputReceiver : MonoBehaviour,IInputReceiver
{
    [SerializeField] float fadeDuration = 1;
    [SerializeField] string entry1, entry2;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] CanvasGroup inputPanelCanvasGroup;
    string startText;
    private void Start()
    {
        startText = text.text;
    }
    public void CorrectEntry()
    {
        if (text.text == entry1)
        {
            GameManager.Instance.UserSelectedTopic(0);
            LeanTween.alphaCanvas(inputPanelCanvasGroup, 0, fadeDuration).setEaseInOutSine();
        }
        else if (text.text == entry2)
        {
            GameManager.Instance.UserSelectedTopic(1);
            LeanTween.alphaCanvas(inputPanelCanvasGroup, 0, fadeDuration).setEaseInOutSine();
        }
    }

    public void EntryFinished()
    {
        if (text.text == entry1 || text.text == entry2) return;
        WrongEntry();

    }

    public string GetInputStartText()
    {
        return startText;
    }

    public TextMeshProUGUI GetInputText()
    {
        return text;
    }

    public string TargetCode()
    {
        return "TopicSelection";
    }

    public void WrongEntry()
    {
        BenefitsManager3D.Instance.DisplayWarning();
    }

}
