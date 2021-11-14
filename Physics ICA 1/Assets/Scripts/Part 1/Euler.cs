using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Euler : MonoBehaviour
{
    Vector3 initialPos;
    Vector3 velocity = new Vector3(0.75f, 0.75f, 0);
    Vector3 gravity = new Vector3(0, -0.05f, 0);

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.position + velocity;
        velocity += gravity;
        transform.position = pos;
    }
}
