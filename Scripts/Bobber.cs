using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{

    public AudioClip snapSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= 0 && !ScubaController.scuba.plopped)
        {
            ScubaController.scuba.PlopBobber();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 0 && ScubaController.scuba.hasCast)
        {
            AudioSource.PlayClipAtPoint(snapSound, transform.position);
            ScubaController.scuba.ResetFishingLine();
        }
    }
}
