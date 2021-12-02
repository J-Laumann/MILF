using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Succ : MonoBehaviour
{
    public bool isActive;
    public LayerMask fishLayer;
    public float range, force;
    float timer;

    public GameObject effect;

    public static Succ succ;

    // Start is called before the first frame update
    void Start()
    {

        succ = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;

        Collider[] cols = Physics.OverlapSphere(transform.position + transform.forward * range, range, fishLayer);
        foreach(Collider col in cols)
        {
            FishData fd = col.gameObject.GetComponent<FishData>();
            if (fd)
            {
                if (fd.health <= 0)
                {
                    col.transform.position = Vector3.Slerp(col.transform.position, transform.position, force);
                }
                else if (Time.time >= timer)
                {
                    fd.TakeDamage(2, 0);
                }
            }
        }
        if(cols.Length > 0 && Time.time >= timer)
        {
            timer = Time.time + 0.2f;
        }
    }
}
