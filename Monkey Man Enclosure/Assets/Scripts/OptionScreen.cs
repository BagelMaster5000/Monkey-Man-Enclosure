using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text soundValueLabel = null;
    [SerializeField] private Slider soundValue = null;

    // Start is called before the first frame update
    void Start()
    {
        soundValue.value = (int)(GameManager.soundSettings * 10);
        soundValueLabel.text = (int)(soundValue.value * 10) + "%";

        //AudioManager.instance.Play("Menu");
    }

    private void OnEnable()
    {
        soundValue.value = (int)(GameManager.soundSettings * 10);
    }

    //Volume
    public void adjustVolume()
    {
        soundValueLabel.text = (int)(soundValue.value * 10) + "%";
        GameManager.soundSettings = soundValue.value / 10;

        AudioListener.volume = GameManager.soundSettings;
    }


    public void Back()
    {
        NextSceneFader.instance.FadeToNextScene("Title Scene", true);
    }
}
