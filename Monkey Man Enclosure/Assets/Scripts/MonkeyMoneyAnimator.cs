using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonkeyMoneyAnimator : MonoBehaviour
{
    [Header("Money Pileup")]
    [SerializeField] Image monkeyMoneyRenderer;
    [SerializeField] Sprite[] monkeyMoneyTiers;
    [SerializeField] float[] monkeyMoneyTierAmts;

    [Header("Money Counter")]
    [SerializeField] TMPro.TextMeshProUGUI monkeyMoneyCounter;

    private void Start() => StartCoroutine(RefreshingMoneyGraphicAndText());

    IEnumerator RefreshingMoneyGraphicAndText()
    {
        PlayerInventory playerInventory = PlayerInventory.instance;
        int curMoney;

        while (true)
        {
            curMoney = playerInventory.GetMoney();
            RefreshMoneyPileup(curMoney);
            RefreshMoneyCounter(curMoney);

            yield return new WaitForSeconds(GlobalVariables.UIRefreshInterval);
        }
    }

    private void RefreshMoneyPileup(int curMoney)
    {
        for (int m = monkeyMoneyTierAmts.Length - 1; m >= 0; m--)
        {
            if (curMoney >= monkeyMoneyTierAmts[m])
            {
                monkeyMoneyRenderer.sprite = monkeyMoneyTiers[m];
                monkeyMoneyRenderer.enabled = true;
                break;
            }
            if (m == 0)
                monkeyMoneyRenderer.enabled = false;
        }
    }

    private void RefreshMoneyCounter(int curMoney)
    {
        monkeyMoneyCounter.text = "$" + curMoney.ToString("0.00");
    }

}
