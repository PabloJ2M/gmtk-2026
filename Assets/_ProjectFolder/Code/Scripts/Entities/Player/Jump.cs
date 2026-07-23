using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Controller
{
    [RequireComponent(typeof(ObjectPhysics))]
    public class Jump : MonoBehaviour
    {
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float bufferTime = 0.1f, coyoteTime = 0.2f;

        [Header("Detection")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 45f;

        private readonly HashSet<Collider2D> _contacts =  new();
        
        private ObjectAnimations _animator;
        private ObjectPhysics _physics;
        
        private float _bufferTimer, _coyoteTimer;

        private void Awake()
        {
            _animator = GetComponent<ObjectAnimations>();
            _physics = GetComponent<ObjectPhysics>();
        }
        private void Update()
        {
            if (_bufferTimer > 0f)
                _bufferTimer -= Time.deltaTime;
            
            if (_coyoteTimer > 0f)
                _coyoteTimer -= Time.deltaTime;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_physics.CompareLayer(collision.gameObject, groundMask)) return;
            if (!_physics.IsContactInAngle(collision, Vector2.up, maxGroundAngle)) return;

            _contacts.Add(collision.collider);
            _physics.SetIsGrounded(true);
            
            InputBufferCollisionEvent();
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!_physics.CompareLayer(collision.gameObject, groundMask)) return;
            _contacts.Remove(collision.collider);
            
            if (_contacts.Count > 0) return;
            
            _physics.SetIsGrounded(false);
            InputCoyoteCollisionEvent();
        }
        
        private void OnJump()
        {
            _bufferTimer = bufferTime;
            if (!_physics.IsGrounded() && _coyoteTimer <= 0f) return;
            
            _physics.SetLinearVelocityY(jumpForce);
            // _animator.SetTrigger("jump");
            _coyoteTimer = 0f;
        }
        
        private void InputBufferCollisionEvent()
        {
            if (_bufferTimer > 0)
                OnJump();
        }
        private void InputCoyoteCollisionEvent()
        {
            if (_bufferTimer <= 0)
                _coyoteTimer = coyoteTime;
        }
    }
}