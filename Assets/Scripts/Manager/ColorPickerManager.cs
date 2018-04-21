

using System;
using System.IO;
using UnityEngine;

public class ColorPickerManager : MonoBehaviour
{
    private static ColorPickerManager _instance;

    public static ColorPickerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public RTColorPicker ColorPicker;
    private Rect _boxRect = new Rect(0, 0, 220, Screen.height);
    private string _customColorsFilePath;
    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {
        ColorPicker.SetPos((int)_boxRect.width + 5, 20);

        //Set our callback functions for RTColorPicker (Optional)
        ColorPicker.OnColorChangeEvent += OnColorChange;
        ColorPicker.OnColorCancelEvent += OnColorPickerCancel;
        ColorPicker.OnColorOKEvent += OnColorPickerOk;
        //ColorPicker.OnCustomColorSaveEvent += OnColorPickerCustomColorsSave;
        ColorPicker.OnCustomColorLoadedEvent += OnColorPickerCustomColorsLoaded;

        //Only Setup the following if we're not running in a Webplayer
        if (!Application.isWebPlayer)
        {
            //Setup the default filepath for our custom colors	
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                _customColorsFilePath = Environment.CurrentDirectory + "/custom_colors.txt";
            else
                _customColorsFilePath = Environment.CurrentDirectory + "\\custom_colors.txt";

            //Load Custom Colors File
            LoadCustomColors();
        }//if
    }

    private void OnColorPickerCancel(RGBColor oldcolor, RGBColor newcolor)
    {
       // Debug.Log("OnColorPickerCancel " + oldcolor.ToUnityColor().ToString() + " " + newcolor.ToUnityColor().ToString());
    }

    private void OnColorPickerOk(RGBColor oldcolor, RGBColor newcolor)
    {
        //Debug.Log("OnColorPickerOk " + oldcolor.ToUnityColor().ToString() + " " + newcolor.ToUnityColor().ToString());
    }

    private void OnColorPickerCustomColorsLoaded()
    {
       // Debug.Log("OnColorPickerCustomColorsLoaded ");
    }

    private void OnColorChange(RGBColor rgbcolor, string s)
    {
       // Debug.Log("OnColorChange " + rgbcolor.ToUnityColor().ToString() + " " + s);
    }



    /*In this example we'll be loading a custom colors file from the local system. This is only 
possible if we are not in the WebPlayer. Webplayer games could load the Custom Colors
using WWW or getting the data from a external javascript call.

The file we are loading is saved in the OnColorPickercustomColorsSave function below*/
    void LoadCustomColors()
    {
        if (!Application.isWebPlayer)
        {
            if (File.Exists(_customColorsFilePath))
            {
                StreamReader SR = File.OpenText(_customColorsFilePath);
                string Line = SR.ReadLine();
                string FileContent = "";
                while (Line != null)
                {
                    //Add the Line Terminator as the LoadCustomColors
                    //function splits the values by line terminator
                    FileContent += Line + Environment.NewLine;
                    Line = SR.ReadLine();
                }//while

                SR.Close();

                ColorPicker.LoadCustomColors(FileContent);
            }//if		
        }//if
    }//LoadCustomColors

    public void PickColor(Renderer renderer)
    {
        //ColorPicker.Show(new RGBColor(Color.red));
        Material mat = renderer.sharedMaterial; //share才能保留住
       // ColorPicker.Show(mat);
        PickColor(mat);
    }

    public void PickColor(Material material)
    {
        ColorPicker.Show(material);
    }
}
