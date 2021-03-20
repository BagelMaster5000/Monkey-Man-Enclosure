using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VisitorDetour : MonoBehaviour
{
    private Visitor focusVisitor;
    private NavMeshAgent focusAgent;

    public Transform capturePoint;
    public Transform destinationPoint;

    [Range(0, 1)] public float chanceToCapture = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if(focusAgent != null && focusAgent.remainingDistance < focusAgent.stoppingDistance)
        {
            //If the visitor gets to the enclosure
            if(focusAgent.destination == destinationPoint.position)
            {
                //Have the visitor throw their item and then leave in an amount of time
                StartCoroutine(DelayThrow());
            }
            else if(focusAgent.destination == capturePoint.position)
            {
                //Have the visitor go back to where they were supposed to go
                focusAgent.destination = focusVisitor.destination.position;
            }
        }
    }

    private IEnumerator DelayThrow()
    {
        yield return new WaitForSeconds(0.1f);

        focusVisitor.ThrowItem();

        yield return new WaitForSeconds(0.1f);

        //Have the visitor go back to where they got captured
        focusAgent.destination = capturePoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Visitor")
        {
            //Get the Visitor
            focusVisitor = other.GetComponent<Visitor>();

            //If they are a disruptor, and we capture them
            if(focusVisitor.disruptor == true && Random.Range(0f, 1f) <= chanceToCapture)
            {
                focusVisitor.disruptor = false; //Prevent this visitor from getting captured in the future
                focusAgent = focusVisitor.GetComponent<NavMeshAgent>();

                //Send the visitor to their destination
                focusAgent.destination = destinationPoint.position;
            }
        }
    }
}
