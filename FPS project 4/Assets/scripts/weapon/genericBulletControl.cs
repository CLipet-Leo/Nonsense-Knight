using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class genericBulletControl : MonoBehaviour
{
    public int damage;
    public void OnCollisionEnter(Collision collision)
    { 
        if (collision.gameObject.tag == "enemy")
        {
            collision.gameObject.GetComponent<enemyGunBehaviour>().getHit(damage, collision.transform.position);

        }
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<playerMovementControl>().getHit(damage);
        }
        if(collision.gameObject.tag != "bullet")
        {
            Destroy(gameObject);

        }
    }

}
