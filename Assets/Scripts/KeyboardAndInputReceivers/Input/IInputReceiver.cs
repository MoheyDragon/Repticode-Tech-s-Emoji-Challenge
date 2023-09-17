using TMPro;
public interface IInputReceiver
{
    void CorrectEntry();
    void WrongEntry();
    void EntryFinished();
    string TargetCode();
    TextMeshProUGUI GetInputText();
    string GetInputStartText();
}
