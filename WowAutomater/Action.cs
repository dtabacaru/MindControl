using WindowsInput.Native;

namespace WowAutomater
{
    public class Action
    {
        private VirtualKeyCode m_ActionKey;

        public Action(VirtualKeyCode actionKey)
        {
            m_ActionKey = actionKey;
        }

        public void Act()
        {
            Input.KeyPress(m_ActionKey);
            Helper.WaitSeconds(Automater.RegisterDelay);
        }

    }
}
