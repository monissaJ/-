using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;//引用Color类型
using System.Runtime.InteropServices;
using SharpGL;
using System.Windows.Forms;


namespace 线圈模型
{


    //此处注意：静态变量在实例中不可以引用，实例指的是具体的一个对象，所以此处结构体中的变量不能设置为静态变量
    /// 定义一个线圈类型的结构体
    public struct Circles
    {
        //[MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NURBS.n * 3, ArraySubType = UnmanagedType.R8)]
        public float[] controlC;//控制点数组：用于绘制（此处不可初始化长度，是错误的）
        public Color colo;

        public void ResetCons(int i, float x,float y,float z)
        {
            controlC[i + 0] = x;
            controlC[i + 1] = y;
            controlC[i + 2] = z;
        }
    }


    /// 总体定义线圈共有x行y列 
    public class Circle_sum
    {
        public static int x = 200;//线圈有x行
        public static int y = 100;//线圈有y列
       
        public static Circles[,] circles = new Circles[Circle_sum.x, Circle_sum.y];//定义100 * 100的矩阵，用于存放100 * 100个线圈

    }



    class Circle
    {
       

        /// <summary>
        /// 更新线圈矩阵
        /// </summary>
        /// <param name="control">单个线圈的控制点坐标</param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="H"></param>
        /// <param name="r"></param>
        /// <param name="d"></param>
        public static void Update_Circle(float[] control, double A, double B, double H, double r, double d)
        {
            for (int i = 0; i < Circle_sum.x; i++ )//初始化circles中结构体数组的长度
            {
                for(int j = 0;j < Circle_sum.y; j++)
                {
                    Circle_sum.circles[i, j].controlC = new float[(NURBS.n+1) * 3];
                }   
            }

            //先定义最左下角，下标为[0][0]的线圈中的控制点坐标
            for (int i = 0; i <= NURBS.n; i++)
            {
                Circle_sum.circles[0, 0].ResetCons(i * 3, control[i * 3 + 0], control[i * 3 + 1], control[i * 3 + 2]);
            }

            //再全部求出第一列的线圈的控制点坐标
            for (int i = 1; i < Circle_sum.x; i++)//行下标
            {
                for (int j = 0; j <= NURBS.n; j++)//每个格子有n*3个值
                {                   
                    Circle_sum.circles[i, 0].ResetCons(j * 3, Circle_sum.circles[i - 1, 0].controlC[j * 3 + 0], Circle_sum.circles[i - 1, 0].controlC[j * 3 + 1] + (float)(0.7 * H * r), Circle_sum.circles[i - 1, 0].controlC[j * 3 + 2]);
                    //Circle_sum.circles[i, 0].ResetCons(j * 3, Circle_sum.circles[i - 1, 0].controlC[j * 3 + 0], Circle_sum.circles[i - 1, 0].controlC[j * 3 + 1] + (float)(H / 2 + r), Circle_sum.circles[i - 1, 0].controlC[j * 3 + 2]);
                }
            }

            //求出全部线圈的控制点坐标
            for (int i = 0; i < Circle_sum.x; i++)
            {
                for (int j = 1; j < Circle_sum.y; j++)//[i,j]格
                {
                    for(int k = 0;k <= NURBS.n; k++)//每个格子里有n个控制点坐标,n*3个长度
                    {
                        Circle_sum.circles[i, j].ResetCons(k * 3, Circle_sum.circles[i, j - 1].controlC[k * 3 + 0] + (float)(A + B - 2 * d), Circle_sum.circles[i, j - 1].controlC[k * 3 + 1], Circle_sum.circles[i, j - 1].controlC[k * 3 + 2]);
                        //Circle_sum.circles[i, j].ResetCons(k * 3, Circle_sum.circles[i, j - 1].controlC[k * 3 + 0] + (float)(2 * r * (H - A * r + 1)), Circle_sum.circles[i, j - 1].controlC[k * 3 + 1], Circle_sum.circles[i, j - 1].controlC[k * 3 + 2]);
                    }                   
                }
            }

        }

        public static void Draw(IntPtr theNurb, OpenGL gl,int i,int j)
        {
            //IntPtr theNurb = gl.NewNurbsRenderer();
            gl.BeginCurve(theNurb);
            gl.NurbsCurve(theNurb, NURBS.n + NURBS.k + 2, NURBS.u0, 3, Circle_sum.circles[i, j].controlC, 4, OpenGL.GL_MAP1_VERTEX_3);
            gl.EndCurve(theNurb);
            theNurb = IntPtr.Zero;
        }

        /// <summary>
        /// 刷新绘图区
        /// </summary>
        /// <param name="gl">绘图区</param>
        public static void RefreshPic(OpenGL gl)
        {

            //初始化设置
            gl.ClearColor(0.8f, 0.8f, 0.8f, 0.0f);//设置清除颜色
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);//把窗口清除为当前颜色
            //gl.ClearDepth(1.0);//指定深度缓冲区中每个像素需要的值
            //gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT);//清除深度缓冲区

            //SharpGL.Win32.SwapBuffers();//实现双缓冲技术,交换两个缓冲区指针。

            //gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);//  Clear the color and depth buffer.

            gl.LoadIdentity();//  Load the identity matrix.

            gl.MatrixMode(OpenGL.GL_PROJECTION);//set the projection matrix.设置投影矩阵

            gl.LoadIdentity();//Load the identity.//加载单位矩阵

            //gl.Perspective(50.0f, (Double)Width / (Double)Height, 0.01, 100.0);//Create a perspective transformation.//透视转换
            //gl.Perspective(150.0f, (Double)Width / (Double)Height, 0.01, 100.0);//Create a perspective transformation.//透视转换

            gl.Perspective(40.0f, (Double)(Variable.eyex * 5) / (Double)(Variable.eyey * 5), 0.01, 1000.0);

            
            //gl.LookAt(100, 100, 150, 100, 100, 0, 0, 1, 0);
           
            gl.LookAt(Variable.eyex, Variable.eyey, Variable.eyez, Variable.eyex, Variable.eyey, 0, 0, 1, 0);
            //可以把这个数据设为全局变量
            

            //gl.LookAt(eyex, eyey, eyez, eyex, eyey, 0, 0, 1, 0);

            //gl.LookAt(0, 0, 30, 0, 0, 0, 0, 1, 0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);//Set  the modelview matrix.设置模型视图矩阵

            //gl.Ortho(-1, 2, -3, 5, -100, 100);

            ////////////////////////////////////////////////////

            //if (NURBS.isMove == true)//线圈生成后，滚轮控制才有效
            //{
            //    openGLControl1.MouseWheel += new MouseEventHandler(openglcontrol_MouseWheel);
            //}

            //openGLControl1.MouseWheel += new MouseEventHandler(openglcontrol_MouseWheel);
        }
    }
}
