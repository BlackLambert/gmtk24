using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Flags]
    public enum BodyPartSlotType
    {
        Tip = 1 << 1,
        Front = 1 << 2,
        Back = 1 << 3,
        Side = 1 << 4
    }
}
