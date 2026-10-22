# Software architecture
The solution follows the chain:

```text
Physical transmitter (USB supported gamepad mode)
            |
            v
 RC device adapter (axes + timestamp + device ID)
            |
            v
 Calibration / mapping (min, center, max, reverse)
            |
            v
 Filtering / deadzone / expo
            |
            v
 Manual flight controller (limits, modes, output)
            |
            v
 Unity quadrotor dynamics (Rigidbody, fixed Δt)
            |
      +-----+------+
      v            v
   Scene / HUD   Telemetry + scenario result
```

## Component contracts
| Component | Input | Output |
|---|---|---|
| RC Adapter | OS input samples | deviceId, raw axes, timestamp, connection state |
| Mapping | raw axes, profile | roll/pitch/yaw in [-1,1], throttle [0,1] |
| Flight Control | calibrated command, state | bounded desired rates/forces |
| Dynamics | forces, torques, fixed time | position, velocity, quaternion, angular speed |
| Scenario | state/events | goal progress, collision, finish/failure |
| Telemetry | input, command, state | timestamped CSV/JSON rows |
| UI | system state | views and controls, not direct manipulation of physics |

## Important invariants
- The RC Adapter does not mutate Unity rigid bodies.
- The frame-rate-independent physics integration runs in a fixed-step update.
- Channel indices are not assumed to be CH1=roll, CH2=pitch, etc.
- Loss of input clears the fresh-input state and activates a defined fallback.
- Physical actuation is absent from mandatory simulator scope.
- Raw inputs and transformed outputs share a consistent time base in logs.

## Three editable diagrams
- [Context](../diagrams/01_context.dot)
- [Use Cases](../diagrams/02_use_cases.dot)
- [Components](../diagrams/03_components.dot)

All are Graphviz `.dot` source. PNG/SVG exports can be generated from these sources.
