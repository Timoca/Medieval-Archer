using UnityEngine;
using UnityEngine.InputSystem;

public class BowController : MonoBehaviour
{
    [Header("Bow Settings")]
    [SerializeField] float fixedDistance = 1f;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        if (playerTransform == null)
        {
            Debug.LogError("BowController: Geen parent gevonden. Zorg ervoor dat de Bow een kind is van de speler.");
        }
    }

    void Update()
    {
        UpdateBowPosition();
        RotateBowTowardsMouse();
    }

    private void UpdateBowPosition()
    {
        // Krijg de muispositie in wereldruimte
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f; // Zorg ervoor dat z=0 voor 2D

        // Bereken de richting van de speler naar de muis
        Vector3 direction = (mousePosition - playerTransform.position).normalized;

        // Stel de lokale positie van de bow in op de vaste afstand in de richting van de muis
        Vector3 newLocalPosition = playerTransform.position + direction * fixedDistance;
        newLocalPosition.z = 0f; // Zorg ervoor dat z=0 blijft

        transform.localPosition = newLocalPosition;
    }

    private void RotateBowTowardsMouse()
    {
        // Krijg de muispositie in wereldruimte
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f; // Zorg ervoor dat z=0 voor 2D

        // Bereken de richting van de bow naar de muis
        Vector3 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Draai de bow naar de muispositie
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}