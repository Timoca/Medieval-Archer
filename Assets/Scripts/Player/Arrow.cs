using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float speed;

    // Stel de richting en snelheid in
    public void SetDirection(Vector2 arrowDirection, float arrowSpeed)
    {
        speed = arrowSpeed;

        // Draai de pijl zodat deze in de richting beweegt
        float angle = Mathf.Atan2(arrowDirection.y, arrowDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Update()
    {
        // Beweeg de pijl elke frame in de lokale rechterrichting
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Voeg hier logica toe voor wat er gebeurt bij botsing, bijvoorbeeld:
        // - Degenereer schade aan vijanden
        // - Vernietig de pijl
        // Voor nu vernietigen we de pijl bij elke botsing
        Destroy(gameObject);
    }

    void Start()
    {
        // Vernietig de pijl na 5 seconden om te voorkomen dat pijlen in de scene blijven hangen
        Destroy(gameObject, 5f);
    }
}