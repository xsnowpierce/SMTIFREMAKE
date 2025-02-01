using System;
using System.Linq;
using Database;
using Game.Level;
using Game.Movement;
using UnityEngine;

namespace Game.Console.Commands.LoadLevel
{
    [CreateAssetMenu(menuName = "Commands/LoadLevel", fileName = "LoadLevel")]
    public class LoadLevelCommand : ConsoleCommand
    {
        private readonly float PLAYER_HEIGHT = 2.5f;
        private readonly string DUNGEON_SCENE = "Dungeon";
        
        public override bool Process(string[] args, string sceneName, out string completionText)
        {
            if (!sceneName.Equals(DUNGEON_SCENE))
            {
                completionText = "<color=red>This command cannot be completed on this scene.</color>";
                return true;
            }
            
            Transform player = FindObjectOfType<PlayerMovement>().transform;
            Databases databases = FindObjectOfType<Databases>();
            MapLoader loader = FindObjectOfType<MapLoader>();
            
            if (player == null || databases == null || loader == null)
            {
                completionText = "<color=red>ERROR: One or more required objects were null.</color>";
                return false;
            }

            if (args.Length <= 0)
            {
                // Not enough args
                completionText = "<color=red>Not enough arguments. Usage: '" + commandUsage + "'</color>";
                return true;
            }

            if (args.Length == 1)
            {
                if (args[0].ToLower().Equals("unload"))
                {
                    /*if (loader.GetCurrentLevelInfo() == null)
                    {
                        completionText = "<color=red>There is no level loaded.</color>";
                        return true;
                    }
                    else
                    {
                        loader.DestroyLevel();
                        player.position = new Vector3(0, PLAYER_HEIGHT, 0);
                        completionText = "Level has been unloaded.";
                        return true;
                    }*/
                }
                else if (args[0].ToLower().Equals("list"))
                {
                    string levelStrings = "";
                    foreach (GameObject maps in databases.GetMaps())
                    {
                        levelStrings += maps.GetComponent<MapData>().GetMapInfo().mapID + ", ";
                    }
                    completionText = "Levels available are:"+"\n"+levelStrings;
                    return true;
                }
            }

            if (args.Length > 2)
            {
                // Too many args
                completionText = "<color=red>Too many arguments. Usage: '" + commandUsage + "'</color>";
                return true;
            }

            GameObject newMap = null;
            int floorNum = 0;


            if (args.Length > 0)
            {
                // Load the first floor in the level
                newMap = databases.GetMapFromStringID(args[0]);
                floorNum = 1;

                if (newMap == null)
                {
                    completionText = "A level with that name does not exist. Please try again.";
                    return true;
                }

                if (args.Length == 2)
                {
                    // Load the specified floor in the specified level
                    try
                    {
                        floorNum = Int32.Parse(args[1]);
                    }
                    catch (FormatException)
                    {
                        completionText =
                            "<color=red>Floor number must be written as a numeric. Please try again.</color>";
                        return true;
                    }
                }

                MapData mapData = newMap.GetComponent<MapData>();
                
                if (floorNum > mapData.GetMapInfo().floors.Length + 1)
                {
                    completionText =
                        "The floor number specified was too large for this level. The floor count in this level is only " +
                        mapData.GetMapInfo().floors.Length + 1;
                    return true;
                }
                if (floorNum < 0)
                {
                    completionText = "The floor number cannot be less than 1.";
                    return true;
                }

                player.transform.position = new Vector3(0, PLAYER_HEIGHT, 0);
                GameObject obj = databases.GetMapFromStringID(args[0]);
                loader.LoadLevel(obj, floorNum);

                completionText = "Teleporting to " + mapData.GetMapInfo().mapName + " on Floor " + (floorNum) + ".";
                return true;
            }

            completionText = "<color=red>An error has occurred.</color>";
            return false;
        }
    }
}