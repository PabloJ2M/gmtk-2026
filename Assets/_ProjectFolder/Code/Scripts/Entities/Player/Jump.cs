using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Controller
{
    [RequireComponent(typeof(ObjectPhysics))]
    public class Jump : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private InputBuffer inputBuffer;
        [SerializeField] private InputCoyote inputCoyote;
        [SerializeField] private InputHold inputHold;

        [SerializeField, Range(0f, 1f)] private float jumpCutMultiplier = 0.5f;
        [SerializeField] private float holdForce = 12f;
        
        [Header("Detection")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 45f;

        private readonly HashSet<Collider2D> _contacts =  new();
        
        private ObjectAnimations _animator;
        private ObjectPhysics _physics;
        private bool _isPressing;

        private void Awake()
        {
            _animator = GetComponent<ObjectAnimations>();
            _physics = GetComponent<ObjectPhysics>();
        }
        private void FixedUpdate()
        {
            if (!inputHold.HasHoldTime) return;
            
            if (_physics.GetLinearVelocityY() <= 0f)
            {
                inputHold.Consume();
                return;
            }
            
            _physics.AddForceY(holdForce, ForceMode2D.Force);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_physics.CompareLayer(collision.gameObject, groundMask)) return;
            if (!_physics.IsContactInAngle(collision, Vector2.up, maxGroundAngle)) return;

            _contacts.Add(collision.collider);
            _physics.SetIsGrounded(true);

            if (inputBuffer.TryConsume()) JumpHandler();
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!_physics.CompareLayer(collision.gameObject, groundMask)) return;
            _contacts.Remove(collision.collider);
            
            if (_contacts.Count > 0) return;
            _physics.SetIsGrounded(false);
            
            if (_physics.GetLinearVelocityY() <= 0f) inputCoyote.Set();
        }
        
        private void OnJump(InputValue input)
        {
            _isPressing = input.isPressed;
            if (_isPressing)
            {
                JumpHandler();
                inputHold.Set();
            }
            else
            {
                inputHold.Consume();
                
                if (_physics.GetLinearVelocityY() > 0f)
                    _physics.SetLinearVelocityY(_physics.GetLinearVelocityY() * jumpCutMultiplier);
            }
        }
        private void JumpHandler()
        {
            inputBuffer.Set();
            if (!_physics.IsGrounded() && !inputCoyote.HasCoyoteTime) return;
            if (_isPressing) inputHold.Set();
            
            _physics.SetLinearVelocityY(jumpForce);
            // _animator.SetTrigger("jump");
            inputCoyote.Consume();
        }
    }
}