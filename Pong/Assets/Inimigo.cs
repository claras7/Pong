using UnityEngine;

public class Inimigo : MonoBehaviour
{
    private float velocidade = 8.0f;
    public Transform bolinha;

    void Update()
    {
        Vector2 novaPos = transform.position;

        if (bolinha.position.y > transform.position.y)
            novaPos.y += velocidade * Time.deltaTime;
        else if (bolinha.position.y < transform.position.y)
            novaPos.y -= velocidade * Time.deltaTime;

        novaPos.y = Mathf.Clamp(novaPos.y, -3.5f, 3.5f);
        transform.position = novaPos;
    }
}