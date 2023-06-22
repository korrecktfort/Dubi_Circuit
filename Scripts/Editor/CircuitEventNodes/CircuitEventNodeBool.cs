using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dubi.Circuit
{    
    public class CircuitEventNodeBool : CircuitEventNode<bool>
    {
        public CircuitEventNodeBool(DragParent dragParent) : base(dragParent)
        {
        }       
    }
}