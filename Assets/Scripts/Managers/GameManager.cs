using System.Collections;
using UnityEngine;
using System;
using System.IO;
public class GameManager : Singleton<GameManager>,IRetryReceiver
{
    #region StatesHandler
    GameStates.State currentState;
    public GameStates.State GetCurrentState => currentState;
    public Action<GameStates.State, GameStates.State> StateChanged;
    CameraAnimator cameraAnimator;

    [SerializeField] TeacherController teacherController;
    #region NoInteractionHandling
    [Space]
    [SerializeField] float noInteractionWaitDuration = 30;

    int enterPageWaitIndex;
    IEnumerator CO_HandleNoInteraction()
    {
        enterPageWaitIndex++;
        int sameEntry = enterPageWaitIndex;
        GameStates.State enteredState = currentState;
        yield return Seconds.GetCachedWaitForSeconds(noInteractionWaitDuration);

        if (currentState == enteredState && enterPageWaitIndex == sameEntry)
        {
            HandleNoInteraction();
        }
    }
    protected override void Awake()
    {
        base.Awake();
    }
    private void HandleNoInteraction()
    {
        switch (currentState)
        {
            case GameStates.State.Login:
                RestartGame();
                break;
            case GameStates.State.Results:
                RestartGame();
                break;
            case GameStates.State.BenefitsMenu:
                RestartGame();
                break;
            default:
                break;
        }
    }
    private void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    #endregion

    private void Start()
    {
        KeyboardManager.Instance.enabled = false;
        cameraAnimator = Camera.main.GetComponent<CameraAnimator>();
        _yesNoReceiver = yesNoReceiver.GetComponent<IInputReceiver>();
        timesEnteredUserTest = -1;
        EnterIdle();
    }

