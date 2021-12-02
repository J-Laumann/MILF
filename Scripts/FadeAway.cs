using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAway : MonoBehaviour
{

    public float startTime, fadeTime;
    Image img;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        StartCoroutine(Fade());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(startTime);
        float timer = 1;
        Color tempColor = img.color;
        while (timer > 0)
        {
            timer -= Time.deltaTime / fadeTime;
            tempColor.a = timer;
            img.color = tempColor;
            yield return new WaitForEndOfFrame();
        }
        tempColor.a = 0;
        img.color = tempColor;
        img.gameObject.SetActive(false);
    }
}
