using System;

namespace DNT.Diag.Commbox.GL.C168
{
  internal class C168Constant : Constant
  {
    ///////////////////////////////////////////////////////
    //设置Commbox宏定义区
    ////////////////////////////////////////////////////////
    public int TIMEVALUE //万分之一秒微妙
    {
      get { return 1000000; }
    }
    public int COMMBOXINFOLEN //共有18个数据需从COMMBOX得到
    {
      get { return 18; }
    }
    public int VERSIONLEN
    {
      get { return 2; }
    }
    public int MINITIMELEN
    {
      get { return 3; }
    }
    public int COMMBOXPORTNUM
    {
      get { return 4; }
    }
    public int COMMBOXIDLEN
    {
      get { return 10; }
    }
    public int MAXIM_BLOCK //命令缓从区的最大数
    {
      get { return 0x40; }
    }
    public override int LINKBLOCK //链路保持的命令缓冲区
    {
      get { return MAXIM_BLOCK; }
    }
    ///////////////////////////////////////////////////////
    // CommBox 固定信息 宏定义表
    ///////////////////////////////////////////////////////
    public byte NULLADD                   //表示此块无使用
    {
      get { return 0xFF; }
    }
    public int SWAPBLOCK      //数据交换区的块表识
    {
      get { return MAXIM_BLOCK + 1; }
    }

    public int START_BAUD   //上位机同下位机通信在复位或上电时波特率为57600
    {
      get { return 57600; }
    }
    public int CMD_DATALEN       //非发送命令最大长度
    {
      get { return 4; }
    }


