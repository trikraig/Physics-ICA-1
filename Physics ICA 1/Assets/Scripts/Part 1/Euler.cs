using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Euler : MonoBehaviour
{
    Vector3 Gravity = new Vector3(0, -0.05f, 0);

    public int mass = 1;
    public Vector3 force = new Vector3(0.75f, 0.75f, 0);    
    private Vector3 velocity;   


    private void Start()
    {
        velocity = new Vector3(force.x * mass, force.y * mass, force.z * mass);
    }

    private void FixedUpdate()
    {
        velocity += Gravity;
        transform.position += velocity;
    }
}
