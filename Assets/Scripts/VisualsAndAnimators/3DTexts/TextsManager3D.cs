using UnityEngine;
using System;
public class TextsManager3D : Singleton<TextsManager3D>
{
    TextGroup[] TextsGroups;
    [SerializeField] float midRotateY=0, endRotateY=-179;
    [Space]
    [SerializeField] Color baseColor;
    [SerializeField] Color fadedColor;
    [SerializeField] Material textsMaterial;
    private Vector3 midVector;
    private Vector3 endVector;
    public Vector3 GetMidVector => midVector;
    public Vector3 GetEndVector => endVector;

    protected override void Awake()
    {
        base.Awake();
        int textsGroups = 4;
        TextsGroups = new TextGroup[textsGroups];
        for (int i = 0; i < textsGroups; i++)
            TextsGroups[i] = transform.GetChild(i).GetComponent<TextGroup>();
    }
    private void Start()
    {
        LanguageManager.Instance.OnLangaugeSelected += OnLangaugeSelected;
        midVector = new Vector3(0, midRotateY, 0);
        endVector = new Vector3(0, endRotateY, 0);
        FadeInTexts();
    }
    public void MoveTexts(int index,Action callBack=null)
    {
        if (index>0)
            StopPreviousTextsMovement(index-1);

        TextsGroups[index].EnterPlay(callBack);
    }
    [SerializeField] GameObject []englishTexts;
    [SerializeField] GameObject []arabicTexts;
    public void SetTextsLangauges(LanguageManager.Language language)
    {
        if (language==LanguageManager.Language.Arabic)
        {
            foreach (GameObject text in englishTexts)
                text.SetActive(false);

            foreach (GameObject text in arabicTexts)
                text.SetActive(true);
            TextsGroups[2] = transform.GetChild(4).GetComponent<TextGroup>();
            TextsGroups[3] = transform.GetChild(5).GetComponent<TextGroup>();
        }
    }
    public void OnLangaugeSelected()
    {
        SetTextsLangauges(LanguageManager.Instance.GetCurrentLanguage);
    }
    public void FadeOutTexts()
    {
        textsMaterial.SetColor("_BASE_COLOR", fadedColor);
        textsMaterial.SetFloat("_REFLECTIONS_WEIGHT", 0);
    }
    public void FadeInTexts()
    {
        textsMaterial.SetColor("_BASE_COLOR", baseColor);
        textsMaterial.SetFloat("_REFLECTIONS_WEIGHT", 1);
    }
    private void StopPreviousTextsMovement(int index)
    {
        TextsGroups[index].Stop();
    }
}
