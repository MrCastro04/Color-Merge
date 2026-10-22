using System;
using System.Linq;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Level.Scripts.Conditions;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Editor
{
    // Инспектор LevelConfig: список условий ([SerializeReference]) с кнопкой «+ Add condition» — меню типов условий,
    // у каждого условия кнопка удаления. Ниже — подсказки, если условия настроены неработоспособно.
    [CustomEditor(typeof(LevelConfig))]
    public class LevelConfigEditor : UnityEditor.Editor
    {
        private const string CONDITIONS_FIELD = "_conditions";

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (property.propertyPath == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(property);

                    continue;
                }

                if (property.propertyPath == CONDITIONS_FIELD)
                {
                    DrawConditions(property.Copy());
                    continue;
                }

                EditorGUILayout.PropertyField(property, true);
            }

            serializedObject.ApplyModifiedProperties();

            DrawWarnings((LevelConfig)target);
        }

        private void DrawConditions(SerializedProperty conditions)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Conditions", EditorStyles.boldLabel);

            for (int i = 0; i < conditions.arraySize; i++)
            {
                var element = conditions.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(GetTypeName(element.managedReferenceFullTypename), EditorStyles.boldLabel);

                bool isRemoved = GUILayout.Button("✕", GUILayout.Width(24));

                EditorGUILayout.EndHorizontal();

                if (!isRemoved)
                    DrawConditionFields(element);

                EditorGUILayout.EndVertical();

                if (isRemoved)
                {
                    element.managedReferenceValue = null;
                    conditions.DeleteArrayElementAtIndex(i);
                    break;
                }
            }

            if (GUILayout.Button("+ Add condition"))
                ShowAddMenu(conditions);
        }

        private static void DrawConditionFields(SerializedProperty element)
        {
            var child = element.Copy();
            var end = element.GetEndProperty();
            bool enterChildren = true;

            while (child.NextVisible(enterChildren) && !SerializedProperty.EqualContents(child, end))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(child, true);
            }
        }

        // Timer в списке может быть только один.
        private void ShowAddMenu(SerializedProperty conditions)
        {
            var config = (LevelConfig)target;
            var menu = new GenericMenu();

            var types = TypeCache.GetTypesDerivedFrom<LevelCondition>()
                .Where(type => !type.IsAbstract)
                .OrderBy(type => type.Name);

            foreach (var type in types)
            {
                var label = new GUIContent(type.Name);

                if (type == typeof(Scripts.Conditions.Timer) && config.HasTimer)
                    menu.AddDisabledItem(label);
                else
                    menu.AddItem(label, false, () => AddCondition(type));
            }

            menu.ShowAsContext();
        }

        private void AddCondition(Type type)
        {
            serializedObject.Update();

            var conditions = serializedObject.FindProperty(CONDITIONS_FIELD);
            conditions.arraySize++;
            conditions.GetArrayElementAtIndex(conditions.arraySize - 1).managedReferenceValue = Activator.CreateInstance(type);

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawWarnings(LevelConfig config)
        {
            var conditions = config.Conditions;
            var itemGoals = conditions.OfType<Get_Item_With_Level_X>().ToList();
            var generatorGoals = conditions.OfType<Upgraded_Generator_With_Level_X>().ToList();
            var mergeGoals = conditions.OfType<Make_Merge_X_Times>().ToList();

            if (conditions.Any(condition => condition == null))
                EditorGUILayout.HelpBox("В списке есть пустое условие — удали его.", MessageType.Error);

            if (itemGoals.Count == 0 && generatorGoals.Count == 0 && mergeGoals.Count == 0)
                EditorGUILayout.HelpBox("Нет условий-целей (Get_Item / Upgraded_Generator / Make_Merge): уровень пройдётся с первым мерджем.", MessageType.Warning);

            if (config.InitialItems.Any(item => config.BlockedSlots.Contains(item.SlotID)) || config.InitialCandies.Any(candy => config.BlockedSlots.Contains(candy.SlotID)))
                EditorGUILayout.HelpBox("Предмет стоит на стене (Blocked Slots) — в игре он не появится.", MessageType.Info);

            if (config.InitialCandies.Count > 0 && config.CandyConnector == null)
                EditorGUILayout.HelpBox("Конфеты на поле есть, а Candy Connector не задан — они не появятся.", MessageType.Error);

            if (!config.HasTimer && conditions.OfType<TimedLevelCondition>().Any(condition => condition.HasTimer))
                EditorGUILayout.HelpBox("Лимит времени (_hasTimer) работает только вместе с условием Timer.", MessageType.Warning);

            if (generatorGoals.Count > 0 && !config.IsGeneratorUpgradeEnabled)
                EditorGUILayout.HelpBox("Upgraded_Generator без кнопки улучшения (_generatorUpgradeEnabled) не выполнить.", MessageType.Error);

            if (config.ItemChain == null)
                return;

            int chainLength = config.ItemChain.Items.Count;

            if (itemGoals.Any(goal => goal.ItemLevel > chainLength) || generatorGoals.Any(goal => goal.GeneratorLevel > chainLength))
                EditorGUILayout.HelpBox($"Уровень в условии больше длины цепочки ({chainLength}).", MessageType.Error);

            if (itemGoals.Count > 0 && generatorGoals.Any(goal => goal.GeneratorLevel >= config.TargetItemLevel))
                EditorGUILayout.HelpBox("Генератор, прокачанный до уровня цели-предмета, будет выдавать её сам.", MessageType.Warning);
        }

        // «Content Assets.Game...Conditions.Get_Item_With_Level_X» → «Get_Item_With_Level_X».
        private static string GetTypeName(string managedReferenceFullTypename)
        {
            if (string.IsNullOrEmpty(managedReferenceFullTypename))
                return "Missing condition";

            int lastDot = managedReferenceFullTypename.LastIndexOf('.');
            return lastDot < 0 ? managedReferenceFullTypename : managedReferenceFullTypename.Substring(lastDot + 1);
        }
    }
}
