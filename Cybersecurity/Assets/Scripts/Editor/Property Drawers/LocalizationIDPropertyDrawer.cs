using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Tell the MyRangeDrawer that it is a drawer for properties with the MyRangeAttribute.
[CustomPropertyDrawer(typeof(LocalizationID))]
public class RangeDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Get the data from the attribute
        //LocalizationIDAttribute range = (LocalizationIDAttribute)attribute;

        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use the LocalizationIDAttribute with a string.");
            return;
        }

        //Draw the regular GUI + a field with the localized Dutch text
        property.stringValue = EditorGUI.TextField(new Rect(position.x, position.y, position.width,
                                                   EditorGUIUtility.singleLineHeight), label.text, property.stringValue);

        EditorGUI.HelpBox(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing),
                          "Localized Text: " + LocalizationManager.GetText(property.stringValue, LocalizationManager.Language.Dutch), MessageType.None);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 50;
    }
}