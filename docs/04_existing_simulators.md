# Comparison of existing FPV simulators
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
