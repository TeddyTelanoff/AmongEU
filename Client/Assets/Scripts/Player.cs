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
        playerController = GetComponent<PlayerController>();
    }
}
