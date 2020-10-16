using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public Dictionary<int, int> input;

    private void Awake()
    {
        input = new Dictionary<int, int>();
        for (int i = 0; i < (int)PlayerInput.Last; i++)
            input.Add(i, (int)ButtonMode.None);
    }

    public void FixedUpdate()
    {
        input[(int)PlayerInput.Left] = Input.GetKeyDown(KeyCode.A) ? (int)ButtonMode.Pressed :
            Input.GetKey(KeyCode.A) ? (int)ButtonMode.Held :
            Input.GetKeyUp(KeyCode.A) ? (int)ButtonMode.Release :
            (int)ButtonMode.None;
        input[(int)PlayerInput.Right] = Input.GetKeyDown(KeyCode.D) ? (int)ButtonMode.Pressed :
            Input.GetKey(KeyCode.D) ? (int)ButtonMode.Held :
            Input.GetKeyUp(KeyCode.D) ? (int)ButtonMode.Release :
            (int)ButtonMode.None;
        input[(int)PlayerInput.Forward] = Input.GetKeyDown(KeyCode.W) ? (int)ButtonMode.Pressed :
            Input.GetKey(KeyCode.W) ? (int)ButtonMode.Held :
            Input.GetKeyUp(KeyCode.W) ? (int)ButtonMode.Release :
            (int)ButtonMode.None;
        input[(int)PlayerInput.Back] = Input.GetKeyDown(KeyCode.S) ? (int)ButtonMode.Pressed :
            Input.GetKey(KeyCode.S) ? (int)ButtonMode.Held :
            Input.GetKeyUp(KeyCode.S) ? (int)ButtonMode.Release :
            (int)ButtonMode.None;

        ClientSend.SendInput(input);
    }
}

enum ButtonMode
{
    None,
    Pressed,
    Held,
    Release,

    Last = Release
}

enum PlayerInput
{
    Left,
    Right,
    Forward,
    Back,

    Last = Back
}