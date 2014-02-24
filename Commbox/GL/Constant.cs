using System;

namespace DNT.Diag.Commbox.GL
{
  internal abstract class Constant
  {
    public int REPLAYTIMES //错误运行次数
    {
      get { return 3; }
    }

    public abstract int LINKBLOCK { get; }
    public abstract byte WR_DATA { get; }
    public abstract byte WR_LINK { get; }
    public abstract byte SEND_DATA { get; }
    public abstract byte RESET { get; }
    public abstract byte STOP_REC { get; }
    public abstract byte STOP_EXECUTE { get; }
    public abstract byte SET_ONEBYONE { get; }
    public abstract byte SET_UPBAUD { get; }
    public abstract byte DELAYSHORT { get; }
    public abstract byte DELAYTIME { get; }
    //批处理执行次数
    public byte RUN_ONCE
    {
      get { return 0x00; }
    }
    public byte RUN_MORE
    {
      get { return 0x01; }
    }
    //通讯校验和方式
    public byte CHECK_SUM
    {
      get { return 0x01; }
    }
    public byte CHECK_REVSUM
    {
      get { return 0x02; }
    }
    public byte CHECK_CRC
    {
      get { return 0X03; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  通讯口 PORT
    ///////////////////////////////////////////////////////////////////////////////
    public byte DH                    //高电平输出,1为关闭,0为打开
    {
      get { return 0x80; }
    }
    public byte DL2                   //低电平输出,1为关闭,0为打开,正逻辑发送通讯线
    {
      get { return 0x40; }
    }
    public byte DL1                   //低电平输出,1为关闭,0为打开,正逻辑发送通讯线,带接受控制
    {
      get { return 0x20; }
    }
    public byte DL0                   //低电平输出,1为关闭,0为打开,正逻辑发送通讯线,带接受控制
    {
      get { return 0x10; }
    }
    public byte PWMS                  //PWM发送线
    {
      get { return 0x08; }
    }
    public byte PWMR
    {
      get { return 0x04; }
    }
    public byte COMS                  //标准发送通讯线路
    {
      get { return 0x02; }
    }
    public byte COMR
    {
      get { return 0x01; }
    }
    public byte SET_NULL                  //不选择任何设置
    {
      get { return 0x00; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  通讯物理控制口
    ///////////////////////////////////////////////////////////////////////////////
    public byte PWC                       //通讯电平控制,1为5伏,0为12伏
    {
      get { return 0x80; }
    }
    public byte REFC                      //通讯比较电平控制,1为通讯电平1/5,0为比较电平控制1/2
    {
      get { return 0x40; }
    }
    public byte CK                        //K线控制开关,1为双线通讯,0为单线通讯
    {
      get { return 0x20; }
    }
    public byte SZFC                      //发送逻辑控制,1为负逻辑,0为正逻辑
    {
      get { return 0x10; }
    }
    public byte RZFC                      //接受逻辑控制,1为负逻辑,0为正逻辑
    {
      get { return 0x08; }
    }
    public byte DLC0                      //DLC1接受控制,1为接受关闭,0为接受打开
    {
      get { return 0x04; }
    }
    public byte DLC1                      //DLC0接受控制,1为接受关闭,0为接受打开
    {
      get { return 0x02; }
    }
    public byte SLC                       //线选地址锁存器控制线(待用)
    {
      get { return 0x01; }
    }
    public byte CLOSEALL                      //关闭所有发送口线，和接受口线
    {
      get { return 0x08; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  通讯控制字1设定
    ///////////////////////////////////////////////////////////////////////////////
    public byte RS_232
    {
      get { return 0x00; }
    }
    public byte EXRS_232
    {
      get { return 0x20; }
    }
    public byte SET_VPW
    {
      get { return 0x40; }
    }
    public byte SET_PWM
    {
      get { return 0x60; }
    }
    public byte BIT9_SPACE
    {
      get { return 0x00; }
    }
    public byte BIT9_MARK
    {
      get { return 0x01; }
    }
    public byte BIT9_EVEN
    {
      get { return 0x02; }
    }
    public byte BIT9_ODD
    {
      get { return 0x03; }
    }
    public byte SEL_SL
    {
      get { return 0x00; }
    }
    public byte SEL_DL0
    {
      get { return 0x08; }
    }
    public byte SEL_DL1
    {
      get { return 0x10; }
    }
    public byte SEL_DL2
    {
      get { return 0x18; }
    }
    public byte SET_DB20
    {
      get { return 0x04; }
    }
    public byte UN_DB20
    {
      get { return 0x00; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  通讯控制字3设定
    ///////////////////////////////////////////////////////////////////////////////
    public byte ONEBYONE
    {
      get { return 0x80; }
    }
    public byte INVERTBYTE
    {
      get { return 0x40; }
    }
    public byte ORIGNALBYTE
    {
      get { return 0x00; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  通讯设置参数时间
    ///////////////////////////////////////////////////////////////////////////////

    public byte SETBYTETIME           //字节间时间设定 db20? ，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0x88; }
    }
    public byte SETVPWSTART            //设置vpw发送数据时需发送0的时间。
    {
      get { return 0x08; }
    }
    public byte SETWAITTIME               //空闲等待时间设定 db20?，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0x8C; }
    }
    public byte SETLINKTIME           //链路保持时建设定 db20?，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0x90; }
    }
    public byte SETRECBBOUT           //接受字节超时错误判断 db20（vpw为指令数） ，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0x94; }
    }
    public byte SETRECFROUT            //接受一帧超时错误判断?db20?，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0x98; }
    }
    public byte SETVPWRECS
    {
      get { return 0x14; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  操作数据缓冲区
    ///////////////////////////////////////////////////////////////////////////////
    public abstract byte SUB_BYTE { get; }              //8D 结果地址1 数据1                  结果地址1=数据1-结果地址1

    ///////////////////////////////////////////////////////////////////////////////
    //  接受命令类型定义
    ///////////////////////////////////////////////////////////////////////////////

    public abstract byte GET_PORT1 { get; }             //等到通讯口的当前状态
    public abstract byte SET55_BAUD { get; }                //计算0x55的波特率
    public byte REC_FR                //接受一帧命令  E0 开始时回传开始接受信号，然后长期等待接受，接到数据实时回传，
    {
      get { return 0xE0; }
    }
    public byte REC_LEN_1             //接受1个数据，返回
    {
      get { return 0xE1; }
    }
    public byte REC_LEN_2             //接受2个数据，返回
    {
      get { return 0xE2; }
    }
    public byte REC_LEN_3             //接受3个数据，返回
    {
      get { return 0xE3; }
    }
    public byte REC_LEN_4             //接受4个数据，返回
    {
      get { return 0xE4; }
    }
    public byte REC_LEN_5             //接受5个数据，返回
    {
      get { return 0xE5; }
    }
    public byte REC_LEN_6             //接受6个数据，返回
    {
      get { return 0xE6; }
    }
    public byte REC_LEN_7             //接受7个数据，返回
    {
      get { return 0xE7; }
    }
    public byte REC_LEN_8             //接受8个数据，返回
    {
      get { return 0xE8; }
    }
    public byte REC_LEN_9             //接受9个数据，返回
    {
      get { return 0xE9; }
    }
    public byte REC_LEN_10                //接受10个数据，返回
    {
      get { return 0xEA; }
    }
    public byte REC_LEN_11                //接受11个数据，返回
    {
      get { return 0xEB; }
    }
    public byte REC_LEN_12                //接受12个数据，返回
    {
      get { return 0xEC; }
    }
    public byte REC_LEN_13                //接受13个数据，返回
    {
      get { return 0xED; }
    }
    public byte REC_LEN_14                //接受14个数据，返回
    {
      get { return 0xEE; }
    }
    public byte REC_LEN_15                //接受15个数据，返回
    {
      get { return 0xEF; }
    }
    public byte RECEIVE               //连续接受  F0  开始时回传开始接受信号，然后长期等待接受，接到数据实时回传，
    {
      get { return 0xF0; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  ComBox记录信息和当前状态种类定义
    ///////////////////////////////////////////////////////////////////////////////

    public abstract byte GET_TIME { get; }          //得到时间设定    DD 返回字节时间、等待发送时间、链路保持时间、字节超时时间、接受超时时间
    public abstract byte GET_SET { get; }           //得到链路设定  DE 返回链路控制字(3字节)、通讯波特率
    public abstract byte GET_PORT { get; }          //得到端口设置  DF 返回端口p0，p1，p2，p3

    public abstract byte DISCONNECT_COMM { get; }           //没有连接上串口
    public abstract byte DISCONNECT_COMMBOX { get; }         //没有连接上COMMBOX设备
    public abstract byte COMMBOXID_ERR { get; }         //COMMBOX ID错误

    public byte SK0
    {
      get { return 0; }
    }
    public byte SK1
    {
      get { return 1; }
    }
    public byte SK2
    {
      get { return 2; }
    }
    public byte SK3
    {
      get { return 3; }
    }
    public byte SK4
    {
      get { return 4; }
    }
    public byte SK5
    {
      get { return 5; }
    }
    public byte SK6
    {
      get { return 6; }
    }
    public byte SK7
    {
      get { return 7; }
    }
    public byte SK_NO
    {
      get { return 0xFF; }
    }
    public byte RK0
    {
      get { return 0; }
    }
    public byte RK1
    {
      get { return 1; }
    }
    public byte RK2
    {
      get { return 2; }
    }
    public byte RK3
    {
      get { return 3; }
    }
    public byte RK4
    {
      get { return 4; }
    }
    public byte RK5
    {
      get { return 5; }
    }
    public byte RK6
    {
      get { return 6; }
    }
    public byte RK7
    {
      get { return 7; }
    }
    public byte RK_NO
    {
      get { return 0xFF; }
    }
  }
}
