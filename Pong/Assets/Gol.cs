using UnityEngine;

public class Gol : MonoBehaviour
{
    public bool golDoJogador;
    public GameManager gm;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bolinha"))
        {
            if (golDoJogador)
                gm.PontoInimigo();
            else
                gm.PontoJogador();
        }
    }
}
