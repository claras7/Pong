using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class PongServidor : MonoBehaviour
{
    public Rigidbody2D bola;
    public Transform raqueteEsquerda;
    public Transform raqueteDireita;

    public float velocidadeRaquete = 6f;
    public float velocidadeInicialBola = 6f;

    UdpClient udp;
    IPEndPoint clienteEndPoint;

    float inputCliente = 0f;

    void Start()
    {
        udp = new UdpClient(5000);
        udp.BeginReceive(ReceberInput, null);

        ReiniciarBola();
    }

    void Update()
    {
        // --- Movimentação da raquete do SERVIDOR ---
        float input = Input.GetAxisRaw("Vertical");
        raqueteEsquerda.transform.Translate(Vector3.up * input * velocidadeRaquete * Time.deltaTime);

        // --- Movimentação da raquete do CLIENTE ---
        raqueteDireita.transform.Translate(Vector3.up * inputCliente * velocidadeRaquete * Time.deltaTime);

        // Envia o estado do jogo para o cliente
        EnviarEstado();
    }

    void ReiniciarBola()
    {
        bola.linearVelocity = Vector2.zero;
        bola.position = Vector2.zero;

        Vector2 direcao = new Vector2(Random.Range(0, 2) == 0 ? -1 : 1,
                                      Random.Range(-1f, 1f)).normalized;

        bola.linearVelocity = direcao * velocidadeInicialBola;
    }

    // --- RECEBENDO O INPUT DO CLIENTE ---
    void ReceberInput(IAsyncResult ar)
    {
        byte[] data = udp.EndReceive(ar, ref clienteEndPoint);
        string msg = Encoding.UTF8.GetString(data);

        float valor;
        if (float.TryParse(msg, out valor))
            inputCliente = valor;

        udp.BeginReceive(ReceberInput, null);
    }

    // --- ENVIANDO ESTADO DO JOGO ---
    void EnviarEstado()
    {
        if (clienteEndPoint == null) return;

        string estado =
            bola.position.x + ";" +
            bola.position.y + ";" +
            raqueteEsquerda.position.y + ";" +
            raqueteDireita.position.y;

        byte[] data = Encoding.UTF8.GetBytes(estado);
        udp.Send(data, data.Length, clienteEndPoint);
    }
}
