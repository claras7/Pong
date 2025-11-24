using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class PongServidor : MonoBehaviour
{
    [Header("Referências")]
    public Rigidbody2D bola;
    public Transform raqueteEsquerda;
    public Transform raqueteDireita;

    [Header("Configuração")]
    public float velocidadeRaquete = 6f;
    public float velocidadeInicialBola = 6f;

    private UdpClient udp;
    private IPEndPoint clienteEndPoint;

    private float inputCliente = 0f;

    void Start()
    {
        udp = new UdpClient(5000);
        udp.BeginReceive(ReceberInput, null);

        ReiniciarBola();
    }

    void Update()
    {
        // --- Movimentação da raquete do SERVIDOR (esquerda) ---
        float inputServidor = Input.GetAxisRaw("Vertical");
        raqueteEsquerda.Translate(Vector3.up * inputServidor * velocidadeRaquete * Time.deltaTime);

        // --- Movimentação da raquete do CLIENTE (direita) ---
        raqueteDireita.Translate(Vector3.up * inputCliente * velocidadeRaquete * Time.deltaTime);

        // --- Envia o estado do jogo para o cliente ---
        EnviarEstado();
    }

    void ReiniciarBola()
    {
        bola.linearVelocity = Vector2.zero;
        bola.position = Vector2.zero;

        Vector2 direcao = new Vector2(
            UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1,
            UnityEngine.Random.Range(-1f, 1f)
        ).normalized;

        bola.linearVelocity = direcao * velocidadeInicialBola;
    }

    // --- RECEBENDO INPUT DO CLIENTE ---
    void ReceberInput(IAsyncResult ar)
    {
        try
        {
            byte[] data = udp.EndReceive(ar, ref clienteEndPoint);
            string msg = Encoding.UTF8.GetString(data);

            if (float.TryParse(msg, out float valor))
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    inputCliente = valor;
                });
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Erro ao receber input: " + ex.Message);
        }

        udp.BeginReceive(ReceberInput, null);
    }

    // --- ENVIANDO ESTADO DO JOGO PARA O CLIENTE ---
    void EnviarEstado()
    {
        if (clienteEndPoint == null) return;

        string estado =
            bola.position.x + ";" +
            bola.position.y + ";" +
            raqueteEsquerda.position.y + ";" +
            raqueteDireita.position.y;

        byte[] data = Encoding.UTF8.GetBytes(estado);

        try
        {
            udp.Send(data, data.Length, clienteEndPoint);
        }
        catch (Exception ex)
        {
            Debug.Log("Erro ao enviar estado: " + ex.Message);
        }
    }
}
