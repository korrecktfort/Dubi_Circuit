using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UIElements;
#endif

namespace Dubi.Circuit
{
    public abstract class CircuitEventBase : ScriptableObject
    {
#if UNITY_EDITOR        
        [System.NonSerialized] public VisualElement node = null;
        [SerializeField] public Vector2 nodePos = Vector2.zero;
#endif

        public abstract bool Unlocked { get; }

        public abstract void StateChanged();
    }
}
