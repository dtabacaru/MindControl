using System;
using WowApi;

namespace WowAutomater
{
    public class RecordPathEventArgs : EventArgs
    {
        public Waypoint CurrentWaypoint;

        public RecordPathEventArgs(Waypoint currentWaypoint)
        {
            CurrentWaypoint = currentWaypoint;
        }
    }

    public static class RecordWowPath
    {
        public static double SplitDistance = 0.5;
        private static volatile bool m_RecordPath = false;

        public static void RecordPath()
        {
            m_RecordPath = true;

            double lastSplitX = -1;
            double lastSplitY = -1;
            while (m_RecordPath)
            {
                Api.Sync.WaitOne();

                if (Api.PlayerData.Found)
                {
                    double currentX = Api.PlayerData.PlayerXPosition;
                    double currentY = Api.PlayerData.PlayerYPosition;
                    uint currentMapId = Api.PlayerData.MapId;

                    if (lastSplitX == -1)
                    {
                        lastSplitX = currentX;
                        lastSplitY = currentY;

                        RecordPathEvent?.Invoke(null, new RecordPathEventArgs(new Waypoint(currentMapId, currentX, currentY)));

                        continue;
                    }

                    double splitDistance = Math.Sqrt((currentX - lastSplitX) * (currentX - lastSplitX) + (currentY - lastSplitY) * (currentY - lastSplitY));

                    if (splitDistance < SplitDistance)
                        continue;

                    lastSplitX = currentX;
                    lastSplitY = currentY;

                    RecordPathEvent?.Invoke(null, new RecordPathEventArgs(new Waypoint(currentMapId, currentX, currentY)));
                }
            }

            StopEvent?.Invoke(null, new EventArgs());
        }

        public static void StopRecordingPath()
        {
            m_RecordPath = false;
        }

        public delegate void RecordPathEventHandler(object sender, RecordPathEventArgs wea);
        public static event RecordPathEventHandler RecordPathEvent;

        public delegate void StopRecordingHandler(object sender, EventArgs ea);
        public static event StopRecordingHandler StopEvent;
    }
}
