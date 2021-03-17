using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("General")]
    [SerializeField] LevelSO curLevel;
    public enum GameState { MAINVIEW, SHOP, GAMEOVER, PAUSED };
    public GameState curGameState;

    //[Header("Shop Menu")]
    //[SerializeField] ShopMenu shopMenuController;

    // Win or Lose Menu

    [Header("Monkey Generation")]
    [SerializeField] GameObject monkeyPrefab;
    Monkey[] monkeys;
    [SerializeField] Transform manTransform;
    Man man;
    [SerializeField] Bounds primateSpawnBounds;
    float minDistanceBetweenPrimates = 2;
    int maxSpawnCollisionChecks = 15;
    [SerializeField] LayerMask primateLayer;

    #region Setup
    private void Awake()
    {
        man = manTransform.GetComponent<Man>();
        instance = this;
    }

    private void Start()
    {
        PositionMan();
        SpawnMonkeys();
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
                //print("iteration #" + collisionCheckAttempts);

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

    public void ToggleShopVisibility()
    {
        if (curGameState == GameState.MAINVIEW)
            curGameState = GameState.SHOP;
        else if (curGameState == GameState.SHOP)
            curGameState = GameState.MAINVIEW;

    }

    public void Whistle()
    {

    }

    public void Win()
    {

    }

    public void Lose()
    {

    }

}
