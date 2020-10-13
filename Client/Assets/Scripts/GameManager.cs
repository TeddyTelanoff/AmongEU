using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public List<Player> Players = new List<Player>();
    public GameObject playerPrefab;

    public void CreatePlayer(int id)
    {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<Player>().Id = id;
    }

    public void DestroyPlayer(int index)
    {
        Destroy(Players[index].gameObject);
    }
}
