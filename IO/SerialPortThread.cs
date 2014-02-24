using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;

namespace DNT.Diag.IO
{
  internal class SerialPortThread
  {
    SerialPort _port = null;
    ToEcuBuffer _toEcu = null;
    FromEcuBuffer _fromEcu = null;
    Task _writeTask = null;
    byte[] _readBuff = new byte[256];
    byte[] _writeBuff = new byte[256];

    public SerialPortThread(ToEcuBuffer toEcu, FromEcuBuffer fromEcu, SerialPort port)
    {
      _port = port;
      _toEcu = toEcu;
      _fromEcu = fromEcu;

      _port.DataReceived += (object sender, SerialDataReceivedEventArgs e) =>
      {
        try
        {
          int count = _port.BytesToRead;
          while (count != 0)
          {
            count = Math.Min(count, _readBuff.Length);
            _port.Read(_readBuff, 0, count);
            _fromEcu.Write(_readBuff, 0, count);
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
            int count = _toEcu.BytesToRead;
            while (count != 0)
            {
              count = Math.Min(count, _writeBuff.Length);
              _toEcu.Read(_writeBuff, 0, count);
              _port.Write(_writeBuff, 0, count);
              count = _toEcu.BytesToRead;
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
