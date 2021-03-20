using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("Clock")]
    [SerializeField, Tooltip("In seconds")] private float hourDuration;                        //The amount of real time that needs to pass for an hour in-game to pass
    private int totalHoursToWin = 8;
    private int hoursPassed = 0;
    private float disruptionFrequency;                                      //How frequent (real time) visitor disruption occur
    [SerializeField] private float hungerRate;                              //How frequent (real time) the Man loses hunger
    [SerializeField, Tooltip("Will be negative")] private float hungerAmount;                        //How much hunger the man loses per the rate
    [SerializeField] private float poopRate;                                //How frequent (real time) primates will poop

    [Header("Dev Only")]
    public float time = 0f;
    private Man man;
    private Monkey[] monkeys;

    // Start is called before the first frame update
    void Start()
    {
        //Set Disruption Frequency
        disruptionFrequency = 100 * GameController.instance.curLevel.disruptionFrequency;

        if (hungerAmount > 0)
            hungerAmount *= -1;

        man = FindObjectOfType<Man>();
        monkeys = FindObjectsOfType<Monkey>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.instance.curGameState == GameController.GameState.MAINVIEW)   //Countdown only while the player is actively managing the monkey exhibit
        {
            //Check for the hour
            if(time >= hourDuration)
            {
                PlayerInventory.instance.RecieveWage();     //Give the player more money
                hoursPassed++;

                //Check to win game
                if (hoursPassed > totalHoursToWin)
                    GameController.instance.Win();

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
                man.ModifyHunger(hungerAmount);

                hungerRate += hungerRate;
            }

            //Check if poop
            if(time >= poopRate)
            {
                MakeMonkeyPoop();      //Choose a random primate to make poop

                poopRate += poopRate;
            }

            time += Time.deltaTime; //Increment the amount of realtime passed
        }
    }


    private void SendDisruption()
    {
        //TODO
    }

    private void MakeMonkeyPoop()
    {
        //Choose a random primate
        Monkey pooper = monkeys[Random.Range(0, monkeys.Length)];

        //Make them poop
        pooper.Poop();
    }
}
