using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PongServidor4 : MonoBehaviour
{
    public Transform[] raquetes; // 0-3: 0 e 1 lado esquerdo, 2 e 3 lado direito
    public Transform bolinha;
    public GameManager gm;
    private UdpClient udpServer;
    private Dictionary<int, IPEndPoint> clientes = new Dictionary<int, IPEndPoint>();
    private float velocidadeRaquete = 8f;

    void Start()
    {
        udpServer = new UdpClient(9050);
        Debug.Log("Servidor iniciado na porta 9050");
        _ = ReceberMensagensAsync();
    }

    private async Task ReceberMensagensAsync()
    {
        while (true)
        {
            var result = await udpServer.ReceiveAsync();
            string msg = Encoding.UTF8.GetString(result.Buffer);
            string[] partes = msg.Split(';');

            // mensagem: "ID;UP" ou "ID;DOWN"
            int id = int.Parse(partes[0]);
            string comando = partes[1];

            // registra cliente se ainda não estiver
            if (!clientes.ContainsKey(id))
            {
                clientes.Add(id, result.RemoteEndPoint);
                Debug.Log($"Cliente {id} conectado: {result.RemoteEndPoint}");
            }

            if (comando == "UP") MoverRaquete(raquetes[id], 1);
            else if (comando == "DOWN") MoverRaquete(raquetes[id], -1);
        }
    }

    void Update()
    {
        if (gm != null && gm.jogoAcabou) return;

        // Movimenta raquete do servidor local, caso queira controlar 1 raquete diretamente
        // Exemplo: raquete[0]
        if (Input.GetKey(KeyCode.W)) MoverRaquete(raquetes[0], 1);
        else if (Input.GetKey(KeyCode.S)) MoverRaquete(raquetes[0], -1);

        // Envia estado do jogo para todos clientes
        foreach (var cliente in clientes.Values)
        {
            EnviarEstado(cliente);
        }
    }

    private void MoverRaquete(Transform raquete, int direcao)
    {
        Vector3 pos = raquete.position;
        pos.y += direcao * velocidadeRaquete * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, -4f, 4f);
        raquete.position = pos;
    }

    private void EnviarEstado(IPEndPoint cliente)
    {
        string msg = $"{raquetes[0].position.y};{raquetes[1].position.y};" +
                     $"{raquetes[2].position.y};{raquetes[3].position.y};" +
                     $"{bolinha.position.x};{bolinha.position.y};" +
                     $"{gm.JogadorScore};{gm.InimigoScore}";

        byte[] data = Encoding.UTF8.GetBytes(msg);
        udpServer.Send(data, data.Length, cliente);
    }

    private void OnApplicationQuit()
    {
        udpServer?.Close();
    }
}
