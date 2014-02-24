using System;

namespace DNT.Diag.Data
{
  public class TroubleCodeItem
  {
    string _code;
    string _content;
    string _description;

    public TroubleCodeItem()
    {
      _code = "";
      _content = "";
      _description = "";
    }

    public string Code
    {
      get { return _code; }
      set { _code = value; }
    }

    public string Content
    {
      get { return _content; }
      set { _content = value; }
    }

    public string Description
    {
      get { return _description; }
      set { _description = value; }
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;

      if (this == obj) return true;

      TroubleCodeItem item = obj as TroubleCodeItem;
      if (item == null) return false;

      return Code.Equals(item.Code) &&
        Content.Equals(item.Content) &&
        Description.Equals(item.Description);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
