using System.Collections;
using UnityEngine;

namespace Entity.Controller
{
    public class ClimbEdge : MonoBehaviour
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Vector2 climbOffset = Vector2.one, bodyOffset = 0.5f * Vector2.one;
        
        [Header("Detection")]
        [SerializeField] private LayerMask edgeMask;
        [SerializeField] private float distance = 1f;
        [SerializeField] private float height = 1f;

        private ObjectAnimations _animator;
        private ObjectPhysics _physics;

        private WaitForSeconds _climbDelay;
        private bool _isClimbing;

        private void Awake()
        {
            _animator = GetComponent<ObjectAnimations>();
            _physics = GetComponent<ObjectPhysics>();
            _climbDelay = new(duration);
        }
        private void Update()
        {
            if (_isClimbing || _physics.IsGrounded()) return;
            
            if (TryFindLedge(out Vector2 corner))
                StartClimb(corner);
        }
        
        private bool TryFindLedge(out Vector2 corner)
        {
            corner = default;
 
            Vector2 direction = _physics.GetHorizontalDirection();
            if (direction.x == 0f) return false;
 
            //Detect wall in front
            RaycastHit2D wallHit = _physics.RayCast2D(direction, 0.5f * bodyOffset, distance, edgeMask);
            if (!wallHit.collider) return false;
 
            //Detect area above player 
            Vector2 heightOffset = height * Vector2.up;
            RaycastHit2D headHit = _physics.RayCast2D(direction, heightOffset, distance, edgeMask);
            if (headHit.collider) return false;
            
            //Detect surface for new position
            Vector2 downOrigin = distance * direction + heightOffset + Vector2.up;
            RaycastHit2D ledgeHit = _physics.RayCast2D(Vector2.down, downOrigin, 1f + height, edgeMask);
            if (!ledgeHit.collider) return false;
 
            corner = new Vector2(wallHit.point.x - bodyOffset.x * direction.x, ledgeHit.point.y - bodyOffset.y);
            return true;
        }
        private void StartClimb(Vector2 corner)
        {
            _isClimbing = true;
            _physics.SetFreeze(true, duration);
            _physics.SetConstrain(RigidbodyConstraints2D.FreezeAll, duration);
            // _animator.SetBool("isClimbing", true);
            
            transform.position = corner;
 
            StopAllCoroutines();
            StartCoroutine(FinishClimb());
        }
        private IEnumerator FinishClimb()
        {
            yield return _climbDelay;
            Vector2 direction = _physics.GetHorizontalDirection();
            transform.position += new Vector3(climbOffset.x * direction.x, climbOffset.y, 0f);
            
            _physics.SetIsGrounded(true);
            // _animator.SetBool("isClimbing", false);
            _isClimbing = false;
        }
        
        private void OnDrawGizmos()
        {
            Vector2 origin = transform.position;
            Vector2 upper = height * Vector2.up;
 
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin + upper, origin + upper + Vector2.right * distance);
        }
    }
}