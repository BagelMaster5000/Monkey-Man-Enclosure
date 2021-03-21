using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [Header("Clock")]
    [SerializeField, Tooltip("In seconds")] private float hourDuration;                        //The amount of real time that needs to pass for an hour in-game to pass
    private int totalHoursToWin = 8;
    private int hoursPassed = 0;
    public float visitorFrequency = .15f;                                      //How frequent (real time) visitors occur
    private float disruptionFrequency;                                      //How frequent (real time) visitor disruption occur
    [SerializeField] private float hungerRate;                              //How frequent (real time) the Man loses hunger
    [SerializeField, Tooltip("Will be negative")] private float hungerAmount;                        //How much hunger the man loses per the rate
    [SerializeField] private float poopRate;                                //How frequent (real time) primates will poop

    [Header("Dev Only")]
    public float time = 0f;
    private Man man;
    private Monkey[] monkeys;

    [Header("Visitor Helpers")]
    public GameObject visitorPrefab;
    public Transform[] startPoints;
    public Transform[] endPoints;
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Set Disruption Frequency
        disruptionFrequency = hourDuration * GameController.instance.curLevel.disruptionFrequency;
        visitorFrequency = hourDuration * visitorFrequency;

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

            //Check for a new visitor
            if (time >= visitorFrequency)
            {
                SendVisitor(false);       //Send in a visitor

                visitorFrequency += visitorFrequency;
            }

            //Check for a new disrutption
            if (time >= disruptionFrequency)
            {
                SendVisitor(true);       //Send in a visitor to make a disruption

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


    private void SendVisitor(bool disruption)
    {
        Visitor focusVisitor = Instantiate(visitorPrefab, startPoints[index].position, Quaternion.identity).GetComponent<Visitor>();

        focusVisitor.destination = endPoints[index];

        focusVisitor.disruptor = disruption;

        focusVisitor.GoTowardsPoint(endPoints[index].position);

        index = (index + 1) % startPoints.Length;
    }

    private void MakeMonkeyPoop()
    {
        if(monkeys.Length == 0)
            monkeys = FindObjectsOfType<Monkey>();

        //Choose a random primate
        Monkey pooper = monkeys[Random.Range(0, monkeys.Length)];

        //Make them poop
        pooper.Poop();
    }
}
