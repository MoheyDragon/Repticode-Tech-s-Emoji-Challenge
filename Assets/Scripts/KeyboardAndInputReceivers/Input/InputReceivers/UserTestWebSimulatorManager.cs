using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
public class UserTestWebSimulatorManager : Singleton<UserTestWebSimulatorManager>, IInputReceiver,IRetryReceiver
{
    [SerializeField] Task[] tasks;
    [SerializeField] float showTime;
    [Space]
    [Header("Canvas Groups")]
    [Space]
    [SerializeField] CanvasGroup mainCanvasGroup;
    [SerializeField] CanvasGroup frame, timerCircleCG, emojiCG, guideTextCG, targetCodeTextCG, wrongEntryWarning;
                     CanvasGroup inputTextCG;
    [Space]
    [SerializeField] TextMeshProUGUI inputText;
    TextMeshProUGUI targetCodeText;
    Image timerCircle;
    [Space]
    [SerializeField] UserTestControlPanel controlPanel;
    [SerializeField] DialogueManager userTestMiddleGuideText;
    [SerializeField] DialogueManager upperPanelDilogue;
    [SerializeField] Image inputPanelImage;
    bool[] tasksDone;
    void Start()
    {
        _AssignReferences();
        currentTaskIndex = -1;
    }
    void _AssignReferences()
    {
        LanguageManager.Instance.OnLangaugeSelected += OnLangaugeSelected;
        face = emojiCG.transform.GetChild(0).GetComponent<Image>();
        tasksRetried = new bool[3];
        tasksDone = new bool[3];
        targetCodeText = targetCodeTextCG.GetComponent<TextMeshProUGUI>();
        taskRestartCG = taskRestarter.GetComponent<CanvasGroup>();
        timerCircle = timerCircleCG.transform.GetChild(0).GetComponent<Image>();
        inputTextCG = inputText.GetComponent<CanvasGroup>();
    }

