using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.DB
{
  public class VehicleDBCommand : VehicleDBItem
  {
    public VehicleDBCommand(SQLiteConnection conn)
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
