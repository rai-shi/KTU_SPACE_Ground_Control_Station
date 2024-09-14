using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;
using UnityEngine.Events;
public class AltitudeGraph : MonoBehaviour
{    
    private GraphChart graph;
    [SerializeField] GameObject print_function_object;
    private PrintFunctions print_function;
    [SerializeField] string altitude_category;  
   
    int x_alti = 0;
    void Start()
    {
        graph = GetComponent<GraphChart>();
        print_function = print_function_object.GetComponent<PrintFunctions>();
        print_function.onAltitudeDataReadyEvent.AddListener(onAltitudeDataReady);  
    }
    
    void onAltitudeDataReady(float data)
    {
        graph.DataSource.StartBatch(); // start a new update batch
        graph.DataSource.AddPointToCategoryRealtime(altitude_category,x_alti,data);
        x_alti++;
        graph.DataSource.EndBatch();
    }
    
    void Update()
    {   
           
        
    }
    public void onClick()
    {
        graph.DataSource.ClearCategory(altitude_category);
    }
}
