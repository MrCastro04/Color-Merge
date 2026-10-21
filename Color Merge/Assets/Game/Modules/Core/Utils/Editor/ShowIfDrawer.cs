using Assets.Game.Modules.Core.Utils.Scripts;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Editor
{
    // Скрывает поле, пока соседнее bool-поле выключено. Работает и внутри [SerializeReference]-элементов списка.
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsVisible(property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsVisible(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : -EditorGUIUtility.standardVerticalSpacing;
        }

        private bool IsVisible(SerializedProperty property)
        {
            var showIf = (ShowIfAttribute)attribute;
            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');
            string siblingPath = lastDot < 0 ? showIf.BoolFieldName : path.Substring(0, lastDot + 1) + showIf.BoolFieldName;

            var boolProperty = property.serializedObject.FindProperty(siblingPath);

            return boolProperty == null || boolProperty.propertyType != SerializedPropertyType.Boolean || boolProperty.boolValue;
        }
    }
}
