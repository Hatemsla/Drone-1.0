using System;
using UnityEngine;
using WebSocketSharp;

namespace Sockets
{
    public class TestWebSockets : MonoBehaviour
    {
        private WebSocket _ws;
        private string _message;
        private bool _isOpen;
        private bool _isMessage;

        public int msgCount = 0;
        public float msgInSec;
        public float time;

        private void Start()
        {
            _ws = new WebSocket("ws://192.168.1.130:8080");
            
            _ws.OnOpen += WsOnOpen;
            _ws.OnMessage += WsOnMessage;
            
            _ws.Connect();
        }

        private void Update()
        {
            time += Time.deltaTime;
            msgInSec = msgCount / time;
        }

        private void WsOnMessage(object sender, MessageEventArgs e)
        {
            // _isMessage = true;
            // _message = e.Data;
            msgCount++;
        }

        private void WsOnOpen(object sender, EventArgs e)
        {
            _isOpen = true;
            Debug.Log("Ws open!");
        }
    }
}