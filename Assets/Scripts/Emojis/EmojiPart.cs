using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiPart : MonoBehaviour
{
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void FillPart(float fillDuration)
    {
        LeanTween.value(gameObject,0,1, fillDuration).setEaseInOutSine().setOnUpdate((float value)=> image.fillAmount= value);
    }
    public void Setup(Sprite taskSprite)
    {
        image.sprite = taskSprite;
    }
    public void Reset()
    {
        image.fillAmount = 0;
    }
}
