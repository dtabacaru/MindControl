using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace WowAutomater
{
    public static class Input
    {
        public static InputSimulator InputSimulator = new InputSimulator();

        public static void MoveMouseTo(double x, double y)
        {
            InputSimulator.Mouse.MoveMouseTo(x, y);
        }

        public static void KeyPress(VirtualKeyCode vk)
        {
            InputSimulator.Keyboard.KeyPress(vk);
            InputSleep();
        }

        public static void KeyDown(VirtualKeyCode vk)
        {
            InputSimulator.Keyboard.KeyDown(vk);
            InputSleep();
        }

        public static void KeyUp(VirtualKeyCode vk)
        {
            InputSimulator.Keyboard.KeyUp(vk);
            InputSleep();
        }

        public static void RightClick()
        {
            InputSimulator.Mouse.RightButtonClick();
            InputSleep();
        }

        public static void RightClickDown()
        {
            InputSimulator.Mouse.RightButtonDown();
            InputSleep();
        }

        public static void RightClickUp()
        {
            InputSimulator.Mouse.RightButtonUp();
            InputSleep();
        }

        public static void LeftClick()
        {
            InputSimulator.Mouse.LeftButtonClick();
            InputSleep();
        }

        public static void Click(int button)
        {
            InputSimulator.Mouse.XButtonClick(button);
        }

        public static void BottomClick()
        {
            InputSimulator.Mouse.XButtonClick(3);
            InputSleep();
        }

        public static void TopClick()
        {
            InputSimulator.Mouse.XButtonClick(4);
            InputSleep();
        }

        public static void LeftClickDown()
        {
            InputSimulator.Mouse.LeftButtonDown();
            InputSleep();
        }

        public static void LeftClickUp()
        {
            InputSimulator.Mouse.LeftButtonUp();
            InputSleep();
        }

        public static void InputSleep()
        {
            //Helper.WaitSeconds(0.001 + (Helper.RandomNumberGenerator.NextDouble() * 0.001));
            Helper.WaitSeconds(Helper.RandomNumberGenerator.NextDouble() * 0.001);
        }

    }
}
