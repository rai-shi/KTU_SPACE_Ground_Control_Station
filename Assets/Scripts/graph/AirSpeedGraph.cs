using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;
using UnityEngine.Events;
public class AirSpeedGraph : MonoBehaviour
{
    private GraphChart graph;
    [SerializeField] GameObject print_function_object;
    private PrintFunctions print_function;
    [SerializeField] string speed_category;   
    int x_speed = 0;
    void Start()
    {
        graph = GetComponent<GraphChart>();
        print_function = print_function_object.GetComponent<PrintFunctions>();
        print_function.onSpeedDataReadyEvent.AddListener(onSpeedDataReady); 
   

    }
    
    void onSpeedDataReady(float data)
    {
        graph.DataSource.StartBatch(); 
        graph.DataSource.AddPointToCategoryRealtime(speed_category,x_speed,data);
        x_speed++;
        graph.DataSource.EndBatch();
    }
    
    void Update()
    {   
           
        
    }

    // void seed()
    // {
    //     graph.DataSource.StartBatch();

    //     graph.DataSource.AddPointToCategory("AltitudeCategory",Random.value*10f,Random.value*10f);
        
    //     graph.DataSource.EndBatch(); 
    // }
    public void onClick()
    {
        graph.DataSource.ClearCategory(speed_category);
    }
}
