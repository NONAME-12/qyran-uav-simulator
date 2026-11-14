# Qyran UAV Simulator

Development of a UAV Control Simulator in Unity Using an External Remote Controller.

Software Engineering, SE-2404. Nurkhan Orazbay: RC input and flight control.
Dauren Zheksenov: simulation environment and quadrotor dynamics.
Supervisor: Zarina Al'mabekovna Kutpanova.

## Run

Open the simulator folder in Unity Hub using Unity 6000.3.11f1.
Open Assets/Qyran/Scenes/TrainingYard.unity and press Play.

The Windows application builds through Qyran > Build Windows in the editor.
The result is Builds/Windows/Qyran.exe in the repository root. Keep the executable
and its accompanying folders together.

## First working version

- Training yard with a quadrotor, launch pad, gates and collision geometry.
- Keyboard and mouse controls, and a USB joystick/gamepad adapter using Unity Input System.
- Four-channel mapping, inversion, centre/range calibration, deadzone and expo.
- Device-specific JSON controller profiles.
- Thrust and angle stabilisation applied to a Rigidbody, with chase and FPV cameras.
- Low-throttle arming interlock; disarming on input loss, focus loss and configuration changes.
- Takeoff, a five-second hover at two metres, and landing on the launch pad.
- CSV telemetry recorded at the 50 Hz physics update rate.

The flight model uses collective thrust and a PD attitude controller. It is an initial
integration model; individual rotor dynamics and validation against real flight data
remain future work.

## Controls

| Input | Action |
| --- | --- |
| W / S | Pitch forward / back |
| A / D | Roll left / right |
| Q / E | Yaw left / right |
| Up / Down | Adjust throttle |
| Mouse slider | Adjust throttle on the flight panel |
| H | Set throttle to 50%, the model's level hover thrust |
| Space | Arm / disarm |
| C | Chase / FPV camera |
| R | Reset flight |
| L | Start / stop recording |
| Tab | Controller settings |

Throttle must be low and sticks centred before arming. H sets thrust; it does not
hold altitude or cancel vertical velocity.

## USB controller

Connect the transmitter in USB joystick mode. Open Controller, select USB
controller, choose the device and assign four different axes. Raw device axes
are visible below the mapping controls.

Centre roll, pitch and yaw, keep throttle low, then select Capture centre.
Move every channel through its complete range and select Finish range.
Check channel directions, adjust inversion as needed and save the profile.
Close the panel, lower throttle and arm.

Profiles are stored in %USERPROFILE%/AppData/LocalLow/Qyran/Qyran/profiles.
CSV recordings are in the sibling telemetry folder, accessible from the
controller panel.

The USB adapter is tested with synthetic joystick events in Unity Input System.
A physical transmitter has not yet been available for hardware testing.

## Tests

Open Window > General > Test Runner and run both EditMode and PlayMode suites.
There are 30 EditMode tests and 16 PlayMode tests.
Tests cover channel processing, asymmetric calibration, profile persistence,
device events, disconnects, focus loss, arming interlocks, CSV formatting and
Rigidbody flight behaviour.

The earlier browser diagnostics remain in prototype/rc-diagnostics. Their tests run with:

~~~shell
node --test prototype/rc-diagnostics/control_math.test.js
~~~

## Five-minute demonstration

1. Open the training scene, arm with Space and demonstrate takeoff, tilt and camera switching.
2. Open Controller, show mapping, calibration controls and the handling of an absent USB device.
3. Show RcInput.cs, ChannelMath.cs and ArmInterlock.cs: device values become normalised commands with a guarded arming step.
4. Record a short flight and open its CSV.
5. Show the EditMode and PlayMode test results.

## Project material

- [GitHub](https://github.com/NONAME-12/qyran-uav-simulator)
- [YouTrack](https://qyran-uav-simulator.youtrack.cloud/agiles/204-1/current)
- [First pre-defense report](docs/PreDefense_1.md)
- [Context](diagrams/01_context.png), [use cases](diagrams/02_use_cases.png), [components](diagrams/03_components.png)
