using System;
using System.Text;
using System.IO;

using DNT.Diag.Interop;
using DNT.Diag.DB;
using DNT.Diag.IO;

namespace DNT.Diag.ECU.Mikuni
{
  public class Powertrain : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Powertrain(Commbox box, VehicleDB db, PowertrainModel model)
    {
      _native = NativeMethods.RMikuniPowertrainConstruct(box.Native, db.Native, (int)model);
      base.Native = NativeMethods.RMikuniPowertrainCast(_native);
    }

    ~Powertrain()
    {
      NativeMethods.RMikuniPowertrainDestruct(_native);
    }

    public string Version
    {
      get
      {
        byte[] utf8 = new byte[100];
        int length = NativeMethods.RMikuniPowertrainGetECUVersion(_native, utf8);
        if (length <= 0)
          throw new IOException(LastInfo);
        return UTF8Encoding.UTF8.GetString(utf8, 0, length);
      }
    }

    public void TPSIdleSetting()
    {
      if (!NativeMethods.RMikuniPowertrainTPSIdleSetting(_native))
        throw new IOException(LastInfo);
    }

    public void LongTermLearnValueZoneInitialization()
    {
      if (!NativeMethods.RMikuniPowertrainLongTermLearnValueZoneInitialization(_native))
        throw new IOException(LastInfo);
    }

    public void ISCLearnValueInitialization()
    {
      if (!NativeMethods.RMikuniPowertrainISCLearnValueInitialization(_native))
        throw new IOException(LastInfo);
    }
  }
}
