using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpGL;
//using System.Math;

namespace 线圈模型
{
    //定义一个点结构体
    public struct Points
    {
        public Double x;
        public Double y;
        public Double z;

        public void ResetPoints(Double x0, Double y0, Double z0)//这样可以不用重复写，可以直接把三个值全部赋值
        {
            x = x0;
            y = y0;
            z = z0;
        }
    };



    public class NURBS
    {

        public static int m = 8;//数据点个数为9(q0~q8)
        public static int n = 10;//控制点个数为11(d0~d10)
        public static int k = 3;//B样条次数为3
        public static Double[] u = new Double[n + k + 2];//定义节点矢量组

        public static Points[] Control_Points_New = new Points[n + 1];//定义一个控制点数组
        public static Points[] datas = new Points[m + 1];//定义一个数据点数组


        public static float[] u0 = new float[n + k + 2];//节点数组：用于绘制
        public static float[] ctrlpoints = new float[(n + 1) * 3];//控制点数组：用于绘制


    }



    public class NURBS曲线
    {
        /// <summary>
        /// 求数据点坐标
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="H"></param>
        /// <param name="r"></param>
        /// <param name="d"></param>
        public static void Data_Create(Double A, Double B, Double H, Double r, Double d)
        {
            NURBS.datas[0].ResetPoints(0, 0, 0);
            NURBS.datas[1].ResetPoints(B / 2, H / 2 * (1 - r), d * (1 - r));
            NURBS.datas[2].ResetPoints((B + 2 * d) / 4, H / 2, d);
            NURBS.datas[3].ResetPoints(B / 2 - d, H / 2 * (1 + r), d * (1 - r));
            NURBS.datas[4].ResetPoints((A + B) / 2 - d, H, 0);
            NURBS.datas[5].ResetPoints(A + B / 2 - d, H / 2 * (1 + r), d * (1 - r));
            NURBS.datas[6].ResetPoints(A + (3 * B / 4) - 5 * d / 2, H / 2, d);
            NURBS.datas[7].ResetPoints(A + B / 2 - 2 * d, H / 2 * (1 - r), d * (1 - r));
            NURBS.datas[8].ResetPoints(A + B - 2 * d, 0, 0);

            //NURBS.datas[0].ResetPoints(-(r * (H - A * r) + A), -(H / 2 + A), 0);
            //NURBS.datas[1].ResetPoints(-(r * (H - A * r)), -(H / 2), -(Math.Sqrt(d * d - (H / 2) * (H / 2))));
            //NURBS.datas[2].ResetPoints(-(r + (r * (H - A * r))) / 2, 0, - d);
            //NURBS.datas[3].ResetPoints(- r, H / 2, - Math.Sqrt(d * d - (H / 2) * (H / 2)));
            //NURBS.datas[4].ResetPoints(0, H / 2 + r, 0);
            //NURBS.datas[5].ResetPoints(r, H / 2, - Math.Sqrt(d * d - (H / 2) * (H / 2)));
            //NURBS.datas[6].ResetPoints((r + (r * (H - A * r)))/2, 0, - d);
            //NURBS.datas[7].ResetPoints(r * (H - A * r), - H / 2, - Math.Sqrt(d * d - (H / 2) * (H / 2)));
            //NURBS.datas[8].ResetPoints(r * (H - A * r) + A, -(H / 2 + A), 0);


        }


