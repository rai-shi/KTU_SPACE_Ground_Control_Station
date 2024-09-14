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
using Random = UnityEngine.Random;

public class PrintFunctions : MonoBehaviour
{
    int dataLength;
    string teamID;
    string missionTime;
    string packetCount;
    string mode;
    string state;
    string altitude;
    float Faltitude;
    string speed;
    float Fspeed;
    string heatshield;
    string parachute;
    string temperature;
    float Ftemperature;
    string presure;
    float Fpresure;
    string voltage;
    float Fvoltage;
    string gps_time;
    string gps_alti;
    float Fgpsalti;
    string gps_lati;
    string gps_long;
    string gps_sat;
    string tilt_x;
    string tilt_y;
    string rot_z;
    string cmdecho;

    [SerializeField] private Image isStreaming;
    bool isCoorect = true;

    // for listening if data is received
    [SerializeField] GameObject serialobject;
    private Serial serial;

    // cansat object
    [SerializeField] GameObject cansat;

    [SerializeField] private TextMeshProUGUI teamIDText;
    [SerializeField] private TextMeshProUGUI missionTimeText;
    [SerializeField] private TextMeshProUGUI packetCountText;

    // heatshield data
    [SerializeField] private TextMeshProUGUI heatshieldP;
    [SerializeField] private TextMeshProUGUI heatshieldN;

    // parachute data
    [SerializeField] private TextMeshProUGUI parachuteC;
    [SerializeField] private TextMeshProUGUI parachuteN;

    [SerializeField] private TextMeshProUGUI cmdEchoText;
    [SerializeField] private TextMeshProUGUI tiltXText;
    [SerializeField] private TextMeshProUGUI tiltYText;
    [SerializeField] private TextMeshProUGUI rotZText;
    [SerializeField] private TextMeshProUGUI gpsAltiText;
    [SerializeField] private TextMeshProUGUI gpsLatiText;
    [SerializeField] private TextMeshProUGUI gpsLongText;
    [SerializeField] private TextMeshProUGUI gpsSatText;
    [SerializeField] private TextMeshProUGUI gpsTimeText;

    // plot legend text data
    [SerializeField] private TextMeshProUGUI plotAltitude;
    [SerializeField] private TextMeshProUGUI plotGpsAltitude;
    [SerializeField] private TextMeshProUGUI plotAirSpeed;
    [SerializeField] private TextMeshProUGUI plotTemperature;
    [SerializeField] private TextMeshProUGUI plotPresure;
    [SerializeField] private TextMeshProUGUI plotVoltage;

    [SerializeField] private TextMeshProUGUI rawTelemetryText;
    
    // state data
    [SerializeField] private Image launchWait;
    [SerializeField] private Image ascent;
    [SerializeField] private Image rocketSeperation;
    [SerializeField] private Image descent;
    [SerializeField] private Image heatShieldReleased;
    [SerializeField] private Image landed;

    // mode indicator
    [SerializeField] private GameObject simMode;
    [SerializeField] private GameObject flightMode;

    [SerializeField] private Button activateButton;
    [SerializeField] private Button simpButton;
    [SerializeField] private Button deactivateButton;

    // graph event objects
    public UnityEvent<float> onAltitudeDataReadyEvent = new UnityEvent<float>(); 
    public UnityEvent<float> onSpeedDataReadyEvent = new UnityEvent<float>(); 
    public UnityEvent<float> onTemperatureDataReadyEvent = new UnityEvent<float>(); 
    public UnityEvent<float> onPresureDataReadyEvent = new UnityEvent<float>();
    public UnityEvent<float> onVoltageDataReadyEvent = new UnityEvent<float>(); 
    public UnityEvent<float> onGpsAltitudeDataReadyEvent = new UnityEvent<float>(); 

    // tablo event object
    public UnityEvent<List<string>> onTableDataReceivedEvent = new UnityEvent<List<string>>(); 

    // map event object
    public UnityEvent<float, float> onMapDataReceivedEvent = new UnityEvent<float, float>();


