using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActionAsset;

    private Player player;
    private InputAction moveAction;
    private Animator animator;

    private Vector2 movementInput;
    public bool isMoving;

    private void Awake()
    {
        // Zoek en activeer de Move actie
        moveAction = inputActionAsset.FindActionMap("Player").FindAction("Movement");

        moveAction.Enable();

        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Lees de beweging input
        movementInput = moveAction.ReadValue<Vector2>();
        isMoving = movementInput.magnitude > 0;

        // Stel de Animator parameter in
        animator.SetBool("isRunning", isMoving);

        // Beweeg de speler
        MovePlayer();

        // Behandel flip op basis van muispositie
        HandleFlip();
    }

    private void MovePlayer()
    {
        // Bereken de nieuwe positie
        Vector3 newPosition = transform.position + new Vector3(movementInput.x, movementInput.y, 0) * player.speed * Time.deltaTime;
        transform.position = newPosition;
    }

    private void HandleFlip()
    {
        // Haal de muispositie in wereldruimte op
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPosition.z = transform.position.z; // Zorg dat de z-positie gelijk blijft

        // Bepaal of de muis links of rechts van de speler is
        if (mouseWorldPosition.x < transform.position.x)
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

    void OnDestroy()
    {
        // Zorg ervoor dat de input handler wordt uitgeschakeld
        moveAction.Disable();
    }
}