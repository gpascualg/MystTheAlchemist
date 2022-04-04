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
    public float TimeLeft;
    public float MaxTime;
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
    public Action<float> OnTimeReset;
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
    public float WelcomeTime = 10f;

    public GameObject Instructions;
    public float TimeInstructions = 10f;

    public Sprite DubiousSprite;
    public AudioSource PickupSound;
    public AudioSource CreateSound;
    public AudioSource CureSound;
    public AudioSource PoisonSound;
    public AudioSource ErrorSound;

    public Image BGSound;
    public Sprite BGSoundOn;
    public Sprite BGSoundOff;
    private bool isBGSoundOn;

    public Image FXSound;
    public Sprite FXSoundOn;
    public Sprite FXSoundOff;
    private bool isFXSoundOn;

    public List<int> CollectedWorldIds = new List<int>();

    public const float INITIAL_TIME = 300.0f;

    private int numButterflies = 0;
    public GameObject ButterflyPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        time = 0.0f;
        OnTimeReset?.Invoke(INITIAL_TIME);
        OnTimeChange?.Invoke((int)INITIAL_TIME);
        time = INITIAL_TIME;
        EndScreen.SetActive(false);

        menu = Menus.None;
        status = Status.Start;
        //status = Status.Playing;

#if !UNITY_EDITOR
        status = Status.Start;
