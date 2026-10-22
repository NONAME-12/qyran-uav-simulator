# Project management, responsibilities, risks and links
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
