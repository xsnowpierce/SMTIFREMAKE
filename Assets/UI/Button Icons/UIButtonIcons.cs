using System;
using UnityEngine;

namespace Game.UI
{
    [Serializable]
    public struct UIButtonIcon
    {
        public Sprite aButton;
        public Sprite bButton;
        public Sprite xButton;
        public Sprite yButton;
        public Sprite rbButton;
        public Sprite lbButton;
        public Sprite rtButton;
        public Sprite ltButton;
        public Sprite r3Button;
        public Sprite l3Button;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
    }

    public enum UIButtonType
    {
        Xbox
    }

    public enum UIButtonID
    {
        aButton,
        bButton,
        xButton,
        yButton,
        rbButton,
        lbButton,
        rtButton,
        ltButton,
        r3Button,
        l3Button,
        startButton,
        selectButton,
        dpadUp,
        dpadDown,
        dpadLeft,
        dpadRight
    }
    
    [CreateAssetMenu(fileName = "New Button Icons", menuName = "Misc/UI Buttons")]
    public class UIButtonIcons : ScriptableObject
    {
        public string iconPackName;
        public UIButtonType buttonType;
        public UIButtonIcon buttons;

        public Sprite GetButtonImage(UIButtonID buttonID)
        {
            switch (buttonID)
            {
                case UIButtonID.aButton:
                    return buttons.aButton;
                case UIButtonID.bButton:
                    return buttons.bButton;
                case UIButtonID.xButton:
                    return buttons.xButton;
                case UIButtonID.yButton:
                    return buttons.yButton;
                case UIButtonID.rbButton:
                    return buttons.rbButton;
                case UIButtonID.lbButton:
                    return buttons.lbButton;
                case UIButtonID.rtButton:
                    return buttons.rtButton;
                case UIButtonID.ltButton:
                    return buttons.ltButton;
                case UIButtonID.r3Button:
                    return buttons.r3Button;
                case UIButtonID.l3Button:
                    return buttons.l3Button;
                case UIButtonID.startButton:
                    return buttons.startButton;
                case UIButtonID.selectButton:
                    return buttons.selectButton;
                case UIButtonID.dpadUp:
                    return buttons.dpadUp;
                case UIButtonID.dpadDown:
                    return buttons.dpadDown;
                case UIButtonID.dpadLeft:
                    return buttons.dpadLeft;
                case UIButtonID.dpadRight:
                    return buttons.dpadRight;
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttonID), buttonID, null);
            }
        }
    }
}