#endif

        if (status == Status.Start)
        {
            StartUI.SetActive(true);
        }

        // Subscribe Inventory
        InventoryUI.GetComponent<Inventory>().Subscribe();
        MixingUI.GetComponent<Mixing>().Subscribe();
        ReceiptsUI.GetComponent<ReceiptsList>().Subscribe();
        ProgressBar.GetComponentInChildren<LifeProgressBar>().Subscribe();

        // Sound
        isBGSoundOn = !(!PlayerPrefs.HasKey("BGSound") || PlayerPrefs.GetInt("BGSound") == 1);
        isFXSoundOn = !(!PlayerPrefs.HasKey("FXSound") || PlayerPrefs.GetInt("FXSound") == 1);
        ToggleBGSound();
        ToggleFXSound();
    }

    private void OnEnable()
    {
        MainPlayer.OnItemCollected += OnItemCollected;
    }

    private void OnItemCollected(int id)
    {
        CollectedWorldIds.Add(id);
    }

    public void ToggleBGSound()
    {
        if (isBGSoundOn)
        {
            BGSound.sprite = BGSoundOff;
            isBGSoundOn = false;
            Camera.main.GetComponent<AudioSource>().Stop();
        }
        else
        {
            BGSound.sprite = BGSoundOn;
            isBGSoundOn = true;
            Camera.main.GetComponent<AudioSource>().Play();
        }

        PlayerPrefs.SetInt("BGSound", isBGSoundOn.ToInt());
    }

    public void ToggleFXSound()
    {
        if (isFXSoundOn)
        {
            FXSound.sprite = FXSoundOff;
            isFXSoundOn = false;
        }
        else
        {
            FXSound.sprite = FXSoundOn;
            isFXSoundOn = true;
        }

        PlayerPrefs.SetInt("FXSound", isBGSoundOn.ToInt());
    }

    public void PlayFX(AudioSource source)
    {
        if (!isFXSoundOn)
        {
            return;
        }

        source.Play();
    }

    public void SpawnButterfly(Vector3 position)
    {
        if (numButterflies < 25)
        {
            numButterflies++;
            position.z = -0.45f;
            Instantiate(ButterflyPrefab, position, Quaternion.identity);
        }
    }

    public void DespawnButterfly(GameObject butterfly)
    {
        --numButterflies;
        Destroy(butterfly);
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
                if (HasSavedGame() || GamePaused)
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
                BGSound.gameObject.SetActive(true);
                FXSound.gameObject.SetActive(true);
                CloseInventory();
                CloseMixing();
                MainPlayer.ItemsInventory.Clear();
                Inventory.Instance.CleanInterface();
                status = Status.Dead;
                SaveGame();
            }
            else
            {
                time -= Time.deltaTime;
            }

            if (TimeInstructions <= 0)
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
                    BGSound.gameObject.SetActive(false);
                    FXSound.gameObject.SetActive(false);
                }
                else
                {
                    ProgressBar.SetActive(false);
                    InventoryIcon.SetActive(false);
                    MenuUI.SetActive(true);
                    BGSound.gameObject.SetActive(true);
                    FXSound.gameObject.SetActive(true);
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
        BGSound.gameObject.SetActive(false);
        FXSound.gameObject.SetActive(false);
        MenuUI.SetActive(false);
        status = Status.Playing;
        GameStarted = true;
        NewGameButton.SetActive(false);
        ContinueUI.SetActive(true);

        CollectedWorldIds.Clear();
        GenerateMap.Instance.GenerateAll();
        MainPlayer.transform.position = new Vector3(0, 0, -0.5f);

        SaveGame();
    }

    public void Continue()
    {
        if (!GameStarted)
        {
            LoadGame();
            GameStarted = true;
        }

        MenuUI.SetActive(false);
        status = Status.Playing;
        ProgressBar.SetActive(true);
        InventoryIcon.SetActive(true);
        BGSound.gameObject.SetActive(false);
        FXSound.gameObject.SetActive(false);
        GamePaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        time = 0.0f;
        OnTimeReset?.Invoke(INITIAL_TIME);
        OnTimeChange?.Invoke((int)INITIAL_TIME);
        time = INITIAL_TIME;

        ProgressBar.SetActive(true);
        InventoryIcon.SetActive(true);
        BGSound.gameObject.SetActive(false);
        FXSound.gameObject.SetActive(false);
        EndScreen.SetActive(false);

        status = Status.Playing;
        GameStarted = true;
        CollectedWorldIds.Clear();
        GenerateMap.Instance.GenerateAll();

        MainPlayer.transform.position = new Vector3(0, 0, -0.5f);

        SaveGame();
    }

    public void RestoreSeconds(int seconds)
    {
        if (seconds > 0)
        {
            GameManager.Instance.PlayFX(GameManager.Instance.CureSound);
        }
        else
        {
            GameManager.Instance.PlayFX(GameManager.Instance.PoisonSound);
        }

        OnTimeChange?.Invoke(seconds);
        time += seconds;
    }

    public void OpenInventory()
    {
        InventoryUI.gameObject.SetActive(true);
        ReceiptsUI.gameObject.SetActive(true);
        menu |= Menus.Inventory;
        OnInventoryOpened?.Invoke();
    }

    public void CloseInventory()
    {
        InventoryUI.gameObject.SetActive(false);
        ReceiptsUI.gameObject.SetActive(false);
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
                Inventory = MainPlayer.SerializeItemsInventory(),
                CollectedWorldIds = CollectedWorldIds,
                PlayerPosition = MainPlayer.transform.position,
                TimeLeft = time,
                MaxTime = LifeProgressBar.Instance.MaxTime
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
            //foreach (var id in CollectedWorldIds)
            //{
            //    GenerateMap.Instance.DestroyCollected(id);
            //}

            MainPlayer.transform.position = MainPlayer.transform.position = new Vector3(data.PlayerPosition.x, data.PlayerPosition.y, -0.5f);
            time = 0.0f;
            OnTimeReset?.Invoke(data.MaxTime);
            OnTimeChange?.Invoke((int)data.TimeLeft);
            time = data.TimeLeft;
        }
        AfterLoadGame?.Invoke();

        return true;
    }
    public void LoadGameUnconditional()
    {
        LoadGame();
    }

    public bool HasSavedGame()
    {
        var path = Path.Combine(Application.persistentDataPath, "player.dat");
        return File.Exists(path);
    }
}
