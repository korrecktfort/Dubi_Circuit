using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dubi.Circuit
{
    [CreateAssetMenu(menuName = "Dubi/Circuit Events/Bool")]
    public class CircuitEventBool : CircuitEvent<bool>
    {
        protected override bool ValidReceive => base.Value;
    }
}
