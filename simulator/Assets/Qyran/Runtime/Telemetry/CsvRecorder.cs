using System;
using System.Globalization;
using System.IO;
using System.Text;
using Qyran.Input;
using UnityEngine;

namespace Qyran.Telemetry
{
    public sealed class CsvRecorder : IDisposable
    {
        static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        StreamWriter writer;
        double startedAt;
        public string FilePath { get; private set; } = "";
        public int Samples { get; private set; }
        public bool IsRecording => writer != null;

        public void Start(string directory, double now)
        {
            Stop();
            Directory.CreateDirectory(directory);
            FilePath = Path.Combine(directory, "flight-" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".csv");
            writer = new StreamWriter(FilePath, false, new UTF8Encoding(false));
            writer.WriteLine("time_s,source,armed,roll,pitch,yaw,throttle,x_m,y_m,z_m,vx_m_s,vy_m_s,vz_m_s,pitch_deg,yaw_deg,roll_deg,thrust_n,collisions");
            startedAt = now;
            Samples = 0;
        }

        public void Write(double now, string source, bool armed, FlightCommand input, Rigidbody body, float thrust, int collisions)
        {
            if (writer == null) return;
            Vector3 p = body.position;
            Vector3 v = body.linearVelocity;
            Vector3 a = body.rotation.eulerAngles;
            writer.WriteLine(string.Join(",",
                (now - startedAt).ToString("F3", Culture), source, armed ? "1" : "0",
                F(input.Roll), F(input.Pitch), F(input.Yaw), F(input.Throttle),
                F(p.x), F(p.y), F(p.z), F(v.x), F(v.y), F(v.z),
                F(Mathf.DeltaAngle(0, a.x)), F(a.y), F(Mathf.DeltaAngle(0, a.z)), F(thrust),
                collisions.ToString(Culture)));
            Samples++;
            if (Samples % 50 == 0) writer.Flush();
        }

        static string F(float value) => value.ToString("F4", Culture);

        public void Stop()
        {
            writer?.Dispose();
            writer = null;
        }

        public void Dispose() => Stop();
    }
}
