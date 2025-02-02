using Entity;
using UnityEngine;

namespace Data
{
    public class PartyMemberStats : MonoBehaviour
    {
        [Header("Party Stats")]
        public int rowID;
        public Entity.Entity entity;
        public int health;
        public int mana;
    }
}