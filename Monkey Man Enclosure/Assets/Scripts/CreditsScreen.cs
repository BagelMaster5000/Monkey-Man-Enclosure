using UnityEngine;

public class CreditsScreen : MonoBehaviour
{
    public void Back()
    {
        NextSceneFader.instance.FadeToNextScene("Title Scene", false);
    }
}
