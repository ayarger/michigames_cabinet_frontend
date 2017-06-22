using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class Persistence
{
    private static Dictionary<string, string> recorded_data = new Dictionary<string, string>();

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    public static void SetString(string key, string val)
    {
        PlayerPrefs.SetString(key, val);
    }

    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static void SetInt(string key, int val)
    {
        PlayerPrefs.SetInt(key, val);
    }

    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static void SetFloat(string key, float val)
    {
        PlayerPrefs.SetFloat(key, val);
    }

    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    private static BinaryFormatter bf = new BinaryFormatter();

    // serializableObject is any struct or class marked with [Serializable]
    public static string SaveObject(string prefKey, object serializableObject)
    {
        MemoryStream memoryStream = new MemoryStream();
        bf.Serialize(memoryStream, serializableObject);
        string tmp = System.Convert.ToBase64String(memoryStream.ToArray());
        PlayerPrefs.SetString(prefKey, tmp);
        return tmp;
    }

    // Passing in alt_data causes the function to ignore player_prefs.
    public static T LoadObject<T>(string prefKey, String alt_data = null)
    {
        string serializedData = "";

        if (alt_data == null)
        {
            if (!PlayerPrefs.HasKey(prefKey))
            {
                Debug.LogError("Attempted to Load non-existant Player Pref!");
                return default(T);
            }

            serializedData = PlayerPrefs.GetString(prefKey);
        }
        else
        {
            serializedData = alt_data;
        }

        MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));

        T deserializedObject = (T)bf.Deserialize(dataStream);

        return deserializedObject;
    }
}