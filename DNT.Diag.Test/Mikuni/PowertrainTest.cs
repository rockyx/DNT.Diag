using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DNT.Diag;
using DNT.Diag.IO;
using DNT.Diag.Data;
using DNT.Diag.DB;
using DNT.Diag.ECU;
using DNT.Diag.ECU.Mikuni;

namespace DNT.Diag.Test.Mikuni
{
  [TestClass]
  public class PowertrainTest
  {
    string zhCNPath = Path.GetFullPath("zh-CN");
    Commbox _box;
    VehicleDB _db;
    Powertrain _ecu;

    [TestInitialize]
    public void Initialize()
    {
      Register.Init(zhCNPath);
      _db = new VehicleDB(Path.GetFullPath("."), "DCJ");
      _db.Open();

      _box = new Commbox(CommboxVer.C168);
      _box.Connect();

      _ecu = new Powertrain(_box, _db, PowertrainModel.DCJ_16A);
      _ecu.IOChannelInit();
    }

    [TestCleanup]
    public void Cleanup()
    {
      _box.Disconnect();
      _db.Close();
    }

    [TestMethod]
    public void TroubleCodeTest()
    {
      try
      {
        var tcf = _ecu.TroubleCode;
        var ctc = tcf.Current;
        var htc = tcf.History;
        tcf.Clear();
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void DataStreamTest()
    {
      try
      {
        var dsf = _ecu.DataStream;
        var ldl = dsf.LiveData;

        foreach (var ld in ldl)
        {
          ld.IsShowed = true;
        }

        ldl.Collate();

        dsf.Start();
        Thread.Sleep(10000);
        dsf.Stop();

        dsf.Once();

        ldl = dsf.LiveData;
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void ISCLearnValueInitializationTest()
    {
      try
      {
        _ecu.ISCLearnValueInitialization();
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void LongTermLearnValueZoneInitializationTest()
    {
      try
      {
        _ecu.LongTermLearnValueZoneInitialization();
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void TPSIdleSettingTest()
    {
      try
      {
        _ecu.TPSIdleSetting();
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    [TestMethod]
    public void VersionTest()
    {
      try
      {
        var version = _ecu.Version;
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }
  }
}
