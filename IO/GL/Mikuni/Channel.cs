using System;
using System.Threading;

namespace DNT.Diag.IO.GL.Mikuni
{
  internal class Channel<_Const, _Box> : IO.Mikuni.Channel, IChannel
    where _Const : Constant
    where _Box : CommboxImpl<_Const>
  {
    _Box _box;
    Function<_Const, _Box> _func;

    public Channel(_Box box)
    {
      _box = box;
      _func = new Function<_Const, _Box>(box);
    }

    public override void Send(byte[] data, int offset, int count)
    {
      _func.Send(data, offset, count, true);
    }

    public override byte[] Recv()
    {
      List<byte> ret = new List<byte>();
      byte[] b = new byte[1];
      byte before = 0;

      while (_box.ReadBytes(b, 0, 1) == 1)
      {
        ret.Add(b[0]);
        if (before == 0x0D && b[0] == 0x0A)
          break;
        before = b[0];
      }

      FinishExecute();
      return ret.ToArray();
    }

    public override void StartHeartbeat(byte[] data, int offset, int count)
    {
      _func.Heartbeat = Formater.Pack(data, offset, count);
      _func.StartHeartbeat(true);
    }

    public override void StopHeartbeat()
    {
      _func.StartHeartbeat(false);
    }

    public override void SetByteInterval(Timer tx, Timer rx)
    {
      throw new NotImplementedException();
    }

    public override void SetFrameInterval(Timer tx, Timer rx)
    {
      throw new NotImplementedException();
    }

    public override void SetTimeout(Timer time)
    {
      throw new NotImplementedException();
    }

    public void FinishExecute()
    {
      try
      {
        _box.StopNow(true);
        _box.DelBatch();
      }
      catch (CommboxException)
      {
        throw new ChannelException();
      }
    }

    public override IO.Mikuni.Options Options
    {
      get
      {
        return base.Options;
      }
      set
      {
        base.Options = value;

        try
        {
          byte parity = 0;
          byte cmd2 = _box.Const.SET_NULL;
          byte cmd3 = _box.Const.SET_NULL;

          if (Options.Parity == IO.Mikuni.Parity.None)
          {
            parity = _box.Const.BIT9_MARK;
            cmd2 = 0xFF;
            cmd3 = 0x02;
          }
          else
          {
            parity = _box.Const.BIT9_EVEN;
            cmd2 = 0xFF;
            cmd3 = 0x03;
          }

          _box.SetCommCtrl(_box.Const.PWC | _box.Const.RZFC | _box.Const.CK | _box.Const.REFC, _box.Const.SET_NULL);
          _box.SetCommLine(_box.Const.SK_NO, _box.Const.RK1);
          _box.SetCommLink(_box.Const.RS_232 | parity | _box.Const.SEL_SL | _box.Const.UN_DB20, cmd2, cmd3);
          _box.SetCommBaud(19200);
          _box.SetCommTime(_box.Const.SETBYTETIME, Timer.FromMilliseconds(5));
          _box.SetCommTime(_box.Const.SETWAITTIME, Timer.FromMilliseconds(0));
          _box.SetCommTime(_box.Const.SETRECBBOUT, Timer.FromMilliseconds(500));
          _box.SetCommTime(_box.Const.SETRECFROUT, Timer.FromMilliseconds(500));
          _box.SetCommTime(_box.Const.SETLINKTIME, Timer.FromMilliseconds(500));


          Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        catch (CommboxException)
        {
          throw new ChannelException();
        }
      }
    }
  }
}
