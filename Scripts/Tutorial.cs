using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorial_bait;

    public GameObject tutorial_Walls1;

    public GameObject tutorial_UI1, tutorial_UI2, tutorial_UI3, tutorial_UI4, tutorial_UI5, tutorial_UI6, tutorial_UI7;

    public static Tutorial t;

    // Start is called before the first frame update
    void Awake()
    {
        t = this;
        tutorial_UI1.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
