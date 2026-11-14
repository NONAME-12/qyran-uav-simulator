using System.IO;
using UnityEditor;
using UnityEngine;
using static Qyran.Editor.SceneGeometry;

namespace Qyran.Editor
{
    public static class TrainingYardBuilder
    {
        public static Transform Build()
        {
            var yard = new GameObject("Training yard").transform;
            var concrete = Material("Concrete", new Color(0.57f, 0.59f, 0.54f), 0, 0.16f);
            var asphalt = Material("Asphalt", new Color(0.19f, 0.21f, 0.19f), 0, 0.18f);
            var earth = Material("Grassland", new Color(0.39f, 0.41f, 0.29f), 0, 0.1f);
            var white = Material("Markings", new Color(0.83f, 0.82f, 0.72f), 0, 0.12f);
            var orange = Material("Safety orange", new Color(0.63f, 0.27f, 0.10f), 0, 0.22f);
            var metal = Material("Galvanized steel", new Color(0.31f, 0.34f, 0.32f), 0.65f, 0.3f);
            var hangar = Material("Hangar cladding", new Color(0.39f, 0.45f, 0.40f), 0.25f, 0.22f);
            var dark = Material("Rubber", new Color(0.08f, 0.095f, 0.08f), 0, 0.15f);
            AddSurfaceTexture(concrete);
            AddSurfaceTexture(earth);
            AddSurfaceTexture(asphalt);

            Box("Ground", new Vector3(0, -0.35f, 30), new Vector3(700, 0.5f, 700), earth, yard, true);
            Box("Apron", new Vector3(0, -0.07f, 21), new Vector3(72, 0.14f, 86), concrete, yard, true);
            Box("Training lane", new Vector3(0, 0.005f, 25), new Vector3(12, 0.012f, 70), asphalt, yard);
            for (int i = 0; i < 11; i++)
                Box("Lane centre marking", new Vector3(0, 0.018f, 8 + i * 5), new Vector3(0.1f, 0.008f, 2), white, yard);
            for (int side = -1; side <= 1; side += 2)
            {
                Box("Lane edge", new Vector3(side * 5.5f, 0.02f, 25), new Vector3(0.07f, 0.008f, 69), white, yard);
                for (int i = 0; i < 8; i++)
                    Box("Apron joint", new Vector3(side * 20, 0.009f, -15 + i * 10), new Vector3(28, 0.004f, 0.025f), asphalt, yard);
            }
            Ring("Launch circle", new Vector3(0, 0.03f, 0), 2.5f, 0.12f, white, yard);
            Ring("Launch outer circle", new Vector3(0, 0.028f, 0), 2.9f, 0.04f, orange, yard);
            Box("Pad H left", new Vector3(-0.6f, 0.035f, 0), new Vector3(0.18f, 0.008f, 1.5f), white, yard);
            Box("Pad H right", new Vector3(0.6f, 0.035f, 0), new Vector3(0.18f, 0.008f, 1.5f), white, yard);
            Box("Pad H crossbar", new Vector3(0, 0.035f, 0), new Vector3(1.2f, 0.008f, 0.18f), white, yard);

            for (int i = 0; i < 3; i++) Gate(new Vector3(0, 0, 14 + i * 15), i + 1, orange, white, metal, yard);
            Hangar(new Vector3(-23, 0, 22), hangar, metal, dark, white, yard);
            for (int i = 0; i < 7; i++)
            {
                float x = 9 + i * 3.5f;
                Shape(PrimitiveType.Cylinder, "Safety bollard", new Vector3(x, 0.4f, -7),
                    new Vector3(0.18f, 0.8f, 0.18f), orange, yard, true);
                Box("Bollard foot", new Vector3(x, 0.025f, -7), new Vector3(0.5f, 0.05f, 0.5f), dark, yard);
            }
            for (int i = 0; i < 24; i++)
            {
                float z = -20 + i * 4;
                Box("Fence post", new Vector3(37, 1, z), new Vector3(0.07f, 2, 0.07f), metal, yard);
                if (i < 23)
                {
                    Box("Fence top rail", new Vector3(37, 1.9f, z + 2), new Vector3(0.04f, 0.04f, 4), metal, yard);
                    Box("Fence mid rail", new Vector3(37, 0.9f, z + 2), new Vector3(0.025f, 0.025f, 4), metal, yard);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                float z = -6 + i * 21;
                Box("Light mast", new Vector3(-35, 4.8f, z), new Vector3(0.12f, 9.6f, 0.12f), metal, yard);
                Box("Light head", new Vector3(-34.7f, 9.6f, z), new Vector3(0.8f, 0.12f, 0.35f), dark, yard);
            }

            Label("Ground designation", "Q Y R A N", new Vector3(13, 0.035f, 4),
                Quaternion.Euler(90, 0, 0), 2.5f, new Color(0.78f, 0.77f, 0.67f), yard);
            Mountains(yard);
            var marker = new GameObject("Hover target at 2 metres").transform;
            marker.SetParent(yard);
            Ring("Hover reference", new Vector3(0, 2.22f, 0), 2.5f, 0.025f, orange, marker);
            marker.gameObject.SetActive(false);
            return marker;
        }

        static void Gate(Vector3 position, int number, Material orange, Material white, Material metal, Transform parent)
        {
            var gate = new GameObject("Training gate " + number).transform;
            gate.SetParent(parent, false);
            gate.localPosition = position;
            for (int side = -1; side <= 1; side += 2)
            {
                Box("Gate upright", new Vector3(side * 2.2f, 2.1f, 0), new Vector3(0.14f, 4.2f, 0.14f), orange, gate, true);
                Box("Gate base", new Vector3(side * 2.2f, 0.08f, 0), new Vector3(0.8f, 0.16f, 1.1f), metal, gate, true);
                for (int i = 0; i < 4; i++)
                    Box("White band", new Vector3(side * 2.2f, 0.8f + i * 0.9f, -0.005f),
                        new Vector3(0.15f, 0.25f, 0.15f), white, gate);
            }
            Box("Gate crossbar", new Vector3(0, 4.13f, 0), new Vector3(4.55f, 0.14f, 0.14f), orange, gate, true);
            Box("Gate marker board", new Vector3(0, 4.55f, 0), new Vector3(0.75f, 0.55f, 0.08f), metal, gate);
            Label("Gate number", number.ToString("00"), new Vector3(0, 4.55f, -0.05f),
                Quaternion.identity, 0.5f, new Color(0.9f, 0.89f, 0.8f), gate);
        }

        static void Hangar(Vector3 position, Material cladding, Material metal, Material dark, Material white, Transform parent)
        {
            var building = new GameObject("Maintenance hangar").transform;
            building.SetParent(parent, false);
            building.localPosition = position;
            Box("Rear wall", new Vector3(0, 3.5f, 6), new Vector3(19, 7, 0.2f), cladding, building, true);
            Box("Side wall west", new Vector3(-9.5f, 3.5f, 0), new Vector3(0.2f, 7, 12), cladding, building, true);
            Box("Side wall east", new Vector3(9.5f, 3.5f, 0), new Vector3(0.2f, 7, 12), cladding, building, true);
            Box("Roof", new Vector3(0, 7.05f, 0), new Vector3(19.8f, 0.22f, 13), metal, building, true);
            Box("Front fascia", new Vector3(0, 6.3f, -6), new Vector3(19, 1.4f, 0.22f), cladding, building, true);
            for (int i = 0; i < 6; i++)
            {
                float x = -9.5f + i * 3.8f;
                Box("Steel column", new Vector3(x, 2.8f, -5.95f), new Vector3(0.16f, 5.6f, 0.16f), metal, building, true);
                Box("Roof rib", new Vector3(x, 7.22f, 0), new Vector3(0.08f, 0.08f, 12.5f), metal, building);
            }
            for (int i = 0; i < 35; i++)
                Box("Cladding seam", new Vector3(-9.3f + i * 0.54f, 3.5f, 5.88f),
                    new Vector3(0.025f, 6.8f, 0.04f), metal, building);
            Box("Workbench", new Vector3(0, 0.85f, 4.4f), new Vector3(6, 0.18f, 1.1f), white, building, true);
            Box("Tool cabinet", new Vector3(-6, 0.9f, 4.8f), new Vector3(2.4f, 1.8f, 0.9f), dark, building, true);
            Label("Hangar sign", "QYRAN  /  FLIGHT TRAINING", new Vector3(0, 6.3f, -6.13f),
                Quaternion.identity, 0.66f, new Color(0.85f, 0.85f, 0.75f), building);
        }

        static void AddSurfaceTexture(Material material)
        {
            const string path = "Assets/Qyran/Art/Surface.png";
            if (!File.Exists(path))
            {
                Directory.CreateDirectory("Assets/Qyran/Art");
                var texture = new Texture2D(128, 128, TextureFormat.RGB24, false);
                var random = new System.Random(37);
                var pixels = new Color[128 * 128];
                for (int i = 0; i < pixels.Length; i++)
                {
                    float value = 0.82f + (float)random.NextDouble() * 0.18f;
                    pixels[i] = new Color(value, value, value);
                }
                texture.SetPixels(pixels);
                texture.Apply();
                File.WriteAllBytes(path, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            material.mainTextureScale = Vector2.one * (material.name == "Grassland" ? 180 : 28);
        }

        static void Mountains(Transform parent)
        {
            var material = Material("Distant hills", new Color(0.43f, 0.48f, 0.40f), 0, 0);
            const int columns = 70;
            const int rows = 7;
            var vertices = new Vector3[(columns + 1) * (rows + 1)];
            var triangles = new int[columns * rows * 6];
            for (int row = 0; row <= rows; row++)
                for (int column = 0; column <= columns; column++)
                {
                    float x = -420 + column * 12;
                    float z = 100 + row * 28;
                    float height = (Mathf.PerlinNoise(column * 0.13f, row * 0.21f) * 35 +
                        Mathf.PerlinNoise(column * 0.3f, row * 0.4f) * 12) * Mathf.Min(1, row * 0.5f);
                    vertices[row * (columns + 1) + column] = new Vector3(x, height - 0.25f, z);
                }
            int index = 0;
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                {
                    int a = row * (columns + 1) + column;
                    triangles[index++] = a;
                    triangles[index++] = a + columns + 1;
                    triangles[index++] = a + 1;
                    triangles[index++] = a + 1;
                    triangles[index++] = a + columns + 1;
                    triangles[index++] = a + columns + 2;
                }
            var mesh = new Mesh { name = "Training yard hills", vertices = vertices, triangles = triangles };
            mesh.RecalculateNormals();
            Directory.CreateDirectory("Assets/Qyran/Art/Meshes");
            const string meshPath = "Assets/Qyran/Art/Meshes/Hills.asset";
            AssetDatabase.DeleteAsset(meshPath);
            AssetDatabase.CreateAsset(mesh, meshPath);
            var hills = new GameObject("Distant hills");
            hills.transform.SetParent(parent, false);
            hills.AddComponent<MeshFilter>().sharedMesh = mesh;
            hills.AddComponent<MeshRenderer>().sharedMaterial = material;
        }
    }
}
