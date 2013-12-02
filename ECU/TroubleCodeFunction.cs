using System;
using System.IO;

using DNT.Diag.Interop;
using DNT.Diag.Data;

namespace DNT.Diag.ECU
{
  public class TroubleCodeFunction
  {
    IntPtr _native;

    internal TroubleCodeFunction(IntPtr native)
    {
      _native = native;
    }

    ~TroubleCodeFunction()
    {
      if (_native == IntPtr.Zero)
        return;
      NativeMethods.RTroubleCodeFunctionFree(_native);
    }

    public TroubleCodeVector Current
    {
      get
      {
        if (!NativeMethods.RTroubleCodeFunctionCurrent(_native))
          throw new IOException();
        return new TroubleCodeVector(NativeMethods.RTroubleCodeFunctionGetTroubleCodes(_native));
      }
    }

    public TroubleCodeVector History
    {
      get
      {
        if (!NativeMethods.RTroubleCodeFunctionHistory(_native))
          throw new IOException();
        return new TroubleCodeVector(NativeMethods.RTroubleCodeFunctionGetTroubleCodes(_native));
      }
    }

    public void Clear()
    {
      if (!NativeMethods.RTroubleCodeFunctionClear(_native))
        throw new IOException();
    }
  }
}
