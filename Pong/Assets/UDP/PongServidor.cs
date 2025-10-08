using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class PongServidor : MonoBehaviour
{
    UdpClient server;
    public int port = 9050;

    public GameManager gm;
    public Bolinha bolinha;
    public Transform playerRaquete;
    public Transform inimigoRaquete;

    private bool vezDoJogador = true;
    private IPEndPoint ultimoCliente = null;

    async void Start()
    {
        server = new UdpClient(port);
        Debug.Log("Servidor iniciado na porta " + port);
        await ReceberMensagensAsync();
    }

    private async Task ReceberMensagensAsync()
    {
        while(true)
        {
            var result = await server.ReceiveAsync();
            string msg = Encoding.UTF8.GetString(result.Buffer);
            ultimoCliente = result.RemoteEndPoint;

            // Movimentos do jogador
            if(!gm.jogoAcabou && vezDoJogador)
            {
                if(msg == "UP")
                    playerRaquete.position += Vector3.up * 0.5f;
                else if(msg == "DOWN")
                    playerRaquete.position += Vector3.down * 0.5f;

                vezDoJogador = false;
            }

            // Envia estado atualizado
            EnviarEstado();
        }
    }

    void Update()
    {
        if(gm.jogoAcabou) return;

        // Bola e raquete inimigo continuam quicando normalmente
        // Aqui podemos adicionar lógica da raquete inimigo se quiser
        // Para teste, raquete inimigo fica fixa

        // Alterna turno
        vezDoJogador = true;
    }

    void EnviarEstado()
    {
        if(ultimoCliente == null) return;

        string estado = $"STATE,{bolinha.transform.position.x},{bolinha.transform.position.y}," +
                        $"{playerRaquete.position.y},{inimigoRaquete.position.y},{vezDoJogador}," +
                        $"{gm.jogoAcabou},{gm.mensagemFim.text}";

        byte[] data = Encoding.UTF8.GetBytes(estado);
        server.Send(data, data.Length, ultimoCliente);
    }

    void OnApplicationQuit()
    {
        server.Close();
    }
}
