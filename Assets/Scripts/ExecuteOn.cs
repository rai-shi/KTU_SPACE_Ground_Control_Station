using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.IO;
using System.IO.Ports;
using TMPro;
using System.Threading;
using UnityEngine.Events;   
using SFB;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class ExecuteOn : MonoBehaviour
{
    private static List<Action> executeOnMainThread = new List<Action>();
    // Bu metot, ana thread üzerinde bir eylemi gerçekleştirmek için kullanılır.
    public void ExecuteOnMainThread(Action action)
    {
        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(action);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lock (executeOnMainThread)
        {
            foreach (var action in executeOnMainThread)
            {
                action();
            }
            executeOnMainThread.Clear();
        }
        
    }
}
