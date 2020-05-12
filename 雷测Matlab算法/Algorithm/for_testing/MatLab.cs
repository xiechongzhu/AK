/*
* MATLAB Compiler: 7.0.1 (R2019a)
* Date: Tue May 12 16:00:41 2020
* Arguments:
* "-B""macro_default""-W""dotnet:Algorithm,MatLab,4.0,private""-T""link:lib""-d""D:\Git\AK
* \雷测Matlab算法\Algorithm\for_testing""-v""class{MatLab:D:\Git\AK\雷测Matlab算法\calc_co
* nst_launch.m,D:\Git\AK\雷测Matlab算法\calc_target_ld.m,D:\Git\AK\雷测Matlab算法\calc_tar
* get_yc.m}"
*/
using System;
using System.Reflection;
using System.IO;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;

#if SHARED
[assembly: System.Reflection.AssemblyKeyFile(@"")]
#endif

namespace Algorithm
{

  /// <summary>
  /// The MatLab class provides a CLS compliant, MWArray interface to the MATLAB
  /// functions contained in the files:
  /// <newpara></newpara>
  /// D:\Git\AK\雷测Matlab算法\calc_const_launch.m
  /// <newpara></newpara>
  /// D:\Git\AK\雷测Matlab算法\calc_target_ld.m
  /// <newpara></newpara>
  /// D:\Git\AK\雷测Matlab算法\calc_target_yc.m
  /// </summary>
  /// <remarks>
  /// @Version 4.0
  /// </remarks>
  public class MatLab : IDisposable
  {
    #region Constructors

    /// <summary internal= "true">
    /// The static constructor instantiates and initializes the MATLAB Runtime instance.
    /// </summary>
    static MatLab()
    {
      if (MWMCR.MCRAppInitialized)
      {
        try
        {
          Assembly assembly= Assembly.GetExecutingAssembly();

          string ctfFilePath= assembly.Location;

          int lastDelimiter= ctfFilePath.LastIndexOf(@"\");

          ctfFilePath= ctfFilePath.Remove(lastDelimiter, (ctfFilePath.Length - lastDelimiter));

          string ctfFileName = "Algorithm.ctf";

          Stream embeddedCtfStream = null;

          String[] resourceStrings = assembly.GetManifestResourceNames();

          foreach (String name in resourceStrings)
          {
            if (name.Contains(ctfFileName))
            {
              embeddedCtfStream = assembly.GetManifestResourceStream(name);
              break;
            }
          }
          mcr= new MWMCR("",
                         ctfFilePath, embeddedCtfStream, true);
        }
        catch(Exception ex)
        {
          ex_ = new Exception("MWArray assembly failed to be initialized", ex);
        }
      }
      else
      {
        ex_ = new ApplicationException("MWArray assembly could not be initialized");
      }
    }


    /// <summary>
    /// Constructs a new instance of the MatLab class.
    /// </summary>
    public MatLab()
    {
      if(ex_ != null)
      {
        throw ex_;
      }
    }


    #endregion Constructors

    #region Finalize

    /// <summary internal= "true">
    /// Class destructor called by the CLR garbage collector.
    /// </summary>
    ~MatLab()
    {
      Dispose(false);
    }


    /// <summary>
    /// Frees the native resources associated with this object
    /// </summary>
    public void Dispose()
    {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary internal= "true">
    /// Internal dispose function
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        disposed= true;

        if (disposing)
        {
          // Free managed resources;
        }

        // Free native resources
      }
    }


    #endregion Finalize

