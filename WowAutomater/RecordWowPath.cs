using System;

namespace ClassicWowNeuralParasite
{
    public class RecordPathEventArgs : EventArgs
    {
        public double X;
        public double Y;

        public RecordPathEventArgs(double x, double y)
        {
            X = x;
            Y = y;
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
                WowApi.Sync.WaitOne();

                if (WowApi.CurrentPlayerData.Found)
                {
                    double currentX = WowApi.CurrentPlayerData.PlayerXPosition;
                    double currentY = WowApi.CurrentPlayerData.PlayerYPosition;

                    if (lastSplitX == -1)
                    {
                        lastSplitX = currentX;
                        lastSplitY = currentY;

                        RecordPathEvent?.Invoke(null, new RecordPathEventArgs(currentX, currentY));

                        continue;
                    }

                    double splitDistance = Math.Sqrt((currentX - lastSplitX) * (currentX - lastSplitX) + (currentY - lastSplitY) * (currentY - lastSplitY));

                    if (splitDistance < SplitDistance)
                        continue;

                    lastSplitX = currentX;
                    lastSplitY = currentY;

                    RecordPathEvent?.Invoke(null, new RecordPathEventArgs(currentX, currentY));
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
