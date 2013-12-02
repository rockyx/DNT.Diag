using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;

namespace DNT.Diag.IO
{
  internal class SerialPortThread
  {
    private SerialPort _port = null;
    private IOBuffer _stream = null;
    private Task _writeTask = null;
    private byte[] _readBuff = new byte[256];
    private byte[] _writeBuff = new byte[256];

    public SerialPortThread(IOBuffer stream, SerialPort port)
    {
      _port = port;
      _stream = stream;
      _port.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
      {
        try
        {
          int count = _port.BytesToRead;
          while (count != 0)
          {
            count = Math.Min(count, _readBuff.Length);
            _port.Read(_readBuff, 0, count);
            _stream.PushToEcuBuffer(_readBuff, 0, count);
            count = _port.BytesToRead;
          }
        }
        catch
        {
        }
      };
      _writeTask = Task.Factory.StartNew(() =>
      {
        while (_port.IsOpen)
        {
          try
          {
            int count = _stream.ToEcuBufferBytesAvailable();
            while (count != 0)
            {
              count = Math.Min(count, _writeBuff.Length);
              _stream.PopFromEcuBuffer(_writeBuff, 0, count);
              _port.Write(_writeBuff, 0, count);
              count = _stream.ToEcuBufferBytesAvailable();
            }
            Thread.Sleep(1);
            Thread.Yield();
          }
          catch
          {

          }
        }
      });
    }
  }
}
