using TMPro;
using UnityEngine;

namespace Game.Battle
{
    public class BattleStats : MonoBehaviour
    {
        [SerializeField] private GameObject moonUIObject;
        [SerializeField] private GameObject statsUIObject;
        [SerializeField] private TMP_Text maccaText;
        [SerializeField] private TMP_Text magText;
        
        private int yen;
        private int mag;

        public void SetStatus(int Yen, int Mag)
        {
            this.yen = Yen;
            this.mag = Mag;

            maccaText.text = yen.ToString();
            magText.text = mag.ToString();
        }

        public void SetMoonVisible(bool value)
        {
            moonUIObject.SetActive(value);
        }

        public void SetCurrencyVisible(bool value)
        {
            statsUIObject.SetActive(value);
        }
    }
}