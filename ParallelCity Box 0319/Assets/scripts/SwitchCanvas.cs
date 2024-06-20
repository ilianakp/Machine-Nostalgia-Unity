using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SwitchCanvas : MonoBehaviour
{
    public GameObject GameCanvas;
    public GameObject InfoCanvas;
    public GameObject OpenCanvas;


    // Start is called before the first frame update
    void Start()
    {
        InfoCanvas.SetActive(false);
        GameCanvas.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        /* this is for not showing info icon in start stage, but we can do it if we want
        if ((!OpenCanvas.activeSelf)&&!GameCanvas.activeSelf)
        {
            GameCanvas.SetActive(true);
        }*/

        if (Input.GetKeyDown(KeyCode.I))
        {
            GameCanvas.SetActive(!GameCanvas.activeSelf);
            InfoCanvas.SetActive(!InfoCanvas.activeSelf);
        }
    }
}
