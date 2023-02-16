using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Sockets
{
    public class TestSockets : MonoBehaviour
    {
        private static Socket _listener;
        private CancellationTokenSource _source;
        public ManualResetEvent AllDone;
        public Renderer objectRenderer;
        private Color _matColor;
        
        public static readonly int Port = 1755;
        public static readonly int WaitTime = 1;
        
        public int msgCount = 0;
        public float msgInSec;
        public float time;

        private TestSockets()
        {
            _source = new CancellationTokenSource();
            AllDone = new ManualResetEvent(false);
        }

        private async void Start()
        {
            objectRenderer = GetComponent<Renderer>();
            await Task.Run(() => ListenEvents(_source.Token));
        }

        private void Update()
        {
            // objectRenderer.material.color = _matColor;
            time += Time.deltaTime;
            msgInSec = msgCount / time;
        }
        
        private void ListenEvents(CancellationToken token)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);
            
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(10);

                while (!token.IsCancellationRequested)
                {
                    AllDone.Reset();

                    // Debug.Log("Waiting for a connection... host: " + ipAddress.MapToIPv4() + " port: " + Port);
                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                    while (!token.IsCancellationRequested)
                    {
                        if (AllDone.WaitOne(WaitTime))
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            AllDone.Set();
            
            StateObject state = new StateObject();
            state.Websocket = handler;
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.Websocket;

            int read = handler.EndReceive(ar);

            if (read > 0)
            {
                state.ColorCode.Append(Encoding.ASCII.GetString(state.Buffer, 0, read));
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback),
                    state);
            }
            else
            {
                if (state.ColorCode.Length > 0)
                {
                    string content = state.ColorCode.ToString();
                    // Debug.Log($"Read {content.Length} bytes from socket.\n Data: {content}");
                    SetColors(content);
                }

                handler.Close();
            }
        }

        private void SetColors(string data)
        {
            msgCount++;
            // string[] colors = data.Split(',');
            // _matColor = new Color()
            // {
            //     r = float.Parse(colors[0]) / 255f,
            //     g = float.Parse(colors[1]) / 255f,
            //     b = float.Parse(colors[2]) / 255f,
            //     a = float.Parse(colors[3]) / 255f
            // };
        }

        private void OnDestroy()
        {
            _source.Cancel();
        }
        
    }

    internal class StateObject
    {
        public StringBuilder ColorCode = new StringBuilder();
        public const int BufferSize = 1024;
        public readonly byte[] Buffer = new byte[BufferSize];
        public Socket Websocket = null;
    }
}
