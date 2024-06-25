using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPSender : MonoBehaviour {
    private UdpClient udpClient;
    private string ipAddress = "127.0.0.1";
    private int port = 12354;

    void Start() {
        udpClient = new UdpClient();
    }

    void Update() {
        /*
        if(Keyboard.current.sKey.wasPressedThisFrame)
        {
            SendData("Start");
            Debug.Log("Start");
        }
        else if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            SendData("End");
            Debug.Log("End");
        }
        */
    }

    public void SendData(string data) 
    {
        byte[] sendData = Encoding.UTF8.GetBytes(data);
        udpClient.Send(sendData, sendData.Length, ipAddress, port);
    }
}
