using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;

public class ClienteTCP : MonoBehaviour
{
    public string serverIP = "192.168.0.15";
    public int serverPort = 6000;

    TcpClient cliente;
    NetworkStream stream;

    void Start()
    {
        cliente = new TcpClient();
        cliente.Connect(serverIP, serverPort);
        stream = cliente.GetStream();
        Debug.Log("Conectado ao servidor TCP!");
    }

    public void EnviarMensagem(string msg)
    {
        if (stream == null) return;
        byte[] data = Encoding.UTF8.GetBytes(msg);
        stream.Write(data, 0, data.Length);
    }
}
