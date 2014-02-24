using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DNT.Diag.Commbox.GL.W80
{
  internal partial class W80Stream
  {
    uint _timeUnit; // 1/10000 seconds
    byte _timeBaseDB; // standard time times
    byte _timeExternDB; // expand time times
    byte[] _port;
    byte[] _buf;
    int _pos;
    bool _isLink; // is heartbeat block
    byte _runFlag;
    int _startPos;
    ushort _boxVer;

    uint TimeUnit
    {
      get { return _timeUnit; }
      set { _timeUnit = value; }
    }

    byte TimeBaseDB
    {
      get { return _timeBaseDB; }
      set { _timeBaseDB = value; }
    }

    byte TimeExternDB
    {
      get { return _timeExternDB; }
      set { _timeExternDB = value; }
    }

    int Pos
    {
      get { return _pos; }
      set { _pos = value; }
    }

    bool IsLink
    {
      get { return _isLink; }
      set { _isLink = value; }
    }

    byte RunFlag
    {
      get { return _runFlag; }
      set { _runFlag = value; }
    }

    int StartPos
    {
      get { return _startPos; }
      set { _startPos = value; }
    }

    byte[] Port
    {
      get { return _port; }
      set { _port = value; }
    }

    Byte[] Buff
    {
      get { return _buf; }
      set { _buf = value; }
    }

    void InitPrivate()
    {
      _timeUnit = 0;
      _timeBaseDB = 0;
      _timeExternDB = 0;
      _port = new byte[Const.MAXPORT_NUM];
      _buf = new byte[Const.MAXBUFF_LEN];
      _pos = 0;
      _isLink = false;
      _runFlag = 0;
      _startPos = 0;
      _boxVer = 0;
    }

    bool CheckSend()
    {
      Timeout = Timer.FromMilliseconds(200);
      byte buffer = 0;

      if (!ReadByte(ref buffer))
        return false;
      
      if (buffer == Const.RECV_OK)
        return true;

      return false;
    }

    bool SendCmd(byte cmd)
    {
      return SendCmd(cmd, null, 0, 0);
    }

    bool SendCmd(byte cmd, params byte[] buffer)
    {
      return SendCmd(cmd, buffer, 0, buffer.Length);
    }

    bool SendCmd(byte cmd, byte[] buffer, int offset, int count)
    {
      if ((count + offset) > buffer.Length)
        throw new IndexOutOfRangeException("buffer");

      byte cs = cmd;
      List<byte> data = new List<byte>();

      data.Add(Utils.LoByte(cmd + _runFlag));

      if (buffer != null && count > 0)
      {
        count += offset;
        for (int i = offset; i < count; i++)
        {
          cs += buffer[i];
          data.Add(buffer[i]);
        }
      }

      data.Add(cs);

      byte[] temp = data.ToArray();

      for (int i = 0; i < 3; i++)
      {
        try
        {
          CheckIdle();
          Write(temp);
          if (CheckSend()) return true;
        }
        catch (StreamException)
        {
          continue;
        }
      }

      return false;
    }

    bool GetCmdData(byte[] buff, int offset, int maxCnt)
    {
      byte[] len = new byte[1];
      if (RecvBytes(buff, offset, 1) != 1) return false;
      if (RecvBytes(len, 0, 1) != 1) return false;
      if (len[0] > maxCnt) len[0] = Utils.LoByte(maxCnt);
      if (RecvBytes(buff, offset, len[0]) != len[0]) return false;
      byte[] cs = new byte[1];
      if (RecvBytes(cs, 0, 1) != 1) return false;
      return len[0] > 0;
    }

    bool DoCmdNow(byte cmd, byte[] buff, int offset, int count)
    {
      if (cmd == Const.WR_DATA)
      {
        if (count <= 0) return false;
        byte[] tmp = new byte[2 + count];
        if (IsLink) tmp[0] = 0xFF; // Write hearbeat
        else tmp[0] = 0x00; // Write communication command
        tmp[1] = Utils.LoByte(count);

        Array.Copy(buff, offset, tmp, 2, count);
        return SendCmd(Const.WR_DATA, tmp);
      }
      else if (cmd == Const.SEND_DATA)
      {
        if (count <= 0) return false;
        byte[] tmp = new byte[4 + count];
        tmp[0] = 0x00; // Write Position
        tmp[1] = Utils.LoByte(count + 2); // data length
        tmp[2] = Const.SEND_DATA; // command
        tmp[3] = Utils.LoByte(count - 1); // command length - 1
        Array.Copy(buff, offset, tmp, 4, count);
        if (!SendCmd(Const.WR_DATA, tmp)) return false;
        return SendCmd(Const.DO_BAT_C);
      }
      else
      {
        return SendCmd(cmd, buff, offset, count);
      }
    }

    bool AddCmdToBuff(byte cmd)
    {
      return AddCmdToBuff(cmd, null, 0, 0);
    }

    bool AddCmdToBuff(byte cmd, params byte[] buff)
    {
      return AddCmdToBuff(cmd, buff, 0, buff.Length);
    }

    bool AddCmdToBuff(byte cmd, byte[] buff, int offset, int count)
    {
      Buff[Pos++] = cmd;
      if (cmd == Const.SEND_DATA)
        Buff[Pos++] = Utils.LoByte(count - 1);

      StartPos = Pos;

      if (count > 0)
      {
        Array.Copy(buff, offset, Buff, Pos, count);
        _pos += count;
      }
      return true;
    }

    bool DoCmd(byte cmd)
    {
      return DoCmd(cmd, null, 0, 0);
    }

    bool DoCmd(byte cmd, params byte[] buff)
    {
      return DoCmd(cmd, buff, 0, buff.Length);
    }

    bool DoCmd(byte cmd, byte[] buff, int offset, int count)
    {
      if (cmd != Const.WR_DATA && cmd != Const.SEND_DATA)
        cmd |= Utils.LoByte(count);
      if (IsDoNow)
        // send to COMMBOX execute
        return DoCmdNow(cmd, buff, offset, count);
      else
        // send to cache
        return AddCmdToBuff(cmd, buff, offset, count);
    }

    bool DoSet(byte cmd)
    {
      return DoSet(cmd, null, 0, 0);
    }

    bool DoSet(byte cmd, params byte[] buff)
    {
      return DoSet(cmd, buff, 0, buff.Length);
    }

    bool DoSet(byte cmd, byte[] buff, int offset, int count)
    {
      try
      {
        bool result = DoCmd(cmd, buff, offset, count);
        if (DoCmd(cmd, buff, offset, count) && IsDoNow)
          CheckResult(Timer.FromMilliseconds(150));
        return true;
      }
      catch (StreamException)
      {
        return false;
      }
    }

    int RecvBytes(byte[] buff, int offset, int count)
    {
      return ReadData(buff, offset, count, Timer.FromMilliseconds(500));
    }

    bool GetBuffData(byte addr, byte[] buff, int offset, int count)
    {
      // addr is relative to AUTOBUFF_0 position
      byte[] tmp = new byte[2];
      tmp[0] = addr;
      tmp[1] = Utils.LoByte(count);
      if (!DoCmd(Const.GET_BUF, tmp)) return false;
      return GetCmdData(buff, offset, count);
    }

    bool InitBox()
    {
      byte[] buf = new byte[32];
      
      Random rand = new Random();
      int i;
      for (i = 1; i < 4; i++)
        buf[i] = Utils.LoByte(rand.Next());

      byte run = 0;
      for (i = 0; i < Password.Length; i++)
        run += Utils.LoByte(Password[i] ^ buf[i % 3 + 1]);

      if (run == 0)
        run = 0x55;

      if (!DoCmd(Const.GET_CPU, buf, 1, 3))
        return false;

      if (!GetCmdData(buf, 0, 32))
        return false;

      RunFlag = 0;
      IsDoNow = true;
      Pos = 0;
      IsDB20 = false;
      TimeUnit = 0;

      for (i = 0; i < 3; i++)
        TimeUnit = (TimeUnit << 8) + buf[i];

      TimeBaseDB = buf[i++];
      TimeExternDB = buf[i++];

      for (i = 0; i < Const.MAXPORT_NUM; i++)
        Port[i] = 0xFF;

      return true;
    }

    bool CheckBox()
    {
      byte[] buff = new byte[32];
      if (!DoCmd(Const.GET_BOXID))
        return false;
      if (!GetCmdData(buff, 0, 32))
        return false;
      _boxVer = (ushort)((buff[10] << 8) | buff[11]);
      return true;
    }

    bool Reset()
    {
      try
      {
        StopNow(true);
        Clear();
        for (int i = 0; i < Const.MAXPORT_NUM; i++)
          Port[i] = 0xFF;
        return DoCmd(Const.RESET);
      }
      catch (StreamException)
      {
        return false;
      }
    }

    bool UpdateBuff(byte type, byte addr, byte data)
    {
      byte[] buff = new byte[3];
      int len = 0;
      buff[0] = addr;
      buff[1] = data;

      if ((type == Const.INC_BYTE)
        || (type == Const.ADD_BYTE)
        || (type == Const.SUB_BYTE))
      {
        len = 1;
      }
      else if ((type == Const.UPDATE_BYTE)
        || (type == Const.ADD_BYTE)
        || (type == Const.SUB_BYTE))
      {
        len = 2;
      }
      else if (type == Const.COPY_BYTE)
      {
        len = 3;
      }

      return DoSet(type, buff, 0, len);
    }

    bool CopyBuff(byte dest, byte src, byte len)
    {
      byte[] buf = new byte[3];
      buf[0] = dest;
      buf[1] = src;
      buf[2] = len;
      return DoSet(Const.COPY_BYTE, buf);
    }
  }
}
