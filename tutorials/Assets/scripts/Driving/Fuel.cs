using UnityEngine;

public class Fuel : MonoBehaviour, IFuel
{
    [SerializeField] private float fuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 1f;

    public float CurrentFuel => fuel;

    public void SetFuel(float amount)
    {
        fuel = amount;
    }

    public void Consume(float baseAmount)
    {
        float fuelConsumed = baseAmount * fuelConsumptionRate * Time.deltaTime;
        fuel -= fuelConsumed;
        if (fuel < 0) fuel = 0;
    }

    public bool HasFuel()
    {
        return fuel > 0;
    }
}
