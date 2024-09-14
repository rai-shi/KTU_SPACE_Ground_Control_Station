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
using UI.Pagination;
// using System.Windows.Forms;


public class DataTable : MonoBehaviour
{
    [SerializeField] private GameObject dataContainerObject;
    private Transform dataContainer;
    [SerializeField] private GameObject dataTemplateObject;
    private Transform dataTemplate;
    [SerializeField] GameObject printFunctionsObject;
    private PrintFunctions printFunctions;

    [SerializeField] PagedRect pagedRect;
    [SerializeField] GameObject bigContent;
    string dataContainerName = "Content";
    string dataTemplateName = "datatemplateObject";
    List<Page> pages = new List<Page>();


    RectTransform containerRectTransform;
    Vector2 currentPosition;
    float amountToIncrease = 50f;

    float templateHeight = 40f;
    int pageCounter = 0;
    int rowCounter = 0;

    Color darkGrayColor = new Color(0.706f, 0.706f, 0.706f, 1.000f);
    Color lightGrayColor = new Color(0.882f, 0.882f, 0.882f, 1.000f);

    private void Awake()
    {
        dataContainer = dataContainerObject.transform;
        dataTemplate = dataTemplateObject.transform;

        containerRectTransform = dataContainer.GetComponent<RectTransform>();

        dataTemplate.gameObject.SetActive(false);
        // pagedRect.gameObject.SetActive(false);

        Page temp_page = bigContent.transform.Find("Page 1").gameObject.GetComponent<Page>();
        pages.Add(temp_page);
    }
    void Start()
    {
        printFunctions = printFunctionsObject.GetComponent<PrintFunctions>();
        printFunctions.onTableDataReceivedEvent.AddListener(onTableDataReceived); 
    }

    void printDataOnTable(List<string> datas)
    {

        if ((dataTemplate == null) || (dataContainer == null) || (containerRectTransform == null))
        {
            string pageName = "Page 1";
            Transform new_page = bigContent.transform.Find(pageName);
            dataContainer = new_page.Find(dataContainerName);
            dataTemplate = dataContainer.Find(dataTemplateName);
            containerRectTransform = dataContainer.GetComponent<RectTransform>();
            dataTemplate.gameObject.SetActive(false);
        }

        Transform newDataRowTransform = Instantiate(dataTemplate, dataContainer);
        RectTransform newDataRowRectTransform = newDataRowTransform.GetComponent<RectTransform>();
        newDataRowRectTransform.anchoredPosition = new Vector2(690, -templateHeight * rowCounter);

        newDataRowTransform.gameObject.SetActive(true);

        containerRectTransform.sizeDelta = new Vector2 (containerRectTransform.sizeDelta.x,
                                                        containerRectTransform.sizeDelta.y + amountToIncrease );
        
        GameObject newDataRowPanelObject = newDataRowTransform.Find("Panel").gameObject;
        if ((rowCounter % 2) == 0)
        {
            Image panelImage = newDataRowPanelObject.GetComponent<Image>();
            panelImage.color = darkGrayColor;
        }
        else
        {
            Image panelImage = newDataRowPanelObject.GetComponent<Image>();
            panelImage.color = lightGrayColor;
        }

        Transform textsTransform = newDataRowPanelObject.transform;

        textsTransform.Find("packetcount").GetComponent<TextMeshProUGUI>().text = datas[0]; 
        textsTransform.Find("altitude").GetComponent<TextMeshProUGUI>().text = datas[1] +"m";
        textsTransform.Find("airspeed").GetComponent<TextMeshProUGUI>().text = datas[2] + "m/s";
        textsTransform.Find("temperature").GetComponent<TextMeshProUGUI>().text = datas[3] + "°C";
        textsTransform.Find("voltage").GetComponent<TextMeshProUGUI>().text = datas[4] + "V";
        textsTransform.Find("pressure").GetComponent<TextMeshProUGUI>().text = datas[5] + "KPa";
        textsTransform.Find("gpstime").GetComponent<TextMeshProUGUI>().text = datas[6] ;
        textsTransform.Find("gpsaltitude").GetComponent<TextMeshProUGUI>().text = datas[7] + "m";
        textsTransform.Find("gpslatitude").GetComponent<TextMeshProUGUI>().text = datas[8] +"°";
        textsTransform.Find("gpslongtitude").GetComponent<TextMeshProUGUI>().text = datas[9]+ "°";
        textsTransform.Find("gpssat").GetComponent<TextMeshProUGUI>().text = datas[10];
        textsTransform.Find("tiltx").GetComponent<TextMeshProUGUI>().text = datas[11] + "°";
        textsTransform.Find("tilty").GetComponent<TextMeshProUGUI>().text = datas[12] + "°";
        textsTransform.Find("rotz").GetComponent<TextMeshProUGUI>().text = datas[13] + "°";
        textsTransform.Find("missiontime").GetComponent<TextMeshProUGUI>().text = datas[14];
        rowCounter++;
    }

    void onTableDataReceived(List<string> datas)
    {
        printDataOnTable(datas);

        if (((pageCounter % 18) == 0) & (pageCounter != 0))
        {
            var page = pagedRect.AddPageUsingTemplate();
            string pageName = "Page " + (pageCounter / 18 + 1).ToString();
            page.PageTitle = pageName;
            pages.Add(page);
            Transform new_page = bigContent.transform.Find(pageName);
            dataContainer = new_page.Find(dataContainerName);
            dataTemplate = dataContainer.Find(dataTemplateName);
            containerRectTransform = dataContainer.GetComponent<RectTransform>();
            dataTemplate.gameObject.SetActive(false);
            rowCounter = 0;
        }
        pageCounter++;
    }

    public void ClearTable()
    {
        foreach (var page in pages)
        {
            pagedRect.RemovePage(page, true);
        }
        pages.Clear();

        Page temp_page = pagedRect.AddPageUsingTemplate();
        string pageName = "Page 1";
        temp_page.PageTitle = pageName;
        pages.Add(temp_page);

        pageCounter = 0;
        rowCounter = 0;
    }
}

// foreach (Transform child in dataContainer)
// {
//     if (child != dataTemplate)
//     {
//         Destroy(child.gameObject);
//     }
// }
// containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, 0); ?????????
// counter = 0;



