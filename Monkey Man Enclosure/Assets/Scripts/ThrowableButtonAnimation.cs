using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableButtonAnimation : MonoBehaviour
{
    Toggle toggleComponent;
    bool selected;

    [Header("Object Graphic")]
    [SerializeField] Transform objectGraphic;
    Vector2 objectGraphicStartingPosition;
    float shakeAmt = 0;
    float curDistanceUp;
    const float DISTANCE_UP = 20;

    [Header("Background Graphic")]
    [SerializeField] Transform backgroundCircle;
    const float IDLE_SCALE = 1;
    const float BOUNCE_SCALE = 1.2f;

    private void Awake()
    {
        objectGraphicStartingPosition = objectGraphic.localPosition;
        toggleComponent = GetComponent<Toggle>();
    }

    private void Start()
    {
        //StartCoroutine(BackgroundCircleBouncing());
        StartCoroutine(ObjectGraphicMoveUpWhenSelected());
        StartCoroutine(RefreshingButtonState());
    }

    void Update()
    {
        toggleComponent.interactable = GameController.instance.curGameState == GameController.GameState.MAINVIEW;
        selected = toggleComponent.isOn;

        if (shakeAmt > 0)
        {
            objectGraphic.localPosition = objectGraphicStartingPosition +
                Vector2.up * curDistanceUp +
                new Vector2(
                    Random.Range(-shakeAmt, shakeAmt),
                    Random.Range(-shakeAmt, shakeAmt));
        }
        else
            objectGraphic.localPosition = objectGraphicStartingPosition + Vector2.up * curDistanceUp;
    }

    private void FixedUpdate()
    {
        if (shakeAmt > 0)
            shakeAmt -= 0.35f;
    }

    public void Shake(float setShakeAmt) => shakeAmt = setShakeAmt;

    IEnumerator ObjectGraphicMoveUpWhenSelected()
    {
        while (true)
        {
            if (selected)
                curDistanceUp = Mathf.Lerp(curDistanceUp, DISTANCE_UP, 0.3f);
            else
                curDistanceUp = Mathf.SmoothStep(curDistanceUp, 0, 0.35f);
            yield return new WaitForFixedUpdate();
        }    
    }

    //IEnumerator BackgroundCircleBouncing()
    //{
    //    float curBackgroundScale = IDLE_SCALE;
    //    while (true)
    //    {
    //        while (!selected)
    //            yield return null;

    //        curBackgroundScale = IDLE_SCALE;
    //        while (curBackgroundScale < BOUNCE_SCALE - 0.01f)
    //        {
    //            curBackgroundScale = Mathf.Lerp(curBackgroundScale, BOUNCE_SCALE, 0.35f);
    //            backgroundCircle.localScale = Vector3.one * curBackgroundScale;
    //            yield return new WaitForFixedUpdate();
    //        }
    //        while (curBackgroundScale > IDLE_SCALE + 0.01f)
    //        {
    //            curBackgroundScale = Mathf.Lerp(curBackgroundScale, IDLE_SCALE, 0.08f);
    //            backgroundCircle.localScale = Vector3.one * curBackgroundScale;
    //            yield return new WaitForFixedUpdate();
    //        }
    //        backgroundCircle.localScale = Vector3.one * IDLE_SCALE;
    //    }
    //}

    IEnumerator RefreshingButtonState()
    {
        while (true)
        {
            toggleComponent.interactable = GameController.instance.curGameState == GameController.GameState.MAINVIEW;
            selected = toggleComponent.isOn;

            yield return new WaitForSeconds(GlobalVariables.UIRefreshInterval);
        }
    }
}
