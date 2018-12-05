using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    private string m_SceneName;
    private SceneLoader m_SceneLoader;

    /*
    [Space(5)]
    [Header("Fader")]
    [Space(10)]
    [SerializeField]
    private ImageFader m_ImageFader;
    */

    [Space(5)]
    [Header("Panels")]
    [Space(10)]
    [SerializeField]
    private GameObject m_LoadingPanel;

    [SerializeField]
    private GameObject m_LoadedPanel;
    private bool m_HasSceneLoaded = false;
    private bool m_AreWeActivated = false;

    private void Start()
    {
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null)
        {
            Debug.LogError("No LevelManager found to load!");
            return;
        }

        LevelDataDefinition levelData = levelManager.CurrentLevel;

        if (levelData == null)
        {
            Debug.LogError("No levelData found to load!");
            return;
        }

        m_SceneName = levelData.GetSceneName();

        m_SceneLoader = LevelManager.Instance.SceneLoader;
        m_SceneLoader.SceneLoadedEvent += OnSceneLoaded;
        m_SceneLoader.SceneActivatedEvent += OnSceneActivated;

        //Make sure the screen turns black
        //m_ImageFader.SetAlphaMax();
    }

    private void OnDestroy()
    {
        if (m_SceneLoader != null)
        {
            m_SceneLoader.SceneLoadedEvent -= OnSceneLoaded;
            m_SceneLoader.SceneActivatedEvent -= OnSceneActivated;
        }
    }



    private void Update()
    {
        //Button bypass
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            ActivateScene();
        }
    }

    //Button callback
    public void ActivateScene()
    {
        if (m_AreWeActivated == true && m_HasSceneLoaded == true)
            OnFadeInComplete();
            //m_ImageFader.FadeIn(OnFadeInComplete);
    }

    //SceneLoader callbacks
    private void OnSceneLoaded()
    {
        ShowLoadedPanel();
        m_HasSceneLoaded = true;
    }

    private void OnSceneActivated()
    {
        //As this callback comes with 1 frame delay, we will receive our own activate callback (as start has already been called!)
        //The second callback is the one of the level we want to load, so ignore that one
        if (m_AreWeActivated)
            return;

        ShowLoadingPanel();
        m_SceneLoader.LoadScene(m_SceneName, LoadSceneMode.Additive, false, true, true);

        //Fade out
        //m_ImageFader.FadeOut();

        m_AreWeActivated = true;
    }

    //Fader callback
    private void OnFadeInComplete()
    {
        //Unload the loading scene & show the loaded scene
        m_SceneLoader.UnloadActiveScene();
        m_SceneLoader.ActivateScene();
    }

    //Utility
    private void ShowLoadingPanel()
    {
        m_LoadingPanel.SetActive(true);
        m_LoadedPanel.SetActive(false);
    }

    private void ShowLoadedPanel()
    {
        m_LoadingPanel.SetActive(false);
        m_LoadedPanel.SetActive(true);
    }
}
