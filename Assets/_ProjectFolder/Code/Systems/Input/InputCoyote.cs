using System;

namespace UnityEngine.InputSystem
{
    [Serializable]
    public class InputCoyote
    {
        [SerializeField, Min(0f)] private float coyoteTime = 0.5f;
        
        private float _expiryTime = float.NegativeInfinity;

        public bool HasCoyoteTime => Time.time < _expiryTime;

        public void Set()
        {
            _expiryTime = Time.time + coyoteTime;
        }

        public void Consume()
        {
            _expiryTime = float.NegativeInfinity;
        }

        public bool TryConsume()
        {
            if (!HasCoyoteTime) {
                return false;
            }

            Consume();
            return true;
        }
    }
}