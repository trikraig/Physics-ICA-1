using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newton : MonoBehaviour
{

    Vector3 initialPos;
    /*public*/ Vector3 initialVelocity = new Vector3(10, 10, 0);
    Vector3 gravity = new Vector3(0, -9.8f, 0);

    float currentTime;
    int currentFrame = 0;
    int FPS = 60;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        currentTime = Time.time;
    }

    private void FixedUpdate()
    {
        float deltaTime = GetDeltaTime();

        //SimpleDisplacement(deltaTime);

        DisplacementConstantAcceleration(deltaTime);

    }
    
    void SimpleDisplacement(float dt)
    {
        // x(t) = x(0) + v(0) * time
        transform.position = initialPos + initialVelocity * dt;
    }

    void DisplacementConstantAcceleration(float dt)
    {
        //Displace position with constant acceleration
        //Vector3 acceleration = constantAcceleration + gravity;
        transform.position = initialPos + initialVelocity * dt + ((gravity * (dt * dt)) / 2);
    }

    float GetDeltaTime()
    {
        currentFrame++;
        return (float)currentFrame / (float)FPS;
    }

}
