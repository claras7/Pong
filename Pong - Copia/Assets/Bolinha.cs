using UnityEngine;
using Random = UnityEngine.Random;

public class Bolinha : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float velocidade = 8f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        JogarBolinha(Random.value > 0.5f);
    }

    public void JogarBolinha(bool direcaoParaDireita)
    {
        float x = direcaoParaDireita ? 1f : -1f;
        float y = Random.Range(-0.5f, 0.5f);
        rb.linearVelocity = new Vector2(x, y).normalized * velocidade;
    }

    public void ResetarBola(bool direcaoParaDireita)
    {
        rb.position = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        JogarBolinha(direcaoParaDireita);
    }
}
