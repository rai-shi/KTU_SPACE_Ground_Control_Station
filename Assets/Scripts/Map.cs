using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour {

    // Relative position (0-1) between from and to
    // private float angle = 0.5f;
    // Move direction
    // private int direction = 1;

    [SerializeField] private OnlineMaps map;
    private OnlineMapsMarker marker;
    // [SerializeField] public RawImage map_obj;
    float latitude = 38.3772f;// 40.9976f; // 38.1952f; //
    float longtitude = -79.6076f; // 39.7710f; // -78.9981f; // 
    float width = 442f;
    float height = 345f;


    [SerializeField] GameObject print_function_object;
    private PrintFunctions print_function;

    void Start()
    {
        print_function = print_function_object.GetComponent<PrintFunctions>();
        print_function.onMapDataReceivedEvent.AddListener(ResetPosition); 
        // if (map_obj == null)
        // {
        //     map_obj = RawImage.FindWithTag("Map");
        //     map_obj.GetComponent<RectTransform>().sizeDelta = new Vector2 (width, height);
        // }
        map = OnlineMaps.instance;
        map.floatZoom = 18f; 
        map.SetPosition(longtitude, latitude);
        map.Redraw();

        marker = OnlineMapsMarkerManager.CreateItem(map.position);
        marker.scale = 1f;
        
    }

    private void Update()
    {
        
    }

    public void ResetPosition(float latitude, float longtitude)
    {
        map.SetPosition(longtitude, latitude);
        map.Redraw();
        marker.position = map.position;
    }

}
