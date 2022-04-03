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
    public List<JSONComponent> Inventory;
}

[System.Serializable]
public class JSONComponent
{

}

public class GameManager : MonoBehaviour
{
    public enum Status
    {
        Menu = 0,
        Playing = 1,
        Dead = 2
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

    public GameObject ETooltip;

    public Material OutlineMaterial;
    public Material NormalMaterial;

    public float StartPage = 2f;
    public GameObject StartUI;
    

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
        //time = 30f;
        EndScreen.SetActive(false);
        //status = Status.Menu;
        status = Status.Playing;

        menu = Menus.None;
        //OpenInventory();
        //OpenMixing();

        // Subscribe Inventory
        InventoryUI.GetComponent<Inventory>().Subscribe();
        ReceiptsUI.GetComponent<ReceiptsList>().Subscribe();
    }

    // Update is called once per frame
    void Update()
    {
        if (status == Status.Menu)
        {
            if (StartPage <= 0)
            {
                StartUI.SetActive(false);
                ProgressBar.SetActive(true);
                status = Status.Playing;
            }
            else
            {
                StartPage -= Time.deltaTime;
            }
        }
        if (status == Status.Playing)
        {
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
                Destroy(MainPlayer.gameObject);
                status = Status.Dead;
            }
            else
            {
                time -= Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
            CloseMixing();
        }
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

    public void ShowETooltip(Vector3 position, float z = 0.0f)
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
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(Application.persistentDataPath, "save.dat")))
        {
            outputFile.Write(JsonUtility.ToJson(new SaveGame()
            {
                Seed = 0
            }));
        }        
    }
}
