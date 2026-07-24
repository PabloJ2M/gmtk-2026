using System.Collections;
using UnityEngine;

namespace Entity.Controller
{
    public class Dash : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float speed = 10f;
        [SerializeField, Min(0f)] private float duration = 0.2f, timeRecovery = 1f;
        [SerializeField] private LayerMask excludedLayers;
        
        private ObjectAnimations _animator;
        private ObjectPhysics _physics;

        private WaitForSeconds _dashDelay;
        private bool _isDashing;
        
        private void Awake()
        {
            _animator = GetComponent<ObjectAnimations>();
            _physics = GetComponent<ObjectPhysics>();
            _dashDelay = new(duration + timeRecovery);
        }
        private void OnSprint()
        {
            if (_isDashing || _physics.IsFreezeHorizontal()) return;
            
            _isDashing = true;
            // _animator.SetTrigger("dash");
            
            _physics.SetFreezeHorizontal(true, duration);
            _physics.ExcludeLayers(excludedLayers, duration);
            
            _physics.SetConstrain(RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation, duration);
            _physics.SetLinearVelocityX(speed * _physics.GetHorizontalDirection().x);
            
            StartCoroutine(RecoverFromDash());
        }
        private IEnumerator RecoverFromDash()
        {
            yield return _dashDelay;
            _isDashing = false;
        }
    }
}