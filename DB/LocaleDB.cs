using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag.DB
{
  public abstract class LocaleDB : AbstractDB
  {
    static Dictionary<string, string> _decrypt;

    static LocaleDB()
    {
      _decrypt = new Dictionary<string, string>();
    }

    protected byte[] Language
    {
      get { return Encrypt(Settings.Language); }
    }

    protected string Decrypt(string name, string cls, byte[] cipherBytes)
    {
      if (cipherBytes == null)
        throw new ArgumentNullException("CipherBytes");

      StringBuilder sb = new StringBuilder(100);
      sb.AppendFormat("{0}_{1}_{2}", name, Settings.Language, cls);
      string key = sb.ToString();

      if (!_decrypt.ContainsKey(key))
      {          
        _decrypt[key] = DecryptToString(cipherBytes);
      }
      return _decrypt[key];
    }
  }
}
