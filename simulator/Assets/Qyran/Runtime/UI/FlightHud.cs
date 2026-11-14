using Qyran.Flight;
using Qyran.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace Qyran.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class FlightHud : MonoBehaviour
    {
        public FlightSession session;
        public RcInput input;
        public FollowCamera flightCamera;

        VisualElement root;
        VisualElement messageBox;
        VisualElement progress;
        readonly Label[] channelValues = new Label[4];
        readonly VisualElement[] channelMarkers = new VisualElement[4];
        Label altitude, speed, flightTime, state, impacts, fps, exercise, message, source, help;
        Button arm, cameraButton, record;
        Slider throttleSlider;
        float nextRefresh;
        float frameTime = 1f / 60;

        void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            root.Query<Button>().ForEach(button => button.focusable = false);
            altitude = root.Q<Label>("altitude");
            speed = root.Q<Label>("speed");
            flightTime = root.Q<Label>("flight-time");
            state = root.Q<Label>("flight-state");
            impacts = root.Q<Label>("impacts");
            fps = root.Q<Label>("fps");
            exercise = root.Q<Label>("exercise");
            message = root.Q<Label>("message");
            source = root.Q<Label>("input-source");
            help = root.Q<Label>("controls-help");
            messageBox = root.Q("message-box");
            progress = root.Q("hover-progress");
            arm = root.Q<Button>("arm-button");
            cameraButton = root.Q<Button>("camera-button");
            record = root.Q<Button>("record-button");
            throttleSlider = root.Q<Slider>("keyboard-throttle");
            throttleSlider.focusable = false;
            throttleSlider.RegisterValueChangedCallback(e => input.SetKeyboardThrottle(e.newValue));
            arm.clicked += session.ToggleArm;
            root.Q<Button>("reset-button").clicked += session.ResetFlight;
            cameraButton.clicked += flightCamera.Toggle;
            record.clicked += session.ToggleRecording;
            string[] names = { "roll", "pitch", "yaw", "throttle" };
            for (int i = 0; i < 4; i++)
            {
                channelValues[i] = root.Q<Label>(names[i] + "-value");
                channelMarkers[i] = root.Q(names[i] + "-marker");
            }
        }

        void Update()
        {
            frameTime = Mathf.Lerp(frameTime, Time.unscaledDeltaTime, 0.04f);
            if (root == null || Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + 0.05f;
            altitude.text = session.Altitude.ToString("F1");
            speed.text = session.Speed.ToString("F1");
            int seconds = Mathf.FloorToInt(session.FlightTime);
            flightTime.text = $"{seconds / 60:00}:{seconds % 60:00}";
            state.text = session.Armed ? "Armed" : "Disarmed";
            arm.text = session.Armed ? "Disarm" : "Arm motors";
            root.EnableInClassList("armed", session.Armed);
            impacts.text = session.Collisions + (session.Collisions == 1 ? " impact" : " impacts");
            fps.text = Mathf.RoundToInt(1 / Mathf.Max(0.001f, frameTime)) + " FPS";
            exercise.text = session.Exercise;
            progress.style.width = Length.Percent(session.ExerciseStage > 0 ? 100 : session.HoverProgress * 100);
            source.text = input.SourceName;
            throttleSlider.style.display = input.Source == InputSource.Keyboard ? DisplayStyle.Flex : DisplayStyle.None;
            throttleSlider.SetValueWithoutNotify(input.Command.Throttle);
            cameraButton.text = flightCamera.ModeName;
            record.text = session.Recorder.IsRecording ? "Stop recording" : "Record";
            message.text = session.Message;
            messageBox.style.display = string.IsNullOrEmpty(session.Message) ? DisplayStyle.None : DisplayStyle.Flex;
            help.text = input.Source == InputSource.Keyboard
                ? "W A S D  Tilt     Q E  Yaw     ↑ ↓  Throttle     H  50% thrust     Space  Arm     C  Camera     R  Reset"
                : "RC sticks  Flight control     Space  Arm / disarm     C  Camera     R  Reset     L  Record";
            for (int i = 0; i < 4; i++)
            {
                float value = input.Command.Channel(i);
                channelValues[i].text = i == 3 ? Mathf.RoundToInt(value * 100) + "%" : value.ToString("F2");
                channelMarkers[i].style.left = Length.Percent(i == 3 ? value * 97 : (value + 1) * 48.5f);
            }
        }
    }
}
