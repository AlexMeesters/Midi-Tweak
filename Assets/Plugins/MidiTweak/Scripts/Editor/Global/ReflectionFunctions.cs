using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Lowscope.Miditweak
{
#if UNITY_EDITOR

    public class ReflectionFunctions
    {
        public static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public static List<ParameterData> GetTypeFields(GameObject _targetGameObject)
        {
            Component[] components = _targetGameObject.GetComponents(typeof(UnityEngine.MonoBehaviour));

            List<ParameterData> data = new List<ParameterData>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null)
                {
                    ParameterData newData = new ParameterData();
                    newData.component = components[i];
                    newData.componentid = components[i].GetInstanceID();
                    data.Add(newData);

                    GetFieldDataFromSerializedObject(components[i], newData.fieldData);
                }

            }

            return data;
        }

        public static void GetFieldDataFromSerializedObject(UnityEngine.Object _component, List<FieldData> _fieldData)
        {
            SerializedObject serializedObject = new SerializedObject(_component);
            SerializedProperty prop = serializedObject.GetIterator();

            if (prop.NextVisible(true))
            {
                do
                {
                    System.Type type = null;

                    switch (prop.type)
                    {
                        case "int": type = typeof(int); break;
                        case "float": type = typeof(float); break;

                        default:
                            break;
                    }

                    if (prop == null)
                        continue;

                    object getParent = GetPropertyParent(prop);

                    if (type != null && getParent != null && getParent.GetType().IsClass)
                    {
                        FieldData newFieldData = new FieldData();
                        newFieldData.fieldName = prop.name;
                        newFieldData.fieldType = type;
                        newFieldData.parentFields = CreateParentList(prop.propertyPath);
                        _fieldData.Add(newFieldData);
                    }
                }
                while (prop.NextVisible(true));
            }
        }

        public static List<String> CreateParentList(string _string)
        {
            string[] parents = _string.Split('.');
            List<String> newList = new List<string>(parents);

            newList.RemoveAt(parents.Length - 1);

            return newList;
        }

        public static bool IsNumericType(object obj)
        {
            switch (Type.GetTypeCode(obj.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static object GetPropertyParent(SerializedProperty _property)
        {
            string path = _property.propertyPath.Replace(".Array.data[", "[");
            object obj = _property.serializedObject.targetObject;

            string[] pathElements = path.Split('.');

            foreach (var pathElement in pathElements.Take(pathElements.Length - 1))
            {
                if (pathElement.Contains("["))
                {
                    var elementName = pathElement.Substring(0, pathElement.IndexOf("["));
                    var index = Convert.ToInt32(pathElement.Substring(pathElement.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, pathElement);
                }
            }

            return obj;
        }

        public static object GetValue(object source, string name)
        {
            if (source == null)
            {
                return null;
            }

            Type sourceType = source.GetType();
            FieldInfo fieldInfo = sourceType.GetField(name, ReflectionFunctions.bindingFlags);

            if (fieldInfo == null)
            {
                PropertyInfo property = sourceType.GetProperty(name, ReflectionFunctions.bindingFlags | BindingFlags.IgnoreCase);

                if (property == null)
                {
                    return null;

                }

                return property.GetValue(source, null);
            }

            return fieldInfo.GetValue(source);
        }

        public static object GetValue(object source, string name, int index)
        {
            if (source == null)
                return null;

            IEnumerable enumerable = GetValue(source, name) as IEnumerable;

            if (enumerable == null)
                return null;

            IEnumerator enumerator = enumerable.GetEnumerator();

            while (index-- >= 0)
            {
                enumerator.MoveNext();
            }

            return enumerator.Current;
        }
    }
#endif
}