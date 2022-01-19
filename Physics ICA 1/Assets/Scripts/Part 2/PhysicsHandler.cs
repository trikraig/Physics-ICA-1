using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHandler : MonoBehaviour
{
    public GameObject m_ballPlane;
    public GameObject m_collideball;
    public GameObject m_collideOtherBall;

    public GameObject planeA;
    public GameObject planeB;
    public GameObject planeC;

    private void FixedUpdate()
    {
        MotionSphereToSphereCollision(m_collideball, m_collideOtherBall);
        CheckSphereToPlaneCollision(m_ballPlane);     
    }

    void MotionSphereToSphereCollision(GameObject ballA, GameObject ballB)
    {
        //Need to find at what point in frame that they collide
        // e.g. when distance between center of two spheres = sum of their radii (r1 + r2)

        Vector3 P1 = ballA.transform.position; // start position vector of sphere 1.
        Vector3 P2 = ballB.transform.position; // start position vector of sphere 2
        Vector3 V1 = ballA.GetComponent<BasicBall>().GetVelocity(); //velocity vector of sphere 1
        Vector3 V2 = ballB.GetComponent<BasicBall>().GetVelocity(); // velocity vector of sphere 2
        float r1 = ballA.GetComponent<BasicBall>().m_radius;
        float r2 = ballB.GetComponent<BasicBall>().m_radius;

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
            float magnitude = (P1CollisionPoint - P2CollisionPoint).magnitude;
            float radiiSum = r1 + r2;

            //Collision occurs when the resulting magnitude is equal to the sum of radii. In this case using <= since close enough 
            if (magnitude <= radiiSum)
            {
                //Point of impact s.
                Vector3 s = P2 + (P1 - P2).normalized * r2; 
                //Vector G from point of impact, through the center of sphere 2 to give the direction in which force acts on sphere 2.
                Vector3 G = P2 - s;
                G.Normalize(); //Directional vector so normalise it.

                //Calculate the proportion of force that gets applied to new velocity. Angle between V1 and G (new directional vector) 
                float q = GetAngle(V1, G);
                var force = Mathf.Cos(q); 

                //Apply proportion of force in direction of V1 in direction G.
                G += V1 * force;

                //Apply new resulting velocity to sphere 2.
                ballB.GetComponent<BasicBall>().SetVelocity(G);
                //Sphere 1 new velocity, assuming mass is 1
                ballA.GetComponent<BasicBall>().SetVelocity(V1 - G);
            }
        }
    }

    void CheckSphereToPlaneCollision(GameObject ball)
    {
        //Check if sphere is on collision course with plane
        //Find angle between N and -V, if angle less than 90 degrees then sphere headed towards plane

        Vector3 planeNormal = Vector3.Cross(planeB.transform.position - planeA.transform.position, planeC.transform.position - planeA.transform.position).normalized;
        Vector3 v = ball.GetComponent<BasicBall>().GetVelocity();
        float angle = Vector3.Angle(planeNormal, -v);

        if (angle < 90)
        {
            PlaneCollision(ball, planeNormal);
        }
    }

    void PlaneCollision(GameObject ballObject, Vector3 planeNormal)
    {
        var k = planeA.transform.position;
        //A vector from k to the start position of the sphere.
        Vector3 P = ballObject.transform.position - k;
        float q1 = Vector3.Angle(planeNormal, P);

        //q1 we already have, q2 is the angle between P and the plane.
        float q2 = 90 - q1;

        // Find d = sin(q2) * |P|
        // The closest distance between the start position of the sphere and the plane
        float d = Mathf.Sin(q2) * P.magnitude;

        //Now need to find the distance to the point of contact between the sphere and the plane
        BasicBall ball = ballObject.GetComponent<BasicBall>();
        Vector3 v = ball.GetVelocity();
        float r = ball.m_radius;
        float s = Vector3.Angle(v, -planeNormal);

        // Need to find Vc, vector from the current sphere position to the collision position
        float distanceToContact = (d - r) / Mathf.Cos(s);

        // If result less than magnitude of velocity vector, then sphere in contact with plane
        if (Mathf.Abs(distanceToContact) <= v.magnitude) 
        {
            //Handle Collision
            planeNormal.Normalize();
            Vector3 vb = 2 * planeNormal * Vector3.Dot(planeNormal, -v.normalized) + v.normalized;
            vb *= v.magnitude;
            ball.SetVelocity(vb);
        }
    }

    Vector3 GetPlaneNormal(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Vector3 v1 = pointB - pointA;
        Vector3 v2 = pointC - pointA;
        return (Vector3.Cross(v1, v2)); //To fix later why returning negative
    }
    float GetAngle(Vector3 a, Vector3 b)
    {
        return Vector3.Dot(a, b) / (a.magnitude * b.magnitude);
    }
}


