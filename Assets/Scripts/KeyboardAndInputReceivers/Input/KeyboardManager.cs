using TMPro;
using UnityEngine;

public class KeyboardManager : Singleton<KeyboardManager>
{
    [SerializeField] TextMeshProUGUI currentInputText;
    int currentLetterIndex;
    string[] nextInput;
    IInputReceiver currentInputReceiver;
    public void SetReceiver(IInputReceiver InputReceiver)
    {
        currentInputReceiver = InputReceiver;
        if (currentInputReceiver.TargetCode()=="YN")
            IsYesNoReceiver = true;
        if (currentInputReceiver.TargetCode() == "TopicSelection")
        {
            IsTopicSelectionReceiver = true;
        }
        if (currentInputReceiver.TargetCode()=="150"|| currentInputReceiver.TargetCode() == "200"||currentInputReceiver.TargetCode() == "300")
        {
            IsSizeReceiver = true;
        }
        nextInput = GeneralMethods.GetStringArrayFromTargetCode(currentInputReceiver.TargetCode());
        currentInputText = currentInputReceiver.GetInputText();
        currentInputText.text = currentInputReceiver.GetInputStartText();
        enabled = true;
    }
    bool IsYesNoReceiver;
    bool IsTopicSelectionReceiver;
    bool IsSizeReceiver;
    public string GetHintKey()
    {
        return nextInput[currentLetterIndex];
    }
    public int GetLetterIndex => currentLetterIndex;
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Escape)|| Input.GetKeyDown(KeyCode.Tab))
                return;
            if (IsYesNoReceiver)
            {
                YesNoHandling();
                return;
            }
            else if (IsTopicSelectionReceiver)
            {
                TopicSelectionHandling(Input.inputString.ToLower());
                return;
            }
            else if(IsSizeReceiver)
            {
                SizeHandling(Input.inputString.ToLower());
                return;
            }
            else if (Input.inputString.ToLower()== nextInput[currentLetterIndex])
            {
                TestHandling();
                return;
            }
            else
                currentInputReceiver.WrongEntry();
        }
    }
    private void YesNoHandling()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            currentInputReceiver.CorrectEntry();
            DisconnectInputReceiver();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            currentInputReceiver.WrongEntry();
            DisconnectInputReceiver();
        }
    }
    private void TopicSelectionHandling(string keyPressed)
    {
        string newText = currentInputText.text[0..^1];
        if (newText.Contains('-'))
        {
            newText = newText.Insert(newText.IndexOf('-'),keyPressed);
            currentInputText.text = newText;
            currentLetterIndex++;
        }
        else
        {
            newText += keyPressed;
            currentInputText.text = newText;
            currentInputReceiver.CorrectEntry();
            DisconnectInputReceiver();
        }
    }
    private void SizeHandling(string keyPressed)
    {
        if (!int.TryParse(keyPressed,out int i))
        {
            return;
        }
        string newText = currentInputText.text[0..^1];
        if (newText.Contains('-'))
        {
            newText = newText.Insert(newText.IndexOf('-'), keyPressed);
            currentInputText.text = newText;
            currentLetterIndex++;
            currentInputReceiver.CorrectEntry();
        }
        else
        {
            newText += keyPressed;
            currentInputText.text = newText;
            currentInputReceiver.CorrectEntry();
            DisconnectInputReceiver();
        }
    }
    private void TestHandling()
    {
        string newText = currentInputText.text[0..^1];
        if (newText.Contains('-'))
        {
            newText = newText.Insert(newText.IndexOf('-'), nextInput[currentLetterIndex]);
            currentInputText.text = newText;
            currentLetterIndex++;
            currentInputReceiver.CorrectEntry();
        }
        else
        {
            newText += nextInput[currentLetterIndex];
            currentInputText.text = newText;
            currentInputReceiver.CorrectEntry();
            DisconnectInputReceiver();
        }
    }
    public void DisconnectInputReceiver()
    {
        if (currentInputReceiver != null)
        {
            currentInputReceiver.EntryFinished();
        }
        currentInputReceiver = null;
        IsYesNoReceiver = false;
        IsTopicSelectionReceiver = false;
        IsSizeReceiver = false;
        currentLetterIndex = 0;
        enabled = false;
    }
}
