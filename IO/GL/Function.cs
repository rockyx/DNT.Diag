using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DNT.Diag.IO.GL
{
  internal class Function<_Const, _Box>
    where _Const : Constant
    where _Box : GL.CommboxImpl<_Const>
  {
    _Box _box;

    public Function(_Box box)
    {
      _box = box;
    }

    public void Send(byte[] data, int offset, int count, bool needRecv)
    {
      try
      {
        _box.BuffId = 0;
        _box.NewBatch();

        if (needRecv)
        {
          _box.SendOutData(data, offset, count);
          _box.RunReceive(_box.Const.RECEIVE);
          _box.EndBatch();
          _box.RunBatch(false);
        }
        else
        {
          _box.SendOutData(data, offset, count);
          _box.EndBatch();
          _box.RunBatch(false);
        }
      }
      catch (CommboxException ex)
      {
        throw new ChannelException();
      }
    }

    public byte[] Heartbeat
    {
      set
      {
        try
        {
          _box.BuffId = Utils.LoByte(_box.Const.LINKBLOCK);
          _box.NewBatch();
          _box.SendOutData(value, 0, value.Length);
          _box.EndBatch();
        }
        catch (CommboxException ex)
        {
          throw new ChannelException();
        }
      }
    }

    public void StartHeartbeat(bool isRun)
    {
      try
      {
        _box.KeepLink(isRun);
      }
      catch (CommboxException)
      {
        throw new ChannelException();
      }
    }
  }
}
