using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Bolinha bo;
    public TextMeshProUGUI jogadorScoreText;
    public TextMeshProUGUI inimigoScoreText;
    public TextMeshProUGUI mensagemFim;

    public int JogadorScore { get; set; } = 0;
    public int InimigoScore { get; set; } = 0;

    private int pontosParaVencer = 12;
    public bool jogoAcabou { get; private set; } = false;

    public void PontoJogador()
    {
        if (jogoAcabou) return;
        JogadorScore++;
        AtualizarTudo();
    }

    public void PontoInimigo()
    {
        if (jogoAcabou) return;
        InimigoScore++;
        AtualizarTudo();
    }

    private void AtualizarTudo()
    {
        jogadorScoreText.text = JogadorScore.ToString();
        inimigoScoreText.text = InimigoScore.ToString();

        if (JogadorScore >= pontosParaVencer && InimigoScore >= pontosParaVencer)
        {
            mensagemFim.text = "EMPATE!";
            Time.timeScale = 0;
            jogoAcabou = true;
        }
        else if (JogadorScore >= pontosParaVencer)
        {
            mensagemFim.text = "JOGADOR VENCEU!";
            Time.timeScale = 0;
            jogoAcabou = true;
        }
        else if (InimigoScore >= pontosParaVencer)
        {
            mensagemFim.text = "INIMIGO VENCEU!";
            Time.timeScale = 0;
            jogoAcabou = true;
        }
        else
        {
            bool vaiParaInimigo = JogadorScore > InimigoScore;
            bo.ResetarBola(vaiParaInimigo);
        }
    }
}
