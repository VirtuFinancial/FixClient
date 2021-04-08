/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: TcpSocketListener.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Net;
using System.Net.Sockets;

namespace FixClient
{
    class TcpSocketListener : TcpListener
    {
        public TcpSocketListener(IPEndPoint endPoint)
            : base(endPoint)
        {
        }

        public new bool Active
        {
            get { return base.Active; }
        }
    }
}
