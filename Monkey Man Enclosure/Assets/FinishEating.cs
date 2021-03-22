using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishEating : MonoBehaviour
{
    private Primate thisPrimate;

    private void Start()
    {
        thisPrimate = GetComponentInParent<Primate>();
    }

    public void StopEating()
    {
        thisPrimate.BecomeIdle();
    }
}
