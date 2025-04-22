using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] float maxFuel = 5000f;
    [SerializeField] float maxCoolent = 10000f;
    [SerializeField] float fuelLevel = 0f;
    [SerializeField] float coolentLevel = 0;
    float comsumptionRateFuel = 50.0f;
    float comsumptionRateCoolent = 100.0f;

    float addFuelAmount = 75.0f;
    [Header("References")]
    [SerializeField] Slider fuelSlider;

    // Start is called before the first frame update
    void Start()
    {
        fuelSlider.maxValue = maxFuel;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (fuelLevel > 0)
        {
            fuelLevel -= comsumptionRateFuel;
            coolentLevel -= comsumptionRateCoolent;

            fuelSlider.value = fuelLevel;
        }
    }

    public void addFuel()
    {
        fuelLevel+=addFuelAmount;
        fuelSlider.value = fuelLevel;
    }
}
