using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;
using UnityEngine.Events;
public class PressureGraph : MonoBehaviour
{
    private GraphChart graph;
    [SerializeField] GameObject print_function_object;
    private PrintFunctions print_function;
   [SerializeField] string presure_category; 
   int x_pres = 0;

    void Start()
    {
        graph = GetComponent<GraphChart>();
        print_function = print_function_object.GetComponent<PrintFunctions>();
        print_function.onPresureDataReadyEvent.AddListener(onPresureDataReady); 
    }
        void onPresureDataReady(float data)
    {
        graph.DataSource.StartBatch(); 
        graph.DataSource.AddPointToCategoryRealtime(presure_category,x_pres,data);
        x_pres++;
        graph.DataSource.EndBatch();
    }
    void Update()
    {   
           
    }
    public void onClick()
    {
        graph.DataSource.ClearCategory(presure_category);
    }

}
