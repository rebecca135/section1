using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteToggle : MonoBehaviour
{
    private float x; 
    private float y;
    private Vector2 pos;

    private float globalScaleMultiplier = 1f;
    private float scaleMin = 0.25f;
    private float scaleMax = 0.6f;
    private float time = 0f;
    public float timeDelay;
    private double temp;
    private bool toggle = false;
    public MediaPipeUDPRecv mediaPipe;
    
    // Start is called before the first frame update
    void Start()
    {
        randomizePosition();
        randomizeScale();
        randomizeColor();
    }

    void randomizePosition()
    {
        x = Random.Range(-8, 8);
        y = Random.Range(-7, 7);
        pos = new Vector2(x, y);
        transform.position = pos;
    }

    void randomizeScale()
    {
        Vector3 randomizedScale = Vector3.one;
        float newScale = Random.Range(scaleMin, scaleMax);
        randomizedScale = new Vector3(newScale, newScale, newScale);
        transform.localScale = randomizedScale * globalScaleMultiplier;
    }

    void randomizeColor() {
        Color myNewColor = new Color(
            (float)Random.Range(0f, 1f),
            (float)Random.Range(0f, 1f),
            (float)Random.Range(0f, 1f)
        );

        SpriteRenderer s = GetComponent<SpriteRenderer>();
        GetComponent<SpriteRenderer>().material.color = myNewColor;
        //s.color = myNewColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (mediaPipe.curr > 0) {
            temp = mediaPipe.curr;
        } else {
            temp = mediaPipe.curr;
        }
        time = time + 1f * Time.deltaTime;
        if (time >= (timeDelay - (temp))) {
            time = 0f;
            if (toggle) {
                toggle = false;
                GetComponent<Renderer>().enabled = toggle;
            } else if (toggle == false) {
                toggle = true;
                GetComponent<Renderer>().enabled = toggle;
            }
            randomizePosition();
            randomizeScale();
            randomizeColor();
        }
    }
}
