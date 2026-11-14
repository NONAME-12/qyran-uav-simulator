using System.Collections;
using NUnit.Framework;
using Qyran.Flight;
using Qyran.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Qyran.Tests
{
    public sealed class FlightResetTests : InputTestFixture
    {
        [UnityTest]
        public IEnumerator ResetTeleportsBodyAndRenderedTransform()
        {
            var keyboard = InputSystem.AddDevice<Keyboard>();
            var inputOwner = new GameObject("Reset test input");
            var input = inputOwner.AddComponent<RcInput>();
            var cameraOwner = new GameObject("Reset test camera");
            var camera = cameraOwner.AddComponent<FollowCamera>();
            var drone = new GameObject("Reset test quadrotor");
            var body = drone.AddComponent<Rigidbody>();
            body.interpolation = RigidbodyInterpolation.Interpolate;
            drone.transform.position = new Vector3(0, 1, 0);
            body.position = drone.transform.position;
            drone.AddComponent<QuadrotorMotor>();
            var session = drone.AddComponent<FlightSession>();
            session.input = input;
            session.flightCamera = camera;
            camera.target = drone.transform;
            try
            {
                yield return null;
                body.position = new Vector3(0, 40, 0);
                body.linearVelocity = new Vector3(2, 1, -3);
                yield return new WaitForFixedUpdate();
                yield return null;
                session.ResetFlight();
                yield return new WaitForFixedUpdate();
                yield return null;
                Assert.That(Vector3.Distance(body.position, new Vector3(0, 1, 0)), Is.LessThan(0.1f));
                Assert.That(Vector3.Distance(drone.transform.position, body.position), Is.LessThan(0.1f));
                Assert.That(session.Armed, Is.False);
                Assert.That(input.Command.Throttle, Is.Zero);
                Assert.That(camera.FirstPerson, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(drone);
                Object.DestroyImmediate(cameraOwner);
                Object.DestroyImmediate(inputOwner);
                InputSystem.RemoveDevice(keyboard);
            }
        }
    }
}
