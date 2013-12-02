using System;
using System.IO.Ports;
using System.Threading;
using System.IO;

using DNT.Diag.Interop;

namespace DNT.Diag.IO
{
  public class Commbox
  {
    private IOBuffer _stream = null;
    private IntPtr _native = IntPtr.Zero;
    private CommboxVer _version = CommboxVer.UNKNOW;
    private SerialPortThread _portThread = null;

    public Commbox(CommboxVer ver)
    {
      _stream = new IOBuffer();
      _native = NativeMethods.RCommboxNew(_stream.Native, (int)ver);
      if (_native == IntPtr.Zero)
        throw new OutOfMemoryException();
      _version = ver;
    }

    ~Commbox()
    {
      NativeMethods.RCommboxFree(_native);
    }

    public CommboxVer Version
    {
      get
      {
        return _version;
      }
    }

    private void OpenC168SerialMode()
    {
      var portNames = SerialPort.GetPortNames();
      foreach (var portName in portNames)
      {
        try
        {
          SerialPort port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
          port.Open();

          Thread.Sleep(50);
          port.DtrEnable = true;
          Thread.Sleep(50);

          _portThread = new SerialPortThread(_stream, port);

          for (int i = 0; i < 3; i++)
          {
            if (!NativeMethods.RCommboxConnect(_native))
              continue;
            if (!NativeMethods.RCommboxC168BeginBaudChange(_native))
              continue;
            port.BaudRate = 57600;
            if (!NativeMethods.RCommboxC168EndBaudChange(_native))
              continue;
            return;
          }
        }
        catch
        {
        }
      }
      throw new IOException();
    }

    private void OpenW80SerialMode()
    {
      throw new NotImplementedException();
    }

    public void Connect()
    {
      if (Version == CommboxVer.C168)
      {
        OpenC168SerialMode(); // C168 Only support SerialPort.
      }
      else if (Version == CommboxVer.W80)
      {
        OpenW80SerialMode(); // W80 only support SerialPort.
      }
      else
      {
        throw new InvalidOperationException();
      }
    }

    public void Disconnect()
    {
      if (!NativeMethods.RCommboxDisconnect(_native))
        throw new IOException();
    }
  }
}
