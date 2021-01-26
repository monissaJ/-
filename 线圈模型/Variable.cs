using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 线圈模型
{
    class Variable//全局变量
    {
        public static float user_hei;//用户输入的高度（长）
        public static float user_wid;//用户输入的宽度（指定的1.4米）
        public static float user_scale;//“篇幅”的比例（宽/高==宽/长）

        public static int picBox2_wid;//pictureBox2的宽度
        public static int picBox2_hei;//pictureBox2的高度

        public static int openglcontrol_wid;//pictureBox2的宽度
        public static int openglcontrol_hei;//pictureBox2的高度

        //lookat的全局变量
        public static double eyex;
        public static double eyey;
        public static double eyez = 100;

    }
}
