using UnityEngine;

public class ScaledDialogueManager : DialogueManager
{
    [SerializeField] float dialogueYPos=0;
    [SerializeField] float dialougueStartYPos=-1000;
    float fadingDuration=0.5f;
    public override void FadeInDialouge()
    {
        base.FadeInDialouge();
        LeanTween.moveLocalY(textMesh.gameObject, dialogueYPos, fadingDuration).setEaseInOutSine();
        LeanTween.scale(textMesh.gameObject, Vector3.one, fadingDuration).setEaseInOutSine();
    }
    public override void HidePreviousDialogue()
    {
        base.HidePreviousDialogue();
        LeanTween.moveLocalY(textMesh.gameObject, dialougueStartYPos, 0).setEaseInOutSine();
        LeanTween.scale(textMesh.gameObject, Vector3.zero, 0).setEaseInOutSine();
    }
}
