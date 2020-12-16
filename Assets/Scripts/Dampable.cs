using UnityEngine;

public class Dampable
{
    public float currentVelocity;
    float lastVelocity;
    float time;
    float timer;

    public Dampable(float time)
    {
        this.time = time;
    }

    public void UpdateAndDampen(float conditional, float currentVelocity)
    {
        if (conditional != 0)
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
            this.currentVelocity = Mathf.Lerp(lastVelocity, 0, timer / time);
        }
    }
}