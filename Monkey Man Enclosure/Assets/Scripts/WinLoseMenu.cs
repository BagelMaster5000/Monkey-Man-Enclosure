using System.Collections;
using TMPro;
using UnityEngine;

public class WinLoseMenu : MonoBehaviour
{
    bool visible;
    [SerializeField] Transform winLoseMenu;

    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] TextMeshProUGUI subText;

    private void Start()
    {
        visible = false;

        winLoseMenu.gameObject.SetActive(false);
    }

    #region In and Out Animation
    void Update()
    {
        if (GameController.instance.curGameState == GameController.GameState.GAMEOVER && !visible)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateIn());
        }
    }

    IEnumerator AnimateIn()
    {
        visible = true;

        // Scale in
        winLoseMenu.gameObject.SetActive(true);
        float curScale = 1.1f;
        while (curScale > 1.005f)
        {
            winLoseMenu.transform.localScale = Vector3.one * curScale;
            curScale = Mathf.Lerp(curScale, 1, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        winLoseMenu.transform.localScale = Vector3.one;
    }
    #endregion

    public void SetTexts(bool victory)
    {
        if (victory)
        {
            mainText.text = "Winner!";
            subText.text = "You saved the monkey man!";
        }
        else
        {
            mainText.text = ":(";
            subText.text = "The monkey man a heart attack...";
        }
    }

    public void RestartGame()
    {

    }

    public void ExitGame()
    {

    }
}
