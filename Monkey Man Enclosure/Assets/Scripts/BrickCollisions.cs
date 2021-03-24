using UnityEngine;

public class BrickCollisions : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Monkey"))
            collision.gameObject.GetComponent<Monkey>().ProjectilePoop();
        else if (collision.gameObject.CompareTag("MonkeyMan"))
        {
            Man man = collision.gameObject.GetComponent<Man>();
            man.ModifySleep((man.maxAwakeLevel - man.awakeLevel) / 4 + (man.maxAwakeLevel / 4));
        }

        Destroy(this);
    }
}
