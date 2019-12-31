using UnityEngine;
using UnityEngine.UI;


namespace Lowscope.Miditweak
{
    [AddComponentMenu("")]
    public class UITweakVisualizer : MonoBehaviour, IListenToParameterChange, IListenToParameterLoad,
        IListenToScopeChange, IListenToSetFrozen, IListenToValueChange, IListenToZoomChange
    {
        [SerializeField]
        private Text minValue = null;

        [SerializeField]
        private Text maxValue = null;

        [SerializeField]
        private Text midValue = null;

        [SerializeField]
        private Text currentValue = null;

        [SerializeField]
        private Text parameterName = null;

        [SerializeField]
        private Text midiID = null;

        [SerializeField]
        private Text parameterID = null;

        [SerializeField]
        private Text zoomPercentagePlus = null;

        [SerializeField]
        private Text zoomPercentageMinus = null;

        [SerializeField]
        private Image valuePointer = null;

        [SerializeField]
        private GameObject recenterIndicator = null;

        private float min = 0;
        private float max = 0;
        private bool isFrozen;
        private int totalParameters;

        public void OnParameterChange(string _gameObject, string _variable, int _index, int _midiIndex)
        {
            if (parameterName != null)
            {
                parameterName.text = string.Format("{0} - {1}", _gameObject, _variable);
            }

            if (parameterID != null)
            {
                parameterID.text = string.Format("{0}/{1}", (1 + _index).ToString(), totalParameters.ToString());
            }

            if (midiID != null)
            {
                midiID.text = string.Format("MIDI ID: {0}", _midiIndex);
            }
        }

        public void OnValueChange(float _value)
        {
            if (_value != 0 && !float.IsNaN(_value))
            {
                float getPercentage = (_value - min) / (max - min);

                if (getPercentage != 0 && valuePointer != null)
                {
                    Quaternion getQuaternion = valuePointer.transform.rotation;
                    Vector3 getEuler = getQuaternion.eulerAngles;
                    getEuler.z = (-180 * getPercentage) + 90;
                    getQuaternion.eulerAngles = getEuler;
                    valuePointer.transform.rotation = getQuaternion;
                }

                if (!isFrozen && currentValue != null)
                {
                    currentValue.text = string.Format("{0:0.###}", Mathf.Lerp(min, max, getPercentage));
                }
            }
        }

        public void OnScopeChange(float _min, float _mid, float _max)
        {
            min = _min;
            max = _max;

            if (minValue != null)
            {
                minValue.text = string.Format("{0:0.###}", _min);
            }

            if (maxValue != null)
            {
                maxValue.text = string.Format("{0:0.###}", _max);
            }

            if (midValue != null)
            {
                midValue.text = string.Format("{0:0.###}", _mid);
            }
        }

        public void OnZoomChange(float _value)
        {
            if (zoomPercentagePlus != null)
            {
                zoomPercentagePlus.text = string.Format("+{0}%", _value.ToString());
            }

            if (zoomPercentageMinus != null)
            {
                zoomPercentageMinus.text = string.Format("-{0}%", _value.ToString());
            }
        }

        public void OnSetFrozen(bool _frozen)
        {
            isFrozen = _frozen;

            if (recenterIndicator != null)
            {
                recenterIndicator.gameObject.SetActive(_frozen);
            }

            if (_frozen && currentValue != null)
            {
                currentValue.text = "Recenter";
            }
        }

        public void OnLoadParameters(MidiBookmark[] _paramters)
        {
            totalParameters = _paramters.Length;
        }
    }
}