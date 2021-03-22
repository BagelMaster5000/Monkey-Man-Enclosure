using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableManager : MonoBehaviour
{
    [SerializeField] Camera cam;

    [Header("Scriptable Objects")]
    [SerializeField] ThrowableSO foodPellets;
    [SerializeField] ThrowableSO brick;
    [SerializeField] ThrowableSO banana;

    [Header("Targeting")]
    Coroutine trackingCoroutine;
    [SerializeField] LayerMask throwableSurface;
    bool tracking;
    [SerializeField] GameObject targetingVisual;

    [Space(10)]
    [SerializeField] Transform AOECircleCenterLoc;
    [SerializeField] Transform AOECircleTransform;
    [SerializeField] LineRenderer AOECircle;
    int pointsInAOECircle = 30;
    float AOECircleRotationSpeed = 15;
    [SerializeField] Bounds AOECircleClampingBounds;

    [Space(10)]
    [SerializeField] Transform throwStartLoc;
    [SerializeField] LineRenderer throwArc;
    [SerializeField] float throwAirTime = 2;
    int pointsInThrowArc = 20;
    // [SerializeField] float throwHeight = 2;

    [Space(10)]
    [SerializeField] LayerMask primatesLayer;

    [Header("Buttons")]
    [SerializeField] Toggle[] throwableButtons = new Toggle[3];
    int curThrowableSelection; // 0 Food Pellet, 1 Brick, 2 Banana
    [SerializeField] TextMeshProUGUI[] throwableAmtTexts = new TextMeshProUGUI[3];

    [Header("Pools")]
    [SerializeField] GameObject[] foodPelletsPool;
    int numFoodPelletsToThrow = 5;
    int foodPelletsPoolIteration = 0;
    float pelletsThrowRandomizer = 0.5f;
    [SerializeField] GameObject[] bricksPool;
    int bricksPoolIteration = 0;
    [SerializeField] GameObject[] bananasPool;
    int bananasPoolIteration = 0;

    [Header("SFX")]
    [SerializeField] SoundPlayer throwableSelectedSFX;
    [SerializeField] SoundPlayer throwWooshSFX;

    private void Start()
    {
        targetingVisual.SetActive(false);

        StartCoroutine(RefreshingButtonStates());
    }


    #region Selecting
    public void SelectFoodPellet() => SelectThrowable(0);
    public void SelectBrick() => SelectThrowable(1);
    public void SelectBanana() => SelectThrowable(2);

    void SelectThrowable(int setSelection)
    {
        //if (GameController.instance.curGameState != GameController.GameState.MAINVIEW)
        //{
        //    TurnOffAllThrowableButtons();
        //    return;
        //}

        

        if (throwableSelectedSFX) throwableSelectedSFX.Play();

        curThrowableSelection = setSelection;

        DrawAOECircle(GetAOECircleSizeBasedOnSelection());

        if (trackingCoroutine != null)
            StopCoroutine(trackingCoroutine);
        trackingCoroutine = StartCoroutine(Tracking());
    }

    private float GetAOECircleSizeBasedOnSelection()
    {
        float sizeOfCircle = 0;

        switch (curThrowableSelection)
        {
            case 0: sizeOfCircle = foodPellets.affectRange; break;
            case 1: sizeOfCircle = brick.affectRange; break;
            case 2: sizeOfCircle = banana.affectRange; break;
        }

        return sizeOfCircle;
    }
    #endregion


    #region Tracking
    IEnumerator Tracking()
    {
        tracking = true;
        while (tracking)
        {
            // Raycasting for throw-to position
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 999, throwableSurface))
            {
                SetAOECenterPos(hit);
                RotateAOECircle();

                DrawPreviewThrowArc();
            }

            // Checking if thrown
            if (Input.GetButtonDown("Fire1"))
            {
                tracking = false;
                Throw();
                break;
            }
            else
                yield return null;
        }
    }

    private void DrawAOECircle(float sizeOfCircle)
    {
        targetingVisual.SetActive(true);

        AOECircle.positionCount = pointsInAOECircle + 2;
        float angle = 0;
        for (int i = 0; i < pointsInAOECircle + 2; i++)
        {
            float x = Mathf.Cos(angle) * sizeOfCircle;
            float z = Mathf.Sin(angle) * sizeOfCircle;

            AOECircle.SetPosition(i, new Vector3(x, 0, z));

            angle += 2 * Mathf.PI / pointsInAOECircle;
        }
    }

    private void RotateAOECircle()
    {
        AOECircleTransform.Rotate(Vector3.up, Time.deltaTime * AOECircleRotationSpeed);
    }

    private void SetAOECenterPos(RaycastHit hit)
    {
        AOECircleCenterLoc.position =
            Vector3.up * hit.point.y +
            Vector3.right * Mathf.Clamp(hit.point.x, AOECircleClampingBounds.min.x, AOECircleClampingBounds.max.x) +
            Vector3.forward * Mathf.Clamp(hit.point.z, AOECircleClampingBounds.min.z, AOECircleClampingBounds.max.z);
    }

    void DrawPreviewThrowArc()
    {
        CalculateInitialThrowVelocities(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity);

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
    void Throw()
    {
        if (throwWooshSFX) throwWooshSFX.Play();

        CalculateInitialThrowVelocities(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity);

        switch (curThrowableSelection)
        {
            case 0: ThrowFoodPellets(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 1: ThrowBrick(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 2: ThrowBanana(initialXVelocity, initialYVelocity, initialZVelocity); break;
        }

        targetingVisual.SetActive(false);

        TurnOffAllThrowableButtons();
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
        PlayerInventory.instance.SubFoodPellet();

        Rigidbody itemToThrow;
        for (int x = 0; x < numFoodPelletsToThrow; x++)
        {
            foodPelletsPool[foodPelletsPoolIteration].SetActive(true);
            itemToThrow = foodPelletsPool[foodPelletsPoolIteration].GetComponent<Rigidbody>();
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
            if (foodPelletsPoolIteration >= foodPelletsPool.Length)
                foodPelletsPoolIteration = 0;
        }

        StartCoroutine(FoodPelletsAffectPrimatesAfterDelay(AOECircleCenterLoc.position));
    }
    IEnumerator FoodPelletsAffectPrimatesAfterDelay(Vector3 throwLandingLocation)
    {
        yield return new WaitForSeconds(throwAirTime);

        Collider[] primatesInRange = Physics.OverlapSphere(throwLandingLocation, foodPellets.affectRange, primatesLayer);
        foreach (Collider primate in primatesInRange)
            primate.GetComponent<Primate>().GoTowardsPoint(throwLandingLocation);
    }

    private void ThrowBrick(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        PlayerInventory.instance.SubBrick();

        Rigidbody itemToThrow;
        bricksPool[bricksPoolIteration].SetActive(true);
        itemToThrow = bricksPool[bricksPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);

        bricksPoolIteration++;
        if (bricksPoolIteration >= bricksPool.Length)
            bricksPoolIteration = 0;

        StartCoroutine(BrickAffectsPrimatesAfterDelay(AOECircleCenterLoc.position));
    }
    IEnumerator BrickAffectsPrimatesAfterDelay(Vector3 throwLandingLocation)
    {
        yield return new WaitForSeconds(throwAirTime);

        Collider[] primatesInRange = Physics.OverlapSphere(throwLandingLocation, brick.affectRange, primatesLayer);
        foreach (Collider primate in primatesInRange)
            primate.GetComponent<Primate>().RunFromPoint(throwLandingLocation, brick.affectRange * 1.2f);
    }

    private void ThrowBanana(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        PlayerInventory.instance.SubBanana();

        Rigidbody itemToThrow;
        bananasPool[bananasPoolIteration].SetActive(true);
        itemToThrow = bananasPool[bananasPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);

        bananasPoolIteration++;
        if (bananasPoolIteration >= bananasPool.Length)
            bananasPoolIteration = 0;

        StartCoroutine(BananaAffectsPrimatesAfterDelay(AOECircleCenterLoc.position));
    }
    IEnumerator BananaAffectsPrimatesAfterDelay(Vector3 throwLandingLocation)
    {
        yield return new WaitForSeconds(throwAirTime);

        Collider[] primatesInRange = Physics.OverlapSphere(throwLandingLocation, banana.affectRange, primatesLayer);
        foreach (Collider primate in primatesInRange)
            if (primate.CompareTag("Monkey"))
                primate.GetComponent<Primate>().GoTowardsPoint(throwLandingLocation);
    }
    #endregion

    IEnumerator RefreshingButtonStates()
    {
        PlayerInventory inventory = PlayerInventory.instance;
        while (true)
        {
            throwableButtons[0].interactable = inventory.GetSupplies().foodPelletAmt > 0 &&
                GameController.instance.curGameState == GameController.GameState.MAINVIEW;
            throwableButtons[1].interactable = inventory.GetSupplies().brickAmt > 0 &&
                GameController.instance.curGameState == GameController.GameState.MAINVIEW;
            throwableButtons[2].interactable = inventory.GetSupplies().bananaAmt > 0 &&
                GameController.instance.curGameState == GameController.GameState.MAINVIEW;

            throwableAmtTexts[0].text = inventory.GetSupplies().foodPelletAmt.ToString();
            throwableAmtTexts[1].text = inventory.GetSupplies().brickAmt.ToString();
            throwableAmtTexts[2].text = inventory.GetSupplies().bananaAmt.ToString();

            yield return new WaitForSeconds(GlobalVariables.UIRefreshInterval);
        }
    }

    void TurnOffAllThrowableButtons()
    {
        foreach (Toggle throwableButton in throwableButtons)
            throwableButton.SetIsOnWithoutNotify(false);
    }
}
