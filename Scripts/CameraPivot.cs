using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{

    public float rotSpeed;
    public Transform target;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        float movement = Input.GetAxis("Mouse X") * rotSpeed;
        transform.Rotate(Vector3.up * movement);
    }
}
