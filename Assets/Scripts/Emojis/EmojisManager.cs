using UnityEngine;
using System.Collections;
using System;
public class EmojisManager : Singleton<EmojisManager>
{
    [SerializeField] EmojiObject[] emojis;
    [SerializeField] ResultEmoji[] resultEmojis;
    [SerializeField] Texture[] emojisTextures;
    readonly string texturePropertyName = "_main";
    readonly string alphaPropertyName = "_alpha";
    readonly string faceAlphaPropertyName = "_faceAlpha";
    readonly string tilingPropertyName = "_tiling";
    readonly string offsetPropertyName = "_offset";
    int emojisCount;
    [Space]
    [Header("Emoji Entry speed values")]
    [SerializeField] float emojisSpeed = 5;
    [SerializeField] float waitTimeBetweenEmojisEntry = 0.05f;
    private void Start()
    {
        Setup();
    }
    private void Setup()
    {
        emojiDisplayOffset = new Vector3(-0.0318000019f, -0.0201999992f, -0.000500000024f);
        emojisCount = emojis.Length;
        introEmojisCount = -1;
        waitTimeBetweenEmojisEntryInSeconds = Seconds.GetCachedWaitForSeconds(waitTimeBetweenEmojisEntry);
        for (int i = 0; i < emojisCount; i++)
        {
            emojis[i].SetTexture(texturePropertyName,emojisTextures[i]);
        }
    }
    [Space]
    [SerializeField] Material transparentMaterial;
    [SerializeField] float fadingDuration;
    public float FadingDuration => fadingDuration;
    WaitForSeconds waitTimeBetweenEmojisEntryInSeconds;
    Action callBack;
    public void Intro(Action callback)
    {
        callBack = callback;
        _MoveSingleEmoji();
    }
    int introEmojisCount;
    private void _MoveSingleEmoji()
    {
        StartCoroutine(CO_WaitBetweenEmojisEntry());
    }
    IEnumerator CO_WaitBetweenEmojisEntry()
    {
        yield return waitTimeBetweenEmojisEntryInSeconds;
        introEmojisCount++;
        if (introEmojisCount == emojisCount)
        {
            introEmojisCount = -1;
            callBack?.Invoke();
            yield break;
        }
        Transform placeInShelves = ShelvesManager.Instance.GetPlaceInShelves(introEmojisCount);
        float distance = Vector3.Distance(emojis[introEmojisCount].transform.position, placeInShelves.position);
        float time = distance / emojisSpeed;
        emojis[introEmojisCount].MoveEmoji(placeInShelves, time, _MoveSingleEmoji);
    }
    public void ShowEmojisNamesOnTop(Action callBack)
    {
        StartCoroutine(ShowNamesInSequence(callBack));
    }
    IEnumerator ShowNamesInSequence(Action callBack)
    {
        DisplayNamesOfEmojisInOneShelve(0);
        yield return Seconds.GetCachedWaitForSeconds(1);
        DisplayNamesOfEmojisInOneShelve(1);
        yield return Seconds.GetCachedWaitForSeconds(1);
        DisplayNamesOfEmojisInOneShelve(2);
        yield return Seconds.GetCachedWaitForSeconds(1);
        callBack();
    }
    private void DisplayNamesOfEmojisInOneShelve(int startIndex)
    {
        for (int i = startIndex; i < emojisCount; i+=3)
            emojis[i].ChangeNameVisibility(true);
    }
    public void HideEmojisNamesOnTopAfterDelay(Action callBack=null)
    {
        StartCoroutine(CO_HideEmojisNamesOnTop(callBack));
    }
    IEnumerator CO_HideEmojisNamesOnTop(Action callBack)
    {
        yield return Seconds.GetCachedWaitForSeconds(0);
        foreach (EmojiObject emoji in emojis)
            emoji.ChangeNameVisibility(false);

        yield return Seconds.GetCachedWaitForSeconds(0);
        callBack?.Invoke();
    }
    public void FadeOutEmojisTextureAfterDelay(Action callBack=null)
    {
        StartCoroutine(CO_FadeOutEmojisTexture(callBack));
    }
    readonly float fadeEmojisDelay=5;
    IEnumerator CO_FadeOutEmojisTexture(Action callBack)
    {
        yield return Seconds.GetCachedWaitForSeconds(fadeEmojisDelay);

        foreach (EmojiObject emoji in emojis)
            emoji.FadeOutEmojiFace(faceAlphaPropertyName, fadingDuration);
        yield return Seconds.GetCachedWaitForSeconds(1);
        callBack?.Invoke();

    }
    public void FadeOutResultEmojis()
    {
        foreach (ResultEmoji emoji in resultEmojis)
            emoji.FadeOut(faceAlphaPropertyName,fadingDuration);
    }
    public void FadeOutSingleEmoji(int index)
    {
        resultEmojis[index].FadeOut(faceAlphaPropertyName, fadingDuration);
    }
    Vector3 emojiDisplayOffset;
    public void FadeInSingleResultEmoji(int index,Action callBack)
    {
        resultEmojis[index].ShowResult(index, fadingDuration, texturePropertyName,faceAlphaPropertyName,tilingPropertyName,offsetPropertyName,emojiDisplayOffset,callBack);
    }
    public void ShowPerformanceOfSingleEmoji(int index, Action callBack)
    {
        resultEmojis[index].ShowPerfromanceBars(index, fadingDuration,
            UserTestWebSimulatorManager.Instance.GetColorByTaskIndex(index), () => callBack?.Invoke());
    }
    public void HideEmojis()
    {
        foreach (EmojiObject emoji in emojis)
        {
            emoji.Hide(alphaPropertyName,fadingDuration);
        }

    }
    public void SetResultEmojisArabicBarNames()
    {
        foreach (ResultEmoji resultEmoji in resultEmojis)
        {
            resultEmoji.SetBarsNamesTexts();
        }
    }
}
