using System.Collections;
using System.Collections.Generic;
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

    [Header("Buttons")]
    [SerializeField] Toggle[] throwableButtons;
    int curThrowableSelection; // 0 Food Pellet, 1 Brick, 2 Banana

    [Header("Pools")]
    [SerializeField] GameObject[] foodPelletsPool;
    int numFoodPelletsToThrow = 5;
    int foodPelletsPoolIteration = 0;
    float throwRandomizor = 0.5f;
    [SerializeField] GameObject[] bricksPool;
    int bricksPoolIteration = 0;
    [SerializeField] GameObject[] bananasPool;
    int bananasPoolIteration = 0;

    private void Start()
    {
        targetingVisual.SetActive(false);
    }


    #region Selecting
    public void SelectFoodPellet() => SelectThrowable(0);
    public void SelectBrick() => SelectThrowable(1);
    public void SelectBanana() => SelectThrowable(2);

    void SelectThrowable(int setSelection)
    {
        curThrowableSelection = setSelection;

        DrawAOECircle(GetAOECircleSizeBasedOnSelection());

        StopAllCoroutines();
        StartCoroutine(Tracking());
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
        DeactivateAllButtons();

        CalculateInitialThrowVelocities(out float initialXVelocity, out float initialYVelocity, out float initialZVelocity);

        switch (curThrowableSelection)
        {
            case 0: ThrowFoodPellets(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 1: ThrowBrick(initialXVelocity, initialYVelocity, initialZVelocity); break;
            case 2: ThrowBanana(initialXVelocity, initialYVelocity, initialZVelocity); break;
        }
    }

    private void DeactivateAllButtons()
    {
        targetingVisual.SetActive(false);
        foreach (Toggle throwableButton in throwableButtons)
            throwableButton.SetIsOnWithoutNotify(false);
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
            foodPelletsPool[foodPelletsPoolIteration].SetActive(true);
            itemToThrow = foodPelletsPool[foodPelletsPoolIteration].GetComponent<Rigidbody>();
            itemToThrow.transform.position =
                throwStartLoc.position +
                new Vector3(
                    Random.Range(-throwRandomizor, throwRandomizor),
                    Random.Range(-throwRandomizor, throwRandomizor),
                    Random.Range(-throwRandomizor, throwRandomizor));
            itemToThrow.useGravity = true;
            itemToThrow.velocity =
                new Vector3(initialXVelocity, initialYVelocity, initialZVelocity) +
                new Vector3(
                    Random.Range(-throwRandomizor, throwRandomizor),
                    Random.Range(-throwRandomizor, throwRandomizor),
                    Random.Range(-throwRandomizor, throwRandomizor));

            foodPelletsPoolIteration++;
            if (foodPelletsPoolIteration >= foodPelletsPool.Length)
                foodPelletsPoolIteration = 0;
        }
    }

    private void ThrowBrick(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        Rigidbody itemToThrow;
        bricksPool[bricksPoolIteration].SetActive(true);
        itemToThrow = bricksPool[bricksPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);

        bricksPoolIteration++;
        if (bricksPoolIteration >= bricksPool.Length)
            bricksPoolIteration = 0;
    }

    private void ThrowBanana(float initialXVelocity, float initialYVelocity, float initialZVelocity)
    {
        Rigidbody itemToThrow;
        bananasPool[bananasPoolIteration].SetActive(true);
        itemToThrow = bananasPool[bananasPoolIteration].GetComponent<Rigidbody>();
        itemToThrow.transform.position = throwStartLoc.position;
        itemToThrow.useGravity = true;
        itemToThrow.velocity = new Vector3(initialXVelocity, initialYVelocity, initialZVelocity);

        bananasPoolIteration++;
        if (bananasPoolIteration >= bananasPool.Length)
            bananasPoolIteration = 0;
    }
    #endregion
}
