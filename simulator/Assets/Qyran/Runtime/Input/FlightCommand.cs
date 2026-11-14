using System;
using UnityEngine;

namespace Qyran.Input
{
    public readonly struct FlightCommand
    {
        public static readonly FlightCommand Zero = new FlightCommand(0, 0, 0, 0);
        public readonly float Roll;
        public readonly float Pitch;
        public readonly float Yaw;
        public readonly float Throttle;

        public FlightCommand(float roll, float pitch, float yaw, float throttle)
        {
            if (!ChannelMath.IsFinite(roll) || !ChannelMath.IsFinite(pitch) ||
                !ChannelMath.IsFinite(yaw) || !ChannelMath.IsFinite(throttle))
                throw new ArgumentException("Flight commands must be finite.");

            Roll = Mathf.Clamp(roll, -1, 1);
            Pitch = Mathf.Clamp(pitch, -1, 1);
            Yaw = Mathf.Clamp(yaw, -1, 1);
            Throttle = Mathf.Clamp01(throttle);
        }

        public float Channel(int index) => index switch
        {
            0 => Roll,
            1 => Pitch,
            2 => Yaw,
            3 => Throttle,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }
}
