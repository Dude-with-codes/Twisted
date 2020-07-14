using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum GameState
    {
        live,
        menu,
        store
    }

    public delegate void StartEvent();
    public event StartEvent gameStarted;
    public event StartEvent gameEnded;

    public static GameController instance;

    public PlayerHandler player;
    public Spawner spawner;
    public TerrainSpawner terrainSpawner;
    public UIController uiController;
    public CameraAnimation cameraAnimation;
    public Store store;
    public float gameSpeed;
    public Button mainButton;
    public GameObject tapText;
    public float curtainTransitionTime;

    [Header("Title")]
    public Transform titleChainBases;
    public float titleInOutTime, titleDelay;
    public Vector3 titleUpPosition, titleDownPosition;
    public bool isTitleDown = false;

    [Header("Hurdles Meta")]
    //public GameObject hurdle;
    public Transform hurdleParent;
    [Range(0f, 5f)]
    public float spawnWait;
    public int hurdleCount;
    public float environmentRotationTime = 0.5f;
    public float cloudSpeed;
    public Transform cloudParent;

    float gameStartTime;
    GameState state = GameState.menu;
    List<GameObject> hurdles = new List<GameObject>();
    float actualSpeed = 0f;
    float randomAngle = 0f;
    int direction = 1, poolIndex = 0, jumpCount = 0;
    Vector3 spawnVector;
    bool initiateStop = false;
    int highscore = 0, currentHighscore = 0;
    bool titleTransition = false;
    float titleProgress = 0f, titleTrasitionTimeStore;

    public int Direction { get; set; }

    public GameState State
    {
        get
        {
            return state;
        }
    }

    void Awake()
    {
        if (!instance)
            instance = this;

        if(!PlayerPrefs.HasKey("Highscore"))
        {
            PlayerPrefs.SetInt("Highscore", highscore);
        }
        else
        {
            highscore = PlayerPrefs.GetInt("Highscore");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameSpeed = 1f;
        mainButton.onClick.AddListener(StartGame);
        uiController.ShowHighscore(highscore);

        gameEnded += TitleTransition;
        gameStarted += TitleTransition;

        TitleTransition();
        titleTrasitionTimeStore = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MakePlayerJump();
        }

        if(state == GameState.live && !initiateStop)
        {
            actualSpeed += (Time.deltaTime / 5f);
            gameSpeed = Mathf.Clamp(((actualSpeed) / 10f) + 1, 1f, 5f);
        }

        if (Time.time <= titleTrasitionTimeStore + titleInOutTime)
        {
            titleProgress += (Time.deltaTime * (isTitleDown ? -1 : 1)) / titleInOutTime;

            titleChainBases.localPosition = Vector3.Lerp(titleUpPosition, titleDownPosition, titleProgress);
        }
    }

    public void SetStateToStore(bool inStore)
    {
        state = inStore ? GameState.store : GameState.menu;
    }

    public void TitleTransition()
    {
        if (state != GameState.live)
            cameraAnimation.TakeToStore();

        StartCoroutine(TransitTitle());
    }

    IEnumerator TransitTitle()
    {
        yield return new WaitForSeconds(titleDelay);

        titleTrasitionTimeStore = Time.time;

        yield return new WaitForSeconds(titleInOutTime);

        isTitleDown = !isTitleDown;

        StopCoroutine(TransitTitle());
    }

    public void StartGame()
    {
        if (state == GameState.menu)
        {
            gameStartTime = Time.time;
            state = GameState.live;
            initiateStop = false;

            mainButton.onClick.RemoveAllListeners();
            mainButton.onClick.AddListener(MakePlayerJump);

            jumpCount = 0;
            poolIndex = 0;
            Direction = 0;
            hurdleParent.eulerAngles = Vector3.zero;
            currentHighscore = 0;
            uiController.AddScore(currentHighscore);

            gameStarted();
        }
    }

    public void Gameover()
    {
        gameEnded();

        state = GameState.menu;
        spawner.waiting = false;

        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(StartGame);

        CheckHighscore();
    }

    public void InitateStop()
    {
        initiateStop = true;
        StartCoroutine(StartStopping());
    }

    IEnumerator StartStopping()
    {
        while(gameSpeed > 0.05f)
        {
            gameSpeed = Mathf.Lerp(gameSpeed, 0f, 0.05f);
            yield return null;
        }

        Gameover();
        StopCoroutine(StartStopping());
    }

    public void AddToHighscore()
    {
        currentHighscore++;
        uiController.AddScore(currentHighscore);
    }

    public void MakePlayerJump()
    {
        player.Jump();
    }

    public void RotateEnvironment()
    {
        Vector3 temp, temp2;
        temp = temp2 = hurdleParent.eulerAngles;
        temp2.z -= (spawner.angleGap * Direction);
        StartCoroutine (EnvironmentTransition(temp, temp2));
    }

    void CheckHighscore()
    {
        if(currentHighscore > highscore)
        {
            highscore = currentHighscore;
            PlayerPrefs.SetInt("Highscore", highscore);
            uiController.ShowHighscore(highscore);
        }
    }

    IEnumerator EnvironmentTransition(Vector3 current, Vector3 target)
    {
        float progress = 0f;

        while(progress < 1f)
        {
            progress += (Time.deltaTime / (environmentRotationTime / gameSpeed));
            hurdleParent.eulerAngles = Vector3.Lerp(current, target, progress);
            yield return null;
        }
    }
}
