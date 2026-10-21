using System;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts
{
    // Показывать поле в инспекторе, только если соседнее bool-поле включено (аналог Odin [ShowIf]).
    // Рисует ShowIfDrawer (Core.Utils.Editor).
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
