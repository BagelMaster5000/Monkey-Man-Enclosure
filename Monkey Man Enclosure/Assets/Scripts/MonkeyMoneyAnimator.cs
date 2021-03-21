using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonkeyMoneyAnimator : MonoBehaviour
{
    public float tempMoney;

    [Header("Money Pileup")]
    [SerializeField] Image monkeyMoneyRenderer;
    [SerializeField] Sprite[] monkeyMoneyTiers;
    [SerializeField] float[] monkeyMoneyTierAmts;

    [Header("Money Counter")]
    [SerializeField] TMPro.TextMeshProUGUI monkeyMoneyCounter;

    private void Start() => StartCoroutine(RefreshingMoneyGraphicAndText());

    IEnumerator RefreshingMoneyGraphicAndText()
    {
        while (true)
        {
            RefreshMoneyPileup();
            RefreshMoneyCounter();

            yield return new WaitForSeconds(GlobalVariables.UIRefreshInterval);
        }
    }

    private void RefreshMoneyPileup()
    {
        for (int m = monkeyMoneyTierAmts.Length - 1; m >= 0; m--)
        {
            if (tempMoney >= monkeyMoneyTierAmts[m])
            {
                monkeyMoneyRenderer.sprite = monkeyMoneyTiers[m];
                monkeyMoneyRenderer.enabled = true;
                break;
            }
            if (m == 0)
                monkeyMoneyRenderer.enabled = false;
        }
    }

    private void RefreshMoneyCounter()
    {
        monkeyMoneyCounter.text = "$" + tempMoney.ToString("0.00");
    }

}
