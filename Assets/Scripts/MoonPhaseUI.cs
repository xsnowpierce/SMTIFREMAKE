using UnityEngine;
using UnityEngine.UI;

public class MoonPhaseUI : MonoBehaviour
{
    [SerializeField] private Text moonPhaseText;
    [SerializeField] private bool useUppercase;
    [Header("Moon Image")]
    [SerializeField] private Image moonPhaseImage;
    [SerializeField] private Sprite[] moonSprites;

    public void UpdateCounter(int value, string value2)
    {
        string text = value2;
        if (useUppercase) text = text.ToUpper();
        moonPhaseText.text = text;
        moonPhaseImage.sprite = moonSprites[value];
    }
}