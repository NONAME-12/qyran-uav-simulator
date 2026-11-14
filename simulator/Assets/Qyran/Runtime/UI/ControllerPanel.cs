using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qyran.Flight;
using Qyran.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Qyran.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class ControllerPanel : MonoBehaviour
    {
        public FlightSession session;
        public RcInput input;

        VisualElement root, panel, usb, keyboardInfo, mappings, rawAxes;
        DropdownField source, devices;
        Label deviceStatus, profileStatus, instruction;
        Button centre, finish, cancel, save;
        Slider deadzone, expo;
        List<InputDevice> deviceList = new List<InputDevice>();
        readonly List<Label> rawValues = new List<Label>();
        readonly List<VisualElement> rawFills = new List<VisualElement>();
        bool open;
        float nextRefresh;

        void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            panel = root.Q("controller-panel");
            usb = root.Q("usb-settings");
            keyboardInfo = root.Q("keyboard-info");
            mappings = root.Q("mapping-rows");
            rawAxes = root.Q("raw-axes");
            source = root.Q<DropdownField>("source-field");
            devices = root.Q<DropdownField>("device-field");
            deviceStatus = root.Q<Label>("device-status");
            profileStatus = root.Q<Label>("profile-status");
            instruction = root.Q<Label>("calibration-instruction");
            centre = root.Q<Button>("centre-button");
            finish = root.Q<Button>("finish-button");
            cancel = root.Q<Button>("cancel-calibration");
            save = root.Q<Button>("save-profile");
            deadzone = root.Q<Slider>("deadzone-field");
            expo = root.Q<Slider>("expo-field");
            source.choices = new List<string> { "Keyboard / mouse", "USB controller" };
            source.SetValueWithoutNotify("Keyboard / mouse");
            source.RegisterValueChangedCallback(e =>
            {
                input.SetSource(e.newValue == "Keyboard / mouse" ? InputSource.Keyboard : InputSource.UsbController);
                Refresh();
            });
            devices.RegisterValueChangedCallback(e =>
            {
                int index = devices.choices.IndexOf(e.newValue);
                if (index >= 0 && index < deviceList.Count) input.SelectDevice(deviceList[index]);
                Refresh();
            });
            root.Q<Button>("controller-button").clicked += TogglePanel;
            root.Q<Button>("close-controller").clicked += ClosePanel;
            input.SettingsRequested += TogglePanel;
            input.CloseSettingsRequested += ClosePanel;
            root.Q<Button>("rescan-button").clicked += Refresh;
            centre.clicked += () =>
            {
                if (input.BeginCalibration(out string error)) profileStatus.text = "";
                else profileStatus.text = error;
            };
            finish.clicked += () =>
            {
                if (input.FinishCalibration(out string error)) profileStatus.text = "Calibration complete. Check directions, then save the profile.";
                else profileStatus.text = error;
                Refresh();
            };
            cancel.clicked += () => { input.CancelCalibration(); Refresh(); };
            save.clicked += () =>
            {
                try { input.SaveProfile(); profileStatus.text = "Controller profile saved."; }
                catch (Exception ex) when (ex is IOException || ex is ArgumentException ||
                    ex is InvalidOperationException || ex is UnauthorizedAccessException)
                { profileStatus.text = ex.Message; }
            };
            deadzone.RegisterValueChangedCallback(e => input.Profile.deadzone = Mathf.Clamp(e.newValue, 0, 0.3f));
            expo.RegisterValueChangedCallback(e => input.Profile.expo = Mathf.Clamp01(e.newValue));
            root.Q<Button>("open-telemetry").clicked += () =>
            {
                string directory = Path.Combine(Application.persistentDataPath, "telemetry");
                Directory.CreateDirectory(directory);
                Application.OpenURL(new Uri(directory + Path.DirectorySeparatorChar).AbsoluteUri);
            };
            Refresh();
        }

        void OnDestroy()
        {
            if (input == null) return;
            input.SettingsRequested -= TogglePanel;
            input.CloseSettingsRequested -= ClosePanel;
        }

        void TogglePanel() => SetOpen(!open);
        void ClosePanel() { if (open) SetOpen(false); }

        void SetOpen(bool value)
        {
            open = value;
            session.SettingsOpen = value;
            session.Disarm();
            panel.EnableInClassList("hidden", !value);
            if (open) Refresh();
            else
            {
                input.CancelCalibration();
                root.panel.focusController.focusedElement?.Blur();
            }
        }

        void Refresh()
        {
            bool useUsb = input.Source == InputSource.UsbController;
            usb.style.display = useUsb ? DisplayStyle.Flex : DisplayStyle.None;
            keyboardInfo.style.display = useUsb ? DisplayStyle.None : DisplayStyle.Flex;
            deviceList = RcInput.AvailableDevices();
            devices.choices = deviceList.Select((d, i) => $"{i + 1}. {d.displayName}").ToList();
            int selected = deviceList.IndexOf(input.Device);
            devices.SetValueWithoutNotify(selected >= 0 ? devices.choices[selected] : "No controller");
            devices.SetEnabled(deviceList.Count > 0);
            deadzone.SetValueWithoutNotify(input.Profile.deadzone);
            expo.SetValueWithoutNotify(input.Profile.expo);
            mappings.Clear();
            rawAxes.Clear();
            rawValues.Clear();
            rawFills.Clear();
            string[] names = { "Roll", "Pitch", "Yaw", "Throttle" };
            var paths = input.Axes.Select(RcInput.RelativePath).ToList();
            for (int i = 0; i < 4; i++)
            {
                var binding = input.Profile.Channel(i);
                var row = new VisualElement();
                row.AddToClassList("mapping-row");
                var label = new Label(names[i]);
                label.AddToClassList("mapping-label");
                row.Add(label);
                var dropdown = new DropdownField { choices = paths.ToList() };
                dropdown.AddToClassList("mapping-dropdown");
                dropdown.SetValueWithoutNotify(binding.path);
                dropdown.SetEnabled(paths.Count > 0);
                dropdown.RegisterValueChangedCallback(e =>
                {
                    binding.path = e.newValue;
                    input.InvalidateCalibration();
                    profileStatus.text = "Mapping changed. Recalibrate before arming.";
                });
                row.Add(dropdown);
                var reverse = new Toggle("Invert");
                reverse.AddToClassList("reverse-toggle");
                reverse.SetValueWithoutNotify(binding.reverse);
                reverse.RegisterValueChangedCallback(e =>
                {
                    binding.reverse = e.newValue;
                    session.Disarm();
                });
                row.Add(reverse);
                mappings.Add(row);
            }
            foreach (var axis in input.Axes)
            {
                var row = new VisualElement();
                row.AddToClassList("raw-axis-row");
                var name = new Label(RcInput.RelativePath(axis));
                name.AddToClassList("raw-axis-name");
                var bar = new VisualElement();
                bar.AddToClassList("raw-axis-bar");
                var fill = new VisualElement();
                fill.AddToClassList("raw-axis-fill");
                bar.Add(fill);
                var value = new Label("0.000");
                value.AddToClassList("raw-axis-value");
                row.Add(name);
                row.Add(bar);
                row.Add(value);
                rawAxes.Add(row);
                rawValues.Add(value);
                rawFills.Add(fill);
            }
        }

        void Update()
        {
            if (!open || Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + 0.05f;
            deviceStatus.text = input.Status;
            bool connected = input.Device != null && input.Device.added && input.Device.enabled;
            centre.SetEnabled(connected && input.HasSignal && input.Calibration == null);
            finish.SetEnabled(connected && input.Calibration != null);
            cancel.SetEnabled(input.Calibration != null);
            save.SetEnabled(connected && input.Profile.calibrated && input.Calibration == null && input.Profile.Validate(out _));
            if (input.Calibration != null)
                instruction.text = $"Move every channel to both endpoints. Samples captured: {input.Calibration.SampleCount}. Then finish the range.";
            else
                instruction.text = input.Profile.calibrated ? "Calibrated. Check channel directions before arming."
                    : "Centre roll, pitch and yaw. Keep throttle low. Capture the centre, then move all four channels to their endpoints.";
            for (int i = 0; i < rawValues.Count && i < input.Axes.Count; i++)
            {
                float value = input.ReadAxis(input.Axes[i]);
                rawValues[i].text = value.ToString("F3");
                rawFills[i].style.width = Length.Percent(Mathf.Clamp01((value + 1) * 0.5f) * 100);
            }
        }
    }
}
