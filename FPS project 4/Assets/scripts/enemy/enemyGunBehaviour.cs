using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class enemyGunBehaviour : MonoBehaviour
{
    public int maxHP = 160;
    public int currentHP;

    public GameObject currentGun;
    public GameObject rifle, shotgun;
    public GameObject deathExplosion;
    public GameObject hitMarker;
    public AudioClip explosionSound;
    public GameObject spawner;

    public Transform handPosition;
    // Start is called before the first frame update
    void Start()
    {
        int randomNumber = Random.Range(0, 2);
        switch (randomNumber)
        {
            case 0:
                currentGun = Instantiate(rifle, handPosition) as GameObject;
                currentGun.transform.localScale = new Vector3(10, 10, 10);
                currentGun.transform.Rotate(new Vector3(41.8f, -81.3f, -93));
                currentGun.transform.localPosition += new Vector3(2.9f, -6.2f, -0.3f);
                break;
            case 1:
                currentGun = Instantiate(shotgun, handPosition) as GameObject;
                currentGun.transform.localScale = new Vector3(10,10,10);
                currentGun.transform.Rotate(new Vector3(44.3f, -82f, -85));
                currentGun.transform.localPosition += new Vector3(7.7f, 7.6f, -1.1f);
                break;
        }
        currentGun.GetComponent<GenericWeaponControl>().isGunEnemy = true;
        currentGun.transform.position = handPosition.position;
        currentHP = maxHP;
    }



    private void Update()
    {
        if (currentHP <= 0)
        {
            Destroy(currentGun);
            Vector3 deathPos = gameObject.transform.position;
            Destroy(gameObject);
            GameObject instExplosion = Instantiate(deathExplosion) as GameObject;
            deathPos.y += 0.3f;
            instExplosion.transform.position = deathPos;
            AudioSource.PlayClipAtPoint(explosionSound, gameObject.transform.position);
            Destroy(instExplosion, 0.4f);
            int deathRoulette = Random.Range(0, 5);
            if (deathRoulette > 2) { 
                GameObject weaponSpawner = Instantiate(spawner) as GameObject;
                deathPos.y += 0.3f;
                weaponSpawner.transform.position = deathPos;
            }
        }
    }
    public void getHit(int damage, Vector3 position)
    {
        currentHP -= damage;

        GameObject hitMark = Instantiate(hitMarker) as GameObject;
        position.y += 0.5f;
        hitMark.transform.position = position;
        hitMark.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        Destroy(hitMark, 0.3f);
    }
}
