using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField] private bool enableDebug;
        [SerializeField] private float updateSpeed = 0f;
        private bool currentDebug;
        [Space(20)]
        
        [SerializeField] private GameObject canvasObject;
        [SerializeField] private Text debugText;
        
        int m_frameCounter = 0;
        float m_timeCounter = 0.0f;
        float m_lastFramerate = 0.0f;
        public float m_refreshTime = 0.5f;

        private void Awake()
        {
            UpdateDebug();
            InvokeRepeating(nameof(UpdateDebugStats), 0.1f, updateSpeed);
        }
        
        private void Update()
        {
            if (currentDebug != enableDebug) UpdateDebug();
            UpdateFramerate();
        }

        private void UpdateDebug()
        {
            canvasObject.SetActive(enableDebug);
            currentDebug = enableDebug;
        }

        private void UpdateDebugStats()
        {
            if (!currentDebug) return;
            
            debugText.text = (int) m_lastFramerate + " FPS";
        }

        private void UpdateFramerate()
        {
            if( m_timeCounter < m_refreshTime )
            {
                m_timeCounter += Time.deltaTime;
                m_frameCounter++;
            }
            else
            {
                //This code will break if you set your m_refreshTime to 0, which makes no sense.
                m_lastFramerate = m_frameCounter/m_timeCounter;
                m_frameCounter = 0;
                m_timeCounter = 0.0f;
            }
        }
    }
}