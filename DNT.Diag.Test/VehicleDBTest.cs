using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using DNT.Diag.DB;
using DNT.Diag.Data;

namespace DNT.Diag.Test
{
  [TestClass]
  public class VehicleDBTest
  {
    VehicleDB _db;
    [TestInitialize]
    public void OpenVehicleDB()
    {
      try
      {
        _db = new VehicleDB();
        _db.Open(System.Environment.CurrentDirectory, "test");
      }
      catch (DatabaseException ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestCleanup]
    public void CloseVehicleDB()
    {
      if (_db != null)
        _db.Close();
    }

    [TestMethod]
    public void QueryTextSuccess()
    {
      try
      {
        string queryText = "Injector On Test";
        string zhCNExpect = "喷油器测试";
        string enUSExpect = "Injector On Test";
        string cls = "QingQi";

        Settings.Language = "zh-CN";
        string result = _db.QueryText(queryText, cls);
        Assert.AreEqual(zhCNExpect, result);

        Settings.Language = "en-US";
        result = _db.QueryText(queryText, cls);
        Assert.AreEqual(enUSExpect, result);
      }
      catch (DatabaseException ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void QueryTextFail()
    {
      try
      {
        string result = _db.QueryText("adfa", "adkfja");
        Assert.Fail("Should not reach here!");
      }
      catch (DatabaseException ex)
      {
      }
    }

    [TestMethod]
    public void QueryCommandSuccess()
    {
      try
      {
        string name = "Synthetic Failure";
        string cls = "Mikuni";
        byte[] expect = 
        {
          0x44, 0x32, 0x30, 0x30, 0x32, 0x30
        };

        var result = _db.QueryCommand(name, cls);
        Assert.IsTrue(expect.SequenceEqual(result));
      }
      catch (DatabaseException ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void QueryCommandFail()
    {
      try
      {
        var result = _db.QueryCommand("dfadf", "adfad");
        Assert.Fail("Query command fail should not reach here!");
      }
      catch (DatabaseException ex)
      {
      }
    }

    [TestMethod]
    public void QueryTroubleCodeSuccess()
    {
      try
      {
        string cls = "QingQi";
        TroubleCodeItem zhCNExpect = new TroubleCodeItem();
        zhCNExpect.Code = "0040";
        zhCNExpect.Content = "从进气歧管压力输入电压低于其最小可能值。";
        zhCNExpect.Description = "接插件接触不良/传感器损坏";

        TroubleCodeItem enUSExpect = new TroubleCodeItem();
        enUSExpect.Code = "0040";
        enUSExpect.Content = "Input voltage from the intake manifold pressure fell below its minimum possible value.";
        enUSExpect.Description = "Connector poor contact / sensor damage";

        Settings.Language = "zh-CN";
        var result = _db.QueryTroubleCode(zhCNExpect.Code, cls);
        Assert.IsTrue(zhCNExpect.Equals(result));

        Settings.Language = "en-US";
        result = _db.QueryTroubleCode(enUSExpect.Code, cls);
        Assert.IsTrue(enUSExpect.Equals(result));
      }
      catch (DatabaseException ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void QueryTroubleCodeFail()
    {
      try
      {
        var result = _db.QueryTroubleCode("dfadf", "dafdsf");
        Assert.Fail("Query trouble code fail should not reach here!");
      }
      catch (DatabaseException ex)
      {
      }
    }

    [TestMethod]
    public void QueryLiveDataSuccess()
    {
      try
      {
        string cls = "QingQi";

        Settings.Language = "zh-CN";

        var result = _db.QueryLiveData(cls);
        foreach (var item in result)
        {
          item.IsEnabled = true;
          item.IsDisplay = true;
        }

        Settings.Language = "en-US";

        result = _db.QueryLiveData(cls);
        foreach (var item in result)
        {
          item.IsEnabled = true;
          item.IsDisplay = true;
        }
      }
      catch (DatabaseException ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void QueryLiveDataFail()
    {
      try
      {
        var result = _db.QueryLiveData("dafd");
        Assert.Fail("Query live data fail should not reach here!");
      }
      catch (DatabaseException ex)
      {
      }
    }
  }
}
