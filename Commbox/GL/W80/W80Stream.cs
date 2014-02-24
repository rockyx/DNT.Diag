using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using DNT.Diag.Buffer;

namespace DNT.Diag.Commbox.GL.W80
{
  internal partial class W80Stream : GLStreamImpl<W80Constant>
  {
    public W80Stream(ToEcuBuffer toEcu, FromEcuBuffer fromEcu)
      : base(toEcu, fromEcu)
    {
      Const = new W80Constant();
      InitPrivate();
    }

    public override ushort BoxVer
    {
      get { return _boxVer; }
    }

    public override void CheckIdle()
    {
      var avail = BytesToRead;
      if (avail > 20)
        Clear();

      byte buffer = Const.READY;
      Timeout = Timer.FromMilliseconds(200);
      while (BytesToRead != 0)
        ReadByte(ref buffer);
      if ((buffer != Const.READY) && (buffer != Const.ERROR))
        throw new StreamException("CheckIdle fail!", Version.W80, buffer);
    }

    public override void CheckResult(Timer time)
    {
      Timeout = time;
      byte rb = 0;
      if (!ReadByte(ref rb))
        throw new StreamException("CheckResult fail!", Version.W80, Const.RECV_ERR);

      if ((rb != Const.READY) && (rb != Const.ERROR))
        throw new StreamException("CheckResult fail!", Version.W80, Const.ERR_CHECK);

      Clear();
    }

    public override int ReadData(byte[] buff, int offset, int length, Timer time)
    {
      Timeout = time;
      var len = Read(buff, offset, length);

      if (len < length)
      {
        var avail = BytesToRead;
        if (avail > 0)
        {
          if (avail <= (length - len))
            len += Read(buff, len + offset, avail);
          else
            len += Read(buff, len + offset, length - len);
        }
      }

      return len;
    }

    public override void SetLineLevel(byte valueLow, byte valueHigh)
    {
      // set port 1
      Port[1] &= Utils.LoByte(~valueLow);
      Port[1] |= valueHigh;
      if (!DoSet(Const.SET_PORT1, Port[1]))
        throw new StreamException("Set commbox line level fail!", Version.W80, Const.ERROR);
    }

    public override void SetCommCtrl(byte valueOpen, byte valueClose)
    {
      // set port 2
      _port[2] &= Utils.LoByte(~valueOpen);
      _port[2] |= valueClose;
      if (!DoSet(Const.SET_PORT2, Port[2]))
        throw new StreamException("Set commbox control fail!", Version.W80, Const.ERROR);
    }

    public override void SetCommLine(byte sendLine, byte recvLine)
    {
      // set port 0
      if (sendLine > 7) sendLine = 0x0F;
      if (recvLine > 7) recvLine = 0x0F;
      Port[0] = Utils.LoByte(sendLine | (recvLine << 4));
      if (!DoSet(Const.SET_PORT0, Port[0]))
        throw new StreamException("Set commbox line fail!", Version.W80, Const.ERROR);
    }

    public override void TurnOverOneByOne()
    {
      // turn receive one send one flag
      if (!DoSet(Const.SET_ONEBYONE))
        throw new StreamException("Commbox turn over one by one fail!", Version.W80, Const.ERROR);
    }

    public override void KeepLink(bool isRunLink)
    {
      if (!DoSet(isRunLink ? Const.RUN_LINK : Const.STOP_LINK))
        throw new StreamException("Commbox keep link fail!", Version.W80, Const.ERROR);
    }

