using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //-- variaveis Animaçoes --\\
    public Animator Anim;
    private bool run;
    private bool jab;
    private bool acertouJab;
    private bool direita;
    private bool acertouDireita;
    public bool esquiva;
    private bool isDashing;
    public bool hurt;

    private bool upperEsq;
    private bool upperDir;

    //-- variaveis Timers Animaçoes --\\
    public float startPunchTime;
    public float startEsquivaTime;
    private float currentAnimTime;
    private float currentUpperTime;

    //-- variaveis Movimento --\\
    public float VelMax;
    public float movimento;

    //-- variaveis dash--\\
    public float ImpluseForce;
    private int leftClickCount;
    private int rightClickCount;

    private float currentDoubleClickTime;
    private bool activateDoubleClickTimer;

    private float currentDashTime;

    //-- variaveis bars vida/mana --\\
    public Slider vida;
    public Slider energia;
    public Slider dashBar;

    //-- variaveis regen --\\
    private float currentRegenTime;

    //-- variaveis golpes --\\
    private float[] timeBtwAttack = new float[5];
    public bool FrontalAtkZone;
    public bool UpperAtkZone;

    //-- KnockBack damage --\\
    public float KnockForce;
    public float damage;
    private float fatigueDamage = 1;
    public float stunnedValue;

    //-- variaveis UI IA --\\
    public Rigidbody2D rigidbody;
    public GameObject enemy;
    public GameObject gameManager;
    public GameObject confusion;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (gameManager.GetComponent<GameManager>().inGame & gameManager.GetComponent<GameManager>().pauseGame == false)
        {
            Regen();
            Timers();

            Movement();

            Dashes();

            SetAnimations();

            Golpes();

            DamagePerEnergy();
        }
        else
        {
            confusion.SetActive(false);
            Anim.SetBool("isRunning", false);
            Anim.SetBool("Dash", false);
            Anim.SetBool("Skill_1", false);
        }   
        
    }

    void Movement()
    {
        //---------------------------------------------- MOVIMENTO HORIZONTAL ---------------------------------------------\\

        run = false;

        //-- RECEBER VALORES DE MOVIMENTO --\\
        movimento = Input.GetAxis("Horizontal");

        if (hurt)
        {
            rigidbody.velocity = new Vector2((movimento * VelMax) / 2, rigidbody.velocity.y);
        }
        else
        {
            rigidbody.velocity = new Vector2(movimento * VelMax, rigidbody.velocity.y);
        }
        

        GetComponent<Rigidbody2D>().velocity = rigidbody.velocity;

        //-- ----------------------------- --\\

        // verificar se esta a correr e virar 
        if (movimento < 0)
        {
            run = true;
            //GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (movimento > 0)
        {
            run = true;
            //GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            run = false;
        }
       
    }

    void Timers()
    {

        dashBar.value = Mathf.Lerp(dashBar.value, dashBar.value + 0.20f, 0.5f * Time.deltaTime);

        //------------ Timers Relacionados ao DASH ------------\\
        if (activateDoubleClickTimer)
        {
            currentDoubleClickTime += Time.deltaTime;

            if (0.2f < currentDoubleClickTime)
            {
                rightClickCount = 0;
                leftClickCount = 0;
                currentDoubleClickTime = 0;
                activateDoubleClickTimer = false;
            }
        }
      
        if (isDashing)
        {
            currentDashTime += Time.deltaTime;

            if (0.3f < currentDashTime)
            {
                dashBar.value = 0;
                isDashing = false;
                currentDashTime = 0;
            }
        }

        //------------ Timers Relacionados aos GOLPES ------------\\
        if (jab)
        {
            currentAnimTime += Time.deltaTime;

            if (acertouJab & 0.2f < currentAnimTime)
            {               
                damage = GetComponent<Skills>().jabDamage / fatigueDamage;
                KnockForce = GetComponent<Skills>().jabKnockback;
                stunnedValue = GetComponent<Skills>().jabStunned;
                acertouJab = false;
            }
            else
            {
                if (startPunchTime < currentAnimTime)
                {
                    jab = false;
                    currentAnimTime = 0;
                }
            }
        }

        if (direita)
        {
            currentAnimTime += Time.deltaTime;

            if (acertouDireita & 0.2f < currentAnimTime)
            {
                damage = GetComponent<Skills>().dirDamage / fatigueDamage;
                KnockForce = GetComponent<Skills>().dirKnockback;
                stunnedValue = GetComponent<Skills>().dirStunned;
                acertouDireita = false;
            }
            else
            {
                if (startPunchTime < currentAnimTime)
                {
                    direita = false;
                    currentAnimTime = 0;
                }
            }
            
        }

        if (esquiva)
        {
            currentAnimTime += Time.deltaTime;

            if (startEsquivaTime < currentAnimTime)
            {
                esquiva = false;
                currentAnimTime = 0;
            }
        }

        //------------ Timers Relacionados aos UPPERS ------------\\
        if (upperEsq)
        {
            if (0.3f < currentUpperTime)
            {
                damage = GetComponent<Skills>().upperEsqDamage / fatigueDamage;
                KnockForce = GetComponent<Skills>().upperEsqKnockback;
                stunnedValue = GetComponent<Skills>().upperEsqStunned;
                upperEsq = false;
                currentUpperTime = 0;
            }
            else
            {
                currentUpperTime += Time.deltaTime;
            }           
        }

        if (upperDir)
        {
            if (0.3f < currentUpperTime)
            {
                damage = GetComponent<Skills>().upperDirDamage / fatigueDamage;
                KnockForce = GetComponent<Skills>().upperEsqKnockback;
                stunnedValue = GetComponent<Skills>().upperDirStunned;
                upperDir = false;
                currentUpperTime = 0;
            }
            else
            {
                currentUpperTime += Time.deltaTime;
            }
        }

    }

    void Regen()
    {
        currentRegenTime += Time.deltaTime;

        if (5 < currentRegenTime)
        {
            vida.value += 0.01f;
            energia.value += 0.01f;
            currentRegenTime = 0;
        }
    }

    void Dashes()
    {
        //--------------------------------------------------- DASHES --------------------------------------------\\   

        if (dashBar.value == 1f)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                activateDoubleClickTimer = true;
                leftClickCount += 1;
                rightClickCount = 0;

                //-- ATIVAR O DASH PARA A ESQUERDA --\\
                if (leftClickCount >= 2)
                {
                    isDashing = true;
                    leftClickCount = 0;
                    rigidbody.AddForce(new Vector2(ImpluseForce * -1, 0));
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                activateDoubleClickTimer = true;
                rightClickCount += 1;
                leftClickCount = 0;

                //-- ATIVAR O DASH PARA A DIREITA --\\
                if (rightClickCount >= 2)
                {
                    isDashing = true;
                    rightClickCount = 0;
                    rigidbody.AddForce(new Vector2(ImpluseForce, 0));
                }
            }
        }
    }

    void SetAnimations()
    {
        Anim.SetBool("isFtWork", run);
        Anim.SetBool("isDashing", isDashing);
        Anim.SetBool("isJab", jab);
        Anim.SetBool("isDireita", direita);
        Anim.SetBool("isEsquiva", esquiva);
        Anim.SetBool("isHurt", hurt);
    }

    public void ResetAnimations()
    {
        run = false;
        isDashing = false;
        jab = false;
        direita = false;
        esquiva = false;
        hurt = false;

        SetAnimations();
    }

    void Golpes()
    {
 
        if (hurt == false)
        {
            //--------------------------- JAB -------------------------\\
            if (GetComponent<Skills>().jabColldown < timeBtwAttack[0])
            {               
                if (Input.GetKeyDown(KeyCode.E))
                {
                    energia.value -= GetComponent<Skills>().jabEnergyCost;
                    jab = true;

                    for (int i = 0; i < timeBtwAttack.Length; i++)
                    {
                        timeBtwAttack[i] = 0;
                    }

                    if (FrontalAtkZone)
                    {
                        acertouJab = true;
                        // Coloquei no fim da animaçao
                        //damage = GetComponent<Skills>().jabDamage;
                        //KnockForce = GetComponent<Skills>().jabKnockback;
                    }
                }
            }

            //---------------------- DIREITA --------------------------\\
            if (GetComponent<Skills>().dirColldown < timeBtwAttack[1])
            {     
                if (Input.GetKeyDown(KeyCode.W))
                {
                    energia.value -= GetComponent<Skills>().dirEnergyCost;
                    direita = true;

                    for (int i = 0; i < timeBtwAttack.Length; i++)
                    {
                        timeBtwAttack[i] = 0;
                    }

                    if (FrontalAtkZone)
                    {
                        acertouDireita = true;
                        // Coloquei no fim da animaçao
                        //damage = GetComponent<Skills>().dirDamage;
                        //KnockForce = GetComponent<Skills>().dirKnockback;
                    }
                }
            }
        }

        //----------------------- ESQUIVA -----------------------------\\
        if (GetComponent<Skills>().esquivaColldown < timeBtwAttack[2])
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
            energia.value -= GetComponent<Skills>().esquivaEnergyCost;
            esquiva = true;
  
            timeBtwAttack[0] = 0;
            timeBtwAttack[1] = 0;
            timeBtwAttack[2] = 0;

            }
        }

        //------------ JAB COM ESQUIVA | UPPER ESQUERDA ---------------\\
        if (GetComponent<Skills>().upperEsqColldown < timeBtwAttack[3] & esquiva)
        {
            if (esquiva & Input.GetKeyDown(KeyCode.E))
            {
                energia.value -= GetComponent<Skills>().upperEsqEnergyCost;
                jab = true;

                for (int i = 0; i < timeBtwAttack.Length; i++)
                {
                    timeBtwAttack[i] = 0;
                }

                if (UpperAtkZone)
                {                   
                    upperEsq = true;
                    // Coloquei no fim da animaçao
                    //damage = GetComponent<Skills>().upperEsqDamage;
                    //KnockForce = GetComponent<Skills>().upperEsqKnockback;
                }
            }
        }

        //------------ DIREITA COM ESQUIVA | UPPER DIREITA ------------\\
        if (GetComponent<Skills>().upperDirColldown < timeBtwAttack[4] & esquiva)
        {
            if (esquiva & Input.GetKeyDown(KeyCode.W))
            {
                energia.value -= GetComponent<Skills>().upperDirEnergyCost;
                direita = true;

                for (int i = 0; i < timeBtwAttack.Length; i++)
                {
                    timeBtwAttack[i] = 0;
                }

                if (UpperAtkZone)
                {
                    upperDir = true;
                    // Coloquei no fim da animaçao
                    // damage = GetComponent<Skills>().upperDirDamage;
                    //KnockForce = GetComponent<Skills>().upperDirKnockback;
                }
            }
        }


        for (int i = 0; i < timeBtwAttack.Length; i++)
        {
            timeBtwAttack[i] += Time.deltaTime;
        }

    }

    void DamagePerEnergy()
    {
        if (energia.value >= 0.75f)
        {
            fatigueDamage = 1;
        }
        else
        {
            if (energia.value >= 0.5f)
            {
                fatigueDamage = 1.25f;
            }
            else
            {
                if (energia.value >= 0.25f)
                {
                    fatigueDamage = 1.5f;
                }
                else
                {
                    // energia -> 0
                    fatigueDamage = 1.75f;
                }
            }
        }
    }

}


