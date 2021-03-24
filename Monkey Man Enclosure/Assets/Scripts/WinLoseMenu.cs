using System.Collections;
using TMPro;
using UnityEngine;

public class WinLoseMenu : MonoBehaviour
{
    bool visible;
    [SerializeField] Transform winLoseMenu;
    [SerializeField] GameObject monkeyManInMenu;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI mainText;
    [SerializeField] TextMeshProUGUI subText;

    [Header("Animations")]
    [SerializeField] Animator menuAnimator;
    [SerializeField] Animator monkeyManAnimator;

    private void Start()
    {
        visible = false;
        monkeyManInMenu.SetActive(true); // For animation to work
    }

    #region In and Out Animation
    public void Victory()
    {
        SetTexts(true);
        BeginAnimations(true);
        StartCoroutine(AnimateIn(true));
    }

    public void Failure()
    {
        SetTexts(false);
        BeginAnimations(false);
        StartCoroutine(AnimateIn(false));
    }

    IEnumerator AnimateIn(bool victory)
    {
        visible = true;

        // Scale in
        float curScale = 1.4f;
        while (curScale > 1.29)
        {
            winLoseMenu.localScale = Vector3.one * curScale;
            curScale = Mathf.Lerp(curScale, 1.28364f, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        winLoseMenu.localScale = Vector3.one * 1.28364f;
    }

    private void BeginAnimations(bool victory)
    {
        if (victory)
        {
            menuAnimator.SetTrigger("Victory");
            monkeyManAnimator.SetTrigger("Victory");
        }
        else
        {
            menuAnimator.SetTrigger("Failure");
            monkeyManAnimator.SetTrigger("Failure");
        }
    }

    private void SetTexts(bool victory)
    {
        if (victory)
        {
            mainText.text = "Winner!";
            subText.text = "You saved the monkey man!";
        }
        else
        {
            mainText.text = ":(";
            subText.text = "Monkey man had a heart attack...";
        }
    }
    #endregion

    public void RestartGame()
    {
        NextSceneFader.instance.FadeToNextScene("Main Scene", false);
    }

    public void ExitGame()
    {
        NextSceneFader.instance.FadeToNextScene("Title Scene", true);
        MusicPlayer.instance.FadeOut();
    }
}
