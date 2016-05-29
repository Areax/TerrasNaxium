using System;
using HarryPotterUnity.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace HarryPotterUnity.UI
{
    public class LessonDescriptions : MonoBehaviour
    {
        private struct LessonDescription
        {
            public string Title { get; set; }
            public string Style { get; set; }
            public string Description { get; set; }
            public Color32 TextColor { get; set; }
        }

        private readonly LessonDescription _creatures = new LessonDescription
        {
            TextColor = new Color32(0xFF, 0x83, 0x00, 0xFF),
            Title = "<b>The Flood</b>",
            Style = "Style: <b>12 cards, 12 points each</b>",
            Description = "Low value creatures that tend to control the board."
        };

        private readonly LessonDescription _demigods = new LessonDescription
        {
            TextColor = new Color32(0x30, 0x98, 0xFF, 0xFF),
            Title = "<b>Demigods</b>",
            Style = "Style: <b>10 cards, 15 points each</b>",
            Description = "Uses elements in order to establish value when vying for board control."
        };

        private readonly LessonDescription _transfiguration = new LessonDescription
        {
            TextColor = new Color32(0xF3, 0x20, 0x2D, 0xFF),
            Title = "<b>High Intellects</b>",
            Style = "Style: <b>8 cards, 18 points each</b>",
            Description = "Must strategically use every card with utmost precision."
        };

        private readonly LessonDescription _potions = new LessonDescription
        {
            TextColor = new Color32(0x00, 0xC6, 0x1A, 0xFF),
            Title = "<b>Potions</b>",
            Style = "Style: <b>Null</b>",
            Description = "Potions contain very powerful spells that are cheap, versatile, and deal high damage, but they often require a lesson sacrifice."
        };

        private readonly LessonDescription _quidditch = new LessonDescription
        {
            TextColor = new Color32(0xE8, 0xE8, 0x2E, 0xFF),
            Title = "<b>Quidditch</b>",
            Style = "Style: <b>Null</b>",
            Description = "Quidditch cards have a relatively high cost but often contain multiple effects, such as dealing damage to your opponent AND crippling their ability to play certain cards on their next turn."
        };

        private Text _lessonDescription;

        private void Start()
        {
            _lessonDescription = GetComponent<Text>();
        }

        public void SwitchText(string lesson)
        {
            LessonDescription newDescription;
            var type = (LessonTypes) Enum.Parse(typeof(LessonTypes), lesson);
            switch (type)
            {
                case LessonTypes.Creatures:
                    newDescription = _creatures;
                    break;
                case LessonTypes.Demigods:
                    newDescription = _demigods;
                    break;
                case LessonTypes.Transfiguration:
                    newDescription = _transfiguration;
                    break;
                case LessonTypes.Potions:
                    newDescription = _potions;
                    break;
                case LessonTypes.Quidditch:
                    newDescription = _quidditch;
                    break;
                default:
                    throw new ArgumentException("Bad Parameter to LessonDescription.SwitchText");
            }

            _lessonDescription.color = newDescription.TextColor;
            _lessonDescription.text = string.Format("{0}\n{1}\n\n{2}", 
                newDescription.Title, 
                newDescription.Style,
                newDescription.Description);
        }

        public void ClearText()
        {
            _lessonDescription.color = Color.clear;
        }
    }
}
