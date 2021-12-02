using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEffects : MonoBehaviour
{

    public float waterHeight;

    private bool isUnderwater;
    public Color normalColor, underwaterColor;
    public float underwaterDensity, normalDensity;
    public GameObject underwaterOverlay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < waterHeight)
        {
            isUnderwater = true;
            RenderSettings.fogColor = underwaterColor;
            RenderSettings.fogDensity = underwaterDensity;
            underwaterOverlay.SetActive(true);
        }
        else if(transform.position.y >= waterHeight)
        {
            SetAboveWater();
        }
    }

    public void SetAboveWater()
    {
        isUnderwater = false;
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity = normalDensity;
        underwaterOverlay.SetActive(false);
    }
}
