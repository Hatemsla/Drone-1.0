using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DroneFootball;
using DroneRace;
using Menu;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace Sockets
{
    public class Server : MonoBehaviour
    {
        public MioData MioData;
        public MenuManager menuManager;
        public DroneFootballController droneFootballController;
        public DroneRaceController droneRaceController;
        
        private static Socket _listener;
        private readonly CancellationTokenSource _source;
        public readonly ManualResetEvent AllDone;
        public string _data;

        public const int Port = 8888;
        public const int Waittime = 1;

        private Server()
        {
            _source = new CancellationTokenSource();
            AllDone = new ManualResetEvent(false);
        }

        private async void Start()
        {
            await Task.Run(() => ListenEvents(_source.Token));
        }

        private void ListenEvents(CancellationToken token)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress =
                ipHostInfo.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(10);

                while (!token.IsCancellationRequested)
                {
                    AllDone.Reset();

                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                    while (!token.IsCancellationRequested)
                    {
                        if (AllDone.WaitOne(Waittime))
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
            
            AllDone.Set();

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
                    MioData = JsonConvert.DeserializeObject<MioData>(_data);
                    if(droneFootballController)
                        SetDroneFootballData(MioData, droneFootballController);
                    else if(droneRaceController)
                        SetDroneRaceData(MioData, droneRaceController);
                }
                handler.Close();
            }
        }

        private void SetDroneFootballData(MioData data, DroneFootballController drone)
        {
            drone.throttle = data.LeftMio.X / 1000f;
            drone.pedals = data.LeftMio.Y / 1000f;
            drone.cyclic.x = data.RightMio.X / 1000f;
            drone.cyclic.y = data.RightMio.Y / 1000f;
        }
        
        private void SetDroneRaceData(MioData data, DroneRaceController drone)
        {
            drone.throttle = data.LeftMio.X / 1000f;
            drone.pedals = data.LeftMio.Y / 1000f;
            drone.cyclic.x = data.RightMio.X / 1000f;
            drone.cyclic.y = data.RightMio.Y / 1000f;
        }

        private void OnDestroy()
        {
            _source.Cancel();
        }

        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
        }
    }
}
