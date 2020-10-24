using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions.Must;

[CustomEditor(typeof(ReferenceCollector))]
public class ReferenceCollectorEditor : Editor
{
    private ReferenceCollector referenceCollector;

    private string searchKey
    {
        get
        {
            return _searchKey;
        }
        set
        {
            _searchKey = value;
            if (value != null)
                searchObject = referenceCollector.Get(_searchKey);
        }
    }
    private string _searchKey;
    private UnityEngine.Object searchObject;

    private void OnEnable()
    {
        referenceCollector = (ReferenceCollector)target;
    }

    /// <summary>
    /// 删除引用为空的元素
    /// </summary>
    private void DelNullItem()
    {
        SerializedObject serializedObject = new SerializedObject(referenceCollector);
        SerializedProperty serializedData = serializedObject.FindProperty("data");
        SerializedProperty element = null;
        for (int i = serializedData.arraySize - 1; i >= 0; i--)
        {
            element = serializedData.GetArrayElementAtIndex(i);
            if (element.FindPropertyRelative("gameObject").objectReferenceValue == null)
                serializedData.DeleteArrayElementAtIndex(i);
        }

        EditorUtility.SetDirty(referenceCollector);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    /// <summary>
    /// 删除所有元素
    /// </summary>
    private void DelAllItem()
    {
        SerializedProperty serializedProperty = serializedObject.FindProperty("data");
        for (int i = serializedProperty.arraySize - 1; i >= 0; --i)
        {
            serializedProperty.DeleteArrayElementAtIndex(i);
        }
        EditorUtility.SetDirty(referenceCollector);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }


    public override void OnInspectorGUI()
    {
        Undo.RecordObject(referenceCollector, "Changed Setting");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("删除空引用"))
            DelNullItem();
        if (GUILayout.Button("删除全部"))
            DelAllItem();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        searchKey = EditorGUILayout.TextField(searchKey);
        searchObject = EditorGUILayout.ObjectField(searchObject, typeof(Object), false);
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        List<int> delIndex = new List<int>();
        SerializedProperty serializedData = serializedObject.FindProperty("data");
        SerializedProperty element = null;
        for (int i = 0; i < serializedData.arraySize; ++i)
        {
            EditorGUILayout.BeginHorizontal();

            element = serializedData.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("key").stringValue = EditorGUILayout.TextField(element.FindPropertyRelative("key").stringValue);
            element.FindPropertyRelative("gameObject").objectReferenceValue = EditorGUILayout.ObjectField(element.FindPropertyRelative("gameObject").objectReferenceValue, typeof(Object), false);
            if (GUILayout.Button("X"))
                delIndex.Add(i);
            EditorGUILayout.EndHorizontal();
        }
        for (int i = delIndex.Count - 1; i >= 0; --i)
        {
            serializedData.DeleteArrayElementAtIndex(delIndex[i]);
        }

        EventType eventType = Event.current.type;
        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (eventType == EventType.DragPerform)
            {
                foreach (var o in DragAndDrop.objectReferences)
                {
                    AddReference(serializedData, o.name, o);
                }

            }
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    private void AddReference(SerializedProperty serializedData, string key, Object gameObject)
    {
        int index = serializedData.arraySize;
        serializedData.InsertArrayElementAtIndex(index);
        SerializedProperty serializedProperty = serializedData.GetArrayElementAtIndex(index);
        serializedProperty.FindPropertyRelative("key").stringValue = key;
        serializedProperty.FindPropertyRelative("gameObject").objectReferenceValue = gameObject;
    }
}
