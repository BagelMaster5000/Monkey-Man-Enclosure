using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    #region Singleton for scene only
    public static PlayerInventory instance = null;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] private int money = 0;
    [SerializeField] private int hourlyWage;
    [SerializeField] private Supplies supplies;
    [SerializeField] private int startFoodPelletsAmt = 2;
    [SerializeField] private int startBrickAmt = 0;
    [SerializeField] private int startBananaAmt = 0;

    [Header("SFX")]
    [SerializeField] SoundPlayer receivedWage;

    // Start is called before the first frame update
    void Start()
    {
        supplies.foodPelletAmt = startFoodPelletsAmt;
        supplies.brickAmt = startBrickAmt;
        supplies.bananaAmt = startBananaAmt;
    }

    public void RecieveWage()
    {
        money += hourlyWage;

        receivedWage.Play();
    }

    public int GetMoney() { return money; }
    public void SubtractMoney(int subAmt) { money -= subAmt; }
    public Supplies GetSupplies() { return supplies; }
    public void AddFoodPellet() { ++supplies.foodPelletAmt; }
    public void SubFoodPellet() { --supplies.foodPelletAmt; }
    public void AddBrick() { ++supplies.brickAmt; }
    public void SubBrick() { --supplies.brickAmt; }
    public void AddBanana() { ++supplies.bananaAmt; }
    public void SubBanana() { --supplies.bananaAmt; }
}

public struct Supplies
{
    public int foodPelletAmt;
    public int brickAmt;
    public int bananaAmt;
}
