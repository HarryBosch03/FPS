using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class RadialSliderUI : Image
    {
        [Space]
        [SerializeField] int segments;
        [SerializeField] int segmentResolution;
        [SerializeField] float elementSize;
        [SerializeField] float segmentOffset;
        [SerializeField][Range(0.0f, 1.0f)] float width;
        [SerializeField][Range(0.0f, 1.0f)] float segmentGap;
        [SerializeField][Range(0.0f, 1.0f)] float fill;
        [SerializeField]bool flip;

        VertexHelper vh;

        public float Fill
        {
            get => fill;
            set
            {
                fill = value;
                SetAllDirty();
            }
        }
        public int Segments
        {
            get => segments;
            set
            {
                segments = value;
                SetAllDirty();
            }
        }

        float Sin(float aDeg) => Mathf.Sin(aDeg * Mathf.Deg2Rad);
        float Cos(float aDeg) => Mathf.Cos(aDeg * Mathf.Deg2Rad);
        Vector2 FromPolar(float a, float d) => new Vector2(Cos(a), Sin(a)) * d;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
            this.vh = vh;

            float segmentSize = elementSize / segments;

            for (int i = 0; i < (int)(segments * fill) + 1; i++)
            {
                float p = fill * segments - i;
                if (p > 1.0f) p = 1.0f;
                Arc(segmentOffset + segmentSize * i + segmentSize * 0.5f - elementSize * 0.5f, segmentSize * (1.0f - segmentGap), segmentResolution, p);    
            }
        }

        public void Arc (float position, float size, int segments, float percent)
        {
            float segmentSize = size / segments;

            for (int i = 0; i < (int)(segments * percent) + 1; i++)
            {
                var angle = i * segmentSize + position - size * 0.5f;
                var p = percent * segments - i;
                if (p > 1.0f) p = 1.0f;

                var sign = flip ? -1.0f : 1.0f;

                var a = FromPolar((angle + segmentSize) * sign, 1.0f);
                var b = FromPolar(angle * sign, 1.0f);
                var c = FromPolar(angle * sign, 1.0f - width);
                var d = FromPolar((angle + segmentSize) * sign, 1.0f - width);

                float uv1 = 1.0f - (i / (float)segments);
                float uv2 = 1.0f - ((i + p) / segments);

                AddQuad
                (
                    Vector3.Lerp(b, a, p), new Vector2(uv2, 1.0f),
                    b, new Vector2(uv1, 1.0f),
                    c, new Vector2(uv1, 0.0f),
                    Vector3.Lerp(c, d, p), new Vector2(uv2, 0.0f)
                );
            }
        }

        public void AddQuad(Vector2 a, Vector2 aUV, Vector2 b, Vector2 bUV, Vector2 c, Vector2 cUV, Vector2 d, Vector2 dUV)
        {
            int start = vh.currentIndexCount;
            AddVert(a, aUV);
            AddVert(b, bUV);
            AddVert(c, cUV);
            vh.AddTriangle(start, start + 1, start + 2);

            AddVert(c, cUV);
            AddVert(d, dUV);
            AddVert(a, aUV);
            vh.AddTriangle(start + 3, start + 4, start + 5);
        }

        public void AddVert(Vector2 vert, Vector2 uv)
        {
            vh.AddVert(vert * rectTransform.rect.size / 2.0f, color, uv);
        }
    }
}
