using System;

using DNT.Diag.Interop;

namespace DNT.Diag.IO
{
  class IOBuffer
  {
    private IntPtr _native;
    public IOBuffer()
    {
      _native = NativeMethods.RIOBufferNew();
      if (_native == IntPtr.Zero)
        throw new OutOfMemoryException();
    }

    ~IOBuffer()
    {
      NativeMethods.RIOBufferFree(_native);
    }

    public void PushToEcuBuffer(byte[] buff, int offset, int count)
    {
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();

      NativeMethods.RIOBufferPushToFromEcuBuffer(_native, buff, offset, count);
    }

    public int PopFromEcuBuffer(byte[] buff, int offset, int count)
    {
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();

      return NativeMethods.RIOBufferPopFromToEcuBuffer(_native, buff, offset, count);
    }

    public int ToEcuBufferBytesAvailable()
    {
      if (_native == IntPtr.Zero)
        throw new NullReferenceException();

      return NativeMethods.RIOBufferToEcuBufferBytesAvailable(_native);
    }

    /// <summary>
    /// Only use for NativeMethods
    /// </summary>
    internal IntPtr Native
    {
      get
      {
        return _native;
      }
    }
  }
}
