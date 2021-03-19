using System.Collections;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] Sound sound;

    void Awake()
    {
        if (sound != null && sound.audioClip)
        {
            sound.audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = sound.volume;
            sound.audioSource.loop = false;
            sound.audioSource.playOnAwake = false;
        }
    }

    public void Play() { sound.audioSource.Play(); }

    public void DetachPlayThenDestroy()
    {
        transform.parent = null;
        sound.audioSource.Play();

        StopAllCoroutines();
        StartCoroutine(DestroyWhenFinished());
    }

    IEnumerator DestroyWhenFinished()
    {
        while (sound.audioSource.isPlaying)
            yield return null;

        Destroy(this.gameObject);
    }
}