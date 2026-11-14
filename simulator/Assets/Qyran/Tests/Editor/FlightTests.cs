using NUnit.Framework;
using Qyran.Flight;
using Qyran.Input;

namespace Qyran.Tests
{
    public sealed class ArmInterlockTests
    {
        [Test]
        public void NeutralConfiguredInputCanArm()
        {
            Assert.That(ArmInterlock.CanArm(FlightCommand.Zero, true, true, out _), Is.True);
        }

        [Test]
        public void HighThrottleCannotArm()
        {
            Assert.That(ArmInterlock.CanArm(new FlightCommand(0, 0, 0, 0.3f), true, true, out _), Is.False);
        }

        [Test]
        public void MissingSignalCannotArm()
        {
            Assert.That(ArmInterlock.CanArm(FlightCommand.Zero, false, true, out _), Is.False);
        }

        [Test]
        public void UncalibratedControllerCannotArm()
        {
            Assert.That(ArmInterlock.CanArm(FlightCommand.Zero, true, false, out _), Is.False);
        }

        [Test]
        public void DeflectedSticksCannotArm()
        {
            Assert.That(ArmInterlock.CanArm(new FlightCommand(0.5f, 0, 0, 0), true, true, out _), Is.False);
        }
    }

}
