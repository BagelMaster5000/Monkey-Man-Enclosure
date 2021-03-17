using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Man : Primate
{
    [Header("Man")]
    //UI
    [SerializeField] private Image hungerIcon;
    [SerializeField] private Image awakeBar;

    //Man
    [SerializeField] private float maxHungerLevel = 100;
    [SerializeField] private float hungerNotificationLevel = 30;
    private float hungerLevel;

    [SerializeField] private float maxAwakeLevel = 100;
    private float awakeLevel = 0;
    private float previousAwakeLevel = 0;
    [SerializeField, Range(1, 10), Tooltip("How much time needs to pass after losing sleep before the man regains sleep")]
    private float regainSleepCooldown;
    private float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        hungerLevel = maxHungerLevel * 0.8f;
        hungerIcon.gameObject.SetActive(false);

        awakeBar.fillAmount = awakeLevel / maxAwakeLevel;
    }

    private void Update()
    {
        if (previousAwakeLevel == awakeLevel)
        {
            time += Time.deltaTime;
            if (time > regainSleepCooldown)
                ModifySleep(-Time.deltaTime);
        }
        else
            time = 0;

        previousAwakeLevel = awakeLevel;
    }

    public void ModifyHunger(float amount)
    {
        //Modify hunger. Can either add or subtract
        //If hunger is less than the notification level, make the icon appear

        hungerLevel = Mathf.Min(hungerLevel + amount, maxHungerLevel);

        hungerIcon.gameObject.SetActive(hungerLevel > hungerNotificationLevel);

        //If the Man starves
        if(hungerLevel <= 0)
        {
            //Gameover
            GameController.instance.Lose();
        }
    }

    public void ModifySleep(float amount)
    {
        //Modify how awake the man is. Add or subtract.
        //Update the man's awake UI bar

        awakeLevel = Mathf.Max(0, awakeLevel + amount);

        awakeBar.fillAmount = awakeLevel / maxAwakeLevel;

        //If the Man awakens
        if (awakeLevel >= maxAwakeLevel)
        {
            //Gameover
            GameController.instance.Lose();
        }
    }
}