    public override void SetCommLink(byte ctrlWord1, byte ctrlWord2, byte ctrlWord3)
    {
      byte[] ctrlWord = new byte[3];
      byte modeControl = (byte)(ctrlWord1 & 0xE0);
      int length = 3;

      ctrlWord[0] = ctrlWord1;
      if ((ctrlWord1 & 0x04) != 0)
        IsDB20 = true;
      else
        IsDB20 = false;

      if ((modeControl == Const.SET_VPW) || (modeControl == Const.SET_PWM))
      {
        if (!DoSet(Const.SET_CTRL, ctrlWord[0]))
          throw new StreamException("Commbox set link fail!", Version.W80, Const.ERROR);
        return;
      }

      ctrlWord[1] = ctrlWord2;
      ctrlWord[2] = ctrlWord3;

      if (ctrlWord3 == 0)
      {
        length--;
        if (ctrlWord2 == 0)
          length--;
      }

      if (((modeControl == Const.EXRS_232) && (length < 2)) ||
        !DoSet(Const.SET_CTRL, ctrlWord, 0, length))
        throw new StreamException("Commbox set link fail!", Version.W80, Const.ERROR);
    }

    public override void SetCommBaud(double baud)
    {
      byte[] baudTime = new byte[2];
      double instructNum = 1000000000000.0 / (TimeUnit * baud);
      if (IsDB20)
        instructNum /= 20;
      instructNum += 0.5;

      if ((instructNum > 65535) || (instructNum < 10))
        throw new StreamException("Commbox set baudrate fail!", Version.W80, Const.ERROR);

      baudTime[0] = Utils.HiByte(instructNum);
      baudTime[1] = Utils.LoByte(instructNum);

      bool result = false;
      if (baudTime[0] == 0)
        result = DoSet(Const.SET_BAUD, baudTime[1]);
      else 
        result = DoSet(Const.SET_BAUD, baudTime, 0, 2);
      if (!result)
        throw new StreamException("Commbox set baudrate fail!", Version.W80, Const.ERROR);
    }

    public override void SetCommTime(byte type, Timer time)
    {
      byte[] timeBuff = new byte[2];
      GetLinkTime(type, time);
      double microTime = time.Microseconds;

      if ((type == Const.SETVPWSTART) || (type == Const.SETVPWRECS))
      {
        if (type == Const.SETVPWRECS)
          microTime = (microTime * 2) / 3;

        type = Utils.LoByte(type + (Const.SETBYTETIME & 0xF0));
        microTime = (microTime * 1000000.0) / TimeUnit;
      }
      else
      {
        microTime = (microTime * 1000000.0) / (TimeBaseDB * TimeUnit);
      }

      timeBuff[0] = Utils.HiByte(microTime);
      timeBuff[1] = Utils.LoByte(microTime);

      bool result = false;
      if (timeBuff[0] == 0)
        result = DoSet(type, timeBuff[1]);
      else 
        result = DoSet(type, timeBuff, 0, 2);
      if (!result)
        throw new StreamException("Commbox set time fail!", Version.W80, Const.ERROR);
    }

    public override void CommboxDelay(Timer time)
    {
      byte[] timeBuff = new byte[2];
      byte delayWord = Const.DELAYSHORT;

      double microTime = ((double)time.Microseconds) / (TimeUnit / 1000000.0);

      if (microTime == 0)
        throw new ArgumentException("time");

      if (microTime > 65535)
      {
        microTime = microTime / TimeBaseDB;
        if (microTime > 65535)
        {
          microTime = (microTime * TimeBaseDB) / TimeExternDB;
          if (microTime > 65535)
            throw new ArgumentException("time");
          delayWord = Const.DELAYDWORD;
        }
        else
        {
          delayWord = Const.DELAYTIME;
        }
      }

      timeBuff[0] = Utils.HiByte(microTime);
      timeBuff[1] = Utils.LoByte(microTime);

      bool result = false;
      if (timeBuff[0] == 0)
        result = DoSet(delayWord, timeBuff[1]);
      else
        result = DoSet(delayWord, timeBuff, 0, 2);

      if (!result)
        throw new StreamException("Commbox delay fail!", Version.W80, Const.ERROR);
    }

