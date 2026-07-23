using System.Collections.Generic;
using UnityEngine;

namespace Entity.Controller
{
    public class WallSlide : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float slideSpeed = 2f;
        
        [Header("Detection")]
        [SerializeField] private LayerMask wallMask;
        [SerializeField, Range(0f, 90f)] private float minWallAngle = 45f;
        
        private readonly HashSet<Collider2D> _contacts = new();
        
        private ObjectAnimations _animator;
        private ObjectPhysics _physics;

        private void Awake()
        {
            _animator = GetComponent<ObjectAnimations>();
            _physics = GetComponent<ObjectPhysics>();
        }
        private void FixedUpdate()
        {
            bool isFalling = _physics.GetLinearVelocityY() < -slideSpeed;
            bool shouldSlide = IsTouchingWall() && !_physics.IsGrounded() && isFalling;
            
            // _animator.SetBool("isSliding", shouldSlide);
            
            if (shouldSlide)
                _physics.SetLinearVelocityY(-slideSpeed);
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_physics.CompareLayer(collision.gameObject, wallMask)) return;
            if (!_physics.IsContactInAngle(collision, -_physics.GetHorizontalDirection(), minWallAngle)) return;
 
            _contacts.Add(collision.collider);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!_physics.CompareLayer(collision.gameObject, wallMask)) return;
            _contacts.Remove(collision.collider);
        }
        
        //Metodos comunes
        private bool IsTouchingWall() => _contacts.Count > 0;
    }
}