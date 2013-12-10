using System;
using System.IO;

using DNT.Diag.Interop;
using DNT.Diag.Data;

namespace DNT.Diag.ECU
{
  public class TroubleCodeFunction
  {
    IntPtr _native;
    AbstractECU _ecu;

    internal TroubleCodeFunction(IntPtr native, AbstractECU ecu)
    {
      _native = native;
      _ecu = ecu;
    }

    ~TroubleCodeFunction()
    {
      if (_native == IntPtr.Zero)
        return;
      NativeMethods.RTroubleCodeFunctionFree(_native);
    }

    private void CheckNative()
    {
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();
    }

    public TroubleCodeVector Current
    {
      get
      {
        CheckNative();
        if (!NativeMethods.RTroubleCodeFunctionCurrent(_native))
          throw new IOException(_ecu.LastInfo);
        return new TroubleCodeVector(NativeMethods.RTroubleCodeFunctionGetTroubleCodes(_native));
      }
    }

    public TroubleCodeVector History
    {
      get
      {
        CheckNative();
        if (!NativeMethods.RTroubleCodeFunctionHistory(_native))
          throw new IOException(_ecu.LastInfo);
        return new TroubleCodeVector(NativeMethods.RTroubleCodeFunctionGetTroubleCodes(_native));
      }
    }

    public void Clear()
    {
      CheckNative();
      if (!NativeMethods.RTroubleCodeFunctionClear(_native))
        throw new IOException(_ecu.LastInfo);
    }
  }
}
