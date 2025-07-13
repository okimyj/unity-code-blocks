using UnityEngine;
using UnityEditor;

public class EditorWindowUtility : EditorWindow
{
	#region Common Draw Helpers
	public void DrawSpaces(int num = 1)
	{
		for (int i = 0; i < num; i++)
		{
			EditorGUILayout.Space();
		}
	}
	public void DrawHorizontalSection(System.Action drawAction)
	{
		EditorGUILayout.BeginHorizontal();
		drawAction.Invoke();
		EditorGUILayout.EndHorizontal();
	}
	public void DrawVerticalSection(System.Action drawAction)
	{
		EditorGUILayout.BeginVertical();
		drawAction.Invoke();
		EditorGUILayout.EndVertical();
	}
	public void DrawLabelField(string label, bool fitTextSize = true, float size = 0f)
	{
		float maxWidth = fitTextSize ? GetLabelSize(label) : size;
		EditorGUILayout.LabelField(label, GUILayout.MaxWidth(maxWidth));
	}
	public float GetLabelSize(string labelText)
	{
		return GUI.skin.label.CalcSize(new GUIContent(labelText)).x;
	}
	#endregion
}
