using System.IO;
using UnityEditor;
using UnityEngine;

namespace Qyran.Editor
{
    public static class SceneGeometry
    {
        const string MaterialDirectory = "Assets/Qyran/Art/Materials";
        public static Material Material(string name, Color color, float metallic = 0, float smoothness = 0.25f)
        {
            Directory.CreateDirectory(MaterialDirectory);
            string path = MaterialDirectory + "/" + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }
            material.color = color;
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", smoothness);
            EditorUtility.SetDirty(material);
            return material;
        }

        public static GameObject Shape(PrimitiveType type, string name, Vector3 position, Vector3 scale,
            Material material, Transform parent = null, bool collider = false)
        {
            var obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localScale = scale;
            obj.GetComponent<Renderer>().sharedMaterial = material;
            if (!collider) Object.DestroyImmediate(obj.GetComponent<Collider>());
            return obj;
        }

        public static GameObject Box(string name, Vector3 position, Vector3 scale, Material material,
            Transform parent = null, bool collider = false) =>
            Shape(PrimitiveType.Cube, name, position, scale, material, parent, collider);

        public static void Beam(string name, Vector3 start, Vector3 end, float thickness, Material material, Transform parent)
        {
            var beam = Box(name, (start + end) * 0.5f, new Vector3(thickness, thickness, (end - start).magnitude), material, parent);
            beam.transform.localRotation = Quaternion.LookRotation(end - start);
        }

        public static void Ring(string name, Vector3 position, float radius, float width, Material material, Transform parent)
        {
            const int segments = 80;
            for (int i = 0; i < segments; i++)
            {
                float a = i * Mathf.PI * 2 / segments;
                float b = (i + 1) * Mathf.PI * 2 / segments;
                Vector3 start = position + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * radius;
                Vector3 end = position + new Vector3(Mathf.Cos(b), 0, Mathf.Sin(b)) * radius;
                var line = Box(name, (start + end) * 0.5f, new Vector3(width, 0.008f, (end - start).magnitude + 0.015f), material, parent);
                line.transform.localRotation = Quaternion.LookRotation(end - start);
            }
        }

        public static void Label(string name, string text, Vector3 position, Quaternion rotation,
            float size, Color color, Transform parent)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            obj.transform.localRotation = rotation;
            var label = obj.AddComponent<TextMesh>();
            label.text = text;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.fontSize = 80;
            label.characterSize = size * 10 / 80;
            label.color = color;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            obj.GetComponent<MeshRenderer>().sharedMaterial = label.font.material;
        }
    }
}
