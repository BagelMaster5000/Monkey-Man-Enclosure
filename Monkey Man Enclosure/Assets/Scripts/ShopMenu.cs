using System.Collections;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
    bool visible;
    [SerializeField] Transform shopMenu;
    const int IN_DISTANCE = 0;
    const int OUT_DISTANCE = 1500;

    float foodPrice = 10;
    float brickPrice = 9;
    float bananaPrice = 8;

    [Header("Pellet Animation")]
    [SerializeField] GameObject[] pellets;
    const int NUM_PELLETS_TO_DROP = 5;
    const float DELAY_BETWEEN_DROPS = 0.1f;

    private void Start()
    {
        visible = false;

        shopMenu.transform.localPosition = Vector3.right * 1999;
        shopMenu.gameObject.SetActive(false);
    }

    #region In and Out Animation
    void Update()
    {
        if (GameController.instance.curGameState == GameController.GameState.SHOP && !visible)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateIn());
        }
        else if (GameController.instance.curGameState != GameController.GameState.SHOP && visible)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateOut());
        }
    }

    IEnumerator AnimateIn()
    {
        visible = true;

        // Slide in from right
        shopMenu.gameObject.SetActive(true);
        while (shopMenu.transform.localPosition.x > IN_DISTANCE + 1)
        {
            shopMenu.transform.localPosition =
                Vector3.right * Mathf.SmoothStep(shopMenu.transform.localPosition.x, IN_DISTANCE, 0.3f);
            yield return new WaitForFixedUpdate();
        }
        shopMenu.transform.localPosition = Vector3.right * IN_DISTANCE;
    }

    IEnumerator AnimateOut()
    {
        visible = false;

        // Punch back
        float curScale = 0.95f;
        shopMenu.transform.localScale = Vector3.one * curScale;
        while (curScale < 0.99f)
        {
            curScale = Mathf.SmoothStep(curScale, 1, 0.3f);
            shopMenu.transform.localScale = Vector3.one * curScale;
            yield return new WaitForFixedUpdate();
        }
        shopMenu.transform.localScale = Vector3.one;

        //yield return new WaitForSeconds(0.1f);

        // Slide out to right
        while (shopMenu.transform.localPosition.x < OUT_DISTANCE - 5)
        {
            shopMenu.transform.localPosition += 
                Vector3.right * Mathf.Lerp(shopMenu.transform.localPosition.x, OUT_DISTANCE, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        shopMenu.transform.localPosition = Vector3.right * OUT_DISTANCE;
        shopMenu.gameObject.SetActive(false);
    }
    #endregion

    #region Purchasing
    public void AttemptBuyFoodPellets()
    {
        // TODO
    }

    public void AttemptBuyBrick()
    {
        // TODO
    }

    public void AttemptBuyBanana()
    {
        // TODO
    }

    void AnimateDroppingFoodPellets()
    {
        // TODO
        //for (int x = 0; x < numFoodPelletsToThrow; x++)
        //{
        //    foodPelletsPool[foodPelletsPoolIteration].SetActive(true);
        //    itemToThrow = foodPelletsPool[foodPelletsPoolIteration].GetComponent<Rigidbody>();
        //    itemToThrow.transform.position =
        //        throwStartLoc.position +
        //        new Vector3(
        //            Random.Range(-throwRandomizor, throwRandomizor),
        //            Random.Range(-throwRandomizor, throwRandomizor),
        //            Random.Range(-throwRandomizor, throwRandomizor));
        //    itemToThrow.useGravity = true;
        //    itemToThrow.velocity =
        //        new Vector3(initialXVelocity, initialYVelocity, initialZVelocity) +
        //        new Vector3(
        //            Random.Range(-throwRandomizor, throwRandomizor),
        //            Random.Range(-throwRandomizor, throwRandomizor),
        //            Random.Range(-throwRandomizor, throwRandomizor));

        //    foodPelletsPoolIteration++;
        //    if (foodPelletsPoolIteration >= foodPelletsPool.Length)
        //        foodPelletsPoolIteration = 0;
        //}
    }
    #endregion
}
