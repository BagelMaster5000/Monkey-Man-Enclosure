using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public void LoadGame()
    {
        NextSceneFader.instance.FadeToNextScene("Main Scene", true);
    }
}
