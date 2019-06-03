using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance { private set; get; }

    [Header("External Game Data")]
    public NumberData numbersDataObject;
    public LevelData levelsDataObject;

    [Header("Properites")]
    public int startBallCount;
    public int ballsCountToCompleteLvl;
    public float ballSpeed;
    public GameObject tutorialHand;
    public float tutorialDelay;

    [Header("Difficulty options")]
    public int maxDifficulty;
    public float speedIncrease;
    public int ballsToPassCountIncrease;

    [Header("Time Variables")]
    /// <summary> How long new ball will take to join chain </summary>
    [Tooltip("How long new ball will take to join chain")]
    public float insertAnimDuration;
    /// <summary> Time between ball explosions at level fail </summary>
    [Tooltip("Time between ball explosions at level fail")]
    public float gameOverDestroyDelay;
    public float nextLevelLoadDelay;

    [Header("Spawn Variables")]
    public GameObject chainedBallPrefab;
    public GameObject freeBallPrefab;
    public GameObject[] bonusBallPrefabs;
    [Range(0, 1f)]public float bonusProbability;
    /// <summary> max possible spawn value will be 2^maxSpawnPower </summary>
    [Tooltip("max possible spawn value will be 2^maxSpawnPower")]
    public int maxSpawnPower;
    

    //Hidden public variables
    [HideInInspector] public Player Player;
    [HideInInspector] public GameObject freeBall;

    [HideInInspector] public PathCreator levelPath;
    [HideInInspector] public int currentScore = 0;
    [HideInInspector] public int mergedBallsCount = 0;
    [HideInInspector] public bool canHaptic = true;
    [HideInInspector] public bool paused = false;
    

    [Header("Events")]
    public UnityEvent onLevelStartEvent;
    public UnityEvent onLevelCompleteEvent;
    public UnityEvent onGameOverEvent;
    public UnityEvent onBallMergeEvent;
    public UnityEvent onManagerResetEvent;


    private int currentLevelIndex;
    private GameObject currentLevelObject;

    public float ballRadius { private set; get; }

    public List<Ball> balls { private set; get; }
    [HideInInspector] public bool gameActive;
    
    private int difficulty = 0;

    #endregion

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {

            Debug.Log("Game Manager already exists");
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;

        gameActive = false;

        ballRadius = chainedBallPrefab.GetComponent<CircleCollider2D>().radius;

        balls = new List<Ball>();

        currentLevelIndex = AppData.GetCurrentLvlIndex();

        difficulty = AppData.GetDifficulty();
        ballSpeed += speedIncrease * difficulty;
        ballsCountToCompleteLvl += ballsToPassCountIncrease * difficulty;

        
    }

    void Start() {

        OnLevelStart();
    }
    
    public void EnableTutorial()
    {
        if(AppData.checkFirstRun())
            tutorialHand.SetActive(true);
    }

    public void DisableTutorial()
    {
        tutorialHand.SetActive(false);
    }

    public void DelayTutorial()
    {
        Invoke("DisableTutorial", tutorialDelay);
    }

    public void CheckNumbersValue(int index) {

        if (index != 0 && balls[index].value == balls[index - 1].value) {
            Merge(index - 1);
            return;
        }

        if (index != balls.Count - 1 && balls[index].value == balls[index + 1].value) {
            Merge(index);
            return;
        }

        balls[index].ParentMovementSwitch(true);
        balls[index].ChildrenMovementSwitch(true);
        balls[index].canMove = true;
        paused = false;

        if (gameActive)
            ReloadNumber();
    }

    public void Merge(int index) {
        AudioManager.Instance.PlayMergeSound();
        Destroy(balls[index].gameObject);
        balls.RemoveAt(index);
        mergedBallsCount++;
        balls[index].UpdateNuberValue(balls[index].value *= 2);   //Balls Value is my new Score

        balls[index].GetComponent<Animation>().Play();
        
        balls[index].ThrowParticle();
        currentScore += balls[index].value;
        OnBallMerge();
        for (int i = index; i < balls.Count; i++) {
            balls[i].index--;
        }
        for (int i = index - 1; i >= 0; i--) {
            balls[i].MoveParent(-1);
        }

        CheckNumbersValueDelayed(index);

    }
    
    public void CheckNumbersValueDelayed(int index)
    {
        StartCoroutine(waitBeforeCheck(index));
    }

    public IEnumerator waitBeforeCheck(int index) {
        yield return new WaitForSeconds(insertAnimDuration);
        CheckNumbersValue(index);
    }

    private IEnumerator StartLevel() {

        float time = ballRadius * 2 * chainedBallPrefab.transform.lossyScale.x / ballSpeed;
        float timer = time;
        int LastValue = 0;
        for (int i = 0; i < startBallCount;) {
            if (!paused && gameActive)
                timer -= Time.fixedDeltaTime;
            if (timer <= 0) {
                GameObject obj = Instantiate(chainedBallPrefab, levelPath.path.GetPointAtDistance(0f), chainedBallPrefab.transform.rotation);
                obj.tag = "Number";
                Ball objMoveController = obj.GetComponent<Ball>();
                objMoveController.inserted = true;
                int Value = LastValue;
                while (LastValue == Value) {
                    Value = RandomiseValue();
                }
                objMoveController.value = Value;
                objMoveController.spriteRenderer.color = getElementColor(objMoveController.value);
                balls.Add(objMoveController);
                objMoveController.index = balls.Count - 1;
                timer = time;
                LastValue = Value;
                i++;
            }
            yield return new WaitForFixedUpdate();
        }
    }


    public IEnumerator DestroyBalls() {
        while (balls.Count > 0) {
            balls[0].ThrowParticle();
            Destroy(balls[0].gameObject);
            balls.RemoveAt(0);

            yield return new WaitForSeconds(gameOverDestroyDelay);
        }

        freeBall.GetComponent<Ball>().ThrowParticle();
        Destroy(freeBall.gameObject);

    }

    public void startPlaying() {
        gameActive = true;
        ReloadNumber();

        StartCoroutine(StartLevel());
    }

    public void SpawnLevel() {
        currentLevelObject = Instantiate(levelsDataObject.levels[currentLevelIndex]);
    }

    public void OnLevelStart() {
        SpawnLevel();
        onLevelStartEvent?.Invoke();
    }

    public void OnLevelComplete() {
        if (gameActive) {
            AudioManager.Instance.PlayWinSound();

            gameActive = false;

            AppData.SetCurrentLvl(AppData.GetCurrentLvl() + 1);


            onLevelCompleteEvent?.Invoke();

            Invoke("LoadNextLevel", nextLevelLoadDelay);

            DoHaptic(2);
        }
    }

    public void LoadNextLevel() {
        Destroy(currentLevelObject);

        ResetManager();

        currentLevelIndex++;
        if (currentLevelIndex >= levelsDataObject.levels.Length) {
            currentLevelIndex = 0;

            if(difficulty < maxDifficulty)
            {
                ballSpeed += speedIncrease;
                ballsCountToCompleteLvl += ballsToPassCountIncrease;
                difficulty++;
                AppData.SetDifficulty(difficulty);
            }

        }

        AppData.SetCurrentLvlIndex(currentLevelIndex);

        UiManager.Instance.LoadNextLevel();


        SpawnLevel();

    }

    public void Restart() {
        currentScore = 0;

        ResetManager();

        startPlaying();

        UiManager.Instance.Restart();
    }

    public void OnGameOver() {
        AudioManager.Instance.PlayLoseSound();
        gameActive = false;
        StartCoroutine(DestroyBalls());

        onGameOverEvent?.Invoke();

        DoHaptic(2);
    }
    public void OnBallMerge() {
        if (mergedBallsCount >= ballsCountToCompleteLvl) {
            OnLevelComplete();
        }
        onBallMergeEvent?.Invoke();
    }

    public void ResetManager() {
        StopAllCoroutines();

        foreach (Ball ball in balls) {
            Destroy(ball.gameObject);
        }

        balls = new List<Ball>();

        Destroy(freeBall);

        mergedBallsCount = 0;

        onManagerResetEvent.Invoke();
    }


    public void ReloadNumber() {

        float useBonus = Random.Range(0f, 1f);

        if (useBonus < bonusProbability)
        {
            if (bonusBallPrefabs.Length == 0)
                return;

            int bonusIndex = Random.Range(0, bonusBallPrefabs.Length);

            freeBall = Instantiate(bonusBallPrefabs[bonusIndex], Player.transform.position, bonusBallPrefabs[bonusIndex].transform.rotation);

        }
        else
        {
            freeBall = Instantiate(freeBallPrefab, Player.transform.position, freeBallPrefab.transform.rotation);
            Ball obj = freeBall.GetComponent<Ball>();
            obj.value = RandomiseValue();
            obj.spriteRenderer.color = getElementColor(obj.value);
        }
    }

    public Color getElementColor(int value) {
        int power = (int)Mathf.Log(value, 2);

        if (power >= numbersDataObject.NumberColors.Length) {
            return numbersDataObject.DefaultForOtherColorsHigherNumbers;
        }

        return numbersDataObject.NumberColors[power];



    }

    public void destroyAfterDelay(GameObject obj, float time) {
        Destroy(obj, time);
    }

    public int RandomiseValue() {
        float random = Random.Range(1f, maxSpawnPower);
        int power = power = (int)Mathf.Floor(maxSpawnPower / random);
        return (int)Mathf.Pow(2, power);
    }

    public void DoHaptic(int hapticForce) {
        if (canHaptic) {
            HapticFeedback.DoHaptic((HapticFeedback.HapticForce)hapticForce);
        }
    }

}
