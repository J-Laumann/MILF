using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfish : MonoBehaviour
{

    public float moveForce, moveTime, gravity, maxFallSpeed, rotIntensity, maxRot;
    public Vector3 spinVector;
    float moveTimer;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //offset = new Vector3(180, 0, 180);
    }

    // Update is called once per frame
    void Update()
    {
        //rotTimer += Time.deltaTime * 2;
        if(rb.velocity.y > -maxFallSpeed)
            rb.velocity += Vector3.down * gravity * Time.deltaTime;
        transform.Rotate(spinVector * Time.deltaTime);

        transform.Rotate(Vector3.right * Time.deltaTime * Random.Range(-rotIntensity, rotIntensity));
        transform.Rotate(Vector3.forward * Time.deltaTime * Random.Range(-rotIntensity, rotIntensity));

        if(transform.rotation.x < -maxRot || transform.rotation.x > maxRot)
        {
            Quaternion tempRot = transform.rotation;
            tempRot.x = Mathf.Clamp(tempRot.x, -maxRot, maxRot);
            transform.rotation = tempRot;
        }

        if (transform.rotation.z < -maxRot || transform.rotation.z > maxRot)
        {
            Quaternion tempRot = transform.rotation;
            tempRot.z = Mathf.Clamp(tempRot.z, -maxRot, maxRot);
            transform.rotation = tempRot;
        }

        /*
        float randX = transform.rotation.x;
        float randZ = transform.rotation.z;
        Debug.Log(transform.rotation);

        randX = Mathf.Clamp(randX + (Random.Range(-rotIntensity, rotIntensity) * Time.deltaTime), 1f - maxRot, 1f + maxRot);
        randZ = Mathf.Clamp(randZ + (Random.Range(-rotIntensity, rotIntensity) * Time.deltaTime), 1f - maxRot, 1f + maxRot);

        Quaternion newRot = transform.rotation;
        newRot.x = randX;
        newRot.z = randZ;
        transform.rotation = newRot;
        */

        //transform.Rotate(Random.insideUnitCircle * Time.deltaTime * rotIntensity);
        /*
        if (rotTimer < 0)
        {
            transform.eulerAngles = (oldRot * Mathf.Abs(rotTimer)) + offset;
        }
        else
        {
            transform.eulerAngles = (destRot * Mathf.Clamp(rotTimer, 0f, 1f)) + offset;
        }
        */

        if (transform.position.y > 0)
        {
            moveTimer = Time.time + moveTime;
            //transform.Translate(Vector3.down * 0.1f);
            Vector3 temp = rb.velocity;
            if(temp.y > 0)
                temp.y = temp.y / 2;
            rb.velocity = temp;
            return;
        }

        if(Time.time > moveTimer)
        {
            rb.velocity = transform.up * (moveForce + Random.Range(-1f * (moveForce / 3), 1f * (moveForce / 3)));
            moveTimer = Time.time + moveTime + Random.Range(-0.5f, 0.5f);
        }

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        moveTimer = 0;
    }
}