    #region Methods

    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the calc_const_launch
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 计算发射点常量
    /// 输入
    /// geo0,发射点纬经高
    /// A0，发射点方位角
    /// 输出
    /// R0,发射地心距，单位m
    /// R0_f,中间量
    /// xyz_e0,发射点的地心系坐标
    /// C_e2f,中间转换矩阵
    /// C_fe2,中间转换矩阵
    /// we_f
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_const_launch()
    {
      return mcr.EvaluateFunction("calc_const_launch", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the calc_const_launch
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 计算发射点常量
    /// 输入
    /// geo0,发射点纬经高
    /// A0，发射点方位角
    /// 输出
    /// R0,发射地心距，单位m
    /// R0_f,中间量
    /// xyz_e0,发射点的地心系坐标
    /// C_e2f,中间转换矩阵
    /// C_fe2,中间转换矩阵
    /// we_f
    /// </remarks>
    /// <param name="geo0">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_const_launch(MWArray geo0)
    {
      return mcr.EvaluateFunction("calc_const_launch", geo0);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the calc_const_launch
    /// MATLAB function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 计算发射点常量
    /// 输入
    /// geo0,发射点纬经高
    /// A0，发射点方位角
    /// 输出
    /// R0,发射地心距，单位m
    /// R0_f,中间量
    /// xyz_e0,发射点的地心系坐标
    /// C_e2f,中间转换矩阵
    /// C_fe2,中间转换矩阵
    /// we_f
    /// </remarks>
    /// <param name="geo0">Input argument #1</param>
    /// <param name="A0">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_const_launch(MWArray geo0, MWArray A0)
    {
      return mcr.EvaluateFunction("calc_const_launch", geo0, A0);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the calc_const_launch MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 计算发射点常量
    /// 输入
    /// geo0,发射点纬经高
    /// A0，发射点方位角
    /// 输出
    /// R0,发射地心距，单位m
    /// R0_f,中间量
    /// xyz_e0,发射点的地心系坐标
    /// C_e2f,中间转换矩阵
    /// C_fe2,中间转换矩阵
    /// we_f
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_const_launch(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_const_launch", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the calc_const_launch MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 计算发射点常量
    /// 输入
    /// geo0,发射点纬经高
    /// A0，发射点方位角
    /// 输出
    /// R0,发射地心距，单位m
    /// R0_f,中间量
    /// xyz_e0,发射点的地心系坐标
    /// C_e2f,中间转换矩阵
    /// C_fe2,中间转换矩阵
    /// we_f
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="geo0">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_const_launch(int numArgsOut, MWArray geo0)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_const_launch", geo0);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the calc_const_launch MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 计算发射点常量
    /// 输入
    /// geo0,发射点纬经高
    /// A0，发射点方位角
    /// 输出
    /// R0,发射地心距，单位m
    /// R0_f,中间量
    /// xyz_e0,发射点的地心系坐标
    /// C_e2f,中间转换矩阵
    /// C_fe2,中间转换矩阵
    /// we_f
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="geo0">Input argument #1</param>
    /// <param name="A0">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_const_launch(int numArgsOut, MWArray geo0, MWArray A0)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_const_launch", geo0, A0);
    }


    /// <summary>
    /// Provides an interface for the calc_const_launch function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// 计算发射点常量
    /// 输入
    /// geo0,发射点纬经高
    /// A0，发射点方位角
    /// 输出
    /// R0,发射地心距，单位m
    /// R0_f,中间量
    /// xyz_e0,发射点的地心系坐标
    /// C_e2f,中间转换矩阵
    /// C_fe2,中间转换矩阵
    /// we_f
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void calc_const_launch(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("calc_const_launch", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld()
    {
      return mcr.EvaluateFunction("calc_target_ld", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now, MWArray R0)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now, R0);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now, MWArray R0, MWArray 
                            R0_f)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now, R0, R0_f);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now, MWArray R0, MWArray 
                            R0_f, MWArray C_e2f)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now, MWArray R0, MWArray 
                            R0_f, MWArray C_e2f, MWArray C_fe2)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now, MWArray R0, MWArray 
                            R0_f, MWArray C_e2f, MWArray C_fe2, MWArray we_f)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2, we_f);
    }


