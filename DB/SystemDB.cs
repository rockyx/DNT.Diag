using System;
using System.Data;
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
  public class SystemDB : LocaleDB
  {
    Connection _conn = null;
    Command _command = null;

    static SystemDB _inst;

    static SystemDB()
    {
      _inst = new SystemDB();
    }

    private SystemDB()
    {
    }

    public static SystemDB Instance
    {
      get { return _inst; }
    }

    public void Open(string filePath)
    {
      if (_command != null)
        _command.Dispose();

      if (_conn != null)
        _conn.Close();

      try
      {
        ConnectionStringBuilder connstr = new ConnectionStringBuilder();
        _conn = new Connection();

        connstr.DataSource = (filePath.EndsWith("/") || filePath.EndsWith("\\")) ? filePath + "sys.db" : filePath + "/sys.db";
        _conn.ConnectionString = connstr.ToString();
        _conn.Open();

        _command = new Command(_conn);
        _command.CommandText = "SELECT Content FROM [Text] WHERE [Name]=:name AND [Language]=:language";
        _command.Parameters.Add(new Parameter(":name", DbType.Binary));
        _command.Parameters.Add(new Parameter(":language", DbType.Binary));
      }
      catch
      {
        if (_command != null)
          _command.Dispose();
        if (_conn != null)
          _conn.Dispose();

        _command = null;
        _conn = null;

        throw new DatabaseException("Cannot Open System Database");
      }
    }

    byte[] Query(string name)
    {
      if (_conn == null || _command == null)
        throw new DatabaseException("Database Not Open Yet!");
 
      var enName = Encrypt(name);

      _command.Prepare();

      _command.Parameters[0].Value = enName;
      _command.Parameters[1].Value = Language;

      byte[] result = null;

      using (var reader = _command.ExecuteReader())
      {
        if (reader.Read())
        {
          result = reader.GetFieldValue<byte[]>(0);
        }
      }

      return result;
    }

    public string QueryText(string name)
    {
      byte[] cipherText = Query(name);
      if (cipherText == null)
        throw new DatabaseException("Query text fail in system database!");

      return Decrypt(name, "System", Query(name));
    }
  }
}
