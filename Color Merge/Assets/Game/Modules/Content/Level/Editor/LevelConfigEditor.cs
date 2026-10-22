using System.Linq;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Level.Scripts.Conditions;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Assets.Game.Modules.Content.Level.Editor
{
    [CustomEditor(typeof(LevelConfig))]
    public class LevelConfigEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawWarnings((LevelConfig)target);
        }

        private static void DrawWarnings(LevelConfig config)
        {
            var conditions = config.Conditions;
            var itemGoal = conditions.OfType<Get_Item_With_Level_X>().FirstOrDefault();
            var generatorGoal = conditions.OfType<Upgraded_Generator_With_Level_X>().FirstOrDefault();
            var mergeGoal = conditions.OfType<Make_Merge_X_Times>().FirstOrDefault();

            if (conditions.Any(condition => condition == null))
                EditorGUILayout.HelpBox("В списке есть пустое условие — удали его.", MessageType.Error);

            if (itemGoal == null && generatorGoal == null && mergeGoal == null)
                EditorGUILayout.HelpBox("Нет условий-целей (Get_Item / Upgraded_Generator / Make_Merge): уровень пройдётся с первым мерджем.", MessageType.Warning);

            if (config.InitialItems.Any(item => config.BlockedSlots.Contains(item.SlotID)) || config.InitialCandies.Any(candy => config.BlockedSlots.Contains(candy.SlotID)))
                EditorGUILayout.HelpBox("Предмет стоит на стене (Blocked Slots) — в игре он не появится.", MessageType.Info);

            if (config.CandyBomb != null && config.CandyConnector == null)
                EditorGUILayout.HelpBox("Есть Candy Bomb, но нет Candy Connector — взрыв не создаст конфеты.", MessageType.Warning);

            if (config.InitialCandies.Count > 0 && config.CandyConnector == null)
                EditorGUILayout.HelpBox("Конфеты на поле есть, а Candy Connector не задан — они не появятся.", MessageType.Error);

            if (generatorGoal != null && !config.IsGeneratorUpgradeEnabled)
                EditorGUILayout.HelpBox("Upgraded_Generator без кнопки улучшения (_generatorUpgradeEnabled) не выполнить.", MessageType.Error);

            if (config.ItemChain == null)
                return;

            int chainLength = config.ItemChain.Items.Count;

            if (itemGoal?.ItemLevel > chainLength || generatorGoal?.GeneratorLevel > chainLength)
                EditorGUILayout.HelpBox($"Уровень в условии больше длины цепочки ({chainLength}).", MessageType.Error);

            if (itemGoal != null && generatorGoal?.GeneratorLevel >= config.TargetItemLevel)
                EditorGUILayout.HelpBox("Генератор, прокачанный до уровня цели-предмета, будет выдавать её сам.", MessageType.Warning);
        }
    }
}
