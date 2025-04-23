using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Fuel")]
    [SerializeField] Slider fuelSlider;
    [SerializeField] float maxFuel = 5000f;
    [SerializeField] float fuelLevel = 0f;
    [SerializeField] float addFuelAmount = 75.0f;
    [SerializeField] float comsumptionRateFuel = 50.0f;

    [Header("Coolent")]
    [SerializeField] Slider coolentSlider;
    [SerializeField] float maxCoolent = 10000f;
    [SerializeField] float coolentLevel = 0;
    [SerializeField] float addCoolentAmount = 75.0f;
    [SerializeField] float comsumptionRateCoolent = 100.0f;

    [Header("Power")]
    [SerializeField] Slider powerSlider;
    [SerializeField] TMP_Text powerText;
    [SerializeField] float quota = 100;
    [SerializeField] float genPower;
    [SerializeField] float genPowerRate = .5f;

    [Header("Temperature")]
    [SerializeField] float temperatrue = 296.0f;
    [SerializeField] float temperatrueModifier = 0.00337837837f;
    [SerializeField] float coolThreshold = 9000.0f;
    [SerializeField] float heatThreshold = 200.0f;

    // Start is called before the first frame update
    void Start()
    {
        fuelSlider.maxValue = maxFuel;
        coolentSlider.maxValue = maxCoolent;
        powerSlider.maxValue = quota;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        updateSliders();
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
        fuelLevel -= comsumptionRateFuel * Time.deltaTime;
        coolentLevel -= comsumptionRateCoolent * Time.deltaTime;
        genPower += (temperatrue * temperatrueModifier) * genPowerRate;
    }

    void updateSliders()
    {
        fuelSlider.value = fuelLevel;
        coolentSlider.value = coolentLevel;
        powerSlider.value = genPower;
        string powerString = "Power "+genPower+"/"+quota;
        powerText.text = powerString;
    }

    public void addFuel()
    {
        fuelLevel+=addFuelAmount;
    }

    public void addCoolent()
    {
        coolentLevel+=addCoolentAmount;
    }
}
