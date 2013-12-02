using System;
using System.Collections.Generic;

using DNT.Diag.Interop;

namespace DNT.Diag.Data
{
  public class TroubleCodeVector : List<TroubleCodeItem>
  {
    IntPtr _native;
    internal TroubleCodeVector(IntPtr native)
    {
      _native = native;
      int size = NativeMethods.RTroubleCodeVectorSize(_native);
      for (int i = 0; i < size; i++)
      {
        Add(new TroubleCodeItem(NativeMethods.RTrobleCodeVectorGet(_native, i)));
      }
    }

    ~TroubleCodeVector()
    {
      NativeMethods.RTroubleCodeVectorFree(_native);
    }
  }
}
