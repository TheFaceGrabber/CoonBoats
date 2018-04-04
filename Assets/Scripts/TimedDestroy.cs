using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float TimerLength = 5;
    // Use this for initialization
    void Start()
    {
        Invoke("Destroy", TimerLength);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }


}