using UnityEngine;

public class Player : MonoBehaviour
{

    private float velocidade = 10.0f;
    

    
    void Update()
    {
        float movimento = Input.GetAxis("Vertical");
        Vector2 novaPos = transform.position;
        novaPos.y += movimento * velocidade * Time.deltaTime;

        novaPos.y = Mathf.Clamp(novaPos.y, -3.5f, 3.5f);
        transform.position = novaPos;
    }
}
