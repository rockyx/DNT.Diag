using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.DB
{
  public abstract class VehicleDBItem : LocaleDB, IDisposable
  {
    SQLiteConnection _conn = null;
    SQLiteCommand _command = null;

    public VehicleDBItem(SQLiteConnection conn, string queryCommand)
    {
      _conn = conn;
      _command = new SQLiteCommand(queryCommand, conn);
    }

    protected SQLiteConnection Connection
    {
      get { return _conn; }
    }

    protected SQLiteCommand Command
    {
      get { return _command; }
    }

    public void Dispose()
    {
      _command.Dispose();
    }
  }
}
