using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "New Party Database", menuName = "Database/Party Additions")]
    public class EntityDatabase : ScriptableObject
    {
        [SerializeField] private Entity.Entity[] entities;

        public Entity.Entity[] GetEntityList() => entities;
    }
}