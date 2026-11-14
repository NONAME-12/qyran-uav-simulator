using System;
using System.IO;
using NUnit.Framework;
using Qyran.Input;

namespace Qyran.Tests
{
    public sealed class ProfileAndCalibrationTests
    {
        string directory;

        public static ControllerProfile Profile() => new ControllerProfile
        {
            device = "Test RC",
            layout = "Joystick",
            roll = new ChannelBinding { path = "roll" },
            pitch = new ChannelBinding { path = "pitch" },
            yaw = new ChannelBinding { path = "yaw" },
            throttle = new ChannelBinding { path = "throttle" }
        };

        [SetUp]
        public void SetUp() => directory = Path.Combine(Path.GetTempPath(), "qyran-tests-" + Guid.NewGuid().ToString("N"));

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }

        [Test]
        public void SavesAndRestoresControllerSettings()
        {
            var store = new ProfileStore(directory);
            var original = Profile();
            original.roll.reverse = true;
            original.expo = 0.6f;
            original.throttle.min = -0.8f;
            original.calibrated = true;
            store.Save("test-device", original);
            Assert.That(store.TryLoad("test-device", out var restored, out _), Is.True);
            Assert.That(restored.roll.reverse, Is.True);
            Assert.That(restored.expo, Is.EqualTo(0.6f));
            Assert.That(restored.throttle.min, Is.EqualTo(-0.8f));
            Assert.That(restored.calibrated, Is.True);
            Assert.That(store.TryLoad("other-device", out _, out _), Is.False);
        }

        [Test]
        public void RejectsDuplicateAxisMapping()
        {
            var profile = Profile();
            profile.yaw.path = profile.roll.path;
            Assert.That(profile.Validate(out _), Is.False);
        }

        [Test]
        public void CorruptSavedProfileDoesNotLoad()
        {
            var store = new ProfileStore(directory);
            Directory.CreateDirectory(directory);
            File.WriteAllText(store.PathFor("test-device"), "{ corrupt");
            Assert.That(store.TryLoad("test-device", out var result, out string error), Is.False);
            Assert.That(result, Is.Null);
            Assert.That(error, Is.Not.Empty);
        }

        [Test]
        public void DeviceNamesCannotEscapeProfileDirectory()
        {
            var store = new ProfileStore(directory);
            Assert.That(Path.GetDirectoryName(store.PathFor("../../outside")), Is.EqualTo(directory));
        }

        [Test]
        public void CalibrationCapturesAsymmetricEndpoints()
        {
            var profile = Profile();
            var calibration = new CalibrationSession(profile, new[] { 0.1f, -0.1f, 0f, -0.9f });
            calibration.Sample(new[] { -0.8f, -0.9f, -0.95f, -0.9f });
            calibration.Sample(new[] { 0.9f, 0.8f, 0.9f, 0.95f });
            Assert.That(calibration.TryFinish(out var calibrated, out _), Is.True);
            Assert.That(calibrated.roll.center, Is.EqualTo(0.1f));
            Assert.That(calibrated.roll.min, Is.EqualTo(-0.8f));
            Assert.That(calibrated.throttle.max, Is.EqualTo(0.95f));
            Assert.That(calibrated.calibrated, Is.True);
            Assert.That(profile.roll.min, Is.EqualTo(-1));
        }

        [Test]
        public void StationarySticksCannotCompleteCalibration()
        {
            var calibration = new CalibrationSession(Profile(), new[] { 0f, 0f, 0f, -1f });
            calibration.Sample(new[] { 0f, 0f, 0f, -1f });
            calibration.Sample(new[] { 0f, 0f, 0f, -1f });
            Assert.That(calibration.TryFinish(out _, out _), Is.False);
        }

        [Test]
        public void MissingOneSideCannotCompleteCalibration()
        {
            var calibration = new CalibrationSession(Profile(), new[] { 0f, 0f, 0f, -1f });
            calibration.Sample(new[] { 0.5f, -1f, -1f, -1f });
            calibration.Sample(new[] { 1f, 1f, 1f, 1f });
            Assert.That(calibration.TryFinish(out _, out _), Is.False);
        }
    }
}
