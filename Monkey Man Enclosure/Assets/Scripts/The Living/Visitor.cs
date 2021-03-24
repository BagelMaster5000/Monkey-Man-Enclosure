using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Visitor : MonoBehaviour
{
    [Header("Visitor")]
    public bool disruptor = false;
    //public GameObject throwablePrefab;

    [Header("NavMesh Helpers")]
    public Transform destination;
    private NavMeshAgent agent;

    public enum Going { ToDestination, ToRailing, ToPath }
    [HideInInspector] public Going state;


    [Header("Throwable Scriptable Objects")]
    [SerializeField] ThrowableSO foodPellets;
    [SerializeField] ThrowableSO brick;
    [SerializeField] ThrowableSO banana;
    float affectRangeExpansionFactor = 1.3f;

    [Header("Throwable Type Chances")]
    [SerializeField, Range(0, 10), Tooltip("Chance a food pellet is thrown. If the chances total more than 10, this will reset to 3.")]
    private float chanceForFood = 3;
    [SerializeField, Range(0, 10), Tooltip("Chance a banana is thrown. If the chances total more than 10, this will reset to 3.")]
    private float chanceForBanana = 3;
    [SerializeField, Range(0, 10), Tooltip("Chance a brick is thrown. If the chances total more than 10, this will reset to 4.")]
    private float chanceForBrick = 4;

    [Header("Throwing")]
    [SerializeField] GameObject targetingVisual;
    [SerializeField] LayerMask primatesLayer;
    [SerializeField] float throwingRangeBorderAmt = 0;
    float maxTargetingMovingAmt = 2;

    [Space(10)]
    [SerializeField] Transform AOECircleCenterLoc;
    [SerializeField] Transform AOECircleTransform;
    [SerializeField] LineRenderer AOECircle;
    int pointsInAOECircle = 30;
    const float CIRCLE_ADJUSTMENT_FACTOR = 1.3f;
    float AOECircleRotationSpeed = -15;
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

    [Header("Animations")]
    [SerializeField] Animator walkingAnimator;
    [SerializeField] float walkingSpeedThreshold = 0.02f;

    [Header("SFX")]
    [SerializeField] SoundPlayer throwWooshSFX;

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

    #region Targeting
    public void StartTargeting(float trackingTime)
    {
        StartCoroutine(Targeting(trackingTime));
    }

    public IEnumerator Targeting(float trackingTime)
    {
        int itemToThrow = Random.Range(0, 3); // 0 Food Pellets, 1 Brick, 2 Banana

        float affectRange = 0;
        switch (itemToThrow)
        {
            case 0: affectRange = foodPellets.affectRange * affectRangeExpansionFactor; break;
            case 1: affectRange = brick.affectRange * affectRangeExpansionFactor; break;
            case 2: affectRange = banana.affectRange * affectRangeExpansionFactor; break;
        }

        Vector3 throwLocation = CalculateThrowLocation(affectRange);

        DrawThrowAOECircle(affectRange);
        targetingVisual.SetActive(true);

        float initialXVelocity, initialYVelocity, initialZVelocity;
        float timeToStopTracking = Time.time + trackingTime;
        Vector3 randomDirectionToMove = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        while (Time.time < timeToStopTracking)
        {
            //MoveThrowLocation(ref throwLocation, randomDirectionToMove);
            AOECircleCenterLoc.position = throwLocation;


            RotateAOECircle();
            DrawPreviewThrowArc(out initialXVelocity, out initialYVelocity, out initialZVelocity);


            yield return null;
        }

        Throw(itemToThrow);

        targetingVisual.SetActive(false);
    }

    //private void MoveThrowLocation(ref Vector3 throwLocation, Vector3 randomDirectionToMove)
    //{
    //    throwLocation += randomDirectionToMove * Time.deltaTime * maxTargetingMovingAmt;
    //    throwLocation = new Vector3(
    //        Mathf.Clamp(throwLocation.x,
    //            aimingBounds.center.x - aimingBounds.extents.x + throwingRangeBorderAmt,
    //            aimingBounds.center.x + aimingBounds.extents.x - throwingRangeBorderAmt),
    //        aimingBounds.center.y,
    //        Mathf.Clamp(throwLocation.z,
    //            aimingBounds.center.z - aimingBounds.extents.z + throwingRangeBorderAmt,
    //            aimingBounds.center.z + aimingBounds.extents.z - throwingRangeBorderAmt));

    //    maxTargetingMovingAmt *= 0.99f;
    //}

    private void RotateAOECircle()
    {
        AOECircleTransform.Rotate(Vector3.up, AOECircleRotationSpeed * Time.deltaTime);
    }

    private Vector3 CalculateThrowLocation(float affectRange)
    {
        return new Vector3(
            Random.Range(aimingBounds.center.x - aimingBounds.extents.x + throwingRangeBorderAmt,
                aimingBounds.center.x + aimingBounds.extents.x - throwingRangeBorderAmt),
            aimingBounds.center.y,
            Random.Range(aimingBounds.center.z - aimingBounds.extents.z + throwingRangeBorderAmt,
                aimingBounds.center.z + aimingBounds.extents.z - throwingRangeBorderAmt));
    }

    private void DrawThrowAOECircle(float affectRange)
    {
        AOECircle.positionCount = pointsInAOECircle + 2;
        float angle = 0;
        for (int i = 0; i < pointsInAOECircle + 2; i++)
        {
            float x = Mathf.Cos(angle) * affectRange * CIRCLE_ADJUSTMENT_FACTOR;
            float z = Mathf.Sin(angle) * affectRange * CIRCLE_ADJUSTMENT_FACTOR;

            AOECircle.SetPosition(i, new Vector3(x, 0, z));

            angle += 2 * Mathf.PI / pointsInAOECircle;
        }
    }

    private void DrawPreviewThrowArc(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity)
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
    #endregion

    #region Throwing
    private void Throw(int itemToThrow)
    {
        CalculateInitialThrowVelocities(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity);

        switch (itemToThrow)
        {
            case 0: ThrowFoodPellets(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 1: ThrowBrick(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 2: ThrowBanana(initialXVelocity, initialYVelocity, initialZVelocity); break;
        }

        throwWooshSFX.Play();
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
            itemToThrow.angularDrag = 1;
            itemToThrow.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            itemToThrow.AddRelativeTorque(Vector3.down * 50000, ForceMode.Impulse);

            foodPelletsPoolIteration++;
            if (foodPelletsPoolIteration >= throwablesPool.foodPelletsPool.Length)
                foodPelletsPoolIteration = 0;

            StartCoroutine(SolidifyItemOnceLanded(itemToThrow));
        }

        StartCoroutine(FoodPelletsAffectPrimatesAfterDelay(AOECircleCenterLoc.position));
    }
    IEnumerator FoodPelletsAffectPrimatesAfterDelay(Vector3 throwLandingLocation)
    {
        yield return new WaitForSeconds(throwAirTime);

        Collider[] primatesInRange = Physics.OverlapSphere(throwLandingLocation, foodPellets.affectRange * affectRangeExpansionFactor, primatesLayer, QueryTriggerInteraction.Ignore);
        foreach (Collider primate in primatesInRange)
            primate.GetComponent<Primate>().GoTowardsPoint(throwLandingLocation);
    }

    private void ThrowBrick(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        Rigidbody itemToThrow;
        throwablesPool.bricksPool[bricksPoolIteration].SetActive(true);
        itemToThrow = throwablesPool.bricksPool[bricksPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);
        itemToThrow.angularDrag = 1;
        itemToThrow.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        itemToThrow.AddRelativeTorque(Vector3.down * 50000, ForceMode.Impulse);

        bricksPoolIteration++;
        if (bricksPoolIteration >= throwablesPool.bricksPool.Length)
            bricksPoolIteration = 0;

        StartCoroutine(SolidifyItemOnceLanded(itemToThrow));
        StartCoroutine(BrickAffectsPrimatesAfterDelay(AOECircleCenterLoc.position));
    }
    IEnumerator BrickAffectsPrimatesAfterDelay(Vector3 throwLandingLocation)
    {
        yield return new WaitForSeconds(throwAirTime);

        Collider[] primatesInRange = Physics.OverlapSphere(throwLandingLocation, brick.affectRange * affectRangeExpansionFactor, primatesLayer, QueryTriggerInteraction.Ignore);
        foreach (Collider primate in primatesInRange)
            primate.GetComponent<Primate>().RunFromPoint(throwLandingLocation, brick.affectRange * 1.2f);
    }

    private void ThrowBanana(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        Rigidbody itemToThrow;
        throwablesPool.bananasPool[bananasPoolIteration].SetActive(true);
        itemToThrow = throwablesPool.bananasPool[bananasPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);
        itemToThrow.angularDrag = 1;
        itemToThrow.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        itemToThrow.AddRelativeTorque(Vector3.down * 50000, ForceMode.Impulse);

        bananasPoolIteration++;
        if (bananasPoolIteration >= throwablesPool.bananasPool.Length)
            bananasPoolIteration = 0;

        StartCoroutine(SolidifyItemOnceLanded(itemToThrow));
        StartCoroutine(BananaAffectsPrimatesAfterDelay(AOECircleCenterLoc.position));
    }
    IEnumerator BananaAffectsPrimatesAfterDelay(Vector3 throwLandingLocation)
    {
        yield return new WaitForSeconds(throwAirTime);

        Collider[] primatesInRange = Physics.OverlapSphere(throwLandingLocation, banana.affectRange * affectRangeExpansionFactor, primatesLayer, QueryTriggerInteraction.Ignore);
        foreach (Collider primate in primatesInRange)
            if (primate.CompareTag("Monkey"))
                primate.GetComponent<Primate>().GoTowardsPoint(throwLandingLocation);
    }

    IEnumerator SolidifyItemOnceLanded(Rigidbody itemThrown)
    {
        yield return new WaitForSeconds(throwAirTime);

        itemThrown.angularDrag = 250;
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
