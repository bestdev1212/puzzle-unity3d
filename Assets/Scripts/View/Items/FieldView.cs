using Core.Items;
using UnityEngine;
using View.Control;

namespace View.Items
{
    /// <summary>
    /// A FieldView represents the view for an field in the gameboard. It is responsible
    /// for visualizing fields that highlight the moves that are available.
    /// </summary>
    public class FieldView : MonoBehaviour
    {
        public Color FieldColor => GameDef.Get.FieldColor;

        private ScaleScript _fieldScale;
        private Colorizer _colorizer;

        private Vector3 _initScale;

        public Field Field { get; private set; }

        public NodeView ParentNode { get; private set; }
        public NodeView ConnectedNode { get; private set; }

        public Vector2 HitRect => new Vector3(transform.localScale.x, 1) + transform.localPosition;

        private void Awake()
        {
            _fieldScale = GetComponent<ScaleScript>();
            _colorizer = GetComponent<Colorizer>();
        }

        public void Init(Field field, NodeView parent, NodeView connected)
        {
            Field = field;
            ParentNode = parent;
            ConnectedNode = connected;

            _fieldScale.SetField(field);

            _colorizer.PrimaryColor = FieldColor;
            _colorizer.Fade(0f);
            
            _initScale = transform.localScale;
        }

        public void Highlight(bool enable)
        {
            // TODO: make configurable
            const float time = 0.5f;
            
            if (enable) {
                _colorizer.PulseAppear(time);
                PulseScale(time);
            } else {
                LeanTween.cancel(gameObject);
                _colorizer.Fade(() => {
                    transform.localScale = _initScale;
                });
            }
        }
        
        private void PulseScale(float time)
        {
            // TODO: make configurable
            const float scale = 0.33f;
            
            LeanTween.scale(gameObject, transform.localScale + Vector3.one * scale, time)
                .setEase(LeanTweenType.easeInOutSine)
                .setLoopPingPong(-1);
        }
    }
}
