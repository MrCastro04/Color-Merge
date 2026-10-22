using System;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string BoolFieldName;

        public ShowIfAttribute(string boolFieldName)
        {
            BoolFieldName = boolFieldName;
        }
    }
}