    ///////////////////////////////////////////////////////
    /*
    //  P1口为通讯口
    public byte DH        0x80                        //高电平输出,1为关闭,0为打开
    public byte DL2       0x40                        //低电平输出,1为关闭,0为打开,正逻辑发送通讯线
    public byte DL1       0x20                        //低电平输出,1为关闭,0为打开,正逻辑发送通讯线,带接受控制
    public byte DL0       0x10                        //低电平输出,1为关闭,0为打开,正逻辑发送通讯线,带接受控制
    public byte PWMS  0x08                        //PWM发送线
    public byte COMS  0x02                        //标准发送通讯线路
    public byte SET_NULL  0x00                    //不选择任何设置

    //P2口为通讯物理控制口
    public byte PWC       0x80                        //通讯电平控制,1为5伏,0为12伏
    public byte REFC  0x40                        //通讯比较电平控制,1为通讯电平1/5,0为比较电平控制1/2
    public byte CK        0x20                        //K线控制开关,1为双线通讯,0为单线通讯
    public byte SZFC  0x10                        //发送逻辑控制,1为负逻辑,0为正逻辑
    public byte RZFC  0x08                        //接受逻辑控制,1为负逻辑,0为正逻辑
    public byte DLC1  0x04                        //DLC1接受控制,1为接受关闭,0为接受打开
    public byte DLC0  0x02                        //DLC0接受控制,1为接受关闭,0为接受打开
    public byte SLC       0x01                        //线选地址锁存器控制线(待用)
    //   P0口选线控制
    public byte CLOSEALL  0x08

    //   通讯控制字设定
    public byte RS_232        0x00                    //通讯控制字1
    public byte EXRS_232  0x20                    //通讯控制字1
    public byte   SET_VPW     0x40                    //通讯控制字1
    public byte SET_PWM     0x60                  //通讯控制字1
    public byte BIT9_SPACE  0x00                  //通讯控制字1
    public byte BIT9_MARK   0x01                  //通讯控制字1
    public byte BIT9_EVEN   0x02                  //通讯控制字1
    public byte BIT9_ODD    0x03                  //通讯控制字1
    public byte SEL_SL        0x00                    //通讯控制字1
    public byte SEL_DL0     0x08                  //通讯控制字1
    public byte SEL_DL1     0x10                  //通讯控制字1
    public byte SEL_DL2     0x18                  //通讯控制字1
    public byte SET_DB20    0x04                  //通讯控制字1
    public byte UN_DB20     0x00                  //通讯控制字1
    public byte ONEBYONE    0x80                  //通讯控制字3
    public byte INVERTBYTE  0x40                  //通讯控制字3
    public byte ORIGNALBYTE 0X00                  //通讯控制字3
    */
    /***************************************************************************
    命令定义区:
    命令分为四类:
    1、写入命令缓冲区命令：
    将以整理好的批处理命令写入缓冲区：格式如下
    命令字 WR_DATA     0xD0?+ 长度（数据[N]+地址） +写入缓冲区地址+命令1+ 命令2。。。+命令N+校验。
    其中命令N：为不含校验的命令，校验方法：为校验和
    命令区存放格式为：长度（数据[N]+地址） +写入缓冲区地址+命令1+ 命令2。。。+命令N
    2、单字节命令：（大于写入命令缓冲区命令字 WR_DATA  0xD0，皆为单字节命令区）
    简称快速命令：格式如下
    命令字+校验和：
    非缓冲区命令：
    其中中断命令2个：停止执行，停止接受
    软件复位，得到命令缓冲区数据，得到链路保持数据，得到上次缓冲区命令的数据
    缓冲区命令：
    1、缓冲区数据操作命令。
    2、开关命令
    3、链路保持命令
    4、接受命令
    3、多字节命令：（命令空间 0x30-0xCF）
    格式如下：命令字（6BIT）+长度（数据长度-1；2BIT）+数据[N]+校验和
    1、设置命令
    2、数据操作命令

    4、发送命令：（命令空间 0x00-0x2F）
    格式如下：
    长度（数据[N]+1）+数据[N]+校验和
    发送命令在写入缓从区时长度可以有0x2F，有0x30个数据，但不写入缓冲区直接发送，追多不超过4个
    5、中断命令2个：停止执行，停止接受
    发送命令字，无校验，仅为一个字节，无运行返回，以等待运行结果标志返回。

    ***************************************************************************/
    //  1、写入命令缓冲区命令：
    public override byte WR_DATA                   //写缓冲区命令字,写入数据到命令缓冲区
    {
      get { return 0xD0; }
    }
    public override byte WR_LINK                   //若写入命令的地址为WR_LINK ，写入数据到链路保持区
    {
      get { return 0xFF; }
    }
    //链路保持区存放在命令缓冲区最后,存放次序:按地址从低到高
    public int SEND_LEN                   //一次发送数据的数据长度,0X70个数据
    {
      get { return 0xA8; }
    }
    public override byte SEND_DATA
    {
      get { return 0x00; }
    }

    //  2、单字节命令：（大于写入命令缓冲区命令字 WR_DATA    0xD0，皆为单字节命令区）

    //  非缓冲区命令
    public override byte RESET             //软件复位命令  清除所有缓冲区和寄存器内容。
    {
      get { return 0xF3; }
    }
    public byte GETINFO
    {
      get { return 0xF4; }
    }
    /*
    得到CPU速度     F9         返回CPU的指令执行时间（按纳秒计，数值传递，3个字节）
    和时间控制参数             返回时间控制的指令执行数：
    其他控制（1byte）（DB20）
    长等待控制（1byte）（DB200）
    缓冲区长度（1byte）
    产品序号（10byte）
    和版本信息                  返回Commbox的硬件版本号。
    等待接受5字节密码：（第五个字节为校验和）同公钥循环与或的校验和，返回命令增值。

    */
    /*
    public byte GET_TIME      0xF5            //得到时间设定    DD 返回字节时间、等待发送时间、链路保持时间、字节超时时间、接受超时时间
    public byte GET_SET           0xF6            //得到链路设定  DE 返回链路控制字(3字节)、通讯波特率
    public byte GET_PORT      0xF7            //得到端口设置  DF 返回端口p0，p1，p2，p3
    public byte GET_LINKDATA  0xF8            //得到链路数据 FC 返回链路保持命令块中的所有内容 (中断命令)
    public byte GET_BUFFDATA    0xF9          //得到缓冲器数据 FD 返回整个缓冲区数据 (中断命令)
    public byte GET_CMMAND      0xFA          //得到命令数据 FE 返回上一执行命令。 (中断命令)
    */
    //中断命令定义
    public override byte STOP_REC          //中断接受命令  强行退出当前接受命令，不返回错误。(中断命令)
    {
      get { return 0xFB; }
    }
    public override byte STOP_EXECUTE          //中断批处理命令   在当前执行时，通过该命令停止当前接受操作，返回错误。(中断命令)
    {
      get { return 0xFC; }
    }

