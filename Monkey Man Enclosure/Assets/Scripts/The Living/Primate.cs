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

    [SerializeField] private float eatingTime = 3f;

    //NavMesh Movement
    NavMeshAgent agent;
    [SerializeField] private float idleWalkDistance = 1f;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            if (state == PrimateState.GoingTowardsSomething)
            {
                state = PrimateState.Eating;

                //TODO Monkey eating sounds

                //TODO Eating timer
            }
            else if(state == PrimateState.RunningFromSomething || state == PrimateState.IdleWalking)
            {
                state = PrimateState.IdleMotionless;

                //Timer till go to random point
            }
        }
        
    }

    public void GoToRandomPoint()
    {
        //Only go to a random point if we aren't doing anything
        if (state == PrimateState.IdleMotionless)
        {
            Vector3 randomDirection = Random.insideUnitSphere * idleWalkDistance; //Get a random direction
            randomDirection += transform.position;  //Add it to our position

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, idleWalkDistance, NavMesh.GetAreaFromName("Enclosure")); //Get the closest point on the NavMesh

            //If we don't get a failed position
            if (hit.position != Vector3.zero)
            {
                state = PrimateState.IdleWalking;

                agent.destination = hit.position;   //Set agent's destination
            }
        }
    }

    public virtual void Poop()
    {
        //Create a piece of poop where the primate is
        Instantiate(poopPrefab, transform.position, Quaternion.identity);
    }

    #region Monkey Interuptions

    public void GoTowardsPoint(Vector3 point)
    {
        //Don't go to a food source if we are running from a brick or are currently eating
        if (state != PrimateState.RunningFromSomething || state != PrimateState.Eating)
        {
            //If we are currently going to another source of food, and our current destination is closer, then do not go to the new point
            if (state == PrimateState.GoingTowardsSomething && agent.remainingDistance < Vector3.Distance(point, transform.position))
                return;


            //TODO Play Monkey sounds

            state = PrimateState.GoingTowardsSomething;

            Vector3 directionOfPoint = point - transform.position;  //Get direction from monkey to point

            NavMeshHit hit;
            NavMesh.SamplePosition(directionOfPoint, out hit, Vector3.Distance(point, transform.position), NavMesh.GetAreaFromName("Enclosure"));   //Get the closest point on the NavMesh

            agent.destination = point;  //Set the navMeshAgent's destination
        }
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

    public void GoToWhistle(Vector3 direction)
    {
        //Direction should be a unit direction towards where the camera is

        //TODO Play Monkey Sounds

        state = PrimateState.GoingTowardsSomething;

        NavMeshHit hit;
        NavMesh.SamplePosition(direction.normalized, out hit, 10, NavMesh.GetAreaFromName("Enclosure")); //Get the closest point on the NavMesh
    }

    #endregion

}
