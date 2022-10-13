using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteToggle : MonoBehaviour
{
    private float x; 
    private float y;
    private Vector2 pos;

    private float globalScaleMultiplier = 1f;
    private float scaleMin = 0.1f;
    private float scaleMax = 0.35f;
    private float time = 0f;
    public float timeDelay;
    public float threshold;
    private double temp;
    private float timeRandom;
    private bool toggle = false;
    public MediaPipeUDPRecv mediaPipe;
    public GameObject dupe;
    
    // Start is called before the first frame update
    void Start()
    {
        randomizeColor();
        randomizePosition();
        randomizeScale();
    }

    void randomizePosition()
    {
        x = Random.Range(-8, 8);
        y = Random.Range((float) -5.5, (float) 5.5);
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
        float colorVar = (float)mediaPipe.curr / 3;
        Color myNewColor = new Color(
            (float)Random.Range((0.1f + colorVar), (0.6f + colorVar)),
            (float)Random.Range((0.1f + colorVar), (0.6f + colorVar)),
            (float)Random.Range((0.1f + colorVar), (0.6f + colorVar)),
            (float)Random.Range((0.5f + colorVar), (0.6f + colorVar))
        );

        SpriteRenderer s = GetComponent<SpriteRenderer>();
        GetComponent<SpriteRenderer>().material.color = myNewColor;
        //s.color = myNewColor;
    }

    void randomizeTime() {
        timeRandom = (float)Random.Range(0f, 0.25f);
    }

    // Update is called once per frame
    void Update()
    {
        if (mediaPipe.curr > 0) {
            temp = mediaPipe.curr * mediaPipe.curr + 0.5;
        } else {
            temp = 3 * (0 - (mediaPipe.curr * mediaPipe.curr)) - 1.5;
        }
        time = time + 1f * Time.deltaTime;
        if (time >= (timeDelay + timeRandom - temp)) {
            time = 0f;
            if (toggle) {
                toggle = false;
                if (mediaPipe.curr > threshold) {
                    GetComponent<Renderer>().enabled = toggle;
                    dupe.GetComponent<Renderer>().enabled = true;
                } else {
                    GetComponent<Renderer>().enabled = toggle;
                    dupe.GetComponent<Renderer>().enabled = false;
                }
            } else if (toggle == false) {
                toggle = true;
                if (mediaPipe.curr > threshold) {
                    GetComponent<Renderer>().enabled = toggle;
                    dupe.GetComponent<Renderer>().enabled = true;
                } else {
                    GetComponent<Renderer>().enabled = toggle;
                    dupe.GetComponent<Renderer>().enabled = false;
                }
            }
            randomizeTime();
            randomizePosition();
            randomizeScale();
            randomizeColor();
        }
    }
}