    private void OnLangaugeSelected()
    {
        if (LanguageManager.Instance.GetCurrentLanguage == LanguageManager.Language.Arabic)
            wrongEntryWarning.transform.GetChild(1).gameObject.SetActive(true);
        else
            wrongEntryWarning.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void RestartAllTasks()
    {
        currentTaskIndex = -1;
    }
    public void BringInControlPanel()
    {
        ResetValues();
        PerformanceManager.Instance.ResetTask(currentTaskIndex);
        controlPanel.BringInCTA(fadeDuration);
    }
    public void StartTest()
    {
        controlPanel.HidePreviousDialogue();
        FadeInWindowElements();
    }
    public TaskDescription GetTaskDescrption()
    {
        currentTaskIndex++;
        return tasks[currentTaskIndex].taskDescription;
    }
    private void FadeInWindowElements()
    {
        LeanTween.alphaCanvas(mainCanvasGroup, 1, showTime).setEaseInOutSine().setOnComplete(() =>
        {
            LeanTween.alphaCanvas(timerCircleCG, 1, showTime).setEaseInOutSine().setOnComplete(() =>
            {
                userTestMiddleGuideText.ShowNextDialogue();
                StartTimer();
                StartSubTest();
            });
        });
    }
    readonly float keepCorrectTextOnScreenDuration = 1;
    IEnumerator CO_StartSubTask()
    {
        yield return Seconds.GetCachedWaitForSeconds(keepCorrectTextOnScreenDuration);
        StartSubTest();
    }
    public void StartSubTest()
    {
        currentSubTaskIndex++;
        SetTargetText(currentTaskIndex);
        StartCoroutine(CO_SetKeyboardReceiverAfterDelay());
        inputText.text = GetStartTextBySubTaskIndex(currentSubTaskIndex);
        LeanTween.alphaCanvas(inputTextCG, 1, fadeDuration).setEaseInOutSine();
        isTimerActive = true;
        _NextDialogues();
    }
    IEnumerator CO_SetKeyboardReceiverAfterDelay()
    {
        yield return Seconds.GetCachedWaitForSeconds(0.1f);
        KeyboardManager.Instance.SetReceiver(this);
        ShowHint();
    }
    private string GetStartTextBySubTaskIndex(int subTaskIndex)
    {
        int numberOfLetters;
        string startText = "";
        switch (subTaskIndex)
        {
            case 0:
                startText = "---";
                break;
            case 1:
                numberOfLetters = tasks[currentTaskIndex].sizeInCode.Length;
                for (int i = 0; i < numberOfLetters; i++)
                {
                    startText += "-";
                }
                break;
            case 2:
                numberOfLetters = tasks[currentTaskIndex].taskDescription.color[0].Length;
                for (int i = 0; i < numberOfLetters; i++)
                {
                    startText += "-";
                }
                break;
            case 3:
                startText = "u+-----";
                break;
            default:
                return startText;
        }
        currentStartText = startText;
        return startText;
    }
    public Texture GetEmojiTextureByTaskIndex(int index)
    {
        return tasks[index].emoji;
    }
    private void SetTargetText(int taskIndex)
    {
        switch (currentSubTaskIndex)
        {
            case 0:
                targetCodeText.text = "400";
                break;
            case 1:
                targetCodeText.text = tasks[taskIndex].sizeInCode;
                break;
            case 2:
                targetCodeText.text = tasks[taskIndex].taskDescription.color[0];
                break;
            case 3:
                targetCodeText.text = "U+" + tasks[taskIndex].code;
                for (int i = 0; i < 3; i++)
                    emojiParts[i].Setup(tasks[taskIndex].emojiFaceParts[i]);
                break;

            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        if (isTimerActive)
        {
            timerCircle.fillAmount -= Time.deltaTime / userTimeLimit;
            if (timerCircle.fillAmount <= 0)
            {
                TimeUp();
            }
        }
    }
    bool[] tasksRetried;
    public void RestartTask()
    {
        isRestarting = true;
        tasksRetried[currentTaskIndex] = true;
        TimeUp();
    }

    [Space]
    [SerializeField] float userTimeLimit = 60;
    [SerializeField] float waitTimeToShowHints = 30;
    bool isTimerActive;
    CanvasGroup taskRestartCG;
    readonly float fadeDuration = 1;
    [SerializeField] RetryConfirmation retryConfirmation;
    public void BringRetryConfirmationPanel()
    {
        Pause(true);
        retryConfirmation.EnterPanel(this,true);
    }
    public void ReturnToTask()
    {
        Pause(false);
        LeanTween.alphaCanvas(wrongEntryWarning, 0, fillDuration).setEaseInOutSine().setDelay(fillDuration);
        if (subTaskOutroInterrupted)
            HandleSubtaskFinish();
    }
    bool isInRetryQuestionSection;
    bool subTaskOutroInterrupted;
    private void Pause(bool isPaused)
    {
        KeyboardManager.Instance.enabled = !isPaused;
        taskRestarter.enabled = !isPaused;
        isTimerActive = !isPaused;
        isInRetryQuestionSection = isPaused;
        if (isPaused)
            HintsKeyboard.Instance.FadeOutKeyboard();
        else
            ShowHint();
    }
    private void ShowTryAgainButton(bool show)
    {
        LeanTween.alphaCanvas(taskRestartCG, show ? 1 : 0, fadeDuration).setEaseInOutSine().setOnComplete(() => taskRestarter.enabled = show);
    }
    private void StartTimer()
    {
        LeanTween.alphaCanvas(targetCodeTextCG, 0, fillDuration);

        if (!tasksRetried[currentTaskIndex])
            ShowTryAgainButton(true);
        isTimerActive = true;
    }
    int currentTaskIndex;
    public int GetCurrentTaskIndex => currentTaskIndex;
    const int totalTasksCount = 3;
    private void HandleSubtaskFinish()
    {
        if (!isTimerActive)
        {
            currentSubTaskIndex = 3;
        }
        if (isInRetryQuestionSection)
        {
            subTaskOutroInterrupted = true;
            return;
        }
        else
            subTaskOutroInterrupted = false;
        CheckBlankEntry();
        HintsKeyboard.Instance.StopHighlightingButtons(GeneralMethods.GetStringArrayFromTargetCode(TargetCode()));
        ResetSubTaskValues();
        LeanTween.alphaCanvas(wrongEntryWarning, 0, fillDuration).setEaseInOutSine().setDelay(fillDuration);
        if (currentSubTaskIndex == 3)
        {
            HideWindowElementsWhileWaitingForUserConfirmationToContinue();
        }
        else
        {
            StartCoroutine(CO_StartSubTask());
        }
    }
    [SerializeField] ActivationEscButton taskRestarter;
    public void TaskWindowFade(bool isExiting=false)
    {
        ShowTryAgainButton(false);
        controlPanel.FadeOut(fadeDuration);
        LeanTween.alphaCanvas(inputTextCG, 0, 0).setEaseInOutSine();
        HintsKeyboard.Instance.FadeOutKeyboard();
        LeanTween.alphaCanvas(mainCanvasGroup, 0, showTime * 2).setEaseInOutSine()
            .setOnComplete(()=>HandleWholeTaskFinish(isExiting));
    }
    
    private void HideWindowElementsWhileWaitingForUserConfirmationToContinue()
    {
        tasksDone[currentTaskIndex] = true;
        LeanTween.alphaCanvas(frame, 0, fadeDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(timerCircleCG, 0, fadeDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(inputTextCG, 0, fadeDuration).setEaseInOutSine();
        if (currentTaskIndex == totalTasksCount - 1||isRestarting)
        {
            TaskWindowFade();
        }
        else
        {
            ShowTryAgainButton(false);
            userTestMiddleGuideText.ShowSpecificText(2);
            controlPanel.OnTaskFinish();
        }
    }
    public bool[] GetDoneTasks() => tasksDone;
    private void HandleWholeTaskFinish(bool isExiting)
    {
        PerformanceManager.Instance.SpeedRecord(currentTaskIndex, timerCircle.fillAmount);
        if (isRestarting)
        {
            isRestarting = false;
            currentTaskIndex--;
        }
        if (currentTaskIndex == totalTasksCount - 1||isExiting)
        {
            //This is to only repeate The last Task
            currentTaskIndex = 1;
            GameManager.Instance.HandleUserTestFinishes();
        }
        else
            GameManager.Instance.StartTask();
    }
    private void ResetSubTaskValues()
    {
        currentStep = -1;
        isTimerActive = false;
        isInWarningPhase = false;
    }
    private void ResetValues()
    {
        ResetSubTaskValues();
        subTaskOutroInterrupted = false;
        userTestMiddleGuideText.HidePreviousDialogue();
        currentSubTaskIndex = -1;
        timerCircle.fillAmount = 1;
        mainCanvasGroup.alpha = 0;
        timerCircleCG.alpha = 0;
        guideTextCG.alpha = 0;
        emojiCG.alpha = 0;
        targetCodeTextCG.alpha = 0;
        emojiCG.gameObject.LeanScale(Vector3.one, 0);
        controlPanel.LoadDialougeIndex();
        userTestMiddleGuideText.LoadDialougeIndex();
        inputText.text = "";
        LeanTween.color(face.rectTransform, Color.white, 0);
        LeanTween.alphaCanvas(frame, 0, 0);
        LeanTween.color(inputPanelImage.rectTransform,ProjectCustomColors.Instance.MoonLight, 0);
        foreach (EmojiPart emojiPart in emojiParts)
            emojiPart.Reset();
    }
    [SerializeField] EmojiPart[] emojiParts;
    Image face;
    [SerializeField] float fillDuration = 0.5f;
    int currentStep;
    private void ShowHint()
    {
        if (currentSubTaskIndex==3)
            StartCoroutine(CO_ShowHint());
    }
    [SerializeField] float hintDisplayPeriod=1;
    IEnumerator CO_ShowHint()
    {
        LeanTween.alphaCanvas(targetCodeTextCG, 1, fillDuration);
        HintsKeyboard.Instance.HighlightButtons(new string[] { KeyboardManager.Instance.GetHintKey() }, () =>
        {
            if (!isInWarningPhase) return;
            if (letterToReturnTo == GetCurrentLetterIndex())
                isInWarningPhase = false;
            else
            {
                // This section is to override a very speical case where keyboard finishes fading in during the next task
                // so this section just force fading again
                LeanTween.alphaCanvas(targetCodeTextCG, 0, fillDuration);
                HintsKeyboard.Instance.StopHighlightingAllButtons();
                HintsKeyboard.Instance.FadeOutKeyboard();
            }
        });
        yield return Seconds.GetCachedWaitForSeconds(hintDisplayPeriod);
        HintsKeyboard.Instance.FadeOutKeyboard();
        LeanTween.alphaCanvas(targetCodeTextCG, 0, fillDuration);
    }
    #region InputReceiverFunctions 
    int currentSubTaskIndex;
    int currentEmojiPartToFill;
    private void FillEmoji()
    {
        currentStep++;
        switch (currentStep)
        {
            case 0:
                currentEmojiPartToFill = 0;
                break;
            case 1:
                return;
            case 2:
                currentEmojiPartToFill = 1;
                break;
            case 3:
                return;
            case 4:
                currentEmojiPartToFill = 2;
                break;
            default:
                break;
        }
        emojiParts[currentEmojiPartToFill].FillPart(fillDuration);
    }
    public void CorrectEntry()
    {
        HandletMiddleGuideText();
        if (currentSubTaskIndex == 1) return;
        isInWarningPhase = false;
        if (currentSubTaskIndex == 3)
            FillEmoji();
        HintsKeyboard.Instance.FadeOutKeyboard();
        LeanTween.alphaCanvas(targetCodeTextCG, 0, fillDuration);
    }
    
    [SerializeField] float minimumLogic = 0.5f;
    bool isInWarningPhase;
    public void WrongEntry()
    {
        PerformanceManager.Instance.LogicRecord(currentTaskIndex);
        if (!isInWarningPhase)
            ShowWarning();
    }
    bool isRestarting;
    int userEnteredSize;
    public void EntryFinished()
    {
        if (currentSubTaskIndex == 1 && int.TryParse(inputText.text,out userEnteredSize))
        {
            int minValue = int.Parse(TargetCode()) - 25;
            int maxValue = int.Parse(TargetCode()) + 25;
            if (userEnteredSize > maxValue || userEnteredSize < minValue)
            {
                WrongEntry();
                isInWarningPhase = false;
                inputText.text = currentStartText;
                StartCoroutine(CO_SetKeyboardReceiverAfterDelay());
                return;
            }
        }
        
        if (isRestarting || !isTimerActive)
        {
            currentSubTaskIndex = 3;
            isInRetryQuestionSection = false;
            HandleSubtaskFinish();
        }
        else
        {
            EmojiDraw();
            GreenHighlightCorrectEntry();
        }
    }
    [Space]
    [Header("Correct Entry Higlight")]
    [SerializeField] float correctEntryHilightDuration=1;
    [SerializeField] int correctEntryLoopCount=2;

    private void GreenHighlightCorrectEntry()
    {
        inputPanelImage.color = ProjectCustomColors.Instance.MoonLight;
        LeanTween.cancel(inputPanelImage.rectTransform);
        LeanTween.color(inputPanelImage.rectTransform, ProjectCustomColors.Instance.Green, correctEntryHilightDuration)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setRepeat(correctEntryLoopCount)
            .setOnComplete(HandleSubtaskFinish);
    }
    private void EmojiDraw()
    {
        switch (currentSubTaskIndex)
        {
            case 0:
                LeanTween.alphaCanvas(frame, 1, showTime).setEaseInOutSine();
                break;
            case 1:
                LeanTween.scale(emojiCG.gameObject, tasks[currentTaskIndex].size, fillDuration).setEaseInOutSine();
                break;
            case 2:
                LeanTween.color(face.rectTransform, tasks[currentTaskIndex].color, fillDuration).setEaseInOutSine();
                break;
        }
    }
    LetterIndex letterToReturnTo;
    private void ShowWarning()
    {
        isInWarningPhase = true;
        LeanTween.alphaCanvas(wrongEntryWarning, 1, fillDuration).setEaseInOutSine();
        HintsKeyboard.Instance.FadeOutKeyboard(0.1f);
        LeanTween.alphaCanvas(targetCodeTextCG, 0, fillDuration);
        letterToReturnTo = GetCurrentLetterIndex();
        LeanTween.cancel(inputPanelImage.rectTransform);
        inputPanelImage.color = ProjectCustomColors.Instance.MoonLight;
        LeanTween.color(inputPanelImage.rectTransform, ProjectCustomColors.Instance.Red, fillDuration).setEaseInOutSine().setLoopPingPong(1).setOnComplete(() =>
        {
            if (letterToReturnTo == GetCurrentLetterIndex()&&isTimerActive)
                ShowHint();
            LeanTween.alphaCanvas(wrongEntryWarning, 0, fillDuration).setEaseInOutSine();
        });
    }
    private LetterIndex GetCurrentLetterIndex()
    {
        return new LetterIndex { taskIndex = currentTaskIndex, subTaskIndex = currentSubTaskIndex, letterIndex = KeyboardManager.Instance.GetLetterIndex }; letterToReturnTo = new LetterIndex { taskIndex = currentTaskIndex, subTaskIndex = currentSubTaskIndex, letterIndex = KeyboardManager.Instance.GetLetterIndex };
    }
    private void TimeUp()
    {
        isTimerActive = false;
        KeyboardManager.Instance.DisconnectInputReceiver();
    }
    string currentGuidTextModifedText;
    public void HandletMiddleGuideText()
    {
        if (currentGuidTextModifedText == "\"---\""||currentGuidTextModifedText== "\"u+---\"")
        {
            userTestMiddleGuideText.ReplaceUserTestGuideText(currentGuidTextModifedText, inputText.text);
            string specialText = "\"" + inputText.text + "\"";
            userTestMiddleGuideText.ReplaceUserTestGuideText(inputText.text, specialText);
        }

        else
            userTestMiddleGuideText.ReplaceUserTestGuideText(currentGuidTextModifedText, inputText.text);

        currentGuidTextModifedText = inputText.text;
    }
    private void _NextDialogues()
    {
        controlPanel.ShowNextDialogue();
        switch (currentSubTaskIndex)
        {
            case 0:
                userTestMiddleGuideText.ColorizeText("\"---\"", ProjectCustomColors.Instance.Green);
                currentGuidTextModifedText= "---";
                break;
            case 1:
                LeanTween.alphaCanvas(emojiCG, 1, showTime / 4).setEaseInOutSine();
                userTestMiddleGuideText.ShowNextDialogue();
                string[] spaces = new string[] { "---", "-----" };
                userTestMiddleGuideText.ReplaceCustomContent(spaces,true);
                userTestMiddleGuideText.ColorizeText("\"---\"", ProjectCustomColors.Instance.Green);
                userTestMiddleGuideText.AddFacialExpressionInUserTest();
                currentGuidTextModifedText = "\"---\"";
                int sizeOfEmoji = int.Parse(tasks[currentTaskIndex].sizeInCode);
                string minValue = (sizeOfEmoji - 25).ToString();
                string maxValue = (sizeOfEmoji + 25).ToString();
                controlPanel.ReplaceRadiusText(new string[] { minValue, maxValue });
                break;
            case 2:
                userTestMiddleGuideText.FadeInDialouge();
                string[] size = new string[] { userEnteredSize.ToString(), "-----" };
                userTestMiddleGuideText.ReplaceCustomContent(size,true);
                userTestMiddleGuideText.AddFacialExpressionInUserTest();
                userTestMiddleGuideText.ColorizeText("\"-----\"", ProjectCustomColors.Instance.Green);
                currentGuidTextModifedText = "-----";
                break;
            case 3:
                userTestMiddleGuideText.FadeInDialouge();
                string[] color = new string[] { userEnteredSize.ToString(), tasks[currentTaskIndex].taskDescription.color[0] };
                userTestMiddleGuideText.ReplaceCustomContent(color,true);
                userTestMiddleGuideText.AddFacialExpressionInUserTest();
                userTestMiddleGuideText.ColorizeText("\"u+---\"", ProjectCustomColors.Instance.Green);
                currentGuidTextModifedText = "\"u+---\"";
                break;

            default:
                break;
        }
    }

    IEnumerator CO_ShowLastMiddleTextAfterWaitToShowColorInPlace(float delay)
    {
        int SubTaskIndexBeforeWait = currentSubTaskIndex;
        yield return Seconds.GetCachedWaitForSeconds(delay);
        if (currentSubTaskIndex != SubTaskIndexBeforeWait) yield break;
        userTestMiddleGuideText.ShowNextDialogue();
        string[] space = new string[] { "---" };
        userTestMiddleGuideText.ReplaceCustomContent(space);
    }
    string currentStartText;
    private void CheckBlankEntry()
    {
        if (inputText.text == currentStartText)
        {
            PerformanceManager.Instance.LogicRecord(currentTaskIndex, true);
        }
    }
    public string TargetCode()
    {
        switch (currentSubTaskIndex)
        {
            case 0:
                return "400";
            case 1:
                return tasks[currentTaskIndex].sizeInCode;
            case 2:
                return tasks[currentTaskIndex].taskDescription.color[0];
            case 3:
                return tasks[currentTaskIndex].code;
            default:
                return "";

        }
    }
    #endregion
    public Color GetColorByTaskIndex(int taskIndex) => tasks[taskIndex].color;
    public Vector3 GetSizeByTaskIndex(int taskIndex) => tasks[taskIndex].size;

    public TextMeshProUGUI GetInputText()
    {
        return inputText;
    }
    public string GetInputStartText()
    {
        return currentStartText;
    }
    public void RetryConformed()
    {
        StartCoroutine(CO_FadeOutRetryCanvasAfterDelay(RestartTask));
    }
    public void RetryDenied()
    {
        StartCoroutine(CO_FadeOutRetryCanvasAfterDelay(ReturnToTask));
        
    }
    IEnumerator CO_FadeOutRetryCanvasAfterDelay(Action callBack)
    {
        yield return Seconds.GetCachedWaitForSeconds(0.5f);
        callBack.Invoke();
    }
}