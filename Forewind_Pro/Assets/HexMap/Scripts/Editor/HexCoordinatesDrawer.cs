using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HexCoordinates))]
public class HexCoordinatesDrawer : PropertyDrawer {

    /// <summary>
    /// 设置编辑器显示属性
    /// </summary>
    /// <param name="position">显示位置</param>
    /// <param name="property">显示内容</param>
    /// <param name="label">显示标识组件</param>
	public override void OnGUI (
		Rect position, SerializedProperty property, GUIContent label
	) {
		HexCoordinates coordinates = new HexCoordinates(
			property.FindPropertyRelative("x").intValue,
			property.FindPropertyRelative("z").intValue
		);

		position = EditorGUI.PrefixLabel(position, label);
		GUI.Label(position, coordinates.ToString());
	}
}