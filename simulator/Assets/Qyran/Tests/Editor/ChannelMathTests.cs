using System;
using NUnit.Framework;
using Qyran.Input;

namespace Qyran.Tests
{
    public sealed class ChannelMathTests
    {
        [TestCase(-1, -1)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(0.5f, 0.5f)]
        [TestCase(-0.5f, -0.5f)]
        [TestCase(4, 1)]
        [TestCase(-4, -1)]
        public void NormalizesBipolarRange(float raw, float expected)
        {
            Assert.That(ChannelMath.Normalize(raw, new ChannelBinding(), false, 0, 0), Is.EqualTo(expected).Within(0.0001f));
        }

        [Test]
        public void NormalizesAsymmetricCalibration()
        {
            var binding = new ChannelBinding { min = -0.8f, center = 0.1f, max = 0.9f };
            Assert.That(ChannelMath.Normalize(-0.35f, binding, false, 0, 0), Is.EqualTo(-0.5f).Within(0.0001f));
            Assert.That(ChannelMath.Normalize(0.5f, binding, false, 0, 0), Is.EqualTo(0.5f).Within(0.0001f));
        }

        [Test]
        public void ReversesAxis()
        {
            Assert.That(ChannelMath.Normalize(0.4f, new ChannelBinding { reverse = true }, false, 0, 0), Is.EqualTo(-0.4f));
        }

        [TestCase(-1, false, 0)]
        [TestCase(1, false, 1)]
        [TestCase(-1, true, 1)]
        [TestCase(1, true, 0)]
        public void ThrottleEndpointsAndReverse(float raw, bool reverse, float expected)
        {
            Assert.That(ChannelMath.Normalize(raw, new ChannelBinding { reverse = reverse }, true, 0.1f, 0.8f), Is.EqualTo(expected));
        }

        [Test]
        public void DeadzoneHasContinuousRescaledOutput()
        {
            Assert.That(ChannelMath.Curve(0.04f, 0.05f, 0), Is.Zero);
            Assert.That(ChannelMath.Curve(0.05f, 0.05f, 0), Is.Zero);
            Assert.That(ChannelMath.Curve(0.525f, 0.05f, 0), Is.EqualTo(0.5f).Within(0.0001f));
        }

        [Test]
        public void ExpoIsCubicAndSymmetric()
        {
            Assert.That(ChannelMath.Curve(0.5f, 0, 1), Is.EqualTo(0.125f));
            Assert.That(ChannelMath.Curve(-0.5f, 0, 1), Is.EqualTo(-0.125f));
        }

        [Test]
        public void RejectsInvalidAndNonFiniteSignals()
        {
            Assert.Throws<ArgumentException>(() => ChannelMath.Normalize(float.NaN, new ChannelBinding(), false, 0, 0));
            Assert.Throws<ArgumentException>(() => ChannelMath.Normalize(0, new ChannelBinding { min = 1, max = -1 }, false, 0, 0));
            Assert.Throws<ArgumentException>(() => ChannelMath.Curve(0, float.PositiveInfinity, 0));
            Assert.Throws<ArgumentException>(() => new FlightCommand(0, 0, 0, float.NaN));
        }

        [Test]
        public void CurveRemainsMonotonicAcrossStickTravel()
        {
            float previous = -1;
            for (int i = -100; i <= 100; i++)
            {
                float current = ChannelMath.Curve(i / 100f, 0.06f, 0.65f);
                Assert.That(current, Is.GreaterThanOrEqualTo(previous));
                previous = current;
            }
        }
    }
}
