using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform corpoPrefab;
    public Transform comidinhaPrefab;
    public Transform paredePrefab;

    public TextMeshProUGUI pontosText;
    public TextMeshProUGUI maiorpontuaText;
    public TextMeshProUGUI fimdejogoText;

    List<Transform> corpo = new List<Transform>();
    List<Transform> comidinha = new List<Transform>();
    List<Transform> parede = new List<Transform>();

    Vector2 direcao = Vector2.up;
    public float tamcelula = 0.3f;
    public float vel = 5.0f;

    public int initialFoods = 10;

    int pontos = 0;
    int maiorpontua = 0;

    float moveTime = 0;
    Vector2 snakeIndex;

    bool fimDeJogo = false;

    // Start is called before the first frame update
    void Start()
    {
        Createparede();
        SpawnComida();

        fimdejogoText.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (fimDeJogo)
        {
            if (Input.GetKeyDown(KeyCode.R)) Restar();
            return;
        }
        ChangeDirection();

        Move();
        ComerComida();
        VerificarColisaoparede();
        CheckCorpoCollision();



    }

    void ChangeDirection()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.y == -1)
        {
            direcao = Vector2.down;
        }
        else if (input.y == 1)
        {
            direcao = Vector2.up;
        }
        else if (input.x == -1)
        {
            direcao = Vector2.left;
        }
        else if (input.x == 1)
        {
            direcao = Vector2.right;
        }
    }
    void Move()

    {
        if (Time.time > moveTime)
        {
            for (int i = corpo.Count - 1; i > 0; i--)
            {
                corpo[i].position = corpo[i - 1].position;
            }
            if (corpo.Count > 0)
                corpo[0].position = (Vector2)transform.position;

            transform.position += (Vector3)direcao * tamcelula;
            moveTime = Time.time + 1 / vel;
            snakeIndex = new Vector2(Mathf.Round(transform.position.x / tamcelula), Mathf.Round(transform.position.y / tamcelula));
        }
    }
    void CrescerCorpo()
    {

        Vector2 position = transform.position;
        if (corpo.Count != 0)
            position = corpo[corpo.Count - 1].position *2;

        corpo.Add(Instantiate(corpoPrefab, position, Quaternion.identity).transform);
    }

    void SpawnComida()
    {
        float x = Random.Range(-23, 23) * tamcelula;
        float y = Random.Range(-13, 11) * tamcelula;
        Vector2 randomPosition = new Vector2(x, y);
        comidinha.Add(Instantiate(comidinhaPrefab, randomPosition, Quaternion.identity).transform);
    }
    void ComerComida()
    {
        for (int i = 0; i < comidinha.Count; ++i)
        {
            Vector2 comidinhaIndex = comidinha[i].position / tamcelula;

            // Usar uma margem de erro pequena para verificar proximidade
            if (Vector2.Distance(comidinhaIndex, snakeIndex) < 0.8f) // Defina uma tolerância pequena
            {
                Destroy(comidinha[i].gameObject);
                comidinha.RemoveAt(i);
                CrescerCorpo();
                pontos++;
                pontosText.text = "Pontos:  " + pontos.ToString();
                SpawnComida();
            }
        }
    }
    void Createparede()
    {
        float h = -24 * tamcelula, v = 11 * tamcelula;

        for (int i = 0; i <= Mathf.Abs((int)(h * 2 / tamcelula)); i++)
        {
            float x = h + tamcelula * i, y = v - 25 * tamcelula;
            parede.Add(Instantiate(paredePrefab, new Vector2(x, v), Quaternion.identity).transform);
            parede.Add(Instantiate(paredePrefab, new Vector2(x, y), Quaternion.identity).transform);
        }

        for (int i = 0; i < 25; i++)
        {
            float y = v - tamcelula * i;
            parede.Add(Instantiate(paredePrefab, new Vector2(h, y), Quaternion.identity).transform);
            parede.Add(Instantiate(paredePrefab, new Vector2(-h, y), Quaternion.identity).transform);
        }
    }
    void VerificarColisaoparede()
    {
        // Percorre todas as paredes
        for (int i = 0; i < parede.Count; i++)
        {
            // Pega a posição da parede atual
            Vector2 posicaoparede = parede[i].position / tamcelula;

            // Verifica se a posição da parede é igual à posição da cobra
            if (posicaoparede.x == snakeIndex.x && posicaoparede.y == snakeIndex.y)
            {

                FimDeJogo();
                break; // Sai do loop, pois já achou a colisão
            }
        }
    }
    void CheckCorpoCollision()
    {
        if (corpo.Count < 3) return;

        for (int i = 0; i < corpo.Count; i++)
        {
            Vector2 idx = corpo[i].position / tamcelula;
            if (Vector2.Distance(idx, snakeIndex) < 1f)
            {
                FimDeJogo();
                break;
            }
        }
    }


        void FimDeJogo()
        {
            fimDeJogo = true;
            fimdejogoText.gameObject.SetActive(true); 
        }
        void Restar()
        {
            fimDeJogo = false;
        fimdejogoText.gameObject.SetActive(false);

        if (pontos > maiorpontua) maiorpontua = pontos;
        maiorpontuaText.text = "Maior Pontuação:  " + maiorpontua.ToString();
        pontos = 0;
        pontosText.text = "Pontos:  " + pontos.ToString();

            for (int i = 0; i < corpo.Count; i++)
            {
                Destroy(corpo[i].gameObject);
            }
            corpo.Clear();

            for (int i = 0; i < comidinha.Count; ++i)
            {
                Destroy(comidinha[i].gameObject);
            }
            comidinha.Clear();
            transform.position = Vector3.zero;
            for (int i = 0; i < initialFoods; ++i) SpawnComida();

        }
  
}



