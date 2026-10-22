using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    // Особый предмет «Candy_Bomb»: не входит в цепочку и не лежит на поле.
    // Создаётся кнопкой бомбы за энергию (мерджи), игрок переносит её на клетку — взрыв:
    // предметы и стены в радиусе _radius клеток вокруг (квадрат, центр не трогается) превращаются в конфеты.
    // Радиус 1 — 8 клеток вокруг, 2 — 24 и т.д.; за краем доски ничего не происходит.
    [CreateAssetMenu(menuName = "Game/Item/Candy Bomb Config", fileName = "Candy_Bomb")]
    public class CandyBombConfig : ItemConfig
    {
        public const int MIN_RADIUS = 1;

        [Tooltip("Радиус взрыва в клетках вокруг бомбы")]
        [SerializeField, Min(MIN_RADIUS)] private int _radius = 1;

        public int Radius => Mathf.Max(MIN_RADIUS, _radius);
    }
}
