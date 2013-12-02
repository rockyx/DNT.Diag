using System;
using System.Text;

using DNT.Diag.Interop;

namespace DNT.Diag.DB
{
  public static class SystemDB
  {
    public static void Init(string path)
    {
      byte[] utf8 = UTF8Encoding.UTF8.GetBytes(path);
      NativeMethods.RSystemDBInit(utf8, utf8.Length);
    }

    public static string QueryText(string name)
    {
      byte[] utf8 = UTF8Encoding.UTF8.GetBytes(name);
      byte[] text = new byte[1024];
      int length = NativeMethods.RSystemDBQueryText(utf8, utf8.Length, text);
      return UTF8Encoding.UTF8.GetString(text, 0, length);
    }
  }
}
