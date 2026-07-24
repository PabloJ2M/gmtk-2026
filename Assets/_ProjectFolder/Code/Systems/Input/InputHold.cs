using System;

namespace UnityEngine.InputSystem
{
    [Serializable]
    public class InputHold
    {
        [SerializeField, Min(0f)] private float holdTime = 0.5f;
        
        private float _expiryTime = float.NegativeInfinity;
        
        public bool HasHoldTime => Time.time < _expiryTime;
        
        public void Set()
        {
            _expiryTime = Time.time + holdTime;
        }

        public void Consume()
        {
            _expiryTime = float.NegativeInfinity;
        }

        public bool TryConsume()
        {
            if (!HasHoldTime) {
                return false;
            }

            Consume();
            return true;
        }
    }
}