using UnityEngine;

namespace Entity.Controller
{
    public class Attack : MonoBehaviour
    {
        [Header("Configuración de Ataque")]
        [SerializeField] private int dano = 10;
        [SerializeField] private float fuerzaKnockback = 8f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Stats playerStats = collision.gameObject.GetComponent<Stats>();

                if (playerStats != null)
                {
                    // 1. Calculamos la dirección del empujón (del enemigo hacia el jugador)
                    Vector2 direccionEmpujon = (collision.transform.position - transform.position).normalized;

                    // Le agregamos un leve impulso hacia arriba (0.3) para que despegue del suelo al ser golpeado
                    direccionEmpujon.y += 0.3f;
                    direccionEmpujon = direccionEmpujon.normalized;

                    // 2. Llamamos a la función consolidada en Stats
                    playerStats.TomarDano(dano, direccionEmpujon, fuerzaKnockback);
                }
            }
        }
    }
}