using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public int MaxY, MinY, MaxX, MinX;
    public bool Reload = false;
    private float hyacinthThreshold = 0.8f;
    private long hyacinthSeed = 1000;
    public GameObject Hyacinth;
    private float lavenderThreshold = 0.9f;
    private long lavenderSeed = 3000;
    public GameObject Lavender;
    // Start is called before the first frame update
    void Start()
    {
        MaxY = 100;
        MinY = -100;
        MaxX = 100;
        MinX = -100;
    }

    // Update is called once per frame
    void Update()
    {
        if (Reload)
        {
            Debug.Log("GENERATING");
            Generate();
            Reload = false;
        }
    }

    public void Generate()
    {
        Debug.Log("In generate");
        for(int x = MinX; x < MaxX; x++)
        {
            Debug.Log(x);
            for(int y = MinY; y < MaxY; y++)
            {
                Debug.Log(y);
                if(OpenSimplex2.Noise2(hyacinthSeed, x, y) > hyacinthThreshold)
                {
                    float newX = x + OpenSimplex2.Noise2(hyacinthSeed + 100, x, y);
                    float newY = y + OpenSimplex2.Noise2(hyacinthSeed + 50, x, y);
                    Vector3 position = new Vector3(newX, newY, 0);
                    Debug.Log("Hyacinth at: (" + newX + ", " + newY + ")");
                    GameObject newObject = Instantiate(Hyacinth, position, Quaternion.identity);
                };
                if (OpenSimplex2.Noise2(lavenderSeed, x, y) > lavenderThreshold)
                {
                    float newX = x + OpenSimplex2.Noise2(lavenderSeed + 30, x, y);
                    float newY = y + OpenSimplex2.Noise2(lavenderSeed + 200, x, y);
                    Vector3 position = new Vector3(newX, newY, 0);
                    Debug.Log("Lavender at: (" + newX + ", " + newY + ")");
                    GameObject newObject = Instantiate(Lavender, position, Quaternion.identity);
                };
            }
        }
    }
}
