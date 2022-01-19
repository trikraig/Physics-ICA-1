using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBall : MonoBehaviour
{
    Vector3 Gravity = new Vector3(0, -0.05f, 0);
    public Vector3 m_velocity;
    public float m_radius = 5.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        //m_velocity += Gravity;
        transform.position += m_velocity;
    }

   public Vector3 GetVelocity()
    {
        return m_velocity;
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        m_velocity = newVelocity;
    }
}