    private void GoToNextState()
    {
        GameStates.State prevState = currentState;
        switch (currentState)
        {
            case GameStates.State.Idle:
                EnterLogin();
                break;
            case GameStates.State.Login:
                EnterWelcomingDialogue();
                break;
            case GameStates.State.WelcomingDialogue:
                EnterShelvesFilling();
                break;
            case GameStates.State.ShelvesFilling:
                EnterShelvesRotate();
                break;
            case GameStates.State.ShelvesRotate:
                EnterEmojisDisplayNames();
                break;
            case GameStates.State.EmojisDisplayNames:
                EnterTutorial();
                break;
            case GameStates.State.Tutorial:
                EnterUserTest();
                break;
            case GameStates.State.UserTest:
                EnterResults();
                break;
            case GameStates.State.Results:
                EnterShowPerformance();
                break;
            case GameStates.State.ShowPerformance:
                EnterBenefitsIntro();
                break;
            case GameStates.State.BenefitsIntro:
                EnterBenefitsMenu();
                break;
            case GameStates.State.BenefitsMenu:
                EnterBenefitsShowcase();
                break;
            default:
                break;
        }
        StartCoroutine(CO_HandleNoInteraction());
        StateChanged?.Invoke(prevState, currentState);
    }
    #region DeleteMe
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == GameStates.State.Login)
            {
                MockScan();
                return;
            }
            if (currentState == GameStates.State.Idle)
            {
                BodyDetected();
                return;
            }
        }
    }
    bool deleteMeMockScanLock;
    private void MockScan()
    {
        if (deleteMeMockScanLock) return;
        deleteMeMockScanLock = true;
        StartScanning();
        StartCoroutine(CO_WaitThenMockScanSuccess());
    }
    IEnumerator CO_WaitThenMockScanSuccess()
    {
        yield return Seconds.GetCachedWaitForSeconds(2);
        ScanSuccess();
    }
    #endregion

    #region Idle
    [Space]
    [Header("Idle Section")]
    [SerializeField] float firstTextEntryDelay;
    private void EnterIdle()
    {
        currentState = GameStates.State.Idle;
        cameraAnimator.ZoomIn();
        StartCoroutine(CO_EnterFirstTexts());
    }
    IEnumerator CO_EnterFirstTexts()
    {
        yield return Seconds.GetCachedWaitForSeconds(firstTextEntryDelay);
        TextsManager3D.Instance.MoveTexts(0, () =>
        {
            TextsManager3D.Instance.MoveTexts(1);
            waitingForBodyDetection = true;
        });
    }
    #endregion

    #region Login
    [Space]
    [Header("Login Section")]
    [SerializeField] ScanFace scanFace;
    bool waitingForBodyDetection;
    private void EnterLogin()
    {
        currentState = GameStates.State.Login;
        waitingForBodyDetection = false;
        TextsManager3D.Instance.FadeOutTexts();
        LeanTween.alphaCanvas(scanFace.canvasGroup, 1, 0.5f).setEaseOutSine();
    }

    public void StartScanning()
    {
        scanFace.Scan();
    }
    public void ScanSuccess()
    {
        scanFace.FadeOut();
        TextsManager3D.Instance.FadeInTexts();
        GetUserData();
        GoToNextState();
    }
        struct LanguageChanger
        {
        public bool IsArabic;
        }
    private void GetUserData()
    {
        string[] mockUserName = {"Yusef"};
        int langugeId;
        //Delete this after integrating face sdk

        #region MockingLanguageSelecting

        string path = Application.dataPath + "/../LanguageChanger.json";
        StreamReader reader = new StreamReader(path);
        string data = reader.ReadToEnd();

        langugeId = JsonUtility.FromJson<LanguageChanger>(data).IsArabic?1:0;
        #endregion

        userData = new UserRetrivedData(mockUserName,langugeId);
        LanguageManager.Instance.SetLanguage(langugeId);
        if (langugeId==1)
        {
            EmojisManager.Instance.SetResultEmojisArabicBarNames();
        }
}
    #endregion

    #region WelcomingDialogue
    [Space]
    [Header("Welcoming Dialogue Section")]
    [SerializeField] DialogueManager upperPanelDialogue;
    UserRetrivedData userData;

    private void EnterWelcomingDialogue()
    {
        currentState = GameStates.State.WelcomingDialogue;
        LeanTween.alphaCanvas(scanFace.canvasGroup, 0, 0.5f).setEaseInOutSine();
        TextsManager3D.Instance.MoveTexts(2);
        upperPanelDialogue.BringDialogueInsideScene(() =>
        {
            upperPanelDialogue.ShowNextDialogue(0, () =>
            {
                upperPanelDialogue.ShowNextDialogue(0, () =>
                {
                    upperPanelDialogue.ShowNextDialogue(5, GoToNextState);
                    upperPanelDialogue.ReplaceCustomContent(userData.userName);
                });
            });
            upperPanelDialogue.ReplaceCustomContent(userData.userName);
        }); 
    }
    #endregion


    #region ShelvesFilling
    private void EnterShelvesFilling()
    {
        currentState = GameStates.State.ShelvesFilling;
        upperPanelDialogue.ShowNextDialogue(4);
        upperPanelDialogue.ReplaceCustomContent(userData.userName);
        cameraAnimator.ZoomOut();
        teacherController.DisplayEmojis(()=>
        EmojisManager.Instance.Intro(GoToNextState));
    }
    
    #endregion

    #region ShelvesRotate
    [Space]
    [Header("Rotate Shelves Section")]
    [SerializeField] ShelvesAnimator ShelvesAnimatorsManager;
    [SerializeField] float shelvesRotatingDelay = 2;
    private void EnterShelvesRotate()
    {
        currentState = GameStates.State.ShelvesRotate;
        cameraAnimator.DisableEmojisOverlayCamera();
        StartCoroutine(CO_DelayShelvesRotating());
    }
    IEnumerator CO_DelayShelvesRotating()
    {
        yield return Seconds.GetCachedWaitForSeconds(shelvesRotatingDelay);
        upperPanelDialogue.ShowNextDialogue();
        ShelvesAnimatorsManager.PlayAnimation(ShelvesAnimatorParameters.enter, GoToNextState);
    }
    #endregion

    #region EnterEmojisDisplayNames
    private void EnterEmojisDisplayNames()
    {
        currentState = GameStates.State.EmojisDisplayNames;
        upperPanelDialogue.ShowNextDialogue();
        EmojisManager.Instance.ShowEmojisNamesOnTop(() =>
        upperPanelDialogue.ShowNextDialogue(2, () =>
        {
             tutorialCTA.BringInCTA();
             upperPanelDialogue.HidePreviousDialogue();
        })
        );
    }
    #endregion

    #region EnterTutorial
    public void EnterTutorialConformed()
    {
        tutorialCTA.FadeOutCTA();
        GoToNextState();
    }
    private void EnterTutorial()
    {
        currentState = GameStates.State.Tutorial;
        StartCoroutine(CO_TutorialSequence());
    }
    [Space]
    [Header("Tutorial Section")]
    [SerializeField] TutorialCTA tutorialCTA;
    [SerializeField] DialogueManager middleBottomScaledDialogue;
    [SerializeField] WebSimulatorManager tutorialWebSimulator;
    IEnumerator CO_TutorialSequence()
    {
        // There is 2 sequences playing in this state , one is in the bottom of the web simulator window 
        // The other one is inside the html box 
        // What is happening here is simply playing next item in each sequence in correct order
        // using only 2 functions for that : NextStepText() & NexthtmlSequenceItem() 
        // other functions are to show canvases of backgrounds and fadeout some items of the sequence

        teacherController.StartTalking();
        yield return Seconds.GetCachedWaitForSeconds(1);
        upperPanelDialogue.ShowNextDialogue();
        upperPanelDialogue.TutorialText();
        tutorialWebSimulator.ShowCanvas();
        tutorialWebSimulator.ShowNextDialogue(1, () => tutorialWebSimulator.HidePreviousDialogue());
        yield return Seconds.GetCachedWaitForSeconds(3);
        upperPanelDialogue.ShowNextDialogue();
        upperPanelDialogue.TutorialText();
        yield return Seconds.GetCachedWaitForSeconds(4);
        //Show Frame
        tutorialWebSimulator.ShowHtmlBox();

        upperPanelDialogue.ShowNextDialogue();
        upperPanelDialogue.TutorialText();

        tutorialWebSimulator.ShowNextDialogue();
        tutorialWebSimulator.ColorizeText("400",ProjectCustomColors.Instance.Green);
        yield return Seconds.GetCachedWaitForSeconds(4);

        //Show Face Frame
        upperPanelDialogue.ShowNextDialogue();
        upperPanelDialogue.TutorialText();
        tutorialWebSimulator.ShowNextDialogue();
        tutorialWebSimulator.ColorizeText("\"--\"", ProjectCustomColors.Instance.Green);
        tutorialWebSimulator.NextHtmlSequenceItem();
        yield return Seconds.GetCachedWaitForSeconds(5);
        //Change emoji Size
        yield return Seconds.GetCachedWaitForSeconds(tutorialWebSimulator.ResizeEmoji());
        yield return Seconds.GetCachedWaitForSeconds(2);
        //Change color
        upperPanelDialogue.ShowNextDialogue();
        upperPanelDialogue.TutorialText();
        tutorialWebSimulator.ShowNextDialogue();
        tutorialWebSimulator.ColorizeText("\"---\"", ProjectCustomColors.Instance.Green);
        yield return Seconds.GetCachedWaitForSeconds(2);
        tutorialWebSimulator.ReplaceTutorialColor();
        tutorialWebSimulator.NextHtmlSequenceItem();
        yield return Seconds.GetCachedWaitForSeconds(3);

        //Show smiling face text
        //upperPanelDialogue.HidePreviousDialogue();
        //tutorialWebSimulator.NextHtmlSequenceItem();
        //yield return Seconds.GetCachedWaitForSeconds(2);
        ////Show bottom text
        //tutorialWebSimulator.NextHtmlSequenceItem();
        //yield return Seconds.GetCachedWaitForSeconds(3);

        //Fill emoji face parts
        upperPanelDialogue.ShowNextDialogue();
        upperPanelDialogue.TutorialText();
        tutorialWebSimulator.ShowNextDialogue();
        yield return Seconds.GetCachedWaitForSeconds(2);
        tutorialWebSimulator.TypeInFacialExpression();
        yield return Seconds.GetCachedWaitForSeconds(8);
        tutorialWebSimulator.FadeOutHtmlBox();
        yield return Seconds.GetCachedWaitForSeconds(0.5f);
        EmojisManager.Instance.HideEmojisNamesOnTopAfterDelay();
        EmojisManager.Instance.FadeOutEmojisTextureAfterDelay();
        tutorialWebSimulator.FadeOutWebSimulator(() =>
        {
            upperPanelDialogue.ShowNextDialogue(0, () =>
            {
                upperPanelDialogue.ControlTextWrapping(true);
                upperPanelDialogue.ShowNextDialogue(0, () =>
                ShelvesAnimatorsManager.PlayAnimation(ShelvesAnimatorParameters.midRotate,GoToNextState));
            });
            upperPanelDialogue.ReplaceCustomContent(userData.userName);
            upperPanelDialogue.ControlTextWrapping(false);
        });
        teacherController.StopTalking();
    }
    #endregion

    #region UserTest
    [Space]
    [Header("User Test")]
    [SerializeField] int retryTestsLimit = 1;
    [SerializeField] DialogueManager userTestMiddleText;
    int timesEnteredUserTest;
    private void EnterUserTest()
    {
        currentState = GameStates.State.UserTest;
        UserTestWebSimulatorManager.Instance.RestartAllTasks();
        if (timesEnteredUserTest>-1)
        {
            StartTask();
            timesEnteredUserTest++;
            return;
        }
        StartTask();
        middleBottomScaledDialogue.HidePreviousDialogue();
        timesEnteredUserTest++;
    }
    public void StartTask()
    {
        int emojiIndex = UserTestWebSimulatorManager.Instance.GetCurrentTaskIndex+1;
        int index = LanguageManager.Instance.GetCurrentLanguageIndex;
        string[] customContent = GetCustomContent(index, emojiIndex, UserTestWebSimulatorManager.Instance.GetTaskDescrption());
        upperPanelDialogue.RepetedTaskDialouge(timesEnteredUserTest > -1, customContent);
        StartCoroutine(ShowTargetEmojiBeforeTasStart(emojiIndex, () =>
        {
            UserTestWebSimulatorManager.Instance.BringInControlPanel();
        }));
    }
    private string[] GetCustomContent(int index, int emojiIndex, TaskDescription taskDescription)
    {
        string taskIndex = (emojiIndex + 1).ToString();
        string[] customContent = { taskIndex
               , taskDescription.size[index], taskDescription.color[index], taskDescription.facialExpression[index] };
        return customContent;
    }
    IEnumerator ShowTargetEmojiBeforeTasStart(int emojiIndex,Action startTask)
    {
        FadeInSingleResultEmoji(emojiIndex,null);
        yield return Seconds.GetCachedWaitForSeconds(EmojisManager.Instance.FadingDuration*3);
        EmojisManager.Instance.FadeOutSingleEmoji(emojiIndex);
        startTask.Invoke();
    }
    bool forceNoRetry;
    public void HandleUserTestFinishes()
    {
        upperPanelDialogue.BreakRepetedDialogue(1);
        if (timesEnteredUserTest == retryTestsLimit)
        {
            upperPanelDialogue.ShowNextDialogue();
            forceNoRetry = true;
        }
        else
            upperPanelDialogue.ShowNextDialogue(1);
        upperPanelDialogue.ReplaceCustomContent(userData.userName);
        GoToNextState();
    }
    #endregion

    #region Results Section

    [Space]
    [Header("Results Section")]
    [SerializeField] private float emojisFadeDuration = 1;
    [SerializeField] private GameObject yesNoReceiver;
    private IInputReceiver _yesNoReceiver;
    private void EnterResults()
    {
        currentState = GameStates.State.Results;
        bool[] doneTasks= UserTestWebSimulatorManager.Instance.GetDoneTasks();
        int currentIndex = 0;

        StartFadingInEmojis();
        void StartFadingInEmojis()
        {
            if (currentIndex < doneTasks.Length)
            {
                if (doneTasks[currentIndex])
                {
                    FadeInSingleResultEmoji(currentIndex, () =>
                    {
                        currentIndex++;
                        StartFadingInEmojis(); // Call the function recursively for the next index
                    });
                }
                else
                {
                    currentIndex++;
                    StartFadingInEmojis(); // Call the function recursively for the next index
                }
            }
            else
            {
                // This part will be executed when all animations are done
                // Now you can add the code to check the retry limit and show the panel
                if (timesEnteredUserTest == retryTestsLimit)
                {
                    UserChoiceToRetry(false);
                    return;
                }

                upperPanelDialogue.HidePreviousDialogue();
                retryConfirmation.EnterPanel(this, false);
            }
        }
    }
    [SerializeField] RetryConfirmation retryConfirmation;
    public void UserChoiceToRetry(bool userChooseYes)
    {
        if (forceNoRetry)
            upperPanelDialogue.ShowNextDialogue();

        upperPanelDialogue.ShowNextDialogue(0, () =>
        {
            HintsKeyboard.Instance.FadeOutKeyboard();
            if (userChooseYes)
                HideResultsAndGoesBackToUserTests();
            else
                GoToNextState();
        });
        string[] customMassage;
        customMassage = GetCustomTextBasedOnUserChoice(userChooseYes);
        upperPanelDialogue.ReplaceCustomContent(customMassage);
    }
    private string[] GetCustomTextBasedOnUserChoice(bool userChooseYes)
    {
        string[] customMassage;
        if (userChooseYes)
            if (LanguageManager.Instance.GetCurrentLanguage==LanguageManager.Language.English)
                customMassage = new string[] { "alright let's improve your programming skills" };
            else
                customMassage = new string[] { "حسناً فلنحسن مهاراتك البرمجية" };
        else
            if (LanguageManager.Instance.GetCurrentLanguage==LanguageManager.Language.English)
                customMassage = new string[] { "you've mastered the course" };
            else
                customMassage = new string[] { "لقد أتقنت الدرس" };

        return customMassage;
    }
    private void HideResultsAndGoesBackToUserTests()
    {
        EmojisManager.Instance.FadeOutResultEmojis();
        Invoke(nameof(EnterUserTest), emojisFadeDuration);
    }
    private void FadeInSingleResultEmoji(int index,Action callBack)
    {
        EmojisManager.Instance.FadeInSingleResultEmoji(index, callBack);
    } 
    #endregion

    #region ShowPerformance Section
    private void EnterShowPerformance()
    {
        currentState = GameStates.State.ShowPerformance;
        upperPanelDialogue.ShowNextDialogue(0,()=>
        {
            teacherController.DisplayLastTexts();
            ShelvesAnimatorsManager.PlayAnimation(ShelvesAnimatorParameters.performance,()=>
            {
                ShowPerfromance();
                upperPanelDialogue.ShowNextDialogue(1, GoToNextState);
            });
        });
    }
    private void ShowPerfromance()
    {
        currentState = GameStates.State.ShowPerformance;
        bool[] doneTasks = UserTestWebSimulatorManager.Instance.GetDoneTasks();
        int currentIndex = 0;

        StartShowPerformance();
        void StartShowPerformance()
        {
            if (currentIndex < doneTasks.Length)
            {
                if (doneTasks[currentIndex])
                {
                    ShowPerformanceOfSingleEmoji(currentIndex, () =>
                    {
                        currentIndex++;
                        StartShowPerformance(); // Call the function recursively for the next index
                    });
                }
                else
                {
                    currentIndex++;
                    StartShowPerformance(); // Call the function recursively for the next index
                }
            }
        }
    }
    private void ShowPerformanceOfSingleEmoji(int index, Action callBack=null)
    {
        EmojisManager.Instance.ShowPerformanceOfSingleEmoji(index, callBack);
    }
    #endregion

    #region BenefitsIntro
    private void EnterBenefitsIntro()
    {
        currentState = GameStates.State.BenefitsIntro;
        upperPanelDialogue.HidePreviousDialogue();
        EmojisManager.Instance.FadeOutResultEmojis();
        Invoke(nameof(RotateShelvesBack), emojisFadeDuration*2);
    }
    private void RotateShelvesBack()
    {
        ShelvesAnimatorsManager.PlayAnimation(ShelvesAnimatorParameters.back, () =>
        {
            upperPanelDialogue.ShowNextDialogue();
            EmojisManager.Instance.HideEmojis();
            TextsManager3D.Instance.MoveTexts(3, () =>
            {
                upperPanelDialogue.ShowNextDialogue(0, () =>
                {
                    upperPanelDialogue.SaveDialougeIndex();
                    GoToNextState();
                });
                upperPanelDialogue.ReplaceCustomContent(userData.userName);
            });
        });
    }
    #endregion

    #region BenefitsMenu
    public void EnterBenefitsMenu()
    {
        GameStates.State prevState = currentState;
        currentState = GameStates.State.BenefitsMenu;
        StateChanged?.Invoke(prevState, currentState);

        upperPanelDialogue.LoadDialougeIndex();
        upperPanelDialogue.ShowNextDialogue();

        BenefitsManager3D.Instance.EnterTopicChoosingMenu();
        
        StartCoroutine(CO_HandleNoInteraction());
    }
    #endregion

    #region BenefitsShowcase
    public void UserSelectedTopic(int topicIndex)
    {
        BenefitsManager3D.Instance.SelectTopic(topicIndex);
        GoToNextState();
    }
    public void EnterBenefitsShowcase()
    {
        currentState = GameStates.State.BenefitsShowcase;
        upperPanelDialogue.ShowNextDialogue(0,()=>
        {
            upperPanelDialogue.ShowNextDialogue(1, () =>
             upperPanelDialogue.ReplaceCustomContent(BenefitsManager3D.Instance.GetUpperDialogueBasedOnTopic()));
        });
    }

    #endregion

    #endregion

    #region Public Calls
    public void BodyDetected()
    {
        if (!waitingForBodyDetection) return;
            GoToNextState();
    }

    public void RetryConformed()
    {
        UserChoiceToRetry(true);
    }

    public void RetryDenied()
    {
        UserChoiceToRetry(false);
    }
    #endregion

}