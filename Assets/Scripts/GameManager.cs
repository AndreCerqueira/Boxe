using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //-- variaveis cronometro --\\
    public GameObject matchTimer;
    public GameObject matchRound;
    public GameObject roundMenuPause;
    private float currentTimer;
    private float roundCount = 1;
    private int secondsCont = 60;
    private int minCont = 2;
    public bool inGame;
    public bool pauseGame;
    private float resetTime;

    //-- variaveis UI AI --\\
    public GameObject msgTIME;
    public GameObject msgKO;
    public GameObject luvas;
    public GameObject fimRound;

    //-- variaveis Players --\\
    public GameObject player;
    public GameObject enemy;

    //-- variaveis queda --\\
    private float currentQuedaTime;

    void Start()
    {
 
    }

    void Update()
    {

        cronometro();

        KOcheck();

    }
    
    void cronometro()
    {
        if (inGame & pauseGame == false)
        {
            currentTimer += Time.deltaTime;

            if (0.25f < currentTimer)
            {
                secondsCont -= 1;
                matchTimer.GetComponent<Text>().text = "0" + minCont + ":" + secondsCont;
                currentTimer = 0;

                if (secondsCont < 10)
                {                                                         // V \\
                    matchTimer.GetComponent<Text>().text = "0" + minCont + ":0" + secondsCont;
                }
            }

            if (secondsCont == 0)
            {
                minCont -= 1;
                secondsCont = 60;
            }
            // se acabou o tempo:
            if (minCont < 0)
            {
                roundCount += 1;
                roundMenuPause.GetComponent<Text>().text = " Fim do " + (roundCount - 1) + "º Round";
                player.GetComponent<Player>().Anim.SetBool("isWalk", true);
                enemy.GetComponent<Enemy>().Anim.SetBool("isWalk", true);
                pauseGame = true;
                msgTIME.SetActive(true);
            }
        }
        else
        {
            if (pauseGame)
            {
                fimRound.SetActive(true);

                float distPlayerConer = player.GetComponent<Transform>().position.x; //-4.75f

                if (distPlayerConer < -4.75f)
                {
                    player.GetComponent<Player>().ResetAnimations();
                    player.GetComponent<Player>().Anim.SetBool("isWalk", false);
                    player.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {                    
                    player.GetComponent<SpriteRenderer>().flipX = true;
                }

                float distEnemyConer = enemy.GetComponent<Transform>().position.x;   //4.75f

                if (distEnemyConer > 4.75f)
                {
                    enemy.GetComponent<Enemy>().ResetAnimations();
                    enemy.GetComponent<Enemy>().Anim.SetBool("isWalk", false);
                    enemy.GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    enemy.GetComponent<SpriteRenderer>().flipX = false;
                }

                

                player.GetComponent<Player>().rigidbody.velocity = new Vector2(enemy.GetComponent<Enemy>().movimento * enemy.GetComponent<Enemy>().VelMax * 1, enemy.GetComponent<Enemy>().rigidbody.velocity.y);
                enemy.GetComponent<Enemy>().rigidbody.velocity = new Vector2(enemy.GetComponent<Enemy>().movimento * enemy.GetComponent<Enemy>().VelMax * -1, enemy.GetComponent<Enemy>().rigidbody.velocity.y);
            }
        }

    }

    void KOcheck()
    {

        if (player.GetComponent<Player>().vida.value <= 0)
        {
          
            player.GetComponent<Player>().Anim.SetBool("isKO", true);
            enemy.GetComponent<Enemy>().Anim.SetBool("idle", true);

            if (0.5f < currentQuedaTime)
            {
                player.GetComponent<Player>().Anim.SetBool("isDeitado", true);
                currentQuedaTime = 0;
            }
            else
            {
                currentQuedaTime += Time.deltaTime;
            }

            msgKO.SetActive(true);
            inGame = false;
        }

        if (enemy.GetComponent<Enemy>().vida.value <= 0)
        {
            enemy.GetComponent<Enemy>().Anim.SetBool("isKO", true);
            player.GetComponent<Player>().Anim.SetBool("idle", true);

            if (0.5f < currentQuedaTime)
            {
                enemy.GetComponent<Enemy>().Anim.SetBool("isDeitado", true);
                currentQuedaTime = 0;
            }
            else
            {
                currentQuedaTime += Time.deltaTime;
            }

            msgKO.SetActive(true);
            inGame = false;

        }

    }

    public void jogar()
    {
        inGame = true;
        SceneManager.LoadScene("JogarScene");
    }

    public void sair()
    {
        Application.Quit();
    }

    public void voltar()
    {
        inGame = false;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void proxRound()
    {
        if (player.GetComponent<SpriteRenderer>().flipX == false & enemy.GetComponent<SpriteRenderer>().flipX == true)
        {
            matchRound.GetComponent<Text>().text = "ROUND " + roundCount;

            fimRound.SetActive(false);
            pauseGame = false;
            msgTIME.SetActive(false);
            minCont = 2;
            secondsCont = 60;
            matchTimer.GetComponent<Text>().text = "0" + minCont + ":" + secondsCont;
        }
    }

    public void controlos()
    {
        SceneManager.LoadScene("MenuControlos");
    }

    public void luvasDown()
    {
        luvas.GetComponent<Rigidbody2D>().gravityScale = 1;
    }

}
