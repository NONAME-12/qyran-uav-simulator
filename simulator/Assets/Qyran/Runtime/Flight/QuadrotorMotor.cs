using Qyran.Input;
using UnityEngine;

namespace Qyran.Flight
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class QuadrotorMotor : MonoBehaviour
    {
        [SerializeField] float maximumTilt = 28;
        [SerializeField] float maximumYawRate = 100;
        [SerializeField] float thrustToWeight = 2;
        [SerializeField] float attitudeGain = 24;
        [SerializeField] float angularDamping = 7;
        [SerializeField] float yawGain = 5;

        Rigidbody body;
        public Rigidbody Body => body != null ? body : body = GetComponent<Rigidbody>();
        public float HoverThrottle => 1 / thrustToWeight;
        public float Thrust { get; private set; }

        public void Step(FlightCommand command, bool armed)
        {
            Thrust = armed ? command.Throttle * Body.mass * Physics.gravity.magnitude * thrustToWeight : 0;
            if (!armed) return;

            Body.AddRelativeForce(Vector3.up * Thrust, ForceMode.Force);
            Quaternion target = Quaternion.Euler(command.Pitch * maximumTilt, Body.rotation.eulerAngles.y,
                -command.Roll * maximumTilt);
            Quaternion error = Quaternion.Inverse(Body.rotation) * target;
            if (error.w < 0) error = new Quaternion(-error.x, -error.y, -error.z, -error.w);
            error.ToAngleAxis(out float angle, out Vector3 axis);
            Vector3 rotationError = angle < 0.001f ? Vector3.zero : axis * (angle * Mathf.Deg2Rad);
            Vector3 angularVelocity = transform.InverseTransformDirection(Body.angularVelocity);
            Vector3 acceleration = rotationError * attitudeGain - angularVelocity * angularDamping;
            acceleration.y = (command.Yaw * maximumYawRate * Mathf.Deg2Rad - angularVelocity.y) * yawGain;
            Body.AddRelativeTorque(Vector3.ClampMagnitude(acceleration, 35), ForceMode.Acceleration);
        }
    }
}
