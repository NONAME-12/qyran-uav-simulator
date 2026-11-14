using Qyran.Flight;
using UnityEditor;
using UnityEngine;
using static Qyran.Editor.SceneGeometry;

namespace Qyran.Editor
{
    public static class QuadrotorBuilder
    {
        public static GameObject Build()
        {
            var drone = new GameObject("Qyran quadrotor");
            var body = drone.AddComponent<Rigidbody>();
            body.mass = 1;
            body.linearDamping = 0.22f;
            body.angularDamping = 0.5f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.maxAngularVelocity = 12;
            var collider = drone.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.62f, 0.22f, 0.62f);
            collider.center = new Vector3(0, -0.03f, 0);
            drone.AddComponent<QuadrotorMotor>();
            var session = drone.AddComponent<FlightSession>();
            var carbon = Material("Carbon frame", new Color(0.065f, 0.075f, 0.07f), 0.35f, 0.5f);
            var shell = Material("Ivory shell", new Color(0.72f, 0.73f, 0.65f), 0.15f, 0.45f);
            var orange = Material("Rotor orange", new Color(0.8f, 0.29f, 0.09f), 0.12f, 0.35f);
            var black = Material("Camera glass", new Color(0.015f, 0.02f, 0.023f), 0.45f, 0.9f);
            var metal = Material("Motor housing", new Color(0.3f, 0.32f, 0.3f), 0.8f, 0.65f);
            Transform frame = drone.transform;
            Box("Lower carbon plate", new Vector3(0, -0.03f, 0), new Vector3(0.22f, 0.025f, 0.36f), carbon, frame);
            Box("Upper shell", new Vector3(0, 0.02f, -0.01f), new Vector3(0.19f, 0.07f, 0.28f), shell, frame);
            Box("Battery", new Vector3(0, 0.085f, -0.035f), new Vector3(0.11f, 0.06f, 0.17f), carbon, frame);
            Box("Battery strap", new Vector3(0, 0.12f, -0.035f), new Vector3(0.125f, 0.008f, 0.035f), orange, frame);
            var camera = Shape(PrimitiveType.Cylinder, "FPV lens", new Vector3(0, 0.035f, 0.16f),
                new Vector3(0.06f, 0.025f, 0.06f), black, frame);
            camera.transform.localRotation = Quaternion.Euler(90, 0, 0);
            Beam("Antenna", new Vector3(0, 0.045f, -0.12f), new Vector3(0, 0.22f, -0.17f), 0.009f, carbon, frame);
            session.rotors = new Transform[4];
            for (int i = 0; i < 4; i++)
            {
                float x = i % 2 == 0 ? -0.24f : 0.24f;
                float z = i < 2 ? 0.24f : -0.24f;
                Vector3 position = new Vector3(x, -0.02f, z);
                Beam("Carbon arm", new Vector3(x * 0.18f, -0.02f, z * 0.18f), position, 0.035f, carbon, frame);
                Shape(PrimitiveType.Cylinder, "Motor " + (i + 1), position + Vector3.up * 0.025f,
                    new Vector3(0.065f, 0.06f, 0.065f), metal, frame);
                var rotor = new GameObject("Propeller " + (i + 1)).transform;
                rotor.SetParent(frame, false);
                rotor.localPosition = position + Vector3.up * 0.062f;
                var bladeMaterial = i < 2 ? orange : shell;
                Box("Propeller blade", Vector3.zero, new Vector3(0.23f, 0.005f, 0.025f), bladeMaterial, rotor);
                Shape(PrimitiveType.Cylinder, "Propeller hub", Vector3.zero,
                    new Vector3(0.024f, 0.012f, 0.024f), carbon, rotor);
                rotor.localRotation = Quaternion.Euler(0, 25 + i * 37, 0);
                session.rotors[i] = rotor;
                Box("Landing foot", position - Vector3.up * 0.085f, new Vector3(0.045f, 0.1f, 0.045f), carbon, frame);
            }
            return drone;
        }
    }
}
