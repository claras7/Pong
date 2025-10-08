using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Bolinha bo;
    public TextMeshProUGUI jogadorScore;
    public TextMeshProUGUI inimigoScore;
    public TextMeshProUGUI mensagemFim;

    private int jScore = 0;
    private int iScore = 0;
    private int pontosParaVencer = 12;

    public bool jogoAcabou { get; private set; } = false;

    public void pontoInimigo()
    {
        if(jogoAcabou) return;
        jScore++;
        AtualizarTudo();
    }

    public void pontoJogador()
    {
        if(jogoAcabou) return;
        iScore++;
        AtualizarTudo();
    }

    void AtualizarTudo()
    {
        jogadorScore.text = jScore.ToString();
        inimigoScore.text = iScore.ToString();

        if(iScore >= pontosParaVencer && jScore >= pontosParaVencer)
        {
            mensagemFim.text = "EMPATE!";
            Time.timeScale = 0;
            jogoAcabou = true;
        }
        else if(iScore >= pontosParaVencer)
        {
            mensagemFim.text = "JOGADOR VENCEU!";
            Time.timeScale = 0;
            jogoAcabou = true;
        }
        else if(jScore >= pontosParaVencer)
        {
            mensagemFim.text = "INIMIGO VENCEU!";
            Time.timeScale = 0;
            jogoAcabou = true;
        }
        else
        {
            bool vaiParaInimigo = jScore > iScore;
            bo.ResetarBola(vaiParaInimigo);
        }
    }
}