# Technology comparison and architecture decision records
Decisions below are provisional architecture choices; claims about device support require physical tests.

## ADR-01 — Simulation engine
| Option | Advantage | Limitation |
|---|---|---|
| Unity / C# (selected) | C# modularity, Rigidbody, UI and existing team scope | Engine update and physics-step consistency must be documented |
| Unreal Engine | High-quality scenes, C++ and Blueprints | Greater adaptation costs for target software scope |
| Godot | Open source, manageable footprint | Controller and physics proof of concept would need different implementation |

## ADR-02 — External RC input
| Option | Advantage | Limitation |
|---|---|---|
| Unity Input System + replaceable device adapter (selected) | Standard controller device abstraction | Transmitter must expose supported USB HID joystick axes |
| Direct Windows HID access | Lower-level access and diagnostics | More device-specific C# code |
| Serial bridge from external receiver | Explicit raw channel transport | Additional interface, drivers and hardware |

**Key condition:** RadioMaster/ExpressLRS radio protocol by itself does *not* guarantee an operating-system USB joystick device. The supported USB mode, axis layout, sign and sample range must be measured.

## ADR-03 — Quadrotor dynamics
| Option | Advantage | Limitation |
|---|---|---|
| Unity Rigidbody + explicit forces/torques (selected) | Integrated collisions and repeatable fixed step | Requires calibration and bounded physical validity |
| Custom six-DoF ODE integrator | Full control over equations/numerics | More development and integration effort |
| PX4 SITL integration | More authentic flight-controller stack | Excessive scope for initial manual simulation |

## ADR-04 — User interface
| Option | Advantage | Limitation |
|---|---|---|
| Unity UI Toolkit (selected pending prototype validation) | Integrated UI and styles | More setup for runtime telemetry |
| Unity uGUI | Common and straightforward UI workflow | Different maintenance style |
| Separate desktop UI | Separation from Unity renderer | IPC and duplicated deployment complexity |

### Versioning requirements
Pin the selected Unity Editor and package versions in the final runnable build. Store configuration examples without secrets. Source URLs: https://docs.unity3d.com/Manual/ ; https://docs.unity3d.com/Packages/com.unity.inputsystem@latest .
