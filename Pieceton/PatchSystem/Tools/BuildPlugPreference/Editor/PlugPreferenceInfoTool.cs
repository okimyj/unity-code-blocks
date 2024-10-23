using UnityEditor;
using Pieceton.PatchSystem;

[CustomEditor(typeof(PlugPreferenceInfo))]
public class PlugPreferenceInfoTool : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("The Preference of the Pieceton does not support Inspector.");
    }
}