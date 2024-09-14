using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;
using UnityEngine.Events;
public class GpsAltitudeGraph : MonoBehaviour
{
    private GraphChart graph;
    [SerializeField] GameObject print_function_object;
    private PrintFunctions print_function;
    [SerializeField] string gpsAltitude_category;  
    int x_gps_alti = 0;
    void Start()
    {
        graph = GetComponent<GraphChart>();
        print_function = print_function_object.GetComponent<PrintFunctions>();
        print_function.onGpsAltitudeDataReadyEvent.AddListener(onGpsAltitudeDataReady); 
    }
    void onGpsAltitudeDataReady(float data)
    {
        graph.DataSource.StartBatch(); 
        graph.DataSource.AddPointToCategoryRealtime(gpsAltitude_category,x_gps_alti,data);
        x_gps_alti++;
        graph.DataSource.EndBatch();
    }


    void Update()
    {   
           
        
    }
    public void onClick()
    {
        graph.DataSource.ClearCategory(gpsAltitude_category);
    }
}
