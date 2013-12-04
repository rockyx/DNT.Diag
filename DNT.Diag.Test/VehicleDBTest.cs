using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DNT.Diag.DB;


namespace DNT.Diag.Test
{
  [TestClass]
  public class VehicleDBTest
  {
    private VehicleDB db;

    [TestInitialize]
    public void Initialize()
    {
      db = null;
    }

    [TestMethod]
    public void Test()
    {
      try
      {
        db = new VehicleDB(Path.GetFullPath("."), "DCJ");
        db.Open();
      }
      catch (FileNotFoundException ex)
      {
        Assert.Fail(ex.Message);
      }
      finally
      {
        try
        {
          db.Close();
        }
        finally
        {
        }
      }

      try
      {
        db = new VehicleDB("", "");
        db.Open();
        Assert.Fail("Vehicle DB should open fail but success!");
      }
      catch (FileNotFoundException ex)
      {
        // this is what we want to
      }
      finally
      {
        try
        {
          db.Close();
        }
        finally
        {
        }
      }
    }
  }
}
