using UnityEngine;

namespace Game
{
    public class DontDestroy : MonoBehaviour
    {
        public string serialID;
        public bool dontDestroy = true;

        private void Awake()
        {
            if (!dontDestroy) return;
            
            DontDestroy[] dontDestroys = FindObjectsOfType<DontDestroy>();
            if (dontDestroys.Length > 1)
            {
                // Check to make sure none have our serialID
                foreach (DontDestroy dontDestroy in dontDestroys)
                {
                    if (dontDestroy.serialID == serialID)
                    {
                        // Destroy this one
                        Destroy(gameObject);
                    }
                }
                // Didn't find any!
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}