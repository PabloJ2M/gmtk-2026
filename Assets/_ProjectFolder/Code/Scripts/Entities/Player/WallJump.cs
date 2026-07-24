using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Controller
{
    public class WallJump : MonoBehaviour
    {
        [SerializeField] private float jumpForce, surfaceForce = 5;
        [SerializeField] private float freezeTime = 0.5f;

        [Header("Detection")]
        [SerializeField] private LayerMask wallMask;
        [SerializeField] private float distance = 0.6f, offset = 0f, radius = 0.1f;

        private ObjectAnimations _animator;
        private ObjectPhysics _physics;

        private void Awake()
        {
            _animator = GetComponent<ObjectAnimations>();
            _physics = GetComponent<ObjectPhysics>();
        }
        
        private void OnJump(InputValue input)
        {
            if (!input.isPressed) return;
            if (_physics.IsFreezeVertical() || _physics.IsGrounded()) return;
            
            //Detect walls for wall jump
            RaycastHit2D wallHit = _physics.CircleCast2D(_physics.GetHorizontalDirection(), offset * Vector2.up, radius, distance, wallMask);
            if (!wallHit.collider) return;

            //Freeze movement
            _physics.SetFreezeHorizontal(true, freezeTime);
            
            //Apply new velocity and change direction
            _physics.SetLinearVelocity(jumpForce * Vector2.up + wallHit.normal * surfaceForce);
            _physics.FlipHorizontalDirection();
            
            // _animator.SetTrigger("jump");
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new(0f, 1f, 0f, 0.5f);

            Vector2 from = offset * Vector2.up + (Vector2)transform.position;
            Vector2 to = from + (Vector2)transform.right * distance;
            
            Gizmos.DrawLine(from, to);
            Gizmos.DrawWireSphere(to,  radius);
        }
    }
}