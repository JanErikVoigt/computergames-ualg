using UnityEngine;
using UnityEngine.InputSystem;

public class ControlTruck : MonoBehaviour
{


    public float steerIntensity = 10.0f;

    [SerializeField] private Transform truckFront;
    public float slowSpeed = 4.0f;
    public float defaultSpeed = 6.0f;
    public float fastSpeed = 10.0f;
    public float lateralSpeed = 0.5f;

    private float progress;
    private float lateralOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.progress=0f;
        this.lateralOffset=0f;
        this.updatePos();
    }

    void updatePos()
    {
        transform.localPosition = new Vector3(-this.lateralOffset, 0f, -this.progress);
    }

    // Update is called once per frame
void Update()
{
    var keyboard = Keyboard.current;
    if (keyboard == null) return;



    // Forward speed control
    float speed = this.defaultSpeed;

    if (keyboard.wKey.isPressed)
    {
        speed = this.fastSpeed;
    }

    if (keyboard.sKey.isPressed)
    {
        speed = this.slowSpeed;
    }

    // Sideways movement
    float lateralVelocity = 0f;
    if (keyboard.aKey.isPressed)
    {
        lateralVelocity -= this.lateralSpeed * speed / this.defaultSpeed;
    }

    if (keyboard.dKey.isPressed)
    {
        lateralVelocity += this.lateralSpeed * speed / this.defaultSpeed;
    }

    this.lateralOffset += Time.deltaTime * lateralVelocity;

    //rotate truck front based on lateral speed
    truckFront.localRotation = Quaternion.Euler(0f, 90f + lateralVelocity * steerIntensity, 0f);

    this.progress += Time.deltaTime * speed;
    this.updatePos();
}

}
