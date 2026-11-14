using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Qyran.Input
{
    public enum InputSource { Keyboard, UsbController }

    [DefaultExecutionOrder(-100)]
    public sealed class RcInput : MonoBehaviour
    {
        readonly float[] raw = new float[4];
        readonly List<AxisControl> axes = new List<AxisControl>();
        ProfileStore store;
        InputDevice device;
        float keyboardThrottle;
        bool focused = true;
        InputActionMap shortcuts;

        public event Action ConfigurationChanged;
        public event Action ArmRequested;
        public event Action ResetRequested;
        public event Action CameraRequested;
        public event Action RecordingRequested;
        public event Action SettingsRequested;
        public event Action CloseSettingsRequested;
        public InputSource Source { get; private set; }
        public ControllerProfile Profile { get; private set; } = new ControllerProfile();
        public FlightCommand Command { get; private set; }
        public bool HasSignal { get; private set; }
        public string Status { get; private set; } = "";
        public InputDevice Device => device;
        public IReadOnlyList<AxisControl> Axes => axes;
        public IReadOnlyList<float> Raw => raw;
        public CalibrationSession Calibration { get; private set; }
        public string SourceName => Source == InputSource.Keyboard ? "Keyboard / mouse" : device?.displayName ?? "USB controller";
        string DeviceKey => device == null ? "" : device.description.manufacturer + "|" + device.description.product + "|" + device.layout;

        void Awake()
        {
            store = new ProfileStore(Path.Combine(Application.persistentDataPath, "profiles"));
            focused = Application.isFocused;
        }

        void OnEnable()
        {
            InputSystem.onDeviceChange += DeviceChanged;
            shortcuts = new InputActionMap("Flight shortcuts");
            BindShortcut("Arm", "<Keyboard>/space", () => ArmRequested?.Invoke());
            BindShortcut("Reset", "<Keyboard>/r", () => ResetRequested?.Invoke());
            BindShortcut("Camera", "<Keyboard>/c", () => CameraRequested?.Invoke());
            BindShortcut("Record", "<Keyboard>/l", () => RecordingRequested?.Invoke());
            BindShortcut("Settings", "<Keyboard>/tab", () => SettingsRequested?.Invoke());
            BindShortcut("Close settings", "<Keyboard>/escape", () => CloseSettingsRequested?.Invoke());
            BindShortcut("Hover thrust", "<Keyboard>/h", () => SetKeyboardThrottle(0.5f));
            BindShortcut("Increase throttle", "<Keyboard>/upArrow", () => SetKeyboardThrottle(keyboardThrottle + 0.05f));
            BindShortcut("Decrease throttle", "<Keyboard>/downArrow", () => SetKeyboardThrottle(keyboardThrottle - 0.05f));
            shortcuts.Enable();
        }

        void BindShortcut(string name, string binding, Action callback)
        {
            shortcuts.AddAction(name, InputActionType.Button, binding).performed += _ =>
            {
                if (focused) callback();
            };
        }

        void OnDisable()
        {
            InputSystem.onDeviceChange -= DeviceChanged;
            shortcuts?.Dispose();
            shortcuts = null;
            ClearSignal();
        }

        public static List<InputDevice> AvailableDevices() => InputSystem.devices
            .Where(d => d.added && d.enabled && (d is Joystick || d is Gamepad)).ToList();

        public void SetSource(InputSource source)
        {
            Source = source;
            Calibration = null;
            keyboardThrottle = 0;
            ClearSignal();
            if (source == InputSource.UsbController && (device == null || !device.added))
                SelectDevice(AvailableDevices().FirstOrDefault());
            ConfigurationChanged?.Invoke();
        }

        public void SelectDevice(InputDevice selected)
        {
            device = selected;
            axes.Clear();
            Calibration = null;
            ClearSignal();
            if (device != null)
            {
                axes.AddRange(device.allControls.OfType<AxisControl>()
                    .Where(a => !(a is ButtonControl) && !a.synthetic && !(a.parent is DpadControl)));
                if (!store.TryLoad(DeviceKey, out var loaded, out string error))
                {
                    Profile = CreateDefaultProfile(device, axes);
                    Status = string.IsNullOrEmpty(error) ? "Set channel mapping and calibrate before arming." : error;
                }
                else
                {
                    Profile = loaded;
                    Status = "Controller profile loaded.";
                }
            }
            else Status = "Connect a USB controller in joystick mode.";
            ConfigurationChanged?.Invoke();
        }

        public static string RelativePath(AxisControl axis) => axis.path.Substring(axis.device.path.Length + 1);

        public static ControllerProfile CreateDefaultProfile(InputDevice target, IReadOnlyList<AxisControl> controls)
        {
            var profile = new ControllerProfile { device = target.displayName, layout = target.layout };
            if (target is Gamepad)
            {
                profile.roll.path = "rightStick/x";
                profile.pitch.path = "rightStick/y";
                profile.pitch.reverse = true;
                profile.yaw.path = "leftStick/x";
                profile.throttle.path = "leftStick/y";
            }
            else
                for (int i = 0; i < 4; i++)
                    profile.Channel(i).path = i < controls.Count ? RelativePath(controls[i]) : "";
            return profile;
        }

        void Update()
        {
            Poll(Time.unscaledDeltaTime);
        }

        public void Poll(float deltaTime)
        {
            if (!focused)
            {
                ClearSignal();
                return;
            }
            if (Source == InputSource.Keyboard)
            {
                var keyboard = Keyboard.current;
                HasSignal = keyboard != null && keyboard.enabled;
                if (!HasSignal) { ClearSignal(); return; }
                float direction = (keyboard.upArrowKey.isPressed ? 1 : 0) - (keyboard.downArrowKey.isPressed ? 1 : 0);
                keyboardThrottle = Mathf.Clamp01(keyboardThrottle + direction * Mathf.Min(deltaTime, 0.05f) * 0.4f);
                raw[0] = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
                raw[1] = (keyboard.sKey.isPressed ? 1 : 0) - (keyboard.wKey.isPressed ? 1 : 0);
                raw[2] = (keyboard.eKey.isPressed ? 1 : 0) - (keyboard.qKey.isPressed ? 1 : 0);
                raw[3] = keyboardThrottle * 2 - 1;
                Command = new FlightCommand(raw[0] * 0.55f, raw[1] * 0.55f, raw[2] * 0.5f, keyboardThrottle);
                return;
            }

            if (device == null || !device.added || !device.enabled)
            {
                Status = "No USB controller signal.";
                ClearSignal();
                return;
            }
            if (!Profile.Validate(out string validation))
            {
                Status = validation;
                ClearSignal();
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                string path = Profile.Channel(i).path;
                var axis = axes.FirstOrDefault(a => RelativePath(a) == path);
                if (axis == null)
                {
                    Status = "The selected device does not contain axis " + path + ".";
                    ClearSignal();
                    return;
                }
                raw[i] = axis.ReadUnprocessedValue();
                if (!ChannelMath.IsFinite(raw[i]))
                {
                    Status = "Invalid controller signal.";
                    ClearSignal();
                    return;
                }
            }
            Calibration?.Sample(raw);
            HasSignal = true;
            Command = Profile.Process(raw);
        }

        public float ReadAxis(AxisControl axis) => device != null && device.added ? axis.ReadUnprocessedValue() : 0;

        public bool BeginCalibration(out string error)
        {
            if (Source != InputSource.UsbController || !HasSignal)
            {
                error = "Select a connected controller and assign four different axes.";
                return false;
            }
            Calibration = new CalibrationSession(Profile, raw);
            error = "";
            ConfigurationChanged?.Invoke();
            return true;
        }

        public bool FinishCalibration(out string error)
        {
            if (Calibration == null) { error = "Capture the centre first."; return false; }
            if (!Calibration.TryFinish(out var calibrated, out error)) return false;
            Profile = calibrated;
            Calibration = null;
            ConfigurationChanged?.Invoke();
            return true;
        }

        public void CancelCalibration()
        {
            Calibration = null;
            ConfigurationChanged?.Invoke();
        }

        public void InvalidateCalibration()
        {
            Calibration = null;
            Profile.calibrated = false;
            ClearSignal();
            ConfigurationChanged?.Invoke();
        }

        public void SaveProfile()
        {
            if (device == null || !device.added) throw new InvalidOperationException("No controller selected.");
            store.Save(DeviceKey, Profile);
        }

        public void ResetThrottle()
        {
            keyboardThrottle = 0;
            Command = FlightCommand.Zero;
        }

        public void SetKeyboardThrottle(float value)
        {
            if (Source == InputSource.Keyboard && ChannelMath.IsFinite(value))
                keyboardThrottle = Mathf.Clamp01(value);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            focused = hasFocus;
            if (!hasFocus) { ResetThrottle(); ClearSignal(); }
        }

        void DeviceChanged(InputDevice changed, InputDeviceChange change)
        {
            if (changed != device) return;
            if (change == InputDeviceChange.Disconnected || change == InputDeviceChange.Removed ||
                change == InputDeviceChange.Disabled)
            {
                Calibration = null;
                ClearSignal();
                Status = "Controller disconnected.";
                ConfigurationChanged?.Invoke();
            }
        }

        void ClearSignal()
        {
            HasSignal = false;
            Command = FlightCommand.Zero;
            Array.Clear(raw, 0, raw.Length);
        }
    }
}
