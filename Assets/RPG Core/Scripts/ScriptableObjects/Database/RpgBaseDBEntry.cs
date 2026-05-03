using System;
using UnityEngine;

namespace Mentor.RpgCore.Database
{
    [CreateAssetMenu(fileName = "BaseDBEntry", menuName = "RPG Core/DB/Test/Base DB Entry")]
    public class RpgBaseDBEntry : ScriptableObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public Sprite Icon { get; set; }

        public RpgBaseDBEntry(string name)
        {
            ID = -1;
            Name = name;
            FileName = StringHelpers.SanitizeFileNamePascalCase(name);
            DisplayName = name;
            Icon = null;
            Description = String.Empty;
        }
    }
}