using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this file is used to take inputs from the user through unity's input field class / game object
//it was adapted from code found in this video https://www.youtube.com/watch?v=uh8XaC0Y5MA by Tom Weiland

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    
    public InputField portField;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("already exists");
            Destroy(this);
        }
    }

    public void StartServer()
    {
        startMenu.SetActive(false);
             
        portField.interactable = false;

        
        Server.StartServer(Convert.ToInt32(portField.text),6);
    }
}