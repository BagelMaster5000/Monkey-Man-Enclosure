using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private float hourDuration;                        //The amount of real time that needs to pass for an hour in-game to pass
    [SerializeField] private float disruptionFrequency;                 //How frequent (real time) visitor disruption occur
    [SerializeField] private float hungerRate;                          //How frequent (real time) the Man loses hunger
    [SerializeField] private float poopRate;                            //How frequent (real time) primates will poop

    private float time = 0f;
    private Man man;
    private Primate[] primates;

    // Start is called before the first frame update
    void Start()
    {
        man = FindObjectOfType<Man>();
        primates = FindObjectsOfType<Primate>();
    }

    // Update is called once per frame
    void Update()
    {
        if(true)//TODO GameController.curGameState == GameState.MainView)   //Countdown only while the player is actively managing the monkey exhibit
        {
            //Check for the hour
            if(time >= hourDuration)
            {
                PlayerInventory.instance.RecieveWage();     //Give the player more money

                hourDuration += hourDuration;
            }

            //Check for a new disrutption
            if(time >= disruptionFrequency)
            {
                SendDisruption();       //Send in a visitor to make a disruption

                disruptionFrequency += disruptionFrequency;
            }

            //Check if hunger
            if(time >= hungerRate)
            {
                man.ModifyHunger(1f);   //TODO consider changing this?

                hungerRate += hungerRate;
            }

            //Check if poop
            if(time >= poopRate)
            {
                MakePrimatePoop();      //Choose a random primate to make poop

                poopRate += poopRate;
            }

            time += Time.deltaTime; //Increment the amount of realtime passed
        }
    }


    private void SendDisruption()
    {
        //TODO
    }

    private void MakePrimatePoop()
    {
        Primate pooper = primates[Random.Range(0, primates.Length)];

        //TODO
    }
}
