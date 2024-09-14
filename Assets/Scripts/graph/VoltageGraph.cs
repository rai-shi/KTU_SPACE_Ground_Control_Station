using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;
using UnityEngine.Events;
public class VoltageGraph : MonoBehaviour
{
    private GraphChart graph;
    [SerializeField] GameObject print_function_object;
    private PrintFunctions print_function;
    [SerializeField] string voltage_category; 
    int x_volt = 0;

    void Start()
    {
        graph = GetComponent<GraphChart>();
        print_function = print_function_object.GetComponent<PrintFunctions>();
        print_function.onVoltageDataReadyEvent.AddListener(onVoltageDataReady); 
    }
    void onVoltageDataReady(float data)
    {
        graph.DataSource.StartBatch(); 
        graph.DataSource.AddPointToCategoryRealtime(voltage_category,x_volt,data);
        x_volt++;
        graph.DataSource.EndBatch();
    }
    void Update()
    {
        
    }
    public void onClick()
    {
        graph.DataSource.ClearCategory(voltage_category);
    }
}
