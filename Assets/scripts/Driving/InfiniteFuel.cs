using UnityEngine;

public class InfiniteFuel : MonoBehaviour, IFuel
{
    public float CurrentFuel => 999f;

    public void Consume(float amount)
    {
        // Do nothing, infinite fuel
    }

    public bool HasFuel()
    {
        return true;
    }
}
