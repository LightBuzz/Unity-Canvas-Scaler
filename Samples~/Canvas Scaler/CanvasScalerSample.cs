using LightBuzz.UI;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerSample : MonoBehaviour
{
    [SerializeField] private AutoCanvasScaler _scaler;
    [SerializeField] private Text _info;

    public void OnValueChanged(float value)
    {
        _scaler.TargetDPI = value;
        _info.text = value.ToString("N0");
    }
}
