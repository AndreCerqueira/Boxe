using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontalAtkRange : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            player.GetComponent<Player>().FrontalAtkZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            player.GetComponent<Player>().FrontalAtkZone = false;
        }
    }
}
