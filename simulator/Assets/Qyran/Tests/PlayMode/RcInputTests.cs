using System.Runtime.InteropServices;
using NUnit.Framework;
using Qyran.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace Qyran.Tests
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct TransmitterState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('Q', 'R', 'C', 'T');
        [FieldOffset(16), InputControl(name = "roll", layout = "Axis")] public float roll;
        [FieldOffset(20), InputControl(name = "pitch", layout = "Axis")] public float pitch;
        [FieldOffset(24), InputControl(name = "yaw", layout = "Axis")] public float yaw;
        [FieldOffset(28), InputControl(name = "throttle", layout = "Axis")] public float throttle;
    }

    [InputControlLayout(stateType = typeof(TransmitterState))]
    public class TestTransmitter : Joystick { }

    public sealed class RcInputTests : InputTestFixture
    {
        TestTransmitter device;
        GameObject owner;
        RcInput input;

        public override void Setup()
        {
            base.Setup();
            InputSystem.RegisterLayout<TestTransmitter>();
            device = InputSystem.AddDevice<TestTransmitter>();
            owner = new GameObject("RC adapter test");
            input = owner.AddComponent<RcInput>();
            input.SendMessage("OnApplicationFocus", true);
            input.SetSource(InputSource.UsbController);
            input.SelectDevice(device);
            input.Profile.roll.path = "roll";
            input.Profile.pitch.path = "pitch";
            input.Profile.yaw.path = "yaw";
            input.Profile.throttle.path = "throttle";
            input.Profile.deadzone = 0;
            input.Profile.expo = 0;
        }

        public override void TearDown()
        {
            Object.DestroyImmediate(owner);
            if (device.added) InputSystem.RemoveDevice(device);
            base.TearDown();
        }

        void Send(float roll, float pitch, float yaw, float throttle)
        {
            InputSystem.QueueStateEvent(device, new TransmitterState { roll = roll, pitch = pitch, yaw = yaw, throttle = throttle });
            InputSystem.Update();
            input.Poll(0.02f);
        }

        [Test]
        public void DiscoversUsbStyleJoystick()
        {
            Assert.That(RcInput.AvailableDevices(), Does.Contain(device));
            Assert.That(input.Axes.Count, Is.GreaterThanOrEqualTo(4));
        }

        [Test]
        public void ProcessesEventsThroughRealInputSystem()
        {
            Send(0.3f, -0.7f, 0.4f, 0.2f);
            Assert.That(input.HasSignal, Is.True);
            Assert.That(input.Command.Roll, Is.EqualTo(0.3f).Within(0.001f));
            Assert.That(input.Command.Pitch, Is.EqualTo(-0.7f).Within(0.001f));
            Assert.That(input.Command.Yaw, Is.EqualTo(0.4f).Within(0.001f));
            Assert.That(input.Command.Throttle, Is.EqualTo(0.6f).Within(0.001f));
        }

        [Test]
        public void RemovalImmediatelyClearsInput()
        {
            Send(0.5f, 0.5f, 0, 1);
            InputSystem.RemoveDevice(device);
            Assert.That(input.HasSignal, Is.False);
            Assert.That(input.Command.Throttle, Is.Zero);
            Assert.That(input.Command.Roll, Is.Zero);
        }

        [Test]
        public void FocusLossClearsInputAndThrottle()
        {
            Send(0, 0, 0, 1);
            input.SendMessage("OnApplicationFocus", false);
            Assert.That(input.HasSignal, Is.False);
            Assert.That(input.Command.Throttle, Is.Zero);
        }

        [Test]
        public void DuplicateMappingCannotGenerateCommands()
        {
            input.Profile.yaw.path = "roll";
            Send(0.5f, 0, 0, 1);
            Assert.That(input.HasSignal, Is.False);
            Assert.That(input.Command.Throttle, Is.Zero);
        }

        [Test]
        public void MissingAxisCannotGenerateCommands()
        {
            input.Profile.roll.path = "not-an-axis";
            Send(0, 0, 0, 1);
            Assert.That(input.HasSignal, Is.False);
        }

        [Test]
        public void DeviceEventsFeedCalibration()
        {
            Send(0, 0, 0, -1);
            Assert.That(input.BeginCalibration(out _), Is.True);
            Send(-1, -1, -1, -1);
            Send(1, 1, 1, 1);
            Assert.That(input.FinishCalibration(out _), Is.True);
            Assert.That(input.Profile.calibrated, Is.True);
        }

        [Test]
        public void MouseThrottleFeedsTheFlightCommand()
        {
            var keyboard = InputSystem.AddDevice<Keyboard>();
            try
            {
                input.SetSource(InputSource.Keyboard);
                input.SetKeyboardThrottle(0.6f);
                input.Poll(0.02f);
                Assert.That(input.Command.Throttle, Is.EqualTo(0.6f).Within(0.001f));
                input.SetKeyboardThrottle(float.NaN);
                input.Poll(0.02f);
                Assert.That(input.Command.Throttle, Is.EqualTo(0.6f).Within(0.001f));
            }
            finally { InputSystem.RemoveDevice(keyboard); }
        }

    }
}
