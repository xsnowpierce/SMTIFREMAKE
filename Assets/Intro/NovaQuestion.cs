using System;
using UnityEngine;

namespace Intro
{
    [CreateAssetMenu(fileName = "NovaQuestion", menuName = "Dialogue/Nova Question")]
    public class NovaQuestion : ScriptableObject
    {
        [Serializable]
        public struct NovaQuestionChoice
        {
            public string englishText;
            public string japaneseText;
            public int pointsAdded;
        }

        [Header("Question Info")]
        public string englishQuestion;
        public string japaneseQuestion;
        
        [Header("Responses")]
        public NovaQuestionChoice[] choices;
    }
}