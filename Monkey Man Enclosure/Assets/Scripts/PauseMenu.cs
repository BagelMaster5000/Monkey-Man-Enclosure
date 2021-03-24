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
            Time.timeScale = 0;
            StopAllCoroutines();
            StartCoroutine(AnimateIn());
        }
        else if (GameController.instance.curGameState != GameController.GameState.PAUSED && visible)
        { 
            Time.timeScale = 1;
            visible = false;
            pauseMenu.gameObject.SetActive(false);
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
            yield return new WaitForSecondsRealtime(0.02f);
        }
        pauseMenu.transform.localScale = Vector3.one;
    }
    #endregion

    public void ContinueGame()
    {
        Time.timeScale = 1;
        GameController.instance.curGameState = GameController.GameState.MAINVIEW;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        NextSceneFader.instance.FadeToNextScene("Main Scene", false);
    }

    public void ExitGame()
    {
        Time.timeScale = 1;
        NextSceneFader.instance.FadeToNextScene("Title Scene", true);
        MusicPlayer.instance.FadeOut();
    }
}
