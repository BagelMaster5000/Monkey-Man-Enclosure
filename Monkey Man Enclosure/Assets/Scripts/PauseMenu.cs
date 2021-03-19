using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    bool visible;
    [SerializeField] Transform pauseMenu;

    private void Start()
    {
        visible = false;

        pauseMenu.gameObject.SetActive(false);
    }

    #region In and Out Animation
    void Update()
    {
        if (GameController.instance.curGameState == GameController.GameState.PAUSED && !visible)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateIn());
        }
        else if (GameController.instance.curGameState != GameController.GameState.PAUSED && visible)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateOut());
        }
    }

    IEnumerator AnimateIn()
    {
        visible = true;

        // Scale in
        pauseMenu.gameObject.SetActive(true);
        float curScale = 1.1f;
        while (curScale > 1.005f)
        {
            pauseMenu.transform.localScale = Vector3.one * curScale;
            curScale = Mathf.Lerp(curScale, 1, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        pauseMenu.transform.localScale = Vector3.one;
    }

    IEnumerator AnimateOut()
    {
        visible = false;

        // Scale out
        float curScale = 1;
        while (curScale > 0.91f)
        {
            pauseMenu.transform.localScale = Vector3.one * curScale;
            curScale -= 0.02f;
            yield return new WaitForFixedUpdate();
        }
        pauseMenu.gameObject.SetActive(false);
    }
    #endregion

    public void ContinueGame()
    {
        GameController.instance.curGameState = GameController.GameState.MAINVIEW;
    }

    public void ExitGame()
    {

    }
}
