using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace 线圈模型
{

    //和聚类有关的变量
    public class cluster
    {
        public static bool iscluster;//是否聚类
        public static bool clusterFinished;//是否聚类完成
        
    }

    class Cluster
    {

        //-----------------------对数据集进行聚类--------------------------------------
        //-----------------------------------------------------------------------------

        /// <summary>
        /// 聚类
        /// </summary>
        /// <param name="col">样本集</param>
        /// <param name="len">样本个数（方便找随机数）</param>
        /// <param name="k">聚类簇数</param>
        /// /// <param name="cou">最大循环轮次</param>
        /// <returns>聚类结果</returns>
        public static Color[] ClusterMethod(Color[] col, int len, int k, int cou)
        {
            Color[] result = new Color[k];//定义一个长度为n的数组，用来接收簇结果，簇结果为四个颜色值

            //定义一个ArrayList数组，可以动态添加内容(内容是分别对应每个簇中的成员）

            List<List<Color>> crowd = new List<List<Color>>(k);

            for (int i = 0; i < k; i++)
            {
                List<Color> crowdList = new List<Color>();
                crowd.Add(crowdList);
            }


            //从样本集中随机选择k个样本作为初始均值向量{μ1,μ2,...,μk}     (均值向量初始化)
            Random ran = new Random();
            int[] ranKey = new int[k];
            for (int i = 0; i < k; i++)
            {
                //产生一个随机数
                int temp = ran.Next(0, len);
                for (int j = 0; j < i - 1; j++)
                {
                    if (temp != ranKey[j])//不等于当前比较的数时继续和下一个比较
                    {
                        continue;
                    }
                    else//如果和当前比较的数相同，则重新生成一个随机数
                    {
                        temp = ran.Next(0, len);
                    }
                }
                //该随机数不和之前产生的任意一个随机数相同
                ranKey[i] = temp;

            }

            //定义一个均值向量数组avg，长度为k，存储的为每次最终的均值向量
            Color[] avg = new Color[k];
            for (int i = 0; i < k; i++)
            {
                avg[i] = col[ranKey[i]];
            }

            //repeat
            //定义一个阈值（最大循环轮次）
            int maxN = cou;
            int flag = 0;
            int n = 0;//定义当前循环轮次，因为还未执行循环，故设为0

            while (n <= maxN)
            {
                //对簇进行初始化
                for (int i = 0; i < k; i++)
                {
                    crowd[i].RemoveRange(0, crowd[i].Count);
                    //把均值向量放入相应的簇（避免最后某个簇中没有内容）
                    crowd[i].Add(avg[i]);
                }

                //对当前簇划分(跟均值向量 之间的距离）
                for (int i = 0; i < len; i++)
                {
                    float[] r = new float[k];
                    float[] R = new float[k];
                    float[] G = new float[k];
                    float[] B = new float[k];
                    float[] C = new float[k];

                    //计算每个样本与各均值向量μi的距离
                    for (int j = 0; j < k; j++)
                    {
                        //计算与result[j]之间的距离，填入C[j]
                        r[j] = (col[i].R + avg[j].R) / 2;
                        R[j] = col[i].R - avg[j].R;
                        G[j] = col[i].G - avg[j].G;
                        B[j] = col[i].B - avg[j].B;
                        C[j] = (float)(Math.Sqrt((2 + r[j] / 256) * (R[j] * R[j]) + 4 * (G[j] * G[j]) + (2 + (255 - r[j]) / 256) * (B[j] * B[j])));
                    }
                    //判断与K个均值向量之间的距离选择距离最小的写入
                    int MaxI = 0;
                    for (int j = 0; j < k; j++)
                    {
                        if (C[MaxI] > C[j])//与C[j]的距离更近
                        {
                            MaxI = j;
                        }
                    }

                    crowd[MaxI].Add(col[i]);//把当前样本加入相应簇
                }

                //对均值向量迭代更新
                Color[] m = new Color[k];
                for (int i = 0; i < k; i++)
                {
                    //计算新均值向量

                    int Rsum = 0;//计算C0的R分量的平均值
                    int Gsum = 0;//计算C0的G分量的平均值
                    int Bsum = 0;//计算C0的G分量的平均值

                    for (int j = 0; j < crowd[i].Count; j++)
                    {
                        Rsum += crowd[i][j].R;
                        Gsum += crowd[i][j].G;
                        Bsum += crowd[i][j].B;
                    }
                    Rsum = Rsum / crowd[i].Count;//求R方向上的平均值
                    Gsum = Gsum / crowd[i].Count;//求G方向上的平均值
                    Bsum = Bsum / crowd[i].Count;//求B方向上的平均值

                    //把三个方向上的平均值综合成新的均值向量                    
                    m[i] = Color.FromArgb((int)Rsum, (int)Gsum, (int)Bsum);

                    //比较与旧的均值向量是否相同
                    if (m[i] != avg[i])
                    {
                        avg[i] = m[i];
                        flag = 1;
                    }
                    n++;
                }
                if (flag == 0)
                {
                    break;
                }
            }

            //结果已经求出，替换到result数组
            for (int i = 0; i < k; i++)
            {
                result[i] = avg[i];
            }

            return result;
        }

    }
}
