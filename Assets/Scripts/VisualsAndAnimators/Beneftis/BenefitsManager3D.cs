using UnityEngine;
using System;
using UnityEngine.Video;
using UnityEngine.UI;
public class BenefitsManager3D : Singleton<BenefitsManager3D>
{
    [SerializeField] TopicsChoosingPanel3DGroup topicsChoosingPanel;

    [SerializeField] SlidesController slidesController;

    [SerializeField] CanvasGroup benefitsInputField;
    [SerializeField] float fadeDuration=0.5f;
    private int currentTopic;
    public int CurrentTopicIndex => currentTopic;

    #region GlobalPopingAttributes
    [Space]
    [Header("GlobalPopingAttributes")]
    [SerializeField] float popingSize=2.5f;
    [SerializeField] float singlePopingDuration=0.2f ;
    [SerializeField] int loopCount=2;

    Vector3 popingVector3;
    #endregion

    #region GameManagerPublicFunctions
    public void EnterTopicChoosingMenu()
    {
        topicsChoosingPanel.Enter(() =>
        {
            ShowExitButton(true);
            TakeInput();
        });
    }
    public void ShowbackButton(bool show)
    {
        LeanTween.alphaCanvas(mainmenuButtonCG, show ? 1 : 0, fadeDuration).setEaseInOutSine().setOnComplete(() => back.enabled = show);
    }
    private void ShowExitButton(bool show)
    {
        LeanTween.alphaCanvas(exitButtonCG, show?1:0, fadeDuration).setEaseInOutSine().setOnComplete(()=>exit.enabled=show);
    }
    private void TakeInput()
    {
        isAcceptingInput = true;
        LeanTween.alphaCanvas(benefitsInputField, 1, fadeDuration).setEaseInOutSine();
        KeyboardManager.Instance.SetReceiver(TopicSelectionInputReceiver);
    }
    [SerializeField] TopicSelectionInputReceiver TopicSelectionInputReceiver;
    public void SelectTopic(int topicIndex)
    {
        currentTopic = topicIndex;
        topicsSeen[currentTopic] = true;
        ShowExitButton(false);
        topicsChoosingPanel.PopChoice(topicIndex, () =>
        {
            topicsChoosingPanel.Exit(()=>EnterTopic(currentTopic));
        });
    }
    public void HideSecondTopicWhenSlideFillScreen()
    {
        //topicsChoosingPanel.WithdrawPanel(1);
    }
    #endregion
    private void EnterTopic(int index)
    {
        if (index == 0)
            StartSlideShow();
        else
            StartVideo();
    }
    private void StartSlideShow()
    {
        slidesController.StartSlides(OnTopicFinishes);
    }
    private void StartVideo()
    {
        slidesController.StartSlides(PlayVideo);
    }
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip[] videoClipsBasedOnLanguage;
    public void PlayVideo()
    {
        videoPlayer.enabled = true;
        videoPlayer.Prepare();
        videoPlayer.Play();
    }
    [Header("Video")]
    [SerializeField] CanvasGroup videoPlayerCG;
    [SerializeField] RawImage videoPlayerImage;
    [SerializeField] Texture videoFirstFrameTexture;
    [SerializeField] Vector3 videoFocusedSize=Vector3.one* 1.229547f;
    private void OnVideoReady(VideoPlayer videoPlayer)
    {
        RenderTexture renderTexture = new RenderTexture(1920,1080, 0);
        videoPlayer.targetTexture = renderTexture;
        LeanTween.alphaCanvas(videoPlayerCG, 1, fadeDuration);
        LeanTween.scale(videoPlayerCG.gameObject, videoFocusedSize, fadeDuration);
        videoPlayerImage.texture = renderTexture;
    }

