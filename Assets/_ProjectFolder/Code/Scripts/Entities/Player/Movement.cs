using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Entity.Controller
{
    [RequireComponent(typeof(ObjectPhysics))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float startAccelRate = 7f, stopAccelRate = 10f, turnAccelRate = 20f;

        private ObjectPhysics _physics;
        private float _accelRate;
        private int _input;

        private void Awake() => _physics = GetComponent<ObjectPhysics>();
        private void FixedUpdate()
        {
            if (_physics.IsFreezeHorizontal()) return;
            
            //Movement logic
            float currentSpeed = _physics.GetLinearVelocityX();
            float targetSpeed = _input * speed;
            float speedDif = targetSpeed - currentSpeed;
            
            //Apply force to move character
            float force = speedDif * GetAccelerationRate(currentSpeed, targetSpeed);
            _physics.AddForceX(force, ForceMode2D.Force);

            //Change direction
            if (_input != 0) _physics.SetHorizontalDirection(_input);
        }
        
        private void OnMove(InputValue input) => _input = Mathf.RoundToInt(input.Get<Vector2>().x);
        private float GetAccelerationRate(float currentSpeed, float targetSpeed)
        {
            if (_input == 0) return stopAccelRate;

            int velocity = Math.Sign(currentSpeed);
            bool isTurning = velocity != 0 && velocity != Math.Sign(targetSpeed);
            return isTurning ? turnAccelRate : startAccelRate;
        }
    }
}