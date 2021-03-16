using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Primate : MonoBehaviour
{
    public enum PrimateState { IdleMotionless, IdleWalking, RunningFromSomething, GoingTowardsSomething, Eating }

    [HideInInspector] public PrimateState state = PrimateState.IdleMotionless;
    public GameObject poopPrefab;

    //NavMesh Movement
    NavMeshAgent agent;
    [SerializeField] private float idleWalkDistance = 1f;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void GoTowardsPoint(Vector3 point)
    {
        //TODO Play Monkey sounds

        state = PrimateState.GoingTowardsSomething;

        agent.destination = point;  //Set the navMeshAgent's destination
    }

    public void RunFromPoint(Vector3 point, float runDistance)
    {
        //TODO Play Monkey Sounds

        state = PrimateState.RunningFromSomething;

        Vector3 directionOfPoint = point - transform.position;  //Get direction from monkey to point
        Vector3 oppositeDirection = -directionOfPoint;

        NavMeshHit hit;
        NavMesh.SamplePosition(oppositeDirection, out hit, runDistance, NavMesh.GetAreaFromName("Enclosure")); //Get the closest point on the NavMesh

        //If we don't get a failed position
        if (hit.position != Vector3.zero)
            agent.destination = hit.position;   //Set agent's destination
        else
            RunFromPoint(point, runDistance * 0.5f);
    }

    public void RandomIdlePoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * idleWalkDistance; //Get a random direction
        randomDirection += transform.position;  //Add it to our position

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, idleWalkDistance, NavMesh.GetAreaFromName("Enclosure")); //Get the closest point on the NavMesh

        //If we don't get a failed position
        if (hit.position != Vector3.zero)
            agent.destination = hit.position;   //Set agent's destination
    }

    public virtual void Poop()
    {
        //Create a piece of poop where the primate is
        Instantiate(poopPrefab, transform.position, Quaternion.identity);
    }
}
