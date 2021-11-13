/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: NetworkStream.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////
namespace Fix;

public class NetworkStream : System.Net.Sockets.NetworkStream
{
    public NetworkStream(System.Net.Sockets.Socket socket, bool ownsSocket)
    : base(socket, ownsSocket)
    {
    }

    public new System.Net.Sockets.Socket Socket { get { return base.Socket; } }
}

