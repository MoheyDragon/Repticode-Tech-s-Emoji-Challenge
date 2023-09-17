using TMPro;
using UnityEngine;

public class YesNoReceiver : MonoBehaviour,IInputReceiver
{
    [SerializeField] TextMeshProUGUI empty;
    public void CorrectEntry()
    {
        GameManager.Instance.UserChoiceToRetry(true);
        HintsKeyboard.Instance.StopHighlightingButtons(new string[] { "y", "n" });
    }

    public void EntryFinished()
    {
    }

    public string GetInputStartText()
    {
        return "";
    }

    public TextMeshProUGUI GetInputText()
    {
        return empty;
    }

    public string TargetCode()
    {
        return "YN";
    }

    public void WrongEntry()
    {
        GameManager.Instance.UserChoiceToRetry(false);
        HintsKeyboard.Instance.StopHighlightingButtons(new string[] { "y", "n" });
    }

}
