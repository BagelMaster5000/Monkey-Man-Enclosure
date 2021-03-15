using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : Primate
{
    [SerializeField] private float baseAwakeAmount = 0.1f;
    [SerializeField] private float awakeRate = 1f;
    [SerializeField] private float distanceModifier = 2f;


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
            //TODO
        }
        else
        {
            //Just do a generic poop
            base.Poop();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Man")
        {
            //TODO make more awake
        }
    }


    public void LookAtTarget(Vector3 lookTarget)
    {
        //TODO?
    }
}
