using Data;
using Database;
using UnityEngine;

namespace Game.Console.Commands.AddEntity
{
    [CreateAssetMenu(fileName = "AddEntity Command", menuName = "Commands/Add Entity")]
    public class AddEntityCommand : ConsoleCommand
    {
        public override bool Process(string[] args, string sceneName, out string completionText)
        {
            if (args.Length == 0)
            {
                completionText =
                    "<color=red>You must specify an integer in regards to which entity to add to the party.</color>";
                return true;
            }
            else if (args.Length > 1)
            {
                completionText =
                    "<color=red>You had too many arguments. Only one is needed.</color>";
                return true;
            }
            
            // Check if the argument is even a number
            bool isNumber = int.TryParse(args[0], out int entityID);
            if (!isNumber)
            {
                completionText =
                    "<color=red>The argument must be a number.</color>";
                return true;
            }
            
            SaveData saveData = FindObjectOfType<SaveData>();
            Databases database = FindObjectOfType<Databases>();
            Entity.Entity addingEntity = database.GetEntity(entityID);

            string addText = "";
            saveData.AddPartyMember(entityID, out addText);
            if(addText == "successful")
                completionText = "'" + addingEntity.entityName + "' has been added to the party.";
            else if (addText == "party_full")
            {
                completionText = "Unable to add party member: Party was full.";
            }
            else
            {
                completionText = "An unknown error has occurred.";
            }
            return true;
        }
    }
}