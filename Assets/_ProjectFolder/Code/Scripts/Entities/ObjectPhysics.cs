using System;
using UnityEngine;

namespace Entity
{
    public class ObjectPhysics : MonoBehaviour
    {
        private Transform _transform;
        private Rigidbody2D _rigidbody2D;
        
        private RigidbodyConstraints2D _defaultConstraints;
        private LayerMask _defaultExcludedLayers;
        private float _defaultGravityScale;
        
        private bool _freezeHorizontal, _freezeVertical;
        private bool _isGrounded;

        private void Awake()
        {
            _transform = transform;
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        private void Start()
        {
            _defaultGravityScale = _rigidbody2D.gravityScale;
            _defaultConstraints = _rigidbody2D.constraints;
            _defaultExcludedLayers = _rigidbody2D.excludeLayers;
        }
        private async void ScheduleReset(float time, int version, Action onComplete, Func<int> currentVersion)
        {
            await Awaitable.WaitForSecondsAsync(time);
            if (this != null && version == currentVersion())
                onComplete?.Invoke();
        }

        #region Transform
        public void SetHorizontalDirection(float value) => _transform.localScale = new Vector3(value, 1f, 1f);
        public Vector2 GetHorizontalDirection() => new Vector2(_transform.localScale.x, 0f);
        public void FlipHorizontalDirection() => SetHorizontalDirection(-_transform.localScale.x);
        #endregion

        #region Gravity
        public void SetGravityScale(float value) => _rigidbody2D.gravityScale = value;
        public void SetGravityScaleZero() => SetGravityScale(0f);
        public void ResetGravityScale() => SetGravityScale(_defaultGravityScale);
        #endregion
        #region Constrains
        
        private int _constrainVersion;
        
        public void SetConstrain(RigidbodyConstraints2D value, float time = 0f)
        {
            _rigidbody2D.constraints = value;
            if (time != 0f) ScheduleReset(time, ++_constrainVersion, ResetConstrain, () => _constrainVersion);
        }
        public void ResetConstrain() => _rigidbody2D.constraints = _defaultConstraints;
        
        #endregion
        #region Exclude Layers

        private int _excludeLayersVersion;
        
        public void ExcludeLayers(LayerMask layers, float time = 0f)
        {
            _rigidbody2D.excludeLayers = layers;
            if(time != 0f) ScheduleReset(time, ++_excludeLayersVersion, ResetExcludedLayers, () => _excludeLayersVersion);
        }
        public void ResetExcludedLayers() => _rigidbody2D.excludeLayers = _defaultExcludedLayers;
        
        #endregion

        #region Physics

        public bool IsGrounded() => _isGrounded;
        public void SetIsGrounded(bool value) => _isGrounded = value;

        public bool IsFreeze() => _freezeHorizontal && _freezeVertical;
        public void SetFreeze(bool value, float time = 0f)
        {
            SetFreezeHorizontal(value, time);
            SetFreezeVertical(value, time);
        }

        private int _freezeHorizontalVersion;
        
        public bool IsFreezeHorizontal() => _freezeHorizontal;
        public void SetFreezeHorizontal(bool value, float time = 0f)
        {
            _freezeHorizontal = value;
            if (time != 0f) ScheduleReset(time, ++_freezeHorizontalVersion, DisableFreezeHorizontal, () => _freezeHorizontalVersion);
        }
        public void DisableFreezeHorizontal() => SetFreezeHorizontal(false);

        private int _freezeVerticalVersion;
        
        public bool IsFreezeVertical() => _freezeVertical;
        public void SetFreezeVertical(bool value, float time = 0f)
        {
            _freezeVertical = value;
            if (time != 0f) ScheduleReset(time, ++_freezeVerticalVersion, DisableFreezeVertical, () => _freezeVerticalVersion);
        }
        public void DisableFreezeVertical() => SetFreezeVertical(false);
        
        #endregion
        #region Forces
        public void AddForce(Vector2 force, ForceMode2D mode) => _rigidbody2D.AddForce(force, mode);
        public void AddForceX(float force, ForceMode2D mode) => _rigidbody2D.AddForceX(force, mode);
        public void AddForceY(float force, ForceMode2D mode) => _rigidbody2D.AddForceY(force, mode);
        #endregion
        #region Velocity
        public void SetLinearVelocity(Vector2 direction) => _rigidbody2D.linearVelocity = direction;
        public void SetLinearVelocityX(float value) => _rigidbody2D.linearVelocityX = value;
        public void SetLinearVelocityY(float value) => _rigidbody2D.linearVelocityY = value;
        
        public Vector2 GetLinearVelocity() => _rigidbody2D.linearVelocity;
        public float GetLinearVelocityX() => _rigidbody2D.linearVelocityX;
        public float GetLinearVelocityY() => _rigidbody2D.linearVelocityY;
        #endregion

        #region Contacts
        public bool CompareLayer(GameObject obj, LayerMask mask) => ((1 << obj.gameObject.layer) & mask) != 0;
        public bool IsContactInAngle(Collision2D collision, Vector2 direction, float maxAngle)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                float angle = Vector2.Angle(collision.GetContact(i).normal, direction);
                if (angle <= maxAngle)
                    return true;
            }
            
            return false;
        }
        #endregion
        #region Casts
        public RaycastHit2D RayCast2D(Vector2 direction, Vector2 offset, float distance, LayerMask mask)
        {
            Vector2 origin = _rigidbody2D.position + offset;
            return Physics2D.Raycast(origin, direction, distance, mask);
        }
        public RaycastHit2D CircleCast2D(Vector2 direction, Vector2 offset, float radius, float distance, LayerMask mask)
        {
            Vector2 origin = _rigidbody2D.position + offset;
            return Physics2D.CircleCast(origin, radius, direction, distance, mask);
        }
        #endregion
    }
}