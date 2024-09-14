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
public class ThemeControl : MonoBehaviour
{
    // white color FFFF
    // dark gray 70 70 70 a1
    // light gray 150 150 150 a1

    [SerializeField] private Image parentBackground;
    // [SerializeField] private Image teleTabBackgroundCanvas;
    [SerializeField] private Image[] darkOnes;
    [SerializeField] private Image[] lightOnes;
    [SerializeField] private TextMeshProUGUI[] darkTexts;
    [SerializeField] private TextMeshProUGUI[] whiteTexts;
    [SerializeField] private Button[] Buttons;
    [SerializeField] private Image[] PlotBackgrounds;


    Color lightColor = new Color(255f/255, 255f/255, 255f/255);
    Color lightGrayColor = new Color(200f/255, 200f/255, 200f/255);
    Color darkColor = new Color(70f/255, 70f/255, 70f/255);
    Color darkGrayColor = new Color(0.588f, 0.588f, 0.588f, 1.000f);
    Color darktextColor = new Color(50f/255, 50f/255, 50f/255);
    Color plotDarkThemeColor = new Color(0.745f, 0.745f, 0.745f, 1.000f);

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void ChangeTheme()
    {
        Debug.Log(parentBackground.color);
        if (parentBackground.color == Color.white)
        {
            SetLightTheme();
        }
        else
        {
            SetDarkTheme();
        } 
    }

    void SetDarkTheme()
    {
        foreach (var dark in darkOnes)
        {
            dark.color = darkColor;
        }
        foreach (var light in lightOnes)
        {
            light.color = darkGrayColor;
        }
        foreach (var plotBG in PlotBackgrounds)
        {
            plotBG.color = plotDarkThemeColor; //plotDarkThemeColor
        }
        foreach (var wtext in whiteTexts)
        {
            wtext.color = Color.white;
        }
        foreach (var dtext in darkTexts)
        {
            dtext.color = darktextColor;
        }
        foreach (var button in Buttons)
        {
            button.GetComponent<Image>().color = Color.white;
        }
        parentBackground.color = Color.white;
        // teleTabBackgroundCanvas.color = darkColor;
    }
    void SetLightTheme()
    {
        foreach (var dark in darkOnes)
        {
            dark.color = lightColor;
        }
        foreach (var light in lightOnes)
        {
            light.color = lightGrayColor;
        }
        foreach (var plotBG in PlotBackgrounds)
        {
            plotBG.color = lightGrayColor;
        }
        foreach (var wtext in whiteTexts)
        {
            wtext.color = darktextColor;
        }
        foreach (var dtext in darkTexts)
        {
            dtext.color = Color.white;
        }
        foreach (var button in Buttons)
        {
            button.GetComponent<Image>().color = darkGrayColor;
        }
        parentBackground.color = darkGrayColor;
        // teleTabBackgroundCanvas.color = lightGrayColor;
    }
}
