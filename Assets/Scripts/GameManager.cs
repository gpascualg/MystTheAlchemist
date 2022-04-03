using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SaveGame
{
    public int Seed;
    public List<JSONReceipt> Receipts;
    public List<JSONItem> Inventory;
    public List<int> CollectedWorldIds;
    public Vector2 PlayerPosition;
}


public class GameManager : MonoBehaviour
{
    public enum Status
    {
        Start = 0,
        Menu = 1,
        Playing = 2,
        Dead = 3
    }

    public enum Menus
    {
        None = 0,
        Inventory = 1,
        Mixing = 2,
    }

    public static GameManager Instance;

    public Player MainPlayer;
    private float time;
    public GameObject EndScreen;
    public Status status;

    public GameObject InventoryUI;
    public GameObject MixingUI;
    public GameObject ReceiptsUI;
    private Menus menu;

    public GameObject ProgressBar;
    public GameObject InventoryIcon;

    public Action<int> OnTimeChange;
    public Action OnInventoryOpened;
    public Action OnInventoryClosed;
    public Action OnMixingOpened;
    public Action OnMixingClosed;
    public Action BeforeLoadGame;
    public Action AfterLoadGame;

    public GameObject ETooltip;

    public Material OutlineMaterial;
    public Material NormalMaterial;

    public int Seed;

    public float StartPage = 2f;
    public GameObject StartUI;
    
    public GameObject MenuUI;
    public GameObject NewGameButton;
    public GameObject ContinueUI;

    public bool GameStarted = false;
    public bool GamePaused = false;

    public GameObject WelcomeText1;
    public GameObject WelcomeText2;
    public GameObject WelcomeDialog;
    public int WelcomeStep = 1;
    public float WelcomeTime = 5f;

    public GameObject Instructions;
    public float TimeInstructions = 10f;

    public List<int> CollectedWorldIds = new List<int>();


    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        time = 300f;
        OnTimeChange?.Invoke((int)time);
        //time = 10f;
        EndScreen.SetActive(false);
        status = Status.Start;
        //status = Status.Playing;

        menu = Menus.None;
        //OpenInventory();
        //OpenMixing();

