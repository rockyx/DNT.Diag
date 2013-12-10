using System;
using System.Runtime.InteropServices;

namespace DNT.Diag.Interop
{
  static class NativeMethods
  {
    // Stream Buffer
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RIOBufferNew();

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RIOBufferFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RIOBufferPushToFromEcuBuffer(IntPtr p, byte[] buff, int offset, int count);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RIOBufferPopFromToEcuBuffer(IntPtr p, byte[] buff, int offset, int count);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RIOBufferToEcuBufferBytesAvailable(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RCommboxNew(IntPtr nativeStream, int version);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RCommboxFree(IntPtr native);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RCommboxConnect(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RCommboxDisconnect(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RCommboxC168BeginBaudChange(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RCommboxC168EndBaudChange(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RRegisterInit(byte[] path, int length);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RRegisterGetIdCode(byte[] code);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RRegisterSave(byte[] reg, int length);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RRegisterCheck();

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RSystemDBInit(byte[] path, int length);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RSystemDBQueryText(byte[] name, int length, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RVehicleDBNew(byte[] path, int pathLength, byte[] name, int nameLength);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RVehicleDBFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RVehicleDBGetLanguage(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RVehicleDBOpen(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RVehicleDBClose(IntPtr p);

    // Abstract ECU
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RAbstractECUFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RAbstractECUGetTroubleCode(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RAbstractECUGetDataStream(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RAbstractECUGetFreezeStream(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RAbstractECUGetActiveTest(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RAbstractECUIOChannelInit(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RAbstractECUGetLastInfo(IntPtr p, byte[] buff);

    // Trouble Code Item
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RTroubleCodeItemFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RTroubleCodeItemGetCode(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RTroubleCodeItemGetContent(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RTroubleCodeItemGetDescription(IntPtr p, byte[] text);

    // Trouble Code Vector
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RTroubleCodeVectorFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RTroubleCodeVectorSize(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RTroubleCodeVectorGet(IntPtr p, int index);

    // Trouble Code Function
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RTroubleCodeFunctionFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RTroubleCodeFunctionCurrent(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RTroubleCodeFunctionHistory(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RTroubleCodeFunctionClear(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RTroubleCodeFunctionGetTroubleCodes(IntPtr p);

    // Live Data Item
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RLiveDataItemFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetShortName(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetContent(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetUnit(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetDefaultValue(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetDescription(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetCmdName(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetCmdClass(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetIndex(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetPosition(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RLiveDataItemIsEnabled(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RLiveDataItemIsShowed(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RLiveDataItemSetShowed(IntPtr p, bool isShowed);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RLiveDataItemIsOutOfRange(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetMinValue(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetMaxValue(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetCommand(IntPtr p, byte[] command);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataItemGetValue(IntPtr p, byte[] text);

    // Live Data List
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RLiveDataListFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RLiveDataListGet(IntPtr p, int index);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataListSize(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataListGetEnabledCount(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataListGetShowedCount(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataListGetNextShowedIndex(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataListGetEnabledIndex(IntPtr p, int index);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataListGetShowedPosition(IntPtr p, int index);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RLiveDataListGetShowedIndex(IntPtr p, int index);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RLiveDataListCollateEnable(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RLiveDataListCollateShowed(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RLiveDataListEmpty(IntPtr p);

    // Data Stream Function
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RDataStreamFunctionFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RDataStreamFunctionGetLiveData(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RDataStreamFunctionStopRead(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RDataStreamFunctionStopCalc(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RDataStreamFunctionReadData(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RDataStreamFunctionReadDataOnce(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RDataStreamFunctionCalcData(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RDataStreamFunctionCalcDataOnce(IntPtr p);

    // Active Test
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RActiveTestFunctionFree(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RActiveTestFunctionChangeState(IntPtr p, int state);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RActiveTestFunctionExecute(IntPtr p, int mode);

    // Bosch
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RBoschCanbusChassisCast(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RBoschCanbusChassisConstruct(IntPtr box, IntPtr db, int model);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RBoschCanbusChassisDestruct(IntPtr p);

    // Denso
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RDensoPowertrainCast(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RDensoPowertrainConstruct(IntPtr box, IntPtr db, int model);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RDensoPowertrainDestruct(IntPtr p);

    // Mikuni
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RMikuniPowertrainCast(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RMikuniPowertrainConstruct(IntPtr box, IntPtr db, int model);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RMikuniPowertrainDestruct(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RMikuniPowertrainGetECUVersion(IntPtr p, byte[] text);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RMikuniPowertrainTPSIdleSetting(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RMikuniPowertrainLongTermLearnValueZoneInitialization(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern bool RMikuniPowertrainISCLearnValueInitialization(IntPtr p);

    // Synerject
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RSynerjectPowertrainCast(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RSynerjectPowertrainConstruct(IntPtr box, IntPtr db, int model);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RSynerjectPowertrainDestruct(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int RSynerjectPowertrainGetECUVersion(IntPtr p, byte[] text);

    // Visteon
    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RVisteonPowertrainCast(IntPtr p);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr RVisteonPowertrainConstruct(IntPtr box, IntPtr db, int model);

    [DllImport("dntdiag", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void RVisteonPowertrainDestruct(IntPtr p);
  }
}