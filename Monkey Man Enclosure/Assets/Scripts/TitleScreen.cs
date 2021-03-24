using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public void LoadGame(int difficulty)
    {
        GlobalVariables.curLevel = difficulty;
        NextSceneFader.instance.FadeToNextScene("Main Scene", true);
    }

    public void GoToOptions()
    {
        NextSceneFader.instance.FadeToNextScene("Options Scene", false);
    }

    public void GoToCredits()
    {
        NextSceneFader.instance.FadeToNextScene("Credits Scene", false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
