using System;
using UnityEngine;

namespace Qyran.Input
{
    public static class ChannelMath
    {
        public static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);

        public static float Curve(float value, float deadzone, float expo)
        {
            if (!IsFinite(value) || !IsFinite(deadzone) || !IsFinite(expo))
                throw new ArgumentException("Channel parameters must be finite.");

            deadzone = Mathf.Clamp(deadzone, 0, 0.9f);
            expo = Mathf.Clamp01(expo);
            float magnitude = Mathf.Abs(value);
            if (magnitude <= deadzone) return 0;
            float scaled = Mathf.Clamp01((magnitude - deadzone) / (1 - deadzone));
            return Mathf.Sign(value) * ((1 - expo) * scaled + expo * scaled * scaled * scaled);
        }

        public static float Normalize(float raw, ChannelBinding binding, bool throttle, float deadzone, float expo)
        {
            if (!IsFinite(raw) || binding == null || !binding.IsValid(throttle))
                throw new ArgumentException("Invalid channel value or calibration.");

            if (throttle)
            {
                float value = Mathf.Clamp01((raw - binding.min) / (binding.max - binding.min));
                return binding.reverse ? 1 - value : value;
            }

            float axis = raw >= binding.center
                ? (raw - binding.center) / (binding.max - binding.center)
                : (raw - binding.center) / (binding.center - binding.min);
            return Curve(Mathf.Clamp(binding.reverse ? -axis : axis, -1, 1), deadzone, expo);
        }
    }
}