    //  单字节缓冲区命令
    //public byte GET_PORT1    0xD8               //等到通讯口的当前状态

    public override byte SET_ONEBYONE              //将原有的接受一个发送一个的标志翻转
    {
      get { return 0xD9; }
    }
    /*
    public byte SET55_BAUD   0xDA             //计算0x55的波特率
    public byte REC_FR       0xE0             //接受一帧命令  E0 开始时回传开始接受信号，然后长期等待接受，接到数据实时回传，
    //待中断当前命令和中断处理命令，当接受的字节超过字节间的最大时间，自动正常退出。
    //若设定了长度接受,超时最长等待时间,自动返回.
    public byte REC_LEN      0xE1             //接受长度数据    E1-EF 开始时回传开始接受信号，接受命令字节低四位为长度的数据自动退出，
    //接到数据实时回传，待中断当前命令和中断处理命令，接受一个字节超过最长等待时间,正常退出.
    public byte RECIEVE      0xF0             //连续接受  F0  开始时回传开始接受信号，然后长期等待接受，接到数据实时回传，
    //直到接受中断当前命令和中断处理命令。
    */
    public byte RUNLINK               //启动链路保持   F1 启动链路保持，定时执行链路保持内容，在每次执行前回传链路保持
    {
      get { return 0xF1; }
    }
    //开始信号，结束时回传链路保持结束信号。
    public byte STOPLINK              //中断链路保持   F2 结束链路保持执行。
    {
      get { return 0xF2; }
    }
    public byte CLR_LINK              //清除链路保持缓冲区
    {
      get { return 0xDE; }
    }

    public byte DO_BAT_00             //批处理命令，执行一次命令缓冲区00地址的命令区
    {
      get { return 0xDF; }
    }

    // 3、多字节命令：（命令空间 0x30-0xCF）
    public byte D0_BAT                //批处理命令，连续执行一次最多4块命令缓冲区的地址命令区；数据最多为4个命令区的首地址
    {
      get { return 0x78; }
    }
    public byte D0_BAT_FOR                //批处理命令，连续执行无数次最多4块命令缓冲区的地址命令区；数据最多为4个命令区的首地址
    {
      get { return 0x7c; }
    }

    //多字节命令
    public byte SETTING               //下位机通讯链路状态字设定：设定3个通讯控制字，无用设定或没有设定都自动清零
    {
      get { return 0x80; }
    }
    public byte SETBAUD               //通讯波特率设定，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0x84; }
    }
    /*
    public byte SETBYTETIME 0x88              //字节间时间设定 db20?（vpw为指令数） ，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    public byte SETWAITTIME 0x8c              //空闲等待时间设定 db20?，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    public byte SETLINKTIME 0x90              //链路保持时建设定 db20?，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    public byte SETRECBBOUT 0x94              //接受字节超时错误判断 db20（vpw为指令数） ，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    public byte SETRECFROUT 0x98              //接受一帧超时错误判断?db20?，只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    */
    public byte ECHO              //回传指定数据，按序回传数据
    {
      get { return 0x9c; }
    }

