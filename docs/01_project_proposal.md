# Project proposal and scope
**Title:** Development of a UAV Control Simulator in Unity Using an External Remote Controller.

## Problem
Hands-on quadrotor training consumes batteries and airframe life, needs safe flying space and suitable weather, and can be unsuitable for first attempts. Existing commercial FPV simulators already address much of this problem; this project is not presented as the first simulator. The engineering objective is an accessible, inspectable software system where a trainer can trace raw external controller signals through the control-processing pipeline to simulated movement and telemetry.

## Target users
(1) Students learning basic multirotor flight, (2) FPV pilots training with their own physical radio, (3) instructors demonstrating the effect of tuning and control choices, and (4) software engineering students studying hardware/software integration.

## Aim
Design, implement and evaluate a Windows desktop Unity simulator for manual quadrotor control using one confirmed compatible external RC transmitter.

## Objectives
1. Identify user and hardware constraints.
2. Model RC device discovery, axes, mapping and calibration, including lost-input handling.
3. Implement modular manual flight command processing and configurable simplified six-DoF dynamics.
4. Build a 3D scene with obstacles, landing zone, FPV and third-person views.
5. Record RC commands and simulated UAV state in synchronized logs.
6. Implement reproducible training cases (takeoff, stable hover, landing).
7. Evaluate functionality, software latency and scenario results with documented conditions.

## Explicit scope
**In:** one quadrotor, one RC device profile, Windows desktop, Unity/C#, four primary channels, basic tuning, error reporting, virtual training, telemetry.
**Out:** swarm control, automatic real-UAV actuation, verified real-world flight-dynamics equivalence, aviation training certification, cross-platform and universal RC compatibility.

## Individual contributions
- **Nurkhan Orazbay:** RC input adapter, axis mapping, calibration, manual control logic, telemetry and diagnostics.
- **Dauren Zheksenov:** Unity scene, physics, quadrotor model, cameras, collisions, training exercises and visual interaction.
- **Joint:** system contracts, integration, requirements, testing, documentation, code review.

## Research questions
RQ1: How consistently can four manual RC axes be delivered and normalized?
RQ2: How do deadzone and expo affect computed control signals?
RQ3: Can a bounded-physics simplified quadrotor produce reproducible basic training scenarios?
RQ4: What delays, invalid inputs and recoverable failures arise in the input-to-simulation pipeline?

## Expected outcomes
Source code, demonstrable desktop simulator, reproducible baseline exercise, device profiles, log format, evaluation report and technical documentation. The final Unity executable is the **target of subsequent implementation**, not a result already observed at this checkpoint.
