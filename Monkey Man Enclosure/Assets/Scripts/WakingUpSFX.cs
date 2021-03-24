using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakingUpSFX : MonoBehaviour
{
    [SerializeField] Man man;
    [SerializeField] Sound alarm;
    float onVolume;

    private void Awake()
    {
        alarm.audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        alarm.audioSource.clip = alarm.audioClip;
        alarm.audioSource.volume = 0;
        onVolume = alarm.volume;
        alarm.audioSource.loop = true;
    }

    private void Start()
    {
        alarm.audioSource.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (man.awakeLevel >= 75)
            alarm.audioSource.volume = Mathf.Lerp(alarm.audioSource.volume, onVolume, 0.2f);
        else
            alarm.audioSource.volume = Mathf.Lerp(alarm.audioSource.volume, 0, 0.2f);
    }
}
