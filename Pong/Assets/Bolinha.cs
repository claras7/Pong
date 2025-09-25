using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bolinha : MonoBehaviour
{
    private Rigidbody2D rb;
    private float velocidade = 8.0f;
    void Start()
    {
        rb= GetComponent<Rigidbody2D>();
        
        float x = Random.Range(-1.0f,1.0f); 
        float y = Random.Range(-1.0f,1.0f);
        
        rb.linearVelocity = new Vector2(x,y).normalized * velocidade;
    }

    


    void Update()
    {
        
    }
}


