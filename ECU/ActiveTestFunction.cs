using System;

using DNT.Diag.Interop;

namespace DNT.Diag.ECU
{
  public class ActiveTestFunction
  {
    IntPtr _native = IntPtr.Zero;
    ActiveState _state = ActiveState.Stop;

    internal ActiveTestFunction(IntPtr native)
    {
      _native = native;
    }

    public ActiveState State
    {
      get { return _state; }
      set
      {
        NativeMethods.RActiveTestFunctionChangeState(_native, (int)value);
        _state = value;
      }
    }

    public bool Execute(int mode)
    {
      return NativeMethods.RActiveTestFunctionExecute(_native, mode);
    }
  }
}
