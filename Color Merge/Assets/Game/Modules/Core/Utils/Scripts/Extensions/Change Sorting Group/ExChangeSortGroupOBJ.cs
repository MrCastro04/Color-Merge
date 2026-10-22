using System;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExChangeSortGroupOBJ
    {
        public static void ChangeGroup(
            this IChangeSortGroupInCanvas obj,
            string group,
            int orderInLayer = 0)
        {
            if (obj.DesiredGroup == String.Empty || obj.DesiredGroup.IsNull()) 
                group = SORTING_LAYERS.TUTOR;

            obj.SortingCanvas.overrideSorting = true;
            obj.SortingCanvas.sortingLayerName = group;
            obj.SortingCanvas.sortingOrder = orderInLayer;
        }

        public static void ResetGroup(this IChangeSortGroupInCanvas obj)
        {
            obj.SortingCanvas.sortingLayerName = obj.StartGroup;
            obj.SortingCanvas.sortingOrder = 0;
            obj.SortingCanvas.overrideSorting = false;
        }

        public static void ChangeGroup(this IChangeSortGroupInSpriteRenderer obj, string group, int orderInLayer = 0)
        {
            foreach (var part in obj.SortingSpriteRenderers)
            {
                part.sortingLayerName = group;
                part.sortingOrder = orderInLayer;
            }
        }

        public static void ResetGroup(this IChangeSortGroupInSpriteRenderer obj)
        {
            foreach (var part in obj.SortingSpriteRenderers)
            {
                part.sortingLayerName = obj.StartGroup;
                part.sortingOrder = 0;
            }
        }
    }
}