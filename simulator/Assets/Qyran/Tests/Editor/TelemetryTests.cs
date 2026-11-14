using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Qyran.Input;
using Qyran.Telemetry;
using UnityEngine;

namespace Qyran.Tests
{
    public sealed class TelemetryTests
    {
        [Test]
        public void CsvUsesInvariantNumbersAndFlushesOnStop()
        {
            string directory = Path.Combine(Path.GetTempPath(), "qyran-csv-" + Guid.NewGuid().ToString("N"));
            var previous = CultureInfo.CurrentCulture;
            var drone = new GameObject("Telemetry test");
            var body = drone.AddComponent<Rigidbody>();
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                using var recorder = new CsvRecorder();
                recorder.Start(directory, 10);
                recorder.Write(10.02, "keyboard", true, new FlightCommand(0.25f, 0, 0, 0.5f), body, 9.81f, 0);
                recorder.Stop();
                string[] lines = File.ReadAllLines(recorder.FilePath);
                Assert.That(lines.Length, Is.EqualTo(2));
                Assert.That(lines[1].Split(',').Length, Is.EqualTo(lines[0].Split(',').Length));
                Assert.That(lines[1], Does.Contain("0.2500").And.Contain("0.5000"));
                Assert.That(recorder.Samples, Is.EqualTo(1));
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
                UnityEngine.Object.DestroyImmediate(drone);
                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }
    }
}
