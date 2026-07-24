using UnityEngine;

namespace Entity.AI
{
    using Controller;
    
    public class atacar : MonoBehaviour
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
                    Vector2 direccionEmpujon = (collision.transform.position - transform.position).normalized;
                    direccionEmpujon.y += 0.3f;
                    direccionEmpujon = direccionEmpujon.normalized;
                    playerStats.TomarDano(dano, direccionEmpujon, fuerzaKnockback);
                }
            }
        }
    }
}