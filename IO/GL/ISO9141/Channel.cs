using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DNT.Diag.IO.GL.ISO9141
{
  internal class Channel<_Const, _Box> : IO.ISO9141.Channel, IChannel
    where _Const : Constant
    where _Box : CommboxImpl<_Const>
  {
    byte _kLine;
    byte _lLine;
    _Box _box;
    Function<_Const, _Box> _func;

    bool ConfigLines()
    {
      if (Options.ComLine == 7)
      {
        _lLine = _box.Const.SK1;
        _kLine = _box.Const.RK1;
      }
      else
      {
        return false;
      }
      return true;
    }

    public Channel(_Box box)
    {
      _box = box;
      _kLine = _box.Const.SK_NO;
      _lLine = _box.Const.RK_NO;
      _func = new Function<_Const, _Box>(box);
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

    public override void Send(byte[] data, int offset, int count)
    {
      _func.Send(data, offset, count, true);
    }

    public override byte[] Recv()
    {
      List<byte> data = new List<byte>();

      byte[] b = new byte[1];
      while (true)
      {
        if (_box.ReadBytes(b, 0, 1) != 1)
          break;
        data.Add(b[0]);
      }

      if (data.Count < 5)
      {
        FinishExecute();
        throw new ChannelException();
      }

      return data.ToArray();
    }

    public override void StartHeartbeat(byte[] data, int offset, int count)
    {
      byte[] buff = null;
      if (offset == 0)
      {
        buff = data;
      }
      else
      {
        buff = new byte[count];
        Array.Copy(data, offset, buff, 0, count);
      }

      _func.Heartbeat = buff;
      _func.StartHeartbeat(true);
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

    public override IO.ISO9141.Options Options
    {
      get
      {
        return base.Options;
      }
      set
      {
        base.Options = value;
        if (!ConfigLines())
          throw new ChannelException();

        try
        {
          _box.SetCommCtrl(_box.Const.PWC | _box.Const.RZFC | _box.Const.CK, _box.Const.SET_NULL);
          _box.SetCommLine(_lLine, _kLine);
          _box.SetCommLink(_box.Const.RS_232 | _box.Const.BIT9_MARK | _box.Const.SEL_SL | _box.Const.SET_DB20, _box.Const.SET_NULL, _box.Const.INVERTBYTE);
          _box.SetCommBaud(5);
          _box.SetCommTime(_box.Const.SETBYTETIME, Timer.FromMilliseconds(5));
          _box.SetCommTime(_box.Const.SETWAITTIME, Timer.FromMilliseconds(25));
          _box.SetCommTime(_box.Const.SETRECBBOUT, Timer.FromMilliseconds(400));
          _box.SetCommTime(_box.Const.SETRECFROUT, Timer.FromMilliseconds(500));
          _box.SetCommTime(_box.Const.SETLINKTIME, Timer.FromMilliseconds(500));

          Thread.Sleep(TimeSpan.FromSeconds(1));
          _box.BuffId = 0;
          _box.NewBatch();

          var addrCode = Options.AddrCode;
          _box.SendOutData(addrCode);
          _box.SetCommLine(_kLine == _box.Const.RK_NO ? _lLine : _box.Const.SK_NO, _kLine);
          _box.RunReceive(_box.Const.SET55_BAUD);
          _box.RunReceive(_box.Const.REC_LEN_1);
          _box.TurnOverOneByOne();
          _box.RunReceive(_box.Const.REC_LEN_1);
          _box.TurnOverOneByOne();
          _box.RunReceive(_box.Const.REC_LEN_1);
          _box.EndBatch();

          int tempLen = 0;
          byte[] tempBuff = new byte[3];

          _box.RunBatch(false);
          tempLen = _box.ReadData(tempBuff, 0, 3, Timer.FromMilliseconds(3));
          if (tempLen != 3)
            throw new ChannelException();

          _box.CheckResult(Timer.FromMilliseconds(500));
          _box.DelBatch();

          _box.SetCommTime(_box.Const.SETBYTETIME, Timer.FromMilliseconds(5));
          _box.SetCommTime(_box.Const.SETWAITTIME, Timer.FromMilliseconds(15));
          _box.SetCommTime(_box.Const.SETRECBBOUT, Timer.FromMilliseconds(80));
          _box.SetCommTime(_box.Const.SETRECFROUT, Timer.FromMilliseconds(200));
        }
        catch (CommboxException ex)
        {
          try
          {
            _box.DelBatch();
          }
          catch
          {
          }
          throw new ChannelException();
        }
      }
    }

    public override void StopHeartbeat()
    {
      _func.StartHeartbeat(false);
    }
  }
}
