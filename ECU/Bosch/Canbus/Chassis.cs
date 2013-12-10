using System;

using DNT.Diag.Interop;
using DNT.Diag.IO;
using DNT.Diag.DB;

namespace DNT.Diag.ECU.Bosch.Canbus
{
  public class Chassis : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Chassis(Commbox box, VehicleDB db, ChassisModel model)
    {
      _native = NativeMethods.RBoschCanbusChassisConstruct(box.Native, db.Native, (int)model);
      base.Native = NativeMethods.RBoschCanbusChassisCast(_native);
    }

    ~Chassis()
    {
      NativeMethods.RBoschCanbusChassisDestruct(_native);
    }
  }
}
