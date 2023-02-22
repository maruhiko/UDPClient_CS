using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SamplePoC.UDPConnect
{
    /// <summary>
    /// UDP通信を行う
    /// </summary>
    internal class UdpServer
    {
        // 自分のIP, PORT
        private IPAddress _ip;
        private int _port;
        private IPEndPoint _receiveEP;

        private UdpClient _udp;
        private bool _isRunning = false;

        internal MyUdpClient(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        internal void Start()
        {
            if (_isRunning) return;

            try
            {
                _receiveEP = new IPEndPoint(_ip, _port);
                _udp = new UdpClient(_receiveEP);
                _udp.EnableBroadcast = true;
                Task.Run(() => Receive());
                _isRunning = true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        internal void Stop()
        {
            _isRunning = false;
            _udp.Close();
        }

        internal bool Send(string text)
        {

            if (!_isRunning) return false;

            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                _udp.Send(buffer, buffer.Length);
                OnSend(text);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to send {text}");
                return false;
            }

            return true;
        }

        private void Receive()
        {
            while (_isRunning)
            {
                try
                {
                    IPEndPoint senderEP = null;
                    byte[] receiveBytes = _udp.Receive(ref senderEP);
                    string text = Encoding.UTF8.GetString(receiveBytes);
                    OnReceive(text, senderEP);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            }
        }

        protected virtual void OnSend(string text)
        {
            Console.WriteLine("send: ", text);
        }

        protected virtual void OnReceive(string text, IPEndPoint endPoint)
        {
            Console.WriteLine(endPoint.ToString(), " : ", text);
        }
    }
}
