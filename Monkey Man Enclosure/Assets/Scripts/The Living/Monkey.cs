using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : Primate
{
    [Header("Monkey")]
    private float awakeRate = 1f;
    private float closestAwakeAmount = 1f;


    [SerializeField, Range(0, 11), Tooltip("Must get at or above this number for the monkey to launch poop.\n(0 = Always projectile poop, 11 = Never projectile poop)")]
    private int chanceToProjectilePoop = 3;
    private Man man;

    // Start is called before the first frame update
    private void OnEnable()
    {
        man = FindObjectOfType<Man>();
    }


    public override void Poop()
    {
        if(Random.Range(1,11) >= chanceToProjectilePoop)
        {
            //TODO choose to launch at camera? or launch in random direction

            //TODO Turn the Monkey over???

            //Spawn a piece of poop and get its rigidbody
            Rigidbody body = Instantiate(poopPrefab, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            //Launch the poop
            Vector3 randomPos = Random.insideUnitSphere;
            randomPos.y = 0;
            body.AddExplosionForce(50f, body.transform.position + Vector3.down + randomPos, 5, 10f, ForceMode.Acceleration);
        }
        else
        {
            //Just do a generic poop
            base.Poop();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Man")
        {
            //Make the man more awake

            StartCoroutine(MakeManAwake());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Man")
        {
            //Stop making the man awake

            StopCoroutine(MakeManAwake());
        }
    }

    private IEnumerator MakeManAwake()
    {
        while(true)
        {
            //Wait fo the awake rate before making the man awake
            yield return new WaitForSeconds(awakeRate);

            //Get the distance between man and monkey
            float distance = Vector3.Distance(man.transform.position, transform.position);

            //Base awake amount times how far away
            man.ModifySleep(Mathf.Min(closestAwakeAmount / distance, closestAwakeAmount));
        }
    }


    public void LookAtTarget(Vector3 lookTarget)
    {
        //TODO?
    }
}