    public override void SendOutData(byte[] buff, int offset, int count)
    {
      if (!DoSet(Const.SEND_DATA, buff, offset, count))
        throw new StreamException("Commbox send data fail!", Version.W80, Const.ERROR);
    }

    public override void RunReceive(byte type)
    {
      if (type == Const.GET_PORT1) IsDB20 = false;
      if (!DoCmd(type))
        throw new StreamException("Commbox receive fail!", Version.W80, Const.RECV_ERR);
    }

    public override void StopNow(bool isStop)
    {
      byte cmd = isStop ? Const.STOP_EXECUTE : Const.STOP_REC;

      for (int i = 0; i < 3; i++)
      {
        Write(cmd);
        if (CheckSend())
        {
          if (isStop)
          {
            try
            {
              CheckResult(Timer.FromMilliseconds(200));
            }
            catch (StreamException)
            {
              continue;
            }
          }
          return;
        }
      }
      throw new StreamException("Commbox stop execute fail!", Version.W80, Const.ERROR);
    }

    public override void Connect()
    {
      //LastError = Const.DISCONNECT_COMM;
      if (!InitBox() || !CheckBox())
      {
        throw new StreamException("Open Commbox Fail!", Version.W80, Const.DISCONNECT_COMM);
      }
      Clear();
    }

    public override void Disconnect()
    {
      Reset();
    }

    public override void NewBatch()
    {
      _pos = 0;
      _isLink = BuffId == Const.LINKBLOCK ? true : false;
      IsDoNow = false;
    }

    public override void EndBatch()
    {
      int i = 0;
      IsDoNow = true;
      Buff[Pos++] = 0; // command block end with 0x00
      if (IsLink)
      {
        // modify updateBuff will used address
        while (Buff[i] != 0)
        {
          byte mode = Utils.LoByte(Buff[i] & 0xFC);
          if ((mode == Const.UPDATE_BYTE)|| 
            (mode == Const.INVERT_BYTE) || 
            (mode == Const.ADD_BYTE) || 
            (mode == Const.DEC_BYTE) || 
            (mode == Const.INC_BYTE))
          {
            Buff[i + 1] += Utils.LoByte(Const.MAXBUFF_LEN - Pos);
          }
          else if (mode == Const.SUB_BYTE)
          {
            Buff[i + 2] += Utils.LoByte(Const.MAXBUFF_LEN - Pos);
            Buff[i + 1] += Utils.LoByte(Const.MAXBUFF_LEN - Pos);
          }
          else if (mode == Const.COPY_BYTE)
          {
            Buff[i + 3] += Utils.LoByte(Const.MAXBUFF_LEN - Pos);
            Buff[i + 2] += Utils.LoByte(Const.MAXBUFF_LEN - Pos);
            Buff[i + 1] += Utils.LoByte(Const.MAXBUFF_LEN - Pos);
          }

          if (Buff[i] == Const.SEND_DATA)
            i += 1 + (Buff[i + 1] + 1) + 1;
          else if ((Buff[i] >= Const.REC_LEN_1) &&
            (Buff[i] <= Const.REC_LEN_15))
            i += 1; // special
          else
            i += (Buff[i] & 0x03) + 1;
        }
      }

      if (!DoCmd(Const.WR_DATA, Buff, 0, Pos))
        throw new StreamException("Commbox end batch fail!", Version.W80, Const.ERROR);
    }

    public override void DelBatch()
    {
      IsDoNow = true;
      Pos = 0;
    }

    public override void RunBatch(bool repeat)
    {
      byte cmd;
      if (BuffId == Const.LINKBLOCK)
        cmd = repeat ? Const.DO_BAT_LN : Const.DO_BAT_L;
      else
        cmd = repeat ? Const.DO_BAT_CN : Const.DO_BAT_C;
      if (!DoCmd(cmd))
        throw new StreamException("Commbox run batch fail!", Version.W80, Const.ERROR);
    }
  }
}
