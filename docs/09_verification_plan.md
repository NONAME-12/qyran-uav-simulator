# Verification and evaluation plan
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
