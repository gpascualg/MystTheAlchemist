using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public int MaxY, MinY, MaxX, MinX;
    public int NumPaths;
    public int PathMaxSteps;
    public int PathMinSteps;
    public bool Reload = false;
    public Transform ComponentsContainer;
    public Transform PathsContainer;
    public int Seed;

    public List<GameObject> Junction4;
    public List<GameObject> JunctionT;
    public List<GameObject> Straight;
    public List<GameObject> Turn;
    public List<GameObject> Filling;

    private bool[,] map;

    // Start is called before the first frame update
    void Start()
    {
        Seed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Reload)
        {
            Debug.Log("GENERATING");

            int width = (int)Mathf.Ceil((MaxX - MinX) / 0.8f);
            int height = (int)Mathf.Ceil((MaxY - MinY) / 0.8f);
            map = new bool[width, height];

            Generate();

            GenerateDrunkardPath(UnityEngine.Random.Range(PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.03f, 0.03f, 0.9f }, map, width, height);
            GenerateDrunkardPath(UnityEngine.Random.Range(PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.03f, 0.9f, 0.03f }, map, width, height);
            GenerateDrunkardPath(UnityEngine.Random.Range(PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.9f, 0.03f, 0.03f }, map, width, height);
            GenerateDrunkardPath(UnityEngine.Random.Range(PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.9f, 0.03f, 0.03f, 0.03f }, map, width, height);

            paintPaths(map, width, height);
            Reload = false;
        }
    }

    public void Generate()
    {
        foreach (Transform child in ComponentsContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in PathsContainer.transform)
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
                        Vector3 position = new Vector3(newX, newY, -0.25f);

                        //Debug.Log(component.Name + " at: (" + newX + ", " + newY + ")");

                        GameObject newObject = Instantiate(component.Prefab, position, Quaternion.identity, ComponentsContainer.transform);

                        Seed += 1;
                    }
                }
            }
        }
    }

    private void GeneratePath(int totalSteps, int x, int y, bool[,] map, int width, int height)
    {
        for (int i = 0; i < totalSteps; ++i)
        {
            map[x, y] = true;

            int prevX = x;
            int prevY = y;

            do
            {
                x = prevX;
                y = prevY;

                int direction = UnityEngine.Random.Range(0, 3);
                if (direction == 0)
                {
                    y += 1;
                }
                else if (direction == 1)
                {
                    y -= 1;
                }
                else if (direction == 2)
                {
                    x += 1;
                }
                else if (direction == 3)
                {
                    x -= 1;
                }
            }
            while (x < 0 || x >= width || y < 0 || y >= height);
        }
    }

    private void GenerateDrunkardPath(int totalSteps, int x, int y, float[] prob, bool[,] map, int width, int height, float nextProb = 0.95f)
    {
        int[] idx = new int[] { 0, 1, 2, 3 };

        for (int i = 0; i < totalSteps; ++i)
        {
            map[x, y] = true;

            int prevX = x;
            int prevY = y;

            do
            {
                x = prevX;
                y = prevY;

                Array.Sort(prob, idx);
                float movementProb = UnityEngine.Random.Range(0.0f, 1.0f);
                float sum = 0.0f;
                for (int j = 3; j >= 0; --j)
                {
                    sum += prob[j];
                    if (movementProb < sum)
                    {
                        int direction = idx[j];
                        if (direction == 0)
                        {
                            y += 1;

                            prob = new float[] { nextProb, 0.0f, (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f };
                            idx = new int[] { 0, 1, 2, 3 };
                        }
                        else if (direction == 1)
                        {
                            y -= 1;

                            prob = new float[] { 0.0f, nextProb, (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f };
                            idx = new int[] { 0, 1, 2, 3 };
                        }
                        else if (direction == 2)
                        {
                            x += 1;

                            prob = new float[] { (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f, nextProb, 0.0f };
                            idx = new int[] { 0, 1, 2, 3 };
                        }
                        else if (direction == 3)
                        {
                            x -= 1;

                            prob = new float[] { (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f, 0.0f, nextProb };
                            idx = new int[] { 0, 1, 2, 3 };
                        }

                        break;
                    }
                }
            }
            while (x < 0 || x >= width || y < 0 || y >= height);
        }
    }

    private void paintPaths(bool[,] map, int width, int height)
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                if (!map[x, y])
                {
                    continue;
                }
                
                bool left = (x > 0) && map[x - 1, y];
                bool right = (x < width - 1) && map[x + 1, y];
                bool top = (y < height - 1) && map[x, y + 1];
                bool bottom = (y > 0) && map[x, y - 1];

                //bool topleft = (y < height - 1) && (x > 0) && map[x - 1, y + 1];
                //bool bottomleft = (y > 0) && (x > 0) && map[x - 1, y - 1];
                //bool topright = (y < height - 1) && (x < width - 1) && map[x + 1, y + 1];
                //bool bottomright = (y > 0) && (x < width - 1) && map[x + 1, y - 1];
                
                int numConnections = left.ToInt() + right.ToInt() + top.ToInt() + bottom.ToInt();
                if (left && right && top && bottom)
                {
                    Instantiate(Junction4[UnityEngine.Random.Range(0, Junction4.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.identity, PathsContainer);
                }
                else if (left && right && top)
                {
                    Instantiate(JunctionT[UnityEngine.Random.Range(0, JunctionT.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                }
                else if (left && right && bottom)
                {
                    Instantiate(JunctionT[UnityEngine.Random.Range(0, JunctionT.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 270)), PathsContainer);
                }
                else if (left && bottom && top)
                {
                    Instantiate(JunctionT[UnityEngine.Random.Range(0, JunctionT.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 180)), PathsContainer);
                }
                else if (top && right && bottom)
                {
                    Instantiate(JunctionT[UnityEngine.Random.Range(0, JunctionT.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.identity, PathsContainer);
                }
                else if (left && bottom)
                {
                    Instantiate(Turn[UnityEngine.Random.Range(0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 270)), PathsContainer);
                }
                else if (left && top)
                {
                    Instantiate(Turn[UnityEngine.Random.Range(0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 180)), PathsContainer);
                }
                else if (right && top)
                {
                    Instantiate(Turn[UnityEngine.Random.Range(0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                }
                else if (right && bottom)
                {
                    Instantiate(Turn[UnityEngine.Random.Range(0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 0)), PathsContainer);
                }
                else if (left || right)
                {
                    Instantiate(Straight[UnityEngine.Random.Range(0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX - 0.2f, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                    Instantiate(Straight[UnityEngine.Random.Range(0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX + 0.2f, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                }
                else if (top || bottom)
                {
                    Instantiate(Straight[UnityEngine.Random.Range(0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY - 0.2f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), PathsContainer);
                    Instantiate(Straight[UnityEngine.Random.Range(0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY + 0.2f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), PathsContainer);
                }

                //// Filling
                //if (top && topleft && left)
                //{
                //    Instantiate(Filling[UnityEngine.Random.Range(0, Filling.Count - 1)], new Vector3(x * 0.4f + MinX, y * 0.4f + MinY, 0), Quaternion.identity, PathsContainer);
                //}
                //if (left && bottomleft && bottom)
                //{
                //    Instantiate(Filling[UnityEngine.Random.Range(0, Filling.Count - 1)], new Vector3(x * 0.4f + MinX, y * 0.4f + MinY, 0), Quaternion.identity, PathsContainer);
                //}
                //if (bottom && bottomright && right)
                //{
                //    Instantiate(Filling[UnityEngine.Random.Range(0, Filling.Count - 1)], new Vector3(x * 0.4f + MinX, y * 0.4f + MinY, 0), Quaternion.identity, PathsContainer);
                //}
                //if (right && topright && top)
                //{
                //    Instantiate(Filling[UnityEngine.Random.Range(0, Filling.Count - 1)], new Vector3(x * 0.4f + MinX, y * 0.4f + MinY, 0), Quaternion.identity, PathsContainer);
                //}
            }
        }
    }
}
