using System;
using System.Collections.Generic;
using System.Linq;
using Entity;
using Game.Level;
using Game.UI;
using Items;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Database
{
    public enum GameLanguage
    {
        English = 0
    }

    public class Databases : MonoBehaviour
    {
        [Serializable]
        private struct LanguageFiles
        {
            public TextAsset textFile;
            public TextAsset nameFile;
            public TextAsset itemFile;
            public TextAsset uiFile;
            public TextAsset skillFile;
            public TextAsset raceFile;
        }

        [SerializeField] private GameLanguage loadLanguage;
        [SerializeField] private ItemDatabase items;
        [SerializeField] private SpriteDatabase sprites;
        [SerializeField] private MapDatabase maps;
        [SerializeField] private EntityDatabase entityDatabase;
        [Space(15)] private Dictionary<string, string> textDictionary;
        private Dictionary<string, string> nameDictionary;
        private Dictionary<string, string> itemDictionary;
        private Dictionary<string, string> uiDictionary;
        private Dictionary<string, string> skillDictionary;
        private Dictionary<string, string> raceDictionary;
        [SerializeField] private LanguageFiles[] languages;
        [SerializeField] private Sprite[] elementSprites;
        [SerializeField] private UIButtonIcons buttonIcons;
        
        [Header("Element Resistance Icons")]
        [SerializeField] private Sprite resistanceWeak;
        [SerializeField] private Sprite resistanceNeutral;
        [SerializeField] private Sprite resistanceResist;
        [SerializeField] private Sprite resistanceRepel;
        [SerializeField] private Sprite resistanceAbsorb;
        [SerializeField] private Sprite resistanceNull;
        
        static Databases instance;

        private void Awake()
        {
            //Singleton method
            if (instance == null)
            {
                //First run, set the instance
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                //Instance is not the same as the one we have, destroy old one, and reset to newest one
                Destroy(instance.gameObject);
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            textDictionary = new Dictionary<string, string>();
            nameDictionary = new Dictionary<string, string>();
            itemDictionary = new Dictionary<string, string>();
            uiDictionary = new Dictionary<string, string>();
            skillDictionary = new Dictionary<string, string>();
            raceDictionary = new Dictionary<string, string>();
            LoadLanguage(loadLanguage);
        }

        private void LoadLanguage(GameLanguage language)
        {
            if (languages[(int)language].textFile != null)
                textDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(languages[(int)language].textFile
                        .ToString());
            if (languages[(int)language].nameFile != null)
                nameDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(languages[(int)language].nameFile
                        .ToString());
            if (languages[(int)language].itemFile != null)
                itemDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(languages[(int)language].itemFile
                        .ToString());
            if (languages[(int)language].uiFile != null)
                uiDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(languages[(int)language].uiFile
                        .ToString());
            if (languages[(int)language].skillFile != null)
                skillDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(languages[(int)language].skillFile
                        .ToString());
            if (languages[(int)language].raceFile != null)
                raceDictionary =
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(languages[(int)language].raceFile
                        .ToString());
            PrintContents();
        }

        [ContextMenu("Print Contents")]
        private void PrintContents()
        {
            Debug.Log("Language loaded with " + textDictionary.Count + " text element(s), " + nameDictionary.Count
                      + " name(s), " + itemDictionary.Count + " item name(s), " + uiDictionary.Count +
                      " UI element(s), " + skillDictionary.Count + " skill name(s), and " + raceDictionary.Count +
                      " race name(s).");
        }

        public string GetTranslatedText(string key)
        {
            textDictionary.TryGetValue(key, out string ret);
            return ret;
        }

        public string GetTranslatedName(string key)
        {
            nameDictionary.TryGetValue(key, out string ret);
            return ret;
        }

        public string GetTranslatedSkill(string key)
        {
            skillDictionary.TryGetValue(key, out string ret);
            return ret;
        }

        public string GetTranslatedItem(string key)
        {
            itemDictionary.TryGetValue(key, out string ret);
            return ret;
        }

        public string GetTranslatedUIElement(string key)
        {
            uiDictionary.TryGetValue(key, out string ret);
            return ret;
        }

        public string GetTranslatedRace(string key)
        {
            raceDictionary.TryGetValue(key, out string ret);
            return ret;
        }

        public Entity.Entity GetEntity(int id)
        {
            if (id < 0)
            {
                Debug.LogError("Given party ID was less than 0.");
            }
            else if (id > entityDatabase.GetEntityList().Length)
            {
                Debug.LogError("Given party ID was larger than the database size.");
            }

            return entityDatabase.GetEntityList()[id];

        }

        public Sprite GetSprite(int id)
        {
            if (id > sprites.GetSpriteDatabase().Count)
            {
                Debug.LogError("Sprite accessed was out of range. Loading sprite 0 as a replacement.");
                return sprites.GetSprite(0);
            }
            else return sprites.GetSprite(id);
        }

        public Item GetItem(int id)
        {
            return items.GetItem(id);
        }

        public GameObject[] GetMaps() => maps.GetMaps();

        public GameObject GetMapFromStringID(string mapID)
        {
            return maps.GetMaps().SingleOrDefault(item => item.GetComponent<MapData>().GetMapInfo().mapID == mapID);
        }

        public Sprite GetElementSprite(Element element)
        {
            switch (element)
            {
                case Element.Physical:
                    return elementSprites[0];
                case Element.Gun:
                    return elementSprites[1];
                case Element.Fire:
                    return elementSprites[2];
                case Element.Ice:
                    return elementSprites[3];
                case Element.Electric:
                    return elementSprites[4];
                case Element.Force:
                    return elementSprites[5];
                case Element.Nerve:
                    return elementSprites[6];
                case Element.Expel:
                    return elementSprites[7];
                case Element.Curse:
                    return elementSprites[8];
                case Element.Almighty:
                    return elementSprites[9];
                case Element.Recovery:
                    return elementSprites[10];
                case Element.Support:
                    return elementSprites[11];
            }
            return null;
        }

        public Sprite GetButtonIcon(UIButtonID buttonID)
        {
            return buttonIcons.GetButtonImage(buttonID);
        }

        public string GetTranslatedRace(DemonRace race)
        {
            string raceName = "";
            switch (race)
            {
                case DemonRace.Human:
                    raceDictionary.TryGetValue("Human", out raceName);
                    break;
                case DemonRace.Herald:
                    raceDictionary.TryGetValue("Herald", out raceName);
                    break;
                case DemonRace.Amatsu:
                    raceDictionary.TryGetValue("Amatsu", out raceName);
                    break;
                case DemonRace.Avian:
                    raceDictionary.TryGetValue("Avian", out raceName);
                    break;
                case DemonRace.Megami:
                    raceDictionary.TryGetValue("Megami", out raceName);
                    break;
                case DemonRace.Deity:
                    raceDictionary.TryGetValue("Deity", out raceName);
                    break;
                case DemonRace.Avatar:
                    raceDictionary.TryGetValue("Avatar", out raceName);
                    break;
                case DemonRace.Holy:
                    raceDictionary.TryGetValue("Holy", out raceName);
                    break;
                case DemonRace.Element:
                    raceDictionary.TryGetValue("Element", out raceName);
                    break;
                case DemonRace.Fury:
                    raceDictionary.TryGetValue("Fury", out raceName);
                    break;
                case DemonRace.Dragon:
                    raceDictionary.TryGetValue("Dragon", out raceName);
                    break;
                case DemonRace.Lady:
                    raceDictionary.TryGetValue("Lady", out raceName);
                    break;
                case DemonRace.Kunitsu:
                    raceDictionary.TryGetValue("Kunitsu", out raceName);
                    break;
                case DemonRace.Divine:
                    raceDictionary.TryGetValue("Divine", out raceName);
                    break;
                case DemonRace.Flight:
                    raceDictionary.TryGetValue("Flight", out raceName);
                    break;
                case DemonRace.Snake:
                    raceDictionary.TryGetValue("Snake", out raceName);
                    break;
                case DemonRace.Yoma:
                    raceDictionary.TryGetValue("Yoma", out raceName);
                    break;
                case DemonRace.Beast:
                    raceDictionary.TryGetValue("Beast", out raceName);
                    break;
                case DemonRace.Night:
                    raceDictionary.TryGetValue("Night", out raceName);
                    break;
                case DemonRace.Jirae:
                    raceDictionary.TryGetValue("Jirae", out raceName);
                    break;
                case DemonRace.Fairy:
                    raceDictionary.TryGetValue("Fairy", out raceName);
                    break;
                case DemonRace.Fallen:
                    raceDictionary.TryGetValue("Fallen", out raceName);
                    break;
                case DemonRace.Brute:
                    raceDictionary.TryGetValue("Brute", out raceName);
                    break;
                case DemonRace.Femme:
                    raceDictionary.TryGetValue("Femme", out raceName);
                    break;
                case DemonRace.Vile:
                    raceDictionary.TryGetValue("Vile", out raceName);
                    break;
                case DemonRace.Raptor:
                    raceDictionary.TryGetValue("Raptor", out raceName);
                    break;
                case DemonRace.Jaki:
                    raceDictionary.TryGetValue("Jaki", out raceName);
                    break;
                case DemonRace.Wilder:
                    raceDictionary.TryGetValue("Wilder", out raceName);
                    break;
                case DemonRace.Wood:
                    raceDictionary.TryGetValue("Wood", out raceName);
                    break;
                case DemonRace.Undead:
                    raceDictionary.TryGetValue("Undead", out raceName);
                    break;
                case DemonRace.Tyrant:
                    raceDictionary.TryGetValue("Tyrant", out raceName);
                    break;
                case DemonRace.Drake:
                    raceDictionary.TryGetValue("Drake", out raceName);
                    break;
                case DemonRace.Haunt:
                    raceDictionary.TryGetValue("Haunt", out raceName);
                    break;
                case DemonRace.Spirit:
                    raceDictionary.TryGetValue("Spirit", out raceName);
                    break;
                case DemonRace.Foul:
                    raceDictionary.TryGetValue("Foul", out raceName);
                    break;
                case DemonRace.Fiend:
                    raceDictionary.TryGetValue("Fiend", out raceName);
                    break;
                case DemonRace.DemonGodEmperor:
                    raceDictionary.TryGetValue("DemonGodEmperor", out raceName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(race), race, null);
            }

            return raceName;
        }

        public Sprite GetResistanceSprite(ResistanceType resistances)
        {
            switch (resistances)
            {
                case ResistanceType.Normal:
                    return resistanceNeutral;
                case ResistanceType.Resist:
                    return resistanceResist;
                case ResistanceType.Null:
                    return resistanceNull;
                case ResistanceType.Weak:
                    return resistanceWeak;
                case ResistanceType.Repel:
                    return resistanceRepel;
                case ResistanceType.Absorb:
                    return resistanceAbsorb;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resistances), resistances, null);
            }
        }
    }
}