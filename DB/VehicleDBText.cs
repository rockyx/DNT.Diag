﻿using System;
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
  public class VehicleDBText : VehicleDBItem
  {
    public VehicleDBText(Connection conn)
      : base(conn, "SELECT [Content] FROM [Text] WHERE [Name]=:name AND [Language]=:language AND [Class]=:class")
    {
      Command.Parameters.Add(":name", DbType.Binary);
      Command.Parameters.Add(":language", DbType.Binary);
      Command.Parameters.Add(":class", DbType.Binary);
    }

    byte[] Query(string name, string cls)
    {
      var enName = Encrypt(name);
      var enCls = Encrypt(cls);

      Command.Prepare();

      Command.Parameters[0].Value = enName;
      Command.Parameters[1].Value = Language;
      Command.Parameters[2].Value = enCls;

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

    public string Get(string name, string cls)
    {
      byte[] cipherText = Query(name, cls);
      if (cipherText == null)
        throw new DatabaseException("Query text fail in vehicle database!");
      return Decrypt(name, cls, cipherText);
    }

  }
}
