using UnityEngine;

namespace Entity
{
    public class ObjectAnimations : MonoBehaviour
    {
        private ObjectPhysics _physics;
        private Animator _animator;
        
        // private readonly string _speedParam = "speed";
        // private readonly string _gravityParam = "gravity";
        // private readonly string _groundParam = "isGrounded";
        
        private void Awake()
        {
            _physics = GetComponent<ObjectPhysics>();
            _animator = GetComponent<Animator>();
        }
        private void FixedUpdate()
        {
            // _animator.SetFloat(_speedParam, Mathf.Abs(_physics.GetLinearVelocityX()));
            // _animator.SetFloat(_gravityParam, _physics.GetLinearVelocityY());
            // _animator.SetBool(_groundParam,  _physics.IsGrounded());
        }
        
        public void SetInt(string name, int value) => _animator.SetInteger(name, value);
        public void SetBool(string name, bool value) => _animator.SetBool(name, value);
        public void SetFloat(string name, float value) => _animator.SetFloat(name, value);
        public void SetTrigger(string name) => _animator.SetTrigger(name);
    }
}