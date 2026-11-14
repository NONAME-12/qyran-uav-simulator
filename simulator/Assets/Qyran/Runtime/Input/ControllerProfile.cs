using System;
using System.Collections.Generic;
using UnityEngine;

namespace Qyran.Input
{
    [Serializable]
    public sealed class ChannelBinding
    {
        public string path = "";
        public float min = -1;
        public float center;
        public float max = 1;
        public bool reverse;

        public bool IsValid(bool throttle)
        {
            return ChannelMath.IsFinite(min) && ChannelMath.IsFinite(center) &&
                   ChannelMath.IsFinite(max) && max - min >= 0.1f &&
                   (throttle || (min < center && center < max));
        }
    }

    [Serializable]
    public sealed class ControllerProfile
    {
        public int version = 1;
        public string device = "";
        public string layout = "";
        public bool calibrated;
        public float deadzone = 0.04f;
        public float expo = 0.25f;
        public ChannelBinding roll = new ChannelBinding();
        public ChannelBinding pitch = new ChannelBinding();
        public ChannelBinding yaw = new ChannelBinding();
        public ChannelBinding throttle = new ChannelBinding();

        public ChannelBinding Channel(int index) => index switch
        {
            0 => roll,
            1 => pitch,
            2 => yaw,
            3 => throttle,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public bool Validate(out string error)
        {
            if (version != 1)
            {
                error = "Unsupported controller profile version.";
                return false;
            }
            if (!ChannelMath.IsFinite(deadzone) || deadzone < 0 || deadzone > 0.3f ||
                !ChannelMath.IsFinite(expo) || expo < 0 || expo > 1)
            {
                error = "Deadzone or expo is outside the supported range.";
                return false;
            }

            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < 4; i++)
            {
                var binding = Channel(i);
                if (binding == null || string.IsNullOrWhiteSpace(binding.path) || !binding.IsValid(i == 3))
                {
                    error = $"Check {new[] { "roll", "pitch", "yaw", "throttle" }[i]} mapping and calibration.";
                    return false;
                }
                if (!paths.Add(binding.path))
                {
                    error = "Each channel needs a different axis.";
                    return false;
                }
            }
            error = "";
            return true;
        }

        public FlightCommand Process(IReadOnlyList<float> raw)
        {
            if (raw == null || raw.Count < 4) throw new ArgumentException("Four channel values are required.");
            return new FlightCommand(
                ChannelMath.Normalize(raw[0], roll, false, deadzone, expo),
                ChannelMath.Normalize(raw[1], pitch, false, deadzone, expo),
                ChannelMath.Normalize(raw[2], yaw, false, deadzone, expo),
                ChannelMath.Normalize(raw[3], throttle, true, deadzone, expo));
        }

        public ControllerProfile Copy() => JsonUtility.FromJson<ControllerProfile>(JsonUtility.ToJson(this));
    }
}
