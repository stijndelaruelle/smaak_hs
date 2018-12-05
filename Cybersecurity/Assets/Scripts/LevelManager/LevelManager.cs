using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public delegate void LevelManagerDelegate();

    [SerializeField]
    private SceneLoader m_SceneLoader;
    public SceneLoader SceneLoader
    {
        get { return m_SceneLoader; }
    }

    [Header("Special Scenes")]
    [SerializeField]
    private string m_SplashScene;

    [SerializeField]
    private string m_MainMenuSceneName;

    [SerializeField]
    private string m_OptionMenuSceneName;

    [SerializeField]
    [Tooltip("Every chapter now has their own loading screen, if we can't find any fall back to this one")]
    private string m_FallBackLoadingSceneName;

    //Current status
    private CampaignDataDefinition  m_CurrentCampaign;
    public CampaignDataDefinition   CurrentCampaign
    {
        get { return m_CurrentCampaign; }
        set { m_CurrentCampaign = value; }
    }

    private ChapterDataDefinition   m_CurrentChapter;
    public ChapterDataDefinition    CurrentChapter
    {
        get { return m_CurrentChapter; }
        set { m_CurrentChapter = value; }
    }

    private LevelDataDefinition     m_CurrentLevel;
    public LevelDataDefinition      CurrentLevel
    {
        get { return m_CurrentLevel; }
        set { m_CurrentLevel = value; }
    }

    private void Start()
    {
        //Load the splash scene if we are the only ones that are loaded.
        if (m_SceneLoader.GetNumberOfLoadedScenes() == 1)
            LoadMainMenuScene(); //LoadSplashScene() //We skip the splash scene as we now use the standard Unity preloaders.
    }

    private void Update()
    {
        //Debug
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMainMenuScene();
            return;
        }
    }

    public void LoadSplashScene()
    {
        //This is not that great, but I have the feeling this will have to be rewritten in the near future anyways
        if (m_SceneLoader.GetNumberOfLoadedScenes() > 1)
        {
            m_SceneLoader.UnloadActiveScene();
        }

        m_SceneLoader.ShowFader();
        m_SceneLoader.LoadScene(m_SplashScene, UnityEngine.SceneManagement.LoadSceneMode.Additive, true, true, true);
    }

    public void LoadMainMenuScene()
    {
        m_SceneLoader.UnloadActiveScene();
        m_SceneLoader.LoadScene(m_MainMenuSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, true, true, true);
    }

    public void LoadOptionMenuScene()
    {
        m_SceneLoader.LoadScene(m_OptionMenuSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, true, true, false);
    }

    private void LoadLoadingSceen(LevelDataDefinition levelData)
    {
        if (levelData == null)
        {
            Debug.Log("Tried loading an invalid level!");
            LoadMainMenuScene();
            return;
        }

        //We load the loading screen, it will in turn load the actual level (and show progress etc...)
        m_SceneLoader.UnloadActiveScene();

        //Load the loading screen
        string loadingScreenName = m_FallBackLoadingSceneName;

        if (m_CurrentChapter.LoadingScreenSceneName != null && m_CurrentChapter.LoadingScreenSceneName != "")
            loadingScreenName = m_CurrentChapter.LoadingScreenSceneName;

        m_SceneLoader.LoadScene(loadingScreenName, UnityEngine.SceneManagement.LoadSceneMode.Additive, true, true, true);
    }

    public void LoadLevel(LevelDataDefinition levelData)
    {
        if (IsLevelDataValid(levelData) == false)
        {
            Debug.LogWarning("Tried loading a level that is not part of our current campaign and/or chapter!");
            return;
        }

        m_CurrentLevel = levelData;
        LoadLoadingSceen(levelData);
    }

    public void LoadNextLevel()
    {
        if (m_CurrentChapter == null)
            return;

        //Figure out what ID the current level has
        int levelID = m_CurrentChapter.GetLevelID(m_CurrentLevel);

        if (levelID < 0)
        {
            LoadMainMenuScene();
            return;
        }

        //Figure out what the data for the next level is
        LevelDataDefinition nextLevelData = m_CurrentChapter.GetLevel(levelID + 1);

        if (nextLevelData == null)
        {
            LoadMainMenuScene();
            return;
        }

        LoadLevel(nextLevelData);
    }

    public void CompleteCurrentLevel()
    {
        //Save game
        if (m_CurrentLevel != null)
            m_CurrentLevel.CompleteLevel();
    }

    private bool IsLevelDataValid(LevelDataDefinition levelData)
    {
        //Check if the levelData matches the Chapter & Campaign
        if (m_CurrentChapter.HasLevel(levelData) == false)
            return false;

        if (m_CurrentCampaign.HasChapter(m_CurrentChapter) == false)
            return false;

        return true;
    }
}