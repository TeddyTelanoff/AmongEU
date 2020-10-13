using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour
{
    public PlayerController playerController;
    public int Id;

    private void Awake()
    {
        GameManager.Instance.Players.Add(this);

        playerController = GetComponent<PlayerController>();
    }

    private void OnDestroy()
    {
        GameManager.Instance.Players.Remove(this);
    }
}
