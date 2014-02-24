using System;
using System.Collections.Generic;

#if ANDROID
using Mono.Data.Sqlite;

using Connection = Mono.Data.Sqlite.SqliteConnection;
using Command = Mono.Data.Sqlite.SqliteCommand;
using ConnectionStringBuilder = Mono.Data.Sqlite.SqliteConnectionStringBuilder;
using Parameter = Mono.Data.Sqlite.SqliteParameter;
#else
using System.Data.SQLite;

using Connection = SQLiteConnection;
using Command = SQLiteCommand;
using ConnectionStringBuilder = SQLiteConnectionStringBuilder;
using Parameter = SQLiteParameter;
#endif


namespace DNT.Diag.DB
{
  public abstract class VehicleDBItem : LocaleDB, IDisposable
  {
    Connection _conn = null;
    Command _command = null;

    public VehicleDBItem(Connection conn, string queryCommand)
    {
      _conn = conn;
      _command = new Command(queryCommand, conn);
    }

    protected Connection Connection
    {
      get { return _conn; }
    }

    protected Command Command
    {
      get { return _command; }
    }

    public void Dispose()
    {
      _command.Dispose();
    }
  }
}
