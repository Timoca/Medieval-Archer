using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionAsset;

    [Header("Shooting Settings")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;

    private Player player;
    private Animator animator;
    private InputAction shootAction;
    private float lastShootTime;
    private bool isShooting = false; // We kijken of de schietknop is ingedrukt

    void Start()
    {
        player = GetComponent<Player>();

        // Initialiseer de Input Action voor schieten
        shootAction = inputActionAsset.FindActionMap("Player").FindAction("Fire");
        shootAction.Enable();

        // Abonneer op de started en canceled events
        shootAction.started += OnShootStarted;
        shootAction.canceled += OnShootCanceled;

        animator = arrowSpawnPoint.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component niet gevonden op " + gameObject.name);
        }
    }

    void Update()
    {
        // Controleer of de schietactie is geactiveerd en of de cooldown voorbij is
        if (isShooting && Time.time >= lastShootTime + player.shootCooldown)
        {
            StartCoroutine(ShootArrowCoroutine());
            lastShootTime = Time.time;
        }
    }

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        isShooting = true;
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        isShooting = false;
    }

    private IEnumerator ShootArrowCoroutine()
    {
        // Trigger de schietanimatie
        animator.SetTrigger("Shoot_Bow");

        // Wacht voor de gewenste vertraging
        yield return new WaitForSeconds(player.shootDelay);

        // Instantieer de pijl
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        // Stel de pijlrichting in
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0f; // Zorg ervoor dat z=0 voor 2D
            Vector3 direction = (mousePosition - arrowSpawnPoint.position).normalized;
            arrowScript.SetDirection(direction, player.arrowSpeed);
        }

        // Speel het schietgeluid af
        AudioManager.Instance.PlaySFX(player.shootClip);

        // Optioneel: Flip de speler als de muis aan de linkerkant is
        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        currentMousePosition.z = 0f;
        HandleFlip(currentMousePosition);
    }

    private void HandleFlip(Vector3 mousePosition)
    {
        if (mousePosition.x < transform.position.x)
        {
            // Muis is links van de speler - flip naar links
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * -1; // Zorg dat x negatief is
            transform.localScale = localScale;
        }
        else
        {
            // Muis is rechts van de speler - flip naar rechts
            Vector3 localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x); // Zorg dat x positief is
            transform.localScale = localScale;
        }
    }

    private void OnDestroy()
    {
        // Zorg ervoor dat we de events loskoppelen wanneer het object vernietigd wordt
        if (shootAction != null)
        {
            shootAction.started -= OnShootStarted;
            shootAction.canceled -= OnShootCanceled;
        }
    }
}