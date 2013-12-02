using System;
using System.Text;

using DNT.Diag.Interop;

namespace DNT.Diag
{
  public static class Register
  {
    public static void Init(string path)
    {
      var utf8 = UTF8Encoding.UTF8.GetBytes(path);
      NativeMethods.RRegisterInit(utf8, utf8.Length);
    }

    public static void Save(string reg)
    {
      var utf8 = UTF8Encoding.UTF8.GetBytes(reg);
      NativeMethods.RRegisterSave(utf8, utf8.Length);
    }

    public static string IdCode
    {
      get
      {
        byte[] utf8 = new byte[100];
        int length = NativeMethods.RRegisterGetIdCode(utf8);
        return UTF8Encoding.UTF8.GetString(utf8, 0, length);
      }
    }

    public static bool IsReged
    {
      get
      {
        return NativeMethods.RRegisterCheck();
      }
    }
  }
}
