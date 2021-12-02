using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{

    Rigidbody rb;
    [HideInInspector]
    public GameObject player;

    public GameObject engine, prop, seat;
    new public GameObject camera;
    public float speed, rotSpeed, maxSpeed;

    int currentRadioSong = 0;
    public AudioSource radioSource;
    public AudioClip[] radioSongs;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if(rb.velocity.magnitude < maxSpeed)
            rb.velocity += (transform.forward * v * speed * Time.deltaTime);

        if(v != 0)
        {
            transform.Rotate(Vector3.up * (rb.velocity.magnitude / maxSpeed) * rotSpeed * h * Time.deltaTime);
            prop.transform.Rotate(Vector3.forward * (rb.velocity.magnitude / maxSpeed) * Time.deltaTime * v * 1280);
        }

        engine.transform.localEulerAngles = Vector3.up * h * 45;

        //Exitting Boat
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            ScubaController scuba = player.GetComponent<ScubaController>();
            scuba.enabled = true;
            scuba.camera.SetActive(true);
            camera.SetActive(false);
            scuba.transform.parent = null;
            scuba.GetComponent<Collider>().isTrigger = false;
            scuba.rb.isKinematic = false;
            this.enabled = false;
        }
    }

    public void NextRadioSong()
    {
        currentRadioSong++;
        if(currentRadioSong >= radioSongs.Length)
        {
            currentRadioSong = 0;
        }
        radioSource.clip = radioSongs[currentRadioSong];
        radioSource.Play();
    }
}
