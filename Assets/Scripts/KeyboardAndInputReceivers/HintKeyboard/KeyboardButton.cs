using UnityEngine;
using UnityEngine.UI;

public class KeyboardButton:MonoBehaviour
{
    Image keyImage;
    private void Start()
    {
        keyImage = GetComponent<Image>();
    }
    public void HilightButton()
    {
        if(!LeanTween.isTweening(keyImage.rectTransform))
        LeanTween.color(keyImage.rectTransform, ProjectCustomColors.Instance.Red, HintsKeyboard.Instance.GetHighlightSpeed).setEaseInOutSine().setLoopPingPong()
            .setRepeat(-1);
    }
    public void StopHighlighting()
    {
        keyImage.color = Color.white;
        LeanTween.cancel(keyImage.rectTransform);
    }
}