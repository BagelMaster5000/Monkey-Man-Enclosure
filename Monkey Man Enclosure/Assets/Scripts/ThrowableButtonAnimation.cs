using UnityEngine;
using UnityEngine.UI;

public class ThrowableButtonAnimation : MonoBehaviour
{
    [SerializeField] Transform objectGraphic;
    Vector2 objectGraphicStartingPosition;

    float shakeAmt = 0;

    Toggle toggleComponent;
    bool selected;

    private void Awake()
    {
        objectGraphicStartingPosition = objectGraphic.localPosition;
        toggleComponent = GetComponent<Toggle>();
    }

    void Update()
    {
        selected = toggleComponent.isOn;

        if (shakeAmt > 0)
        {
            objectGraphic.localPosition = objectGraphicStartingPosition +
                new Vector2(
                    Random.Range(-shakeAmt, shakeAmt),
                    Random.Range(-shakeAmt, shakeAmt));
        }
        else
            objectGraphic.localPosition = objectGraphicStartingPosition;
    }

    private void FixedUpdate()
    {
        if (shakeAmt > 0)
            shakeAmt -= 0.35f;
    }

    public void Shake(float setShakeAmt) => shakeAmt = setShakeAmt;
}
