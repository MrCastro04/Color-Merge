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
                EditorGUILayout.HelpBox("У списку є порожня умова — видали її.", MessageType.Error);

            if (itemGoal == null && generatorGoal == null && mergeGoal == null)
                EditorGUILayout.HelpBox("Немає умов-цілей (Get_Item / Upgraded_Generator / Make_Merge): рівень пройдеться з першим мерджем.", MessageType.Warning);

            if (config.InitialItems.Any(item => config.BlockedSlots.Contains(item.SlotID)) || config.InitialCandies.Any(candy => config.BlockedSlots.Contains(candy.SlotID)))
                EditorGUILayout.HelpBox("Предмет стоїть на стіні (Blocked Slots) — у грі він не з’явиться.", MessageType.Info);

            if (config.CandyBomb != null && config.CandyConnector == null)
                EditorGUILayout.HelpBox("Є Candy Bomb, але немає Candy Connector — вибух не створить цукерки.", MessageType.Warning);

            if (config.InitialCandies.Count > 0 && config.CandyConnector == null)
                EditorGUILayout.HelpBox("Цукерки на полі є, а Candy Connector не задано — вони не з’являться.", MessageType.Error);

            if (generatorGoal != null && !config.IsGeneratorUpgradeEnabled)
                EditorGUILayout.HelpBox("Upgraded_Generator без кнопки покращення (_generatorUpgradeEnabled) не виконати.", MessageType.Error);

            if (config.ItemChain == null)
                return;

            int chainLength = config.ItemChain.Items.Count;

            if (itemGoal?.ItemLevel > chainLength || generatorGoal?.GeneratorLevel > chainLength)
                EditorGUILayout.HelpBox($"Рівень в умові більший за довжину ланцюжка ({chainLength}).", MessageType.Error);

            if (itemGoal != null && generatorGoal?.GeneratorLevel >= config.TargetItemLevel)
                EditorGUILayout.HelpBox("Генератор, прокачаний до рівня цілі-предмета, видаватиме її сам.", MessageType.Warning);
        }
    }
}
