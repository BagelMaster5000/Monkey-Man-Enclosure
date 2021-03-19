using UnityEngine;

[System.Serializable]
public class Sound
{
    // Inspector name
    [HideInInspector] public string name = "Sound";

    [Space(5)]
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.5f;
    [HideInInspector] public AudioSource audioSource;
}