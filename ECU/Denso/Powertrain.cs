using System;

using DNT.Diag.Interop;
using DNT.Diag.IO;
using DNT.Diag.DB;

namespace DNT.Diag.ECU.Denso
{
  public class Powertrain : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Powertrain(Commbox box, VehicleDB db, PowertrainModel model)
    {
      _native = NativeMethods.RDensoPowertrainConstruct(box.Native, db.Native, (int)model);
      base.Native = NativeMethods.RDensoPowertrainCast(_native);
    }

    ~Powertrain()
    {
      NativeMethods.RDensoPowertrainDestruct(_native);
    }
  }
}
