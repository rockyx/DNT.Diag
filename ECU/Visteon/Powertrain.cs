using System;

using DNT.Diag.Interop;

namespace DNT.Diag.ECU.Visteon
{
  public class Powertrain : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Powertrain(IntPtr boxNative, IntPtr dbNative, PowertrainModel model)
    {
      _native = NativeMethods.RVisteonPowertrainConstruct(boxNative, dbNative, (int)model);
      base.Native = NativeMethods.RVisteonPowertrainCast(_native);
    }

    ~Powertrain()
    {
      NativeMethods.RVisteonPowertrainDestruct(_native);
    }
  }
}
