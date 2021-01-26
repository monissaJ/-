using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace 线圈模型
{
    //定义一个结构体，用来存储矩形四个角的坐标（为什么不用point?因为point类型是int，而实际上用户输入的数据不可能总是整数倍，所以重新定义）
    public struct Position
    {
        public float x;
        public float y;

        public void ResetPos(float x0,float y0)
        {
            x = x0;
            y = y0;
        }
    }

    //和绘制矩形有关的全局变量
     public class paint
     {
        public static Pen pen = new Pen(Color.White, 2);//定义画笔

        //计算矩形坐标
        public static float Rec_x;
        public static float Rec_y;
            
        //计算矩形大小
        public static float Rec_width;
        public static float Rec_height;

        public static Position[] Pos = new Position[4];//矩形四个位置坐标（鼠标操作矩形时要使用），位置分别为：左上、左下、右上、右下。

        //定义一个阈值，用来判断鼠标在pictureBox2中的位置是否处于阈值大小内
        public static int threshold = 5;


         public static int flag = 0;//移动无效
    }

    //和鼠标操作有关的全局变量
     public class mouse
     {
         public static float formX;//记录鼠标移动前的位置
         public static float formY;        
     }

     public class pic
     {         
         public static bool isPic;//移动无效
     }

    class PicturePaint     //绘图
    {
        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="g">绘图板</param>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="width">矩形宽度</param>
        /// <param name="height">矩形高度</param>
        public static void Paint(Graphics g)
        {            
            g.DrawRectangle(paint.pen, paint.Rec_x, paint.Rec_y, paint.Rec_width, paint.Rec_height);//参数分别是：画笔，xy坐标，宽，高
            g.Dispose();//释放由graphics使用的所有资源

            //更新四个位置的坐标
            paint.Pos[0].ResetPos(paint.Rec_x, paint.Rec_y);
            paint.Pos[1].ResetPos(paint.Rec_x, paint.Rec_y + paint.Rec_height);
            paint.Pos[2].ResetPos(paint.Rec_x + paint.Rec_width, paint.Rec_y);
            paint.Pos[3].ResetPos(paint.Rec_x + paint.Rec_width, paint.Rec_y + paint.Rec_height);

        }




    }
}
