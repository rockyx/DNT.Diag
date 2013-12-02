using System;
using System.Text;

using DNT.Diag.Interop;

namespace DNT.Diag.Data
{
  public class TroubleCodeItem
  {
    IntPtr _native;

    internal TroubleCodeItem(IntPtr native)
    {
      _native = native;
    }

    ~TroubleCodeItem()
    {
      NativeMethods.RTroubleCodeItemFree(_native);
    }

    public string Code
    {
      get
      {
        byte[] text = new byte[100];
        int length = NativeMethods.RTroubleCodeItemGetCode(_native, text);
        return UTF8Encoding.UTF8.GetString(text, 0, length);
      }
    }

    public string Content
    {
      get
      {
        byte[] text = new byte[1024];
        int length = NativeMethods.RTroubleCodeItemGetContent(_native, text);
        return UTF8Encoding.UTF8.GetString(text, 0, length);
      }
    }

    public string Description
    {
      get
      {
        byte[] text = new byte[1024];
        int length = NativeMethods.RTroubleCodeItemGetDescription(_native, text);
        return UTF8Encoding.UTF8.GetString(text, 0, length);
      }
    }
  }
}
