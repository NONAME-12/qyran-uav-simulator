# Critical literature review — IEEE numbered citations
This review prioritizes original scientific works on quadrotor simulation, modeling, virtual flight training and their limitations. There are **13 academic publications** here, of which one is explicitly labeled a preprint. Technical manuals are referenced separately in [technology decisions](05_technology_decisions.md).

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
