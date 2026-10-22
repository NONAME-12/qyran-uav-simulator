# Requirements and verifiable acceptance criteria
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
