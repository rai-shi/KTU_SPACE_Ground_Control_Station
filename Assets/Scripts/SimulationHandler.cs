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

// using System.Windows.Forms;

public class SimulationHandler : MonoBehaviour
{
    [SerializeField] GameObject serialobject;
    private Serial serial;

    [SerializeField] private TextMeshProUGUI simulationtext;
    
    string presureCSVFileName = "";
    List<string> presureData = new List<string>();

    // threading
    Thread simPThread;

    int counter;
   
    // sim verisi listener event, send butonu aktif edilir

    void Start()
    {
        serial = serialobject.GetComponent<Serial>(); 
    }

    public void SerialWrite(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            if (serial.stream.IsOpen)
            {
                serial.stream.Write(inputText);
                simulationtext.text = "sended: "+ inputText;
            }
        }
        
    }
    
    public void SelectCSV()
    {
        presureCSVFileName = "" ;
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", "", false, (string[] path) => { 
            if (path.Length > 0)
            {
                presureCSVFileName = path[0];
            }  });
        if (presureCSVFileName != "")
        {
            GetPresureData();
            simulationtext.text = "Pressure data is ready to transmit";
        }
        else
        {
            simulationtext.text = "CSV file is not selected.";
        }
    }

    void GetPresureData()
    {
        using (StreamReader reader = new StreamReader(presureCSVFileName))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.Contains("#") | line =="")
                {
                    continue;
                }
                else
                {
                    string[] cells = line.Split(',');
                    presureData.Add(cells[3]);
                    Debug.Log(cells[3]);
                }
            }
        }
    }
    void SIMPThreadFunction()
    {
        
        for (int i = 0; i < presureData.Count ; i++)
        {
            if (serial.stream.IsOpen)
            {
                string simpData = "CMD,2102,SIMP," + presureData[i] + " ";
                // serial.stream.Write(simpData);
                serial.stream.WriteLine(simpData);
                serial.stream.BaseStream.Flush();
                Debug.Log(simpData);
                
                Thread.Sleep(1000);
            }
                      
        }
    }

    public void SIMP()
    {
        simPThread = new Thread(SIMPThreadFunction);
        simPThread.IsBackground = true;
        simPThread.Start();
        // simPThread.StartBackgroundThread();
        simulationtext.text = "sendeding pressure...";
    }

    public void SIMACTIVATE()
    {
        SerialWrite("CMD,2102,SIM,ACTIVATE ");
    }

    public void SIMENABLE()
    {
        SerialWrite("CMD,2102,SIM,ENABLE ");
    }

    public void SIMDISABLE()
    {
        SerialWrite("CMD,2102,SIM,DISABLE ");
        if (simPThread != null && simPThread.IsAlive)
        {
            simPThread.Abort();
        }
    }
    void OnDestroy(){
        simPThread.Join();
    }
    public void OnApplicationQuit()
    {
        Debug.Log("SIMP Thread stopped...");
        simPThread.Abort();
    }

}
