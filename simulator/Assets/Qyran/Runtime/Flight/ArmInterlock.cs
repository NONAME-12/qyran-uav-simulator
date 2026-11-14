using Qyran.Input;

namespace Qyran.Flight
{
    public static class ArmInterlock
    {
        public static bool CanArm(FlightCommand command, bool hasSignal, bool configured, out string reason)
        {
            if (!hasSignal) { reason = "No input signal."; return false; }
            if (!configured) { reason = "Calibrate the controller before arming."; return false; }
            if (command.Throttle > 0.05f) { reason = "Lower throttle before arming."; return false; }
            if (System.Math.Abs(command.Roll) > 0.15f || System.Math.Abs(command.Pitch) > 0.15f ||
                System.Math.Abs(command.Yaw) > 0.15f)
            {
                reason = "Centre the sticks before arming.";
                return false;
            }
            reason = "";
            return true;
        }
    }
}
