using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace DNT.Diag.Commbox.GL.C168
{
  internal partial class C168Stream
  {
    byte _cmdBuffId;

    public byte CmdBuffId
    {
      get { return _cmdBuffId; }
      set { _cmdBuffId = value; }
    }
    byte _cmdUsedNum;

    public byte CmdUsedNum
    {
      get { return _cmdUsedNum; }
      set { _cmdUsedNum = value; }
    }
    byte[] _cmdBuffAdd;

    public byte[] CmdBuffAdd
    {
      get { return _cmdBuffAdd; }
      set { _cmdBuffAdd = value; }
    }
    byte[] _cmdBuffUsed;

    public byte[] CmdBuffUsed
    {
      get { return _cmdBuffUsed; }
      set { _cmdBuffUsed = value; }
    }
    long _timeUnit;

    public long TimeUnit
    {
      get { return _timeUnit; }
      set { _timeUnit = value; }
    }
    byte _timeBaseDB;

    public byte TimeBaseDB
    {
      get { return _timeBaseDB; }
      set { _timeBaseDB = value; }
    }
    byte _timeExternDB;

    public byte TimeExternDB
    {
      get { return _timeExternDB; }
      set { _timeExternDB = value; }
    }
    byte _cmdBuffLen;

    public byte CmdBuffLen
    {
      get { return _cmdBuffLen; }
      set { _cmdBuffLen = value; }
    }
    byte[] _version;

    public byte[] Version
    {
      get { return _version; }
      set { _version = value; }
    }
    byte[] _commboxId;

    public byte[] CommboxId
    {
      get { return _commboxId; }
      set { _commboxId = value; }
    }
    byte[] _commboxPort;

    public byte[] CommboxPort
    {
      get { return _commboxPort; }
      set { _commboxPort = value; }
    }
    byte _headPassword;

    public byte HeadPassword
    {
      get { return _headPassword; }
      set { _headPassword = value; }
    }
    byte _cmdBuffHeader;

    public byte CmdBuffHeader
    {
      get { return _cmdBuffHeader; }
      set { _cmdBuffHeader = value; }
    }
    byte _cmdBuffChecksum;

    public byte CmdBuffChecksum
    {
      get { return _cmdBuffChecksum; }
      set { _cmdBuffChecksum = value; }
    }
    List<byte> _cmdBuffData;

    public List<byte> CmdBuffData
    {
      get { return _cmdBuffData; }
      set { _cmdBuffData = value; }
    }
    int _position;

    public int Position
    {
      get { return _position; }
      set { _position = value; }
    }

    void InitPrivate()
    {
      _cmdBuffId = 0;
      _cmdUsedNum = 0;
      _cmdBuffAdd = new byte[Const.MAXIM_BLOCK + 2];
      _cmdBuffUsed = new byte[Const.MAXIM_BLOCK];
      _timeUnit = 0;
      _timeBaseDB = 0;
      _cmdBuffLen = 0;
      _version = new byte[Const.VERSIONLEN];
      _commboxId = new byte[Const.COMMBOXIDLEN];
      _commboxPort = new byte[Const.COMMBOXPORTNUM];
      _headPassword = 0;
      _cmdBuffHeader = 0;
      _cmdBuffChecksum = 0;
      _cmdBuffData = new List<byte>();
    }

    byte CommboxCommand(byte commandWord)
    {
      return CommboxCommand(commandWord, null, 0, 0);
    }

    byte CommboxCommand(byte commandWord, params byte[] buff)
    {
      return CommboxCommand(commandWord, buff, 0, buff.Length);
    }

    byte CommboxCommand(byte commandWord, byte[] buff, int offset, int count)
    {
      byte checksum = Utils.LoByte(commandWord + count);
      if (commandWord < Const.WR_DATA)
      {
        if (count == 0)
        {
          return Const.ILLIGICAL_LEN;
        }
        checksum--;
      }
      else
      {
        if (count != 0)
        {
          return Const.ILLIGICAL_LEN;
        }
      }

      int i;
      List<byte> command = new List<byte>(2 + count);
      command.Add(Utils.LoByte(checksum + _headPassword));
      count += offset;
      for (i = offset; i < count; i++)
      {
        command.Add(buff[i]);
        checksum += command.Last();
      }

      command.Add(checksum);
      byte[] temp = command.ToArray();
      byte result = Const.SUCCESS;
      for (i = 0; i < 3; i++)
      {
        if (commandWord != Const.STOP_REC && commandWord != Const.STOP_EXECUTE)
        {
          try
          {
            CheckIdle();
            Write(temp);
          }
          catch
          {
            result = Const.SENDDATA_ERROR;
            continue;
          }
        }
        else
        {
          Write(temp);
        }

        result = SendOk(Timer.FromMilliseconds(200 * (count + 3)));
        if (result == Const.SUCCESS)
          break;
      }

      return result;
    }

    byte CommboxEcuOld(byte[] buff, int offset, int count)
    {
      if ((CmdBuffAdd[Const.LINKBLOCK] - CmdBuffAdd[Const.SWAPBLOCK]) < (count + 1))
      {
        return Const.NOBUFF_TOSEND;
      }

      List<byte> command = new List<byte>();
      command.Add(Utils.LoByte(Const.WR_DATA + _headPassword));
      command.Add(Utils.LoByte(count + 2));
      command.Add(_cmdBuffAdd[Const.SWAPBLOCK]);
      command.Add(Utils.LoByte(count - 1));

      byte checksum = Utils.LoByte(Const.WR_DATA + command[1] + command[2] + command[3]);

      count += offset;
      for (int i = offset; i < count; i++)
      {
        command.Add(buff[i]);
        checksum += buff[i];
      }

      command.Add(checksum);
      byte[] temp = command.ToArray();
      byte result = Const.SUCCESS;
      for (int i = 0; i < 3; i++)
      {
        try
        {
          CheckIdle();
          Write(temp);
          result = SendOk(Timer.FromMilliseconds(20 * command.Count));
          if (result == Const.SUCCESS)
            break;
        }
        catch (StreamException)
        {
          result = Const.SENDDATA_ERROR;
          continue;
        }
      }
      return result;
    }

    byte CommboxEcuNew(byte[] buff, int offset, int count)
    {
      if ((CmdBuffAdd[Const.LINKBLOCK] - CmdBuffAdd[Const.SWAPBLOCK]) < (count + 1))
      {
        return Const.NOBUFF_TOSEND;
      }

      List<byte> command = new List<byte>();
      command.Add(Utils.LoByte(Const.WR_DATA + HeadPassword));
      command.Add(Utils.LoByte(count + 3));
      command.Add(_cmdBuffAdd[Const.SWAPBLOCK]);
      command.Add(Const.SEND_CMD);

      byte checksum = Utils.LoByte(Const.WR_DATA + command[1] + command[2] + command[3] + command[4]);

      count += offset;
      for (int i = offset; i < count; i++)
      {
        command.Add(buff[i]);
        checksum += buff[i];
      }

      command.Add(checksum);
      byte[] temp = command.ToArray();
      byte result = Const.SUCCESS;
      for (int i = 0; i < 3; i++)
      {
        try
        {
          CheckIdle();
          Write(temp);
          result = SendOk(Timer.FromMilliseconds(20 * (command.Count + 7)));
          if (result == Const.SUCCESS)
            break;
        }
        catch (StreamException)
        {
          result = Const.SENDDATA_ERROR;
          continue;
        }
      }
      return result;
    }

    byte GetCmdData(byte command, byte[] buff, out int length)
    {
      length = 0;
      byte[] cmdInfo = new byte[2];
      byte checksum = command;

      if (ReadData(cmdInfo, 0, 2, Timer.FromMilliseconds(150)) != 2)
        return Const.TIMEOUT_ERROR;

      if (cmdInfo[0] != command)
      {
        Clear();
        return cmdInfo[0];
      }

      if (ReadData(buff, 0, cmdInfo[1], Timer.FromMilliseconds(150)) != cmdInfo[1] ||
        ReadData(cmdInfo, 0, 1, Timer.FromMilliseconds(150)) != 1)
      {
        return Const.TIMEOUT_ERROR;
      }

      checksum += cmdInfo[1];

      for (int i = 0; i < cmdInfo[1]; i++)
        checksum += buff[i];
      if (checksum != cmdInfo[0])
      {
        return Const.CHECKSUM_ERROR;
      }

      length = cmdInfo[1];
      return Const.SUCCESS;
    }

    byte SetRF(byte cmd, byte cmdInfo)
    {
      int times = Const.REPLAYTIMES;

      cmdInfo += cmd;

      if (cmd == Const.SETRFBAUD)
        times = 2;

      Thread.Sleep(6);

      byte result = Const.SUCCESS;
      while (times-- != 0)
      {
        try
        {
          CheckIdle();
          Write(cmdInfo);
          if (SendOk(Timer.FromMilliseconds(50)) != Const.SUCCESS)
            continue;
          Write(cmdInfo);
          CheckResult(Timer.FromMilliseconds(150));
          Thread.Sleep(100);
          return Const.SUCCESS;
        }
        catch (StreamException)
        {
        }
      }

      return Const.DISCONNECT_COMMBOX;
    }

    byte InitBox()
    {
      HeadPassword = 0x00;
      IsDB20 = false;

      byte result = CommboxDo(Const.GETINFO);
      if (result != Const.SUCCESS)
        return result;

      byte[] temp = new byte[256];
      int tempLength = 0;
      result = GetCmdData(Const.GETINFO, temp, out tempLength);

      if (result != Const.SUCCESS)
        return result;

      if (tempLength < Const.COMMBOXINFOLEN)
      {
        return Const.LOST_VERSIONDATA;
      }

      TimeUnit = 0;
      int pos = 0;
      for (int i = 0; i < Const.MINITIMELEN; i++)
      {
        TimeUnit = (TimeUnit << 8) + temp[pos++];
      }

      TimeBaseDB = temp[pos++];
      TimeExternDB = temp[pos++];
      CmdBuffLen = temp[pos++];

      if (TimeBaseDB == 0 ||
        TimeUnit == 0 ||
        CmdBuffLen == 0)
      {
        return LastError = Const.COMMTIME_ZERO;
      }

      for (int i = 0; i < Const.COMMBOXIDLEN; i++)
        CommboxId[i] = temp[pos++];
      for (int i = 0; i < Const.VERSIONLEN; i++)
        Version[i] = temp[pos++];
      CommboxPort[0] = Const.NULLADD;
      CommboxPort[1] = Const.NULLADD;
      CommboxPort[2] = Const.NULLADD;
      CommboxPort[3] = Const.NULLADD;

      CmdBuffId = Const.NULLADD;
      CmdUsedNum = 0;
      for (int i = 0; i < Const.MAXIM_BLOCK; i++)
        CmdBuffAdd[i] = Const.NULLADD;
      CmdBuffAdd[Const.LINKBLOCK] = CmdBuffLen;
      CmdBuffAdd[Const.SWAPBLOCK] = 0;

      return Const.SUCCESS;
    }

    byte CheckBox()
    {
      byte checksum = 0;
      int i = 0;
      int len = 0;
      byte[] cmdTemp = new byte[5];

      cmdTemp[4] = 0;

      Random rand = new Random();

      while (i < 4)
      {
        cmdTemp[i] = Utils.LoByte(rand.Next());
        cmdTemp[4] += cmdTemp[i++];
      }

      Write(cmdTemp);

      len = Password.Length;
      i = 0;
      checksum = Utils.LoByte(cmdTemp[4] + cmdTemp[4]);
      while (i < len)
      {
        checksum += Utils.LoByte(Password[i] ^ cmdTemp[i % 5]);
        ++i;
      }

      Thread.Sleep(20);

      byte[] buff = new byte[256];
      int length = 0;
      byte result = GetCmdData(Const.GETINFO, buff, out length);

      if (result != Const.SUCCESS)
        return result;

      HeadPassword = buff[0];

      if (checksum != HeadPassword)
      {
        return Const.CHECKSUM_ERROR;
      }

      if (HeadPassword == 0)
        HeadPassword = 0x55;
      return Const.SUCCESS;
    }

    byte SetEchoData(byte[] echoBuff, int offset, int count)
    {
      if (count == 0 || count > 4)
      {
        return Const.ILLIGICAL_LEN;
      }

      byte result = Const.SUCCESS;
      if (IsDoNow)
      {
        byte[] buff = new byte[6];
        result = CommboxDo(Const.ECHO, echoBuff, offset, count);
        if (result != Const.SUCCESS)
          return result;
        if (ReadData(buff, 0, count, Timer.FromMilliseconds(100)) != count)
          return Const.TIMEOUT_ERROR;

        while (count-- > 0)
        {
          if (buff[count] != echoBuff[count + offset])
          {
            return Const.CHECKSUM_ERROR;
          }
        }
        try
        {
          CheckResult(Timer.FromMilliseconds(100));
        }
        catch (StreamException)
        {
          return Const.ERROR_REC;
        }
      }
      return AddToBuff(Const.ECHO, echoBuff, offset, count);
    }

    byte GetAbsAdd(byte buffId, byte buffAdd)
    {
      int length;
      byte startAdd;

      if (CmdBuffId != buffId)
      {
        if (CmdBuffAdd[buffId] == Const.NULLADD)
        {
          return Const.NOUSED_BUFF;
        }

        if (buffId == Const.LINKBLOCK)
        {
          length = CmdBuffLen - CmdBuffAdd[Const.LINKBLOCK];
        }
        else
        {
          int i;
          for (i = 0; i < _cmdUsedNum; ++i)
            if (CmdBuffUsed[i] == buffId)
              break;
          if (i == (CmdUsedNum - 1))
            length = CmdBuffAdd[Const.SWAPBLOCK] - CmdBuffAdd[buffId];
          else
            length = CmdBuffAdd[buffId + 1] - CmdBuffAdd[buffId];
        }
        startAdd = CmdBuffAdd[buffId];
      }
      else
      {
        length = CmdBuffAdd[Const.LINKBLOCK] - CmdBuffAdd[Const.SWAPBLOCK];
        startAdd = CmdBuffAdd[Const.SWAPBLOCK];
      }

      if (buffId < length)
      {
        buffAdd += startAdd;
        return Const.SUCCESS;
      }

      return Const.OUTADDINBUFF;
    }

    byte CommboxDo(byte commandWord)
    {
      return CommboxDo(commandWord, null, 0, 0);
    }

    byte CommboxDo(byte commandWord, params byte[] buff)
    {
      return CommboxDo(commandWord, buff, 0, buff.Length);
    }

    byte CommboxDo(byte commandWord, byte[] buff, int offset, int count)
    {
      if (count > Const.CMD_DATALEN)
      {
        if (commandWord == Const.SEND_DATA && count <= Const.SEND_LEN)
        {
          byte ret;
          if (BoxVer > 0x400)
          {
            // support long command
            ret = CommboxEcuNew(buff, offset, count);
          }
          else
          {
            // keep old version COMMBOX
            ret = CommboxEcuOld(buff, offset, count);
          }
          if (ret != Const.SUCCESS) return ret;
          return CommboxCommand(Const.D0_BAT, _cmdBuffAdd[Const.SWAPBLOCK]);
        }
        else
        {
          return Const.ILLIGICAL_LEN;
        }
      }
      return CommboxCommand(commandWord, buff, offset, count);
    }

    byte DoSet(byte commandWord)
    {
      return DoSet(commandWord, null, 0, 0);
    }

    byte DoSet(byte commandWord, params byte[] buff)
    {
      return DoSet(commandWord, buff, 0, buff.Length);
    }

    byte DoSet(byte commandWord, byte[] buff, int offset, int count)
    {
      int times = Const.REPLAYTIMES;
      while ((times--) > 0)
      {
        try
        {
          if (CommboxDo(commandWord, buff, offset, count) != Const.SUCCESS)
            continue;
          CheckResult(Timer.FromMilliseconds(50));
          break;
        }
        catch (StreamException)
        {
          try
          {
            StopNow(true);
          }
          catch
          {
          }
        }
      }

      if (times == 0)
        return Const.ILLIGICAL_CMD;

      return Const.SUCCESS;
    }

    byte SendOk(Timer time)
    {
      Timeout = time;
      byte receiveBuffer = 0;

      if (ReadByte(ref receiveBuffer))
      {
        if (receiveBuffer == Const.SEND_OK)
        {
          return Const.SUCCESS;
        }
        else if (receiveBuffer >= Const.UP_TIMEOUT && receiveBuffer <= Const.ERROR_REC)
        {
          return Const.SENDDATA_ERROR;
        }
      }
      return Const.TIMEOUT_ERROR;
    }

    byte AddToBuff(byte commandWord)
    {
      return AddToBuff(commandWord, null, 0, 0);
    }

    byte AddToBuff(byte commandWord, params byte[] buff)
    {
      return AddToBuff(commandWord, buff, 0, buff.Length);
    }

    byte AddToBuff(byte commandWord, byte[] buff, int offset, int count)
    {
      Position = _cmdBuffData.Count + count + 1;
      if (CmdBuffId == Const.NULLADD)
      {
        // block is used?
        IsDoNow = true;
        return Const.NOAPPLICATBUFF;
      }

      if ((CmdBuffAdd[Const.LINKBLOCK] - CmdBuffAdd[Const.SWAPBLOCK]) < Position)
      {
        // enough space ?
        IsDoNow = true;
        return Const.NOBUFF_TOSEND;
      }

      if (commandWord < Const.RESET
        && commandWord != Const.CLR_LINK
        && commandWord != Const.DO_BAT_00
        && commandWord != Const.D0_BAT
        && commandWord != Const.D0_BAT_FOR
        && commandWord != Const.WR_DATA)
      {
        // is cache command?
        if (count <= Const.CMD_DATALEN ||
          (commandWord == Const.SEND_DATA && count < Const.SEND_LEN))
        {
          // is legal command?
          if (commandWord == Const.SEND_DATA && BoxVer > 0x400)
          {
            // support long command
            CmdBuffData.Add(Const.SEND_CMD);
            CmdBuffChecksum += Const.SEND_CMD;
            if (count != 0)
            {
              CmdBuffData.Add(Utils.LoByte(commandWord + count - 1));
            }
            else
            {
              CmdBuffData.Add(Utils.LoByte(commandWord + count));
            }
            CmdBuffChecksum += CmdBuffData[CmdBuffData.Count - 1];

            count += offset;
            for (int i = offset; i < count; i++)
            {
              CmdBuffData.Add(buff[i]);
              CmdBuffChecksum += buff[i];
            }
            CmdBuffChecksum += Utils.LoByte(count + 2 - offset);
            Position++;
          }
          else
          {
            if (count != 0)
            {
              CmdBuffData.Add(Utils.LoByte(commandWord + count - 1));
            }
            else
            {
              CmdBuffData.Add(Utils.LoByte(commandWord + count));
            }

            CmdBuffChecksum += CmdBuffData.Last();
            count += offset;
            for (int i = offset; i < count; i++)
            {
              CmdBuffData.Add(buff[i]);
              CmdBuffChecksum += buff[i];
            }
            CmdBuffChecksum += Utils.LoByte(count + 1 - offset);
            Position++;
          }
          return Const.SUCCESS;
        }
        IsDoNow = true;
        return Const.ILLIGICAL_LEN;
      }
      IsDoNow = true;
      return Const.UNBUFF_CMD;
    }

    byte SendCmdToBox(byte command, byte[] buffer, int offset, int count)
    {
      if (IsDoNow) return DoSet(command, buffer, offset, count);
      return AddToBuff(command, buffer, offset, count);
    }

    byte SendCmdToBox(byte command)
    {
      return SendCmdToBox(command, null, 0, 0);
    }

    byte SendCmdToBox(byte command, params byte[] buffer)
    {
      return SendCmdToBox(command, buffer, 0, buffer.Length);
    }
  }

}
