using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public int MaxY, MinY, MaxX, MinX;
    public bool Reload = false;
    public Transform ComponentsContainer;
    public int Seed;

    // Start is called before the first frame update
    void Start()
    {
        MaxY = 100;
        MinY = -100;
        MaxX = 100;
        MinX = -100;
        Seed = 0;
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
        foreach (Transform child in ComponentsContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int currentSeed = Seed;

        for(int x = MinX; x < MaxX; x++)
        {
            for(int y = MinY; y < MaxY; y++)
            {
                Debug.Log(Seed);
                Seed = currentSeed;
                foreach (var component in Receipts.Instance.Components)
                {
                    if (component.ComponentType == ComponentType.Potion) continue;
                    if (OpenSimplex2.Noise2(Seed, x, y) > component.Threshold){

                        float newX = x + OpenSimplex2.Noise2(Seed + UnityEngine.Random.Range(100, 300), x, y);
                        float newY = y + OpenSimplex2.Noise2(Seed + UnityEngine.Random.Range(100, 300), x, y);
                        Vector3 position = new Vector3(newX, newY, 0);

                        Debug.Log(component.Name + " at: (" + newX + ", " + newY + ")");

                        GameObject newObject = Instantiate(component.Prefab, position, Quaternion.identity, ComponentsContainer.transform);

                        Seed += 1;
                    }
                }
            }
        }
    }
}
