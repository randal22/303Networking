using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class ClientOut : MonoBehaviour
{
    
    //tcp handshake
    public static void ShakeReceived()
    {
        using (Packet pack = new Packet((int)PacketsOut.shakeReceived))
        {
            pack.Write(Client.instance.myId);
            pack.Write(UIManager.instance.usernameField.text);

            SendDataTCP(pack);
        }
    }
    private static void SendDataTCP(Packet pack)
    {
        pack.WritePacketLength();
        Client.instance.tcp.DataSend(pack);
    }


    
    public static void clientDataUpdate()
    {
        using (Packet pack = new Packet((int)PacketsOut.playerUpdate))
        {
            pack.Write(Client.instance.myId);
            pack.Write(GameManager.playersDict[Client.instance.myId].transform.position);
            //Debug.Log(GameManager.playersDict[Client.instance.myId].transform.position);
            pack.Write(GameManager.playersDict[Client.instance.myId].GetComponent<Rigidbody>().velocity);
            //Debug.Log(GameManager.playersDict[Client.instance.myId].GetComponent<Rigidbody>().velocity);
            pack.Write(System.DateTime.Now.Second);
            //Debug.Log(System.DateTime.Now.Second);
            
            pack.Write(System.DateTime.Now.Millisecond);
            //Debug.Log(System.DateTime.Now.Millisecond);
            SendDataUDP(pack);
        }
    }
    private static void SendDataUDP(Packet pack)
    {
        pack.WritePacketLength();
        Client.instance.udp.DataSend(pack);
    }
}