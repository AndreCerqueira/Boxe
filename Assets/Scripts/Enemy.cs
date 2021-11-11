using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //-- variaveis Animaçoes --\\
    public Animator Anim;
    private bool run;
    private bool jab;
    private bool direita; 
    private bool esquiva;
    private bool hurt;

    //-- variaveis Movimento --\\
    public float VelMax;
    public float movimento;

    //-- variaveis bars vida/mana --\\
    public Slider vida;
    public Slider energia;
    public Slider dashBar;

    //-- variaveis UI IA --\\
    public Rigidbody2D rigidbody;
    public GameObject enemy;
    public GameObject gameManager;
    public GameObject confusion;

    //-- variaveis combate --\\
    public float reactionTime; 
    public float stunnedTime;     
    public float currentHurtTime; // <---------------
    public float jabDamage;
    private int jabCount;
    private int startJabCount = 3;
    private float timeBtwAttack;
    public bool fightZone;

    void Start()
    {
        movimento = -1;
    }

    void Update()
    {

        if (gameManager.GetComponent<GameManager>().inGame & gameManager.GetComponent<GameManager>().pauseGame == false)
        {
            Timers();

            Movement();       

            TakeDamage();

            SetAnimations();
        }
        else
        {
            confusion.SetActive(false);
            Anim.SetBool("isRunning", false);
            //Anim.SetBool("Dash", false);
            Anim.SetBool("Skill_1", false);
        }        

    }

    void Timers()
    {
        dashBar.value = Mathf.Lerp(dashBar.value, dashBar.value + 0.20f, 0.5f * Time.deltaTime);

        if (hurt)
        {
            if (0.4f < currentHurtTime)
            {
                hurt = false;
                currentHurtTime = 0;
            }
            else
            {
                currentHurtTime += Time.deltaTime;
            }
        }
        
    }

    void Movement()
    { 
        run = false;

        //-- RECEBER VALORES DE MOVIMENTO --\\        

        if (enemy.GetComponent<Transform>().position.x < transform.position.x)
        {
            if (enemy.GetComponent<Player>().stunnedValue < stunnedTime)
            {
                confusion.SetActive(false);

                float distance = transform.position.x - enemy.GetComponent<Transform>().position.x;

                // DISTANCIA NESSESÁRIA PARA O SOCO ACERTAR
                if (distance <= 1.3f)
                {
                    // SE TIVER A SER ATACADO NAO PODE ATACAR
                    if (hurt == false)
                    {
                        // TEMPO QUE DEMORA PARA O PRIMEIRO SOCO
                        if (0.5f < reactionTime)
                        {
                            // ELE DA 1-3 JABS
                            if (jabCount < startJabCount)
                            {
                                //------------------------JAB--------------------------\\                     
                                direita = false;
                                jab = true;
                                enemy.GetComponent<Player>().hurt = true;

                                if (0.5f < timeBtwAttack & jab)
                                {
                                    jabCount += 1;
                                    if (enemy.GetComponent<Player>().esquiva == false)
                                    {
                                        enemy.GetComponent<Player>().vida.value -= jabDamage;
                                    }
                                    energia.value -= 0.01f;
                                    timeBtwAttack = 0;
                                }
                                else
                                {
                                    timeBtwAttack += Time.deltaTime;
                                }
                            }
                            // ELE DA 1 DIREITA
                            else
                            {
                                //----------------------DIREITA------------------------\\
                                jab = false;
                                direita = true;
                                enemy.GetComponent<Player>().hurt = true;

                                if (0.5f < timeBtwAttack & direita)
                                {
                                    jabCount = 0;
                                    startJabCount = Random.Range(1, 4);
                                    if (enemy.GetComponent<Player>().esquiva == false)
                                    {
                                        enemy.GetComponent<Player>().vida.value -= jabDamage * 2;
                                    }
                                    energia.value -= 0.01f * 2;
                                    timeBtwAttack = 0;

                                }
                                else
                                {
                                    timeBtwAttack += Time.deltaTime;
                                }
                            }

                        }
                        // ESPERAR OS 0.5 SEC TEMPO DE REAÇÃO
                        else
                        {
                            reactionTime += Time.deltaTime;
                            run = false;
                        }
                    }
                    // SE ESTA A LEVAR DANO
                    else
                    {
                        reactionTime = 0;
                    }
                }
                else
                {
                    jab = false;
                    direita = false;
                    enemy.GetComponent<Player>().hurt = false;

                    run = true;
                    reactionTime = 0;

                    if (hurt)
                    {
                        run = false;
                    }
                    else
                    {
                        rigidbody.velocity = new Vector2(movimento * VelMax, rigidbody.velocity.y);
                    }
                }
            }
            else
            {
                jab = false;
                direita = false;
                enemy.GetComponent<Player>().hurt = false;
                reactionTime = 0;

                if (stunnedTime != 0)
                {
                    confusion.SetActive(true);
                }                
                
                stunnedTime += Time.deltaTime;
            }

        }

        GetComponent<Rigidbody2D>().velocity = rigidbody.velocity;

    }

    void SetAnimations()
    {
        Anim.SetBool("isFtWork", run);
        //Anim.SetBool("Dash", isDashing);
        Anim.SetBool("isJab", jab);
        Anim.SetBool("isDireita", direita);
        Anim.SetBool("isEsquiva", esquiva);
        Anim.SetBool("isHurt", hurt);

    }

    public void ResetAnimations()
    {
        run = false;
        jab = false;
        direita = false;
        esquiva = false;
        hurt = false;

        SetAnimations();
    }

    public void TakeDamage()
    {
        if (enemy.GetComponent<Player>().FrontalAtkZone || enemy.GetComponent<Player>().UpperAtkZone)
        {
            if (enemy.GetComponent<Player>().damage != 0)
            {
                hurt = true;
                stunnedTime = 0;              

                vida.value -= enemy.GetComponent<Player>().damage;
                enemy.GetComponent<Player>().damage = 0;
            }

            rigidbody.AddForce(new Vector2(enemy.GetComponent<Player>().KnockForce, 0));
            enemy.GetComponent<Player>().KnockForce = 0;
            
        }

    }
}
