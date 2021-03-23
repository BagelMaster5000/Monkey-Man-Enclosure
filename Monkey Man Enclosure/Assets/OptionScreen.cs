using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionScreen : MonoBehaviour
{
    public void Back()
    {
        NextSceneFader.instance.FadeToNextScene("Title Scene", true);
    }
}
