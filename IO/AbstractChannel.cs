using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DNT.Diag.IO
{
  internal abstract class AbstractChannel<TOptions> : IChannel
    where TOptions : AbstractOptions
  {
    public abstract void Send(byte[] data, int offset, int count);
    public abstract byte[] Recv();
    public abstract void StartHeartbeat(byte[] data, int offset, int count);
    public abstract void StopHeartbeat();
    public abstract void SetByteInterval(Timer tx, Timer rx);
    public abstract void SetFrameInterval(Timer tx, Timer rx);
    public abstract void SetTimeout(Timer time);

    private AbstractFormater<TOptions> _formater;
    private TOptions _opts;

    protected AbstractFormater<TOptions> Formater
    {
      get { return _formater; }
      set { _formater = value; }
    }

    public virtual TOptions Options
    {
      get { return _opts; }
      set { _opts = value; }
    }

    public AbstractChannel()
    {
      _formater = null;
    }

    public void Send(params byte[] data)
    {
      Send(data, 0, data.Length);
    }

    public byte[] SendAndRecv(byte[] sData, int sOffset, int sCount)
    {
      if (Formater != null)
      {
        byte[] temp = Formater.Pack(sData, sOffset, sCount);
        if (temp == null) throw new ChannelException("Formater");
        Send(temp);
      }
      else
      {
        Send(sData, sOffset, sCount);
      }

      var rData = Recv();

      if (Formater != null)
        return Formater.Unpack(rData);

      return rData;
    }

    public void StartHeartbeat(params byte[] data)
    {
      if (data == null)
        throw new ArgumentNullException("data");

      StartHeartbeat(data, 0, data.Length);
    }
  }
}
