using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class SegmentedSliderUI : Image
    {
        [SerializeField] int segments;
        [SerializeField] [Range(0.0f, 1.0f)]float segmentGap;
    
        VertexHelper vh;

        public int Segments { get => segments; set => segments = value; }
    
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            this.vh = vh;

            base.OnPopulateMesh(vh);
            vh.Clear();

            for (int i = 0; i < Mathf.Min(segments, (int)(segments * fillAmount) + 1); i++)
            {
                float p = fillAmount * segments - i;
                if (p > 1.0f) p = 1.0f;

                float actualHeight = segments * (1.0f - segmentGap) + (segments - 1) * segmentGap;
                Vector2 postScale = new Vector2(1.0f, segments / actualHeight);

                AddQuad(new Vector2(0.0f, i / (float)segments) * postScale, new Vector2(1.0f, (1.0f - segmentGap) * p / segments) * postScale);
            }
        }

        public void AddQuad (Vector2 corner, Vector2 size)
        {
            var width = new Vector2(size.x, 0.0f);
            var height = new Vector2(0.0f, size.y);

            AddVertex(corner + size, Vector2.one);
            AddVertex(corner + height, Vector2.up);
            AddVertex(corner, Vector2.zero);
            AddTriangle();

            AddVertex(corner + size, Vector2.one);
            AddVertex(corner + width, Vector2.right);
            AddVertex(corner, Vector2.zero);
            AddTriangle();
        }

        public void AddVertex (Vector2 v, Vector2 uv)
        {
            vh.AddVert(v * rectTransform.rect.size + rectTransform.rect.position, color, uv);
        }

        public void AddTriangle ()
        {
            vh.AddTriangle(vh.currentVertCount - 3, vh.currentVertCount - 2, vh.currentVertCount - 1);
        }
    }
}
