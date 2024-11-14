using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

public class healthKit : MonoBehaviour
{
    private bool canHeal;
    private GameObject Player;
    private Vector3 position;
    public int healCooldown;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canHeal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canHeal = false;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canHeal)
        {
            Player = GameObject.FindWithTag("Player");
            Invoke(nameof(activate), healCooldown);
            Player.GetComponent<playerMovementControl>().healFor(200);      
            position = transform.position;
            position.y -= 15;
            gameObject.transform.position = (position);
        }
    }
    private void activate()
    {
        position = transform.position;
        position.y += 15;
        gameObject.transform.position = (position);
    }
}
