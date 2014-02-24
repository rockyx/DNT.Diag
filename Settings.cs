using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT.Diag
{
  public static class Settings
  {
    static string _lang;

    static Settings()
    {
      _lang = "en-US";
    }

    public static string Language
    {
      get { return _lang; }
      set { _lang = value; }
    }
  }
}
