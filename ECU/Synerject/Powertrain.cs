using System;
using System.Text;

using DNT.Diag.Interop;

namespace DNT.Diag.ECU.Synerject
{
  public class Powertrain : AbstractECU
  {
    private IntPtr _native = IntPtr.Zero;

    public Powertrain(IntPtr boxNative, IntPtr dbNative, PowertrainModel model)
    {
      _native = NativeMethods.RSynerjectPowertrainConstruct(boxNative, dbNative, (int)model);
      base.Native = NativeMethods.RSynerjectPowertrainCast(_native);
    }

    ~Powertrain()
    {
      NativeMethods.RSynerjectPowertrainDestruct(_native);
    }

    public string Version
    {
      get
      {
        byte[] utf8 = new byte[100];
        int length = NativeMethods.RSynerjectPowertrainGetECUVersion(_native, utf8);
        return UTF8Encoding.UTF8.GetString(utf8, 0, length);
      }
    }
  }
}
