using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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
  public class VehicleDBTroubleCode : VehicleDBItem
  {
    struct TroubleCodeItem
    {
      public byte[] Content;
      public byte[] Description;
    }

    static Dictionary<string, Data.TroubleCodeItem> _troubleCodes;

    static VehicleDBTroubleCode()
    {
      _troubleCodes = new Dictionary<string, Data.TroubleCodeItem>();
    }

    public VehicleDBTroubleCode(Connection conn)
      : base(conn, "SELECT [Content], [Description] FROM [TroubleCode] WHERE [Code]=:code AND [Language]=:language AND [Class]=:class")
    {
      Command.Parameters.Add(":code", DbType.Binary);
      Command.Parameters.Add(":language", DbType.Binary);
      Command.Parameters.Add(":class", DbType.Binary);
    }

    bool Query(string code, string cls, ref TroubleCodeItem item)
    {
      Command.Prepare();

      var enCode = Encrypt(code);
      var enCls = Encrypt(cls);

      Command.Parameters[0].Value = enCode;
      Command.Parameters[1].Value = Language;
      Command.Parameters[2].Value = enCls;

      using (var reader = Command.ExecuteReader())
      {
        if (reader.Read())
        {
          item.Content = reader.GetFieldValue<byte[]>(0);
          item.Description = reader.GetFieldValue<byte[]>(1);
          return true;
        }
      }

      return false;
    }

    Data.TroubleCodeItem Decrypt(ref TroubleCodeItem item)
    {
      if (item.Content == null)
        return null;

      Data.TroubleCodeItem tc = new Data.TroubleCodeItem();

      tc.Content = DecryptToString(item.Content);
      if (item.Description != null)
        tc.Description = DecryptToString(item.Description);

      return tc;
    }

    public Data.TroubleCodeItem Get(string code, string cls)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0}_{1}_{2}", code, Settings.Language, cls);
      string key = sb.ToString();

      if (!_troubleCodes.ContainsKey(key))
      {
        TroubleCodeItem item = new TroubleCodeItem();
        if (!(Query(code, cls, ref item)))
          throw new DatabaseException("Query trouble code fail in vehicle database!");

        var tc = Decrypt(ref item);
        if (tc == null)
          throw new DatabaseException("Decrypt trouble code item fail!");

        tc.Code = code;
        _troubleCodes[key] = tc;
      }

      return _troubleCodes[key];
    }

  }
}
