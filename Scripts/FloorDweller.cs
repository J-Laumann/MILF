using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FloorDweller : MonoBehaviour
{

    public LayerMask mask;
    public GameObject model, slopeChecker;

    NavMeshAgent nav;
    Vector3 destination;
    float moveTimer;

    public float moveRange, moveTime;

    Boid boid;
    Rigidbody rb;
    float boidTimer;
    public float boidSwapMin, boidSwapMax;

    // Start is called before the first frame update
    void Awake()
    {
        NavMeshHit closestHit;
 
        if (NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 10000f, NavMesh.AllAreas))
            gameObject.transform.position = closestHit.position;
        else
            Debug.LogError("Could not find position on NavMesh!");

        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
        nav.enabled = true;
        destination = transform.position;
        slopeChecker.transform.rotation = Quaternion.identity;

        boid = GetComponent<Boid>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(boid && Time.time >= boidTimer)
        {
            boidTimer = Time.time + Random.Range(boidSwapMin, boidSwapMax);
            boid.enabled = !boid.enabled;
            nav.enabled = false;
            //if swapping to floor
            if (!boid.enabled)
            {
                if(!rb)
                    rb = gameObject.AddComponent<Rigidbody>();
                rb.velocity = boid.velocity;
            }
            else
            {
                transform.Translate(Vector3.up * 3);
            }
        }

        
        if (!boid || !boid.enabled)
        {
            if (Time.time >= moveTimer)
            {
                Vector2 rand = Random.insideUnitCircle;
                Vector3 newDest = new Vector3(rand.x, 0, rand.y);
                newDest *= moveRange;

                destination = transform.position + (newDest);
                if(nav.isOnNavMesh)
                    nav.SetDestination(destination);
                moveTimer = Time.time + moveTime;
            }

            if (Vector3.Distance(slopeChecker.transform.position, transform.position) > 5)
            {
                slopeChecker.transform.position = transform.position;
            }
            model.transform.rotation = slopeChecker.transform.rotation;
        }
        /*
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10, mask))
        {
            Debug.Log("Rotate Crab UP: " + hit.transform.gameObject.name);
            Debug.Log(hit.point + " | " + model.transform.position);
            model.transform.up = hit.point - model.transform.position;

            //var slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            //transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (boid)
        {
            if(rb && other.gameObject.tag == "Ground")
            {
                slopeChecker.transform.rotation = Quaternion.identity;
                slopeChecker.transform.position = transform.position;
                Destroy(rb);
                nav.enabled = true;
            }
        }
    }
}