        // Subscribe Inventory
        InventoryUI.GetComponent<Inventory>().Subscribe();
        MixingUI.GetComponent<Mixing>().Subscribe();
        ReceiptsUI.GetComponent<ReceiptsList>().Subscribe();
    }

    private void OnEnable()
    {
        MainPlayer.OnItemCollected += OnItemCollected;
    }

    private void OnItemCollected(int id)
    {
        CollectedWorldIds.Add(id);
    }

    // Update is called once per frame
    void Update()
    {
        if (status == Status.Start)
        {
            if (StartPage <= 0)
            {
                StartUI.SetActive(false);
                MenuUI.SetActive(true);
                status = Status.Menu;
                if(SavedGame() || GamePaused)
                {
                    NewGameButton.SetActive(false);
                    ContinueUI.SetActive(true);
                }
                else
                {
                    NewGameButton.SetActive(true);
                    ContinueUI.SetActive(false);
                }
            }
            else
            {
                StartPage -= Time.deltaTime;
            }
        }

        if (status == Status.Playing)
        {
            if(WelcomeStep < 3)
            {
                if(WelcomeTime <= 0)
                {
                    if(WelcomeStep == 1)
                    {
                        WelcomeTime = 10f;
                        WelcomeText1.SetActive(false);
                        WelcomeText2.SetActive(true);
                        WelcomeStep = 2;
                    } else if(WelcomeStep == 2)
                    {
                        WelcomeText2.SetActive(false);
                        WelcomeStep = 3;
                        WelcomeDialog.SetActive(false);
                    }
                }
                else
                {
                    WelcomeTime -= Time.deltaTime;
                }
            }
            
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (IsInventoryOpen())
                {
                    CloseInventory();
                }
                else
                {
                    OpenInventory();
                }
            }

            if (time <= 0)
            {
                EndScreen.SetActive(true);
                ProgressBar.SetActive(false);
                InventoryIcon.SetActive(false);
                InventoryIcon.SetActive(false);
                //Destroy(MainPlayer.gameObject);
                status = Status.Dead;
            }
            else
            {
                time -= Time.deltaTime;
            }

            if(TimeInstructions <= 0)
            {
                Instructions.SetActive(false);
            }
            else
            {
                TimeInstructions -= Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsInventoryOpen() || IsMixingOpen())
            {
                CloseInventory();
                CloseMixing();
            }
            else
            {
                if (GamePaused)
                {
                    ProgressBar.SetActive(true);
                    InventoryIcon.SetActive(true);
                    MenuUI.SetActive(false);
                    status = Status.Playing;
                    GamePaused = false;
                    NewGameButton.SetActive(false);
                    ContinueUI.SetActive(true);
                }
                else
                {
                    ProgressBar.SetActive(false);
                    InventoryIcon.SetActive(false);
                    MenuUI.SetActive(true);
                    status = Status.Menu;
                    GamePaused = true;
                }
            }
        }
    }

    public void NewGame()
    {
        ProgressBar.SetActive(true);
        InventoryIcon.SetActive(true);
        MenuUI.SetActive(false);
        status = Status.Playing;
        GameStarted = true;
        NewGameButton.SetActive(false);
        ContinueUI.SetActive(true);
    }

    public void Continue()
    {
        if (!GameStarted)
        {
            //LoadGame();

            GameStarted = true;
        }

        MenuUI.SetActive(false);
        status = Status.Playing;
        ProgressBar.SetActive(true);
        InventoryIcon.SetActive(true);
        GamePaused = false;
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        time = 300f;
        OnTimeChange?.Invoke((int)time);

        ProgressBar.SetActive(true);
        InventoryIcon.SetActive(true);
        EndScreen.SetActive(false);

        status = Status.Playing;
        GameStarted = true;

        MainPlayer.transform.position = Vector3.zero;
    }

    public void RestoreSeconds(int seconds)
    {
        time += seconds;
        OnTimeChange?.Invoke(seconds);
    }

    public void OpenInventory()
    {
        InventoryUI.gameObject.SetActive(true);
        menu |= Menus.Inventory;
        OnInventoryOpened?.Invoke();
    }

    public void CloseInventory()
    {
        InventoryUI.gameObject.SetActive(false);
        menu &= ~Menus.Inventory;
        OnInventoryClosed?.Invoke();
    }

    public bool IsInventoryOpen()
    {
        return (menu & Menus.Inventory) > 0;
    }

    public void ToggleInventory()
    {
        if (IsInventoryOpen())
        {
            OpenInventory();
        }
        else
        {
            CloseInventory();
        }
    }

    public void OpenMixing()
    {
        MixingUI.gameObject.SetActive(true);
        ReceiptsUI.gameObject.SetActive(true);
        menu |= Menus.Mixing;
        OnMixingOpened?.Invoke();
    }

    public void CloseMixing()
    {
        MixingUI.gameObject.SetActive(false);
        ReceiptsUI.gameObject.SetActive(false);
        menu &= ~Menus.Mixing;
        OnMixingClosed?.Invoke();
    }

    public bool IsMixingOpen()
    {
        return (menu & Menus.Mixing) > 0;
    }

    public void ToggleMixing()
    {
        if (IsMixingOpen())
        {
            OpenMixing();
        }
        else
        {
            CloseMixing();
        }
    }

    public void ShowETooltip(Vector3 position)
    {
        ShowETooltip(position, position.z);
    }

    public void ShowETooltip(Vector3 position, float z)
    {
        position.x += 0.1f;
        position.y += 0.1f;
        position.z = z - 0.01f;
        ETooltip.transform.position = position;
        ETooltip.SetActive(true);
    }

    public void HideETooltip()
    {
        ETooltip.SetActive(false);
    }

    public float getTime()
    {
        return this.time;
    }

    public void SaveGame()
    {
        var path = Path.Combine(Application.persistentDataPath, "player.dat");
        using (StreamWriter outputFile = new StreamWriter(path))
        {
            outputFile.Write(JsonUtility.ToJson(new SaveGame()
            {
                Seed = Seed,
                Receipts = MainPlayer.LearnedReceipts.ConvertAll(new Converter<ReceiptComponents, JSONReceipt>(ReceiptComponents.Serializer)),
                Inventory = MainPlayer.SerializeInventory(),
                CollectedWorldIds = CollectedWorldIds,
                PlayerPosition = MainPlayer.transform.position
            }));
        }
    }

    public bool LoadGame()
    {
        var path = Path.Combine(Application.persistentDataPath, "player.dat");
        if (!File.Exists(path))
        {
            return false;
        }

        BeforeLoadGame?.Invoke();
        using (StreamReader inputFile = new StreamReader(path))
        {
            SaveGame data = JsonUtility.FromJson<SaveGame>(inputFile.ReadToEnd());
            Seed = data.Seed - 1000;
            MainPlayer.Deserialize(data.Receipts);
            MainPlayer.Deserialize(data.Inventory);
            CollectedWorldIds = data.CollectedWorldIds;

            GenerateMap.Instance.GenerateAll();
            foreach (var id in CollectedWorldIds)
            {
                GenerateMap.Instance.DestroyCollected(id);
            }

            MainPlayer.transform.position = data.PlayerPosition;
        }
        AfterLoadGame?.Invoke();

        return true;
    }
    public void LoadGameUnconditional()
    {
        LoadGame();
    }

    public bool SavedGame()
    {
        return false;
    }
}
