using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { private set; get; }

    [Header("Ingame UI")]
    public Text currentScore;
    public Text maxScore;
    public Text currentLevel;
    public Text nextLevel;
    public Slider progressBar;
    public Image progressBarBackgroundImage;
    public Material RoadMaterial;
    public Material deathColliderMaterial;
    public Color darkModeBackgroundColor;
    public Color darkModeRoadColor;

    [Header("Game Over UI")]
    public Text gameOverMaxTxt;
    public Text gameOverScoreTxt;
    public Text lvlCompleteTxt;

    [Header("Tap To Play UI")]
    public GameObject tapToPlayPanel;
    public Text tapToPlayMaxTxt;
    public Text tapToPlayScoreTxt;
    
    [Space(10)]
    public GameObject lvlCompletePanel;

    [Header("Sound & Haptic")]
    public Sprite soundOn;
    public Sprite soundOff;

    public Sprite vibrationOn;
    public Sprite vibrationOff;

    public GameObject soundBtn;
    public GameObject vibrationBtn;

    public bool darkBackgroundEnabled { private set; get; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        InitialiseUi();
        RestoreUIData();

        darkBackgroundEnabled = !AppData.GetDarkBackgroundState();
        changeBackground();

    }

    public void changeBackground()
    {
        if (!darkBackgroundEnabled)
        {
            Camera.main.backgroundColor = darkModeBackgroundColor;
            currentScore.color = Color.white;
            maxScore.color = Color.white;
            progressBarBackgroundImage.color = Color.white;
            //RoadMaterial.color = Color.white;
            deathColliderMaterial.color = Color.white;

            darkBackgroundEnabled = true;
        }
        else
        {
            Camera.main.backgroundColor = Color.white;
            currentScore.color = Color.black;
            maxScore.color = Color.black;
            progressBarBackgroundImage.color = darkModeBackgroundColor;
            //RoadMaterial.color = darkModeRoadColor;
            deathColliderMaterial.color = darkModeRoadColor;

            darkBackgroundEnabled = false;
        }

        AppData.SetDarkBackgroundState();
    }

    public void OnSoundBtnClick()
    {
        if (AppData.GetSoundInfo() == 0)
        {
            soundBtn.GetComponent<Image>().sprite = soundOff;
        }
        else
        {
            soundBtn.GetComponent<Image>().sprite = soundOn;
        }
        AppData.SetSoundInfo();
    }


    public void OnVibrationBtnClick()
    {
        if (AppData.GetHapticInfo() == 0)
        {
            vibrationBtn.GetComponent<Image>().sprite = vibrationOff;
        }
        else
        {
            vibrationBtn.GetComponent<Image>().sprite = vibrationOn;
            GameManager.Instance.DoHaptic(2);
        }
        AppData.SetHapticInfo();
    }


    public void InitialiseUi()
    {
        tapToPlayMaxTxt.text = "Best : " + AppData.GetMaxScore().ToString();
        tapToPlayScoreTxt.text = "";
        maxScore.text = AppData.GetMaxScore().ToString();
        currentLevel.text = AppData.GetCurrentLvl().ToString();
        nextLevel.text = (AppData.GetCurrentLvl() + 1).ToString();
        progressBar.value = 0;
        currentScore.text = 0.ToString();
    }
    /// <summary>
    /// Refreshes UI on player progress
    /// </summary>
    public void RefreshUi()
    {
        if (GameManager.Instance.currentScore > AppData.GetMaxScore())
        {
            AppData.SetMaxScore(GameManager.Instance.currentScore);
            maxScore.text = AppData.GetMaxScore().ToString();
        }

        currentScore.text = GameManager.Instance.currentScore.ToString();
        lvlCompleteTxt.text = "Level " + (AppData.GetCurrentLvl() - 1).ToString() + " Completed!";
        if (GameManager.Instance.mergedBallsCount == 0)
        {
            progressBar.value = 0;
        }
        else
        {
            progressBar.value = (float)GameManager.Instance.mergedBallsCount / GameManager.Instance.ballsCountToCompleteLvl;
        }
    }
    /// <summary>
    /// Resets UI before next level
    /// </summary>
    public void ResetUI()
    {

        maxScore.text = AppData.GetMaxScore().ToString();
        currentLevel.text = AppData.GetCurrentLvl().ToString();
        nextLevel.text = (AppData.GetCurrentLvl() + 1).ToString();
        progressBar.value = 0;
    }

    public void LevelComplete()
    {
        if (GameManager.Instance.currentScore > AppData.GetMaxScore())
        {
            AppData.SetMaxScore(GameManager.Instance.currentScore);
            maxScore.text = AppData.GetMaxScore().ToString();
        }

    }
    public void LoadNextLevel()
    {

        lvlCompletePanel.SetActive(false);
        tapToPlayPanel.SetActive(true);
        tapToPlayMaxTxt.text = "Best: " + AppData.GetMaxScore().ToString();
        tapToPlayScoreTxt.text = GameManager.Instance.currentScore.ToString();
    }

    public void Restart()
    {
        currentScore.text = 0.ToString();
        tapToPlayMaxTxt.text = "Best: " + AppData.GetMaxScore().ToString();
        tapToPlayScoreTxt.text = "";
    }

    public void LevelFailed()
    {

        if (GameManager.Instance.currentScore > AppData.GetMaxScore())
        {
            AppData.SetMaxScore(GameManager.Instance.currentScore);
            maxScore.text = AppData.GetMaxScore().ToString();
        }
        gameOverMaxTxt.text = "Best: " + AppData.GetMaxScore().ToString();
        gameOverScoreTxt.text = GameManager.Instance.currentScore.ToString();

    }

    /// <summary>
    /// Restores UI data from playerprefs
    /// </summary>
    private void RestoreUIData()
    {
        if (AppData.GetSoundInfo() == 0)
        {
            soundBtn.GetComponent<Image>().sprite = soundOn;
        }
        else
        {
            soundBtn.GetComponent<Image>().sprite = soundOff;
        }

        if (AppData.GetHapticInfo() == 0)
        {
            vibrationBtn.GetComponent<Image>().sprite = vibrationOn;
        }
        else
        {
            vibrationBtn.GetComponent<Image>().sprite = vibrationOff;
        }
    }

    void OnApplicationQuit()
    {
        //RoadMaterial.color = Color.white;
        deathColliderMaterial.color = Color.white;
    }
}
