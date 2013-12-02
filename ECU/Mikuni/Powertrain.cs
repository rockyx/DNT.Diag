using System;
using System.Text;

using DNT.Diag.Interop;

namespace DNT.Diag.ECU.Mikuni
{
  public class Powertrain : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Powertrain(IntPtr boxNative, IntPtr dbNative, PowertrainModel model)
    {
      _native = NativeMethods.RMikuniPowertrainConstruct(boxNative, dbNative, (int)model);
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
        return UTF8Encoding.UTF8.GetString(utf8, 0, length);
      }
    }

    public bool TPSIdleSetting()
    {
      return NativeMethods.RMikuniPowertrainTPSIdleSetting(_native);
    }

    public bool LongTermLearnValueZoneInitialization()
    {
      return NativeMethods.RMikuniPowertrainLongTermLearnValueZoneInitialization(_native);
    }

    public bool ISCLearnValueInitialization()
    {
      return NativeMethods.RMikuniPowertrainISCLearnValueInitialization(_native);
    }
  }
}
