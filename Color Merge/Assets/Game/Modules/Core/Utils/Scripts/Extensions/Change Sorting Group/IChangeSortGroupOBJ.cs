using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public interface IChangeSortGroupOBJ
    {
        string DesiredGroup { get; }
        string StartGroup { get; set; }
    }

    public interface IChangeSortGroupInCanvas : IChangeSortGroupOBJ
    {
        Canvas SortingCanvas { get; }
    }

    public interface IChangeSortGroupInSpriteRenderer : IChangeSortGroupOBJ
    {
        SpriteRenderer[] SortingSpriteRenderers { get; }
    }
}