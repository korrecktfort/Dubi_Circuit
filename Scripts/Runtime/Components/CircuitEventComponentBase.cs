using System;
using UnityEngine;

namespace Dubi.Circuit
{
    public abstract class CircuitEventComponentBase<T, U> : MonoBehaviour where U : CircuitEvent<T>
    {
        [SerializeField] protected U circuitEvent = null;        
        
        private void OnEnable()
        {
            OnEnabled();
         
            this.circuitEvent?.RegisterCallback(OnValueChanged);

            if(this.circuitEvent != null && this.circuitEvent.Unlocked)
            {
                OnRegister(this.circuitEvent.Value);
            }
        }

        private void OnDisable()
        {
            this.circuitEvent?.DeregisterCallback(OnValueChanged);

            OnDisabled();
        }

        protected virtual void OnEnabled() { }

        protected virtual void OnDisabled() { }

        protected abstract void OnValueChanged(T circuitEventValue);

        protected abstract void OnRegister(T circuitEventValue);
    }
}
