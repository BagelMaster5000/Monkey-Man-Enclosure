using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : Primate
{
    [Header("Monkey")]
    private float awakeRate = 1f;
    private float closestAwakeAmount = 5f;
    private bool nearMan = false;


    [SerializeField, Range(0, 11), Tooltip("Must get at or above this number for the monkey to launch poop.\n(0 = Always projectile poop, 11 = Never projectile poop)")]
    private int chanceToProjectilePoop = 3;
    private Man man;

    protected override void Start()
    {
        if (man == null)
            man = GameObject.FindGameObjectWithTag("MonkeyMan").GetComponent<Man>();

        StartCoroutine(MakeManAwake());

        base.Start();
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


    protected void OnTriggerEnter(Collider other)
    {
        if(other.tag == "MonkeyMan")
        {
            //Make the man more awake
            nearMan = true;
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.tag == "MonkeyMan")
        {
            //Stop making the man awake
            nearMan = false;
        }
    }

    private IEnumerator MakeManAwake()
    {
        while (true)
        {
            if (nearMan == true)
            {
                //Wait fo the awake rate before making the man awake
                yield return new WaitForSeconds(awakeRate);

                //Get the distance between man and monkey
                float distance = Vector3.Distance(man.transform.position, transform.position);

                //Base awake amount times how far away
                man.ModifySleep(Mathf.Min(closestAwakeAmount / distance, closestAwakeAmount));
            }
            yield return null;
        }
    }


    public void LookAtTarget(Vector3 lookTarget)
    {
        //TODO?
    }
}
