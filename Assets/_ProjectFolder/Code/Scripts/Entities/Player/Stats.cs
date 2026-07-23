using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("Salud")]
    public int health = 100;

    [Header("Configuración Knockback")]
    private Rigidbody2D rb;

    private void Awake()
    {
       
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (health <= 0)
        {
            Debug.Log("Player is dead");
          
        }
    }

    //daño y nockbackback
    public void TomarDano(int cantidadDano, Vector2 direccionGolpe, float fuerzaKnockback)
    {
      
        health -= cantidadDano;
        Debug.Log("Player hit! Health is now: " + health);

       
        if (rb != null)
        {
           
            rb.linearVelocity = Vector2.zero;

          
            rb.AddForce(direccionGolpe * fuerzaKnockback, ForceMode2D.Impulse);
        }
    }
}