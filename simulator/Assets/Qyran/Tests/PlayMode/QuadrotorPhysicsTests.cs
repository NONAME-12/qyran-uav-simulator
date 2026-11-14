using System.Collections;
using NUnit.Framework;
using Qyran.Flight;
using Qyran.Input;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Qyran.Tests
{
    public sealed class QuadrotorPhysicsTests
    {
        Scene scene;
        PhysicsScene physics;
        QuadrotorMotor motor;
        Rigidbody body;

        [SetUp]
        public void SetUp()
        {
            scene = SceneManager.CreateScene("Qyran isolated physics test", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            physics = scene.GetPhysicsScene();
            var drone = new GameObject("Test quadrotor");
            SceneManager.MoveGameObjectToScene(drone, scene);
            body = drone.AddComponent<Rigidbody>();
            body.mass = 1;
            body.linearDamping = 0;
            body.angularDamping = 0;
            body.position = new Vector3(0, 10, 0);
            motor = drone.AddComponent<QuadrotorMotor>();
            Physics.SyncTransforms();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return SceneManager.UnloadSceneAsync(scene);
        }

        void Run(FlightCommand command, bool armed, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                motor.Step(command, armed);
                physics.Simulate(0.02f);
            }
        }

        [Test]
        public void HoverThrustBalancesGravity()
        {
            Run(new FlightCommand(0, 0, 0, motor.HoverThrottle), true, 100);
            Assert.That(body.position.y, Is.EqualTo(10).Within(0.03f));
            Assert.That(body.linearVelocity.magnitude, Is.LessThan(0.03f));
        }

        [Test]
        public void FullThrottleClimbs()
        {
            Run(new FlightCommand(0, 0, 0, 1), true, 50);
            Assert.That(body.position.y, Is.GreaterThan(14));
        }

        [Test]
        public void DisarmedMotorsCannotProduceLift()
        {
            Run(new FlightCommand(0, 0, 0, 1), false, 40);
            Assert.That(body.position.y, Is.LessThan(8));
            Assert.That(motor.Thrust, Is.Zero);
        }

        [Test]
        public void AngleControllerRecoversFromTilt()
        {
            body.rotation = Quaternion.Euler(20, 0, 20);
            Run(new FlightCommand(0, 0, 0, 0.5f), true, 200);
            Assert.That(Vector3.Angle(body.rotation * Vector3.up, Vector3.up), Is.LessThan(2));
        }

        [Test]
        public void RollProducesLateralAcceleration()
        {
            Run(new FlightCommand(0.6f, 0, 0, 0.6f), true, 50);
            Assert.That(body.position.x, Is.GreaterThan(0.3f));
        }
    }
}
