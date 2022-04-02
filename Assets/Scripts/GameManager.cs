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

    public static GameManager Instance;

    public Player MainPlayer;
    private float time;
    //public Canvas EndScreen;
    public GameObject EndScreen;
    public Status status;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        //time = 300f;
        time = 5f;
        EndScreen.SetActive(false);
        status = Status.Playing;
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
}
