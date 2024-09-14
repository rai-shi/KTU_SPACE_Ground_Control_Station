using System;
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
 
public class Serial : MonoBehaviour{

    // connection variable
    [SerializeField] private TextMeshProUGUI connectionText;
    [SerializeField] private TextMeshProUGUI notConnectedText;
    [SerializeField] private TextMeshProUGUI connClosedText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TMP_Dropdown comDropdown;
    List<string> comOptions; 

    // stream variable
    public SerialPort stream = new SerialPort();
    public string receivedString = "";

    // event variable
    public UnityEvent<string> onDataReceivedEvent = new UnityEvent<string>(); // Olay nesnesi

    // threading
    Thread readThread;
    private static List<Action> executeOnMainThread = new List<Action>();

    // csvFile 
    private string csvFileName;

    // Bu metot, ana thread üzerinde bir eylemi gerçekleştirmek için kullanılır.
    public static void ExecuteOnMainThread(Action action)
    {
        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(action);
        }
    }
   
    void Start() 
    { 
        connectionText.enabled = false;
        connClosedText.enabled = false;
        errorText.enabled = false;
        notConnectedText.enabled = true;
        notConnectedText.text = "Connect to Port";
    
        comOptions = SerialPort.GetPortNames().ToList();
        comDropdown.AddOptions(comOptions);

        csvFileName = Application.dataPath + "/CSVFile/Flight_2102.csv";
        // csvFileName = Directory.GetCurrentDirectory()+"/CSVFile/Flight_2102.csv" ;
    }
    // this function is connected with refresh available ports button 
    public void CheckPorts()
    {
        comOptions = SerialPort.GetPortNames().ToList(); 
        comDropdown.ClearOptions(); 
        comDropdown.AddOptions(comOptions); 
    }
    
    public void StartConnection()
    {
        // data_stream.DtrEnable = true;
        // data_stream.RtsEnable = true;
        connectionText.enabled = true;
        connClosedText.enabled = false;
        errorText.enabled = false;
        notConnectedText.enabled = false;

        string selectedPort;
        selectedPort = comDropdown.captionText.text;
        if (selectedPort == null || selectedPort == "" || selectedPort == " ")
        {
            errorText.enabled = true;
            errorText.text = "Port didn't selected!";
            return;
        }
        if (!stream.IsOpen)
        {
            stream.PortName = selectedPort;
            stream.BaudRate = 9600;
            stream.ReadTimeout = 60000000;
            stream.WriteTimeout = 60000000;
        }
        try
        {
            if (!stream.IsOpen)
            {
                stream.Open();
                stream.DiscardInBuffer();
                connectionText.text = "Connected";
                stream.Write("CMD,2102,BCN,OFF ");
                readThread = new Thread(ReadFromSerial);
                readThread.Start();
                
            }
        }
        catch (Exception e)
        {
            connectionText.enabled = false;
            errorText.enabled = true;
            errorText.text = "Error: " + e.Message; 
            Debug.LogError("Exception: " + e.ToString());
        }

    }
    
    public void StopConnection()
    {
        connectionText.enabled = false;
        connClosedText.enabled = true;
        errorText.enabled = false;
        notConnectedText.enabled = false;

        if (stream.IsOpen)
        {
            stream.Close();
            connClosedText.text = "Connection Closed";
            if (readThread != null && readThread.IsAlive)
            {
                readThread.Abort();
            }
        }
    }

    void ReadFromSerial()
    {
        while (stream.IsOpen)
        {
            string value = stream.ReadLine();
            // Debug.Log(value);   

            // send data to printFunctions script for printing data on the interface
            ExecuteOnMainThread(() => onDataReceivedEvent.Invoke(value));

            WriteToCSV(value);

            stream.BaseStream.Flush();
        }
        
    }

    void Update () 
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

    void WriteToCSV(string data)
    {
        // csv file writing
        using (StreamWriter sw = File.AppendText(csvFileName))
        {
            sw.WriteLine(data);
        }
    }

     void OnDestroy()
    {
        comDropdown.onValueChanged.RemoveAllListeners();

        if (stream != null && stream.IsOpen)
        {
            stream.Close();
        }
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(); 
        }
    }
    public void OnApplicationQuit()
    {
        Debug.Log("Serial Read Thread stopped...");
        readThread.Abort();
    }
}



