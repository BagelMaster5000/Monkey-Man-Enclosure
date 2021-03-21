using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Visitor : MonoBehaviour
{
    [Header("Visitor")]
    public GameObject throwablePrefab;
    public bool disruptor = false;

    [Header("Throwable Scriptable Objects")]
    [SerializeField] ThrowableSO foodPellets;
    [SerializeField] ThrowableSO brick;
    [SerializeField] ThrowableSO banana;

    [Header("Throwable Type Chances")]
    [SerializeField, Range(0, 10), Tooltip("Chance a food pellet is thrown. If the chances total more than 10, this will reset to 3.")]
    private float chanceForFood = 3;
    [SerializeField, Range(0, 10), Tooltip("Chance a banana is thrown. If the chances total more than 10, this will reset to 3.")]
    private float chanceForBanana = 3;
    [SerializeField, Range(0, 10), Tooltip("Chance a brick is thrown. If the chances total more than 10, this will reset to 4.")]
    private float chanceForBrick = 4;

    [Header("Throw Times")]
    [SerializeField] private float maxTimeUntilThrow;
    private float timeUntilThrow;
    [SerializeField] private float throwDuration = 3f;

    [Header("Throwing")]
    [SerializeField] GameObject targetingVisual;

    [Space(10)]
    [SerializeField] Transform AOECircleCenterLoc;
    [SerializeField] Transform AOECircleTransform;
    [SerializeField] LineRenderer AOECircle;
    int pointsInAOECircle = 30;
    float AOECircleRotationSpeed = 15;
    [SerializeField] Bounds aimingBounds;

    [Space(10)]
    [SerializeField] Transform throwStartLoc;
    [SerializeField] LineRenderer throwArc;
    [SerializeField] float throwAirTime = 2;
    int pointsInThrowArc = 20;

    // Pools
    VisitorThrowablesPools throwablesPool;
    int numFoodPelletsToThrow = 5;
    int foodPelletsPoolIteration = 0;
    float pelletsThrowRandomizer = 0.5f;
    int bricksPoolIteration = 0;
    int bananasPoolIteration = 0;

    [Header("NavMesh Helpers")]
    public Transform destination;
    private NavMeshAgent agent;

    public enum Going { ToDestination, ToRailing, ToPath}
    [HideInInspector] public Going state;

    [Header("Animations")]
    [SerializeField] Animator walkingAnimator;
    [SerializeField] float walkingSpeedThreshold = 0.02f;

    // Start is called before the first frame update
    void OnEnable()
    {
        if(chanceForFood + chanceForBanana + chanceForBrick > 10)
        {
            chanceForFood = 3;
            chanceForBanana = 3;
            chanceForBrick = 4;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        throwablesPool = VisitorThrowablesPools.instance;
    }

    private void Start()
    {
        if (destination != null)
            agent.destination = destination.position;

        if (walkingAnimator != null)
            StartCoroutine(RefreshingAnimationState());

        state = Going.ToDestination;
    }

    public void GoTowardsPoint(Vector3 point)
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        Vector3 directionOfPoint = point - transform.position;  //Get direction from visitor to point

        NavMeshHit hit;
        NavMesh.SamplePosition(directionOfPoint, out hit, 5, ~NavMesh.GetAreaFromName("Enclosure"));   //Get the closest point on the NavMesh

        agent.destination = point;  //Set the navMeshAgent's destination
    }

    #region Throwing
    public void StartThrowing(float trackingTime)
    {
        StartCoroutine(Throwing(trackingTime));
    }

    public IEnumerator Throwing(float trackingTime)
    {
        int itemToThrow = Random.Range(0, 3); // 0 Food Pellets, 1 Brick, 2 Banana

        float affectRange = 0;
        switch (itemToThrow)
        {
            case 0: affectRange = foodPellets.affectRange; break;
            case 1: affectRange = brick.affectRange; break;
            case 2: affectRange = banana.affectRange; break;
        }

        Vector3 throwLocation = CalculateThrowLocation(affectRange);

        DrawThrowAOECircle(affectRange);
        targetingVisual.SetActive(true);

        float initialXVelocity, initialYVelocity, initialZVelocity;
        float timeToStopTracking = Time.time + trackingTime;
        while (Time.time < timeToStopTracking)
        {
            AOECircleCenterLoc.position = throwLocation;

            DrawThrowArc(out initialXVelocity, out initialYVelocity, out initialZVelocity);

            yield return null;
        }

        Throw(itemToThrow);

        targetingVisual.SetActive(false);
    }

    private Vector3 CalculateThrowLocation(float affectRange)
    {
        return new Vector3(
            Random.Range(aimingBounds.center.x - aimingBounds.extents.x + affectRange,
                aimingBounds.center.x + aimingBounds.extents.x - affectRange),
            aimingBounds.center.y,
            Random.Range(aimingBounds.center.z - aimingBounds.extents.z + affectRange,
                aimingBounds.center.z + aimingBounds.extents.z - affectRange));
    }

    private void DrawThrowAOECircle(float affectRange)
    {
        AOECircle.positionCount = pointsInAOECircle + 2;
        float angle = 0;
        for (int i = 0; i < pointsInAOECircle + 2; i++)
        {
            float x = Mathf.Cos(angle) * affectRange;
            float z = Mathf.Sin(angle) * affectRange;

            AOECircle.SetPosition(i, new Vector3(x, 0, z));

            angle += 2 * Mathf.PI / pointsInAOECircle;
        }
    }

    private void DrawThrowArc(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity)
    {
        CalculateInitialThrowVelocities(out initialXVelocity, out initialYVelocity, out initialZVelocity);

        throwArc.transform.rotation = Quaternion.identity;

        throwArc.positionCount = pointsInThrowArc + 1;
        float simulatedElapsedTime = 0;
        for (int i = 0; i < pointsInThrowArc + 1; i++)
        {
            throwArc.SetPosition(i, new Vector3(
                initialXVelocity * simulatedElapsedTime,
                initialYVelocity * simulatedElapsedTime + 0.5f * Physics.gravity.y * Mathf.Pow(simulatedElapsedTime, 2),
                initialZVelocity * simulatedElapsedTime));

            simulatedElapsedTime += throwAirTime / pointsInThrowArc;
        }
    }

    private void Throw(int itemToThrow)
    {
        CalculateInitialThrowVelocities(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity);

        switch (itemToThrow)
        {
            case 0: ThrowFoodPellets(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 1: ThrowBrick(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 2: ThrowBanana(initialXVelocity, initialYVelocity, initialZVelocity); break;
        }
    }

    private void CalculateInitialThrowVelocities(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity)
    {
        initialXVelocity = (AOECircleCenterLoc.position.x - throwStartLoc.position.x) / throwAirTime;
        initialZVelocity = (AOECircleCenterLoc.position.z - throwStartLoc.position.z) / throwAirTime;
        initialYVelocity = (AOECircleCenterLoc.position.y - throwStartLoc.position.y -
            0.5f * Physics.gravity.y * Mathf.Pow(throwAirTime, 2)) /
            throwAirTime;
    }

    private void ThrowFoodPellets(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        Rigidbody itemToThrow;
        for (int x = 0; x < numFoodPelletsToThrow; x++)
        {
            throwablesPool.foodPelletsPool[foodPelletsPoolIteration].SetActive(true);
            itemToThrow = throwablesPool.foodPelletsPool[foodPelletsPoolIteration].GetComponent<Rigidbody>();
            itemToThrow.transform.position =
                throwStartLoc.position +
                new Vector3(
                    Random.Range(-pelletsThrowRandomizer, pelletsThrowRandomizer),
                    Random.Range(-pelletsThrowRandomizer, pelletsThrowRandomizer),
                    Random.Range(-pelletsThrowRandomizer, pelletsThrowRandomizer));
            itemToThrow.useGravity = true;
            itemToThrow.velocity =
                new Vector3(initialXVelocity, initialYVelocity, initialZVelocity) +
                new Vector3(
                    Random.Range(-pelletsThrowRandomizer, pelletsThrowRandomizer),
                    Random.Range(-pelletsThrowRandomizer, pelletsThrowRandomizer),
                    Random.Range(-pelletsThrowRandomizer, pelletsThrowRandomizer));

            foodPelletsPoolIteration++;
            if (foodPelletsPoolIteration >= throwablesPool.foodPelletsPool.Length)
                foodPelletsPoolIteration = 0;
        }
    }

    private void ThrowBrick(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        Rigidbody itemToThrow;
        throwablesPool.bricksPool[bricksPoolIteration].SetActive(true);
        itemToThrow = throwablesPool.bricksPool[bricksPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);

        bricksPoolIteration++;
        if (bricksPoolIteration >= throwablesPool.bricksPool.Length)
            bricksPoolIteration = 0;
    }

    private void ThrowBanana(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        Rigidbody itemToThrow;
        throwablesPool.bananasPool[bananasPoolIteration].SetActive(true);
        itemToThrow = throwablesPool.bananasPool[bananasPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);

        bananasPoolIteration++;
        if (bananasPoolIteration >= throwablesPool.bananasPool.Length)
            bananasPoolIteration = 0;
    }
    #endregion

    IEnumerator RefreshingAnimationState()
    {
        while (true)
        {
            if (agent)
                walkingAnimator.SetBool("Walking", agent.velocity.magnitude > walkingSpeedThreshold);

            yield return new WaitForSeconds(0.05f);
        }
    }
}
