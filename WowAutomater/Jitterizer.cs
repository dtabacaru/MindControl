using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace WowAutomater
{
    public class Jitterizer
    {
        private volatile bool m_Jitterizing = false;

        public bool UpDown = true;
        public bool DownUp = true;
        public bool LeftRight = true;
        public bool RightLeft = true;
        public bool Jump = true;
        public bool Left = false;
        public bool Right = false;
        public bool Clockwise = true;
        public bool CounterClockwise = true;

        public double WaitTime = 0.08;
        public double Rate = 0.01;

        private delegate void JitterAction();
        private List<JitterAction> m_JitterActions = new List<JitterAction>();

        public void Jitter()
        {
            if (!UpDown &&
                !DownUp &&
                !LeftRight &&
                !RightLeft &&
                !Jump &&
                !Left &&
                !Right &&
                !Clockwise &&
                !CounterClockwise)
                return;

            m_Jitterizing = true;

            m_JitterActions.Clear();

            if (UpDown)
                m_JitterActions.Add(new JitterAction(JitterUpDown));
            if (DownUp)
                m_JitterActions.Add(new JitterAction(JitterDownUp));
            if (LeftRight)
                m_JitterActions.Add(new JitterAction(JitterLeftRight));
            if (RightLeft)
                m_JitterActions.Add(new JitterAction(JitterRightLeft));
            if (Jump)
                m_JitterActions.Add(new JitterAction(JitterJump));
            if (Left)
                m_JitterActions.Add(new JitterAction(JitterLeft));
            if (Right)
                m_JitterActions.Add(new JitterAction(JitterRight));
            if (Clockwise)
                m_JitterActions.Add(new JitterAction(JitterClockwise));
            if (CounterClockwise)
                m_JitterActions.Add(new JitterAction(JitterCounterClockwise));

            double rand = Helper.RandomNumberGenerator.NextDouble();
            double probabilityIncrement = 1.0 / m_JitterActions.Count;
            double currentProbability = 0;

            for (int i = 0; i < m_JitterActions.Count; i++)
            {
                if ((rand >= currentProbability) && (rand < (currentProbability + probabilityIncrement)))
                {
                    int thei = i;
                    Task.Run(() =>
                    {
                        m_JitterActions[thei]();
                        m_Jitterizing = false;
                    });

                    break;
                }

                currentProbability += probabilityIncrement;
            }
        }

        public void RandomJitter()
        {
            if (!m_Jitterizing && (Helper.RandomNumberGenerator.NextDouble() <= Rate))
            {
                Jitter();
            }
        }

        private void JitterUpDown()
        {
            JitterUp();
            JitterDown();
        }

        private void JitterDownUp()
        {
            JitterDown();
            JitterUp();
        }

        private void JitterLeftRight()
        {
            JitterLeft();
            JitterRight();
        }

        private void JitterRightLeft()
        {
            JitterRight();
            JitterLeft();
        }

        private void JitterLeft()
        {
            Input.KeyDown(VirtualKeyCode.LEFT);
            Helper.WaitSeconds(WaitTime);
            Input.KeyUp(VirtualKeyCode.LEFT);
        }

        private void JitterRight()
        {
            Input.KeyDown(VirtualKeyCode.RIGHT);
            Helper.WaitSeconds(WaitTime);
            Input.KeyUp(VirtualKeyCode.RIGHT);
        }

        private void JitterUp()
        {
            Input.KeyDown(VirtualKeyCode.UP);
            Helper.WaitSeconds(WaitTime);
            Input.KeyUp(VirtualKeyCode.UP);
        }

        private void JitterDown()
        {
            Input.KeyDown(VirtualKeyCode.DOWN);
            Helper.WaitSeconds(WaitTime);
            Input.KeyUp(VirtualKeyCode.DOWN);
        }

        private void JitterJump()
        {
            Input.KeyPress(VirtualKeyCode.SPACE);
            Helper.WaitSeconds(1.0);
        }

        private void JitterClockwise()
        {
            JitterRight();
            JitterDown();
            JitterLeft();
            JitterUp();
        }

        private void JitterCounterClockwise()
        {
            JitterLeft();
            JitterDown();
            JitterRight();
            JitterUp();
        }
    }
}
