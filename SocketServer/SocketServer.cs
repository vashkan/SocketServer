////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Socket = System.Net.Sockets.Socket;

namespace SocketServerSample
{
    public static class MySocketServer
    {
        public static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
       // public static OutputPort led1 =new OutputPort(Pins.GPIO_PIN_D13,false);

        public static void Main()
        {
            const Int32 c_port = 12000;

            Socket server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, c_port);

            server.Bind(localEndPoint);
            server.Listen(Int32.MaxValue);

            while (true)
            {
                // Wait for a client to connect.
                Socket clientSocket = server.Accept();
                // Process the client request.  true means asynchronous.
                new ProcessClientRequest(clientSocket, true);
            }
        }
    }

    /// <summary>
    /// Processes a client request.
    /// </summary>
    internal sealed class ProcessClientRequest
    {
        private Socket m_clientSocket;
        Drive2 motor; 

        /// <summary>
        /// The constructor calls another method to handle the request, but can 
        /// optionally do so in a new thread.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="asynchronously"></param>
        public ProcessClientRequest(Socket clientSocket, Boolean asynchronously)
        {
            m_clientSocket = clientSocket;

            if (asynchronously)
            {
                new Thread(ProcessRequest).Start();
            }
            else ProcessRequest();
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        private void ProcessRequest()
        {
            const Int32 c_microsecondsPerSecond = 1000000;
            motor = new Drive2(50);

            using (m_clientSocket)
            {
                Byte[] buffer = new Byte[4];

                while (true)
                {
                    try
                    {
                        Int32 bytesRead = m_clientSocket.Receive(buffer, 4, SocketFlags.None);
                        motor.PowerLF = buffer[0];
                        motor.PowerRF = buffer[1];
                        motor.PowerLB = buffer[2];
                        motor.PowerRB = buffer[3];

                        byte[] sendPower = new byte[4] { motor.PowerLF, motor.PowerRF, motor.PowerLB, motor.PowerRB };

                        m_clientSocket.Send(sendPower);
                    }
                    catch (Exception e)
                    {
                        Debug.Print(e.ToString());
                        break;
                    }
                }
            }
        }
    }
}
