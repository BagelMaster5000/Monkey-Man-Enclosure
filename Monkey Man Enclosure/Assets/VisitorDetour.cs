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

    private bool throwing = false;
    private bool handlingVisitor = false;

    [Range(0, 1)] public float chanceToCapture = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if(focusAgent != null && focusAgent.remainingDistance < focusAgent.stoppingDistance)
        {
            //If the visitor gets to the enclosure
            if(focusVisitor.state == Visitor.Going.ToRailing && throwing == false)
            {
                throwing = true;

                //Have the visitor throw their item and then leave in an amount of time
                StartCoroutine(DelayThrow());
            }
            else if(focusVisitor.state == Visitor.Going.ToPath)
            {
                //Have the visitor go back to where they were supposed to go
                focusVisitor.state = Visitor.Going.ToDestination;
                focusAgent.destination = focusVisitor.destination.position;

                handlingVisitor = false;
            }
        }
    }

    private IEnumerator DelayThrow()
    {
        yield return new WaitForSeconds(0.1f);

        focusVisitor.ThrowItem();

        yield return new WaitForSeconds(0.1f);

        //Have the visitor go back to where they got captured
        focusVisitor.state = Visitor.Going.ToPath;
        focusAgent.destination = capturePoint.position;
        throwing = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Visitor" && handlingVisitor == false)
        {
            //Get the Visitor
            focusVisitor = other.GetComponent<Visitor>();

            //If they are a disruptor, and we capture them
            if(focusVisitor.disruptor == true && Random.Range(0f, 1f) <= chanceToCapture)
            {
                handlingVisitor = true;

                focusVisitor.disruptor = false; //Prevent this visitor from getting captured in the future
                focusAgent = focusVisitor.GetComponent<NavMeshAgent>();

                //Send the visitor to their destination
                focusVisitor.state = Visitor.Going.ToRailing;
                focusAgent.destination = destinationPoint.position;
            }
        }
    }
}
