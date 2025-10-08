using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class PongCliente : MonoBehaviour
{
    UdpClient client;
    string serverIP = "127.0.0.1";
    int serverPort = 9050;

    public Transform playerRaquete;
    public Transform inimigoRaquete;
    public Bolinha bolinha;
    public TextMeshProUGUI mensagemFim;

    private bool minhaVez = false;

    async void Start()
    {
        client = new UdpClient();
        client.Connect(serverIP, serverPort);
        await ReceberMensagensAsync();
    }

    void Update()
    {
        if(!minhaVez) return;

        if(Input.GetKeyDown(KeyCode.W))
            EnviarMovimento("UP");
        if(Input.GetKeyDown(KeyCode.S))
            EnviarMovimento("DOWN");
    }

    void EnviarMovimento(string mov)
    {
        byte[] data = Encoding.UTF8.GetBytes(mov);
        client.Send(data, data.Length);
    }

    private async Task ReceberMensagensAsync()
    {
        while (true)
        {
            var result = await client.ReceiveAsync();
            string msg = Encoding.UTF8.GetString(result.Buffer);

            if(msg.StartsWith("STATE"))
            {
                string[] partes = msg.Split(',');

                // Atualiza posições
                bolinha.transform.position = new Vector3(float.Parse(partes[1]), float.Parse(partes[2]), 0);
                playerRaquete.position = new Vector3(playerRaquete.position.x, float.Parse(partes[3]), 0);
                inimigoRaquete.position = new Vector3(inimigoRaquete.position.x, float.Parse(partes[4]), 0);

                // Atualiza turno
                minhaVez = bool.Parse(partes[5]);

                // Atualiza fim de jogo
                bool acabou = bool.Parse(partes[6]);

                // Pega todo o texto do fim de jogo (pode ter vírgulas)
                string texto = string.Join(",", partes, 7, partes.Length - 7);

                if(acabou)
                {
                    Time.timeScale = 0;
                    mensagemFim.text = texto;
                }
            }
        }
    }
}
