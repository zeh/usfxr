using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SfxrSound))]
public class SfxrSoundEditor : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		if (property.isExpanded)
			return EditorGUIUtility.singleLineHeight * 5;
		else
			return EditorGUIUtility.singleLineHeight;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (property.isExpanded)
			DrawUnfolded(position, property, label);
		else
			DrawFolded(position, property, label);
	}

	private void DrawFolded(Rect position, SerializedProperty property, GUIContent label) {
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, property.name);
	}

	private void DrawUnfolded(Rect position, SerializedProperty property, GUIContent label) {
		Rect labelRect = new Rect(position.xMin, position.yMin, position.width, EditorGUIUtility.singleLineHeight);
		Rect dropdownRect = new Rect(position.xMin, labelRect.yMax, position.width, EditorGUIUtility.singleLineHeight);
		Rect cachedRect = new Rect(position.xMin, dropdownRect.yMax, position.width, EditorGUIUtility.singleLineHeight);
		Rect mutationsRect = new Rect(position.xMin, cachedRect.yMax, position.width, EditorGUIUtility.singleLineHeight);
		Rect factorRect = new Rect(position.xMin, mutationsRect.yMax, position.width, EditorGUIUtility.singleLineHeight);

		var soundProperty = property.FindPropertyRelative("sound");
		var cachedProperty = property.FindPropertyRelative("cached");
		var mutationsProperty = property.FindPropertyRelative("mutations");
		var factorProperty = property.FindPropertyRelative("mutationFactor");

		var soundContainer = SfxrSoundContainer.Create();

		property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, property.name);

		if (soundContainer.IsEmpty) {
			EditorGUI.Popup(dropdownRect, "Sound", 0, new string[] { "[No sounds saved]" });
		} else {
			string sound = soundProperty.stringValue;
			List<string> titles = new List<string>(soundContainer.GetTitles());
			int newIdx = 0;
			if (!string.IsNullOrEmpty(sound) && titles.Contains(sound)) {
				int idx = titles.IndexOf(sound);
				newIdx = EditorGUI.Popup(dropdownRect, "Sound", idx, titles.ToArray());
			} else {
				newIdx = EditorGUI.Popup(dropdownRect, "Sound", 0, titles.ToArray());
			}

			soundProperty.stringValue = titles[newIdx];
		}

		EditorGUI.BeginChangeCheck();
		bool newCached = EditorGUI.Toggle(cachedRect, "Cached", cachedProperty.boolValue);
		if (EditorGUI.EndChangeCheck())
			cachedProperty.boolValue = newCached;

		EditorGUI.BeginChangeCheck();
		int newMutations = EditorGUI.IntField(mutationsRect, "Mutations", mutationsProperty.intValue);
		if (EditorGUI.EndChangeCheck())
			mutationsProperty.intValue = Mathf.Clamp(newMutations, 0, 100);

		EditorGUI.BeginChangeCheck();
		float newFactor = EditorGUI.FloatField(factorRect, "Mutation factor", factorProperty.floatValue);
		if (EditorGUI.EndChangeCheck())
			factorProperty.floatValue = newFactor;
	}
}