        /// <summary>
        /// 求控制点坐标
        /// </summary>
        /// <param name="datas">数据点数组</param>
        public static void Controls_Create(Points[] datas)
        {
            Node(NURBS.datas);//更新节点矢量
            
            //-----------求x坐标--------------------------------------------------------------------------------------------   
        
            //建立矩阵n*3矩阵       d0=p0       dn=pn-2   浪费掉一个空间
            Double[,] QieShi = new Double[NURBS.n, NURBS.n];
            //填入数据
            //先写入切矢条件
            QieShi[1, 0] = 0; 
            QieShi[1, 1] = 1; 
            QieShi[1, 2] = 0;
            QieShi[NURBS.n - 1, 0] = 0; 
            QieShi[NURBS.n - 1, 1] = 1; 
            QieShi[NURBS.n - 1, 2] = 0;
            //动态生成数据
            for (int i = 2; i <= NURBS.n - 2;i++ )
            {
                QieShi[i, 0] = a_i(i);
                QieShi[i, 1] = b_i(i);
                QieShi[i, 2] = c_i(i);
            }

            //生成矢量矩阵
            Double[] vectorx = new Double[NURBS.n];
            vectorx[1]=NURBS.datas[0].x;
            vectorx[NURBS.n - 1] = NURBS.datas[NURBS.m].x;
            for(int j=2;j<=NURBS.n-2;j++)
            {
                vectorx[j] = vector_i(j);
            }

            //解矩阵方程
            Double[] controls_datax = new Double[NURBS.n];
            controls_datax = Create_ControlPoints(QieShi, vectorx);
         

            //----------求y坐标---------------------------------------------------------------------------------

            //生成矢量矩阵
            Double[] vectory = new Double[NURBS.n];
            vectory[1] = NURBS.datas[0].y;
            vectory[NURBS.n - 1] = NURBS.datas[NURBS.m].y;
            for (int j = 2; j <= NURBS.n - 2; j++)
            {
                vectory[j] = vector_iy(j);
            }

            //解矩阵方程
            Double[] controls_datay = new Double[NURBS.n];
            controls_datay = Create_ControlPoints(QieShi, vectory);


            //-----------------求z坐标---------------------------------------------------------------------------------

            //生成矢量矩阵
            Double[] vectorz = new Double[NURBS.n];
            vectorz[1] = NURBS.datas[0].z;
            vectorz[NURBS.n - 1] = NURBS.datas[NURBS.m].z;
            for (int j = 2; j <= NURBS.n - 2; j++)
            {
                vectorz[j] = vector_iz(j);
            }

            //解矩阵方程
            Double[] controls_dataz = new Double[NURBS.n];
            controls_dataz = Create_ControlPoints(QieShi, vectorz);

            //-------------------------------------------------------------------------------------------


            //综合到一个点组合
            
            //第一个控制点和最后一个控制点的坐标和数据点相同
            NURBS.Control_Points_New[0].x = NURBS.datas[0].x;
            NURBS.Control_Points_New[0].y = NURBS.datas[0].y;
            NURBS.Control_Points_New[0].z = NURBS.datas[0].z;
            NURBS.Control_Points_New[NURBS.n].x = NURBS.datas[NURBS.m].x;
            NURBS.Control_Points_New[NURBS.n].y = NURBS.datas[NURBS.m].y;
            NURBS.Control_Points_New[NURBS.n].z = NURBS.datas[NURBS.m].z;
            
            //依次把值更新到全局数组
            for (int i = 1; i < NURBS.n; i++)
            {
                NURBS.Control_Points_New[i].x = controls_datax[i];
                NURBS.Control_Points_New[i].y = controls_datay[i];
                NURBS.Control_Points_New[i].z = controls_dataz[i];
            }

        }


        /// <summary>
        /// 使用积累弦长参数法计算节点矢量--------Node
        /// </summary>
        /// <param name="data">数据点数组</param>
        /// <returns>更新A.u[A..+A.k+1]</returns>
        private static void Node(Points[] datas)
        {
            //Double[] U = new Double[A.n + A.k + 1];
            //定义节点矢量U={u[0]...u[n+k+1]}
            for (int i = 0; i <= NURBS.n+ NURBS.k + 1; i++)
            {
                if (i >= 0 && i <= 3)
                {
                    NURBS.u[i] = 0;
                }
                else if (i >= NURBS.n + 1 && i <= NURBS.n + NURBS.k + 1)
                {
                    NURBS.u[i] = 1;
                }
                else
                {
                    Double now_point = System.Math.Sqrt((datas[i - 3].x - datas[i - 4].x) * (datas[i - 3].x - datas[i - 4].x) + (datas[i - 3].y - datas[i - 4].y) * (datas[i - 3].y - datas[i - 4].y) + (datas[i - 3].z - datas[i - 4].z) * (datas[i - 3].z - datas[i - 4].z));
                    Double cou = 0;
                    for (int j = 1; j <= NURBS.m; j++)
                    {
                        cou = cou + System.Math.Sqrt((datas[j].x - datas[j - 1].x) * (datas[j].x - datas[j - 1].x) + (datas[j].y - datas[j - 1].y) * (datas[j].y - datas[j - 1].y) + (datas[j].z - datas[j - 1].z) * (datas[j].z - datas[j - 1].z));
                    }
                    NURBS.u[i] = NURBS.u[i - 1] + now_point / cou;
                }
            }
        }



        /// <summary>
        /// 求aix
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Double ai</returns>
        public static Double a_i(int i)
        {
            Double ai;


            Double numerator = Delta(i + 2) * Delta(i + 2);//分子
            Double denominator = Delta(i) + Delta(i + 1) + Delta(i + 2);//分母
            ai = numerator / denominator;

            return ai;
        }

