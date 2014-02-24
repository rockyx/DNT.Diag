using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.DB
{
  public class VehicleDBLiveData : VehicleDBItem
  {
    static Dictionary<string, Data.LiveDataList> _liveDatas;

    struct LiveDataItem
    {
      public byte[] ShortName;
      public byte[] Content;
      public byte[] Unit;
      public byte[] DefaultValue;
      public byte[] Description;
      public byte[] CmdName;
      public byte[] CmdClass;
      public byte[] Index;
    }

    static VehicleDBLiveData()
    {
      _liveDatas = new Dictionary<string, Data.LiveDataList>();
    }

    VehicleDBCommand _cmd;

    public VehicleDBLiveData(SQLiteConnection conn, VehicleDBCommand cmd)
      : base(conn, "SELECT [ShortName], [Content], [Unit], [DefaultValue], [CommandName], [CommandClass], [Description], [Index] FROM [LiveData] WHERE [Language]=:language AND [Class]=:class")
    {
      _cmd = cmd;

      Command.Parameters.Add(":language", DbType.Binary);
      Command.Parameters.Add(":class", DbType.Binary);
    }

    Data.LiveDataItem Decrypt(ref LiveDataItem item)
    {
      if ((item.ShortName == null) ||
        (item.Content == null) ||
        (item.Index == null))
        return null;

      Data.LiveDataItem ld = new Data.LiveDataItem();
      
      ld.ShortName = DecryptToString(item.ShortName);

      ld.Content = DecryptToString(item.Content);

      if (item.Unit != null)
        ld.Unit = DecryptToString(item.Unit);

      if (item.DefaultValue != null)
      {
        ld.DefaultValue = DecryptToString(item.DefaultValue);
        var minMax = ld.DefaultValue.Split('~');
        if (minMax != null && minMax.Length == 2)
        {
          ld.MinValue = minMax[0];
          ld.MaxValue = minMax[1];
        }
      }

      if ((item.CmdName != null) && (item.CmdClass != null))
      {
        ld.CmdName = DecryptToString(item.CmdName);
        ld.CmdClass = DecryptToString(item.CmdClass);
        ld.Command = _cmd.Get(ld.CmdName, ld.CmdClass);
      }

      if (item.Description != null)
        ld.Description = DecryptToString(item.Description);

      var indexBytes = DecryptToBytes(item.Index);
      ld.IndexForSort = (indexBytes[3] << 24) | (indexBytes[2] << 16) | (indexBytes[1] << 8) | (indexBytes[0]);

      return ld;
    }

    Data.LiveDataList Query(string cls)
    {
      var enCls = Encrypt(cls);

      Command.Parameters[0].Value = Language;
      Command.Parameters[1].Value = enCls;

      Data.LiveDataList lds = new Data.LiveDataList();

      using (var reader = Command.ExecuteReader())
      {
        LiveDataItem item = new LiveDataItem();
        while (reader.Read())
        {
          item.ShortName = reader.IsDBNull(0) ? null : reader.GetFieldValue<byte[]>(0);
          item.Content = reader.IsDBNull(1) ? null : reader.GetFieldValue<byte[]>(1);
          item.Unit = reader.IsDBNull(2) ? null : reader.GetFieldValue<byte[]>(2);
          item.DefaultValue = reader.IsDBNull(3) ? null : reader.GetFieldValue<byte[]>(3);
          item.CmdName = reader.IsDBNull(4) ? null : reader.GetFieldValue<byte[]>(4);
          item.CmdClass = reader.IsDBNull(5) ? null : reader.GetFieldValue<byte[]>(5);
          item.Description = reader.IsDBNull(6) ? null : reader.GetFieldValue<byte[]>(6);
          item.Index = reader.IsDBNull(7) ? null : reader.GetFieldValue<byte[]>(7);

          var ld = Decrypt(ref item);
          if (ld == null)
            throw new DatabaseException("LiveData loss some important field!");

          lds.Add(ld);
        }
      }

      if (lds.Count <= 0)
        throw new DatabaseException("Query livedata fail in vehicle database!");

      return lds;
    }

    public Data.LiveDataList Get(string cls)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0}_{1}", cls, Settings.Language);
      string key = sb.ToString();

      if (!_liveDatas.ContainsKey(key))
      {
        _liveDatas[key] = Query(cls);
      }

      return _liveDatas[key];
    }
  }
}
