using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Visitor")
            Destroy(other.gameObject);
    }
}
