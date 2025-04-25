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
    [SerializeField] float fuelLevel = 0f;
    [SerializeField] float addFuelAmount = 75.0f;
    [SerializeField] float comsumptionRateFuelMax = 100.0f;
    [SerializeField] float comsumptionRateFuelMin = 25.0f;

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
    [SerializeField] float temperatrue = 296.0f;
    [SerializeField] float temperatrueModifier = 0.00337837837f;
    [SerializeField] float coolThreshold = 9000.0f;
    [SerializeField] float heatThreshold = 200.0f;

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

    // Start is called before the first frame update
    void Start()
    {
        fuelSlider.maxValue = maxFuel;
        coolentSlider.maxValue = maxCoolent;
        powerSlider.maxValue = calcQuota();

        fuelComsuptionSlider.maxValue = comsumptionRateFuelMax;
        fuelComsuptionSlider.minValue = comsumptionRateFuelMin;

        clockText.text = startTime + ":00" + "am";
        clockSlider.maxValue = dayLength*60;
    }

    float calcQuota()
    {
        quota = quota + ((dayNum * 5) + 100);
        return quota;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        updateSliders();
        timer();
        if (fuelLevel > 0) { updatePower(); }
        if (fuelLevel < 0) { fuelLevel = 0; }
        if (coolentLevel < 0) { coolentLevel = 0; }
        if (coolentLevel > coolThreshold)
        {
            temperatrue -= temperatrue * temperatrueModifier;
        }
        if (coolentLevel < heatThreshold)
        {
            temperatrue += temperatrue * temperatrueModifier;
        }
    }

    void updatePower()
    {
        fuelLevel -= fuelComsuptionSlider.value * Time.deltaTime;
        coolentLevel -= comsumptionRateCoolent * Time.deltaTime;
        genPower += (((temperatrue * temperatrueModifier) * fuelComsuptionSlider.value) * genPowerRate) * Time.deltaTime;
    }

    void updateSliders()
    {
        fuelSlider.value = fuelLevel;
        coolentSlider.value = coolentLevel;
        powerSlider.value = genPower;
        string powerString = "Power " + genPower + "/" + quota;
        powerText.text = powerString;
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
        float hours  = (curTime/60) + startTime;
        if (hours >= 13)
        {
            hours -= 12;
        }
        string timeString = "";
        int minutes = (int)((hours - (int)hours) * 60);
        if(minutes >= 10){timeString = (int)hours+":"+minutes+" am";}
        else{timeString = (int)hours + ":0" + minutes + " am";}
        clockText.text = timeString;
    }

    void dayEnd()
    {
        if (genPower >= quota)
        {
            genPower = 0;
            curTime = 0;
            powerSlider.maxValue = calcQuota();
        }
        else { gameOver(); }
    }

    void gameOver()
    {
        gameOverScreen.SetActive(true);
        dayText.text = "Days: " + dayNum;
        if (dayNum > highScore)
        {
            highscoreText.SetActive(true);
            highScore = dayNum;
        }
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