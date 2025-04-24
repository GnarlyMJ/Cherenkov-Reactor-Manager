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
    }

    float calcQuota()
    {
        quota = quota + ((dayNum * 5)+100);
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
        if (fuelLevel > 0){updatePower();}
        if (fuelLevel < 0){fuelLevel = 0;}
        if (coolentLevel < 0){coolentLevel = 0;}
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
        string powerString = "Power "+genPower+"/"+quota;
        powerText.text = powerString;
    }

    void timer()
    {
        curTime += Time.deltaTime;
        if(curTime >= dayLength * 60){dayEnd();}
    }    

    void dayEnd()
    {
        if(genPower >= quota)
        {
            genPower = 0;
            quota = calcQuota();
        }
        else{gameOver();}
    }

    void gameOver()
    {}

    public void addFuel()
    {
        fuelLevel+=addFuelAmount;
    }

    public void addCoolent()
    {
        coolentLevel+=addCoolentAmount;
    }
}