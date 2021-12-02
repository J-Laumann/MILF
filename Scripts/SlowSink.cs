using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowSink : MonoBehaviour
{
    Rigidbody rb;
    public float sinkSpeed, speedup;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < 0)
        {
            rb.useGravity = false;

            if(rb.velocity.y > -sinkSpeed)
            {
                rb.velocity += Vector3.down * speedup * Time.deltaTime;
            }
            else if(rb.velocity.y < -sinkSpeed)
            {
                rb.velocity += Vector3.up * speedup * Time.deltaTime;
            }
        }
    }
}
