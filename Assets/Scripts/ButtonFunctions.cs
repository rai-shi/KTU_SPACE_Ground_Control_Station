using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine.Events;
using TMPro;

public class ButtonFunctions : MonoBehaviour
{  
    [SerializeField] GameObject serialobject;
    private Serial serial;

    [SerializeField] GameObject telemetryTabObject;
    [SerializeField] GameObject plotTabObject;
    [SerializeField] TextMeshProUGUI sendedCommandText;

    [SerializeField] GameObject cansatObject;
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] TextMeshProUGUI rawtelemetryText;
    [SerializeField] Image[] states;
    

    void Start()
    {
        TimeSpan currentTime = DateTime.Now.TimeOfDay; 

        // deneme
        int hours = currentTime.Hours;
        int minutes = currentTime.Minutes;
        int seconds = currentTime.Seconds;
        string date = hours.ToString() + "," + minutes.ToString() + "," + seconds.ToString();
        Debug.Log("date: "+ date);
        // deneme
        
        serial = serialobject.GetComponent<Serial>();
        plotTabObject.SetActive(true);
        telemetryTabObject.SetActive(false);
    }
    void Update()
    {
    }

    void SerialWrite(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            if (serial.stream.IsOpen)
            {
                serial.stream.Write(inputText);
                Debug.Log("sended: "+ inputText);
                sendedCommandText.text = "sended: "+ inputText;
            }
        }
    }

    public void CXON()
    {
       SerialWrite("CMD,2102,CX,ON ");
    }
    public void CXOFF()
    {
        SerialWrite("CMD,2102,CX,OFF ");
    }
    public void STUTCTIME()
    {
        TimeSpan currentTime = DateTime.Now.TimeOfDay; 

        int hours = currentTime.Hours;
        int minutes = currentTime.Minutes;
        int seconds = currentTime.Seconds;

        string date = hours.ToString() + "," + minutes.ToString() + "," + seconds.ToString();

        string command = "CMD,2102,ST,"+ date + " ";
        SerialWrite(command);
    }
     public void STGPS()
    {
        SerialWrite("CMD,2102,ST,GPS ");
    }
     public void CAL()
    {
        SerialWrite("CMD,2102,CAL ");
    }
     public void BCNON()
    {
        SerialWrite("CMD,2102,BCN,ON ");
    }
    public void BCNOFF()
    {
        SerialWrite("CMD,2102,BCN,OFF ");
    }
    public void AYR()
    {
        SerialWrite("CMD,2102,AYR,ON ");
    }
        public void AYROFF()
    {
        SerialWrite("CMD,2102,AYR,OFF ");
    }

    public void OpenFolder()
    {
        string folderPath = Application.dataPath;
        folderPath += "/CSVFile/";
        System.Diagnostics.Process.Start(folderPath);
    }

    public void CSVHandler()
    {
        string csvFilePath = Application.dataPath + "/CSVFile/" ;
        string csvFileName = csvFilePath + "Flight_2102.csv";
        string headers = "TEAM_ID,MISSION_TIME,PACKET_COUNT,MODE,STATE,HS_DEPLOYED,PC_DEPLOYED,ALTITUDE,AIR_SPEED,TEMPERATURE,VOLTAGE,PRESSURE,GPS_TIME,GPS_ALTITUDE,GPS_LATITUDE,GPS_LONGITUDE,GPS_SATS,TILT_X,TILT_Y,ROT_Z,CMD_ECHO";

        if (isCSVExist(csvFileName))
        {
            csvClear(csvFileName);
            csvAppendHeader(csvFileName, headers);
        }
        else
        {
            CreateCSVFile(csvFileName);
            csvAppendHeader(csvFileName, headers);
        }
    }
    bool isCSVExist(string filename)
    {
        if (File.Exists(filename))
        {
            return true;
        }
        else return false;
    }
    void csvClear(string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename, false))
        {
            sw.Flush(); 
            sw.Close(); 
        }
    }
    
    void csvAppendHeader(string filename, string headers)
    {
        using (StreamWriter sw = File.AppendText(filename))
        {
            sw.WriteLine(headers);
            sw.Close();
        }
    }

    public static void CreateCSVFile(string filePath)
    {
        // Dosya yoksa, yeni bir dosya oluştur
        using (FileStream fs = File.Create(filePath))
        {
            fs.Close();
        }
    }

    public void OpenTelemetryTab()
    {
        plotTabObject.SetActive(false);
        telemetryTabObject.SetActive(true);
    }
    public void OpenPlotTab()
    {
        plotTabObject.SetActive(true);
        telemetryTabObject.SetActive(false);
    }

    public void ClearScreen()
    {
        foreach (var text in texts)
        {
            string temptext = text.text;
            string[] temptextList = temptext.Split(":");
            text.text = temptextList[0] + ":";
        }
        rawtelemetryText.text = "";

        foreach (var state in states)
        {
            state.GetComponent<Image>().color = new Color32(56,56,56,255);
        }
        cansatObject.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void OpenDownloadPanel()
    {
        Debug.Log("Download Panel Opened...");
    }
    public void CloseDownloadPanel()
    {
        Debug.Log("Download Panel Closed...");
    }

    
    public void CloseGCS()
    {
        Debug.Log("Application Closed...");
        Application.Quit();
    }
}
