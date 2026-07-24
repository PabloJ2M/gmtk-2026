using UnityEngine;

namespace Entity.Controller
{
    public class Stats : MonoBehaviour
    {
        [Header("Salud")]
        public int health = 100;
        
        private ObjectPhysics _physics;

        private void Awake()
        {
            _physics = GetComponent<ObjectPhysics>();
        }

        //daño y nockbackback
        public void TomarDano(int cantidadDano, Vector2 direccionGolpe, float fuerzaKnockback)
        {
            health -= cantidadDano;
            Debug.Log("Player hit! Health is now: " + health);
            
            if (_physics != null)
            {
                _physics.SetLinearVelocity(fuerzaKnockback * direccionGolpe);
            }
            
            if (health <= 0)
            {
                Debug.Log("Player is dead");
            }
        }
    }
}