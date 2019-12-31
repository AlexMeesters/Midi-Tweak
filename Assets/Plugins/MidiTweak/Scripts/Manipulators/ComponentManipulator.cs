using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Lowscope.Miditweak
{
    [System.Serializable]
    public class ComponentManipulator
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public List<ParameterDataCollection> parameterDataCollection = new List<ParameterDataCollection>();

        private object GetFieldParent(Component _component, ref FieldData _fieldData)
        {
            if (_component == null)
            {
                return null;
            }

            var getField = _component.GetType().GetField(_fieldData.parentFields[0], bindingFlags);

            if (getField == null)
            {
                return null;
            }

            object obj = getField.GetValue(_component);

            for (int i = 1; i < _fieldData.parentFields.Count; i++)
            {
                obj = obj.GetType().GetField(_fieldData.parentFields[i], bindingFlags).GetValue(obj);
            }

            return obj;
        }

        public float SetFieldValue(Component _component, FieldData _fieldData, float _value)
        {

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(_component);
                EditorSceneManager.MarkSceneDirty(_component.gameObject.scene);
            }
#endif

            if (_fieldData.parentFields.Count > 0)
            {
                object obj = GetFieldParent(_component, ref _fieldData);

                FieldInfo getParentFieldInfo = obj.GetType().GetField(_fieldData.fieldName, bindingFlags);

                var convertedVariable = System.Convert.ChangeType(_value, getParentFieldInfo.FieldType);

                getParentFieldInfo.SetValue(obj, convertedVariable);

                return System.Convert.ToSingle(convertedVariable);
            }
            else
            {
                FieldInfo getFieldInfo = _component.GetType().GetField(_fieldData.fieldName, bindingFlags);

                var convertedVariable = System.Convert.ChangeType(_value, getFieldInfo.FieldType);

                getFieldInfo.SetValue(_component, convertedVariable);

                return System.Convert.ToSingle(convertedVariable);
            }
        }

        public object GetFieldValue(Component _component, FieldData _fieldData)
        {
            if (_fieldData.parentFields.Count > 0)
            {
                object obj = GetFieldParent(_component, ref _fieldData);

                if (obj == null)
                {
                    return null;
                }

                return obj.GetType().GetField(_fieldData.fieldName, bindingFlags).GetValue(obj);
            }
            else
            {
                if (_component == null)
                {
                    return null;
                }

                FieldInfo getFieldInfo = _component.GetType().GetField(_fieldData.fieldName, bindingFlags);

                return (getFieldInfo == null)? null : getFieldInfo.GetValue(_component);
            }
        }

#if UNITY_EDITOR

        public void StoreFieldData(Component _component, FieldData _fieldData)
        {
            int getGameObjectIndex = GetStoredGameObjectIndex(_component.gameObject);
            int getComponentIndex = GetStoredComponentIndex(_component);

            if (getComponentIndex != -1)
            {
                parameterDataCollection[getGameObjectIndex].data[getComponentIndex].fieldData.Add(_fieldData);
            }
            else
            {
                ParameterDataCollection getSaveData = null;

                if (getGameObjectIndex == -1)
                {
                    getSaveData = new ParameterDataCollection();
                    getSaveData.targetGameobject = _component.gameObject;
                    parameterDataCollection.Add(getSaveData);
                }
                else
                {
                    getSaveData = parameterDataCollection[getGameObjectIndex];
                }

                ParameterData newReflectionData = new ParameterData();
                newReflectionData.component = _component;
                newReflectionData.componentid = _component.GetInstanceID();
                newReflectionData.fieldData.Add(_fieldData);
                getSaveData.data.Add(newReflectionData);
            }
        }

        public bool RemoveFieldData(Component _component, FieldData _fieldData)
        {
            if (_component == null || _component.gameObject == null)
            {
                return false;
            }

            int getGameObjectIndex = GetStoredGameObjectIndex(_component.gameObject);
            int getComponentIndex = GetStoredComponentIndex(_component);

            if (getGameObjectIndex != -1 && getComponentIndex != -1)
            {
                foreach (var item in parameterDataCollection[getGameObjectIndex].data[getComponentIndex].fieldData)
                {
                    if (item.fieldName == _fieldData.fieldName)
                    {
                        parameterDataCollection[getGameObjectIndex].data[getComponentIndex].fieldData.Remove(item);

                        if (parameterDataCollection[getGameObjectIndex].data[getComponentIndex].fieldData.Count == 0)
                        {
                            parameterDataCollection[getGameObjectIndex].data.RemoveAt(getComponentIndex);
                        }

                        if (parameterDataCollection[getGameObjectIndex].data.Count == 0)
                        {
                            parameterDataCollection.RemoveAt(getGameObjectIndex);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public void ClearAllEmptyReferences()
        {
            for (int i = 0; i < parameterDataCollection.Count; i++)
            {
                for (int i2 = 0; i2 < parameterDataCollection[i].data.Count; i2++)
                {
                    if (parameterDataCollection[i].data[i2].component == null || parameterDataCollection[i].data[i2].component.GetType() == typeof(UnityEngine.Component))
                    {
                        parameterDataCollection[i].data.RemoveAt(i2);
                        continue;
                    }
                }

                if (parameterDataCollection[i].targetGameobject == null || parameterDataCollection[i].data.Count == 0)
                {
                    parameterDataCollection.RemoveAt(i);
                    continue;
                }
            }
        }

        /// <summary>
        /// Ensure that the field data still exists
        /// </summary>
        public bool ValidateFieldData(Component _component, FieldData _fieldData)
        {
            int getGameObjectIndex = GetStoredGameObjectIndex(_component.gameObject);
            int getComponentIndex = GetStoredComponentIndex(_component);

            if (getGameObjectIndex != -1 && getComponentIndex != -1)
            {
                foreach (var item in parameterDataCollection[getGameObjectIndex].data[getComponentIndex].fieldData)
                {
                    if (item.fieldName == _fieldData.fieldName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets an existing stored component within the save data
        /// </summary>
        /// <param name="_component"></param>
        /// <returns> Returns the index of the reflection data, which contains the component references </returns>
        private int GetStoredComponentIndex(Component _component)
        {
            int getGameObjectIndex = GetStoredGameObjectIndex(_component.gameObject);

            if (getGameObjectIndex == -1)
            {
                return -1;
            }

            for (int i = 0; i < parameterDataCollection[getGameObjectIndex].data.Count; i++)
            {
                if (parameterDataCollection[getGameObjectIndex].data[i].component == _component)
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetStoredGameObjectIndex(GameObject _gameObject)
        {
            if (_gameObject == null)
            {
                return -1;
            }

            for (int i = 0; i < parameterDataCollection.Count; i++)
            {
                if (parameterDataCollection[i].targetGameobject == _gameObject)
                {
                    return i;
                }
            }

            return -1;
        }

        public void RecoverComponentsByInstanceID()
        {
            for (int i = 0; i < parameterDataCollection.Count; i++)
            {
                for (int i2 = 0; i2 < parameterDataCollection[i].data.Count; i2++)
                {
                    if (parameterDataCollection[i].data[i2].component == null)
                    {
                        UnityEngine.Object getObject = EditorUtility.InstanceIDToObject(parameterDataCollection[i].data[i2].componentid);
                        if (getObject != null)
                        {
                            parameterDataCollection[i].data[i2].component = (Component)getObject;
                        }
                    }
                }
            }
        }
        #endif
    }
}