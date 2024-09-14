using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;
using UnityEngine.Events;
public class TemperatureGraph : MonoBehaviour
{
    private GraphChart graph;
    [SerializeField] GameObject print_function_object;
    private PrintFunctions print_function;
    [SerializeField] string temperature_category;
    int x_temp = 0;
    void Start()
    {
        graph = GetComponent<GraphChart>();
        print_function = print_function_object.GetComponent<PrintFunctions>();
        print_function.onTemperatureDataReadyEvent.AddListener(onTemperatureDataReady);
    }
    void onTemperatureDataReady(float data)
    {
        graph.DataSource.StartBatch(); 
        graph.DataSource.AddPointToCategoryRealtime(temperature_category,x_temp,data);
        x_temp++;
        graph.DataSource.EndBatch();
    }
    void Update()
    {   
    }
    public void onClick()
    {
        graph.DataSource.ClearCategory(temperature_category);
    }

}
