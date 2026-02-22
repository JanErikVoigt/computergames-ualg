using UnityEngine;
using UnityEngine.UI;

public class Fuel : MonoBehaviour
{
    public float currentFuel = 100f;
    public Text fuelDisplay;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fuelDisplay != null)
        {
            fuelDisplay.text = "Fuel: " + Mathf.Max(0, (int)currentFuel).ToString();
        }
    }
}
