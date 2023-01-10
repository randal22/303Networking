using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;

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

    private void Start()
    {
        //unity settings
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        
        
        

    }
    public Player InstaPlayerGameObj()
    {
        //spawn player in at coords
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    
}