        /// <summary>
        /// 求bix
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Double bi</returns>
        private static Double b_i(int i)
        {
            Double bi;

            Double numerator1 = Delta(i + 2) * (Delta(i) + Delta(i + 1));
            Double denominator1 = Delta(i) + Delta(i + 1) + Delta(i + 2);

            Double numerator2 = Delta(i + 1) * (Delta(i + 2) + Delta(i + 3));
            Double denominator2 = Delta(i + 1) + Delta(i + 2) + Delta(i + 3);

            bi = numerator1 / denominator1 + numerator2 / denominator2;

            return bi;
        }

        /// <summary>
        /// 求cix
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Double ci</returns>
        private static Double c_i(int i)
        {
            Double ci;

            Double numerator = Delta(i + 1) * Delta(i + 1);
            Double denominator = Delta(i + 1) + Delta(i + 2) + Delta(i + 3);

            ci = numerator / denominator;

            return ci;
        }



        /// <summary>
        /// 求Delta(i)
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Double Delta;</returns>
        private static Double Delta(int i)
        {
            Double Delta;

            Delta = NURBS.u[i + 1] - NURBS.u[i];

            return Delta;
        }



        /// <summary>
        /// 求vectorx
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Double vector</returns>
        private static Double vector_i(int i)
        {
            Double vector;
            vector = (Delta(i + 1) + Delta(i + 2)) * NURBS.datas[i - 1].x;
            return vector;
        }

        /// <summary>
        /// 求vectory
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Double vector</returns>
        private static Double vector_iy(int i)
        {
            Double vector;
            vector = (Delta(i + 1) + Delta(i + 2)) * NURBS.datas[i - 1].y;
            return vector;
        }

        /// <summary>
        /// 求vectorz
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Double vector</returns>
        private static Double vector_iz(int i)
        {
            Double vector;
            vector = (Delta(i + 1) + Delta(i + 2)) * NURBS.datas[i - 1].z;
            return vector;
        }



        /// <summary>
        /// 解三线性方程组    
        /// </summary>
        /// <param name="qieshi">系数矩阵n*3</param>
        /// <param name="vector">结果矩阵n*1</param>
        /// <returns>x[n]     其中x[0]没有值</returns>
        public static double[] Create_ControlPoints(double[,] qieshi,double[] vector)
        {
            double[] x=new double[NURBS.n];
            double[] r = new double[NURBS.n - 1];
            double[] y = new double[NURBS.n];
            //求r数组
            for (int i = 1; i <= NURBS.n - 2;i++ )
            {
                r[i] = R(qieshi, i);
            }
            //求y数组
            for (int i = 1; i <= NURBS.n - 2;i++ )
            {
                y[i] = Y(qieshi, vector, i);
            }
            //求y[n-1]
            y[NURBS.n - 1] = (vector[NURBS.n - 1] - y[NURBS.n - 2] * qieshi[NURBS.n - 1, 0]) / (qieshi[NURBS.n - 1,1] - r[NURBS.n - 2] * qieshi[NURBS.n - 1, 0]);
            //求x
            //先求x[n-2]
            x[NURBS.n - 1] = y[NURBS.n - 1];
            for (int i = NURBS.n - 2; i >= 1;i-- )
            {
                x[i] = y[i] - r[i] * x[i + 1];
            }
            return x;
        }

        /// <summary>
        /// 求r[i]
        /// </summary>
        /// <param name="qieshi">切矢矩阵</param>
        /// <param name="vector">矢量数组</param>
        /// <param name="i">下标</param>
        /// <returns>double r</returns>
        public static double R(double[,] qieshi,int i)
        {
            double r;
            if(i==1)
            {
                r = qieshi[1, 2] / qieshi[1, 1];               //     c1/b1
            }
            else
            {
                r = qieshi[i, 2] / (qieshi[i, 1] - R(qieshi, i - 1) * qieshi[i, 0]);
            }
            return r;
        }

        /// <summary>
        /// 求y数组
        /// </summary>
        /// <param name="qiehshi">切矢矩阵</param>
        /// <param name="vector">矢量数组</param>
        /// <param name="i">下标</param>
        /// <returns>double y</returns>
        public static double Y(double[,] qiehshi,double[] vector,int i)
        {
            double y;
            if(i==1)
            {
                y = vector[1] / qiehshi[1, 1];     //      d1/b1
            }
            else
            {
                y = (vector[i] - Y(qiehshi, vector, i - 1) * qiehshi[i, 0]) / (qiehshi[i, 1] - R(qiehshi, i - 1) * qiehshi[i, 0]);
            }
            return y;
        }

    }
}


       