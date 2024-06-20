using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;

public class TrackAsCsv : MonoBehaviour
{
    // Start is called before the first frame update
    string FilePath;
    string StartTime;
    string EndTime;
    string[] percentages;
    public GameObject [] bars;

    void Start()
    {
        FilePath = Application.dataPath+"/";
        print(FilePath);
        if (File.Exists(FilePath))
           File.Delete(FilePath);
        FilePath = FilePath + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";

        StartTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        percentages = new string[bars.Length];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            for (int i = 0; i < bars.Length; i++)
            {
                percentages[i] = bars[i].GetComponent<Image>().fillAmount.ToString();
                print(percentages[i]);
            }


        EndTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            string[] output = new string[4 + bars.Length];

            output[0] = "StartTime";
            output[1] = StartTime;
            output[2] = "EndTime";
            output[3] = EndTime;

            for (int i = 4; i < output.Length; i++)
            {
                output[i] = percentages[i-4];
            }



            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < output.Length; i++)
            { sb.AppendLine(string.Join(",", output[i])); }

            if (!File.Exists(FilePath))
            { File.WriteAllText(FilePath, sb.ToString()); }
            else
            { File.AppendAllText(FilePath, sb.ToString()); }    
        }
    }
}
