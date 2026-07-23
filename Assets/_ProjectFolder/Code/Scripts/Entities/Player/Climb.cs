using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Controller
{
    public class Climb : MonoBehaviour
    {
        [SerializeField] private string climbTag = "LadderTag";
        [SerializeField] private float climbSpeed = 1;

        private ObjectPhysics _physics;
        private int _input;

        private bool _isOverLadder, _hasInteract;

        private void Awake() => _physics = GetComponent<ObjectPhysics>();
        private void FixedUpdate()
        {
            //Limit interaction inside ladder area
            if (!_isOverLadder) return;
            
            TriggerInteraction();
            MoveDirection();
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(climbTag)) return;
            _isOverLadder = true;
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag(climbTag)) return;
            _isOverLadder = _hasInteract = false;
            _physics.ResetGravityScale();
        }
        
        private void OnMove(InputValue value) => _input = Mathf.RoundToInt(value.Get<Vector2>().y);

        private void TriggerInteraction()
        {
            if (_hasInteract || _input == 0) return;
            
            _physics.SetGravityScaleZero();
            _physics.SetLinearVelocityY(0);
            _hasInteract = true;
        }
        private void MoveDirection()
        {
            if (_hasInteract)
                _physics.SetLinearVelocityY(_input * climbSpeed);
        }
    }
}