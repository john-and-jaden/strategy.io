using UnityEngine;

public class Dampable
{
    private float speed;
    private float minSpeed;
    private float maxSpeed;
    public float Speed { get { return speed; } }
    private float lastSpeed;
    private float dampTime;
    private float timer;
    public Dampable(float dampTime) : this(dampTime, float.MinValue, float.MaxValue) { }
    public Dampable(float dampTime, float minSpeed, float maxSpeed)
    {
        this.dampTime = dampTime;
        this.minSpeed = minSpeed;
        this.maxSpeed = maxSpeed;
    }
    public void UpdateSpeed(float acceleration)
    {
        bool isAccelerating = Mathf.Abs(acceleration) > Mathf.Epsilon;
        bool directionHasChanged = Mathf.Sign(acceleration) != Mathf.Sign(speed);
        if (isAccelerating)
        {
            // Update speed and reset timer
            if (directionHasChanged)
            {
                speed = 0;
            }
            lastSpeed = speed = Mathf.Clamp(speed + acceleration, minSpeed, maxSpeed);
            timer = 0;
        }
        else
        {
            // Dampen speed
            speed = Mathf.Lerp(lastSpeed, 0, timer / dampTime);
            timer += Time.deltaTime;
        }
    }
}