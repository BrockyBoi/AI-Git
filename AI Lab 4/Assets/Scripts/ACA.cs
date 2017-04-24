using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACA : MonoBehaviour
{
    static List<ACA> agents = new List<ACA>();

    [Range(0.01f, 5f)]
    public float speed = 1.0f;
    [Range(1, 5)]
    public float size = 1.0f;
    [Range(0.5f, 10f)]
    public float tH;
    const float maxF = 20f;
    const float worldSize = 20f;

    public Vector3 goalVelocity;
    Vector3 velocity;
    Vector3 force;

    // Use this for initialization
    void Start()
    {
        goalVelocity = Random.onUnitSphere;
        goalVelocity.y = 0;
        goalVelocity.Normalize();
        size = 1.0f;
        speed = 1.0f;
        tH = 2.0f;
        agents.Add(this);
        if (agents.Count == 1)
        {
            for (int x = 0; x < 24; x++)
            {
                ACA next = Instantiate(this);
                next.transform.position = new Vector3(Random.Range(0f, 20f), 0, Random.Range(0f, 20f));
                next.name = "Agent " + (x + 1);
            }
        }
    }

    float ttcTorus(ACA i, ACA j, out Vector3 offset)
    {
        float xDiff, yDiff;
        float t1, t2, t3, t4;
        Vector3 o1, o2, o3, o4;
        if (transform.position.x > worldSize / 2)
        {
            xDiff = worldSize;
        }
        else xDiff = -worldSize;

        if (transform.position.z > worldSize / 2)
        {
            yDiff = worldSize;
        }
        else yDiff = -worldSize;

        o1 = new Vector3(xDiff, 0, 0);
        o2 = new Vector3(0, 0, yDiff);
        o3 = new Vector3(xDiff, 0, yDiff);
        o4 = new Vector3(0, 0, 0);

        ttc(i, j, out t1, o1);
        ttc(i, j, out t2, o2);
        ttc(i, j, out t3, o3);
        ttc(i, j, out t4, o4);

        if (t1 < t2 && t1 < t3 && t1 < t4)
        {
            offset = o1;
            return t1;
        }
        else if (t2 < t1 && t2 < t3 && t2 < t4)
        {
            offset = o2;
            return t2;
        }
        else if (t3 < t1 && t3 < t2 && t3 < t4)
        {
            offset = o3;
            return t3;
        }
        else
        {
            offset = o4;
            return t4;
        }


    }
    void ttc(ACA i, ACA j, out float t, Vector3 offset)
    {
        // Ignore this parameter initially
       //offset = Vector3.zero;
        // Now compute the TTC and put the result in t

        t = Mathf.Infinity; // temporary value

        float r = (i.size / 2f) + (j.size / 2f);
        Vector3 w;
        w = j.transform.position - (i.transform.position - offset);

        float c = Vector3.Dot(w, w) - r * r;

        if (c < 0)
        {
            t = 0;
            return;
        }

        Vector3 v = i.velocity - j.velocity;
        float a = Vector3.Dot(v, v);
        float b = Vector3.Dot(w, v);

        float discr = b * b - a * c;

        if (discr <= 0)
        {
            return;
        }
        t = (b - Mathf.Sqrt(discr)) / a;
        if (t < 0)
        {
            t = Mathf.Infinity;
            return;
        }


        //F goal = (Vg - V) * K
        //Favoid = (tH - t / t) * K * ((x1 + tv1) - (x2 +tv2)).normalized
    }

    void OnGUI()
    {
        transform.localScale = new Vector3(size, 1f, size);
        goalVelocity.y = 0;
        goalVelocity = goalVelocity.normalized * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // Compute force towards 
        force = 2f * (goalVelocity - velocity);
        for (int x = 0; x < agents.Count; x++)
        {
            if (agents[x] != this)
            {
                // Compute TTC for all agents not us
                // for those under the threshold 
                float t;
                Vector3 offset;
                t = ttcTorus(this, agents[x], out offset);

                if (t > tH)
                    continue;
                Vector3 avoid;
                    avoid = ((transform.position - offset + t * velocity) - (agents[x].transform.position + t * agents[x].velocity)).normalized;

                if (avoid.x != 0 && avoid.z != 0)
                {
                    avoid /= Mathf.Sqrt(Vector3.Dot(avoid, avoid));
                }

                float mag = 0;
                if (t >= 0 && t <= tH)
                {
                    mag = (tH - t) / (t + .001f);
                }
                if (mag > maxF)
                    mag = maxF;

                avoid *= mag;

                force += avoid;
                // Add to the cumulative force on agent

            }
        }
    }

    void LateUpdate()
    {
        // Convert force to velocity and to change in position
        velocity += force * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // Wrap around torus world
        if (transform.position.x > worldSize)
            transform.position -= new Vector3(worldSize, 0, 0);
        else if (transform.position.x < 0)
            transform.position += new Vector3(worldSize, 0, 0);
        if (transform.position.z > worldSize)
            transform.position -= new Vector3(0, 0, worldSize);
        else if (transform.position.z < 0)
            transform.position += new Vector3(0, 0, worldSize);

        // Draw our actual velocity and the target velocity
        Debug.DrawLine(transform.position, transform.position + velocity);
        Debug.DrawLine(transform.position, transform.position + goalVelocity, Color.black);
    }
}