    private void OnVideoFinshes(VideoPlayer videoPlayer)
    {
        videoPlayer.Stop();
        videoPlayer.enabled = false;
        LeanTween.scale(videoPlayerCG.gameObject, Vector3.one, fadeDuration);
        LeanTween.alphaCanvas(videoPlayerCG, 0, fadeDuration).setOnComplete(()=>
        {
            videoPlayerImage.texture = videoFirstFrameTexture;
        });
        slidesController.OnVideoFinish(OnTopicFinishes);
    }
    bool[] topicsSeen;
    bool[] topicsExited;
    public void Exit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    private void OnTopicFinishes()
    {
        if (topicsSeen[0] && topicsSeen[1])
            Exit();
        else
            ReturnToTopicSelectionMenu();
    }
    public void OntopicExit()
    {
        topicsExited[currentTopic] = true;
        ReturnToTopicSelectionMenu();
    }
    private void ReturnToTopicSelectionMenu()
    {
        videoPlayer.Stop();
        slidesController.Reset();
        ShowbackButton(false);
        GameManager.Instance.EnterBenefitsMenu();
    }
    private void Start()
    {
        LanguageManager.Instance.OnLangaugeSelected += OnLangaugeSelected;
        topicsSeen = new bool[2];
        topicsExited = new bool[2];
        popingVector3 = Vector3.one * popingSize;
        LeanTween.scale(warningText.gameObject, Vector3.zero, 0);
        videoPlayer.loopPointReached += OnVideoFinshes;
        videoPlayer.prepareCompleted += OnVideoReady;
        back = mainmenuButtonCG.GetComponent<ActivationEscButton>();
        exit = exitButtonCG.GetComponent<ActivationEscButton>();
    }
    private void OnLangaugeSelected()
    {
        switch (LanguageManager.Instance.GetCurrentLanguage)
        {
            case LanguageManager.Language.English:
                warningText.transform.GetChild(0).gameObject.SetActive(true);
                videoPlayer.clip = videoClipsBasedOnLanguage[0];
                break;
            case LanguageManager.Language.Arabic:
                warningText.transform.GetChild(1).gameObject.SetActive(true);
                videoPlayer.clip = videoClipsBasedOnLanguage[1];
                break;
            default:
                break;
        }
    }
    [Space]
    [SerializeField] CanvasGroup mainmenuButtonCG;
    [SerializeField] CanvasGroup exitButtonCG;
    ActivationEscButton back,exit;
    
    [SerializeField] CanvasGroup warningText;
    [SerializeField] float warningDisplayDuration=3;
    bool isAcceptingInput;
    public void DisplayWarning()
    {
        LeanTween.alphaCanvas(benefitsInputField, 0, fadeDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(warningText, 1, fadeDuration).setEaseInOutSine();
        LeanTween.scale(warningText.gameObject, Vector3.one, fadeDuration).setEaseInOutSine();
        LeanTween.alphaCanvas(warningText, 0, fadeDuration).setDelay(warningDisplayDuration).setEaseInOutSine();
        LeanTween.scale(warningText.gameObject, Vector3.zero, fadeDuration).setEaseInOutSine().setDelay(warningDisplayDuration).setOnComplete(() =>
        {
            if (!isAcceptingInput) return;
            TakeInput();
        }
        );
    }
    public void PopUpSelectedTopic(GameObject selectedTopic,Action callBack)
    {
        isAcceptingInput = false;
        LeanTween.scale(selectedTopic, popingVector3, singlePopingDuration).setEaseInOutSine().setLoopPingPong(loopCount)
            .setOnComplete(callBack);
    }
    public string[] GetUpperDialogueBasedOnTopic()
    {
        if (!topicsExited[currentTopic])
            ShowbackButton(true);
        string [] customMassage;
        if (currentTopic == 0)
            if (LanguageManager.Instance.GetCurrentLanguage == LanguageManager.Language.English)
                customMassage = new string[] { "reliable infrastructure to support the education sector" };
            else
                customMassage = new string[] { "بُنى تعليم تحتية موثوقة لدعم القطاع التعليمي" };
        else
            if (LanguageManager.Instance.GetCurrentLanguage == LanguageManager.Language.English)
                customMassage = new string[] { "our educational solution components:" };
            else
                customMassage = new string[] { "مكونات حلولنا التعليمية" };
        return customMassage;
    }
}

