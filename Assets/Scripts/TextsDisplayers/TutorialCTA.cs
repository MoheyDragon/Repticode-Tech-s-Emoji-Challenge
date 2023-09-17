using UnityEngine;

public class TutorialCTA : MonoBehaviour
{
    DialogueManager dialogueManager;
    CanvasGroup canvasGroup;
    ActivationEscButton startTutorialButton;

    float fadeOutDuration=1.5f;
    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
        canvasGroup = GetComponent<CanvasGroup>();
        startTutorialButton = GetComponent<ActivationEscButton>();
    }
    public void BringInCTA()
    {
        dialogueManager.BringDialogueInsideScene(()=>
        {
            dialogueManager.ShowNextDialogue(0,()=>
            startTutorialButton.enabled = true
          );
        });
    }
    public void FadeOutCTA()
    {
        startTutorialButton.enabled = false;
        LeanTween.alphaCanvas(canvasGroup, 0, fadeOutDuration);
    }
}
