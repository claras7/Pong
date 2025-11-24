using UnityEngine;
using TMPro; // se estiver usando TextMeshPro
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [Header("Referências da UI")]
    public TMP_InputField inputField;  // Campo para digitar mensagem
    public TMP_Text chatHistorico;     // Texto onde aparece o histórico
    public ClienteTCP clienteTCP;      // Script que envia mensagens via TCP

    // Chamado quando o jogador clica no botão "Enviar"
    public void EnviarMensagem()
    {
        string msg = inputField.text;
        if (!string.IsNullOrEmpty(msg))
        {
            // Adiciona ao histórico local
            chatHistorico.text += "\nEu: " + msg;

            // Envia a mensagem para o servidor via TCP
            clienteTCP.EnviarMensagem(msg);

            // Limpa o campo de digitação
            inputField.text = "";
            inputField.ActivateInputField(); // mantém o foco para digitar novamente
        }
    }

    // Chamado quando uma mensagem chega do servidor
    public void ReceberMensagem(string msg)
    {
        chatHistorico.text += "\nServidor: " + msg;
    }
}
