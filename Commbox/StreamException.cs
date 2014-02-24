using System;

namespace DNT.Diag.Commbox
{
  public class StreamException : ApplicationException
  {
    Version _version;
    int _errorCode;

    public StreamException()
      : base()
    {
    }

    public StreamException(string message, Version version, int errorCode)
      : base(message)
    {
      _version = version;
      _errorCode = errorCode;
    }

    public int ErrorCode
    {
      get { return _errorCode; }
    }

    public Version Version
    {
      get { return _version; }
    }
  }
}
