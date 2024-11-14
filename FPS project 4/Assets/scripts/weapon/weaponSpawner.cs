using UnityEngine;

public class weaponSpawner : MonoBehaviour
{
    public int gunType;
    public GameObject Player;
    private bool canPlayerGrabGun = false;
    public int bulletsLeft = -1;
    public AudioClip dingSound;

    public void Awake()
    {
        gunType = Random.Range(0, 3);
    }


    public void instantiateGunType(int gun, int bullets)
    {
        gunType = gun;
        bulletsLeft = bullets;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canPlayerGrabGun = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canPlayerGrabGun = false;

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canPlayerGrabGun)
        {
            Player = GameObject.FindWithTag("Player");
            switch (gunType)
            {
                case 0:
                    Player.gameObject.GetComponent<playerMovementControl>().EquipRifle(bulletsLeft);
                    break;
                case 1:
                    Player.gameObject.GetComponent<playerMovementControl>().EquipShotgun(bulletsLeft);
                    break;
                case 2:
                    Player.gameObject.GetComponent<playerMovementControl>().EquipPistol(bulletsLeft);
                    break;
            }
            AudioSource.PlayClipAtPoint(dingSound, gameObject.transform.position, 1.5f);
            Destroy(gameObject);
        }
    }
}
