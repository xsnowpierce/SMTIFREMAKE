using Database;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.UI 
{

    public class BattleResultsItem : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TMP_Text itemText;

        public void SetItemInfo(Item item, Databases database)
        {
            itemText.text = database.GetTranslatedItem(item.itemKey);
        }
    }
    
}