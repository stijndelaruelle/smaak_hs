using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public delegate void SceneLoaderDelegate();

    [SerializeField]
    private ImageFader m_ImageFader;

    //Cache
    private string m_SceneName;
    private LoadSceneMode m_Mode;
    private bool m_AutoActivate = false;
    private bool m_SetAsMainScene = false;
    private bool m_IsLoadingWithFade = false;

    //Async handles
    private AsyncOperation m_AsyncLoadProgress = null;
    private AsyncOperation m_AsyncUnloadProgress = null;
    private bool m_IsLoaded; //Avoid multiple calls

    //Events
    public event SceneLoaderDelegate SceneLoadedEvent;
    public event SceneLoaderDelegate SceneActivatedEvent;


    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneActivated; //Actually get's called when a scene get's activated. Not loaded!
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        //Make sure the screen is transparant
        if (m_ImageFader!= null)
            m_ImageFader.SetAlphaMin();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneActivated;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Update()
    {
        //Async loading can only be done up until 90%.
        if (m_AsyncLoadProgress != null && m_IsLoaded == false)
        {
            float progress = GetProgress();

            if (progress >= 1.0f)
            {
                SceneLoaded();
            }
        }
    }


    public void LoadScene(string sceneName, LoadSceneMode mode, bool autoActivate, bool setAsMainScene, bool fade)
    {
        //Only load scenes that aren't loaded yet
        if (IsSceneLoaded(sceneName))
            return;

        //Don't allow 2 scenes to be loaded simultaniously (support can be added later)
        if (m_AsyncLoadProgress != null)
        {
            Debug.LogWarning("Trying to load 2 scenes simultainously!");
            return;
        }

        //Cache
        m_SceneName = sceneName;
        m_Mode = mode;
        m_AutoActivate = autoActivate;
        m_SetAsMainScene = setAsMainScene;
        m_IsLoadingWithFade = fade;
        m_IsLoaded = false;

        if (m_ImageFader != null && fade == true && autoActivate == true)
        {
            m_ImageFader.FadeIn(OnFadeInCompleteAutoActivate);
        }
        else
        {
            OnFadeInCompleteAutoActivate();
        }
    }

    public void UnloadActiveScene()
    {
        //Don't allow 2 scenes to be unloaded simultaniously (support can be added later)
        if (m_AsyncUnloadProgress != null)
        {
            Debug.LogWarning("Trying to unload 2 scenes simultainously!");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        m_AsyncUnloadProgress = SceneManager.UnloadSceneAsync(scene);

        //Activate the scene below it.
        int openSceneCount = SceneManager.sceneCount;
        if (openSceneCount >= 2)
        {
            for (int i = openSceneCount - 2; i >= 0; --i)
            {
                Scene newActiveScene = SceneManager.GetSceneAt(openSceneCount - 2);
                if (newActiveScene.isLoaded)
                    SceneManager.SetActiveScene(newActiveScene);
            }
        }
    }

    public void ActivateScene()
    {
        //When the scene doesn't auto activate, we can do it here! (trough a button or at the end of a fade?)
        if (m_IsLoadingWithFade)
        {
            m_ImageFader.FadeIn(OnFadeInCompleteManualActivate);
        }
        else
        {
            OnFadeInCompleteManualActivate();
        }
    }

    //Fader
    public void ShowFader()
    {
        if (m_ImageFader != null)
            m_ImageFader.SetAlphaMax();
    }

    public void HideFader()
    {
        if (m_ImageFader != null)
            m_ImageFader.SetAlphaMin();
    }

    //Accessors
    public float GetProgress()
    {
        if (m_AsyncLoadProgress == null)
            return 0.0f;

        return m_AsyncLoadProgress.progress / 0.9f; //Async loading only get's up to 0.9 (so let's return it as if it's 1.0)
    }

    public bool IsSceneLoaded(string sceneName)
    {
        int openSceneCount = SceneManager.sceneCount;

        for (int i = 0; i < openSceneCount; ++i)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == sceneName)
                return true;
        }

        return false;
    }

    public int GetNumberOfLoadedScenes()
    {
        return SceneManager.sceneCount;
    }

    //Callbacks
    private void OnFadeInCompleteAutoActivate()
    {
        //We always load aSync!
        m_AsyncLoadProgress = SceneManager.LoadSceneAsync(m_SceneName, m_Mode);
        m_AsyncLoadProgress.allowSceneActivation = m_AutoActivate;

        if (m_IsLoadingWithFade && m_ImageFader != null)
        {
            m_ImageFader.FadeOut(); //No callback needed, we just let it happen
        }
    }

    private void OnFadeInCompleteManualActivate()
    {
        if (m_AsyncLoadProgress != null)
            m_AsyncLoadProgress.allowSceneActivation = true;

        if (m_IsLoadingWithFade && m_ImageFader != null)
        {
            m_ImageFader.FadeOut(); //No callback needed, we just let it happen
        }
    }

    private void SceneLoaded()
    {
        //Called when the scene is loaded
        if (SceneLoadedEvent != null)
            SceneLoadedEvent();

        m_IsLoaded = true;
    }

    private void OnSceneActivated(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name == m_SceneName && loadMode == m_Mode)
        {
            StartCoroutine(OnSceneActivatedFrameDelayRoutine(scene));
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        //Allow another scene to be unloaded
        m_AsyncUnloadProgress = null;
    }

    private IEnumerator OnSceneActivatedFrameDelayRoutine(Scene scene)
    {
        //https://forum.unity3d.com/threads/scenemanager-setactivescene-does-not-work-solved-workarounds.381705/
        yield return new WaitForEndOfFrame();

        if (m_SetAsMainScene)
        {
            SceneManager.SetActiveScene(scene);
        }

        m_IsLoaded = false;
        m_AsyncLoadProgress = null;

        if (SceneActivatedEvent != null)
            SceneActivatedEvent();
    }
}