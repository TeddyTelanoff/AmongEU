using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int MaxPlayers = 10;
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
            return;
        }

        for (int i = 1; i <= MaxPlayers; i++)
        {
            Player player = CreatePlayer(i);
            player.gameObject.SetActive(false);
        }
    }

    public Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public GameObject playerPrefab;

    public Player CreatePlayer(int id)
    {
        GameObject player = Instantiate(playerPrefab);
        player.GetComponent<Player>().Id = id;
        return player.GetComponent<Player>();
    }
}
