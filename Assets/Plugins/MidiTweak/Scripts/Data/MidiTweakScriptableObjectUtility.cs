using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lowscope.Miditweak
{
    public static class MidiTweakScriptableObjectUtility
    {
        private static T CreateAssetAtPath<T>(T _instance, string _path) where T: UnityEngine.Object
        {
            if (!string.IsNullOrEmpty(_path))
            {
                string typeName = typeof(T).ToString();
                typeName = typeName.Substring(typeName.LastIndexOf(".") + 1) + ".asset";

                string newPath = string.Format("{0}{1}{2}", _path, "/", typeName);
                AssetDatabase.CreateAsset(_instance, newPath);
                return _instance;
            }

            return null;
        }

        public static T GetOrCreateScriptableObject<T>(string _resourcePath) where T : ScriptableObject
        {
            T[] getInstances = Resources.LoadAll<T>("");
            T getInstance = null;

            if (getInstances.Length > 0)
            {
                getInstance = getInstances[0];
            }

            if (getInstance == null)
            {
                getInstance = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR

                GameObject dataVizualizer = Resources.Load("MidiTweak_DataVizualizer") as GameObject;

                if (dataVizualizer != null)
                {
                    string path = AssetDatabase.GetAssetPath(dataVizualizer);

                    return CreateAssetAtPath<T>(getInstance, System.IO.Path.GetDirectoryName(path));
                }
                else
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    return CreateAssetAtPath<T>(getInstance, "Assets/Resources");
                }
#endif
            }

            return getInstance;
        }
    }
}
