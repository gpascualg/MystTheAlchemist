using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public class GenerateMap : MonoBehaviour
{
    public static GenerateMap Instance;

    public int MaxY, MinY, MaxX, MinX;
    public int NumPaths;
    public int PathMaxSteps;
    public int PathMinSteps;
    public bool Reload = false;
    public Transform ComponentsContainer;
    public Transform PathsContainer;

    public List<GameObject> Prefabs;

    public List<GameObject> Junction4;
    public List<GameObject> JunctionT;
    public List<GameObject> Straight;
    public List<GameObject> Turn;
    public List<GameObject> Filling;

    private bool[,] map;
    private HashSet<Vector2> flowerSpots;

    private Dictionary<int, GameObject> collectibles;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateAll();
    }

    // Update is called once per frame
    void Update()
    {
        if (Reload)
        {
            Debug.Log("GENERATING");
            GenerateAll();
            Reload = false;
        }
    }

    public static int RandomRange(int seed, int min, int max)
    {
        UnityEngine.Random.InitState(seed);
        var prob = UnityEngine.Random.Range(1e-6f, 1.0f) - 1e-6f;
        //return (int)(RandomProb(seed, seed, seed) * (max - min)) + min;
        return (int)(prob * (max - min)) + min;
    }

    public static float RandomProb(int seed, float x, float y)
    {
        return Mathf.Clamp01(
            Mathf.Abs(
                OpenSimplex2.Noise2_ImproveX(seed, x, y)
            )
        ) - 1e-6f;
    }

    public void GenerateAll()
    {
        int width = (int)Mathf.Ceil((MaxX - MinX) / 0.8f);
        int height = (int)Mathf.Ceil((MaxY - MinY) / 0.8f);
        map = new bool[width, height];
        flowerSpots = new HashSet<Vector2>();
        collectibles = new Dictionary<int, GameObject>();

        Generate();

        int seed = GameManager.Instance.Seed;
        for (int i = 0; i < NumPaths; ++i)
        {
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.03f, 0.03f, 0.9f }, map, width, height);
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.03f, 0.9f, 0.03f }, map, width, height);
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.9f, 0.03f, 0.03f }, map, width, height);
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.9f, 0.03f, 0.03f, 0.03f }, map, width, height);
        }

        paintPaths(map, width, height);

        GameManager.Instance.Seed += 1000;
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

        for(int x = MinX; x < MaxX; x++)
        {
            for(int y = MinY; y < MaxY; y++)
            {
                int currentSeed = GameManager.Instance.Seed;
                foreach (var component in Receipts.Instance.Components)
                {
                    if (component.ComponentType == ComponentType.Potion) continue;
                    if (component.Prefab == null) continue;

                    if (OpenSimplex2.Noise2(currentSeed, x, y) > component.Threshold)
                    {

                        float newX = x + OpenSimplex2.Noise2(currentSeed + RandomRange(currentSeed + x * (MaxY - MinY) + y, 100, 300), x, y);
                        float newY = y + OpenSimplex2.Noise2(currentSeed + RandomRange(currentSeed + x * (MaxY - MinY) + y, 100, 300), x, y);
                        Vector3 position = new Vector3(newX, newY, -0.25f);

                        GameObject go = Instantiate(Prefabs[(int)component.ComponentType], position, Quaternion.identity, ComponentsContainer.transform);
                        var worldComponent = go.GetComponent<WorldComponent>();
                        worldComponent.WorldId = collectibles.Count;
                        worldComponent.AlchemicComponent = component;
                        go.GetComponent<SpriteRenderer>().sprite = component.Sprite;
                        collectibles.Add(collectibles.Count, go);
                    }

                    ++currentSeed;
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

                int direction = RandomRange(GameManager.Instance.Seed + x * height + y, 0, 3);
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

    private void GenerateDrunkardPath(int seed, int totalSteps, int x, int y, float[] prob, bool[,] map, int width, int height, float nextProb = 0.85f)
    {
        int[] idx = new int[] { 0, 1, 2, 3 };

        for (int i = 0; i < totalSteps; ++i)
        {
            map[x, y] = true;

            prob[0] = prob[0] * (y < height - 1).ToInt();
            prob[1] = prob[1] * (y > 0).ToInt();
            prob[2] = prob[2] * (x < width - 1).ToInt();
            prob[3] = prob[3] * (x > 0).ToInt();
            
            float probSum = prob[0] + prob[1] + prob[2] + prob[3];
            prob[0] = prob[0] / probSum;
            prob[1] = prob[1] / probSum;
            prob[2] = prob[2] / probSum;
            prob[3] = prob[3] / probSum;

            Array.Sort(prob, idx);
            float movementProb = RandomProb(seed, x, y);
            float sum = 0.0f;
            for (int j = 3; j >= 0; --j)
            {
                sum += prob[j];
                if (movementProb <= sum + 1e-6f)
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
                
                int numConnections = left.ToInt() + right.ToInt() + top.ToInt() + bottom.ToInt();
                if (left && right && top && bottom)
                {
                    Instantiate(Junction4[RandomRange(x * height + y, 0, Junction4.Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.identity, PathsContainer);
                }
                else if (left && right && top)
                {
                    Instantiate(JunctionT[RandomRange(x * height + y, 0, JunctionT.Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                }
                else if (left && right && bottom)
                {
                    Instantiate(JunctionT[RandomRange(x * height + y, 0, JunctionT.Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 270)), PathsContainer);
                }
                else if (left && bottom && top)
                {
                    Instantiate(JunctionT[RandomRange(x * height + y, 0, JunctionT.Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 180)), PathsContainer);
                }
                else if (top && right && bottom)
                {
                    Instantiate(JunctionT[RandomRange(x * height + y, 0, JunctionT.Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.identity, PathsContainer);
                }
                else if (left && bottom)
                {
                    Instantiate(Turn[RandomRange(x * height + y, 0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 270)), PathsContainer);
                }
                else if (left && top)
                {
                    Instantiate(Turn[RandomRange(x * height + y, 0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 180)), PathsContainer);
                }
                else if (right && top)
                {
                    Instantiate(Turn[RandomRange(x * height + y, 0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                }
                else if (right && bottom)
                {
                    Instantiate(Turn[RandomRange(x * height + y, 0, Turn.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 0)), PathsContainer);
                }
                else if (left || right)
                {
                    Instantiate(Straight[RandomRange(x * height + y, 0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX - 0.2f, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                    Instantiate(Straight[RandomRange(x * height + y, 0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX + 0.2f, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), PathsContainer);
                }
                else if (top || bottom)
                {
                    Instantiate(Straight[RandomRange(x * height + y, 0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY - 0.2f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), PathsContainer);
                    Instantiate(Straight[RandomRange(x * height + y, 0, Straight.Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY + 0.2f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), PathsContainer);
                }

                bool topLeft = (y < height - 1) && (x > 0) && map[x - 1, y + 1];
                bool bottomLeft = (y > 0) && (x > 0) && map[x - 1, y - 1];
                bool topRight = (y < height - 1) && (x < width - 1) && map[x + 1, y + 1];
                bool bottomRight = (y > 0) && (x < width - 1) && map[x + 1, y - 1];

                // Filling
                if (top && topLeft && left)
                {
                    var center = new Vector2(x * 0.8f + MinX - 0.4f, y * 0.8f + MinY + 0.4f);
                    if (RandomProb(GameManager.Instance.Seed, x, y) >= 0.5f)
                    {
                        if (flowerSpots.Add(center))
                        {
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, PathsContainer);
                        }
                    }
                    else
                    {
                        drawFlowersAt(center);
                    }
                }
                if (left && bottomLeft && bottom)
                {
                    var center = new Vector2(x * 0.8f + MinX - 0.4f, y * 0.8f + MinY - 0.4f);
                    if (RandomProb(GameManager.Instance.Seed, x, y) >= 0.5f)
                    {
                        if (flowerSpots.Add(center))
                        {
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, PathsContainer);
                        }
                    }
                    else
                    {
                        drawFlowersAt(center);
                    }
                }
                if (bottom && bottomRight && right)
                {
                    var center = new Vector2(x * 0.8f + MinX + 0.4f, y * 0.8f + MinY - 0.4f);
                    if (RandomProb(GameManager.Instance.Seed, x, y) >= 0.5f)
                    {
                        if (flowerSpots.Add(center))
                        {
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, PathsContainer);
                        }
                    }
                    else
                    {
                        drawFlowersAt(center);
                    }
                }
                if (right && topRight && top)
                {
                    var center = new Vector2(x * 0.8f + MinX + 0.4f, y * 0.8f + MinY + 0.4f);
                    if (RandomProb(GameManager.Instance.Seed, x, y) >= 0.5f)
                    {
                        if (flowerSpots.Add(center))
                        {
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, PathsContainer);
                        }
                    }
                    else
                    {
                        drawFlowersAt(center);
                    }
                }
            }
        }
    }

    public float FlowerXSpacing = 0.15f / 2;
    public float FlowerYSpacing = 0.15f / 1.5f;
    public float FlowerNudge = 0.05f;
    
    private void drawFlowersAt(Vector2 center)
    {
        if (!flowerSpots.Add(center))
        {
            return;
        }

        var component = Receipts.Instance.GetRandomComponentOfType(ComponentType.Flower);
        for (int i = 0; i < 4; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                var x = (j - 2f) * FlowerXSpacing;
                var y = (i - 1.5f) * FlowerYSpacing;
                var nx = OpenSimplex2S.Noise2(i * 5 + j, center.x, center.y) * FlowerNudge;
                var ny = OpenSimplex2S.Noise2(i * 5 + j + 100, center.x, center.y) * FlowerNudge;
                var go = Instantiate(component.Prefab, new Vector3(center.x + x + nx, center.y + y + ny, -0.1f), Quaternion.identity, PathsContainer);
                go.GetComponent<WorldComponent>().WorldId = collectibles.Count;
                collectibles.Add(collectibles.Count, go);
            }
        }
    }

    public void DestroyCollected(int id)
    {
        Destroy(collectibles[id]);
    }
}
