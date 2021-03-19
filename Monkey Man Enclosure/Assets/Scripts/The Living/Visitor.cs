using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Visitor : MonoBehaviour
{
    [Header("Visitor")]
    public GameObject throwablePrefab;

    public bool disruptor = false;
    [Header("Throwable Type Chances")]
    [SerializeField, Range(0, 10), Tooltip("Chance a food pellet is thrown. If the chances total more than 10, this will reset to 3.")]
    private float chanceForFood = 3;
    [SerializeField, Range(0, 10), Tooltip("Chance a banana is thrown. If the chances total more than 10, this will reset to 3.")]
    private float chanceForBanana = 3;
    [SerializeField, Range(0, 10), Tooltip("Chance a brick is thrown. If the chances total more than 10, this will reset to 4.")]
    private float chanceForBrick = 4;

    [Header("Throw Times")]
    [SerializeField] private float maxTimeUntilThrow;
    private float timeUntilThrow;
    [SerializeField] private float throwDuration = 3f;

    [Header("NavMesh Helpers")]
    public Transform destination;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void OnEnable()
    {
        if(chanceForFood + chanceForBanana + chanceForBrick > 10)
        {
            chanceForFood = 3;
            chanceForBanana = 3;
            chanceForBrick = 4;
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (destination != null)
            agent.destination = destination.position;
    }

    public void GoTowardsPoint(Vector3 point)
    {
        Vector3 directionOfPoint = point - transform.position;  //Get direction from monkey to point

        NavMeshHit hit;
        NavMesh.SamplePosition(directionOfPoint, out hit, 5, ~NavMesh.GetAreaFromName("Enclosure"));   //Get the closest point on the NavMesh

        agent.destination = point;  //Set the navMeshAgent's destination
    }


    public void ThrowItem()
    {
		//TODO
    }

    //TODO GoAlongPath CoRoutine?
}
