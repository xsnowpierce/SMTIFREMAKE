using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class BetaUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text topLeftText;

        private void Awake()
        {
            topLeftText.text = "Build: " + Application.version + "-" + Application.buildGUID;
        }
    }
}