using System;
using System.Threading;

namespace DNT.Diag.IO.GL.ISO14230
{
  internal class Channel<_Const, _Box> : IO.ISO14230.Channel, IChannel
    where _Const : Constant
    where _Box : CommboxImpl<_Const>
  {

    _Box _box;
    Function<_Const, _Box> _func;
    byte _kLine;
    byte _lLine;

    public Channel(_Box box)
    {
      _box = box;
      _func = new Function<_Const, _Box>(box);
      _kLine = _box.Const.SK_NO;
      _lLine = _box.Const.RK_NO;

      StartCommunicationInit();
    }

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

    void StartCommunicationInit()
    {
      StartComms[IO.ISO14230.InitType.Fast] = () =>
      {
        try
        {
          int valueOpen = 0;
          if (Options.LLine)
            valueOpen = _box.Const.PWC | _box.Const.RZFC | _box.Const.CK;
          else
            valueOpen = _box.Const.PWC | _box.Const.RZFC | _box.Const.CK;

          _box.BuffId = 0xFF;

          _box.SetCommCtrl(valueOpen, _box.Const.SET_NULL);
          _box.SetCommLine(_lLine, _kLine);
          _box.SetCommLink(_box.Const.RS_232 | _box.Const.BIT9_MARK | _box.Const.SEL_SL | _box.Const.UN_DB20, _box.Const.SET_NULL, _box.Const.SET_NULL);
          _box.SetCommBaud(Options.BaudRate);
          _box.SetCommTime(_box.Const.SETBYTETIME, Timer.FromMilliseconds(5));
          _box.SetCommTime(_box.Const.SETWAITTIME, Timer.FromMilliseconds(0));
          _box.SetCommTime(_box.Const.SETRECBBOUT, Timer.FromMilliseconds(400));
          _box.SetCommTime(_box.Const.SETRECFROUT, Timer.FromMilliseconds(500));
          _box.SetCommTime(_box.Const.SETLINKTIME, Timer.FromMilliseconds(500));

          Thread.Sleep(1);

          _box.BuffId = 0;

          _box.NewBatch();

          var fastCmd = Formater.Pack(Options.FastCmd);

          _box.SetLineLevel(_box.Const.COMS, _box.Const.SET_NULL);
          _box.CommboxDelay(Timer.FromMilliseconds(25));
          _box.SetLineLevel(_box.Const.SET_NULL, _box.Const.COMS);
          _box.CommboxDelay(Timer.FromMilliseconds(25));
          _box.SendOutData(fastCmd);
          _box.RunReceive(_box.Const.REC_FR);
          _box.EndBatch();
          _box.RunBatch(false);

          var buff = ReadOneFrame();
          if (buff == null)
            throw new ChannelException();

          FinishExecute();
          _box.SetCommTime(_box.Const.SETWAITTIME, Timer.FromMilliseconds(55));
          _box.DelBatch();
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
      };

      StartComms[IO.ISO14230.InitType.Addr] = () =>
      {
        try
        {
          _box.SetCommCtrl(_box.Const.PWC | _box.Const.REFC | _box.Const.RZFC | _box.Const.CK, _box.Const.SET_NULL);
          _box.SetCommBaud(5);
          _box.SetCommTime(_box.Const.SETBYTETIME, Timer.FromMilliseconds(5));
          _box.SetCommTime(_box.Const.SETWAITTIME, Timer.FromMilliseconds(12));
          _box.SetCommTime(_box.Const.SETRECBBOUT, Timer.FromMilliseconds(400));
          _box.SetCommTime(_box.Const.SETRECFROUT, Timer.FromMilliseconds(500));
          _box.SetCommTime(_box.Const.SETLINKTIME, Timer.FromMilliseconds(500));

          Thread.Sleep(TimeSpan.FromSeconds(1));

          _box.BuffId = 0;

          _box.NewBatch();

          _box.SendOutData(Options.AddrCode);
          _box.SetCommLine(_kLine == _box.Const.RK_NO ? _lLine : _box.Const.SK_NO, _kLine);
          _box.RunReceive(_box.Const.SET55_BAUD);
          _box.RunReceive(_box.Const.REC_LEN_1);
          _box.TurnOverOneByOne();
          _box.RunReceive(_box.Const.REC_LEN_1);
          _box.TurnOverOneByOne();
          _box.RunReceive(_box.Const.REC_LEN_1);
          _box.EndBatch();

          byte[] temp = new byte[3];

          _box.RunBatch(false);
          if (_box.ReadData(temp, 0, temp.Length, Timer.FromSeconds(5)) <= 0)
            throw new ChannelException();

          _box.CheckResult(Timer.FromSeconds(5));

          _box.DelBatch();
          _box.SetCommTime(_box.Const.SETWAITTIME, Timer.FromMilliseconds(55));

          if (temp[2] != 0) throw new ChannelException();
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
      };
    }

    int ReadMode80(byte[] buff)
    {
      byte[] len = new byte[1];
      if (_box.ReadBytes(len, 0, 1) != 1)
        return -1;

      buff[3] = len[0];

      return _box.ReadBytes(buff, 4, len[0] + 1) + 4;
    }

    int ReadMode8XCX(byte[] buff)
    {
      int length = (buff[0] & 0xC0) == 0xC0 ? buff[0] - 0xC0 : buff[0] - 0x80;
      return _box.ReadBytes(buff, 3, length + 1) + 3;
    }

    int ReadMode00(byte[] buff)
    {
      return _box.ReadBytes(buff, 3, buff[1]) + 3;
    }

    int ReadModeXX(byte[] buff)
    {
      return _box.ReadBytes(buff, 3, buff[0] - 1) + 3;
    }

    byte[] ReadOneFrame()
    {
      byte[] buff = new byte[256];
      int len = _box.ReadBytes(buff, 0, 3);

      if (len != 3)
        return null;

      if (buff[1] == Options.SourceAddres)
      {
        if (buff[0] == 0x80)
          len = ReadMode80(buff);
        else
          len = ReadMode8XCX(buff);
      }
      else
      {
        if (buff[0] == 0x00)
          len = ReadMode00(buff);
        else
          len = ReadModeXX(buff);
      }

      byte[] ret = new byte[len];
      Array.Copy(buff, ret, len);
      return ret;
    }

    public override void Send(byte[] data, int offset, int count)
    {
      _func.Send(data, offset, count, true);
    }

    public override byte[] Recv()
    {
      return ReadOneFrame();
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

    public override IO.ISO14230.Options Options
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
        StartComms[Options.InitType]();
      }
    }
  }
}
