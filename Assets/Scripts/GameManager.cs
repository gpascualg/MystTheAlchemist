using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Menus menu;

    public GameObject ProgressBar;
    public GameObject InventoryIcon;

    public Action<int> OnTimeChange;
    public Action OnInventoryOpened;
    public Action OnInventoryClosed;
    public Action OnMixingOpened;
    public Action OnMixingClosed;

    public Material OutlineMaterial;
    public Material NormalMaterial;

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
        status = Status.Playing;

        menu = Menus.None;
        //OpenInventory();
        //OpenMixing();

        // Subscribe Inventory
        InventoryUI.GetComponent<Inventory>().Subscribe();
    }

    // Update is called once per frame
    void Update()
    {
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
        menu |= Menus.Mixing;
        OnMixingOpened?.Invoke();
    }

    public void CloseMixing()
    {
        MixingUI.gameObject.SetActive(false);
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

    public float getTime()
    {
        return this.time;
    }
}
