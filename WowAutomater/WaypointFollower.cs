using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;
using WowApi;

namespace WowAutomater
{
    public class Waypoint
    {
        public uint MapId;
        public double X;
        public double Y;

        public Waypoint(uint mapId, double x, double y)
        {
            MapId = mapId;
            X = x;
            Y = y;
        }
    }

    public static class WaypointFollower
    {
        public static double TurnToleranceRad = 0.08;
        public static double PositionTolerance = 0.08;
        public static double ClosestPointDistance = 1.00;

        private static int m_WaypointIndex = 0;
        private static volatile bool m_FollowingWaypoints = false;
        private static EventWaitHandle m_StopWaypointEventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private static TurningDirection m_TurningDirection = TurningDirection.None;

        private static List<Waypoint> m_Waypoints = new List<Waypoint>();

        public static Jitterizer Jitterizer = new Jitterizer();

        private static bool m_Initialized = false;

        public static void SetWaypoints(List<Waypoint> waypoints)
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

            m_Waypoints.Clear();
            m_Waypoints.AddRange(waypoints);

            m_WaypointIndex = 0;
        }

        private static void FindClosestWaypoint()
        {
            double closestDistance = double.MaxValue;
            int closestWaypointIndex = 0;

            for (int i = 0; i < m_Waypoints.Count; i++)
            {
                double distance = Math.Sqrt(Math.Pow((m_Waypoints[i].X - Api.PlayerData.PlayerXPosition), 2) +
                                            Math.Pow((m_Waypoints[i].Y - Api.PlayerData.PlayerYPosition), 2));

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
            if (!m_FollowingWaypoints && m_Waypoints.Count > 0)
            {
                m_FollowingWaypoints = true;

                Helper.WaitSeconds(1);
                Input.KeyDown(VirtualKeyCode.VK_W);

                Task.Run(() =>
                {
                    while (m_FollowingWaypoints)
                    {
                        Api.Sync.WaitOne();

                        double distanceToWaypoint = Math.Sqrt(Math.Pow(m_Waypoints[m_WaypointIndex].X - Api.PlayerData.PlayerXPosition, 2) +
                                                           Math.Pow(m_Waypoints[m_WaypointIndex].Y - Api.PlayerData.PlayerYPosition, 2));

                        if (distanceToWaypoint > ClosestPointDistance)
                            FindClosestWaypoint();

                        double actualHeading_x = (Api.PlayerData.PlayerXPosition - Math.Sin(Api.PlayerData.PlayerHeading)) -
                                                 Api.PlayerData.PlayerXPosition;

                        double actualHeading_y = (Api.PlayerData.PlayerYPosition - Math.Cos(Api.PlayerData.PlayerHeading)) -
                                                 Api.PlayerData.PlayerYPosition;

                        double desiredHeading_x = m_Waypoints[m_WaypointIndex].X - Api.PlayerData.PlayerXPosition;
                        double desiredHeading_y = m_Waypoints[m_WaypointIndex].Y - Api.PlayerData.PlayerYPosition;

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

                        bool xReached = Math.Abs(Api.PlayerData.PlayerXPosition - m_Waypoints[m_WaypointIndex].X) < PositionTolerance;
                        bool yReached = Math.Abs(Api.PlayerData.PlayerYPosition - m_Waypoints[m_WaypointIndex].Y) < PositionTolerance;

                        if (xReached && yReached)
                        {
                            if (m_WaypointIndex == (m_Waypoints.Count - 1))
                            {
                                if (!loop)
                                {
                                    break;
                                }

                                m_Waypoints.Reverse();
                                m_WaypointIndex = 0;
                            }

                            if(m_Waypoints[m_WaypointIndex+1].MapId != m_Waypoints[m_WaypointIndex].MapId)
                            {
                                m_TurningDirection = TurningDirection.None;
                                Input.KeyUp(VirtualKeyCode.VK_D);
                                Input.KeyUp(VirtualKeyCode.VK_A);

                                while(Api.PlayerData.MapId != m_Waypoints[m_WaypointIndex + 1].MapId)
                                {
                                    Helper.WaitSeconds(0.001);
                                    continue;
                                }
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
