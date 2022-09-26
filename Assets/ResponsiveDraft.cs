using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveDraft : MonoBehaviour
{
    private Animation anim;
    private string[] alphabet = new string[3] {"A", "B", "C"};
    private float x; 
    private float y;
    private Vector2 pos;

    private float globalScaleMultiplier = 1f;
    private float scaleMin = .05f;
    private float scaleMax = .2f;

    //sprite.BackgroundColors = GameObject.Find("BackgroundColors").transform;

    public int objCount = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        //move this later to update
        populateSprites();        
    }

    public void populateSprites()
    {
        //this is dependent on mediaPipe input
        int objC = Random.Range(0, objCount);
        string randLetter;

        for (int i = 0; i < objC; i++) {
            randLetter = alphabet[Random.Range(0, 3)];
            GameObject mySprite = createSprite(randLetter, i);
            //mySprite.transform.SetParent(backgroundColors);
        }        
    }

    GameObject createSprite(string letter, int num) {
        GameObject obj = Instantiate(Resources.Load("petalBloom" + letter) as GameObject, transform);
        SpriteRenderer spr = obj.GetComponent<SpriteRenderer>();
        obj.name = "petalBloom" + letter + num;

        randomizePosition(obj);
        randomizeScale(obj);
        randomizeColor(obj);

        playAndDisappear(letter);

        return obj;
    }

    void randomizePosition(GameObject obj)
    {
        x = Random.Range(-6, 6);
        y = Random.Range(-5, 5);
        pos = new Vector2(x, y);
        obj.transform.position = pos;
    }

    void randomizeScale(GameObject obj)
    {
        Vector3 randomizedScale = Vector3.one;
        float newScale = Random.Range(scaleMin, scaleMax);
        randomizedScale = new Vector3(newScale, newScale, newScale);
        obj.transform.localScale = randomizedScale * globalScaleMultiplier;
    }

    void randomizeColor(GameObject obj) {
        Color myNewColor = new Color(
            (float)Random.Range(0f, 1f),
            (float)Random.Range(0f, 1f),
            (float)Random.Range(0f, 1f)
        );

        SpriteRenderer s = GetComponent<SpriteRenderer>();
        s.color = myNewColor;
    }
    
    IEnumerator playAndDisappear(string letter) {
        anim.Play("PetalBloom" + letter);
        yield return new WaitForSeconds(anim["PetalBloom" + letter].length);
        gameObject.SetActive(false);
    }

    //Update is called once per frame
    void Update()
    {
        
    }
}