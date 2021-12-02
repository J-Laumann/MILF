using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bait : MonoBehaviour
{
    public string baitName;
    public float destroyTime, radius;
    public LayerMask fishLayer;
    public int id;

    public List<FishData> feeders;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("AttractFish", 0, 1);
        Invoke("Done", destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void AttractFish()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, fishLayer);
        foreach(Collider col in cols)
        {
            FishData fd = col.GetComponent<FishData>();
            if (fd)
            {
                if (fd.baitIDs.Contains(id))
                {
                    Boid b = col.GetComponent<Boid>();
                    FloorDweller f = col.GetComponent<FloorDweller>();
                    if (b)
                    {
                        if (!feeders.Contains(fd))
                        {
                            feeders.Add(fd);
                            b.boidSpeed /= 2;
                            fd.invuln = false;
                        }
                        b.fishBrainDir = transform.position - b.transform.position;
                    }
                    else if (f)
                    {
                        if (!feeders.Contains(fd))
                        {
                            feeders.Add(fd);
                            fd.invuln = false;
                            f.enabled = false;
                        }
                        f.GetComponent<NavMeshAgent>().SetDestination(transform.position);
                    }
                }
            }
        }
    }

    void Done()
    {
        foreach(FishData fd in feeders)
        {
            Boid b = fd.GetComponent<Boid>();
            FloorDweller f = fd.GetComponent<FloorDweller>();
            if (b)
            {
                b.boidSpeed *= 2;
                b.fishBrainDir = Vector3.zero;
            }
            else if (f)
                f.enabled = true;
            
        }
        Destroy(gameObject);
    }
}
