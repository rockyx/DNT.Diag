using System;
using System.Text;
using System.IO;

using DNT.Diag.Interop;

namespace DNT.Diag.DB
{
  public class VehicleDB
  {
    private IntPtr _native;

    public VehicleDB(string path, string name)
    {
      byte[] pathUtf8 = UTF8Encoding.UTF8.GetBytes(path);
      byte[] nameUtf8 = UTF8Encoding.UTF8.GetBytes(name);
      _native = NativeMethods.RVehicleDBNew(pathUtf8, pathUtf8.Length, nameUtf8, nameUtf8.Length);
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();
    }

    ~VehicleDB()
    {
      NativeMethods.RVehicleDBFree(_native);
    }

    public void Open()
    {
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();
      if (!NativeMethods.RVehicleDBOpen(_native))
        throw new FileNotFoundException();
    }

    public void Close()
    {
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();
      NativeMethods.RVehicleDBClose(_native);
    }

    public string GetLanguage()
    {
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();
      byte[] utf8 = new byte[100];
      int length = NativeMethods.RVehicleDBGetLanguage(_native, utf8);
      return UTF8Encoding.UTF8.GetString(utf8, 0, length);
    }

    public IntPtr Native
    {
      get { return _native; }
    }
  }
}
