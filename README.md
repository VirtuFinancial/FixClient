# FIX Client

[![FIX Client](https://github.com/GaryHughes/FixClient/actions/workflows/dotnet.yml/badge.svg)](https://github.com/GaryHughes/FixClient/actions/workflows/dotnet.yml)

## Synopsis

FIX Client is a Windows desktop program designed for manually testing [FIX protocol](https://www.fixtrading.org/online-specification/) end points. It can simulate both initiator and acceptor behaviour. FIX Client is written in C# using Winforms and is built on a C# FIX library that can be used standalone to build other programs.  

## Build Requirements

* Visual Studio Community Edition.

## Installation

* Install the [.NET 5.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-5.0.3-windows-x64-installer)
    * **NB** FIX Client is not compatible with **.NET 5.0.4** Plesae see [here](https://github.com/GaryHughes/FixClient/issues/2) for details.  
* FIX Client is distributed as .NET Core single file application, just copy FixClient.exe and run it.

## Acknowledgements

This repository includes parts of the www.fixtrading.org XML Repository Copyright (c) FIX Protocol Ltd. All Rights Reserved.
