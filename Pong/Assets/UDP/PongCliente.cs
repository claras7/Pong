using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PongCliente : MonoBehaviour
{
    public Transform raqueteServidor;
    public Transform raqueteCliente;
    public Transform bolinha;
    public GameManager gm;
    public string ipServidor = "127.0.0.1";
    private UdpClient client;

    private async void Start()
    {
        client = new UdpClient();
        client.Connect(ipServidor, 9050);
        Debug.Log("Cliente conectado ao servidor em " + ipServidor);
        _ = ReceberEstadoAsync();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            EnviarMensagem("UP");
        else if (Input.GetKey(KeyCode.DownArrow))
            EnviarMensagem("DOWN");
    }

    private async Task ReceberEstadoAsync()
    {
        var endPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            var result = await client.ReceiveAsync();
            string msg = Encoding.UTF8.GetString(result.Buffer);
            string[] dados = msg.Split(';');

            if (dados.Length >= 6)
            {
                float yServidor = float.Parse(dados[0]);
                float yCliente = float.Parse(dados[1]);
                float xBola = float.Parse(dados[2]);
                float yBola = float.Parse(dados[3]);
                int scoreJogador = int.Parse(dados[4]);
                int scoreInimigo = int.Parse(dados[5]);

                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    raqueteServidor.position = new Vector3(raqueteServidor.position.x, yServidor, 0);
                    raqueteCliente.position = new Vector3(raqueteCliente.position.x, yCliente, 0);
                    bolinha.position = new Vector3(xBola, yBola, 0);
                    gm.jogadorScoreText.text = scoreJogador.ToString();
                    gm.inimigoScoreText.text = scoreInimigo.ToString();
                });
            }
        }
    }

    private void EnviarMensagem(string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        client.Send(data, data.Length);
    }

    private void OnApplicationQuit()
    {
        client?.Close();
    }
}
