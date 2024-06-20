using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using UnityEngine.UI;

public class activateScreenshot : MonoBehaviour
{
    bool showCanvas;
    public GameObject canvas;
    public Camera cam_map;
    public Camera cam_city;

    public GameObject[] bars = new GameObject[6];
    float[] percent = new float[6];

    string FilePath;

    string JoinTime;
    string StartTime;
    string OutTime;
    int infofreq;

    bool exit;

    void Start()
    {
        showCanvas = false;
        canvas.SetActive(showCanvas);
        exit = false;
        JoinTime = "0";
    }

    void Update()
    {
        //time recording
        {
          if (JoinTime.Length == 1)
          { JoinTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");}

          if (Input.GetKeyDown(KeyCode.Return))
          { StartTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");} 
        }

        //cleanRam
        if (File.Exists(FilePath))
            File.Delete(FilePath);
        FilePath = Application.dataPath +"/"+ DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".csv";

        // when player presses escape to exit the game, take a screenshot
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OutTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            print("escape pressed");
            if (!exit)
            {
                takeScreenshot(cam_map, "path");
                takeScreenshot(cam_city, "city");

                saveData();
            }
            exit = true;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!exit)
            {
                // show info -- activate canvas
                showCanvas = !showCanvas;
                canvas.SetActive(showCanvas);
                infofreq++;
            }
        }

        if (exit == true)
        {
            Application.Quit();
        }

        for (int i = 0; i < bars.Length; i++)
        {
            percent[i] = bars[i].GetComponent<Image>().fillAmount;
        }
    }

    void takeScreenshot(Camera cam, string type)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        // Render the camera's view.
        cam.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;

        string filename = ScreenShotName(type);
        byte[] bytes = image.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }

    public static string ScreenShotName(string name)
    {
        return string.Format("{0}/{1}_{2}.png",
                             Application.dataPath,
                             name,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    void saveData()
    {

        List<string> output = new List<string>();

        //Record Time
        output.Add("Join Time: ");
        output.Add(JoinTime);
        output.Add("Start Time: ");
        output.Add(StartTime);
        output.Add("End Time: ");
        output.Add(OutTime);

        //Record Frequncy
        output.Add("Info page open Frequency: ");
        output.Add((infofreq/2).ToString());

        //Record 

        /*
        for (int i = 0; i < percent.Length; i++)
        {
            output.Add("City" + i);
            output.Add(percent[i].ToString());
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < output.Count; i++)
        { sb.AppendLine(string.Join(",", output[i])); }
     
        File.AppendAllText(FilePath, sb.ToString());

        Debug.Log("Took data to: "+ Application.dataPath +"/"+ DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");*/
    } 
        
}
