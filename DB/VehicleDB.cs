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

using DNT.Diag.Data;

namespace DNT.Diag.DB
{
  public class VehicleDB
  {
    Connection _conn;
    VehicleDBText _text;
    VehicleDBCommand _command;
    VehicleDBTroubleCode _troubleCode;
    VehicleDBLiveData _liveData;

    public VehicleDB()
    {
      _conn = null;
      _text = null;
      _command = null;
      _troubleCode = null;
      _liveData = null;
    }

    public void Close()
    {
      if (_liveData != null)
        _liveData.Dispose();

      if (_troubleCode != null)
        _troubleCode.Dispose();

      if (_command != null)
        _command.Dispose();

      if (_text != null)
        _text.Dispose();

      if (_conn != null)
        _conn.Close();

      _liveData = null;
      _troubleCode = null;
      _command = null;
      _text = null;
      _conn = null;
    }

    public void Open(string filePath, string dbName)
    {
      try
      {
        Close();

        ConnectionStringBuilder connstr = new ConnectionStringBuilder();
        StringBuilder sb = new StringBuilder();
        if (filePath.EndsWith("/") || filePath.EndsWith("\\"))
        {
          sb.AppendFormat("{0}{1}.db", filePath, dbName);
        }
        else
        {
          sb.AppendFormat("{0}/{1}.db", filePath, dbName);
        }

        connstr.DataSource = sb.ToString();
        _conn = new Connection();
        _conn.ConnectionString = connstr.ToString();
        _conn.Open();

        _text = new VehicleDBText(_conn);
        _command = new VehicleDBCommand(_conn);
        _troubleCode = new VehicleDBTroubleCode(_conn);
        _liveData = new VehicleDBLiveData(_conn, _command);
      }
      catch
      {
        Close();
        throw new DatabaseException("Cannot open vehicle database!");
      }
    }

    void CheckOpened<T>(T obj)
    {
      if (obj == null)
        throw new DatabaseException("Vehicle database doesn't open yet!");
    }

    public string QueryText(string name, string cls)
    {
      CheckOpened(_text);
      return _text.Get(name, cls);
    }

    public byte[] QueryCommand(string name, string cls)
    {
      CheckOpened(_command);
      return _command.Get(name, cls);
    }

    public TroubleCodeItem QueryTroubleCode(string code, string cls)
    {
      CheckOpened(_troubleCode);
      return _troubleCode.Get(code, cls);
    }

    public LiveDataList QueryLiveData(string cls)
    {
      CheckOpened(_liveData);
      return _liveData.Get(cls);
    }
  }
}
