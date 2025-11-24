using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class PongCliente : MonoBehaviour
{
    public Rigidbody2D bola;
    public Transform raqueteEsquerda;
    public Transform raqueteDireita;

    UdpClient udp;
    IPEndPoint servidorEndPoint;

    string ipDoServidor = "127.0.0.1"; // TROQUE SE FOR ONLINE

    void Start()
    {
        servidorEndPoint = new IPEndPoint(IPAddress.Parse(ipDoServidor), 5000);

        udp = new UdpClient();
        udp.Connect(servidorEndPoint);

        udp.BeginReceive(ReceberEstado, null);
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Vertical");

        // Envia input da raquete direita para o servidor
        byte[] data = Encoding.UTF8.GetBytes(input.ToString());
        udp.Send(data, data.Length);
    }

    // --- RECEBE POSIÇÕES DO SERVIDOR ---
    void ReceberEstado(IAsyncResult ar)
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

            bola.position = new Vector2(bx, by);
            raqueteEsquerda.position = new Vector3(raqueteEsquerda.position.x, ly, 0);
            raqueteDireita.position = new Vector3(raqueteDireita.position.x, ry, 0);
        }

        udp.BeginReceive(ReceberEstado, null);
    }
}
