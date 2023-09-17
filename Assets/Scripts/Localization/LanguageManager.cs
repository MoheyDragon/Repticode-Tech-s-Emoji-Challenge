using UnityEngine;
using System;
public class LanguageManager : Singleton<LanguageManager>
{
    public enum Language {English,Arabic};
    Language CurrentLanguage;
    [SerializeField] Language defaultLanguage;
    [SerializeField] FontSettings[] fontSettings;
    #region Public calls
    public Action OnLangaugeSelected;
    public void SetLanguage(int languageIndex)
    {
        CurrentLanguage = (Language)languageIndex;
        OnLangaugeSelected?.Invoke();
    }
    public Language GetCurrentLanguage => CurrentLanguage;
    public int GetCurrentLanguageIndex => (int)CurrentLanguage;

    public FontSettings GetFontSettingsBasedOnLanguageSelected()
    {
        return fontSettings[GetCurrentLanguageIndex];
    }
    #endregion
}
