using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour
{
	public static List<Task> tasks = new List<Task>();

    private void Awake()
    {
        tasks.Add(this);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDestroy()
    {
        tasks.Remove(this);
    }
}