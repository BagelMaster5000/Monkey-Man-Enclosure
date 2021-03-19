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

    [Header("Pellet Animations")]
    [SerializeField] GameObject[] pelletsPool;
    int pelletIterator = 0;
    const int NUM_PELLETS_TO_DROP = 5;
    const float DELAY_BETWEEN_DROPS = 0.1f;
    const float PELLET_DROP_POSITION_VARIABILITY = 50;

    [Header("Dispenser Animation")]
    [SerializeField] Animator dispenserAnimator;
    [SerializeField] Transform dispenserHolder;
    Vector3 startDispenserPosition;
    bool dispenserRumbling;
    const float DISPENSER_RUMBLE_AMT = 15;

    [Header("SFX")]
    [SerializeField] SoundPlayer pelletDropSFX;
    [SerializeField] SoundPlayer dispenserCrankSFX;
    [SerializeField] SoundPlayer menuOpeningSFX;
    [SerializeField] SoundPlayer purchaseGoodSFX;
    [SerializeField] SoundPlayer purchaseNeutralSFX;

    private void Start()
    {
        visible = false;

        shopMenu.transform.localPosition = Vector3.right * 1999;
        shopMenu.gameObject.SetActive(false);

        startDispenserPosition = dispenserHolder.localPosition;
    }

    #region In and Out Animation
    void Update()
    {
        if (GameController.instance.curGameState == GameController.GameState.SHOP && !visible)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateIn());

            DeactivateAllPellets();
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

        if (menuOpeningSFX) menuOpeningSFX.Play();

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
        if (purchaseGoodSFX) purchaseGoodSFX.Play();

        dispenserAnimator.SetTrigger("Crank");
        StopAllCoroutines();
        StartCoroutine(DispenserRumbling());
        StartCoroutine(AnimateDroppingFoodPellets());
    }

    public void AttemptBuyBrick()
    {
        // TODO
        if (purchaseNeutralSFX) purchaseNeutralSFX.Play();
    }

    public void AttemptBuyBanana()
    {
        // TODO
        if (purchaseNeutralSFX) purchaseNeutralSFX.Play();
    }

    IEnumerator AnimateDroppingFoodPellets()
    {
        if (dispenserCrankSFX) dispenserCrankSFX.Play();

        int curPellet = pelletIterator;
        pelletIterator += NUM_PELLETS_TO_DROP;
        if (pelletIterator >= pelletsPool.Length)
            pelletIterator -= pelletsPool.Length;
        for (int n = 0; n < NUM_PELLETS_TO_DROP; n++)
        {
            pelletsPool[curPellet].SetActive(false);
            yield return null;
            pelletsPool[curPellet].SetActive(true);
            pelletsPool[curPellet].transform.localPosition =
                new Vector3(
                    Random.Range(-PELLET_DROP_POSITION_VARIABILITY, PELLET_DROP_POSITION_VARIABILITY),
                    Random.Range(-PELLET_DROP_POSITION_VARIABILITY, PELLET_DROP_POSITION_VARIABILITY),
                    Random.Range(-PELLET_DROP_POSITION_VARIABILITY, PELLET_DROP_POSITION_VARIABILITY));

            curPellet++;
            if (curPellet >= pelletsPool.Length)
                curPellet = 0;

            if (pelletDropSFX) pelletDropSFX.Play();

            yield return new WaitForSeconds(DELAY_BETWEEN_DROPS);
        }

        yield return new WaitForSeconds(0.1f);

        dispenserRumbling = false;
    }

    void DeactivateAllPellets()
    {
        foreach (GameObject pelletObject in pelletsPool)
            pelletObject.SetActive(false);
    }

    IEnumerator DispenserRumbling()
    {
        dispenserRumbling = true;

        while (dispenserRumbling)
        {
            dispenserHolder.localPosition = startDispenserPosition +
                new Vector3(
                    Random.Range(-DISPENSER_RUMBLE_AMT, DISPENSER_RUMBLE_AMT),
                    Random.Range(-DISPENSER_RUMBLE_AMT, DISPENSER_RUMBLE_AMT),
                    Random.Range(-DISPENSER_RUMBLE_AMT, DISPENSER_RUMBLE_AMT));
            yield return null;
        }
        while ((dispenserHolder.localPosition - startDispenserPosition).magnitude > 0.01f)
        {
            dispenserHolder.localPosition =
                new Vector3(
                    Mathf.Lerp(dispenserHolder.localPosition.x, startDispenserPosition.x, 0.2f),
                    Mathf.Lerp(dispenserHolder.localPosition.y, startDispenserPosition.y, 0.2f),
                    Mathf.Lerp(dispenserHolder.localPosition.z, startDispenserPosition.z, 0.2f));
            yield return null;
        }

        dispenserHolder.localPosition = startDispenserPosition;
    }
    #endregion
}
