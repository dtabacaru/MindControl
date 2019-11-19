using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace ClassicWowNeuralParasite
{
    public static class WaypointFollower
    {
        public static double TurnToleranceRad = 0.08;
        public static double PositionTolerance = 0.08;
        public static double ClosestPointDistance = 1.00;

        private static int m_WaypointIndex = 0;
        private static volatile bool m_FollowingWaypoints = false;
        private static EventWaitHandle m_StopWaypointEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static TurningDirection m_TurningDirection = TurningDirection.None;

        private static List<double> m_XCoordinates = new List<double>();
        private static List<double> m_YCoordinates = new List<double>();

        public static Jitterizer Jitterizer = new Jitterizer();

        private static bool m_Initialized = false;

        public static void SetWaypoints(List<double> xCoordinates, List<double> yCoordinates)
        {
            if(!m_Initialized)
            {
                Jitterizer.UpDown = false;
                Jitterizer.DownUp = false;
                Jitterizer.Left = true;
                Jitterizer.Right = true;
                Jitterizer.Clockwise = false;
                Jitterizer.CounterClockwise = false;
                m_Initialized = true;
            }

            m_XCoordinates.Clear();
            m_YCoordinates.Clear();

            m_XCoordinates.AddRange(xCoordinates);
            m_YCoordinates.AddRange(yCoordinates);

            m_WaypointIndex = 0;
        }

        private static void FindClosestWaypoint()
        {
            double closestDistance = double.MaxValue;
            int closestWaypointIndex = 0;

            for (int i = 0; i < m_XCoordinates.Count; i++)
            {
                double distance = Math.Sqrt(Math.Pow((m_XCoordinates[i] - WowApi.PlayerData.PlayerXPosition), 2) +
                                            Math.Pow((m_YCoordinates[i] - WowApi.PlayerData.PlayerYPosition), 2));

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWaypointIndex = i;
                }
            }

            m_WaypointIndex = closestWaypointIndex;
        }

        public static void StopFollowingWaypoints()
        {
            if (m_FollowingWaypoints)
            {
                m_FollowingWaypoints = false;
                m_StopWaypointEventWaitHandle.WaitOne();

                m_TurningDirection = TurningDirection.None;
            }
        }

        enum TurningDirection
        {
            None,
            Left,
            Right
        }

        public static void FollowWaypoints(bool loop)
        {
            if (!m_FollowingWaypoints)
            {
                if (m_XCoordinates.Count == 0 || m_YCoordinates.Count == 0)
                    return;

                m_FollowingWaypoints = true;

                Helper.WaitSeconds(1);
                Input.KeyDown(VirtualKeyCode.VK_W);

                Task.Run(() =>
                {
                    while (m_FollowingWaypoints)
                    {
                        WowApi.Sync.WaitOne();

                        double distanceToWaypoint = Math.Sqrt(Math.Pow(m_XCoordinates[m_WaypointIndex] - WowApi.PlayerData.PlayerXPosition, 2) +
                                                           Math.Pow(m_YCoordinates[m_WaypointIndex] - WowApi.PlayerData.PlayerYPosition, 2));

                        if (distanceToWaypoint > ClosestPointDistance)
                            FindClosestWaypoint();

                        double actualHeading_x = (WowApi.PlayerData.PlayerXPosition - Math.Sin(WowApi.PlayerData.PlayerHeading)) -
                                                 WowApi.PlayerData.PlayerXPosition;

                        double actualHeading_y = (WowApi.PlayerData.PlayerYPosition - Math.Cos(WowApi.PlayerData.PlayerHeading)) -
                                                 WowApi.PlayerData.PlayerYPosition;

                        double desiredHeading_x = m_XCoordinates[m_WaypointIndex] - WowApi.PlayerData.PlayerXPosition;
                        double desiredHeading_y = m_YCoordinates[m_WaypointIndex] - WowApi.PlayerData.PlayerYPosition;

                        double requriedTurn = Math.Atan2(actualHeading_x * desiredHeading_y - actualHeading_y * desiredHeading_x,
                                                         actualHeading_x * desiredHeading_x + actualHeading_y * desiredHeading_y);

                        if (Math.Abs(requriedTurn) < TurnToleranceRad)
                        {
                            if (m_TurningDirection != TurningDirection.None)
                            {
                                Input.KeyUp(VirtualKeyCode.VK_D);
                                Input.KeyUp(VirtualKeyCode.VK_A);
                            }

                            m_TurningDirection = TurningDirection.None;
                        }
                        else if (requriedTurn > 0)
                        {
                            if (m_TurningDirection != TurningDirection.Right)
                            {
                                Input.KeyUp(VirtualKeyCode.VK_A);
                                Input.KeyDown(VirtualKeyCode.VK_D);
                            }

                            m_TurningDirection = TurningDirection.Right;
                        }
                        else if (requriedTurn < 0)
                        {
                            if (m_TurningDirection != TurningDirection.Left)
                            {
                                Input.KeyUp(VirtualKeyCode.VK_D);
                                Input.KeyDown(VirtualKeyCode.VK_A);
                            }

                            m_TurningDirection = TurningDirection.Left;
                        }

                        Jitterizer.RandomJitter();

                        bool xReached = Math.Abs(WowApi.PlayerData.PlayerXPosition - m_XCoordinates[m_WaypointIndex]) < PositionTolerance;
                        bool yReached = Math.Abs(WowApi.PlayerData.PlayerYPosition - m_YCoordinates[m_WaypointIndex]) < PositionTolerance;

                        if (xReached && yReached)
                        {
                            if (m_WaypointIndex == (m_XCoordinates.Count - 1))
                            {
                                if (!loop)
                                {
                                    break;
                                }

                                m_XCoordinates.Reverse();
                                m_YCoordinates.Reverse();

                                m_WaypointIndex = 0;
                            }

                            m_WaypointIndex++;
                        }
                    }

                    Input.KeyUp(VirtualKeyCode.VK_W);
                    Input.KeyUp(VirtualKeyCode.VK_A);
                    Input.KeyUp(VirtualKeyCode.VK_D);
                    m_StopWaypointEventWaitHandle.Set();
                });
            }
        }

    }
}