    /// <summary>
    /// Provides a single output, 8-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <param name="xyz_e0">Input argument #8</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now, MWArray R0, MWArray 
                            R0_f, MWArray C_e2f, MWArray C_fe2, MWArray we_f, MWArray 
                            xyz_e0)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2, we_f, xyz_e0);
    }


    /// <summary>
    /// Provides a single output, 9-input MWArrayinterface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <param name="xyz_e0">Input argument #8</param>
    /// <param name="h_end">Input argument #9</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_ld(MWArray lambda0, MWArray nav_now, MWArray R0, MWArray 
                            R0_f, MWArray C_e2f, MWArray C_fe2, MWArray we_f, MWArray 
                            xyz_e0, MWArray h_end)
    {
      return mcr.EvaluateFunction("calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2, we_f, xyz_e0, h_end);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now, 
                              MWArray R0)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now, R0);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now, 
                              MWArray R0, MWArray R0_f)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now, R0, R0_f);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now, 
                              MWArray R0, MWArray R0_f, MWArray C_e2f)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now, 
                              MWArray R0, MWArray R0_f, MWArray C_e2f, MWArray C_fe2)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now, 
                              MWArray R0, MWArray R0_f, MWArray C_e2f, MWArray C_fe2, 
                              MWArray we_f)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2, we_f);
    }


    /// <summary>
    /// Provides the standard 8-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <param name="xyz_e0">Input argument #8</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now, 
                              MWArray R0, MWArray R0_f, MWArray C_e2f, MWArray C_fe2, 
                              MWArray we_f, MWArray xyz_e0)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2, we_f, xyz_e0);
    }


    /// <summary>
    /// Provides the standard 9-input MWArray interface to the calc_target_ld MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="lambda0">Input argument #1</param>
    /// <param name="nav_now">Input argument #2</param>
    /// <param name="R0">Input argument #3</param>
    /// <param name="R0_f">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <param name="xyz_e0">Input argument #8</param>
    /// <param name="h_end">Input argument #9</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_ld(int numArgsOut, MWArray lambda0, MWArray nav_now, 
                              MWArray R0, MWArray R0_f, MWArray C_e2f, MWArray C_fe2, 
                              MWArray we_f, MWArray xyz_e0, MWArray h_end)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_ld", lambda0, nav_now, R0, R0_f, C_e2f, C_fe2, we_f, xyz_e0, h_end);
    }


    /// <summary>
    /// Provides an interface for the calc_target_ld function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// 雷达数据预示落点计算
    /// 输入
    /// nav_now,1 x，2 y，3 z,4 速度x(m/s),5 速度y(m/s)，6 速度z(m/s)，7初始经度（°）
    /// R0_f,
    /// R0,
    /// xyz_e0
    /// C_e2f,
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void calc_target_ld(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("calc_target_ld", numArgsOut, ref argsOut, argsIn);
    }


    /// <summary>
    /// Provides a single output, 0-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc()
    {
      return mcr.EvaluateFunction("calc_target_yc", new MWArray[]{});
    }


    /// <summary>
    /// Provides a single output, 1-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now);
    }


    /// <summary>
    /// Provides a single output, 2-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now, MWArray R0)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now, R0);
    }


    /// <summary>
    /// Provides a single output, 3-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now, MWArray R0, MWArray R0_f)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now, R0, R0_f);
    }


    /// <summary>
    /// Provides a single output, 4-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now, MWArray R0, MWArray R0_f, MWArray 
                            xyz_e0)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now, R0, R0_f, xyz_e0);
    }


    /// <summary>
    /// Provides a single output, 5-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now, MWArray R0, MWArray R0_f, MWArray 
                            xyz_e0, MWArray C_e2f)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f);
    }


    /// <summary>
    /// Provides a single output, 6-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now, MWArray R0, MWArray R0_f, MWArray 
                            xyz_e0, MWArray C_e2f, MWArray C_fe2)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f, C_fe2);
    }


    /// <summary>
    /// Provides a single output, 7-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now, MWArray R0, MWArray R0_f, MWArray 
                            xyz_e0, MWArray C_e2f, MWArray C_fe2, MWArray we_f)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f, C_fe2, we_f);
    }


    /// <summary>
    /// Provides a single output, 8-input MWArrayinterface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <param name="h_end">Input argument #8</param>
    /// <returns>An MWArray containing the first output argument.</returns>
    ///
    public MWArray calc_target_yc(MWArray nav_now, MWArray R0, MWArray R0_f, MWArray 
                            xyz_e0, MWArray C_e2f, MWArray C_fe2, MWArray we_f, MWArray 
                            h_end)
    {
      return mcr.EvaluateFunction("calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f, C_fe2, we_f, h_end);
    }


    /// <summary>
    /// Provides the standard 0-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", new MWArray[]{});
    }


    /// <summary>
    /// Provides the standard 1-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now);
    }


    /// <summary>
    /// Provides the standard 2-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now, MWArray R0)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now, R0);
    }


    /// <summary>
    /// Provides the standard 3-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now, MWArray R0, MWArray 
                              R0_f)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now, R0, R0_f);
    }


    /// <summary>
    /// Provides the standard 4-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now, MWArray R0, MWArray 
                              R0_f, MWArray xyz_e0)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now, R0, R0_f, xyz_e0);
    }


    /// <summary>
    /// Provides the standard 5-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now, MWArray R0, MWArray 
                              R0_f, MWArray xyz_e0, MWArray C_e2f)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f);
    }


    /// <summary>
    /// Provides the standard 6-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now, MWArray R0, MWArray 
                              R0_f, MWArray xyz_e0, MWArray C_e2f, MWArray C_fe2)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f, C_fe2);
    }


    /// <summary>
    /// Provides the standard 7-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now, MWArray R0, MWArray 
                              R0_f, MWArray xyz_e0, MWArray C_e2f, MWArray C_fe2, MWArray 
                              we_f)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f, C_fe2, we_f);
    }


    /// <summary>
    /// Provides the standard 8-input MWArray interface to the calc_target_yc MATLAB
    /// function.
    /// </summary>
    /// <remarks>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return.</param>
    /// <param name="nav_now">Input argument #1</param>
    /// <param name="R0">Input argument #2</param>
    /// <param name="R0_f">Input argument #3</param>
    /// <param name="xyz_e0">Input argument #4</param>
    /// <param name="C_e2f">Input argument #5</param>
    /// <param name="C_fe2">Input argument #6</param>
    /// <param name="we_f">Input argument #7</param>
    /// <param name="h_end">Input argument #8</param>
    /// <returns>An Array of length "numArgsOut" containing the output
    /// arguments.</returns>
    ///
    public MWArray[] calc_target_yc(int numArgsOut, MWArray nav_now, MWArray R0, MWArray 
                              R0_f, MWArray xyz_e0, MWArray C_e2f, MWArray C_fe2, MWArray 
                              we_f, MWArray h_end)
    {
      return mcr.EvaluateFunction(numArgsOut, "calc_target_yc", nav_now, R0, R0_f, xyz_e0, C_e2f, C_fe2, we_f, h_end);
    }


    /// <summary>
    /// Provides an interface for the calc_target_yc function in which the input and
    /// output
    /// arguments are specified as an array of MWArrays.
    /// </summary>
    /// <remarks>
    /// This method will allocate and return by reference the output argument
    /// array.<newpara></newpara>
    /// M-Documentation:
    /// 遥测预示落点计算
    /// 输入
    /// nav_now,1 纬度（°），2 经度（°），3 高度（m）,4 东向速度(m/s),5 北向速度(m/s)，
    /// 6 天向速度(m/s)
    /// R0_f
    /// R0
    /// xyz_e0
    /// C_e2f，转换矩阵
    /// C_fe2
    /// we_f
    /// h_end, 落点附近海拔
    /// </remarks>
    /// <param name="numArgsOut">The number of output arguments to return</param>
    /// <param name= "argsOut">Array of MWArray output arguments</param>
    /// <param name= "argsIn">Array of MWArray input arguments</param>
    ///
    public void calc_target_yc(int numArgsOut, ref MWArray[] argsOut, MWArray[] argsIn)
    {
      mcr.EvaluateFunction("calc_target_yc", numArgsOut, ref argsOut, argsIn);
    }



    /// <summary>
    /// This method will cause a MATLAB figure window to behave as a modal dialog box.
    /// The method will not return until all the figure windows associated with this
    /// component have been closed.
    /// </summary>
    /// <remarks>
    /// An application should only call this method when required to keep the
    /// MATLAB figure window from disappearing.  Other techniques, such as calling
    /// Console.ReadLine() from the application should be considered where
    /// possible.</remarks>
    ///
    public void WaitForFiguresToDie()
    {
      mcr.WaitForFiguresToDie();
    }



    #endregion Methods

    #region Class Members

    private static MWMCR mcr= null;

    private static Exception ex_= null;

    private bool disposed= false;

    #endregion Class Members
  }
}
