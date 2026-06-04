using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleJogador : MonoBehaviour
{
    public float horizontalInput;
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private float velocidadeAndar = 5f;
    [SerializeField] private float velocidadeCorrer = 9f;
    [SerializeField] private float forcaPulo = 12f;

    private float velocidadeAtual;
    private bool noChao;
    private bool olhandoParaDireita = true;

    //---- VARIÁVEIS PARA O SISTEMA DE CORRIDA ----
    private float tempoMaximoClique = 0.3f; // Tempo máximo entre cliques para considerar como corrida
    private float tempoUltimoCliqueA;
    private float tempoUltimoCliqueD;
    private bool estaCorrendo = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        // 1. Detectar duplo toque no "D" (para correr para a direita)
        if (Input.GetKeyDown(KeyCode.D))
        {
            float tempoDesdeUltimoD = Time.time - tempoUltimoCliqueD;
            if (tempoDesdeUltimoD <= tempoMaximoClique)
            {
                estaCorrendo = true;
            }
            tempoUltimoCliqueD = Time.time;
        }
        // 2. Detectar duplo toque no "A" (para correr para a esquerda)
        if (Input.GetKeyDown(KeyCode.A))
        {
            float tempoDesdeUltimoA = Time.time - tempoUltimoCliqueA;
            if (tempoDesdeUltimoA <= tempoMaximoClique)
            {
                estaCorrendo = true;
            }
            tempoUltimoCliqueA = Time.time;
        }
        // 3. Cancelar a corrida se o jogador soltar as teclas 
        if (horizontalInput == 0)
        {
            estaCorrendo = false;
        }
        // 4. Gerenciar velocidades e animações
        if (estaCorrendo && horizontalInput != 0)
        {
            velocidadeAtual = velocidadeCorrer;
            anim.SetFloat("Velocidade", 2f); // Animação de corrida
        }
        else if (horizontalInput != 0)
        {
            velocidadeAtual = velocidadeAndar;
            anim.SetFloat("Velocidade", 1f); // Animação de andar
        }
        else
        {
            velocidadeAtual = 0f;
            anim.SetFloat("Velocidade", 0f); // Animação de parada
        }
        // Envia para o Animator se o jogador está no chão ou voando
        anim.SetBool("noChão", noChao);

        //Comando para Pular 
        if (Input.GetButtonDown("Jump") && noChao)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            noChao = false;
        }
        AjustarDirecao(horizontalInput);
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * velocidadeAtual, rb.linearVelocity.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "CenarioTilemap" || collision.gameObject.name == "Chão")
        {
            noChao = true;
        }
    }

    private void AjustarDirecao(float direcao)
    {
        if (direcao > 0 && !olhandoParaDireita) Flipar();
        else if (direcao < 0 && olhandoParaDireita) Flipar();
    }
    private void Flipar()
    {
        olhandoParaDireita = !olhandoParaDireita;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}


