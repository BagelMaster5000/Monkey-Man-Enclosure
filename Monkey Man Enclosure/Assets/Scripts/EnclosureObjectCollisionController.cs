using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
[RequireComponent(typeof(Rigidbody))]
public class EnclosureObjectCollisionController : MonoBehaviour
{
    SoundPlayer collisionSFX;
    bool collidedOnce = false;

    [SerializeField] float disappearDelay = 7;
    Rigidbody rb;
    LayerMask throwableSurface;

    private void OnEnable()
    {
        collidedOnce = false;
        StopAllCoroutines();
        StartCoroutine(ShrinkIntoNonexistenceAfterDelay());
        rb.constraints = RigidbodyConstraints.None;
    }

    private void Awake()
    {
        throwableSurface = LayerMask.NameToLayer("ThrowableSurface");
        collisionSFX = GetComponent<SoundPlayer>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collidedOnce)
        {
            collidedOnce = true;

            collisionSFX.Play();
        }

        if (collision.gameObject.layer == throwableSurface)
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
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
