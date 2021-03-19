using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
public class CollisionSoundPlayer : MonoBehaviour
{
    SoundPlayer collisionSFX;
    float timeUntilNextPossiblePlay;
    const float DELAY_BETWEEN_PLAYS = 5;

    private void Awake()
    {
        collisionSFX = GetComponent<SoundPlayer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time > timeUntilNextPossiblePlay)
        {
            collisionSFX.Play();
            timeUntilNextPossiblePlay = Time.time + DELAY_BETWEEN_PLAYS;
        }
    }
}
