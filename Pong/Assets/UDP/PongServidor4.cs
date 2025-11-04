using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PongServidor4 : MonoBehaviour
{
    [Header("Referências")]
    public Transform[] raquetes; // 0 = servidor, 1-3 = clientes
    public Rigidbody2D rbBola;
    public GameManager gm;

    [Header("Configurações")]
    public float velocidadeRaquete = 8f;
    public float intervaloEnvio = 0.05f; // 20x por segundo

    private UdpClient udpServer;
    private Dictionary<int, IPEndPoint> clientes = new Dictionary<int, IPEndPoint>();
    private float tempoEnvio = 0f;
    private bool rodando = true;

    void Start()
    {
        udpServer = new UdpClient(9050);
        Debug.Log("Servidor iniciado na porta 9050");

        rbBola.linearVelocity = new Vector2(5f, 3f);
        _ = ReceberMensagensAsync();
    }

    private async Task ReceberMensagensAsync()
    {
        while (rodando)
        {
            try
            {
                var result = await udpServer.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);
                string[] partes = msg.Split(';');

                int id = int.Parse(partes[0]);
                string comando = partes[1];

                if (!clientes.ContainsKey(id))
                {
                    clientes.Add(id, result.RemoteEndPoint);
                    Debug.Log($"Cliente {id} conectado: {result.RemoteEndPoint}");
                }

                if (id >= 1 && id < raquetes.Length)
                {
                    if (comando == "UP") MoverRaquete(raquetes[id], 1);
                    else if (comando == "DOWN") MoverRaquete(raquetes[id], -1);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Erro ao receber mensagem: " + e.Message);
            }

            await Task.Yield();
        }
    }

    void Update()
    {
        // Movimento do servidor (raquete 0)
        if (Input.GetKey(KeyCode.W)) MoverRaquete(raquetes[0], 1);
        else if (Input.GetKey(KeyCode.S)) MoverRaquete(raquetes[0], -1);

        // Movimento da bola
        rbBola.position += rbBola.linearVelocity * Time.deltaTime;

        if (rbBola.position.y > 4.5f || rbBola.position.y < -4.5f)
            rbBola.linearVelocity = new Vector2(rbBola.linearVelocity.x, -rbBola.linearVelocity.y);

        // Enviar atualizações para clientes
        tempoEnvio += Time.deltaTime;
        if (tempoEnvio >= intervaloEnvio)
        {
            tempoEnvio = 0f;
            EnviarEstadoParaTodos();
        }
    }

    private void MoverRaquete(Transform raquete, int direcao)
    {
        Vector3 pos = raquete.position;
        pos.y += direcao * velocidadeRaquete * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, -4f, 4f);
        raquete.position = pos;
    }

    private void EnviarEstadoParaTodos()
    {
        string msg = $"{raquetes[0].position.y};{raquetes[1].position.y};{raquetes[2].position.y};{raquetes[3].position.y};" +
                     $"{rbBola.position.x};{rbBola.position.y};{gm.JogadorScore};{gm.InimigoScore}";

        byte[] data = Encoding.UTF8.GetBytes(msg);

        foreach (var kvp in clientes)
        {
            try
            {
                udpServer.Send(data, data.Length, kvp.Value);
            }
            catch { }
        }
    }

    private void OnApplicationQuit()
    {
        rodando = false;
        udpServer?.Close();
    }
}
