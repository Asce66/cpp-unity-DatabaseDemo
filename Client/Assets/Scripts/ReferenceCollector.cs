using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
{
    /// <summary>
    /// key-value键值对,用于面板数据存放
    /// </summary>
    [System.Serializable]
    public class ReferenceData
    {
        public string key;
        public Object gameObject;
    }

    //序列化的List
    public List<ReferenceData> data = new List<ReferenceData>();
    //存放实际的引用对象(Object并非C#的Object而是UnityEngine的)
    public readonly Dictionary<string, Object> dict = new Dictionary<string, Object>();
#if UNITY_EDITOR
    public void Add(string key, UnityEngine.Object gameObject)
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        UnityEditor.SerializedProperty serializedProperty = serializedObject.FindProperty("data");
        int i = 0;
        //搜素要添加的key是否已经在字典中
        for (; i < data.Count; ++i)
        {
            if (data[i].key == key)
                break;
        }
        //i位于字典末尾，则key不在字典内,需要添加数据
        if (i == data.Count)
        {
            serializedProperty.InsertArrayElementAtIndex(i);
        }
        UnityEditor.SerializedProperty element = serializedProperty.GetArrayElementAtIndex(i);
        element.FindPropertyRelative("key").stringValue = key;
        element.FindPropertyRelative("gameObject").objectReferenceValue = gameObject;

        //应用与更新
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }

    public void Remove(string key)
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(this);
        UnityEditor.SerializedProperty serializedData = serializedObject.FindProperty("data");
        int i = 0;
        //查找list中查找是否存在
        for (; i < data.Count; ++i)
        {
            if (data[i].key == key)
                return;
        }
        if (i < data.Count)
        {
            serializedData.DeleteArrayElementAtIndex(i);
        }

        //应用于更新
        UnityEditor.EditorUtility.SetDirty(this);
        serializedObject.ApplyModifiedProperties();
        serializedObject.UpdateIfRequiredOrScript();
    }
#endif

    public T Get<T>(string key) where T : class
    {
        if (dict.ContainsKey(key))
            return dict[key] as T;
        return null;
    }

    public Object Get(string key)
    {
        if (dict.ContainsKey(key))
            return dict[key];
        return null;
    }

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        dict.Clear();
        foreach (var item in data)
        {
            if (dict.ContainsKey(item.key) == false)
                dict[item.key] = item.gameObject;
        }
    }

}
