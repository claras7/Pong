using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class PongCliente : MonoBehaviour
{
    [Header("Referências Visuais")]
    public Rigidbody2D bola;
    public Transform raqueteEsquerda;
    public Transform raqueteDireita;

    [Header("Configuração de Rede")]
    public string serverIP = "192.168.0.15"; // Coloque aqui o IP do servidor
    public int serverPort = 5000;

    private UdpClient udp;
    private IPEndPoint servidorEndPoint;

    void Start()
    {
        servidorEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        udp = new UdpClient();
        udp.Connect(servidorEndPoint);

        udp.BeginReceive(ReceberEstado, null);
    }

    void Update()
    {
        // Lê input do cliente (raquete direita)
        float input = Input.GetAxisRaw("Vertical");

        // Envia input para o servidor
        byte[] data = Encoding.UTF8.GetBytes(input.ToString());
        udp.Send(data, data.Length);
    }

    void ReceberEstado(IAsyncResult ar)
    {
        try
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udp.EndReceive(ar, ref ep);

            string msg = Encoding.UTF8.GetString(data);
            string[] valores = msg.Split(';');

            if (valores.Length == 4)
            {
                float bx = float.Parse(valores[0]);
                float by = float.Parse(valores[1]);
                float ly = float.Parse(valores[2]);
                float ry = float.Parse(valores[3]);

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    bola.position = new Vector2(bx, by);
                    raqueteEsquerda.position = new Vector3(raqueteEsquerda.position.x, ly, 0);
                    raqueteDireita.position = new Vector3(raqueteDireita.position.x, ry, 0);
                });
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Erro ao receber estado: " + ex.Message);
        }

        udp.BeginReceive(ReceberEstado, null);
    }
}
