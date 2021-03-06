﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ObjectCollider : MonoBehaviour
{
    public GameManeger gameManeger;

    [Tooltip("Referencia texto que conta objetos")]
    public GameObject Object;

    public AudioSource somObjeto;

    public TextMeshProUGUI triggerText;

    private ObjectCount objectCountScript;

    public List<string> objectList = new List<string>();

    static private int count = 0;

    private GameObject Other;

    private bool isTrigger = false;

    static public bool energy = false;

    public GameObject cartaoDeFuncionario;

    public GameObject moeda;

    public GameObject painelDesligado;

    public GameObject painelLigado;

    public GameObject Lanche;

    public GameObject Chave;

    void Start()
    {
        
        objectCountScript = Object.GetComponent<ObjectCount>();
        if (energy) {
            painelLigado.GetComponent<SpriteRenderer>().enabled = true;
        }
        else {
            painelDesligado.GetComponent<SpriteRenderer>().enabled = true;
        }
        //energy = gameManeger.eletricidade;
    }

    void Awake()
    {
        /* Recuperando os itens quando se troca de cena */
        int quantItens = PlayerPrefs.GetInt("Quant_Itens");
        for (int i = 0; i < quantItens; i++)
        {
            objectList.Add(PlayerPrefs.GetString("item_" + i));
        }
    }

    void OnDestroy()
    {
        PlayerPrefs.SetInt("Quant_Itens", objectList.Count);
        for (int i = 0; i < objectList.Count; i++)
        {
            PlayerPrefs.SetString("item_" + i, objectList[i]);
        }
    }

    void Update()
    {
        if (isTrigger)
        {
            PressZ();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Other = other.gameObject;
        if (Other.CompareTag("Object"))
        {
            isTrigger = true;
            triggerText.gameObject.SetActive(true);
            triggerText.text = "Aperte E para interagir";
        }

        if (Other.name == "Fios")
        {
            if (energy)
            {
                
                Other.GetComponent<BoxCollider2D>().enabled = true;
                triggerText.text = "Os fios elétricos podem dar choque, desligue a eletricidade para passar";
                triggerText.gameObject.SetActive(true);
                StartCoroutine(DisableText(triggerText.gameObject));

            }
            if (!energy)
            {
                Other.transform.GetChild(0).gameObject.SetActive(false);
                Other.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            //Particles.Stop();
            triggerText.gameObject.SetActive(false);
            isTrigger = false;
        }
    }

    void PressZ()
    {
        if (Input.GetKeyDown("e"))
        {
            //Particles.Stop();
            if (Other.name == "Carteira")
            {
                //objectList.Add("Cartão de funcionário");
                //objectList.Add("Moeda");
                Other.GetComponent<SpriteRenderer>().enabled = false;
                Other.GetComponent<BoxCollider2D>().enabled = false;
                //Other.SetActive(false);
                cartaoDeFuncionario.SetActive(true);
                moeda.SetActive(true);
                count++;
                objectCountScript.CountText(count);
                triggerText.text = "Voce achou uma carteira com moedas e um cartão de funcionário";
               
            }
            else if (Other.name == "Maquina de Lanches")
            {
                if (objectList.Contains("Moeda") && energy)
                {
                    objectList.Remove("Moeda");
                    //objectList.Add("Lanche");
                    Lanche.SetActive(true);
                    count++;
                    objectCountScript.CountText(count);
                    triggerText.text = "Você usou as moedas para pegar o lanche";
                }
                else if (objectList.Contains("Moeda") && !energy) {
                    triggerText.text = "A máquina não funcionará sem eletricidade";
                }
                else if (!objectList.Contains("Moeda") && energy) {
                    triggerText.text = "A máquina não funcionará sem dinheiro";
                }

                else {
                    triggerText.text = "Você não tem os itens adequados";
                }
            }
            else if (Other.name == "Chave quebrada" )
            {
                if(objectList.Contains("Super cola"))
                {
                    objectList.Remove("Chave quebrada");
                    Other.GetComponent<SpriteRenderer>().enabled = false;
                    Other.GetComponent<BoxCollider2D>().enabled = false;
                    //objectList.Add("Chave consertada");
                    count++;
                    objectCountScript.CountText(count);
                    triggerText.text = "Você usou a super cola para colar a chave";
                    Chave.SetActive(true);
                }
                else {
                    triggerText.text = "A chave está quebrada";
                }
                
            }
            else if (Other.name == "PAINEL DE ENERGIA" || Other.name == "PAINEL DE ENERGIA DESLIGADA")
            {
                if (energy)
                {
                    energy = false;
                    gameManeger.eletricidade = energy;
                    painelLigado.GetComponent<SpriteRenderer>().enabled = false;
                    painelDesligado.GetComponent<SpriteRenderer>().enabled = true;
                    triggerText.text = "Você desligou a energia";
                }
                else
                {
                    energy = true;
                    gameManeger.eletricidade = energy;
                    painelDesligado.GetComponent<SpriteRenderer>().enabled = false;
                    painelLigado.GetComponent<SpriteRenderer>().enabled = true;
                    triggerText.text = "Você ligou a energia";
                }
            }
            else if(Other.name == "Saida") {
                if(objectList.Contains("Camisa Final")) {
                    triggerText.text = "Parabéns!";
                    GameManeger.seg = 0;
                    StartCoroutine(GoCredits());
                }
                else {
                    triggerText.text = "Você ainda não possui a Camisa do time";
                }
            }
            else // camisa, taco de beisebol, super cola, pé de cabra, cartao funcionario
            {
                objectList.Add(Other.name);
                Other.SetActive(false);
                if (Other.name == "camisa" || Other.name == "Super cola" || Other.name == "Moeda" || Other.name == "Chave consertada")    
                    triggerText.text = "Você pegou a " + Other.name;
                else if (Other.name == "Camisa Final") {
                    triggerText.text = "Parabéns! Você vestiu a Camisa do time! Agora é só sair do shopping, mas cuidado com o tempo";
                    GameManeger.seg = 600;
                    SpawnPoint.Checked = true;
                }
                else
                    triggerText.text = "Você pegou o " + Other.name;
                count++;
                objectCountScript.CountText(count);
            }
            triggerText.gameObject.SetActive(true);
            isTrigger = false;
            StartCoroutine(DisableText(triggerText.gameObject));
            somObjeto.Play();
        }
    }

    private IEnumerator GoCredits() {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("01 - Credits");
    }

    private IEnumerator DisableText(GameObject texto)
    {
        yield return new WaitForSeconds(1.5f);
        texto.SetActive(false);
    }
}
