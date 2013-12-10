using System;
using System.Text;
using System.IO;

using DNT.Diag.Interop;

namespace DNT.Diag.ECU
{
  public abstract class AbstractECU
  {
    IntPtr _native = IntPtr.Zero;
    TroubleCodeFunction _troubleCode = null;
    DataStreamFunction _dataStream = null;
    DataStreamFunction _freezeStream = null;
    ActiveTestFunction _activeTest = null;

    protected AbstractECU()
    {
      _troubleCode = new TroubleCodeFunction(IntPtr.Zero, this);
      _dataStream = new DataStreamFunction(IntPtr.Zero, this);
      _freezeStream = new DataStreamFunction(IntPtr.Zero, this);
      _activeTest = new ActiveTestFunction(IntPtr.Zero);
    }

    ~AbstractECU()
    {
      NativeMethods.RAbstractECUFree(_native);
    }

    protected IntPtr Native
    {
      get { return _native; }
      set { _native = value; }
    }

    public string LastInfo
    {
      get
      {
        byte[] buff = new byte[1024];
        int length = NativeMethods.RAbstractECUGetLastInfo(_native, buff);
        return UTF8Encoding.UTF8.GetString(buff, 0, length);
      }
    }

    public virtual void IOChannelInit()
    {
      if (!NativeMethods.RAbstractECUIOChannelInit(_native))
        throw new IOException(LastInfo);
      _troubleCode = new TroubleCodeFunction(NativeMethods.RAbstractECUGetTroubleCode(_native), this);
      _dataStream = _dataStream = new DataStreamFunction(NativeMethods.RAbstractECUGetDataStream(_native), this);
      _freezeStream = new DataStreamFunction(NativeMethods.RAbstractECUGetFreezeStream(_native), this);
      _activeTest = new ActiveTestFunction(NativeMethods.RAbstractECUGetActiveTest(_native));
    }

    public TroubleCodeFunction TroubleCode
    {
      get
      {
        return _troubleCode;
      }
    }

    public DataStreamFunction DataStream
    {
      get
      {
        return _dataStream;
      }
    }

    public DataStreamFunction FreezeStream
    {
      get
      {
        return _freezeStream;
      }
    }

    public ActiveTestFunction ActiveTest
    {
      get
      {
        return _activeTest;
      }
    }
  }
}
