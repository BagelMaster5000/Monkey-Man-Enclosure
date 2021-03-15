using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Primate : MonoBehaviour
{
    public enum PrimateState { IdleMotionless, IdleWalking, RunningFromSomething, GoingTowardsSomething, Eating }

    [HideInInspector] public PrimateState state = PrimateState.IdleMotionless;

    public GameObject poopPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GoTowardsPoint(Vector3 point)
    {
		//TODO
    }

    public void RunFromPoint(Vector3 point)
    {
		//TODO
    }

    public virtual void Poop()
    {
        //Create a piece of poop where the primate is
        Instantiate(poopPrefab, transform.position, Quaternion.identity);
    }
}
