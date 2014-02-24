using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DNT.Diag.Test
{
  [TestClass]
  public class SystemDBTest
  {
    [TestMethod]
    public void QueryText()
    {
      var inst = DNT.Diag.DB.SystemDB.Instance;
      try
      {
        inst.Open(System.Environment.CurrentDirectory);

        string queryName = "Communicating";
        string zhCNExpect = "正在通讯...";
        string enUSExpect = "Communicating...";

        Settings.Language = "zh-CN";
        string result = inst.QueryText(queryName);
        Assert.AreEqual(zhCNExpect, result);

        Settings.Language = "en-US";
        result = inst.QueryText(queryName);
        Assert.AreEqual(enUSExpect, result);
      }
      catch (DNT.Diag.DB.DatabaseException ex)
      {
        Assert.Fail(ex.Message);
      }
    }
  }
}
