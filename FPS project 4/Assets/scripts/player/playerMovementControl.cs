using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class playerMovementControl : MonoBehaviour
{
    [Header("Health")]
    public int maxHP;
    public int currentHP;
    public Color damageColor;
    private ProgressBar healthBar;
    private VisualElement damagePanel;
    [Header("movement")]
    public float moveSpeed = 8f;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;
    public int gunType;
    public GameObject cam;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.A;
    public KeyCode runKey = KeyCode.LeftShift;


    [Header("Air Check")]
    public float playerHeight;
    public float playerWidth;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    public float horizontalInput;
    public float verticalInput;
    Vector3 moveDir;

    public Transform orientation;
    public Camera playerCam;
    Rigidbody rb;

    public float sens;
    float xRotation, yRotation;

    public GameObject currentGun;
    public GameObject rifle, shotgun, pistol;

    public GameObject WeaponSpawner;

    public Transform handPosition;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        currentHP = maxHP;

        EquipRifle(-1);

        // R�cup�re le GameObject "PlayerUi" pour acc�der � l'UIDocument
        var playerUiObject = GameObject.Find("PlayerUI");
        if (playerUiObject != null)
        {
            var uiDocument = playerUiObject.GetComponent<UIDocument>();
            if (uiDocument != null)
            {
                var root = uiDocument.rootVisualElement;
                // Trouve la Progress Bar dans l'arbre de l'UI
                healthBar = root.Q<ProgressBar>("health-value");
                healthBar.highValue = maxHP;
                UpdateHealthBar();
                damagePanel = root.Q<VisualElement>("Panel");
            }
            else
            {
                Debug.LogError("UIDocument non trouv� dans PlayerUi.");
            }
        }
        else
        {
            Debug.LogError("GameObject 'PlayerUi' introuvable.");
        }
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight*0.5f +0.8f, whatIsGround);

        MyInput();
        SpeedControl();
        rb.drag = groundDrag;
        UpdateHealthBar();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        UpdateDamage();
    }
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHP;
        }
    }

    private void UpdateDamage()
    {
        if (damagePanel != null)
        {
            var color = damagePanel.style.borderBottomColor.value;
            color.a = Mathf.Max(0, color.a - Time.deltaTime);
            damagePanel.style.borderBottomColor = new StyleColor(color);
            damagePanel.style.borderTopColor = new StyleColor(color);
            damagePanel.style.borderLeftColor = new StyleColor(color);
            damagePanel.style.borderRightColor = new StyleColor(color);
        }
    }

    private void MyInput()
    {

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            StartCrouch();
        }
        if (Input.GetKeyUp(crouchKey))
        {
            EndCrouch();

        }
        if (Input.GetKeyDown(runKey))
        {
            StartRun();
        }
        if (Input.GetKeyUp(runKey))
        {
            EndRun();
        }
        if (currentGun != null && Input.GetKeyDown(KeyCode.F))
        {
            ThrowWeapon();
        }
    }
    private void MovePlayer()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDir() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        else if (grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded)
            rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        //rb.useGravity = !OnSlope();
    }

    private void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        playerCam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void StartCrouch()
    {
        moveSpeed /= 1.3f;
        rb.transform.localScale -= new Vector3(0, 0.05f, 0);
        jumpForce -= 5;
    }

    private void EndCrouch()
    {
        moveSpeed *= 1.3f;
        rb.transform.localScale += new Vector3(0, 0.05f, 0);
        jumpForce += 5;
    }
    private void StartRun()
    {
        moveSpeed *= 1.4f;
    }
    private void EndRun()
    {
        moveSpeed /= 1.4f;
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    public void EquipRifle(int bulletsLeft)
    {
        if (currentGun != null)
            Destroy(currentGun);

        currentGun = Instantiate(rifle, handPosition) as GameObject;
        currentGun.GetComponent<GenericWeaponControl>().giveBullets(bulletsLeft);
        currentGun.transform.localScale = new Vector3(17, 15, 17);
        currentGun.transform.Rotate(new Vector3(-6, 5.9f, 0.3f));
        currentGun.transform.position = handPosition.position;
        gunType = 0;
        currentGun.GetComponent<GenericWeaponControl>().isGunPlayer = true;
        currentGun.transform.localPosition += new Vector3(-4.4f, 3.4f, 12.2f);
    }

    public void EquipShotgun(int bulletsLeft)
    {
        if (currentGun != null)
            Destroy(currentGun);

        currentGun = Instantiate(shotgun, handPosition) as GameObject;
        currentGun.GetComponent<GenericWeaponControl>().isGunPlayer = true;
        currentGun.transform.Rotate(new Vector3(0, 0, -17f));
        currentGun.transform.position = handPosition.position;
        currentGun.transform.localPosition += new Vector3(-1.2f, -21.5f, 24.1f);
        currentGun.transform.localScale = new Vector3(17, 15, 17);
        gunType = 1;
    }

    public void EquipPistol(int bulletsLeft)
    {
        if (currentGun != null)
            Destroy(currentGun);

        currentGun = Instantiate(pistol, handPosition) as GameObject;
        currentGun.GetComponent<GenericWeaponControl>().isGunPlayer = true;
        currentGun.GetComponent<GenericWeaponControl>().giveBullets(bulletsLeft);
        currentGun.transform.position = handPosition.position;
        currentGun.transform.Rotate(new Vector3(-5, -2, 0));
        currentGun.transform.localPosition += new Vector3(-19, -3, -55);
        currentGun.transform.localScale = new Vector3(17, 15, 17);
        gunType = 2;

    }
    public void ThrowWeapon()
    {
        int bullets = currentGun.GetComponent<GenericWeaponControl>().bulletsInMag;
        if (currentGun != null) Destroy(currentGun);
        GameObject currentWeaponSpawner = Instantiate(WeaponSpawner) as GameObject;
        currentWeaponSpawner.transform.position = transform.position;
        currentWeaponSpawner.GetComponent<weaponSpawner>().instantiateGunType(gunType, bullets);
    }

    public void healFor(int heal)
    {
        currentHP += heal;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void getHit(int damage)
    {
        currentHP -= damage;
        damagePanel.style.borderBottomColor = damageColor;
        damagePanel.style.borderTopColor = damageColor;
        damagePanel.style.borderLeftColor = damageColor;
        damagePanel.style.borderRightColor = damageColor;
        if (currentHP <= 0)
        {
            loose();
        }
    }

    private void loose()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
