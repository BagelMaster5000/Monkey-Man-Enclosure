using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("General")]
    public LevelSO curLevel;
    public enum GameState { MAINVIEW, SHOP, GAMEOVER, PAUSED };
    public GameState curGameState;

    //[Header("Shop Menu")]
    //[SerializeField] ShopMenu shopMenuController;

    [SerializeField] WinLoseMenu winLoseMenu;

    [Header("Monkey Generation")]
    [SerializeField] GameObject monkeyPrefab;
    Monkey[] monkeys;
    [SerializeField] Transform manTransform;
    Man man;
    [SerializeField] Bounds primateSpawnBounds;
    float minDistanceBetweenPrimates = 2;
    int maxSpawnCollisionChecks = 15;
    [SerializeField] LayerMask primateLayer;

    [Header("Whistle")]
    [SerializeField] Button whistleButton;

    [Header("SFX")]
    [Range(0, 1)]
    [SerializeField] float badWhistleChance = 0.2f;
    [SerializeField] SoundPlayer whistleGoodSFX;
    [SerializeField] SoundPlayer whistleBadSFX;
    [SerializeField] SoundPlayer goodEndingJingle;
    [SerializeField] SoundPlayer badEndingJingle;

    #region Setup
    private void Awake()
    {
        if (manTransform)
            man = manTransform.GetComponent<Man>();
        instance = this;
    }

    private void Start()
    {
        if (manTransform)
            PositionMan();
        if (monkeyPrefab)
            SpawnMonkeys();

        StartCoroutine(RefreshingWhistleButtonState());

        //Set the volume
        AudioListener.volume = GameManager.soundSettings;
    }

    private void PositionMan()
    {
        Vector3 manLocation = new Vector3(
            Random.Range(primateSpawnBounds.center.x - primateSpawnBounds.extents.x,
                primateSpawnBounds.center.x + primateSpawnBounds.extents.x),
            primateSpawnBounds.center.y,
            Random.Range(primateSpawnBounds.center.z - primateSpawnBounds.extents.z,
                primateSpawnBounds.center.z + primateSpawnBounds.extents.z));

        manTransform.position = manLocation;
        manTransform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
    }

    private void SpawnMonkeys()
    {
        monkeys = new Monkey[curLevel.monkeyAmount];
        Vector3 spawnLocation = Vector3.one;
        for (int n = 0; n < curLevel.monkeyAmount; n++)
        {
            // Tries to find a location that doesn't collide with another primate. Gives up after 5 attempts
            bool done = false;
            for (int collisionCheckAttempts = 0; collisionCheckAttempts < maxSpawnCollisionChecks && !done; collisionCheckAttempts++)
            {
                spawnLocation = new Vector3(
                    Random.Range(primateSpawnBounds.center.x - primateSpawnBounds.extents.x,
                        primateSpawnBounds.center.x + primateSpawnBounds.extents.x),
                    primateSpawnBounds.center.y,
                    Random.Range(primateSpawnBounds.center.z - primateSpawnBounds.extents.z,
                        primateSpawnBounds.center.z + primateSpawnBounds.extents.z));

                if (Physics.OverlapSphere(spawnLocation, minDistanceBetweenPrimates, primateLayer).Length == 0)
                    done = true;
            }

            monkeys[n] = Instantiate(
                monkeyPrefab,
                spawnLocation,
                Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up)).GetComponent<Monkey>();
        }
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && curGameState != GameState.GAMEOVER)
            TogglePauseVisibility();
    }

    public void ToggleShopVisibility()
    {
        if (curGameState == GameState.MAINVIEW)
            curGameState = GameState.SHOP;
        else if (curGameState == GameState.SHOP)
            curGameState = GameState.MAINVIEW;
    }

    public void TogglePauseVisibility()
    {
        if (curGameState != GameState.PAUSED && curGameState != GameState.GAMEOVER)
            curGameState = GameState.PAUSED;
        else if (curGameState == GameState.PAUSED)
            curGameState = GameState.MAINVIEW;
    }

    #region Whistle
    IEnumerator RefreshingWhistleButtonState()
    {
        while (true)
        {
            whistleButton.interactable = curGameState == GameState.MAINVIEW;
            yield return new WaitForSeconds(GlobalVariables.UIRefreshInterval);
        }
    }

    public void Whistle()
    {
        foreach (Monkey monkey in monkeys)
            monkey.GoToWhistle(Vector3.back);
        man.ModifySleep((man.maxAwakeLevel - man.awakeLevel) / 4 + (man.maxAwakeLevel / 4));

        if (Random.Range(0.0f, 1.0f) <= badWhistleChance)
            whistleBadSFX.Play();
        else
            whistleGoodSFX.Play();
    }
    #endregion

    #region Game Over
    [ContextMenu("Test Win")]
    public void Win()
    {
        if (curGameState == GameState.GAMEOVER) return;

        SetGameToBeOver();

        winLoseMenu.Victory();

        MusicPlayer.instance.FadeOut();
        StartCoroutine(PlayVictoryJingleWithDelay(0.5f));
    }
    IEnumerator PlayVictoryJingleWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        goodEndingJingle.Play();
    }

    [ContextMenu("Test Lose")]
    public void Lose()
    {
        if (curGameState == GameState.GAMEOVER) return;

        SetGameToBeOver();

        winLoseMenu.Failure();

        MusicPlayer.instance.FadeOut();
        StartCoroutine(PlayFailureJingleWithDelay(0));
    }
    IEnumerator PlayFailureJingleWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        badEndingJingle.Play();
    }

    private void SetGameToBeOver()
    {
        curGameState = GameState.GAMEOVER;
        foreach (Monkey monkey in monkeys)
            monkey.gameObject.SetActive(false);
        man.gameObject.SetActive(false);
        foreach (VisitorDetour visitorDetour in FindObjectsOfType<VisitorDetour>())
            visitorDetour.gameObject.SetActive(false);
        foreach (Visitor visitor in FindObjectsOfType<Visitor>())
            visitor.gameObject.SetActive(false);
    }
    #endregion
}
