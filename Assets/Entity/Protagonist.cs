using UnityEngine;

namespace Entity
{
    [CreateAssetMenu(fileName = "New Protagonist", menuName = "Entity/Protagonist")]
    public class Protagonist : Human
    {
        [Header("Protagonist Settings")] 
        public bool isFemale = false;
    }
}