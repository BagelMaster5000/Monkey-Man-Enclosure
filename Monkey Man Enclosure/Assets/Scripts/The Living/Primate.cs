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
    [SerializeField, Range(0, 10)] private float minIdleTime = 1f;
    [SerializeField, Range(0, 10)] private float maxIdleTime = 4f;


    private Animator anim;

    //NavMesh Movement
    NavMeshAgent agent;
    [SerializeField] private float idleWalkDistance = 1f;

    float durationOfFoodAttraction = 4.5f;
    float timeToStopGoingTowardsSomething;

    [Header("Sound Helpers")]
    public GameObject SeeFoodObject;
    public GameObject ScreamObject;
    private SoundPlayer[] seeFoods;
    private SoundPlayer[] screams;
    public SoundPlayer eatingSound;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();

        agent = GetComponent<NavMeshAgent>();

        state = PrimateState.IdleWalking;
        agent.destination = transform.position;

        if (SeeFoodObject != null)
            seeFoods = SeeFoodObject.GetComponents<SoundPlayer>();
        if (ScreamObject != null)
            screams = ScreamObject.GetComponents<SoundPlayer>();
    }

    protected virtual void Update()
    {
        //If we aren't sitting still and have reached the target...
        if (state != PrimateState.IdleMotionless && agent.remainingDistance < agent.stoppingDistance)
        {
            if (state == PrimateState.GoingTowardsSomething)    //Start eating
            {
                state = PrimateState.Eating;

                //Monkey eating sounds
                if (eatingSound != null)
                    eatingSound.Play();

                //Make primate eat
                anim.SetTrigger("Eating");
            }
            else if(state == PrimateState.RunningFromSomething || state == PrimateState.IdleWalking)    //Go to motionless
            {
                BecomeIdle();
            }
        }

        if (state == PrimateState.GoingTowardsSomething && Time.time > timeToStopGoingTowardsSomething)
            BecomeIdle();


        UpdateAnim();
    }

    protected virtual void UpdateAnim()
    {
        if (anim != null)
        {
            if (state == PrimateState.IdleMotionless)
            {
                anim.SetBool("Walking", false);
            }
            else if (state == PrimateState.Eating)
            {
                anim.SetBool("Walking", false);
            }
            else    //state is running, going, or idle walking
            {
                anim.SetBool("Walking", true);
            }
        }
    }

    public void BecomeIdle()
    {
        agent.destination = transform.position;

        state = PrimateState.IdleMotionless;

        //Timer till go to random point (Idle for a psuedo random amount of time)
        StartCoroutine(IdleMotionlessTimer(Random.Range(minIdleTime, maxIdleTime)));
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
        Instantiate(poopPrefab, transform.position,
            Quaternion.Euler(Random.Range(-15, 15), Random.Range(160, 200), Random.Range(-15, 15)));
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

            timeToStopGoingTowardsSomething = Time.time + durationOfFoodAttraction;

            //Play Monkey sounds (see food)
            PlaySound(seeFoods);


            state = PrimateState.GoingTowardsSomething;
            UpdateAnim();

            Vector3 directionOfPoint = point - transform.position;  //Get direction from monkey to point
            Vector3 directionFromPrimate = directionOfPoint + transform.position;

            NavMeshHit hit;
            if(NavMesh.SamplePosition(directionFromPrimate, out hit, 5, ~NavMesh.GetAreaFromName("Enclosure")))   //Get the closest point on the NavMesh
                agent.destination = hit.position;  //Set the navMeshAgent's destination
        }
    }

    public void RunFromPoint(Vector3 point, float runDistance)
    {
        StopCoroutine("IdleMotionlessTimer");

        //Play Monkey Sounds (scream)
        PlaySound(screams);

        state = PrimateState.RunningFromSomething;
        UpdateAnim();

        Vector3 directionOfPoint = point - transform.position;  //Get direction from monkey to point
        Vector3 oppositeDirection = -directionOfPoint * runDistance;
        Vector3 directionFromPrimate = oppositeDirection + transform.position;

        NavMeshHit hit;

        //Check if there is a close point on the navmesh
        if (NavMesh.SamplePosition(directionFromPrimate, out hit, 5, ~NavMesh.GetAreaFromName("Enclosure")))
            agent.destination = hit.position;   //Set agent's destination
        else    //Try a closer point
            RunFromPoint(point, runDistance * 0.5f);
    }

    public void GoToWhistle(Vector3 direction)
    {
        //Direction should be a unit direction towards where the camera is

        StopCoroutine("IdleMotionlessTimer");

        timeToStopGoingTowardsSomething = Time.time + durationOfFoodAttraction;

        //Play Monkey Sounds (whistle)
        PlaySound(seeFoods);

        state = PrimateState.GoingTowardsSomething;
        UpdateAnim();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position + direction.normalized * 20, out hit, 50, ~NavMesh.GetAreaFromName("Enclosure"))) //Get the closest point on the NavMesh
            agent.destination = hit.position;
    }

    #endregion



    public virtual void PlaySound(SoundPlayer[] soundArray)
    {
        int rand = Random.Range(0, soundArray.Length);
        soundArray[rand].Play();
    }
}
