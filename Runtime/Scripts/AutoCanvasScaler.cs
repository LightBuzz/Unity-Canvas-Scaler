using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LightBuzz.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [AddComponentMenu("Layout/LightBuzz Canvas Scaler")]
    public class AutoCanvasScaler : UIBehaviour
    {
        [Header("Target DPI")]
        [SerializeField]
        [Range(1.0f, 400.0f)]
        [Tooltip("The desired target DPI.")]
        private float _targetDPI = 100.0f;

        private Canvas _canvas;
        private CanvasScaler _scaler;

        private float _previousScaleFactor = 1.0f;

        private float _previousReferencePixelsPerUnit = 100.0f;

        public float TargetDPI
        {
            get => _targetDPI;
            set => _targetDPI = value;
        }

        protected AutoCanvasScaler()
        {
        }

        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_EDITOR
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
#endif
            _canvas = GetComponent<Canvas>();
            _scaler = GetComponent<CanvasScaler>();

            _previousScaleFactor = _scaler.scaleFactor;
            _previousReferencePixelsPerUnit = _scaler.referencePixelsPerUnit;
        }

        protected override void OnDisable()
        {
            SetScaleFactor(1.0f);
            SetReferencePixelsPerUnit(100.0f);

#if UNITY_EDITOR
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
#endif

            base.OnDisable();
        }

        protected virtual void Update()
        {
            Handle();
        }

        protected void Handle()
        {
            if (_canvas == null)
            {
                Debug.LogError("Canvas should not be null.");
                return;
            }
            if (!_canvas.isRootCanvas)
            {
                Debug.LogError("Canvas should be root.");
                return;
            }
                        
            RenderMode mode = _canvas.renderMode;

            float dpi = Screen.dpi != 0 ?
                Screen.dpi :
                _scaler.fallbackScreenDPI;
            
            float scale = mode == RenderMode.WorldSpace ? 
                _scaler.dynamicPixelsPerUnit :
                dpi / _targetDPI;

            float ppu = mode == RenderMode.WorldSpace ?
                _scaler.referencePixelsPerUnit :
                _scaler.referencePixelsPerUnit * _targetDPI / _scaler.defaultSpriteDPI;

            float factor = Mathf.Sqrt(scale);
            scale /= factor;

            SetScaleFactor(scale);
            SetReferencePixelsPerUnit(ppu);
        }

        protected void SetScaleFactor(float value)
        {
            if (value == _previousScaleFactor) return;

            _canvas.scaleFactor = value;
            _previousScaleFactor = value;
        }

        protected void SetReferencePixelsPerUnit(float value)
        {
            if (value == _previousReferencePixelsPerUnit) return;
            
            _canvas.referencePixelsPerUnit = value;
            _previousReferencePixelsPerUnit = value;
        }

#if UNITY_EDITOR
        public void OnAfterAssemblyReload()
        {
            SetScaleFactor(1.0f);
            SetReferencePixelsPerUnit(100.0f);
            Handle();
        }
#endif
    }
}
