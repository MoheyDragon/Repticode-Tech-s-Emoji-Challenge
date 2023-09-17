using UnityEngine;
using System;
public class EmojiObject : MonoBehaviour
{
    [SerializeField] GameObject emojiName3DText;
    [SerializeField] protected MeshRenderer meshRenderer;
    public void SetTexture(string propertyName,Texture texture)
    {
        meshRenderer.material.SetTexture(propertyName,texture);
    }
    public void MoveEmoji(Transform placeInShelves,float time,Action callBack)
    {
        ShowEmoji(true);
        transform.SetParent(placeInShelves);
        LeanTween.scale(gameObject, Vector3.one, 0).setEaseInOutSine();
        LeanTween.moveLocal(gameObject, Vector3.zero, time).setEaseInOutSine().setOnComplete(callBack);
        transform.localRotation = Quaternion.identity;
    }
    public void ShowEmoji(bool shown)
    {
        gameObject.SetActive(shown);
    }
    public void ChangeNameVisibility(bool shown)
    {
        emojiName3DText.SetActive(shown);
    }
    public void FadeOutEmojiFace(string faceAlphaPropertyName,float duration)
    {
        if (meshRenderer.material.GetFloat(faceAlphaPropertyName)==1)
            return;
        meshRenderer.material.color = ProjectCustomColors.Instance.Red;
        LeanTween.value(gameObject, 0, 1, duration).setEaseInOutSine().setOnUpdate((float value) => meshRenderer.material.SetFloat(faceAlphaPropertyName, value));
    }
    public void Hide(string alphaPropertyName,float duration)
    {
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        LeanTween.value(gameObject, 1, 0, duration).setEaseInOutSine().setOnUpdate((float value) => meshRenderer.material.SetFloat(alphaPropertyName, value));
    }
}
