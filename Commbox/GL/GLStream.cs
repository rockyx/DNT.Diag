using System;

using DNT.Diag.Buffer;

namespace DNT.Diag.Commbox.GL
{
  internal abstract class GLStream : AbstractStream
  {
    byte _lastError; // error code

    public byte LastError
    {
      get { return _lastError; }
      set { _lastError = value; }
    }
    bool _isDB20;

    public bool IsDB20
    {
      get { return _isDB20; }
      set { _isDB20 = value; }
    }
    bool _isDoNow;

    public bool IsDoNow
    {
      get { return _isDoNow; }
      set { _isDoNow = value; }
    }
    byte[] _password;

    public byte[] Password
    {
      get { return _password; }
      set { _password = value; }
    }
    Timer _reqByteToByte;

    public Timer ReqByteToByte
    {
      get { return _reqByteToByte; }
      set { _reqByteToByte = value; }
    }
    Timer _reqWaitTime;

    public Timer ReqWaitTime
    {
      get { return _reqWaitTime; }
      set { _reqWaitTime = value; }
    }
    Timer _resByteToByte;

    public Timer ResByteToByte
    {
      get { return _resByteToByte; }
      set { _resByteToByte = value; }
    }
    Timer _resWaitTime;

    public Timer ResWaitTime
    {
      get { return _resWaitTime; }
      set { _resWaitTime = value; }
    }

    Random _rnd;
    byte _buffId;

    public byte BuffId
    {
      get { return _buffId; }
      set { _buffId = value; }
    }

    public GLStream(ToEcuBuffer toEcu, FromEcuBuffer fromEcu)
      : base(toEcu, fromEcu)
    {
      _lastError = 0;
      _isDB20 = false;
      _isDoNow = true;
      _reqByteToByte = new Timer();
      _reqWaitTime = new Timer();
      _resByteToByte = new Timer();
      _resWaitTime = new Timer();
      _rnd = new Random();
      _buffId = 0;
      _password = new byte[] 
      { 0x0C, 0x22, 0x17, 0x41, 0x57, 0x2D, 0x43, 0x17, 0x2D, 0x4D };
    }

    public abstract void CheckIdle();
    public abstract ushort BoxVer { get; }
    public abstract void CheckResult(Timer time);
    public abstract void StopNow(bool isStop);
    public abstract int ReadData(byte[] buff, int offset, int length, Timer time);
    public abstract void NewBatch();
    public abstract void DelBatch();
    public abstract void EndBatch();
    public abstract void SetLineLevel(byte valueLow, byte valueHigh);
    public void SetCommCtrl<T1, T2>(T1 valueOpen, T2 valueClose)
    {
      SetCommCtrl(Utils.LoByte(valueOpen), Utils.LoByte(valueClose));
    }
    public abstract void SetCommCtrl(byte valueOpen, byte valueClose);
    public abstract void SetCommLine(byte sendLine, byte recvLine);
    public abstract void TurnOverOneByOne();
    public abstract void KeepLink(bool isRunLink);
    public void SetCommLink<T1, T2, T3>(T1 ctrlWord1, T2 ctrlWord2, T3 ctrlWord3)
    {
      SetCommLink(Utils.LoByte(ctrlWord1), Utils.LoByte(ctrlWord2), Utils.LoByte(ctrlWord3));
    }
    public abstract void SetCommLink(byte ctrlWord1, byte ctrlWord2, byte ctrlWord3);
    public abstract void SetCommBaud(double baud);
    public abstract void SetCommTime(byte type, Timer time);
    public abstract void RunReceive(byte type);
    public abstract void CommboxDelay(Timer time);
    public void SendOutData(params byte[] buff)
    {
      SendOutData(buff, 0, buff.Length);
    }
    public abstract void SendOutData(byte[] buff, int offset, int count);
    public abstract void RunBatch(bool repeat);
    public int ReadBytes(byte[] buff, int offset, int count)
    {
      return ReadData(buff, offset, count, _resWaitTime);
    }
  }
}
