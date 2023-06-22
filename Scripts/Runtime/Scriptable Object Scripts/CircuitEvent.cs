using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dubi.Circuit
{
    public abstract class CircuitEvent<T> : CircuitEventBase
    {
        protected abstract bool ValidReceive { get; }

        [SerializeField] CircuitEvent<T> parent = null;
        public CircuitEvent<T> Parent => this.parent;

        [SerializeField] CircuitEvent<T>[] children = new CircuitEvent<T>[0];
        public CircuitEvent<T>[] Children { get => children;}

        [SerializeField] T value;
        public T Value => this.value;

        Action<T> action = null;

        public override bool Unlocked => !this.lockable || this.lockable && this.parent != null && this.parent.ValidReceive && this.parent.Unlocked;

        /// If lockable, the value state change take effect if parent & parent value is valid.
        /// otherwise value state changes have no immediate effect (no action invoke)
        [SerializeField] bool lockable = false; /// -> we can attach a parent
        public void ChangeValue(T value)
        {
            this.value = value;
            StateChanged();
        }
       
        public override void StateChanged()
        {
            if (this.Unlocked)
            {
                this.action?.Invoke(this.value);
                
                if (this.children.Length > 0)
                    foreach (CircuitEvent<T> child in this.children)
                        child.StateChanged();
            }
        }   

        public void RegisterCallback(params Action<T>[] actions)
        {
            foreach(Action<T> action in actions)            
                this.action += action;                      
        }     

        public void DeregisterCallback(params Action<T>[] actions)
        {
            foreach(Action<T> action in actions)
                this.action -= action;
        }              
    }
}