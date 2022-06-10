PG Proxy/"Profiler" for .NET 
========
pg_proxy_net is a console-based profiler for PostgreSQL. 
It is a very simple TCP proxy for PostgreSQL 
written using .NET Core, based on Netproxy.

It is intended to be a PG analogue to Microsoft SQL-Server Profiler. 

It outputs the sql statements executed by an application (up to the buffer limit of 81'920 bytes interpreted as utf8-unicode). <br />
It opens TCP port 6666 on the loopback interface and forwards to 127.0.0.1:5432 (PostgreSQL-default-server-port).<br />
You now connect your application to 127.0.0.1:6666 instead of 127.0.0.1:5432, and you'll see what it is doing. <br />
It will not work with TLS/encryption. 

Netproxy is a simple ipv6/ipv4 UDP & TCP proxy based on .NET 5.0, tested on *win10-x64* and *ubuntu.16.20-x64*.

Why? 
====
To create an analogue to SQL-Server profiler without having to enable logging on the server. <br />


Why NetProxy ? 
====
We needed a simple, crossplatform IPV6 compatible UDP forwarder, and couldn't find a satisfying solution. 
Nginx was obviously a great candidate but building it on Windows with UDP forwarding enabled was quite a pain.

The objective is to be able to expose as an ipv6 endpoint a server located in an ipv4 only server provider.

Limitations
===========
Each remote client is mapped to a port of the local server therefore:
- The original IP of the client is hidden to the server the packets are forwarded to.
- The number of concurrent clients is limited by the number of available ports in the server running the proxy.

Disclaimer
==========
Error management exist, but is minimalist. <br />
IPV6 is not supported on the forwarding side.

Usage
=====
- Compile for your platform following instructions at https://www.microsoft.com/net/core
- Rewrite the `config.json` file to fit your need
- Run NetProxy

Configuration
=============
`config.json` contains a map of named forwarding rules, for instance :

    {
     "http": {
     "localport": 80,
     "localip":"",
     "protocol": "tcp",
     "forwardIp": "xx.xx.xx.xx",
     "forwardPort": 80
     },
    ...
    }

- *localport* : The local port the forwarder should listen to.
- *localip* : An optional local binding IP the forwarder should listen to. If empty or missing, it will listen to ANY_ADDRESS.
- *protocol* : The protocol to forward. `tcp`,`udp`, or `any`.
- *forwardIp* : The ip the traffic will be forwarded to.
- *forwardPort* : The port the traffic will be forwarded to.

