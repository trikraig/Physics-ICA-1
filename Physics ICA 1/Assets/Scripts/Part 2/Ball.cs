using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    // Members
    public float m_radius = 0.5f;
    Vector3 m_initialPos;
    public Vector3 m_velocity = new Vector3(0.75f, 0.75f, 0);
    Vector3 m_gravity = new Vector3(0, -0.00f, 0);
    public int m_mass = 5;

    //
    //Other Scene Stuff
    public Ball otherBall;

    public GameObject planeA;
    public GameObject planeB;
    public GameObject planeC;

    // Start is called before the first frame update
    void Start()
    {
        m_initialPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Update position
        Vector3 pos = transform.position + m_velocity;
        m_velocity += m_gravity;
        transform.position = pos;

        //Check for collisions
        if (m_velocity != Vector3.zero)
        {
            //SphereToSphereCollision();
            MotionSphereToSphereCollision();

            SphereToPlaneCollision();
        }   
    }
    void SphereToPlaneCollision()
    {
        //Very rough collision check, to make better
        bool collided = transform.position.x > planeA.transform.position.x;

        if (collided)
        {

            Vector3 Va = m_velocity;

            Vector3 A = planeA.transform.position;
            Vector3 B = planeB.transform.position;
            Vector3 C = planeC.transform.position;

            Vector3 V1 = B - A;
            Vector3 V2 = C - A;

            //Normal is cross product of these two vectors.
            Vector3 N = CrossProduct(V1, V2);

            Vector3 VbNoramlised = 2 * N * (DotProduct(N, -Va.normalized)) + Va.normalized;

            Vector3 Vb = VbNoramlised * GetMagnitude(Va);

            m_velocity = Vb;
        }
    }

    void MotionSphereToSphereCollision()
    {
        //Need to find at what point in frame that they collide
        // e.g. when distance between center of two spheres = sum of their radii (r1 + r2)

        Vector3 P1 = transform.position; // start position vector of sphere 1.
        Vector3 P2 = otherBall.transform.position; // start position vector of sphere 2
        Vector3 V1 = m_velocity; //velocity vector of sphere 1
        Vector3 V2 = otherBall.m_velocity; // velocity vector of sphere 2
        float r1 = m_radius;
        float r2 = otherBall.m_radius;

        float dXp = P1.x - P2.x;
        float dYp = P1.y - P2.y;
        float dZp = P1.z - P2.z;
        float dXv = V1.x - V2.x;
        float dYv = V1.y - V2.y;
        float dZv = V1.z - V2.z;

        var A = (dXv * dXv) + (dYv * dYv) + (dZv * dZv);
        var B = (2 * dXp * dXv) + (2 * dYp * dYv) + (2 * dZp * dZv);
        var C = (dXp * dXp) + (dYp * dYp) + (dZp * dZp) - ((r1 + r2) * (r1 + r2));

        bool collision = B * B > 4 * (A * C);
        if (collision)
        {
            var sqrt = Mathf.Sqrt(B * B - 4 * (A * C));
            var t1 = -B + sqrt / 2 * A;
            var t2 = -B - sqrt / 2 * A;

            //The smallest of the two t values is the point along the vector that the spheres collide.
            float minT = Mathf.Min(t1, t2);
            Vector3 P1CollisionPoint = P1 + (V1 * minT);
            Vector3 P2CollisionPoint = P2 + (V2 * minT);
            float magnitude = GetMagnitude(P1CollisionPoint - P2CollisionPoint);
            float radiiSum = r1 + r2;

            //Collision occurs when the resulting magnitude is equal to the sum of radii. In this case using <= since close enough 
            if (magnitude <= radiiSum)
            {
                //Apply forces since collided
                //m_velocity = new Vector3(0, 0, 0);

                //Need to calculate G - the resulting vector after the two spheres collided

                //G = Vector from center of other sphere from point of impact s
                //Vector3 G = P2 - P2CollisionPoint;
                //otherBall.m_velocity = G;

                Vector3 G = (P2CollisionPoint - P1CollisionPoint).normalized;
                /*
                                newVelocity.x *= 0.01f;
                                newVelocity.y *= 0.01f;
                                newVelocity.z *= 0.01f;
                */

                //#TODO - cos(q) * F / mass^2
                float newForce = Mathf.Cos(GetAngle(V1, G));
                
                //cba to write operator
                G.x *= newForce;
                G.y *= newForce;
                G.z *= newForce;

                //Apply new resulting velocity
                otherBall.m_velocity = G;
            }
        }
    }


    //Moving sphere to stationary sphere - for reference.
    void SphereToSphereCollision()
    {
        Vector3 a = otherBall.transform.position - transform.position; //vector from center of sphere 1 to center of sphere 2
        float AMag = GetMagnitude(a);

        Vector3 V = m_velocity; //Vector of motion of sphere 1 (this)
        float angle = GetAngle(transform.position, otherBall.transform.position);

        float radiiSum = m_radius + otherBall.m_radius;

        // d = sin(q) * |A|
        float d = Mathf.Sin(angle) * AMag;

        if (d < radiiSum)
        {
            float e = Mathf.Sqrt((radiiSum * radiiSum) - (d * d));

            float Vc = Mathf.Cos(angle) * GetMagnitude(a) - e;

            if (d < Vc)
            {
                Debug.Log("Collision");
                otherBall.m_velocity += m_velocity;
                m_velocity = Vector3.zero;
            }

        }
    }

    //-----------------------------Helper functions--------------------------------------------

    float GetMagnitude(Vector3 vector)
    {
        float result = (vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z);
        result = Mathf.Sqrt(result);
        return result;
    }

    float GetAngle(Vector3 a, Vector3 b)
    {
        return DotProduct(a, b) / (GetMagnitude(a) * GetMagnitude(b));
    }

    float DotProduct(Vector3 a, Vector3 b)
    {
        return (a.x + a.y + a.z) + (b.x + b.y + b.z);
    }

    Vector3 CrossProduct(Vector3 v1, Vector3 v2)
    {
        float x = v1.y * v2.z - v2.y * v1.z;
        float y = (v1.x * v2.z - v2.x * v1.z) * -1;
        float z = v1.x * v2.y - v2.x * v1.y;
        return new Vector3(x, y, z);
    }
}
