using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        time = 300f;
        //time = 5f;
        EndScreen.SetActive(false);
        status = Status.Playing;
        menu = Menus.Inventory | Menus.Mixing;
    }

    // Update is called once per frame
    void Update()
    {
        if (status == Status.Playing)
        {
            if (time <= 0)
            {
                EndScreen.SetActive(true);
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
    }

    public void OpenInventory()
    {
        InventoryUI.gameObject.SetActive(true);
        menu |= Menus.Inventory;
    }

    public void CloseInventory()
    {
        InventoryUI.gameObject.SetActive(false);
        menu &= ~Menus.Inventory;
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
    }

    public void CloseMixing()
    {
        MixingUI.gameObject.SetActive(false);
        menu &= ~Menus.Mixing;
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
}
