using UnityEditor;
using UnityEngine;


public class GenericWeaponControl : MonoBehaviour
{
    //public Transform playerPos;
    public int damage;

    public bool isGunPlayer, isGunEnemy;

    public GameObject bullet;

    public float shootForce, upwardForce;

    public float timeBtShooting, spread, reloadTime;

    public int magSize, bulletPerTap;

    public bool isFullAuto;

    public int bulletsInMag, bulletsShot;

    private bool isAssigned = false;



    public bool shooting, reloading, readyToShoot;

    private Camera fpsCam;

    public Transform attackPoint;

    public bool allowInvoke = true;


    public GameObject muzzleFlash;
    public AudioClip bulletNoise;

    private float volume = 0.2f;


    private void Awake()
    {
        
        readyToShoot = true;
        if (isGunPlayer)
        {
            fpsCam = Camera.main;
        }
        giveBullets(-1);

    }


    public void giveBullets(int bullets)
    {
        if (bullets >= 0 && bullets < magSize)
        {
            bulletsInMag = bullets;
            
        } else
        {
            bulletsInMag = magSize;
        }
    }

    private void Update()
    {
        if (!isAssigned)
        {
            isAssigned = true;
            if (isGunPlayer)
            {
                if (fpsCam == null)
                {
                    fpsCam = Camera.main;
                }
            }
            if (isGunEnemy)
            {
                spread += 5;
            }
        }
        if (isGunPlayer)
        {

            playerInput();
        }
        if (isGunEnemy)
        {
            enemyInput();
        }
        
    }

    private void playerInput()
    {
        if (isFullAuto) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if(readyToShoot && shooting && !reloading && bulletsInMag > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && bulletsInMag < magSize && !reloading) reload();
        if (readyToShoot && shooting && bulletsInMag <= 0 && !reloading) reload();

    }

    private void enemyInput()
    {
        if (readyToShoot && shooting && !reloading && bulletsInMag > 0)
        {
            bulletsShot = 0;
            Shoot();
        }
        if (readyToShoot && shooting && bulletsInMag <= 0 && !reloading) reload();

    }


    private void Shoot()
    {
        readyToShoot  = false;
        bulletsShot++;
        Vector3 targetPoint;


        if (isGunPlayer)
        {
            Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            targetPoint = ray.GetPoint(75);
        } else
        {
            Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.position - attackPoint.position;
            Ray ray = new Ray(attackPoint.position, direction.normalized);
            targetPoint = ray.GetPoint(75);
        }
        

        //calc direction with the gun and the target

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;


        //if spread is there( for shotgun) calculate spread

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread+ new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity) as GameObject;

        currentBullet.transform.forward = directionWithSpread;
        currentBullet.GetComponent<genericBulletControl>().damage = damage;
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized*shootForce, ForceMode.Impulse);
        Destroy(currentBullet, 2);        


        if(muzzleFlash != null)
        {
            GameObject mFlash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
            mFlash.GetComponent<muzzleFlashRender>().posOfMuzzle = attackPoint;
            Destroy(mFlash, 0.075f);
        }

        if (isGunPlayer)
        {
            Invoke(nameof(resetShot), timeBtShooting);
        } else
        {
            Invoke(nameof(resetShot), timeBtShooting*2);
        }
        


        if (bulletsShot < bulletPerTap)
        {
            Shoot();
        } else
        {
            bulletsInMag--;
        
            AudioSource.PlayClipAtPoint(bulletNoise, gameObject.transform.position, volume);
        }

    }

    private void resetShot()
    {
        readyToShoot = true;

    }
    private void reload()
    {
        reloading = true;
        if (isGunPlayer)
        {
            Invoke(nameof(endReload), reloadTime);
        }
        else
        {
            Invoke(nameof(endReload), reloadTime*2);
        }

    }
    private void endReload()
    {
        reloading = false;
        bulletsInMag = magSize;
    }


}
