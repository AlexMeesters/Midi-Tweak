using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Used to have a good summary of used interfaces.
    /// Easy methodology of checking if an object has specific interfaces implemented.
    /// </summary>

    public class InterfaceManager
    {
        private List<IListenToParameterChange> parameterChangeInterfaces = new List<IListenToParameterChange>();
        private List<IListenToParameterLoad> parameterLoadInterfaces = new List<IListenToParameterLoad>();
        private List<IListenToScopeChange> scopeChangeInteraces = new List<IListenToScopeChange>();
        private List<IListenToSetFrozen> setFrozenInterfaces = new List<IListenToSetFrozen>();
        private List<IListenToValueChange> valueChangeInterfaces = new List<IListenToValueChange>();
        private List<IListenToZoomChange> zoomChangeInterfaces = new List<IListenToZoomChange>();
        private List<ILoadKeyAssignmentList> keyAssignmentInterfaces = new List<ILoadKeyAssignmentList>();

        /// <summary>
        /// Scans object if it contains any of the midi tweak interfaces. Adds them to lists for notification.
        /// </summary>
        public void ScanAddInterfaces(GameObject _gameObject)
        {
            RetrieveAddInterfaces(parameterChangeInterfaces, _gameObject);
            RetrieveAddInterfaces(parameterLoadInterfaces, _gameObject);
            RetrieveAddInterfaces(scopeChangeInteraces, _gameObject);
            RetrieveAddInterfaces(setFrozenInterfaces, _gameObject);
            RetrieveAddInterfaces(valueChangeInterfaces, _gameObject);
            RetrieveAddInterfaces(zoomChangeInterfaces, _gameObject);
            RetrieveAddInterfaces(keyAssignmentInterfaces, _gameObject);
        }

        public void AddInterface(IListenToParameterChange _interface) { parameterChangeInterfaces.Add(_interface);  }
        public void AddInterface(IListenToParameterLoad _interface) { parameterLoadInterfaces.Add(_interface); }
        public void AddInterface(IListenToScopeChange _interface) { scopeChangeInteraces.Add(_interface); }
        public void AddInterface(IListenToSetFrozen _interface) { setFrozenInterfaces.Add(_interface); }
        public void AddInterface(IListenToValueChange _interface) { valueChangeInterfaces.Add(_interface); }
        public void AddInterface(IListenToZoomChange _interface) { zoomChangeInterfaces.Add(_interface); }
        public void AddInterface(ILoadKeyAssignmentList _interface) { keyAssignmentInterfaces.Add(_interface); }

        public void RetrieveAddInterfaces<T>(List<T> _targetList, GameObject _gameObject)
        {
            T[] retrieveInterfaces = _gameObject.GetComponentsInChildren<T>();

            for (int i = 0; i < retrieveInterfaces.Length; i++)
            {
                _targetList.Add(retrieveInterfaces[i]);
            }
        }

        public void NotifyParameterChange(string _gameObject, string _name, int _index, int _midiIndex)
        {
            parameterChangeInterfaces.ForEach((_action) => _action.OnParameterChange(_gameObject, _name, _index, _midiIndex));
        }

        public void NotifyParameterLoad(MidiBookmark[] _parameters)
        {
            parameterLoadInterfaces.ForEach((_action) => _action.OnLoadParameters(_parameters));
        }

        public void NotifyScopeChange(float _min, float _mid, float _max)
        {
            scopeChangeInteraces.ForEach((_action) => _action.OnScopeChange(_min, _mid, _max));
        }

        public void NotifySetFrozen(bool _frozen)
        {
            setFrozenInterfaces.ForEach((_action) => _action.OnSetFrozen(_frozen));
        }

        public void NotifyValueChange(float _value)
        {
            valueChangeInterfaces.ForEach((_action) => _action.OnValueChange(_value));
        }

        public void NotifyZoomChange(float _value)
        {
            zoomChangeInterfaces.ForEach((_action) => _action.OnZoomChange(_value));
        }

        public void NotifyKeyAssignment(KeyBinding[] _keyBindings)
        {
            keyAssignmentInterfaces.ForEach((_action) => _action.OnKeyAssignmentsLoaded(_keyBindings));
        }
    }

}