using System.Collections;
using NUnit.Framework;
using Qyran.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

namespace Qyran.Tests
{
    public sealed class DesktopInputTests : InputTestFixture
    {
        [UnityTest]
        public IEnumerator KeyboardShortcutsFeedTheFlightCommand()
        {
            var keyboard = InputSystem.AddDevice<Keyboard>();
            var owner = new GameObject("Desktop input test");
            var input = owner.AddComponent<RcInput>();
            try
            {
                yield return null;
                input.SendMessage("OnApplicationFocus", true);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.H, Key.W, Key.D));
                yield return null;
                input.Poll(0.02f);
                Assert.That(input.Command.Throttle, Is.EqualTo(0.5f).Within(0.001f));
                Assert.That(input.Command.Roll, Is.GreaterThan(0));
                Assert.That(input.Command.Pitch, Is.LessThan(0));
            }
            finally
            {
                Object.Destroy(owner);
                InputSystem.RemoveDevice(keyboard);
            }
        }

        [Test]
        public void BriefShortcutIsNotLostBetweenFrames()
        {
            var keyboard = InputSystem.AddDevice<Keyboard>();
            var owner = new GameObject("Short key press test");
            var input = owner.AddComponent<RcInput>();
            try
            {
                input.SendMessage("OnApplicationFocus", true);
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.H));
                InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                InputSystem.Update();
                input.Poll(0.02f);
                Assert.That(input.Command.Throttle, Is.EqualTo(0.5f).Within(0.001f));
            }
            finally
            {
                Object.DestroyImmediate(owner);
                InputSystem.RemoveDevice(keyboard);
            }
        }
    }
}
