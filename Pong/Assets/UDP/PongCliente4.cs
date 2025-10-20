using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PongCliente4 : MonoBehaviour
{
    public int meuID; // 0-3
    public Transform[] raquetes; // posições de todas as raquetes
    public Transform bolinha;
    public GameManager gm;
    public string ipServidor = "127.0.0.1";
    private UdpClient client;

    private async void Start()
    {
        client = new UdpClient();
        client.Connect(ipServidor, 9050);
        Debug.Log($"Cliente {meuID} conectado ao servidor em {ipServidor}");
        _ = ReceberEstadoAsync();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) EnviarMensagem("UP");
        else if (Input.GetKey(KeyCode.DownArrow)) EnviarMensagem("DOWN");
    }

    private async Task ReceberEstadoAsync()
    {
        var endPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            var result = await client.ReceiveAsync();
            string msg = Encoding.UTF8.GetString(result.Buffer);
            string[] dados = msg.Split(';');

            if (dados.Length >= 8)
            {
                UnityMainThreadDispatcher.Enqueue(() =>
                {
                    for (int i = 0; i < 4; i++)
                        raquetes[i].position = new Vector3(raquetes[i].position.x, float.Parse(dados[i]), 0);

                    bolinha.position = new Vector3(float.Parse(dados[4]), float.Parse(dados[5]), 0);
                    gm.jogadorScoreText.text = dados[6];
                    gm.inimigoScoreText.text = dados[7];
                });
            }
        }
    }

    private void EnviarMensagem(string comando)
    {
        string msg = $"{meuID};{comando}";
        byte[] data = Encoding.UTF8.GetBytes(msg);
        client.Send(data, data.Length);
    }

    private void OnApplicationQuit()
    {
        client?.Close();
    }
}
