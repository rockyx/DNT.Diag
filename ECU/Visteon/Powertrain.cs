using System;

using DNT.Diag.Interop;
using DNT.Diag.IO;
using DNT.Diag.DB;

namespace DNT.Diag.ECU.Visteon
{
  public class Powertrain : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Powertrain(Commbox box, VehicleDB db, PowertrainModel model)
    {
      _native = NativeMethods.RVisteonPowertrainConstruct(box.Native, db.Native, (int)model);
      base.Native = NativeMethods.RVisteonPowertrainCast(_native);
    }

    ~Powertrain()
    {
      NativeMethods.RVisteonPowertrainDestruct(_native);
    }
  }
}
