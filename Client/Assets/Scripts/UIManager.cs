using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this code is used to take inputs from the user through unity's input field class / game object
//it was adapted from code found in this video https://www.youtube.com/watch?v=uh8XaC0Y5MA by Tom Weiland

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipField;
    public InputField portField;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("already exists!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        ipField.interactable = false;
        portField.interactable = false;
        Client.instance.MakeConnectionToServer();
    }
}