using System;
using System.IO;
using Qyran.Input;
using Qyran.Telemetry;
using UnityEngine;

namespace Qyran.Flight
{
    [RequireComponent(typeof(QuadrotorMotor))]
    public sealed class FlightSession : MonoBehaviour
    {
        public RcInput input;
        public FollowCamera flightCamera;
        public Transform[] rotors;
        public Transform hoverMarker;

        Vector3 spawnPosition;
        Quaternion spawnRotation;
        float hoverTime;
        float flightTime;
        float lastCollisionAt = -10;
        double messageUntil;
        readonly CsvRecorder recorder = new CsvRecorder();

        public QuadrotorMotor Motor { get; private set; }
        public bool Armed { get; private set; }
        public bool SettingsOpen { get; set; }
        public int Collisions { get; private set; }
        public float Altitude => Mathf.Max(0, transform.position.y - spawnPosition.y);
        public float Speed => Motor.Body.linearVelocity.magnitude;
        public float FlightTime => flightTime;
        public float HoverProgress => Mathf.Clamp01(hoverTime / 5);
        public int ExerciseStage { get; private set; }
        public string Exercise => ExerciseStage switch
        {
            0 => "Take off and hold 2 m for 5 seconds",
            1 => "Land on the launch pad",
            _ => "Exercise complete"
        };
        public string Message => Time.unscaledTimeAsDouble < messageUntil ? message : "";
        string message = "";
        public CsvRecorder Recorder => recorder;

        void Awake()
        {
            Motor = GetComponent<QuadrotorMotor>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
        }

        void Start()
        {
            input.ConfigurationChanged += InputChanged;
            input.ArmRequested += ToggleArm;
            input.ResetRequested += ResetFlight;
            input.CameraRequested += flightCamera.Toggle;
            input.RecordingRequested += ToggleRecording;
            Physics.gravity = new Vector3(0, -9.81f, 0);
        }

        void OnDestroy()
        {
            if (input != null)
            {
                input.ConfigurationChanged -= InputChanged;
                input.ArmRequested -= ToggleArm;
                input.ResetRequested -= ResetFlight;
                if (flightCamera != null) input.CameraRequested -= flightCamera.Toggle;
                input.RecordingRequested -= ToggleRecording;
            }
            recorder.Dispose();
        }

        void Update()
        {
            if (rotors != null)
                for (int i = 0; i < rotors.Length; i++)
                    rotors[i].Rotate(Vector3.up, (i % 2 == 0 ? 1 : -1) * (Armed ? 800 + input.Command.Throttle * 2600 : 0) * Time.deltaTime);
            if (hoverMarker != null)
                hoverMarker.gameObject.SetActive(ExerciseStage == 0 && Altitude > 0.5f);
        }

        void FixedUpdate()
        {
            if (Armed && (!input.HasSignal || SettingsOpen || input.Calibration != null))
            {
                Disarm();
                Notify("Input interrupted. Motors disarmed.");
            }
            Motor.Step(input.Command, Armed);
            if (Armed) flightTime += Time.fixedDeltaTime;

            float distance = Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                new Vector2(spawnPosition.x, spawnPosition.z));
            if (ExerciseStage == 0)
            {
                bool holding = Armed && Mathf.Abs(Altitude - 2) < 0.5f && distance < 2.5f && Speed < 1.5f;
                hoverTime = holding ? hoverTime + Time.fixedDeltaTime : 0;
                if (hoverTime >= 5) { ExerciseStage = 1; Notify("Hover complete. Land on the launch pad."); }
            }
            else if (ExerciseStage == 1 && Altitude < 0.12f && distance < 2.5f && Speed < 0.3f)
            {
                ExerciseStage = 2;
                Disarm();
                Notify("Takeoff, hover and landing complete.", 8);
            }

            try
            {
                recorder.Write(Time.timeAsDouble, input.Source == InputSource.Keyboard ? "desktop" : "usb",
                    Armed, input.Command, Motor.Body, Motor.Thrust, Collisions);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                recorder.Stop();
                Notify("Recording stopped: " + ex.Message);
            }

            if (transform.position.y < -10 || transform.position.sqrMagnitude > 250000)
            {
                ResetFlight();
                Notify("Flight area limit reached. Returned to launch pad.");
            }
        }

        public void ToggleArm()
        {
            if (Armed) { Disarm(); return; }
            bool configured = input.Source == InputSource.Keyboard || input.Profile.calibrated;
            if (SettingsOpen) { Notify("Close controller settings before arming."); return; }
            if (!ArmInterlock.CanArm(input.Command, input.HasSignal, configured, out string reason))
            {
                Notify(reason);
                return;
            }
            if (Vector3.Dot(transform.up, Vector3.up) < 0.7f)
            {
                Notify("Reset the quadrotor before arming.");
                return;
            }
            Armed = true;
            Notify("Motors armed.");
        }

        public void Disarm()
        {
            Armed = false;
            input.ResetThrottle();
        }

        public void ResetFlight()
        {
            Disarm();
            var interpolation = Motor.Body.interpolation;
            Motor.Body.interpolation = RigidbodyInterpolation.None;
            Motor.Body.position = spawnPosition;
            Motor.Body.rotation = spawnRotation;
            Motor.Body.linearVelocity = Vector3.zero;
            Motor.Body.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            Physics.SyncTransforms();
            Motor.Body.interpolation = interpolation;
            Motor.Body.WakeUp();
            Collisions = 0;
            flightTime = 0;
            hoverTime = 0;
            ExerciseStage = 0;
            flightCamera.Snap();
        }

        public void ToggleRecording()
        {
            if (recorder.IsRecording)
            {
                recorder.Stop();
                Notify($"Saved {recorder.Samples} telemetry samples.");
                return;
            }
            try
            {
                recorder.Start(Path.Combine(Application.persistentDataPath, "telemetry"), Time.timeAsDouble);
                Notify("Recording telemetry.");
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Notify("Could not record telemetry: " + ex.Message);
            }
        }

        public void Notify(string text, float duration = 4)
        {
            message = text;
            messageUntil = Time.unscaledTimeAsDouble + duration;
        }

        void InputChanged()
        {
            if (Armed) Disarm();
        }

        void OnApplicationFocus(bool focused)
        {
            if (!focused && input != null) Disarm();
        }

        void OnApplicationPause(bool paused)
        {
            if (paused && input != null) Disarm();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (Armed && collision.relativeVelocity.magnitude > 1.8f && Time.time - lastCollisionAt > 0.5f)
            {
                Collisions++;
                lastCollisionAt = Time.time;
                Notify("Impact detected.");
            }
        }
    }
}
