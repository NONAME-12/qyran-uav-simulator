# Qyran UAV Simulator

**Diploma title:** Development of a UAV Control Simulator in Unity Using an External Remote Controller

**Program:** Software Engineering, SE-2404  
**Team:** Nurkhan Orazbay — RC-controller input and flight-control software; Dauren Zheksenov — Unity simulation environment and quadrotor physics  
**Supervisor:** Zarina Al'mabekovna Kutpanova

## Problem and purpose
Physical quadrotor flight training involves hardware cost, risk and suitable operating conditions. The project develops an inspectable desktop simulator where a physical RC controller can feed calibrated commands into a simplified quadrotor model and training scenario. Target users include drone students, beginner FPV pilots and educators. The diploma deliverable is a **Unity software simulator**; controlling real aircraft is outside its required scope.

## Deliverables for the first pre-defense
- [One structured pre-defense report covering all 11 required components](docs/PreDefense_1.md)
- [System Context diagram](diagrams/01_context.svg), [Use Case diagram](diagrams/02_use_cases.svg), [Component diagram](diagrams/03_components.svg), each with editable `.dot` source
- [Independent RC-input diagnostics demonstrator](prototype/rc-diagnostics/index.html), with [automated channel-math tests](prototype/rc-diagnostics/control_math.test.js)
- Example [controller profile](config/controller-profile.example.json)

## Technologies
Planned final implementation: **Unity / C#**, Unity Input System or a compatible adapter, Rigidbody-based 6-DoF physics, JSON configuration and CSV telemetry. The current technical proof of concept uses HTML/JavaScript and is **not** the finished Unity build.

## Project links and progress
- [GitHub source repository](https://github.com/NONAME-12/qyran-uav-simulator)
- [YouTrack Kanban with 15 assigned issues](https://qyran-uav-simulator.youtrack.cloud/agiles/204-1/current)

On 22 Oct 2026 the design/research materials and browser diagnostics are available. Hardware transmitter input, Unity integration, real interviews and substantive supervisor review are not yet evidenced. YouTrack is the source of truth for current issue statuses; this README is not a second task board.

## Run and test the existing prototype
Open `prototype/rc-diagnostics/index.html` in Chrome/Edge. The **Simulated** mode runs without a controller; the optional browser Gamepad mode depends on USB-compatible hardware.

If Node.js is available, test the mathematical mappings:
```shell
node --test prototype/rc-diagnostics/control_math.test.js
```

The full Unity application is a later development milestone. Do not interpret browser input diagnostics as evidence of flight physics or actual transmitter integration.
