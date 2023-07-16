using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Builder;
using DroneFootball;
using DroneRace;
using Newtonsoft.Json;
using UnityEngine;

namespace Sockets
{
    public class Server : MonoBehaviour
    {
        public MioData MioData;
        public DroneController player;
        
        private static Socket _listener;
        private CancellationTokenSource _source;
        public ManualResetEvent allDone;
        private string _data;

        public const int Port = 8888;
        public const int Waittime = 1;

        private void Awake()
        {
            _source = new CancellationTokenSource();
            allDone = new ManualResetEvent(false);
        }

        private async void Start()
        {
            await Task.Run(() => ListenEvents(_source.Token));
        }

        private void ListenEvents(CancellationToken token)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(10);
                
                while (!token.IsCancellationRequested)
                {
                    allDone.Reset();

                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
                    
                    while (!token.IsCancellationRequested)
                    {
                        if (allDone.WaitOne(Waittime))
                        {
                            break;
                        }
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
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            
            allDone.Set();

            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int read = handler.EndReceive(ar);

            if (read > 0)
            {
                _data = Encoding.ASCII.GetString(state.buffer, 0, read);
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback),
                    state);
            }
            else
            {
                if (_data.Length > 1)
                {
                    Debug.Log(_data);
                    MioData = JsonConvert.DeserializeObject<MioData>(_data);
                    if(player)
                        SetDroneData(MioData, player);
                }
                handler.Close();
            }
        }

        private void SetDroneData(MioData data, DroneController drone)
        {
            drone.throttle = data.LeftMio.X / 1000f;
            drone.pedals = data.LeftMio.Y / 1000f;
            drone.cyclic.x = data.RightMio.X / 1000f;
            drone.cyclic.y = data.RightMio.Y / 1000f;
        }

        private void OnDestroy()
        {
            _source.Cancel();
            _source.Dispose();
        }

        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
        }
    }
}
