using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -0.02f)
        {
            rb.velocity += Vector3.up * Time.deltaTime * 13;
            rb.useGravity = true;
        }
        else if(transform.position.y <= 0f)
        {
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
        }

    }
}
