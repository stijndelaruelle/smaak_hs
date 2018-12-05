using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Could be an interface and have a "ResetListener" instance added to every GameObject to call the "Reset" function instead of every object listening for themselves.
public abstract class LocalizedObject : MonoBehaviour
{
    protected virtual void Start()
    {
        LocalizationManager localizationManager = LocalizationManager.Instance;

        if (localizationManager != null)
        {
            localizationManager.LanguageChangedEvent += OnLanguageChanged;
            localizationManager.TokenChangedEvent += OnTokenChanged;
        }
    }

    protected virtual void OnDestroy()
    {
        LocalizationManager localizationManager = LocalizationManager.Instance;

        if (localizationManager != null)
        {
            localizationManager.LanguageChangedEvent -= OnLanguageChanged;
            localizationManager.TokenChangedEvent -= OnTokenChanged;
        }
    }

    protected abstract void OnLanguageChanged(LocalizationManager.Language language); //Forced override
    protected virtual void OnTokenChanged(string key) { }
}
