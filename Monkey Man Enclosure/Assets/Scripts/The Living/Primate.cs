using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Primate : MonoBehaviour
{
    public enum PrimateState { IdleMotionless, IdleWalking, RunningFromSomething, GoingTowardsSomething, Eating }

    [HideInInspector] public PrimateState state;

    [Header("Primate")]
    public GameObject poopPrefab;
    [SerializeField, Range(0, 10)] private float maxIdleTime = 5f;
    [SerializeField] private float eatingTime = 3f;

    //NavMesh Movement
    NavMeshAgent agent;
    [SerializeField] private float idleWalkDistance = 1f;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        state = PrimateState.IdleWalking;
        agent.destination = transform.position;
    }

    private void Update()
    {
        //If we aren't sitting still and have reached the target...
        if (state != PrimateState.IdleMotionless && agent.remainingDistance < agent.stoppingDistance)
        {
            if (state == PrimateState.GoingTowardsSomething)    //Start eating
            {
                state = PrimateState.Eating;

                //TODO Monkey eating sounds

                //TODO Eating timer
            }
            else if(state == PrimateState.RunningFromSomething || state == PrimateState.IdleWalking)    //Go to motionless
            {
                state = PrimateState.IdleMotionless;

                //Timer till go to random point (Idle for a psuedo random amount of time)
                StartCoroutine(IdleMotionlessTimer(Random.Range(0, maxIdleTime)));
            }
        }
        
    }

    public IEnumerator IdleMotionlessTimer(float timeToSit)
    {
        float time = 0;
        while (time < timeToSit)
        {
            time += Time.deltaTime;
            yield return null;
        }

        GoToRandomPoint();
    }

    public void GoToRandomPoint()
    {
        //Only go to a random point if we aren't doing anything
        if (state == PrimateState.IdleMotionless)
        {
            Vector3 randomDirection = Random.insideUnitSphere * idleWalkDistance; //Get a random direction
            randomDirection += transform.position;  //Add it to our position 

            NavMeshHit hit;
            //Check if there is a close point on the navmesh
            if (NavMesh.SamplePosition(randomDirection, out hit, 5, ~NavMesh.GetAreaFromName("Enclosure")))
            {
                state = PrimateState.IdleWalking;

                agent.destination = hit.position;   //Set agent's destination
            }
            else
                GoToRandomPoint();
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
            StopCoroutine("IdleMotionlessTimer");

            //If we are currently going to another source of food, and our current destination is closer, then do not go to the new point
            if (state == PrimateState.GoingTowardsSomething && agent.remainingDistance < Vector3.Distance(point, transform.position))
                return;


            //TODO Play Monkey sounds

            state = PrimateState.GoingTowardsSomething;

            Vector3 directionOfPoint = point - transform.position;  //Get direction from monkey to point

            NavMeshHit hit;
            NavMesh.SamplePosition(directionOfPoint, out hit, 5, ~NavMesh.GetAreaFromName("Enclosure"));   //Get the closest point on the NavMesh

            agent.destination = point;  //Set the navMeshAgent's destination
        }
    }

    public void RunFromPoint(Vector3 point, float runDistance)
    {
        StopCoroutine("IdleMotionlessTimer");

        //TODO Play Monkey Sounds

        state = PrimateState.RunningFromSomething;

        Vector3 directionOfPoint = point - transform.position;  //Get direction from monkey to point
        Vector3 oppositeDirection = -directionOfPoint * runDistance;

        NavMeshHit hit;

        //Check if there is a close point on the navmesh
        if (NavMesh.SamplePosition(oppositeDirection, out hit, 5, ~NavMesh.GetAreaFromName("Enclosure")))
            agent.destination = hit.position;   //Set agent's destination
        else    //Try a closer point
            RunFromPoint(point, runDistance * 0.5f);
    }

    public void GoToWhistle(Vector3 direction)
    {
        //Direction should be a unit direction towards where the camera is

        StopAllCoroutines();

        //TODO Play Monkey Sounds

        state = PrimateState.GoingTowardsSomething;

        NavMeshHit hit;
        NavMesh.SamplePosition(direction.normalized * 20, out hit, 50, ~NavMesh.GetAreaFromName("Enclosure")); //Get the closest point on the NavMesh
    }

    #endregion

}
