using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AppData
{
    public static int GetCurrentLvl()
    {
        return PlayerPrefs.GetInt(Constants.LevelID, 1);
    }
    public static void SetCurrentLvl(int lvlID)
    {
        PlayerPrefs.SetInt(Constants.LevelID, lvlID);
    }

    public static int GetCurrentLvlIndex()
    {
        return PlayerPrefs.GetInt(Constants.LevelInd, 0);
    }
    public static void SetCurrentLvlIndex(int lvlInd)
    {
        PlayerPrefs.SetInt(Constants.LevelInd, lvlInd);
    }

    public static int GetMaxScore()
    {
        return PlayerPrefs.GetInt(Constants.MaxScore, 0);
    }
    public static void SetMaxScore(int maxScore)
    {
        PlayerPrefs.SetInt(Constants.MaxScore, maxScore);
    }
    public static int GetDifficulty()
    {
        return PlayerPrefs.GetInt(Constants.Difficulty, 0);
    }
    public static void SetDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt(Constants.Difficulty, difficulty);
    }


    //0 true ..  //1 false
    public static int GetSoundInfo()
    {
        return PlayerPrefs.GetInt(Constants.Sound, 0);
    }
    public static void SetSoundInfo()
    {
        PlayerPrefs.SetInt(Constants.Sound, GetSoundInfo() == 0 ? 1 : 0);
        AudioManager.Instance.canPlay = GetSoundInfo() == 0 ? true : false;
    }

    public static int GetHapticInfo()
    {
        return PlayerPrefs.GetInt(Constants.Haptic, 0);
    }

    public static void SetHapticInfo()
    {
        PlayerPrefs.SetInt(Constants.Haptic, GetHapticInfo() == 0 ? 1 : 0);
        GameManager.Instance.canHaptic = !GameManager.Instance.canHaptic;
    }

    public static bool checkFirstRun()
    {
        bool state = PlayerPrefs.GetInt(Constants.FirstRun, 1) == 0 ? false : true;
        return state;
    }

    public static void DisableFirstRun()
    {
        PlayerPrefs.SetInt(Constants.FirstRun, 0);
    }

    public static bool GetDarkBackgroundState()
    {
        bool state = PlayerPrefs.GetInt(Constants.DarkBackgroundState, 0) == 0 ? false : true;
        return state;
    }

    public static void SetDarkBackgroundState()
    {
        PlayerPrefs.SetInt(Constants.DarkBackgroundState, UiManager.Instance.darkBackgroundEnabled ? 1 : 0);
    }
}
