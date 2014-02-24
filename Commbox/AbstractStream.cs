using System;
using System.Text;
using System.Diagnostics;

using DNT.Diag.Buffer;

namespace DNT.Diag.Commbox
{
  internal abstract class AbstractStream : IStream
  {
    ToEcuBuffer _toEcuBuffer;
    FromEcuBuffer _fromEcuBuffer;
    Timer _timeout;

    public abstract void Connect();
    public abstract void Disconnect();

    void ShowData(byte[] buff, int offset, int count, string tag)
    {
      count += offset;
      if (count > buff.Length)
        throw new IndexOutOfRangeException("buff");

      StringBuilder sb = new StringBuilder(100);

      sb.Append(DateTime.Now.ToString("HH:mm::ss.fff"));
      sb.AppendFormat(" {0} : ", tag);

      for (int i = offset; i < count; i++)
        sb.AppendFormat("{0:X2}", buff[i]);

      Trace.WriteLine(sb.ToString());
    }

    public AbstractStream(ToEcuBuffer toEcu, FromEcuBuffer fromEcu)
    {
      _toEcuBuffer = toEcu;
      _fromEcuBuffer = fromEcu;
    }

    protected Timer Timeout
    {
      get { return _timeout; }
      set { _timeout = value; }
    }

    protected void Write(params byte[] buff)
    {
      Write(buff, 0, buff.Length);
    }

    protected void Write(byte[] buff, int offset, int count)
    {
      ShowData(buff, offset, count, "Send");
      _toEcuBuffer.Write(buff, offset, count);
    }

    protected int Read(byte[] buff, int offset, int count)
    {
      _fromEcuBuffer.Timeout = Timeout;
      var ret = _fromEcuBuffer.Read(buff, offset, count);
      ShowData(buff, offset, ret, "Recv");
      return ret;
    }

    protected bool ReadByte(ref byte b)
    {
      _fromEcuBuffer.Timeout = Timeout;

      byte[] buff = new byte[1];
      var ret = _fromEcuBuffer.Read(buff, 0, 1);

      ShowData(buff, 0, ret, "Recv");

      if (ret != 1) return false;

      b = buff[0];
      return true;
    }

    protected int BytesToRead
    {
      get { return _fromEcuBuffer.BytesToRead; }
    }

    protected void Clear()
    {
      _fromEcuBuffer.Clear();
      _toEcuBuffer.Clear();
    }
  }
}
