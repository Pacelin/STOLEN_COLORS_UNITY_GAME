using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UniOwl.Localization
{
    public class LocaleSwitcher : MonoBehaviour
    {
        private int _currentLocaleIndex;
        
        private async void Start()
        {
            await LocalizationSettings.InitializationOperation.Task;

            _currentLocaleIndex =
                LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        }

        public void NextLocale()
        {
            _currentLocaleIndex = MathUtils.NextLoop(_currentLocaleIndex, LocalizationSettings.AvailableLocales.Locales.Count);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_currentLocaleIndex];
        }
        
        public void PreviousLocale()
        {
            _currentLocaleIndex = MathUtils.PreviousLoop(_currentLocaleIndex, LocalizationSettings.AvailableLocales.Locales.Count);
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_currentLocaleIndex];
        }
    }
}
