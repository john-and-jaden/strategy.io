using UnityEngine;

public class Dampable
{
    public float currentVelocity;
    float lastVelocity;
    float dampTime;
    float timer;

    public Dampable(float time)
    {
        this.dampTime = time;
    }

    public void UpdateAndDampen(float movingConditional, float currentVelocity)
    {
        if (movingConditional != 0)
        {
            // Update momentum
            this.currentVelocity = currentVelocity;
            lastVelocity = currentVelocity;
            timer = 0;
        }
        else
        {
            // Update timer
            timer += Time.deltaTime;

            // Dampen 
            this.currentVelocity = Mathf.Lerp(lastVelocity, 0, timer / dampTime);
        }
    }
}