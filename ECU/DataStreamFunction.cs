using System;

using DNT.Diag.Interop;
using DNT.Diag.Data;

namespace DNT.Diag.ECU
{
  public class DataStreamFunction
  {
    IntPtr _native;

    internal DataStreamFunction(IntPtr native)
    {
      _native = native;
    }

    ~DataStreamFunction()
    {
      NativeMethods.RDataStreamFunctionFree(_native);
    }

    public LiveDataList LiveData
    {
      get
      {
        return new LiveDataList(NativeMethods.RDataStreamFunctionGetLiveData(_native));
      }
    }

    public void StopRead()
    {
      if (_native == IntPtr.Zero)
        return;

      NativeMethods.RDataStreamFunctionStopRead(_native);
    }

    public void StopCalc()
    {
      if (_native == IntPtr.Zero)
        return;

      NativeMethods.RDataStreamFunctionStopCalc(_native);
    }

    public bool ReadData()
    {
      return NativeMethods.RDataStreamFunctionReadData(_native);
    }

    public bool ReadDataOnce()
    {
      return NativeMethods.RDataStreamFunctionReadDataOnce(_native);
    }

    public bool CalcData()
    {
      return NativeMethods.RDataStreamFunctionCalcData(_native);
    }

    public bool CalcDataOnce()
    {
      return NativeMethods.RDataStreamFunctionCalcDataOnce(_native);
    }
  }
}
