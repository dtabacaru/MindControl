using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WowAutomater;

namespace MindControlUI
{
    public enum WebInterfaceCommandType
    {
        Unknown,
        Screenshot,
        Start,
        Stop,
        Relay
    }

    public class AutomaterWebInterface
    {
        private int Port = 8001;
        private int BackLog = 20;

        private TcpListener m_Server;

        private volatile bool m_RunFlag = false;
        private EventWaitHandle m_StopWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        public AutomaterWebInterface()
        {
            m_Server = new TcpListener(IPAddress.Any, Port);
        }

        private byte[] GetScreenJpegBytes()
        {
            Rectangle bounds = new Rectangle(0, 0, 1920, 1080);
            byte[] jpegImage;

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(new Point(0, 0), Point.Empty, bounds.Size);
                }

                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Jpeg);

                    jpegImage = memoryStream.ToArray();
                }
            }

            return jpegImage;
        }

        private void Relay(NetworkStream ns, string rawRelayString)
        {
            string relayString = rawRelayString.Substring(6, rawRelayString.Length - 6);
            relayString = relayString.Replace("%20", " ");

            Automater.SetRelayString(relayString);

            string contentString = "Sent: " + relayString;

            string responseString = "HTTP/1.1 200 OK\r\n";
            responseString += "Content-Length: " + contentString.Length.ToString() + "\r\n";
            responseString += "Content-Type: text/html\r\n";
            responseString += "Connection: Closed\r\n\r\n";
            responseString += contentString;

            ns.Write(ASCIIEncoding.ASCII.GetBytes(responseString), 0, responseString.Length);
        }

        private void SendScreenShot(NetworkStream ns)
        {
            byte[] jpegImage = GetScreenJpegBytes();

            string responseString = "HTTP/1.1 200 OK\r\n";
            responseString += "Content-Length: " + jpegImage.Length.ToString() + "\r\n";
            responseString += "Content-Type: image/jpeg\r\n";
            responseString += "Connection: Closed\r\n\r\n";

            List<byte> contentBytes = new List<byte>();
            contentBytes.AddRange(ASCIIEncoding.ASCII.GetBytes(responseString));
            contentBytes.AddRange(jpegImage);

            ns.Write(contentBytes.ToArray(), 0, contentBytes.Count);
        }

        private void Start(NetworkStream ns)
        {
            string contentString = "Started";

            Automater.RemoteStart();

            string responseString = "HTTP/1.1 200 OK\r\n";
            responseString += "Content-Length: " + contentString.Length.ToString() + "\r\n";
            responseString += "Content-Type: text/html\r\n";
            responseString += "Connection: Closed\r\n\r\n";
            responseString += contentString;

            ns.Write(ASCIIEncoding.ASCII.GetBytes(responseString), 0, responseString.Length);
        }

        private void Stop(NetworkStream ns)
        {
            string contentString = "Stopped";

            Automater.RemoteStop();

            string responseString = "HTTP/1.1 200 OK\r\n";
            responseString += "Content-Length: " + contentString.Length.ToString() + "\r\n";
            responseString += "Content-Type: text/html\r\n";
            responseString += "Connection: Closed\r\n\r\n";
            responseString += contentString;

            ns.Write(ASCIIEncoding.ASCII.GetBytes(responseString), 0, responseString.Length);
        }

        private void HandleTcpClientConnection(TcpClient tcpClient)
        {
            try
            {
                using (NetworkStream ns = tcpClient.GetStream())
                {
                    byte[] data = new byte[16384];
                    int bread = ns.Read(data, 0, 16384);

                    byte[] truncatedData = new byte[bread];
                    Array.Copy(data, truncatedData, bread);

                    string dataString = ASCIIEncoding.ASCII.GetString(truncatedData);

                    string[] dataStringLines = dataString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                    if (dataStringLines.Length < 1)
                        return;

                    string requestString = dataStringLines[0];

                    string[] requestParts = requestString.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (requestParts.Length != 3)
                        return;

                    if (requestParts[0].ToUpper() != "GET")
                        return;

                    if (requestParts[2].ToUpper() != "HTTP/1.1")
                        return;

                    WebInterfaceCommandType command = ParseCommandString(requestParts[1]);

                    switch (command)
                    {
                        case WebInterfaceCommandType.Relay:
                            Relay(ns, requestParts[1]);
                            break;
                        case WebInterfaceCommandType.Start:
                            Start(ns);
                            break;
                        case WebInterfaceCommandType.Stop:
                            Stop(ns);
                            break;
                        case WebInterfaceCommandType.Screenshot:
                            SendScreenShot(ns);
                            break;
                        default:
                            return;
                    }

                }
            }
            catch { }
        }

        private WebInterfaceCommandType ParseCommandString(string commandString)
        {
            if(commandString.Length < 1)
                return WebInterfaceCommandType.Unknown;

            if (commandString == "/")
                return WebInterfaceCommandType.Screenshot;

            if (commandString.Length < 5)
                return WebInterfaceCommandType.Unknown;

            if (commandString.Substring(0,5) == "/stop")
                return WebInterfaceCommandType.Stop;

            if (commandString.Length < 6)
                return WebInterfaceCommandType.Unknown;

            if (commandString.Substring(0, 6) == "/start")
                return WebInterfaceCommandType.Start;

            if (commandString.Substring(0, 6) == "/relay")
                return WebInterfaceCommandType.Relay;

            return WebInterfaceCommandType.Unknown;
        }

        private void Run()
        {
            m_Server.Start(BackLog);
            m_RunFlag = true;

            while (m_RunFlag)
            {
                if(!m_Server.Pending())
                {
                    System.Threading.Thread.Sleep(10);
                    continue;
                }
                else
                {
                    TcpClient tcpClient = m_Server.AcceptTcpClient();

                    Task.Run(() =>
                    {
                        HandleTcpClientConnection(tcpClient);
                        tcpClient.Dispose();
                    });
                }
            }

            m_StopWaitHandle.Set();
        }

        public void Start()
        {
            Task.Run(() =>
            {
                Run();
            });
        }

        public void Stop()
        {
            m_RunFlag = false;
            m_Server.Stop();
            m_StopWaitHandle.WaitOne();
        }
    }
}
