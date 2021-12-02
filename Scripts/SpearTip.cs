using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpearTip : MonoBehaviour
{
    Rigidbody rb;
    public GameObject bubbles;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (!ScubaController.scuba.spearedFish && ScubaController.scuba.hasShot)
            {
                FishData fd = other.GetComponent<FishData>();
                if (fd)
                {
                    if (fd.invuln)
                    {
                        Instantiate(bubbles, transform.position, transform.rotation);
                    }
                    else
                    {
                        Boid b = other.gameObject.GetComponent<Boid>();
                        if (b)
                            b.enabled = false;
                        FloorDweller floor = other.gameObject.GetComponent<FloorDweller>();
                        if (floor)
                            floor.enabled = false;
                        NavMeshAgent nav = other.gameObject.GetComponent<NavMeshAgent>();
                        if (nav)
                            nav.enabled = false;

                        ScubaController.scuba.spearedFish = fd;
                        other.transform.parent = transform;
                        rb.velocity = Vector3.zero;
                        rb.useGravity = false;
                        transform.LookAt(other.transform);

                        //Tutorial
                        if (Tutorial.t)
                        {
                            if (Tutorial.t.tutorial_UI5.activeSelf)
                            {
                                Tutorial.t.tutorial_UI5.SetActive(false);
                                Tutorial.t.tutorial_UI6.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
        else if(other.gameObject.layer == 0 && !ScubaController.scuba.spearedFish)
        {
            ScubaController.scuba.ResetGun();
        }
    }
}