    public byte SETPORT0              //只有一个字节的数据，设定端口0
    {
      get { return 0xa0; }
    }
    public byte SETPORT1              //只有一个字节的数据，设定端口1
    {
      get { return 0xa4; }
    }
    public byte SETPORT2              //只有一个字节的数据，设定端口2
    {
      get { return 0xa8; }
    }
    public byte SETPORT3              //只有一个字节的数据，设定端口3
    {
      get { return 0xac; }
    }
    //已删除public byte SETALLPORT  0x6F             //只有四个字节的数据，设定端口0，1，2，3

    public override byte SET_UPBAUD                //设置上位机的通讯波特率 ,仅有数据位1位,定义如下:其他非法
    {
      get { return 0xb0; }
    }
    /*
    public byte UP_9600BPS   0x00
    public byte UP_19200BPS  0x01
    public byte UP_38400BPS  0x02
    public byte UP_57600BPS  0x03
    public byte UP_115200BPS 0x04
    */
    public override byte DELAYSHORT                //设定延时时间 (DB20)只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0xb4; }
    }
    public override byte DELAYTIME             //设定延时时间 (DB20)只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0xb8; }
    }
    public byte DELAYLONG             //设定延时时间 (DB200) 只用2个数据位，单字节为低字节，双字节高字节在前，低字节在后。
    {
      get { return 0xbC; }
    }

    //Operat Buff CMD
    //指定修改
    /*
    public byte UPDATE_1BYTE 0xc1             //81 结果地址  数据1                  结果地址=数据1
    public byte UPDATE_2BYTE 0xc3             //83 结果地址1 数据1 结果地址2 数据2    结果地址1=数据1 结果地址2=数据2
    //数据拷贝
    public byte COPY_DATA    0xcA             //8A 结果地址1  操作地址1 长度            COPY 操作地址1 TO 结果地址1 FOR 长度 字节
    //自增命令
    public byte DEC_DATA     0xc4             //84 结果地址                           结果地址=结果地址-1
    public byte INC_DATA     0xc0             //80 结果地址                           结果地址=结果地址+1
    public byte INC_2DATA    0xc5             //85 结果地址1 结果地址2                结果地址1=结果地址1+1 结果地址2=结果地址2+1
    //加法命令
    public byte ADD_1BYTE    0xc2             //82 结果地址  操作地址1 数据1        结果地址=操作地址1+数据1
    public byte ADD_2BYTE  0xc7               //87 结果地址1 结果地址2 数据1  数据2   结果地址1=结果地址1+数据1 结果地址2=结果地址2+数据2
    public byte ADD_DATA     0xc6                 //86 结果地址1 操作地址1 操作地址2      结果地址1=操作地址1+操作地址2
    //减法命令
    public byte SUB_DATA   0xce           //8E 结果地址1 操作地址1 操作地址2      结果地址1=操作地址1-操作地址2
    public byte SUB_BYTE     0xcD             //8D 结果地址1 数据1                  结果地址1=数据1-结果地址1
    public byte INVERT_DATA  0xcC             //8C 结果地址1                          结果地址1=~结果地址
    //取数据
    public byte GET_NDATA    0xc9             //88 地址                             返回数据缓冲区指定的数据


    public byte UPDATE_1BYTE_A    0xc0            //81 结果地址  数据1                  结果地址=数据1
    public byte UPDATE_2BYTE_A    0xc0            //83 结果地址1 数据1 结果地址2 数据2    结果地址1=数据1 结果地址2=数据2
    //自增命令
    public byte DEC_DATA_A        0xc4            //84 结果地址                           结果地址=结果地址-1
    public byte INC_DATA_A        0xc0            //80 结果地址                           结果地址=结果地址+1
    public byte INC_2DATA_A       0xc4            //85 结果地址1 结果地址2                结果地址1=结果地址1+1 结果地址2=结果地址2+1
    //加法命令
    public byte ADD_1BYTE_A       0xc0            //82 结果地址  操作地址1 数据1        结果地址=操作地址1+数据1
    public byte ADD_2BYTE_A       0xc4            //87 结果地址1 结果地址2 数据1  数据2   结果地址1=结果地址1+数据1 结果地址2=结果地址2+数据2
    public byte ADD_DATA_A        0xc4            //86 结果地址1 操作地址1 操作地址2      结果地址1=操作地址1+操作地址2
    //减法命令
    public byte SUB_DATA_A        0xcc            //8E 结果地址1 操作地址1 操作地址2      结果地址1=操作地址1-操作地址2
    public byte SUB_BYTE_A        0xcc            //8D 结果地址1 数据1                  结果地址1=数据1-结果地址1
    public byte INVERT_DATA_A 0xcC            //8C 结果地址1                          结果地址1=~结果地址
    */
    //取数据
    public byte GET_DATA          //88 地址                             返回数据缓冲区指定的数据
    {
      get { return 0xc8; }
    }

    /***************************************************************************
    返回命令定义区:
    返回命令分为两类:
    1 单字节返回:无长度和校验,仅返回单字节
    1 错误,成功信息:
    2 接受的数据:(接受数据,通讯端口数据)
    使用于缓冲区命令
    2 多字节返回:
    1 格式:接受的命令字 + 长度 + 数据 + 校验和
    长度：仅包含数据个数
    使用于非缓冲区命令
    3 中断命令不返回：以执行结果返回

    ***************************************************************************/
    /*
    // 1 单字节返回:无长度和校验,仅返回单字节
    //接受返回错误信息定义
    public byte UP_TIMEOUT 0xC0               //接受命令超时错误
    public byte UP_DATAEER 0xC1               //接受命令数据错误
    public byte OVER_BUFF  0xC2               //批处理缓冲区溢出,不判断链路保持数据是否会破坏缓冲区数据,
    //仅判断数据长度+数据地址>链路保持的开始位置成立溢出.
    public byte ERROR_REC  0xC3               //其他接受错误

    //执行操作错误
    public byte SUCCESS    0xAA               //执行成功
    public byte RUN_ERR    0xC4               //运行启动检测错误
    */

    //RF多对一的设定接口
    public byte SETDTR_L
    {
      get { return 0x02; }
    }
    public byte SETDTR_H
    {
      get { return 0x03; }
    }
    public byte MAX_RFADD                     //0x00-0x2F间的0x30个地址
    {
      get { return 0x2F; }
    }
    public byte SETADD                        //切换无线通讯设备到新地址
    {
      get { return 0x10; }
    }
    public byte CHANGADD                      //改变当前与之通讯的无线设备的地址
    {
      get { return 0x40; }
    }

    public byte SETRFBAUD                     //改变无线串口通讯波特率
    {
      get { return 0x04; }
    }
    public byte RESET_RF                      //复位无线通讯主设备，该命令需在9600波特率下实现
    {
      get { return 0x00; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  上下位机通讯波特率
    ///////////////////////////////////////////////////////////////////////////////

    public byte UP_9600BPS
    {
      get { return 0x00; }
    }
    public byte UP_19200BPS
    {
      get { return 0x01; }
    }
    public byte UP_38400BPS
    {
      get { return 0x02; }
    }
    public byte UP_57600BPS
    {
      get { return 0x03; }
    }
    public byte UP_115200BPS
    {
      get { return 0x04; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  操作数据缓冲区
    ///////////////////////////////////////////////////////////////////////////////
    //数据拷贝
    public byte COPY_DATA             //8A 结果地址1  操作地址1 长度            COPY 操作地址1 TO 结果地址1 FOR 长度 字节
    {
      get { return 0xCA; }
    }
    //修改数据
    public byte UPDATE_1BYTE              //81 结果地址  数据1                  结果地址=数据1
    {
      get { return 0xC1; }
    }
    public byte UPDATE_2BYTE              //83 结果地址1 数据1 结果地址2 数据2    结果地址1=数据1 结果地址2=数据2
    {
      get { return 0xC3; }
    }
    //自增命令
    public byte DEC_DATA              //84 结果地址                           结果地址=结果地址-1
    {
      get { return 0xC4; }
    }
    public byte INC_DATA              //80 结果地址                           结果地址=结果地址+1
    {
      get { return 0xC0; }
    }
    public byte INC_2DATA             //85 结果地址1 结果地址2                结果地址1=结果地址1+1 结果地址2=结果地址2+1
    {
      get { return 0xC5; }
    }
    //加法命令
    public byte ADD_1BYTE             //82 结果地址  操作地址1 数据1        结果地址=操作地址1+数据1
    {
      get { return 0xC2; }
    }
    public byte ADD_2BYTE             //87 结果地址1 数据1  结果地址2 数据2   结果地址1=结果地址1+数据1 结果地址2=结果地址2+数据2
    {
      get { return 0xC7; }
    }
    public byte ADD_DATA              //86 结果地址1 操作地址1 操作地址2      结果地址1=操作地址1+操作地址2
    {
      get { return 0xC6; }
    }
    //减法命令
    public byte SUB_DATA              //8E 结果地址1 操作地址1 操作地址2      结果地址1=操作地址1-操作地址2
    {
      get { return 0xCE; }
    }
    public override byte SUB_BYTE              //8D 结果地址1 数据1                  结果地址1=数据1-结果地址1
    {
      get { return 0xCD; }
    }
    public byte INVERT_DATA               //8C 结果地址1                          结果地址1=~结果地址
    {
      get { return 0xCC; }
    }


    ///////////////////////////////////////////////////////////////////////////////
    //  接受命令类型定义
    ///////////////////////////////////////////////////////////////////////////////

    public override byte GET_PORT1             //等到通讯口的当前状态
    {
      get { return 0xD8; }
    }
    public override byte SET55_BAUD                //计算0x55的波特率
    {
      get { return 0xDA; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    //  ComBox记录信息和当前状态种类定义
    ///////////////////////////////////////////////////////////////////////////////

    public override byte GET_TIME          //得到时间设定    DD 返回字节时间、等待发送时间、链路保持时间、字节超时时间、接受超时时间
    {
      get { return 0xF5; }
    }
    public override byte GET_SET           //得到链路设定  DE 返回链路控制字(3字节)、通讯波特率
    {
      get { return 0xF6; }
    }
    public override byte GET_PORT          //得到端口设置  DF 返回端口p0，p1，p2，p3
    {
      get { return 0xF7; }
    }
    public byte GET_LINKDATA          //得到链路数据 FC 返回链路保持命令块中的所有内容 (中断命令)
    {
      get { return 0xF8; }
    }
    public byte GET_BUFFDATA          //得到缓冲器数据 FD 返回整个缓冲区数据 (中断命令)
    {
      get { return 0xF9; }
    }
    public byte GET_CMMAND            //得到命令数据 FE 返回上一执行命令。 (中断命令)
    {
      get { return 0xFA; }
    }

    ///////////////////////////////////////////////////////////////////////////////
    // 返回失败时，可根据Error_Record的值查找错误表定义
    ///////////////////////////////////////////////////////////////////////////////

    public byte ILLIGICAL_LEN             //设置命令数据非法超长
    {
      get { return 0xFF; }
    }
    public byte NOBUFF_TOSEND             //无交换缓冲区用于发送数据存放
    {
      get { return 0xFE; }
    }
    public byte SENDDATA_ERROR                //上位机发送数据异常
    {
      get { return 0xFD; }
    }
    public byte CHECKSUM_ERROR                //接受命令回复校验和出错
    {
      get { return 0xFC; }
    }
    public byte TIMEOUT_ERROR             //接受数据超时错误
    {
      get { return 0xFB; }
    }
    public byte LOST_VERSIONDATA          //读到的Commbox数据长度不够.
    {
      get { return 0xFA; }
    }
    public byte ILLIGICAL_CMD         //无此操作功能,没有定义.
    {
      get { return 0xF9; }
    }
    public override byte DISCONNECT_COMM           //没有连接上串口
    {
      get { return 0xF8; }
    }
    public override byte DISCONNECT_COMMBOX         //没有连接上COMMBOX设备
    {
      get { return 0xF7; }
    }
    public byte NODEFINE_BUFF         //没有此命令块存在,未定义
    {
      get { return 0xF6; }
    }
    public byte APPLICATION_NOW           //现有缓冲区申请,未取消,不能再此申请
    {
      get { return 0xF5; }
    }
    public byte BUFFBUSING            //此缓冲区有数据未被撤销,不能使用,需删除此缓冲区,方可使用
    {
      get { return 0xF4; }
    }
    public byte BUFFFLOW          //整个缓冲区无可使用的空间,不能申请,需删除缓冲区释放空间,方可使用
    {
      get { return 0xF3; }
    }
    public byte NOAPPLICATBUFF            //未申请错误,需先申请,方可使用
    {
      get { return 0xF2; }
    }
    public byte UNBUFF_CMD            //不是缓冲区命令,不能加载
    {
      get { return 0xF1; }
    }
    public byte NOUSED_BUFF           //该缓冲区现没有使用,删除无效
    {
      get { return 0xF0; }
    }
    public byte KEEPLINK_ERROR            //链路保持已断线
    {
      get { return 0xEF; }
    }
    public byte UNDEFINE_CMD          //无效命令,未曾定义
    {
      get { return 0xEE; }
    }
    public byte UNSET_EXRSBIT         //没有设定扩展RS232的接受数据位个数
    {
      get { return 0xED; }
    }
    public byte COMMBAUD_OUT          //按照定义和倍增标志计算通讯波特率超出范围
    {
      get { return 0xEC; }
    }
    public byte COMMTIME_OUT          //按照定义和倍增标志计算通讯时间超出范围
    {
      get { return 0xEB; }
    }
    public byte OUTADDINBUFF          //缓冲区寻址越界
    {
      get { return 0xEA; }
    }
    public byte COMMTIME_ZERO         //commbox时间基数为零
    {
      get { return 0xE9; }
    }
    public byte SETTIME_ERROR         //延时时间为零
    {
      get { return 0xE8; }
    }
    public byte NOADDDATA         //没有向申请的缓冲区填入命令,申请的缓冲区被撤销
    {
      get { return 0xE7; }
    }
    public byte TESTNOLINK            //选择的线路没有连通
    {
      get { return 0xE6; }
    }
    public byte PORTLEVELIDLE         //端口电平为常态
    {
      get { return 0xE5; }
    }
    public override byte COMMBOXID_ERR         //COMMBOX ID错误
    {
      get { return 0xE4; }
    }

    public byte UP_TIMEOUT            //COMMBOX接受命令超时错误
    {
      get { return 0xC0; }
    }
    public byte UP_DATAEER            //COMMBOX接受命令数据错误
    {
      get { return 0xC1; }
    }
    public byte OVER_BUFF             //COMMBOX批处理缓冲区溢出,不判断链路保持数据是否会破坏缓冲区数据,
    {
      get { return 0xC2; }
    }
    //仅判断数据长度+数据地址>链路保持的开始位置成立溢出.
    public byte ERROR_REC             //COMMBOX其他接受错误
    {
      get { return 0xC3; }
    }
    //COMMBOX执行操作错误
    public byte SUCCESS               //COMMBOX执行成功
    {
      get { return 0xAA; }
    }
    public byte SEND_OK
    {
      get { return 0x55; }
    }
    public byte RF_ERR
    {
      get { return 0xC8; }
    }
    public byte RUN_ERR               //COMMBOX运行启动检测错误
    {
      get { return 0xC4; }
    }
    public byte SEND_CMD
    {
      get { return 0x01; }
    }
  }
}
