using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BodyPartCollectionSettings", menuName = "Game/BodyPartCollectionSettings", order = 1)]
    public class BodyPartCollectionSettings : ScriptableObject
    {
        public IReadOnlyList<BodyPart> BodyParts => _bodyParts;
        
        [SerializeField] private List<BodyPart> _bodyParts;
    }
}
