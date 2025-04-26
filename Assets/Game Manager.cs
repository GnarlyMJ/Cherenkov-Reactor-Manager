using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Fuel")]
    [SerializeField] Slider fuelSlider;
    [SerializeField] Slider fuelComsuptionSlider;
    [SerializeField] float maxFuel = 5000f;
    [SerializeField] float maxFuelBase = 5000f;
    [SerializeField] float fuelLevel = 0f;
    [SerializeField] float addFuelAmount = 75.0f;
    [SerializeField] float comsumptionRateFuelMax = 100.0f;
    [SerializeField] float comsumptionRateFuelMaxBase = 100.0f;
    [SerializeField] float comsumptionRateFuelMin = 25.0f;
    [SerializeField] float comsumptionRateFuelMinBase = 25.0f;
    [SerializeField] float meltdownThreshold = 5000f * .75f;

    [Header("Coolent")]
    [SerializeField] Slider coolentSlider;
    [SerializeField] float maxCoolent = 10000f;
    [SerializeField] float coolentLevel = 0;
    [SerializeField] float addCoolentAmount = 75.0f;
    [SerializeField] float comsumptionRateCoolent = 100.0f;

    [Header("Power")]
    [SerializeField] Slider powerSlider;
    [SerializeField] TMP_Text powerText;
    [SerializeField] float genPower;
    [SerializeField] float genPowerRate = .5f;

    [Header("Temperature")]
    [SerializeField] Slider thermometer;
    [SerializeField] float temperatrue = 1073.15f;
    [SerializeField] float temperatrueModifier = 0.00337837837f;
    [SerializeField] float coolThreshold = 9000.0f;
    [SerializeField] float heatThreshold = 200.0f;
    [SerializeField] float freezeThreshold = 366.15f;
    [SerializeField] float meltdownBegins = 2973.15f;
    [SerializeField] float meltdownTimer = 0;
    [SerializeField] bool melting = false;

    [Header("Day Manager")]
    [SerializeField] float quota = 0;
    [SerializeField] int dayNum = 0;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject highscoreText;
    [SerializeField] TMP_Text dayText;
    [SerializeField] int highScore = 0;

    [Header("Clock")]
    [SerializeField] Slider clockSlider;
    [SerializeField] TMP_Text clockText;
    [SerializeField] int startTime = 9;
    [SerializeField] float dayLength = 24; //Minutes
    [SerializeField] float curTime = 0;
    [SerializeField] string timeSection = " am";

    [Header("Store")]
    [SerializeField] GameObject StoreMenu;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] bool storeOpen = false;
    [SerializeField] float money = 0;
    [SerializeField] int FuelEficiencyUpgrades = 0;
    [SerializeField] int minFuelConsumpitionUpgrades = 0;
    [SerializeField] int maxFuelConsumpitionUpgrades = 0;
    [SerializeField] int maxFuelAmountUpgrades = 0;
    [SerializeField] int coolantEficiencyUpgrades = 0;
    [SerializeField] int shiftLengthUpgrades = 0;
    // Start is called before the first frame update
    void Start()
    {
        fuelSlider.maxValue = maxFuel;
        coolentSlider.maxValue = maxCoolent;
        powerSlider.maxValue = calcQuota();

        fuelComsuptionSlider.maxValue = comsumptionRateFuelMax;
        fuelComsuptionSlider.minValue = comsumptionRateFuelMin;

        thermometer.maxValue = 3073.15f;

        clockText.text = startTime + ":00" + "am";
        clockSlider.maxValue = dayLength * 60;
    }

    float calcQuota()
    {
        quota = (dayNum * 1000 * ((dayLength + shiftLengthUpgrades) * 60) * .25f) + 1000;
        return quota;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (!storeOpen)
        {
            updateSliders();
            timer();
            if (fuelLevel > 0) { updatePower(); }
            if(fuelLevel > meltdownThreshold){startMeltdown();}
            coolentLevel -= (comsumptionRateCoolent * (1 - coolantEficiencyUpgrades * .75f)) * Time.deltaTime;
            if (fuelLevel < 0) { fuelLevel = 0; }
            if (coolentLevel < 0) { coolentLevel = 0; startMeltdown();}
            if (coolentLevel > coolThreshold)
            {
                temperatrue -= temperatrue * temperatrueModifier;
            }

            if (temperatrue >= meltdownBegins || melting)
            {
                startMeltdown();
            }

            if(temperatrue <= freezeThreshold)
            {
                startMeltdown();
            }
        }
    }

    void updatePower()
    {
        fuelLevel -= fuelComsuptionSlider.value * Time.deltaTime;
        temperatrue += (temperatrue * temperatrueModifier) * (fuelComsuptionSlider.value * .05f);
        if (FuelEficiencyUpgrades > 1)
        {
            genPower += ((temperatrue * temperatrueModifier) * fuelComsuptionSlider.value * (1 + Mathf.Pow(.5f, FuelEficiencyUpgrades)) * genPowerRate) * Time.deltaTime;
        }
        else
        {
            genPower += ((temperatrue * temperatrueModifier) * fuelComsuptionSlider.value * genPowerRate) * Time.deltaTime;
        }
    }

    void updateSliders()
    {
        fuelSlider.value = fuelLevel;
        coolentSlider.value = coolentLevel;
        powerSlider.value = genPower;
        string powerString = "Power " + genPower + "/" + quota;
        powerText.text = powerString;
        thermometer.value = temperatrue;
    }

    void timer()
    {
        curTime += Time.deltaTime;
        updateClock();
        if (curTime >= dayLength * 60) { dayEnd(); }
    }

    void updateClock()
    {
        clockSlider.value = curTime;
        float hours = (curTime / 60) + startTime;
        if (hours >= 13)
        {
            hours -= 12;
            if (timeSection.Equals(" am"))
            {
                timeSection = " pm";
            }
            else
            {
                timeSection = " am";
            }
        }
        string timeString = "";
        int minutes = (int)((hours - (int)hours) * 60);
        if (minutes >= 10) { timeString = (int)hours + ":" + minutes + timeSection; }
        else { timeString = (int)hours + ":0" + minutes + timeSection; }
        clockText.text = timeString;
    }

    void dayEnd()
    {
        if (genPower >= quota)
        {
            genPower = genPower - quota;
            curTime = 0;
            dayNum ++;
            money = calcMoney();
            powerSlider.maxValue = calcQuota();
            melting = false;
            meltdownTimer = 0;
            openStore();
        }
        else { gameOver(); }
    }

    float calcMoney()
    {
        float genMoney = quota * (.5f * dayNum);
        moneyText.text = "Money: $"+genMoney;
        return money + genMoney;
    }

    public void resetGame()
    {
        dayNum = 0;
        quota = 1000;

        fuelSlider.maxValue = maxFuelBase;
        coolentSlider.maxValue = maxCoolent;
        powerSlider.maxValue =quota;

        fuelComsuptionSlider.maxValue = comsumptionRateFuelMaxBase;
        fuelComsuptionSlider.minValue = comsumptionRateFuelMinBase;

        fuelSlider.value = comsumptionRateFuelMinBase;

        FuelEficiencyUpgrades = 0;
        maxFuelAmountUpgrades = 0;
        coolantEficiencyUpgrades = 0;
        maxFuelConsumpitionUpgrades = 0;
        minFuelConsumpitionUpgrades = 0;
        shiftLengthUpgrades = 0;

        money = 0;

        thermometer.maxValue = 3073.15f;
        temperatrue = 1073.15f;
        melting = false;
        meltdownTimer = 0;

        clockText.text = startTime + ":00" + "am";
        clockSlider.maxValue = dayLength * 60;

        curTime = 0;

        genPower = 0;

        coolentLevel = 9000;

        gameOverScreen.SetActive(false);
        highscoreText.SetActive(false);
        storeOpen = false;
    }

    void gameOver()
    {
        storeOpen = true;
        gameOverScreen.SetActive(true);
        dayText.text = "Days: " + dayNum;
        if (dayNum > highScore)
        {
            highscoreText.SetActive(true);
            highScore = dayNum;
        }
    }

    void startMeltdown()
    {
        melting = true;
        meltdownTimer += Time.deltaTime;
        if (meltdownTimer >= 5)
        {
            gameOver();
        }
    }

    void openStore()
    {
        StoreMenu.SetActive(true);
        storeOpen = true;
    }

    public void closeStore()
    {
        StoreMenu.SetActive(false);
        storeOpen = false;

        maxFuel = maxFuelBase + (maxFuelBase * (maxFuelAmountUpgrades * .75f));
        comsumptionRateFuelMax = comsumptionRateFuelMaxBase + (comsumptionRateFuelMaxBase * (maxFuelConsumpitionUpgrades * .9f));
        comsumptionRateFuelMin = comsumptionRateFuelMinBase - (comsumptionRateFuelMinBase * (minFuelConsumpitionUpgrades * .9f));

        fuelSlider.maxValue = maxFuel;
        coolentSlider.maxValue = maxCoolent;

        fuelComsuptionSlider.maxValue = comsumptionRateFuelMax;
        fuelComsuptionSlider.minValue = comsumptionRateFuelMin;

        meltdownThreshold = maxFuel * .75f;

        temperatrue = 1073.15f;
    }

    public void purchaseUpgrade(string upgrade)
    {
        float price = getPrice(upgrade);
        if (money >= price)
        {
            switch (upgrade)
            {
                case "fuelEfficiency":
                    FuelEficiencyUpgrades++;
                    break;
                case "fuelConsumptionMax":
                    maxFuelConsumpitionUpgrades++;
                    break;
                case "fuelConsumptionMin":
                    minFuelConsumpitionUpgrades++;
                    break;
                case "fuelAmountMax":
                    maxFuelAmountUpgrades++;
                    break;
                case "coolantEficiency":
                    coolantEficiencyUpgrades++;
                    break;
                case "shiftLength":
                    shiftLengthUpgrades++;
                    break;
            }
            money -= price;
            moneyText.text = "Money: $" + money;
        }
    }

    public void sellUpgrade(string upgrade)
    {
        float price = getPrice(upgrade);
        switch (upgrade)
        {
            case "fuelEfficiency":
                if (FuelEficiencyUpgrades > 0)
                {
                    FuelEficiencyUpgrades--;
                    money += price * .75f;
                }
                break;
            case "fuelConsumptionMax":
                if (maxFuelConsumpitionUpgrades > 0)
                {
                    maxFuelConsumpitionUpgrades--;
                    money += price * .75f;
                }
                break;
            case "fuelConsumptionMin":
                if (minFuelConsumpitionUpgrades > 0)
                {
                    minFuelConsumpitionUpgrades--;
                    money += price * .75f;
                }
                break;
            case "fuelAmountMax":
                if (maxFuelAmountUpgrades > 0)
                {
                    maxFuelAmountUpgrades--;
                    money += price * .75f;
                }
                break;
            case "coolantEficiency":
                if (coolantEficiencyUpgrades > 0)
                {
                    coolantEficiencyUpgrades--;
                    money += price * .75f;
                }
                break;
            case "shiftLength":
                if (shiftLengthUpgrades > 0)
                {
                    shiftLengthUpgrades--;
                    money += price * .75f;
                }
                break;
        }
        moneyText.text = "Money: $" + money;
    }

    float getPrice(string upgrade)
    {
        float price = 0;
        switch (upgrade)
        {
            case "fuelEfficiency":
                price = 100.00f;
                break;
            case "fuelConsumptionMax":
                price = 150.00f;
                break;
            case "fuelConsumptionMin":
                price = 125.50f;
                break;
            case "fuelAmountMax":
                price = 200.00f;
                break;
            case "coolantEficiency":
                price = 50.00f;
                break;
            case "shiftLength":
                price = 1000.00f;
                break;
        }
        return price;
    }

    public void addFuel()
    {
        fuelLevel += addFuelAmount;
    }

    public void addCoolent()
    {
        coolentLevel += addCoolentAmount;
    }
}