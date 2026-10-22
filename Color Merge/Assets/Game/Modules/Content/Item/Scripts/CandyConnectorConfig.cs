using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    // Особый предмет «Candy_Connector»: не входит в цепочку.
    // Предмет уровня X + конфета (в любую сторону) → предмет уровня X + 1 на месте цели.
    // Две конфеты между собой не мерджатся — просто меняются местами. Ставится на поле списком LevelConfig._initialCandies.
    [CreateAssetMenu(menuName = "Game/Item/Candy Connector Config", fileName = "Candy_Connector")]
    public class CandyConnectorConfig : ItemConfig
    {
    }
}
