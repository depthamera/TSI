using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TSI.Editor
{
    /// <summary>
    /// SO 역할별 커스텀 아이콘을 생성하는 에디터 유틸리티.
    /// 메뉴: TSI > Tools > Generate SO Icons
    /// 생성된 아이콘은 각 SO 클래스의 [Icon] 어트리뷰트에서 참조됩니다.
    /// </summary>
    public static class SOIconGenerator
    {
        private const int Size = 64;
        private const string OutputDir = "Assets/_Project/02_Editor/Icons";
        private const float BorderWidth = 2.5f;
        private const float AntiAlias = 1.5f;

        [MenuItem("TSI/Tools/Generate SO Icons")]
        public static void Generate()
        {
            Directory.CreateDirectory(OutputDir);
            var center = new Vector2(Size * 0.5f, Size * 0.5f);

            // Key/Token SO — Blue Hexagon
            CreateIcon("icon_so_key",
                new Color(0.29f, 0.56f, 0.85f),
                new Color(0.16f, 0.36f, 0.62f),
                p => SdfHexagon(p, center, 24f));

            // Config/Profile SO — Green Rounded Square
            CreateIcon("icon_so_config",
                new Color(0.36f, 0.72f, 0.36f),
                new Color(0.20f, 0.50f, 0.20f),
                p => SdfRoundedBox(p, center, new Vector2(22f, 22f), 7f));

            // FSM State SO — Orange Circle
            CreateIcon("icon_so_state",
                new Color(0.94f, 0.68f, 0.31f),
                new Color(0.72f, 0.48f, 0.14f),
                p => SdfCircle(p, center, 24f));

            // FSM Condition SO — Yellow Diamond
            CreateIcon("icon_so_condition",
                new Color(0.97f, 0.86f, 0.44f),
                new Color(0.75f, 0.65f, 0.22f),
                p => SdfDiamond(p, center, 24f));

            // FSM TransitionTable SO — Purple Square
            CreateIcon("icon_so_table",
                new Color(0.61f, 0.35f, 0.71f),
                new Color(0.40f, 0.20f, 0.50f),
                p => SdfBox(p, center, new Vector2(22f, 22f)));

            AssetDatabase.Refresh();
            Debug.Log($"[SOIconGenerator] 5 icons generated → {OutputDir}");
        }

        private static void CreateIcon(string name, Color fill, Color border, Func<Vector2, float> sdf)
        {
            var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);

            for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
            {
                float d = sdf(new Vector2(x + 0.5f, y + 0.5f));
                Color c;

                if (d < -BorderWidth)
                    c = fill;
                else if (d < 0f)
                    c = border;
                else if (d < AntiAlias)
                    c = Color.Lerp(border, Color.clear, d / AntiAlias);
                else
                    c = Color.clear;

                tex.SetPixel(x, y, c);
            }

            tex.Apply();
            File.WriteAllBytes(Path.Combine(OutputDir, $"{name}.png"), tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);
        }

        // ── Signed Distance Functions ──────────────────────────

        private static float SdfCircle(Vector2 p, Vector2 c, float r)
            => (p - c).magnitude - r;

        private static float SdfBox(Vector2 p, Vector2 c, Vector2 h)
        {
            float dx = Mathf.Abs(p.x - c.x) - h.x;
            float dy = Mathf.Abs(p.y - c.y) - h.y;
            var d = new Vector2(Mathf.Max(dx, 0f), Mathf.Max(dy, 0f));
            return d.magnitude + Mathf.Min(Mathf.Max(dx, dy), 0f);
        }

        private static float SdfRoundedBox(Vector2 p, Vector2 c, Vector2 h, float r)
        {
            float dx = Mathf.Abs(p.x - c.x) - h.x + r;
            float dy = Mathf.Abs(p.y - c.y) - h.y + r;
            float outside = new Vector2(Mathf.Max(dx, 0f), Mathf.Max(dy, 0f)).magnitude;
            float inside = Mathf.Min(Mathf.Max(dx, dy), 0f);
            return outside + inside - r;
        }

        private static float SdfDiamond(Vector2 p, Vector2 c, float r)
        {
            float dx = Mathf.Abs(p.x - c.x);
            float dy = Mathf.Abs(p.y - c.y);
            // L1 distance normalized to approximate Euclidean distance
            return (dx + dy) * 0.7071f - r;
        }

        private static float SdfHexagon(Vector2 p, Vector2 c, float r)
        {
            float dx = Mathf.Abs(p.x - c.x);
            float dy = Mathf.Abs(p.y - c.y);
            // Pointy-top hexagon
            return Mathf.Max(dx * 0.866025f + dy * 0.5f, dy) - r;
        }
    }
}
