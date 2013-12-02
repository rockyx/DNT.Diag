using System;

using DNT.Diag.Interop;

namespace DNT.Diag.ECU.Bosch.Canbus
{
  public class Chassis : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Chassis(IntPtr boxNative, IntPtr dbNative, ChassisModel model)
    {
      _native = NativeMethods.RBoschCanbusChassisConstruct(boxNative, dbNative, (int)model);
      base.Native = NativeMethods.RBoschCanbusChassisCast(_native);
    }

    ~Chassis()
    {
      NativeMethods.RBoschCanbusChassisDestruct(_native);
    }
  }
}
