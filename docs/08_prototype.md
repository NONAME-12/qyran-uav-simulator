# Prototype and user journey
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
