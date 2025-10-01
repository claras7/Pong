using TMPro;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Bolinha bo;
    public TextMeshProUGUI jogadorSocre;
    public TextMeshProUGUI inimigoSocre;

    private int jScore = 0;
    private int iScore = 0;

    public void pontoInimigo()
    {
        jScore += 1;
        atualizarTudo();
    }
    public void pontoJogador()
    {
        iScore += 1;
        atualizarTudo();
    }


    public void atualizarTudo()
    {
        //reseta bolinha
        jogadorSocre.text = jScore.ToString();
        inimigoSocre.text = iScore.ToString();
        bo.JogarBolinha();
    }
}
