using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
public class EnclosureObjectCollisionController : MonoBehaviour
{
    SoundPlayer collisionSFX;
    bool collidedOnce = false;

    [SerializeField] float disappearDelay = 7;

    private void OnEnable()
    {
        collidedOnce = false;
        StopAllCoroutines();
        StartCoroutine(ShrinkIntoNonexistenceAfterDelay());
    }

    private void Awake()
    {
        collisionSFX = GetComponent<SoundPlayer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collidedOnce)
        {
            collidedOnce = true;

            collisionSFX.Play();
        }
    }

    IEnumerator ShrinkIntoNonexistenceAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay);

        Vector3 startingScale = transform.localScale;
        float curScale = transform.localScale.x;
        while (curScale > 0.02f)
        {
            curScale = Mathf.SmoothStep(curScale, 0, 0.12f);
            transform.localScale = Vector3.one * curScale;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);

        transform.localScale = startingScale;
    }
}
