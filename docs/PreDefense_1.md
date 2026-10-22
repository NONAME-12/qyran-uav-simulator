# Qyran UAV Simulator — Pre-Defense 1 documentation

**Topic:** Development of a UAV Control Simulator in Unity Using an External Remote Controller  
**Team:** Nurkhan Orazbay (RC input, calibration, flight control, diagnostics) and Dauren Zheksenov (Unity simulation, quadrotor dynamics, cameras and training scenes)  
**Supervisor:** Zarina Al'mabekovna Kutpanova  
**GitHub:** https://github.com/NONAME-12/qyran-uav-simulator  
**YouTrack:** https://qyran-uav-simulator.youtrack.cloud/agiles/204-1/current

This document contains the **11 components specified in the course checkpoint**. It is the only main Markdown report: task states are maintained in YouTrack, not replicated across additional status files.

**Checkpoint evidence (22 Oct 2026):** The supplied YouTrack screenshot shows 15 issues: 9 queued, 2 in development, 1 in review and 3 marked done (QYR-1–QYR-3). Their card states do not independently verify review. The repository includes design material and a self-contained browser-based RC diagnostic example; it does not yet contain a running Unity aircraft simulator. No user interviews or supervisor approval are claimed.

## Contents

1. Project proposal and scope
2. Evidence of relevance and interview plan
3. Critical scientific literature review (IEEE)
4. Existing simulator comparison
5. Technology decisions
6. Functional and non-functional requirements
7. Architecture and editable diagrams
8. UX/prototype
9. Verification/evaluation protocol
10. Project management and risks
11. Supervisor feedback

---

## Project proposal and scope
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


---

## Relevance evidence and user validation

## Confirmed evidence
Scientific studies on drone flight simulation and teleoperation [1]–[3] support the use of simulated training and the need to evaluate realistic control behavior. They do not prove that the Qyran application is effective. The project's target user's practical needs should be checked independently.

## External discussions
**Status on 22 October 2026: no verifiable records of 3–5 respondent interviews or client agreement are available.** No interview quotes, dates or identities have been invented in this report.

## Interview instrument (short, structured)
Suggested respondents: beginner drone pilot, experienced FPV pilot, instructor, or a student using an RC transmitter.

1. Which drone simulators have you used and what did you practice?
2. What is difficult when connecting your own RC transmitter?
3. Which manual-flight actions do you want to practice safely?
4. How should the simulator report mapping mistakes or device disconnects?
5. Which parameters and flight statistics should be visible after the exercise?
6. Which improvements are more important: visual quality, input response, physical behavior, or usable feedback?

## Evidence template
Date | Respondent role | Consent status | Current practice | Verbatim answer / factual note | Interpretation | Requirement ID

Treat a respondent's statement as evidence of that person's need, **not** a verified market-wide claim. For privacy, share anonymized summaries with explicit consent. Links to actual evidence must be added only after real discussions.

## Preliminary assumptions for later validation
A1. Physical RC calibration should show raw and normalized axes (FR-02–05). 
A2. A disconnected transmitter should create a visible error state (FR-14).
A3. Basic hover, obstacle contact and landing should support reproducible exercise feedback (FR-10, FR-13).
These are **design hypotheses**, not user-confirmed findings.


---

