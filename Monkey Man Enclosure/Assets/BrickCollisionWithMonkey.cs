using UnityEngine;

public class BrickCollisionWithMonkey : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Monkey"))
            collision.gameObject.GetComponent<Monkey>().ProjectilePoop();

        Destroy(this);
    }
}
