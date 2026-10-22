# Qyran UAV Control Simulator

**Diploma:** Development of a UAV Control Simulator in Unity Using an External Remote Controller  
**Program:** Software Engineering, SE-2404  
**Team:** Nurkhan Orazbay — external RC integration, channel mapping, flight-control logic, telemetry; Dauren Zheksenov — Unity simulation, quadrotor dynamics, cameras, training scenes.  
**Supervisor:** Zarina Almaбековна Kutpanova.

**GitHub:** https://github.com/NONAME-12/qyran-uav-simulator  
**YouTrack:** https://qyran-uav-simulator.youtrack.cloud/agiles/204-1/current

## Project overview
Qyran is a research-oriented desktop training simulator for one quadrotor. The design separates device input, calibration and mapping, flight-control commands, six-degree-of-freedom dynamics, rendering, exercise logic, and telemetry. The diploma deliverable is **software and simulation**; real aircraft integration is outside the required scope.

## First checkpoint: 22–24 October 2026
The repository contains the project proposal, critical scientific review (13 scientific works), a comparison of four existing simulators, architectural decisions, FR/NFR requirements, three editable diagrams, user-interface prototype, verification protocol, risk register, and YouTrack task specifications. A small independent browser-based RC input diagnostics demonstrator and automated mathematics tests are also supplied.

These are **completed analysis/design artifacts**. Hardware RC compatibility, a compiled Unity application, interviews with external respondents, and supervisor approval are **not represented as completed tests**. See [checkpoint status](docs/12_checkpoint_status.md) for exact boundaries.

## Read these files
- [Project proposal](docs/01_project_proposal.md)
- [Evidence and user research](docs/02_relevance_evidence.md)
- [Scientific literature review](docs/03_literature_review.md)
- [Existing simulator comparison](docs/04_existing_simulators.md)
- [Technology choices](docs/05_technology_decisions.md)
- [Software requirements](docs/06_requirements.md)
- [Architecture](docs/07_architecture.md)
- [UX and prototype](docs/08_prototype.md)
- [Verification protocol](docs/09_verification_plan.md)
- [Project management](docs/10_project_management.md)
- [Supervisor review register](docs/11_supervisor_feedback.md)
- [Checkpoint status](docs/12_checkpoint_status.md)
- [YouTrack issue specifications](youtrack/issue_specs.md)

## Demonstrator
Open `prototype/rc-diagnostics/index.html` in Chrome/Edge. Simulated input mode works without RC hardware; browser Gamepad API mode requires a supported USB joystick device. This is a **separate technical proof of concept, not Unity integration or flight validation**.

Run mathematical processing checks with Node.js:
```sh
node --test prototype/rc-diagnostics/control_math.test.js
```

## Development and assessment
One shared repository. Each student commits from their own GitHub account. Create task branches using **real** YouTrack issue IDs, attach commit/PR links, and request teammate review. A task moves to Done only after the stated acceptance criteria have actually been checked.
