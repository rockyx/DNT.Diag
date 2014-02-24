using System;

using DNT.Diag.Buffer;

namespace DNT.Diag.Commbox.GL
{
  internal abstract class GLStreamImpl<T> : GLStream
    where T : Constant
  {
    T _const = null;

    public T Const
    {
      protected set
      {
        _const = value;
      }
      get
      {
        return _const;
      }
    }

    protected void GetLinkTime(byte type, Timer time)
    {
      if (type == Const.SETBYTETIME)
      {
        ReqByteToByte = time;
      }
      else if (type == Const.SETWAITTIME)
      {
        ReqWaitTime = time;
      }
      else if (type == Const.SETRECBBOUT)
      {
        ResByteToByte = time;
      }
      else if (type == Const.SETRECFROUT)
      {
        ResWaitTime = time;
      }
    }

    public GLStreamImpl(ToEcuBuffer toEcu, FromEcuBuffer fromEcu)
      : base(toEcu, fromEcu)
    {
    }
  }
}
