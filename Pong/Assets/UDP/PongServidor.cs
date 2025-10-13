using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PongServidor : MonoBehaviour
{
    public Transform raqueteServidor;
    public Transform raqueteCliente;
    public Transform bolinha;
    public GameManager gm;

    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;
    private bool clienteConectado = false;
    private float velocidadeRaquete = 8f;

    private void Start()
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

            if (!clienteConectado)
            {
                clienteConectado = true;
                remoteEndPoint = result.RemoteEndPoint;
                Debug.Log("Cliente conectado: " + remoteEndPoint);
            }

            if (msg == "UP") MoverRaquete(raqueteCliente, 1);
            else if (msg == "DOWN") MoverRaquete(raqueteCliente, -1);
        }
    }

    private void Update()
    {
        if (gm != null && gm.jogoAcabou) return;

        // Movimenta raquete do servidor
        if (Input.GetKey(KeyCode.UpArrow)) MoverRaquete(raqueteServidor, 1);
        else if (Input.GetKey(KeyCode.DownArrow)) MoverRaquete(raqueteServidor, -1);

        // Envia estado do jogo se cliente conectado
        if (clienteConectado) EnviarEstado();
    }

    private void MoverRaquete(Transform raquete, int direcao)
    {
        Vector3 pos = raquete.position;
        pos.y += direcao * velocidadeRaquete * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, -4f, 4f);
        raquete.position = pos;
    }

    private void EnviarEstado()
    {
        if (remoteEndPoint == null) return;

        string msg = $"{raqueteServidor.position.y};{raqueteCliente.position.y};" +
                     $"{bolinha.position.x};{bolinha.position.y};" +
                     $"{gm.JogadorScore};{gm.InimigoScore}";
        byte[] data = Encoding.UTF8.GetBytes(msg);
        udpServer.Send(data, data.Length, remoteEndPoint);
    }

    private void OnApplicationQuit()
    {
        udpServer?.Close();
    }
}
