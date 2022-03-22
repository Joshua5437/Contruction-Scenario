using UnityEditor;
using UnityEngine;

namespace Manus.Polygon
{
	[CustomPropertyDrawer(typeof(Bone))]
	public class BoneDrawer : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI(Rect p_Position, SerializedProperty p_Property, GUIContent p_Label)
		{
			EditorGUI.BeginProperty(p_Position, p_Label, p_Property);
			var t_Indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel += 0;
			
			// Calculate rects
			float t_Margin = 1f;
			float t_IconSize = EditorGUIUtility.singleLineHeight - t_Margin * 2f;

			bool t_Optional = p_Property.FindPropertyRelative("optional").boolValue;
			bool t_Filled = p_Property.FindPropertyRelative("bone").objectReferenceValue as Transform != null;

			var t_AmountRect = new Rect(p_Position.x, p_Position.y, p_Position.width - t_IconSize - t_Margin, p_Position.height);
			var t_OptionalRect = new Rect(p_Position.x -5f, p_Position.y + t_Margin, t_IconSize, t_IconSize);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(t_AmountRect, p_Property.FindPropertyRelative("bone"), p_Label);

			Texture t_Texture = null;
			if (!t_Optional)
			{
				if (t_Filled)
					t_Texture = (Texture)Resources.Load("Editor/requiredBoneFilled");
				else
					t_Texture = (Texture)Resources.Load("Editor/requiredBoneEmpty");
			} 
			else
			{
				if (t_Filled)
					t_Texture = (Texture)Resources.Load("Editor/optionalBoneFilled");
				else
					t_Texture = (Texture)Resources.Load("Editor/optionalBoneEmpty");
			}

			GUI.DrawTexture(t_OptionalRect, t_Texture);
			EditorGUI.indentLevel = t_Indent;

			EditorGUI.EndProperty();
		}
	}
}
