#define ANALYTICS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public class AnalyticsManager
{
    //https://docs.unity3d.com/Manual/UnityAnalyticsStandardEvents.html

    //Proression events
    public static void LevelStartEvent(string levelName, bool firstTime)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("first_time", firstTime);
        customParams.Add("debug", isDebug);

        //A new level was loaded
        AnalyticsResult result = AnalyticsEvent.LevelStart(levelName, customParams);
#endif
    }

    public static void LevelCompleteEvent(string levelName, bool firstTime, int tries, double currentTime, double totalTime)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("first_time", firstTime);
        customParams.Add("tries", tries);
        customParams.Add("current_time", currentTime);
        customParams.Add("total_time", totalTime);

        customParams.Add("Debug", isDebug);

        //A new level was loaded
        AnalyticsEvent.LevelComplete(levelName, customParams);
#endif
    }

    public static void LevelFailEvent(string levelName, int tries, double currentTime, bool deathByEnemy, bool deathByQuiz, bool resetByUI, bool resetByEndUI)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("tries", tries);
        customParams.Add("current_time", currentTime);

        customParams.Add("death_by_enemy", deathByEnemy);
        customParams.Add("death_by_quiz", deathByQuiz);
        customParams.Add("reset_by_ui", resetByUI);
        customParams.Add("reset_by_endui", resetByEndUI);

        customParams.Add("debug", isDebug);

        //A new level was loaded
        AnalyticsEvent.LevelFail(levelName, customParams);
#endif
    }

    public static void LevelQuitEvent(string levelName, int tries, double currentTime, bool quitByVictory, bool quitByUI)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("tries", tries);
        customParams.Add("current_time", currentTime);

        customParams.Add("quit_by_victory", quitByVictory);
        customParams.Add("quit_by_ui", quitByUI);

        customParams.Add("debug", isDebug);

        //A new level was loaded
        AnalyticsEvent.LevelQuit(levelName, customParams);
#endif
    }

    //Quiz events
    public static void QuestionAnswererdEvent(string levelName, string question, string answer, int timesAnswered, bool correct, double time)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();

        customParams.Add("level_name", levelName);
        customParams.Add("question", question);
        customParams.Add("answer", answer);

        customParams.Add("times_answered", timesAnswered);
        customParams.Add("correct", correct);
        customParams.Add("time", time);

        customParams.Add("debug", isDebug);

        AnalyticsEvent.Custom("question_answered", customParams);
#endif
    }

    //Video events
    public static void StartVideoEvent(string videoName, bool watchInMenu, bool watchInGame)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("video_name", videoName);
        customParams.Add("watch_in_menu", watchInMenu);
        customParams.Add("watch_in_game", watchInGame);

        customParams.Add("debug", isDebug);

        AnalyticsEvent.Custom("video_started", customParams);
#endif
    }

    public static void StopVideoEvent(string videoName, bool finished, double timePlayed)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("video_name", videoName);
        customParams.Add("finished", finished);
        customParams.Add("time_played", timePlayed);

        customParams.Add("debug", isDebug);

        AnalyticsEvent.Custom("video_stopped", customParams);
#endif
    }

    //Hints event
    public static void HintEnabledEvent(string levelName, int tries, double currentTime, double totalTime)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("level_name", levelName);
        customParams.Add("tries", tries);
        customParams.Add("current_time", currentTime);
        customParams.Add("total_time", totalTime);

        customParams.Add("Debug", isDebug);

        AnalyticsEvent.Custom("hints_enabled", customParams);
#endif
    }

    //Character creation event
    public static void CharacterCreationEvent(bool isMale, bool isFemale, string skinColor, string extraColor, double time)
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("male", isMale);
        customParams.Add("female", isFemale);
        customParams.Add("skin_color", skinColor);
        customParams.Add("extra_color", extraColor);

        customParams.Add("time", time);

        customParams.Add("Debug", isDebug);

        AnalyticsEvent.Custom("character_creation", customParams);
#endif
    }

    //Delete save game event
    public static void DeleteSaveGameEvent()
    {
#if ANALYTICS
        bool isDebug = (Application.isEditor == true);

        Dictionary<string, object> customParams = new Dictionary<string, object>();
        customParams.Add("Debug", isDebug);

        AnalyticsEvent.Custom("delete_savegame", customParams);
#endif
    }
}
