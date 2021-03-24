using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Man : Primate
{
    [Header("Man")]
    //UI
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject hungerIcon;
    [SerializeField] private Image awakeBar;
    private Vector3 targetPostition;

    //Man
    [SerializeField] private float maxHungerLevel = 100;
    [SerializeField] private float portionOfHungerToFillWhenEating = 0.2f;
    [SerializeField] private float hungerRestorationAmount = 10;
    [SerializeField] private float hungerNotificationLevel = 30;
    [SerializeField] private float extremeHungerNotificationLevel = 15;

    public float maxAwakeLevel = 100; 
    private float previousAwakeLevel = 0;
    [SerializeField, Range(1, 10), Tooltip("How much time needs to pass after losing sleep before the man regains sleep")]
    private float regainSleepCooldown;
    private bool regainingSleep = false;
    private float time = 0;


    // Cause of death
    private int causeOfDeath = 0; // 0 awoke, 1 starved

    [Header("Dev Only")]
    public float hungerLevel = 0;
    public float awakeLevel = 0;


    // Start is called before the first frame update
    protected override void Start()
    {
        hungerLevel = maxHungerLevel * 0.8f;
        hungerIcon.gameObject.SetActive(false);

        awakeBar.fillAmount = 1 - awakeLevel / maxAwakeLevel;

        StartCoroutine(RefreshingAwakeBar());
        StartCoroutine(CheckIfEating());
        StartCoroutine(AnimatingHungerIcon());

        base.Start();
    }

    protected override void Update()
    {
        //Keep the man's UI pointed at the camera
        targetPostition = new Vector3(canvas.transform.position.x,
                                        Camera.main.transform.position.y,
                                        Camera.main.transform.position.z);
        canvas.transform.LookAt(targetPostition); //Make the MonkeyMan's UI canvas look at the camera


        //ReSleep timer
        if (previousAwakeLevel == awakeLevel || regainingSleep == true)
        {
            time += Time.deltaTime;
            if (time > regainSleepCooldown)
            {
                regainingSleep = true;
                ModifySleep(-5*Time.deltaTime);
            }
        }
        else
            time = 0;

        previousAwakeLevel = awakeLevel;

        base.Update();
    }

    public void ModifyHunger(float amount)
    {
        //Modify hunger. Can either add or subtract
        //If hunger is less than the notification level, make the icon appear

        hungerLevel = Mathf.Min(hungerLevel + amount, maxHungerLevel);

        //If the Man starves
        if(hungerLevel <= 0)
        {
            //Gameover
            causeOfDeath = 1;
            GameController.instance.Lose();
        }
    }

    public void ModifySleep(float amount)
    {
        //Modify how awake the man is. Add or subtract.
        //Update the man's awake UI bar

        awakeLevel = Mathf.Max(0, awakeLevel + amount);

        if (amount > 0)
            regainingSleep = false;

        //If the Man awakens
        if (awakeLevel >= maxAwakeLevel)
        {
            //Gameover
            causeOfDeath = 0;
            GameController.instance.Lose();
        }
    }

    IEnumerator RefreshingAwakeBar()
    {
        while (true)
        {
            awakeBar.fillAmount = 1 - awakeLevel / maxAwakeLevel;

            yield return new WaitForSeconds(GlobalVariables.UIRefreshInterval);
        }
    }

    IEnumerator CheckIfEating()
    {
        bool isEating = false;
        while (true)
        {
            if (state != PrimateState.Eating)
                isEating = false;
            else if (!isEating && state == PrimateState.Eating)
            {
                isEating = true;
                ModifyHunger((maxHungerLevel - hungerLevel) * portionOfHungerToFillWhenEating + hungerRestorationAmount);
            }
            yield return null;
        }
    }

    IEnumerator AnimatingHungerIcon()
    {
        bool hungerIconVisibility = true;
        while (true)
        {
            if (hungerLevel <= extremeHungerNotificationLevel)
            {
                hungerIcon.SetActive(hungerIconVisibility);
                hungerIconVisibility = !hungerIconVisibility;
                yield return new WaitForSeconds(0.3f);
            }
            else if (hungerLevel <= hungerNotificationLevel)
            {
                hungerIconVisibility = true;
                hungerIcon.SetActive(hungerIconVisibility);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                hungerIcon.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public int GetCauseOfDeath()
    {
        return causeOfDeath;
    }

    public override void PlaySound(SoundPlayer[] soundArray)
    {
        
    }
}
