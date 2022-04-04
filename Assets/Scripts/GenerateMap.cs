using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;



public enum TileType
{
    Empty,
    Stone,
    Sand,
    River
}

[System.Serializable]
public class PerTypePrefabs
{
    public List<GameObject> StonePrefabs;
    public List<GameObject> SandPrefabs;
    public List<GameObject> RiverPrefabs;

    public List<GameObject> this[int key]
    {
        get 
        {
            if (key == (int)TileType.Stone) return StonePrefabs;
            if (key == (int)TileType.Sand) return SandPrefabs;
            if (key == (int)TileType.River) return RiverPrefabs;
            return null;
        }
    }
}

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

    private GameObject RealComponentsContainer;
    private GameObject RealPathsContainer;

    public List<GameObject> Prefabs;
    public List<GameObject> GrassFiller;

    public PerTypePrefabs Junction4;
    public PerTypePrefabs JunctionT;
    public PerTypePrefabs Straight;
    public PerTypePrefabs Turn;
    public List<GameObject> Filling;

    private TileType[,] map;
    private HashSet<Vector2> flowerSpots;

    private Dictionary<int, GameObject> collectibles;
    private int objectCount;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {}

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
        // CONTAINERS
        if (RealComponentsContainer != null)
        {
            Destroy(RealComponentsContainer);
        }

        if (RealPathsContainer != null)
        {
            Destroy(RealPathsContainer);
        }

        RealComponentsContainer = new GameObject("Container");
        RealComponentsContainer.transform.parent = ComponentsContainer;

        RealPathsContainer = new GameObject("Container");
        RealPathsContainer.transform.parent = PathsContainer;

        // MAP
        int width = (int)Mathf.Ceil((MaxX - MinX) / 0.8f);
        int height = (int)Mathf.Ceil((MaxY - MinY) / 0.8f);
        map = new TileType[width, height];
        flowerSpots = new HashSet<Vector2>();
        collectibles = new Dictionary<int, GameObject>();
        objectCount = 0;

        int seed = GameManager.Instance.Seed;
        for (int i = 0; i < NumPaths; ++i)
        {
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.03f, 0.03f, 0.9f }, map, width, height, TileType.Stone);
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.03f, 0.9f, 0.03f }, map, width, height);
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.03f, 0.9f, 0.03f, 0.03f }, map, width, height);
            GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), -MinX, -MinY, new float[] { 0.9f, 0.03f, 0.03f, 0.03f }, map, width, height);
        }

        var riverProb = new float[] { 0.0f, 1.0f, 0.0f, 0.0f };
        GenerateDrunkardPath(seed, RandomRange(seed++, PathMinSteps, PathMaxSteps), RandomRange(seed, MinX, MaxX) - MinX, MaxY - 1, riverProb, map, width, height, TileType.River, 0.99f);

        paintPaths(map, width, height);

        // OBJECTS
        Generate(map, width, height);

        GameManager.Instance.Seed += 1000;
    }

    public void Generate(TileType[,] map, int width, int height)
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                if (map[x, y] == TileType.Sand) continue;

                int currentSeed = GameManager.Instance.Seed;

                float realX = x * 0.8f + MinX;
                float realY = y * 0.8f + MinY;

                if (map[x, y] != TileType.River)
                {
                    // Filler grass
                    int fillerSeed = currentSeed + x * (MaxY - MinY) + y;
                    for (int n = 0, nTotal = RandomRange(fillerSeed, 0, 15); n < nTotal; ++n)
                    {
                        float newX = realX + (RandomProb(currentSeed * 300 + x + n, x, y) - 0.5f) * 1.0f;
                        float newY = realY + (RandomProb(currentSeed * 400 + y + n, x, y) - 0.5f) * 1.0f;
                        Vector3 position = new Vector3(newX, newY, 0.25f);
                        var go = Instantiate(GrassFiller[RandomRange(fillerSeed + n, 0, GrassFiller.Count)], position, Quaternion.identity, RealComponentsContainer.transform);

                        if (RandomProb(fillerSeed + n, x, y) > 0.5f)
                        {
                            go.transform.localScale = new Vector3(-1, 1, 1);
                        }
                    }
                }

                if (realX * realX + realY * realY < 1.5f)
                {
                    continue;
                }

                // Actual objects
                foreach (var component in Receipts.Instance.Components)
                {
                    if (component.ComponentType == ComponentType.Potion) continue;
                    if (component.Name == "Water" && map[x, y] != TileType.River) continue;
                    if (map[x, y] == TileType.River && component.Name != "Water") continue;
                    if (component.Name == "Stone" && map[x, y] != TileType.Stone) continue;
                    if (map[x, y] == TileType.Stone && component.Name != "Stone") continue;

                    int n = 0;
                    while (RandomProb(currentSeed + n, x, y) > component.Threshold && n < 10)
                    {
                        if (GameManager.Instance.CollectedWorldIds.Contains(objectCount))
                        {
                            ++objectCount;
                            ++n;
                            if (component.Name != "Water" && component.Name != "Stone")
                            {
                                break;
                            }
                            
                            continue;
                        }

                        float newX = realX + (RandomProb(currentSeed * 100 + x + y + n, x, y) - 0.5f) * 1.0f;
                        float newY = realY + (RandomProb(currentSeed * 200 + x + y + n, x, y) - 0.5f) * 1.0f;
                        Vector3 position = new Vector3(newX, newY, -0.25f);

                        GameObject go = Instantiate(Prefabs[(int)component.ComponentType], position, Quaternion.identity, RealComponentsContainer.transform);
                        var worldComponent = go.GetComponent<WorldComponent>();
                        worldComponent.WorldId = objectCount;
                        worldComponent.AlchemicComponent = component;
                        go.GetComponent<SpriteRenderer>().sprite = component.Sprite;
                        collectibles.Add(objectCount, go);
                        ++objectCount;
                        ++n;
                        if (component.Name != "Water" && component.Name != "Stone")
                        {
                            break;
                        }
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

    private void GenerateDrunkardPath(int seed, int totalSteps, int x, int y, float[] prob, TileType[,] map, int width, int height, TileType type = TileType.Sand, float nextProb = 0.85f)
    {
        int[] idx = new int[] { 0, 1, 2, 3 };
        int[] originalIdx = new int[4];
        idx.CopyTo(originalIdx, 0);

        float[] probUp = new float[] { nextProb, 0.0f, (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f };
        float[] probDown = new float[] { 0.0f, nextProb, (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f };
        float[] probRight = new float[] { (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f, nextProb, 0.0f };
        float[] probLeft = new float[] { (1.0f - nextProb) / 2.0f, (1.0f - nextProb) / 2.0f, 0.0f, nextProb };

        for (int i = 0; i < totalSteps; ++i)
        {
            map[x, y] = type;

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

                        probUp.CopyTo(prob, 0);
                        originalIdx.CopyTo(idx, 0);
                    }
                    else if (direction == 1)
                    {
                        y -= 1;

                        probDown.CopyTo(prob, 0);
                        originalIdx.CopyTo(idx, 0);
                    }
                    else if (direction == 2)
                    {
                        x += 1;

                        probRight.CopyTo(prob, 0);
                        originalIdx.CopyTo(idx, 0);
                    }
                    else if (direction == 3)
                    {
                        x -= 1;

                        probLeft.CopyTo(prob, 0);
                        originalIdx.CopyTo(idx, 0);
                    }

                    break;
                }
            }

        }
    }

    private void paintPaths(TileType[,] map, int width, int height)
    {
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                if (map[x, y] == TileType.Empty)
                {
                    continue;
                }

                var type = map[x, y];
                int typeIdx = (int)type;

                bool left = (x > 0) && map[x - 1, y] != TileType.Empty && (type != TileType.River || map[x - 1, y] == type);
                bool right = (x < width - 1) && map[x + 1, y] != TileType.Empty && (type != TileType.River || map[x + 1, y] == type);
                bool top = (y < height - 1) && map[x, y + 1] != TileType.Empty && (type != TileType.River || map[x, y + 1] == type);
                bool bottom = (y > 0) && map[x, y - 1] != TileType.Empty && (type != TileType.River || map[x, y - 1] == type);

                if (left && right && top && bottom)
                {
                    if (type == TileType.River) continue;
                    Instantiate(Junction4[typeIdx][RandomRange(x * height + y, 0, Junction4[typeIdx].Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.identity, RealPathsContainer.transform);
                }
                else if (left && right && top)
                {
                    if (type == TileType.River) continue;
                    Instantiate(JunctionT[typeIdx][RandomRange(x * height + y, 0, JunctionT[typeIdx].Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), RealPathsContainer.transform);
                }
                else if (left && right && bottom)
                {
                    if (type == TileType.River) continue;
                    Instantiate(JunctionT[typeIdx][RandomRange(x * height + y, 0, JunctionT[typeIdx].Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 270)), RealPathsContainer.transform);
                }
                else if (left && bottom && top)
                {
                    if (type == TileType.River) continue;
                    Instantiate(JunctionT[typeIdx][RandomRange(x * height + y, 0, JunctionT[typeIdx].Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 180)), RealPathsContainer.transform);
                }
                else if (top && right && bottom)
                {
                    if (type == TileType.River) continue;
                    Instantiate(JunctionT[typeIdx][RandomRange(x * height + y, 0, JunctionT[typeIdx].Count)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.identity, RealPathsContainer.transform);
                }
                else if (left && bottom)
                {
                    Instantiate(Turn[typeIdx][RandomRange(x * height + y, 0, Turn[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 270)), RealPathsContainer.transform);
                }
                else if (left && top)
                {
                    Instantiate(Turn[typeIdx][RandomRange(x * height + y, 0, Turn[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 180)), RealPathsContainer.transform);
                }
                else if (right && top)
                {
                    Instantiate(Turn[typeIdx][RandomRange(x * height + y, 0, Turn[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), RealPathsContainer.transform);
                }
                else if (right && bottom)
                {
                    Instantiate(Turn[typeIdx][RandomRange(x * height + y, 0, Turn[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 0)), RealPathsContainer.transform);
                }
                else if (left || right)
                {
                    Instantiate(Straight[typeIdx][RandomRange(x * height + y, 0, Straight[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX - 0.2f, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), RealPathsContainer.transform);
                    Instantiate(Straight[typeIdx][RandomRange(x * height + y, 0, Straight[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX + 0.2f, y * 0.8f + MinY, 0), Quaternion.Euler(new Vector3(0, 0, 90)), RealPathsContainer.transform);
                }
                else if (top || bottom)
                {
                    Instantiate(Straight[typeIdx][RandomRange(x * height + y, 0, Straight[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY - 0.2f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), RealPathsContainer.transform);
                    Instantiate(Straight[typeIdx][RandomRange(x * height + y, 0, Straight[typeIdx].Count - 1)], new Vector3(x * 0.8f + MinX, y * 0.8f + MinY + 0.2f, 0), Quaternion.Euler(new Vector3(0, 0, 0)), RealPathsContainer.transform);
                }

                bool topLeft = (y < height - 1) && (x > 0) && map[x - 1, y + 1] == map[x, y];
                bool bottomLeft = (y > 0) && (x > 0) && map[x - 1, y - 1] == map[x, y];
                bool topRight = (y < height - 1) && (x < width - 1) && map[x + 1, y + 1] == map[x, y];
                bool bottomRight = (y > 0) && (x < width - 1) && map[x + 1, y - 1] == map[x, y];

                // Filling
                if (top && topLeft && left)
                {
                    var center = new Vector2(x * 0.8f + MinX - 0.4f, y * 0.8f + MinY + 0.4f);
                    if (RandomProb(GameManager.Instance.Seed, x, y) >= 0.5f)
                    {
                        if (flowerSpots.Add(center))
                        {
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, RealPathsContainer.transform);
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
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, RealPathsContainer.transform);
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
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, RealPathsContainer.transform);
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
                            Instantiate(Filling[RandomRange(x * height + y, 0, Filling.Count - 1)], new Vector3(center.x, center.y, 0), Quaternion.identity, RealPathsContainer.transform);
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
                if (GameManager.Instance.CollectedWorldIds.Contains(objectCount))
                {
                    ++objectCount;
                    continue;
                }

                var x = (j - 2f) * FlowerXSpacing;
                var y = (i - 1.5f) * FlowerYSpacing;
                var nx = OpenSimplex2S.Noise2(i * 5 + j, center.x, center.y) * FlowerNudge;
                var ny = OpenSimplex2S.Noise2(i * 5 + j + 100, center.x, center.y) * FlowerNudge;

                GameObject go = Instantiate(Prefabs[(int)component.ComponentType], new Vector3(center.x + x + nx, center.y + y + ny, -0.1f), Quaternion.identity, RealComponentsContainer.transform);
                var worldComponent = go.GetComponent<WorldComponent>();
                worldComponent.WorldId = objectCount;
                worldComponent.AlchemicComponent = component;
                go.GetComponent<SpriteRenderer>().sprite = component.Sprite;

                collectibles.Add(objectCount, go);
                ++objectCount;
            }
        }
    }

    public void DestroyCollected(int id)
    {
        Destroy(collectibles[id]);
    }
}