    // threading
    private static List<Action> executeOnMainThread = new List<Action>();

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
        isStreaming.GetComponent<Image>().color = new Color32(255,0,0,255);

        heatshieldP.enabled = false;
        heatshieldN.enabled = false;
        parachuteC.enabled = false;
        parachuteN.enabled = false;

        flightMode.SetActive(true);
        simMode.SetActive(false);

        launchWait.GetComponent<Image>().color = new Color32(56,56,56,255);
        ascent.GetComponent<Image>().color = new Color32(56,56,56,255);
        rocketSeperation.GetComponent<Image>().color = new Color32(56,56,56,255);
        descent.GetComponent<Image>().color = new Color32(56,56,56,255);
        heatShieldReleased.GetComponent<Image>().color = new Color32(56,56,56,255);
        landed.GetComponent<Image>().color = new Color32(56,56,56,255);

        // activateButton.interactable = false;
        // simpButton.interactable = false;
        // deactivateButton.interactable = false;

        serial = serialobject.GetComponent<Serial>();
        serial.onDataReceivedEvent.AddListener(OnDataReceived); 
    }

    // verinin serial.cs den yakalandığı yer
    void OnDataReceived(string data)
    {
        // ExecuteOnMainThread(() =>{ rawTelemetryText.text = data; }); // all telemetry yazdırma kısmı
        rawTelemetryText.text = data; // all telemetry yazdırma kısmı
        
        string[] parsedData = ParseData(data); // verilerin parse edilme kısmı
        if (parsedData.Length == 0)
        {
            isStreaming.GetComponent<Image>().color = new Color32(255,0,0,255); // red
            return;
        }
        else if (parsedData.Length < 21)
        {
            isStreaming.GetComponent<Image>().color = new Color32(255,210,0,255); // yellow
            return;
        }
        else if (parsedData.Length >= 25 )
        {
            isStreaming.GetComponent<Image>().color = new Color32(255,210,0,255); // yellow
            return;
        }
        else{
            isStreaming.GetComponent<Image>().color = new Color32(0,255,0,255); // green
            isCoorect = CheckData(parsedData);
            if (isCoorect==true)
            {
                ProcessTelemetry(); // verilerin işlenme kısmı
            }
            else
            {
                isStreaming.GetComponent<Image>().color = new Color32(255,210,0,255); // yellow
                return; 
            }
            
        }
    }

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

    string[] ParseData(string rawTelemetry)
    {
        string[] parsedData = rawTelemetry.Split(",");
        return parsedData;
    }

    void PrintJustLabelData(string teamID, string missionTime, 
                            string packetCount, string cmdEcho, 
                            string tiltX, string tiltY, string rotZ,
                            string gpsAlti, string gpsLati, string gpsLong, 
                            string gpsSat, string gpsTime)
    {
        // ExecuteOnMainThread(() =>{
            
            teamIDText.text = "TEAM ID: " + teamID;
            missionTimeText.text = "MISSION TIME: " + missionTime;
            packetCountText.text = "PACKET COUNT: " + packetCount;
            cmdEchoText.text = "CMD ECHO:" + cmdEcho;
            tiltXText.text = "TILT X:" + tiltX + "°";
            tiltYText.text = "TILT Y:" + tiltY + "°";
            rotZText.text = "ROT Z:" + rotZ + "°";
            gpsAltiText.text = "GPS ALT:" + gpsAlti + "m";
            gpsLatiText.text = "GPS LAT:" + gpsLati + "°";
            gpsLongText.text = "GPS LONG:" + gpsLong + "°";
            gpsSatText.text = "GPS SATS:" + gpsSat;
            gpsTimeText.text = "GPS TIME:" + gpsTime;
        // });
        
    }

    void ParachuteAndHeatShieldData(string parachute, string heatshield)
    {
        if (parachute == "C")
        {
            // ExecuteOnMainThread(() =>{
                parachuteC.enabled = true;
                parachuteN.enabled = false;
            // });
        }
        else if (parachute == "N")
        {
            // ExecuteOnMainThread(() =>{
                parachuteC.enabled = false;
                parachuteN.enabled = true;
            // });
        }

        if (heatshield == "P")
        {
            // ExecuteOnMainThread(() =>{
                heatshieldP.enabled = true;
                heatshieldN.enabled = false;
            // });
        }
        else if (heatshield == "N")
        {
            // ExecuteOnMainThread(() =>{
                heatshieldP.enabled = false;
                heatshieldN.enabled = true;
            // });
        }
    }

    void PrintStateData(string state)
    {
        if (state == "AC")
        {
            // ExecuteOnMainThread(() =>{
                ascent.GetComponent<Image>().color = new Color32(29,231,29,255);
            // });
        }
        else if (state == "DC")
        {
            // ExecuteOnMainThread(() =>{
                descent.GetComponent<Image>().color = new Color32(29,231,29,255);
            // });
        }
        else if (state == "HR")
        {
            // ExecuteOnMainThread(() =>{
                heatShieldReleased.GetComponent<Image>().color = new Color32(29,231,29,255);
            // });
        }
        else if (state == "LD")
        {
            // ExecuteOnMainThread(() =>{
                landed.GetComponent<Image>().color = new Color32(29,231,29,255);
            // });
        }
        else if (state == "LW")
        {
            // ExecuteOnMainThread(() =>{
                launchWait.GetComponent<Image>().color = new Color32(29,231,29,255);
            // });
        }
        else if (state == "RS")
        {
            // ExecuteOnMainThread(() =>{
                rocketSeperation.GetComponent<Image>().color = new Color32(29,231,29,255);
            // });
        }
    }


    void RotateCansat(string tiltX, string tiltY, string rotZ)
    {
        float tiltXFloat = float.Parse(tiltX, System.Globalization.CultureInfo.InvariantCulture);
        float tiltYFloat = float.Parse(tiltY, System.Globalization.CultureInfo.InvariantCulture);
        float rotZFloat = (-1)*float.Parse(rotZ, System.Globalization.CultureInfo.InvariantCulture); 

        cansat.transform.rotation = Quaternion.Euler(tiltYFloat, rotZFloat,tiltXFloat);
    }

    void ModeIndicator(string mode)
    {
        if (mode=="S")
        {
            flightMode.SetActive(false);
            simMode.SetActive(true);
        } 
        else if (mode=="F")
        {
            flightMode.SetActive(true);
            simMode.SetActive(false);
        }
    }

    void PlotTextData(string altitude, string gpsAltitude, string airSpeed, string temperature, string presure, string voltage)
    {
        plotAltitude.text = altitude;
        plotGpsAltitude.text = gpsAltitude;
        plotAirSpeed.text = airSpeed;
        plotTemperature.text = temperature;
        plotPresure.text = presure;
        plotVoltage.text = voltage;
    }

    bool CheckData(string[] data)
    {
        dataLength = data.Length;

        if(data[0]=="2102")
        {
            teamID = data[0];
        }
        else
        {
            return false;
        }

        missionTime = data[1];
        packetCount = data[2];
        mode = data[3];
        state = data[4];
        altitude = data[5];
        speed = data[6];
        heatshield = data[7];
        parachute = data[8];
        temperature = data[9];
        presure = data[10];
        voltage = data[11];
        gps_time = data[12];
        gps_alti = data[13];
        gps_lati = data[14];
        gps_long = data[15];
        gps_sat = data[16];
        tilt_x = data[17];
        tilt_y = data[18];
        rot_z = data[19];
        string[] cmdechoArray = new string[data.Length - 20];
        Array.Copy(data, 20, cmdechoArray, 0, data.Length - 20);
        cmdecho = string.Join(",", cmdechoArray);

        try
        {
            Faltitude = float.Parse(altitude, System.Globalization.CultureInfo.InvariantCulture);
            Fspeed = float.Parse(speed, System.Globalization.CultureInfo.InvariantCulture);
            Ftemperature = float.Parse(temperature, System.Globalization.CultureInfo.InvariantCulture);
            Fpresure = float.Parse(presure, System.Globalization.CultureInfo.InvariantCulture);
            Fvoltage = float.Parse(voltage, System.Globalization.CultureInfo.InvariantCulture);
            Fgpsalti = float.Parse(gps_alti, System.Globalization.CultureInfo.InvariantCulture);
            if ((Faltitude < 0.0f) & (mode == "S"))
            {
                float randomValue = Random.Range(0.0f, 0.2f);
                Faltitude = randomValue;
                altitude = randomValue.ToString();
            }
            // if ((Fspeed < 0.0f) | (Fspeed == -0.0f))
            // {
            //     Debug.Log("speed");
            //     Debug.Log(Fspeed);
            //     Fspeed = 0.0f;
            //     speed = "0.0";
            // }
        }
        catch
        {
            return false;
        }

        return true;
    }

    void ProcessTelemetry()
    {
        // get data one by one
        // int dataLength = data.Length;
        // string teamID = data[0];
        // string missionTime = data[1];
        // string packetCount = data[2];
        // string mode = data[3];
        // string state = data[4];
        // string altitude = data[5];
        // string speed = data[6];
        // string heatshield = data[7];
        // string parachute = data[8];
        // string temperature = data[9];
        // string presure = data[10];
        // string voltage = data[11];
        // string gps_time = data[12];
        // string gps_alti = data[13];
        // string gps_lati = data[14];
        // string gps_long = data[15];
        // string gps_sat = data[16];
        // string tilt_x = data[17];
        // string tilt_y = data[18];
        // string rot_z = data[19];
        // string[] cmdechoArray = new string[data.Length - 20];
        // Array.Copy(data, 20, cmdechoArray, 0, data.Length - 20);
        // string cmdecho = string.Join(",", cmdechoArray);

        // check if echo has simulation commands
        // if (cmdechoArray.Last() == "SIMENABLE")
        // {
        //     activateButton.interactable = true;
        // }
        // else if (cmdechoArray.Last() == "SIMACTIVATE")
        // {
        //     simpButton.interactable = true;
        //     deactivateButton.interactable = true;
        //     ModeIndicator(mode);
        // }
        // else if (cmdechoArray.Last() == "SIMDISABLE")
        // {
        //     activateButton.interactable = false;
        //     simpButton.interactable = false;
        //     deactivateButton.interactable = false;
        //     ModeIndicator(mode);
        // }

        ModeIndicator(mode);
        PrintJustLabelData(teamID, missionTime, packetCount, cmdecho    
                            , tilt_x, tilt_y, rot_z, 
                            gps_alti, gps_lati, gps_long, 
                            gps_sat, gps_time);

        ParachuteAndHeatShieldData(parachute, heatshield);

        PrintStateData(state);

        RotateCansat(tilt_x, tilt_y, rot_z);
        PlotTextData(altitude, gps_alti, speed, temperature, presure, voltage);

        // grafiklerin events
        ExecuteOnMainThread(() => onAltitudeDataReadyEvent.Invoke(Faltitude));
        ExecuteOnMainThread(() => onSpeedDataReadyEvent.Invoke(Fspeed));
        ExecuteOnMainThread(() => onTemperatureDataReadyEvent.Invoke(Ftemperature));
        ExecuteOnMainThread(() => onPresureDataReadyEvent.Invoke(Fpresure));
        ExecuteOnMainThread(() => onVoltageDataReadyEvent.Invoke(Fvoltage));
        ExecuteOnMainThread(() => onGpsAltitudeDataReadyEvent.Invoke(Fgpsalti)); 

        List<string> tableData = new List<string>();
        tableData.AddRange(new string[15]{ packetCount, altitude, speed, temperature, voltage, presure, 
                           gps_time, gps_alti, gps_lati, gps_long, gps_sat, tilt_x, tilt_y, rot_z, missionTime});
                           
        ExecuteOnMainThread(() => onTableDataReceivedEvent.Invoke(tableData));

        // map event
        ExecuteOnMainThread(() => onMapDataReceivedEvent.Invoke(float.Parse(gps_lati, System.Globalization.CultureInfo.InvariantCulture),
                                                                float.Parse(gps_long, System.Globalization.CultureInfo.InvariantCulture)));

    }
}





