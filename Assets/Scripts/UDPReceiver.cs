using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UDPReceiver : MonoBehaviour
{
    UdpClient udpClient;
    public static int receivedInt;
    int listenPort = 12354;

    void Start()
    {
        udpClient = new UdpClient(listenPort);
        BeginReceive();
    }

    void BeginReceive()
    {
        udpClient.BeginReceive(new AsyncCallback(OnReceived), null);
    }

    void OnReceived(IAsyncResult result)
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
        byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEP);
        // バイト配列を整数に変換
        if (receivedBytes.Length == 4)
        {
            receivedInt = BitConverter.ToInt32(receivedBytes, 0);
            Debug.Log("Received int: " + receivedInt);
        }
        

        // 再度受信を開始
        BeginReceive();
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }

}