/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Session.Net.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace FixClient
{
    public partial class Session
    {
        public IPEndPoint EndPoint()
        {
            if(Behaviour == Fix.Behaviour.Initiator)
                return new IPEndPoint(Fix.Network.GetAddress(Host), Port);
            return new IPEndPoint(0, Port);
        }

        public IPEndPoint BindEndPoint()
        {
            IPAddress address = Fix.Network.GetLocalAddress(BindHost);
            IPEndPoint endPoint = address != null ? new IPEndPoint(address, BindPort) : new IPEndPoint(0, BindPort);
            return endPoint;
        }
    }
}
