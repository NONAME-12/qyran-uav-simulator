using System;
using System.Collections.Generic;
using UnityEngine;

namespace Qyran.Input
{
    public sealed class CalibrationSession
    {
        readonly ControllerProfile candidate;
        readonly float[] minimum = new float[4];
        readonly float[] maximum = new float[4];

        public int SampleCount { get; private set; }

        public CalibrationSession(ControllerProfile profile, IReadOnlyList<float> center)
        {
            if (center == null || center.Count != 4) throw new ArgumentException("Four centre values are required.");
            candidate = profile.Copy();
            for (int i = 0; i < 4; i++)
            {
                if (!ChannelMath.IsFinite(center[i])) throw new ArgumentException("Centre must be finite.");
                candidate.Channel(i).center = center[i];
                minimum[i] = center[i];
                maximum[i] = center[i];
            }
        }

        public void Sample(IReadOnlyList<float> values)
        {
            if (values == null || values.Count != 4) return;
            for (int i = 0; i < 4; i++)
                if (!ChannelMath.IsFinite(values[i])) return;
            for (int i = 0; i < 4; i++)
            {
                minimum[i] = Mathf.Min(minimum[i], values[i]);
                maximum[i] = Mathf.Max(maximum[i], values[i]);
            }
            SampleCount++;
        }

        public bool TryFinish(out ControllerProfile result, out string error)
        {
            result = null;
            if (SampleCount < 2)
            {
                error = "Move all four channels through their full range.";
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                var channel = candidate.Channel(i);
                channel.min = minimum[i];
                channel.max = maximum[i];
                if (maximum[i] - minimum[i] < 0.2f || !channel.IsValid(i == 3))
                {
                    error = $"Move {new[] { "roll", "pitch", "yaw", "throttle" }[i]} to both endpoints.";
                    return false;
                }
            }
            candidate.calibrated = true;
            if (!candidate.Validate(out error)) return false;
            result = candidate.Copy();
            return true;
        }
    }
}
