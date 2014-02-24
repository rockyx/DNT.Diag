using System;
using System.Collections.Generic;
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
  public class VehicleDBCommand : VehicleDBItem
  {
    public VehicleDBCommand(Connection conn)
      : base(conn, "SELECT [Command] FROM [Command] WHERE [Name]=:name AND [Class]=:class")
    {
      Command.Parameters.Add(":name", DbType.Binary);
      Command.Parameters.Add(":class", DbType.Binary);
    }

    byte[] Query(string name, string cls)
    {
      Command.Prepare();

      var enName = Encrypt(name);
      var enCls = Encrypt(cls);

      Command.Parameters[0].Value = enName;
      Command.Parameters[1].Value = enCls;

      byte[] result = null;
      using (var reader = Command.ExecuteReader())
      {
        if (reader.Read())
        {
          result = reader.GetFieldValue<byte[]>(0);
        }
      }

      return result;
    }

    public byte[] Get(string name, string cls)
    {
      var cipher = Query(name, cls);
      if (cipher == null)
        throw new DatabaseException("Query command fail in vehicle database!");

      var temp = DecryptToBytes(cipher);
      int length = (temp[0] << 8) + temp[1];

      byte[] result = new byte[length];
      Array.Copy(temp, 2, result, 0, length);
      return result;
    }
  }
}
