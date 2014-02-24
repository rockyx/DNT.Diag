using System;

namespace DNT.Diag.DB
{
  public class DatabaseException : ApplicationException
  {
    public DatabaseException()
    {
    }

    public DatabaseException(string message)
      : base(message)
    {
    }
  }
}
