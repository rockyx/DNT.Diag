using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

using DNT.Diag.Buffer;

namespace DNT.Diag.Commbox.GL.C168
{
  internal partial class C168Stream : GLStreamImpl<C168Constant>
  {
    public C168Stream(ToEcuBuffer toEcu, FromEcuBuffer fromEcu)
      : base(toEcu, fromEcu)
    {
      Const = new C168Constant();
      InitPrivate();
    }

    public override void CheckIdle()
    {
      var avail = BytesToRead;
      if (avail > 240)
      {
        Clear();
      }
      else
      {
        Timeout = Timer.FromMilliseconds(200);
        byte receiveBuffer = Const.SUCCESS;
        while (BytesToRead != 0)
          ReadByte(ref receiveBuffer);

        if (receiveBuffer != Const.SUCCESS)
          throw new StreamException("Commbox check idle fail!", Commbox.Version.C168, Const.KEEPLINK_ERROR);
      }
    }

    public override ushort BoxVer
    {
      get
      {
        return Utils.LoWord((_version[0] << 8) | _version[1]);
      }
    }

    public override void CheckResult(Timer timer)
    {
      Timeout = timer;
      byte receiveBuffer = 0;
      LastError = Const.SUCCESS;

      if (!ReadByte(ref receiveBuffer))
      {
        LastError = Const.TIMEOUT_ERROR;
      }
      else
      {
        if (receiveBuffer != Const.SUCCESS)
        {
          while (ReadByte(ref receiveBuffer)) ;
          LastError = receiveBuffer;
        }
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox check result fail!", Commbox.Version.C168, LastError);
    }

    public override void StopNow(bool stopExecute)
    {
      LastError = Const.SUCCESS;
      if (stopExecute)
      {
        byte receiveBuffer = 0;
        int times = Const.REPLAYTIMES;
        while (times-- != 0)
        {
          LastError = CommboxDo(Const.STOP_EXECUTE);
          if (LastError != Const.SUCCESS)
            break;
          Timeout = Timer.FromMilliseconds(600);
          if (!ReadByte(ref receiveBuffer) || 
            receiveBuffer != Const.RUN_ERR)
          {
            LastError = Const.TIMEOUT_ERROR;
            continue;
          }
          break;
        }
      }
      else
      {
        LastError = CommboxDo(Const.STOP_REC);
      }
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox StopNow fail!", Commbox.Version.C168, LastError);
    }

    public override int ReadData(byte[] buff, int offset, int count, Timer time)
    {
      Timeout = time;
      return Read(buff, offset, count);
    }

    public override void NewBatch()
    {
      LastError = Const.SUCCESS;
      if (BuffId > Const.MAXIM_BLOCK)
      {
        LastError = Const.NODEFINE_BUFF;
      }
      else if (CmdBuffId != Const.NULLADD)
      {
        LastError = Const.APPLICATION_NOW;
      }
      else if (CmdBuffAdd[BuffId] != Const.NULLADD )
      {
        try
        {
          if (BuffId != Const.LINKBLOCK)
            DelBatch();
        }
        catch (StreamException)
        {
          LastError = Const.NOADDDATA;
        }
      }

      // Ok precheck finished.
      if (LastError == Const.SUCCESS)
      {
        CmdBuffData.Clear();
        CmdBuffHeader = Utils.LoByte(Const.WR_DATA + HeadPassword);

        if (BuffId == Const.LINKBLOCK)
        {
          CmdBuffData.Add(0xFF);
          CmdBuffAdd[Const.LINKBLOCK] = CmdBuffLen;
        }
        else
        {
          CmdBuffData.Add(CmdBuffAdd[Const.SWAPBLOCK]);
        }
        CmdBuffChecksum = Utils.LoByte(Const.WR_DATA + CmdBuffData.Count + CmdBuffData.First());

        CmdBuffId = BuffId;
        IsDoNow = false;
      }
      else
      {
        throw new StreamException("Commbox new batch fail!", Commbox.Version.C168, LastError);
      }
    }

    public override void EndBatch()
    {
      LastError = Const.SUCCESS;
      if (CmdBuffId == Const.NULLADD)
      {
        // block is used?
        LastError = Const.NOAPPLICATBUFF;
      }
      else if (CmdBuffData.Count == 0x01)
      {
        LastError = Const.NOADDDATA;
      }
      else
      {
        // Every thing is ok, we will send command to commbox
        IsDoNow = true;

        List<byte> cmd = new List<byte>();

        cmd.Add(CmdBuffHeader);
        cmd.Add(Utils.LoByte(CmdBuffData.Count));
        cmd.AddRange(CmdBuffData);
        cmd.Add(CmdBuffChecksum);

        byte[] sendCmd = cmd.ToArray(); // pack header, length, data and checksum to one byte array

        int times = Const.REPLAYTIMES;
        while (times != 0)
        {
          try
          {
            // there only one way to set result to true.
            CheckIdle();
            Write(sendCmd);
            LastError = SendOk(Timer.FromMilliseconds(20 * (sendCmd.Length + 10)));
            if (LastError == Const.SUCCESS)
              break;
            StopNow(true); // If code reach here, the result still equal false.
          }
          catch (StreamException ex)
          {
            // if CheckIdle() and SendOk() fail, just subtract times.
            // if StopNow() fail, we terminate.
            if (ex.Message.Contains("StopNow"))
            {
              break;
            }
          }
          --times;
        }

        if (LastError == Const.SUCCESS)
        {
          // Write command to commbox success, update our status.
          if (CmdBuffId == Const.LINKBLOCK)
          {
            CmdBuffAdd[Const.LINKBLOCK] = Utils.LoByte(CmdBuffLen - CmdBuffData.Count);
          }
          else
          {
            CmdBuffAdd[CmdBuffId] = CmdBuffAdd[Const.SWAPBLOCK];
            CmdBuffUsed[CmdUsedNum++] = CmdBuffId;
            CmdBuffAdd[Const.SWAPBLOCK] += Utils.LoByte(CmdBuffData.Count);
          }
        }
      }
      // what ever success or fail, we make CmdBuffId to NULLADD.
      CmdBuffId = Const.NULLADD;

      // fail, throw exception.
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox end batch fail!", Commbox.Version.C168, LastError);
    }

    public override void DelBatch()
    {
      LastError = Const.SUCCESS;

      if (BuffId > Const.LINKBLOCK)
      {
        // block not exist
        LastError = Const.NODEFINE_BUFF;
      }
      else if (CmdBuffId == BuffId)
      {
        CmdBuffId = Const.NULLADD;
        return;
      }
      else if (CmdBuffAdd[BuffId] == Const.NULLADD)
      {
        // block used?
        LastError = Const.NOUSED_BUFF;
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox delete batch fail!", Commbox.Version.C168, LastError);

      if (BuffId == Const.LINKBLOCK)
      {
        CmdBuffAdd[Const.LINKBLOCK] = CmdBuffLen;
      }
      else
      {
        int i;
        for (i = 0; i < CmdUsedNum; ++i)
        {
          if (CmdBuffUsed[i] == BuffId)
            break;
        }

        byte[] data = new byte[3];
        data[0] = CmdBuffAdd[BuffId];
        if (i < (CmdUsedNum - 1))
        {
          data[1] = CmdBuffAdd[CmdBuffUsed[i + 1]];
          data[2] = Utils.LoByte(CmdBuffAdd[Const.SWAPBLOCK] - data[1]);
          LastError = DoSet(Utils.LoByte(Const.COPY_DATA - Const.COPY_DATA % 4), data);
        }
        else
        {
          data[1] = CmdBuffAdd[Const.SWAPBLOCK];
        }

        if (LastError == Const.SUCCESS)
        {
          byte deleteBuffLen = Utils.LoByte(data[1] - data[0]);
          for (i = i + 1; i < CmdUsedNum; ++i)
          {
            CmdBuffUsed[i - 1] = CmdBuffUsed[i];
            CmdBuffAdd[CmdBuffUsed[i]] -= deleteBuffLen;
          }
          CmdUsedNum--;
          CmdBuffAdd[Const.SWAPBLOCK] -= deleteBuffLen;
          CmdBuffAdd[BuffId] = Const.NULLADD;
        }
        else
        {
          throw new StreamException("Commbox delete batch fail!", Commbox.Version.C168, LastError);
        }
      }
    }

    public override void SetLineLevel(byte valueLow, byte valueHigh)
    {
      // only one byte data, set port 1
      CommboxPort[1] &= Utils.LoByte(~valueLow);
      CommboxPort[1] |= valueHigh;
      LastError = SendCmdToBox(Const.SETPORT1, CommboxPort[1]);
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox set line level fail!", Commbox.Version.C168, LastError);
    }

    public override void SetCommCtrl(byte valueOpen, byte valueClose)
    {
      // only one byte data, set port 2
      CommboxPort[2] &= Utils.LoByte(~valueOpen);
      CommboxPort[2] |= valueClose;
      LastError = SendCmdToBox(Const.SETPORT2, CommboxPort[2]);
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox set control fail!", Commbox.Version.C168, LastError);
    }

    public override void SetCommLine(byte sendLine, byte recvLine)
    {
      // only one byte data, set port 0
      if (sendLine > 7) sendLine = 0x0F;
      if (recvLine > 7) recvLine = 0x0F;
      CommboxPort[0] = Utils.LoByte((sendLine & 0x0F) | ((recvLine << 4) & 0xF0));
      LastError = SendCmdToBox(Const.SETPORT0, CommboxPort[0]);
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox set line fail!", Commbox.Version.C168, LastError);
    }

    public override void TurnOverOneByOne()
    {
      // turn receive one send one flag.
      LastError = SendCmdToBox(Const.SET_ONEBYONE);
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox turn over one by one fail!", Commbox.Version.C168, LastError);
    }

    public override void KeepLink(bool isRunLink)
    {
      if (isRunLink) 
        LastError = SendCmdToBox(Const.RUNLINK);
      else
        LastError = SendCmdToBox(Const.STOPLINK);

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox keep link fail!", Commbox.Version.C168, LastError);
    }

    public override void SetCommLink(byte ctrlWord1, byte ctrlWord2, byte ctrlWord3)
    {
      LastError = Const.SUCCESS;
      byte[] ctrlWord = new byte[3]; // control word 3
      byte modeControl = Utils.LoByte(ctrlWord1 & 0xE0);

      int length = 3;
      ctrlWord[0] = ctrlWord1;

      if ((ctrlWord1 & 0x04) != 0)
        IsDB20 = true;
      else
        IsDB20 = false;

      if (modeControl == Const.SET_VPW || modeControl == Const.SET_PWM)
      {
        LastError = SendCmdToBox(Const.SETTING, ctrlWord[0]);
        if (LastError != Const.SUCCESS)
          throw new StreamException("Commbox set link fail!", Commbox.Version.C168, LastError);
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

      if (modeControl == Const.EXRS_232 && length < 2)
      {
        LastError = Const.UNSET_EXRSBIT;
      }

      if (LastError == Const.SUCCESS)
        LastError = SendCmdToBox(Const.SETTING, ctrlWord, 0, length);

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox set link fail!", Commbox.Version.C168, LastError);
    }

    public override void SetCommBaud(double baud)
    {
      LastError = Const.SUCCESS;
      byte[] baudTime = new byte[2];
      double instructNum = ((1000000.0 / TimeUnit) * 1000000) / baud;

      if (IsDB20) instructNum /= 20;
      instructNum += 0.5;

      if (instructNum > 65535 || instructNum < 10)
      {
        LastError = Const.COMMBAUD_OUT;
      }

      if (LastError == Const.SUCCESS)
      {
        baudTime[0] = Utils.HiByte(instructNum);
        baudTime[1] = Utils.LoByte(instructNum);

        if (baudTime[0] == 0)
          LastError = SendCmdToBox(Const.SETBAUD, baudTime[1]);
        else
          LastError = SendCmdToBox(Const.SETBAUD, baudTime, 0, 2);
      }
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox set baudrate fail!", Commbox.Version.C168, LastError);
    }

    public override void SetCommTime(byte type, Timer time)
    {
      LastError = Const.SUCCESS;
      byte[] timeBuff = new byte[2];
      GetLinkTime(type, time);

      long microTime = time.Microseconds;

      if (type == Const.SETVPWRECS || type == Const.SETVPWSTART)
      {
        if (Const.SETVPWRECS == type)
          microTime = (microTime * 2) / 3;
        type = Utils.LoByte(type + (Const.SETBYTETIME & 0xF0));
        microTime = (long)((microTime * 1000000.0) / TimeUnit);
      }
      else
      {
        microTime = (long)((microTime * 1000000.0) / (TimeBaseDB * TimeUnit));
      }

      if (microTime > 65535)
      {
        LastError = Const.COMMTIME_OUT;
      }

      if (LastError == Const.SUCCESS)
      {
        if (type == Const.SETBYTETIME ||
          type == Const.SETWAITTIME ||
          type == Const.SETRECBBOUT ||
          type == Const.SETRECFROUT ||
          type == Const.SETLINKTIME)
        {
          timeBuff[0] = Utils.HiByte(microTime);
          timeBuff[1] = Utils.LoByte(microTime);

          if (timeBuff[0] == 0)
            LastError = SendCmdToBox(type, timeBuff[1]);
          else
            LastError = SendCmdToBox(type, timeBuff, 0, 2);
        }
        else
        {
          LastError = Const.UNDEFINE_CMD;
        }
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox set time fail!", Commbox.Version.C168, LastError);
    }

    public override void RunReceive(byte type)
    {
      LastError = Const.SUCCESS;
      if (type == Const.GET_PORT1) IsDB20 = false;
      if (type == Const.GET_PORT1 ||
        type == Const.SET55_BAUD ||
        (type >= Const.REC_FR && type <= Const.RECEIVE))
      {
        if (IsDoNow)
          LastError = CommboxDo(type);
        else
          LastError = AddToBuff(type);
      }
      else
      {
        LastError = Const.UNDEFINE_CMD;
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox run receive fail!", Commbox.Version.C168, LastError);
    }

    public void UpdateBuff(byte type, byte[] buffer)
    {
      LastError = Const.SUCCESS;
      byte[] temp = new byte[4];
      LastError = 0;
      temp[0] = buffer[1];

      LastError = GetAbsAdd(buffer[1], temp[1]);

      if (LastError == Const.SUCCESS)
      {
        // add 
        if ((type == Const.INVERT_DATA) || 
          (type == Const.DEC_DATA) ||
          (type == Const.INC_DATA))
        {
        }
        // add + data
        else if ((type == Const.UPDATE_1BYTE) ||
          (type == Const.SUB_BYTE))
        {
          temp[1] = buffer[2];
        }
        // add + add
        else if ((type == Const.INC_2DATA))
        {
          temp[1] = buffer[3];
          LastError = GetAbsAdd(buffer[2], temp[1]);
        }
        // add + add + data
        else if ((type == Const.COPY_DATA) ||
          (type == Const.ADD_1BYTE))
        {
          temp[1] = buffer[3];
          LastError = GetAbsAdd(buffer[2], temp[1]);
          if (LastError == Const.SUCCESS)
            temp[2] = buffer[4];
        }
        // add + data + add + data
        else if ((type == Const.UPDATE_2BYTE) ||
          (type == Const.ADD_2BYTE))
        {
          temp[1] = buffer[2];
          temp[2] = buffer[4];
          LastError = GetAbsAdd(buffer[3], temp[2]);
          if (LastError == Const.SUCCESS)
            temp[3] = buffer[5];
        }
        // add + add + add
        else if ((type == Const.ADD_DATA) ||
          (type == Const.SUB_DATA))
        {
          temp[1] = buffer[3];
          LastError = GetAbsAdd(buffer[2], temp[1]);
          if (LastError == Const.SUCCESS)
          {
            temp[2] = buffer[5];
            LastError = GetAbsAdd(buffer[4], temp[2]);
          }
        }
        else
        {
          LastError = Const.UNDEFINE_CMD;
        }

        if (LastError == Const.SUCCESS)
          LastError = SendCmdToBox(Utils.LoByte(type - type % 4), temp, 0, type % 4 + 1);
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox update buffer fail!", Commbox.Version.C168, LastError);
    }

    public override void CommboxDelay(Timer time)
    {
      LastError = Const.SUCCESS;
      byte[] timeBuff = new byte[2];
      byte delayWord = Const.DELAYSHORT;
      double microTime = time.Microseconds;

      microTime = microTime / (_timeUnit / 1000000.0);

      if (microTime == 0)
      {
        LastError = Const.SETTIME_ERROR;
      }

      if (LastError == Const.SUCCESS)
      {
        if (microTime > 65535)
        {
          microTime = microTime / _timeBaseDB;
          if (microTime > 65535)
          {
            microTime = (microTime * _timeBaseDB) / _timeExternDB;
            if (microTime > 65535)
            {
              LastError = Const.COMMTIME_OUT;
            }
            else
            {
              delayWord = Const.DELAYLONG;
            }
          }
          else
          {
            delayWord = Const.DELAYTIME;
          }
        }

        if (LastError == Const.SUCCESS)
        {
          timeBuff[0] = Utils.HiByte(microTime);
          timeBuff[1] = Utils.LoByte(microTime);

          if (timeBuff[0] == 0)
          {
            if (IsDoNow)
              LastError = CommboxDo(delayWord, timeBuff[1]);
            else
              LastError = AddToBuff(delayWord, timeBuff[1]);
          }
          else
          {
            if (IsDoNow)
              LastError = CommboxDo(delayWord, timeBuff, 0, 2);
            else
              LastError = AddToBuff(delayWord, timeBuff, 0, 2);
          }
        }
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox delay fail!", Commbox.Version.C168, LastError);
    }

    public override void SendOutData(byte[] buff, int offset, int count)
    {
      if (count == 0 || buff == null)
      {
        LastError = Const.ILLIGICAL_LEN;
        throw new StreamException("Commbox send out data fail!", Commbox.Version.C168, LastError);
      }

      if (IsDoNow) 
        LastError = CommboxDo(Const.SEND_DATA, buff, offset, count);
      else
        LastError = AddToBuff(Const.SEND_DATA, buff, offset, count);
      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox send out data fail!", Commbox.Version.C168, LastError);
    }

    public override void RunBatch(bool repeat)
    {
      if (_cmdBuffAdd[BuffId] == Const.NULLADD)
      {
        LastError = Const.NOUSED_BUFF;
        throw new StreamException("Commbox run batch fail!", Commbox.Version.C168, LastError);
      }

      BuffId = _cmdBuffAdd[BuffId];

      if (repeat)
      {
        LastError = CommboxDo(Const.D0_BAT_FOR, BuffId);
      }
      else
      {
        if (BuffId == CmdBuffUsed[0])
        {
          LastError = CommboxDo(Const.DO_BAT_00);
        }
        else
        {
          LastError = CommboxDo(Const.D0_BAT, BuffId);
        }
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox run batch fail!", Commbox.Version.C168, LastError);
    }

    public override void Connect()
    {
      SetRF(Const.RESET_RF, 0);
      SetRF(Const.SETDTR_L, 0);
      
      LastError = InitBox();
      
      if (LastError == Const.SUCCESS)
      {
        LastError = CheckBox();
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox connect fail!", Commbox.Version.C168, LastError);
    }

    public void BeginBaudChange()
    {
      LastError = CommboxDo(Const.SET_UPBAUD, Const.UP_57600BPS);

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox begin baud change fail!", Commbox.Version.C168, LastError);

      Thread.Sleep(50);
      SetRF(Const.SETRFBAUD, Const.UP_57600BPS);
      Thread.Sleep(50);
    }

    public void EndBaudChange()
    {
      SetRF(Const.SETRFBAUD, Const.UP_57600BPS);
      LastError = CommboxDo(Const.SET_UPBAUD, Const.UP_57600BPS);
      if (LastError == Const.SUCCESS)
      {
        CheckResult(Timer.FromMilliseconds(100));
        Clear();
      }

      if (LastError != Const.SUCCESS)
        throw new StreamException("Commbox end baud change fail!", Commbox.Version.C168, LastError);
    }

    public override void Disconnect()
    {
      StopNow(true);
      DoSet(Const.RESET);
      DoSet(Const.RESET_RF);
      Clear();
    }
  }
}
