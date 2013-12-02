using System;

using DNT.Diag.Interop;

namespace DNT.Diag.ECU.Denso
{
  public class Powertrain : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Powertrain(IntPtr boxNative, IntPtr dbNative, PowertrainModel model)
    {
      _native = NativeMethods.RDensoPowertrainConstruct(boxNative, dbNative, (int)model);
      base.Native = NativeMethods.RDensoPowertrainCast(_native);
    }

    ~Powertrain()
    {
      NativeMethods.RDensoPowertrainDestruct(_native);
    }
  }
}