## Critical literature review — IEEE numbered citations
This review prioritizes original scientific works on quadrotor simulation, modeling, virtual flight training and their limitations. There are **13 academic publications** here, of which one is explicitly labeled a preprint. Technical manuals are referenced separately in [technology decisions(PreDefense_1.md).

| Ref | Approach / result | Limitation | Design response in Qyran |
|---|---|---|---|
| [1] | Examines simulation-based preparation of drone pilots | Does not validate Qyran learning outcomes | Define explicit flight exercises, do not claim verified skill transfer |
| [2] | Modular flexible quadrotor simulation research | Research platform not turnkey RC teaching interface | Separate input, dynamics and rendering modules |
| [3] | Builds virtual UAV teleoperation environment | Control equipment and evaluation differ | Explicit controller compatibility test |
| [4] | 2025 survey comparing aerial robot simulators | General comparison is not an RC hardware test | Use consistent criteria and reproducible benchmarking |
| [5] | Systematic review and meta-analysis of XR pilot training | XR/manned aviation conclusions not directly desktop FPV | Restrict training-effect claims |
| [6] | Quadrotor dynamics under disturbance and calibration | More physical detail than bounded bachelor MVP | Make model simplifications explicit |
| [7] | X-Plane flight dynamics compared with measured quadrotor data | Results depend on aircraft and model | Separate visual plausibility from physical validation |
| [8] | Comparative review of drone simulators | Product features change over time | Verify current claims against vendor information |
| [9] | Neural-network MPC for quadrotor | MPC out of manual flight MVP scope | Retain straightforward controller layers |
| [10] | IoT-associated quadrotor dynamics simulation | Not training-focused | Reuse relevant modeling concepts only |
| [11] | Modular multi-quadrotor RL simulator (preprint) | Multi-agent RL not needed for one RC-driven UAV | Define scope boundary |
| [12] | Reinforcement learning for UAV actions | Autonomous policy not equivalent to human RC input | Keep human-in-the-loop control path |
| [13] | Scope of XR simulation as adjunct to flight training | Cannot imply measured learning effects in Qyran | Plan user evaluation without claims |

## Synthesis and research gap
Research shows that viable simulation architectures, dynamics and training scenarios already exist. Qyran's intended contribution is **engineering integration and transparent evaluation**: exposing raw device input, calibrated commands, manual controller outputs, virtual state, and synchronized logs in one reproducible learning system. No fundamental new UAV dynamics theory is claimed.

## References
[1] A. Somerville, T. Lynar, K. Joiner, and G. Wild, “Use of simulation for pre-training of drone pilots,” *Drones*, vol. 8, no. 11, Art. no. 640, 2024, doi: 10.3390/drones8110640.  
[2] Y. Song, S. Naji, E. Kaufmann, A. Loquercio, and D. Scaramuzza, “Flightmare: A flexible quadrotor simulator,” *Proc. Mach. Learn. Res.*, vol. 155, pp. 1147–1157, 2021. https://proceedings.mlr.press/v155/song21a.html  
[3] M. E. Mora-Soto *et al*., “Building a realistic virtual simulator for unmanned aerial vehicle teleoperation,” *Applied Sciences*, vol. 11, no. 24, Art. no. 12018, 2021, doi: 10.3390/app112412018.  
[4] C. A. Dimmig *et al*., “Survey of simulators for aerial robots: An overview and in-depth systematic comparisons,” *IEEE Robotics & Automation Magazine*, vol. 32, no. 2, pp. 153–166, 2025, doi: 10.1109/MRA.2024.3433171.  
[5] A. Somerville *et al*., “Applications of extended reality in pilot flight simulator training: A systematic review with meta-analysis,” *Visual Computing for Industry, Biomedicine, and Art*, vol. 8, Art. no. 25, 2025, doi: 10.1186/s42492-025-00206-w.  
[6] L. Zhou *et al*., “Dynamic modeling and experimental calibration of a quadrotor unmanned aerial vehicle under wind disturbances,” *Journal of Aerospace Engineering*, vol. 39, no. 6, 2026, doi: 10.1061/JAEEEZ.ASENG-7087.  
[7] M.-H. Do, C. E. Lin, and Y.-C. Lai, “Validation of the flight dynamics engine of the X-Plane simulator in comparison with the real flight data of the quadrotor UAV using CIFER,” *Drones*, vol. 7, no. 9, Art. no. 548, 2023, doi: 10.3390/drones7090548.  
[8] M. Nikolaiev and M. Novotarskyi, “Comparative review of drone simulators,” *Information and Computing Intelligent Systems*, no. 4, pp. 79–99, 2024, doi: 10.20535/2786-8729.4.2024.300614.  
[9] B. Jiang *et al*., “Neural network based model predictive control for a quadrotor UAV,” *Aerospace*, vol. 9, no. 8, Art. no. 460, 2022, doi: 10.3390/aerospace9080460.  
[10] X. Lu and Z. Xing, “Application of IoT quadrotor dynamics simulation,” *Electronics*, vol. 11, no. 4, Art. no. 590, 2022, doi: 10.3390/electronics11040590.  
[11] Z. Huang *et al*., “QuadSwarm: A modular multi-quadrotor simulator for deep reinforcement learning with direct thrust control,” arXiv:2306.09537, 2023. **Preprint.** https://arxiv.org/abs/2306.09537  
[12] S.-E. Shen and Y.-C. Huang, “Application of reinforcement learning in controlling quadrotor UAV flight actions,” *Drones*, vol. 8, no. 11, Art. no. 660, 2024, doi: 10.3390/drones8110660.  
[13] G. Ross and A. Gilbey, “Extended reality (xR) flight simulators as an adjunct to traditional flight training methods: A scoping review,” *CEAS Aeronautical Journal*, vol. 14, pp. 799–815, 2023, doi: 10.1007/s13272-023-00688-5.

Bibliographic metadata and source interpretations should be checked against the publisher before a final thesis submission.


---

## Comparison of existing FPV simulators
**Systems:** Liftoff, VelociDrone, FPV SkyDive, DRL Simulator. The comparison is based on publicly described product features, **not** an equal hands-on benchmark. Unknown values are labeled “not verified”; this does not mean the feature is absent.

| Criterion | Liftoff | VelociDrone | FPV SkyDive | DRL Simulator | Qyran planned |
|---|---|---|---|---|---|
| Virtual UAV flight | Yes | Yes | Yes | Yes | Yes |
| FPV camera | Yes | Yes | Yes | Yes | Yes |
| External controller support advertised | Yes | Yes | Yes | Yes | One tested USB RC |
| Custom calibration/mapping details | Check | Check | Check | Check | Observable raw + normalized axes |
| Third-person flight mode | Check | Check | Check | Check | Yes |
| Training scenarios | Yes | Yes | Yes | Yes | Basic hover, landing, obstacle |
| Adjustable physics | Verify options | Verify options | Verify options | Verify options | Mass/controller parameters |
| Raw RC channel monitoring | Not verified | Not verified | Not verified | Not verified | Planned |
| Export synchronized control + state logs | Not verified | Not verified | Not verified | Not verified | Planned |
| Open and editable internal code | No | No | No | No | Own project source |

**Reason to develop Qyran:** We are not claiming commercial simulators lack training capabilities or that our physics is superior. This diploma targets a modifiable and inspectable implementation suitable for laboratory analysis of the complete input-to-flight pipeline.

Product references:
- Liftoff: https://store.steampowered.com/app/410340/Liftoff_FPV_Drone_Racing/
- VelociDrone: https://www.velocidrone.com/
- FPV SkyDive: https://store.steampowered.com/app/1278060/FPV_SkyDive__FPV_Drone_Racing__Freestyle_Simulator/
- DRL Simulator: https://www.drl.io/simulator

**Baseline for internal tests:** exactly the same virtual quadrotor and scenario with keyboard input versus normalized RC input; linear mapping compared with configured deadzone/expo.


---

## Technology comparison and architecture decision records
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


---

## Requirements and verifiable acceptance criteria
Priorities: **Must** (core acceptance) and **Should** (desirable).

| ID | Priority | Requirement | Acceptance |
|---|---|---|---|
| FR-01 | Must | Start/reset simulation | Same scenario can be restarted |
| FR-02 | Must | Detect external RC | UI shows a supported input device or explicit error |
| FR-03 | Must | Read four main axes | Roll, pitch, yaw, throttle shown separately |
| FR-04 | Must | Calibrate axes | Min, center, max and inversion saved and re-applied |
| FR-05 | Must | Configure deadzone/expo | Calculated command matches documented formula |
| FR-06 | Must | Manual flight logic | Outputs setpoint/forces, not direct Transform teleport |
| FR-07 | Must | Quadrotor dynamics | Positive controlled thrust affects ascent predictably |
| FR-08 | Must | Collisions/reset | Contact registered; reset returns to initial state |
| FR-09 | Must | Flight cameras | Switch FPV/third person without resetting position |
| FR-10 | Must | Training case | Takeoff, hover and landing can be performed |
| FR-11 | Should | Configure model | Mass and controller limits editable with validation |
| FR-12 | Must | Telemetry | Records time, command, position, orientation, events |
| FR-13 | Should | Exercise report | Duration, collisions and outcome observable |
| FR-14 | Must | Input disconnect handling | Stale RC input never treated as fresh command |
| FR-15 | Should | Save RC profiles | Profile persisted/reloaded with axis mapping |
| FR-16 | Must | Repeatability | Config and environment recorded for replay/test |

| ID | Target quality | Measurable evaluation |
|---|---|---|
| NFR-01 | Robustness | No unhandled crash on RC absence; clear message |
| NFR-02 | Software input responsiveness | Planned input-to-command p95 ≤ 30 ms on defined PC; not yet measured |
| NFR-03 | Frame performance | Planned ≥ 60 FPS in basic scene on defined PC; not yet measured |
| NFR-04 | Observability | Logs include timestamps, config and flight state |
| NFR-05 | Usability | Fresh user can map 4 axes with a written guide |
| NFR-06 | Modularity | Input, controller, dynamics and rendering have explicit boundaries |
| NFR-07 | Safety | The required build never issues real vehicle actuation |
| NFR-08 | Reproducibility | Record Unity version, branch/commit, scenario and parameters |

**Ownership:** Nurkhan handles FR-02–05, FR-06, FR-12, FR-14–15; Dauren handles FR-01, FR-07–11, FR-13; FR-16 and evaluation are joint. These criteria describe future system acceptance, not a claim of tests already passed.


---

## Software architecture
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


---

## Prototype and user journey
## Four proposed screens
1. **Dashboard:** launch exercise, controller status, training scenario selection.
2. **Controller Setup:** device detection, four channels, USB-axis assignment, min/center/max, reverse, deadzone and expo.
3. **Flight View:** FPV/third-person camera, altitude, position, orientation, collision and session status.
4. **Results:** duration, number of contacts, exercise outcome and CSV export.

## Working separate proof of concept
The `prototype/rc-diagnostics/` folder contains a browser-based input diagnostic demonstrator with simulated axes and optional Gamepad API input. It demonstrates numerical channel mapping; it is **not a Unity scene** and **not proof of compatibility with a specific external transmitter**.

## User path
Launch → select controller → check raw axes → calibrate → choose exercise → start simulated flight → finish → inspect result and logs.

## Empty and failure states
- No compatible RC device: show disconnected and allow simulated/keyboard mode for diagnostics.
- Mapping error: show raw and normalized values with axis assignment and inversion.
- Device removed: flag input loss; do not silently reuse stale input.
- No recorded session: result panel displays empty state instead of invented telemetry.


---

## Verification and evaluation plan
No end-to-end Unity/physical controller measurements are claimed as of this checkpoint.

| Test | Setup | Expected observation / acceptance | Baseline |
|---|---|---|---|
| TC-01 | RC disconnected | Explicit empty/error state, no stale commands | N/A |
| TC-02 | Move each stick separately | Four axes mapped independently | Raw USB axis readings |
| TC-03 | Set min/center/max | Neutral maps near 0; endpoints bounded | Default profile |
| TC-04 | Set deadzone/expo | Result matches formula within numerical tolerance | Linear mapping |
| TC-05 | Disconnect mid-session | Input-lost event and defined fallback | No failure handling |
| TC-06 | Hover in fixed scene | Controlled altitude change and stabilized state | Keyboard |
| TC-07 | Hit obstacle | Collision count/event, working reset | Scene without event recorder |
| TC-08 | Switch cameras | State preserved in both perspectives | Single camera |
| TC-09 | Measure input→command | Publish p50/p95 and PC configuration | Keyboard pipeline |
| TC-10 | Complete hover/landing | Report duration, contact count, completion | Same scenario keyboard |

## Conditions
Record Windows build, PC CPU/GPU, Unity version, display refresh, fixed physics timestep, input mode, calibration values and model parameters. Measure software timestamps at acquisition, mapping, command generation and physics update separately to avoid conflating signal and render latency.

## Metrics
- Channel mapping error: |normalized value − expected value|.
- Input-to-command latency: timestamp command − timestamp raw sample; report p50, p95, max.
- Exercise success, time, collisions, hover position error.
- FPS and physics step stability.

## Evidence existing at this checkpoint
The independent JavaScript control-math demonstrator includes automated checks for clipping, center/midpoint, sign, throttle range, reverse, deadzone, expo and invalid calibration. These tests do not establish physical RC support or aircraft dynamics accuracy.


---

## Project management, responsibilities, risks and links
**GitHub:** https://github.com/NONAME-12/qyran-uav-simulator
**YouTrack:** https://qyran-uav-simulator.youtrack.cloud/agiles/204-1/current

## Allocation
| Owner | Software module | Independent deliverable |
|---|---|---|
| Nurkhan Orazbay | RC USB input, mapping/calibration, manual control, telemetry/diagnostics | Reusable input-control subsystem and test protocol |
| Dauren Zheksenov | Unity environment, six-DoF model, cameras, collisions, training missions | Reproducible simulation subsystem |
| Both | Architecture, interface contract, integration, test plan, review | One evaluated system and traceable Git history |

## Planned development gates
**Gate 1 — Problem definition / design:** requirements, scientific review, comparison, ADR, diagrams, UX prototype, tests methodology (available as documents).
**Gate 2 — RC input:** demonstrate one compatible physical transmitter and measured raw/normalized channels.
**Gate 3 — Virtual quadrotor:** fixed-step physics and test scene.
**Gate 4 — Integration:** RC controls virtual UAV.
**Gate 5 — Evaluation:** scripted training, latency, scenario logs and release notes.

## Risks
| Risk | Impact | Mitigation / owner |
|---|---|---|
| Physical controller not recognized as USB gamepad | Cannot ingest RC channels | Device probe first; alternate supported adapter / Nurkhan |
| Mismatched channel order or sign | Incorrect flight commands | Calibration profile + test cases / Nurkhan |
| Unstable simplified physics | Unrealistic/faulty movement | Bounded dynamics, fixed timestep, deterministic tests / Dauren |
| Low framerate or input lag | Poor usability | Measure FPS/latency on specified PC / both |
| No user interview evidence | Requirements relevance under-supported | Conduct genuine short interviews / both |
| No supervisor review | Formal checkpoint incomplete | Request dated comments; record changes / both |
| Late single batch commit | Weak traceability of contributions | Individual task branches, PRs, realistic commit logs / both |

## Traceability discipline
Each task must have one owner, due date, acceptance criteria and evidence link. Branch/commit/PR should contain its **real** YouTrack ID. Different authors must use their own accounts; one bulk import is not evidence of progressive two-person contribution. Do not change tickets to Done without proof.


---

## Supervisor feedback and decisions log
**Supervisor:** Zarina Almaбековна Kutpanova.

No dated or attributable review of these exact checkpoint materials has been supplied. This section is a structured record **awaiting actual feedback**, not a fabricated endorsement.

| Date | Reviewed artifact | Exact feedback | Decision/change | Evidence link |
|---|---|---|---|---|
| — | Project scope | Not yet supplied | — | — |
| — | RC input strategy | Not yet supplied | — | — |
| — | Architecture and testing | Not yet supplied | — | — |

## Specific decision points
- One shared group submission versus separate individual reports.
- Required level of simplified physical-model validity.
- Confirmed RC hardware + USB connection pathway.
- Mandatory training exercises at defense.
- Whether the user interview process and evidence storage are acceptable.

Update the table after the supervisor actually reviews it.
