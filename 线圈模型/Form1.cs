using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

using SharpGL;






namespace 线圈模型
{
    

    

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        
   

        private void button2_Click(object sender, EventArgs e)
        {
            //选择矩形后才生成线圈
            if(cluster.clusterFinished == true)
            {
                


                //把文本框的背景颜色归位
                textBox1.BackColor = Color.FromArgb(255, 255, 255);
                textBox2.BackColor = Color.FromArgb(255, 255, 255);
                textBox3.BackColor = Color.FromArgb(255, 255, 255);
                textBox4.BackColor = Color.FromArgb(255, 255, 255);

                //判断用户是否输入数据，如果没有输入，则提示
                if (textBox1.Text == "")
                {
                    MessageBox.Show("输入空信息！", "提示");
                    textBox1.Focus();
                }
                else if (textBox2.Text == "")
                {
                    MessageBox.Show("输入空信息！", "提示");
                    textBox2.Focus();
                }
                else if (textBox3.Text == "")
                {
                    MessageBox.Show("输入空信息！", "提示");
                    textBox3.Focus();
                }
                else if (textBox4.Text == "")
                {
                    MessageBox.Show("输入空信息！", "提示");
                    textBox4.Focus();
                }
                else if (textBox5.Text == "")
                {
                    MessageBox.Show("输入空信息！", "提示");
                    textBox5.Focus();
                }
                else
                {
                    //接收用户数据
                    Double A = Convert.ToDouble(textBox1.Text);//针编弧宽度
                    Double B = Convert.ToDouble(textBox2.Text);//沉降弧宽度
                    Double H = Convert.ToDouble(textBox3.Text);//线圈的高度
                    Double r = Convert.ToDouble(textBox4.Text);//交织点系数
                    Double d = Convert.ToDouble(textBox5.Text);//纱线的截面直径

                    //判断用户输入的数据是否符合规定
                    if (A < 3 * d || A > 5 * d)
                    {
                        MessageBox.Show("A的值需满足：3d <= A <= 5d", "Input data error!");
                        textBox1.BackColor = Color.FromArgb(252, 230, 202);
                        textBox1.Text = "";
                        textBox1.Focus();
                    }
                    else if (B < 3 * d || B > 7 * d)
                    {
                        MessageBox.Show("B的值需满足：3d <= B <= 7d!", "Input data error!");
                        textBox2.BackColor = Color.FromArgb(252, 230, 202);
                        textBox2.Text = "";
                        textBox2.Focus();
                    }
                    else if (H < 3 * d || H > 7 * d)
                    {
                        MessageBox.Show("H的值需满足：3d <= H <= 7d!", "Input data error!");
                        textBox3.BackColor = Color.FromArgb(252, 230, 202);
                        textBox3.Text = "";
                        textBox3.Focus();
                    }
                    else if ((r < d / H) || (r > H - 2 * d))
                    {
                        MessageBox.Show("r的值需满足：(d/H) <= r <= (H-2d)!", "Input data error!");
                        textBox4.BackColor = Color.FromArgb(252, 230, 202);
                        textBox4.Text = "";
                        textBox4.Focus();
                    }
                    else//输入数据满足要求时开始生成图形
                    {
                        //先修改OpenGLControl1的大小（默认）
                        openGLControl1.Width = Variable.openglcontrol_wid;
                        openGLControl1.Height = Variable.openglcontrol_hei;
                        //判断矩形的宽高比例，确定openglcontrol1的宽高比例(矩形宽/高就是用户输入的值得宽高比例                
                        if (Variable.user_wid > Variable.user_hei)//矩形的宽度比较“大”，按OpenGLcontrol1的宽来重新定义高
                        {

                            openGLControl1.Height = Convert.ToInt16(openGLControl1.Width / Variable.user_scale);
                        }
                        else//矩形的高度比较“大”，按OpenGLcontrol1的高来重新定义宽
                        {
                            openGLControl1.Width = Convert.ToInt16(openGLControl1.Height * Variable.user_scale);
                        }

                        //计算数据点                
                        NURBS曲线.Data_Create(A, B, H, r, d);

                        //反求控制点
                        NURBS曲线.Controls_Create(NURBS.datas);

                        //更新节点矢量数组的值
                        for (int i = 0; i <= (NURBS.n + NURBS.k + 1); i++)
                        {
                            NURBS.u0[i] = (float)NURBS.u[i]; //把控制点数据传给变量             
                        }

                        //更新控制点数组的值
                        for (int i = 0; i <= NURBS.n; i++)
                        {
                            NURBS.ctrlpoints[i * 3 + 0] = (float)NURBS.Control_Points_New[i].x; //把控制点数据传给变量
                            NURBS.ctrlpoints[i * 3 + 1] = (float)NURBS.Control_Points_New[i].y; //把控制点数据传给变量
                            NURBS.ctrlpoints[i * 3 + 2] = (float)NURBS.Control_Points_New[i].z; //把控制点数据传给变量
                        }

                        //更新线圈矩阵
                        Circle.Update_Circle(NURBS.ctrlpoints, A, B, H, r, d);


                        //每次点击更新全局变量
                        Variable.eyex = openGLControl1.Width / 5;
                        Variable.eyey = openGLControl1.Height / 10;

                        //每次点击需要重新刷新绘图区
                        OpenGL gl = openGLControl1.OpenGL;
                        Circle.RefreshPic(gl);

                        //gl.lookat也恢复到初始值
                        gl.LookAt(Variable.eyex, Variable.eyey, Variable.eyez, Variable.eyex, Variable.eyey, 0, 0, 1, 0);

                        //左视图
                        //gl.LookAt(Variable.eyex-70, Variable.eyey, Variable.eyez-30, 0, Variable.eyey, Variable.eyez-30, 0, 1, 0);

                        gl.MatrixMode(OpenGL.GL_MODELVIEW);//Set  the modelview matrix.设置模型视图矩阵
                    }
                }
            }else
            {
                MessageBox.Show("请先进行聚类！", "提示", MessageBoxButtons.OK);
            } 
        }

        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
           
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!绘制曲线(Begin)!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!第一个圈!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            OpenGL gl = openGLControl1.OpenGL;//Get the OpenGL object.

            gl.Flush();//强制刷新缓存

            gl.Finish();//强制执行操作
            
            //设置线的宽度和点大小 
            float linewidth = float.Parse(textBox5.Text);//把string转化为float
            
            //gl.LineWidth(0.5f);
            //gl.LineWidth(linewidth);
            gl.LineWidth(5.0f);
            
            gl.PointSize(3.0f);
            gl.Color(0.0f, 0.0f, 0.0f);//设置画笔颜色为黑色

            ////////////////////////////////////绘制NURBS/////////////////////////////////////////////////////////

            //gl.Enable(OpenGL.GL_MAP1_VERTEX_3);
            gl.Enable(OpenGL.GL_LINE_SMOOTH); // 平滑线条    

            gl.Color(col.R,col.G,col.B);

            ////从下往上生成------------------------------------------------------------------------------------
            ////------------------------设定100 * 100个线圈-------------------------------------------------------------------------

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!内存溢出!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            IntPtr theNurb;
            theNurb = gl.NewNurbsRenderer();
            for (int i = 0; i < Circle_sum.x; i++)
            {
                for (int j = 0; j < Circle_sum.y; j++)
                {                 
                    Circle.Draw(theNurb, gl, i, j);
                }
            }

            
           //-------------------------------------------------------------------------------------------

            ///////////////////////////////////////////////////////////////////////////////////////////////////////


            //////测试：数据点
            ////gl.Color(1.0, 0.0, 0.0);//红色
            ////gl.Begin(OpenGL.GL_POINTS);
            ////for (int i = 0; i <= NURBS.m; i++)
            ////{
            ////    gl.Vertex(NURBS.datas[i].x, NURBS.datas[i].y, NURBS.datas[i].z);
            ////}
            ////gl.End();

            //////测试：控制点
            ////gl.Color(1.0, 1.0, 1.0);//黑色
            ////gl.Begin(OpenGL.GL_POINTS);
            ////for (int i = 0; i <= NURBS.n; i++)
            ////{
            ////    gl.Vertex(NURBS.Control_Points_New[i].x, NURBS.Control_Points_New[i].y, NURBS.Control_Points_New[i].z);
            ////}
            ////gl.End();

            //////测试-------------------坐标轴--------------------------------------Begin
            ////gl.LineWidth(3.0f);

            ////gl.Color(0.5, 0.0, 0.0);//x
            ////gl.Begin(OpenGL.GL_LINE_STRIP);
            ////gl.Vertex(0, 0, 0);
            ////gl.Vertex(40, 0, 0);
            ////gl.End();

            ////gl.Color(0.0, 0.5, 0.0);//y
            ////gl.Begin(OpenGL.GL_LINE_STRIP);
            ////gl.Vertex(0, 0, 0);
            ////gl.Vertex(0, 40, 0);
            ////gl.End();

            ////gl.Color(0.0, 0.0, 0.5);//z
            ////gl.Begin(OpenGL.GL_LINE_STRIP);
            ////gl.Vertex(0, 0, 0);
            ////gl.Vertex(0, 0, 40);
            ////gl.End();
            //////测试-----------------坐标轴----------------------------------------End

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!绘制曲线(End)!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            openGLControl1.MouseWheel += new MouseEventHandler(openglcontrol_MouseWheel);
        }


        
        //double eyex = 11;//x方向
        //double eyey = 6.5;//y方向
        //double eyez = 30;//y方向
        //double eyex = 100;//x方向
        //double eyey = 100;//y方向
        //double eyez = 150;//y方向
        private void openGLControl1_Load(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl1.OpenGL;//  Get the OpenGL object.

            Circle.RefreshPic(gl);

            Variable.eyex = openGLControl1.Width / 1;
            Variable.eyey = openGLControl1.Height / 1;

            //gl.LookAt(2, 2, 6, 2, 2, 0, 0, 1, 0);//Use the "look at"helper function to position and aim the camera.
            //gl.LookAt(-4, 2, 2, 0, 2, 2, 0, 1, 0);//左视
            gl.LookAt(Variable.eyex, Variable.eyey, Variable.eyez, Variable.eyex, Variable.eyey, 0, 0, 1, 0);
            //gl.LookAt(eyex, eyey, eyez, eyex, eyey, 0, 0, 1, 0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);//Set  the modelview matrix.设置模型视图矩阵

            //gl.Ortho(-1, 2, -3, 5, -100, 100);
           
            //openGLControl1.MouseWheel += new MouseEventHandler(openglcontrol_MouseWheel);
            
        }


        /// <summary>
        /// 鼠标控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openglcontrol_MouseEnter(object sender, EventArgs e)//当鼠标移到 openGLControl1内，获取焦点
        {
            openGLControl1.Focus();
        }       
        void openglcontrol_MouseWheel(object sender, MouseEventArgs e)
        {
            //eyex += (e.Delta/ 10 );
            //eyey += (e.Delta/ 10 );

            Variable.eyez += (e.Delta/60);//e.Delta向上滑是120，向下滑是-120
          
            ///////////////////////////////////////重新绘制//////////////////////////////////////////////////////
            ///////////////刷新绘图区，否则后面绘制的图像将会和前面绘制的图像同时存在////////////////////////////

            //  Get the OpenGL object.
            OpenGL gl = openGLControl1.OpenGL;

            Circle.RefreshPic(gl);

            if (Variable.eyez > 3)
            {
                
                //gl.LookAt(0, 0, eyez, 0, 0, 0, 0, 1, 0);
                gl.LookAt(Variable.eyex, Variable.eyey, Variable.eyez, Variable.eyex, Variable.eyey, 0, 0, 1, 0);
            }
            else if (Variable.eyez <= 3)
            {
                Variable.eyez = 3;
                gl.LookAt(Variable.eyex, Variable.eyey, Variable.eyez, Variable.eyex, Variable.eyey, 0, 0, 1, 0);
            }
            label8.Text = Convert.ToString(Variable.eyez);
            //else if (Variable.eyez >= 100)
            //{
            //    gl.LookAt(Variable.eyex, Variable.eyey, Variable.eyez, Variable.eyex, Variable.eyey, 0, 0, 1, 0);
            //}

            gl.MatrixMode(OpenGL.GL_MODELVIEW);//Set  the modelview matrix.设置模型视图矩阵
            //////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////           
        }



        
        Color col = Color.Black;
        /// <summary>
        /// 用户选择线圈颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.ShowDialog();//显示颜色选取框

            //这里可以不选取颜色，因为默认为黑色
            //while (cd.ShowDialog() != DialogResult.OK)
            //{
            //    MessageBox.Show("请选取颜色", "选取颜色错误提示");
            //}

            col = cd.Color;//更新全局变量
            pictureBox1.BackColor = cd.Color;//显示用户选取的颜色
        }

        
        /// <summary>
        /// 用户选择图片=====需要两个全局变量wid和hei，分别对应pictureBox2初始的大小，在Form_Load中更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "JPG文件(*.jpg)|*.jpg";
            DialogResult result = openFileDialog1.ShowDialog();
            String str = openFileDialog1.FileName;

            //此处必须选择图片,如果不选择图片，则所有变量都保持原本的数据
            if(result == DialogResult.OK)
            {
                //先将 pictureBox2的大小调节至原来的大小
                pictureBox2.Width = Variable.picBox2_wid;
                pictureBox2.Height = Variable.picBox2_hei;

                panel1.Width = pictureBox2.Width + 10;
                panel1.Height = pictureBox2.Height + 10;

                Image myImage = System.Drawing.Image.FromFile(str);

                //判断myImage的宽的比例大还是高度的比例大，选择比例大的进行重新修正pictureBox的大小            
                float ImageWidth = myImage.Width;
                float ImageHeight = myImage.Height;
                float scale = ImageWidth / ImageHeight;//计算宽度与高度的比例
                if (scale >= 1)    //宽度所占的比例大，按宽度为原大小
                {
                    pictureBox2.Width = Variable.picBox2_wid;
                    pictureBox2.Height = (int)((float)(pictureBox2.Width) / scale);

                    panel1.Width = pictureBox2.Width + 10;
                    panel1.Height = pictureBox2.Height + 10;
                }
                else
                {
                    pictureBox2.Height = Variable.picBox2_hei;
                    pictureBox2.Width = (int)((float)(pictureBox2.Height) * scale);

                    panel1.Width = pictureBox2.Width + 10;
                    panel1.Height = pictureBox2.Height + 10;
                }


                this.pictureBox2.Image = myImage;

                cluster.iscluster = false;//设置标志位为不聚类
                pic.isPic = true;//图片已经选择
                cluster.clusterFinished = false;//图片未聚类

                //更新聚类结果
                pictureBox3.BackColor = Color.White;
                pictureBox4.BackColor = Color.White;
                pictureBox5.BackColor = Color.White;
                pictureBox6.BackColor = Color.White;
            }
        }


        //--------------------------选择一个区域，对这个区域中的像素点进行聚类------------------------
        //---------------------------------------------------------------------------------------------

        /// <summary>
        /// 聚类====目前是整张图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if(cluster.iscluster == true)//聚类
            {
                //获取picBox中的图片，设置为Bitmap格式
                Bitmap newImage = (Bitmap)pictureBox2.Image;

                //计算矩形相对于图片框的比例
                float leftX = paint.Rec_x / pictureBox2.Width;//左边起始
                float X = paint.Rec_width / pictureBox2.Width;//宽度
                float topY = paint.Rec_y / pictureBox2.Height;//顶部起始
                float Y = paint.Rec_height / pictureBox2.Height;//高度

                //计算矩形在实际图片中的位置
                int newX = (int)(leftX * pictureBox2.Image.Width);
                int newY = (int)(topY * pictureBox2.Image.Height);

                //获取矩形大小
                int picwidth = (int)(X * pictureBox2.Image.Width);
                int picheight = (int)(Y * pictureBox2.Image.Height);

                //定义一个相应的二维数组
                Color[,] col = new Color[picwidth, picheight];//m行n列数组

                //获取图像像素，存到对应的位置
                for (int i = 0; i < picwidth; i++)
                {
                    for (int j = 0; j < picheight; j++)
                    {
                        col[i, j] = newImage.GetPixel(i + newX, j + newY);
                    }
                }


                //将二维数组转换为一维数组
                Color[] colArray = new Color[picwidth * picheight];
                for (int i = 0; i < picwidth; i++)
                {
                    for (int j = 0; j < picheight; j++)
                    {
                        colArray[(picheight * i) + j] = col[i, j];
                    }
                }


                //样本集为数组col，聚类簇数为4
                int n = 4;
                //定义一个数组用于接收聚类结果
                Color[] result = new Color[n];


                //最大循环次数
                int cou = Convert.ToInt16(textBox6.Text);
                result = Cluster.ClusterMethod(colArray, picwidth * picheight, n, cou);

                pictureBox3.BackColor = result[0];
                pictureBox4.BackColor = result[1];
                pictureBox5.BackColor = result[2];
                pictureBox6.BackColor = result[3];

                //聚类完成，给一个信号：可以生成线圈模型
                cluster.clusterFinished = true;
            }
            else
            {
                MessageBox.Show("请选择聚类区域！", "提示", MessageBoxButtons.OK);
            }

            
        }





        
        /// <summary>
        /// 用户在“篇幅”处输入长，图片框中可以由用户自己选择需要聚类的图片区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if(pic.isPic == true)//图片已经选择，矩形选择才有意义
            {
                pictureBox2.Refresh();//清除之前绘制的图像

                //将pictureBox2设为画布,在该控件上绘制矩形
                Graphics g = pictureBox2.CreateGraphics();

                //计算绘制的位置坐标（初始位置位于中心）---------------------------------------
                //---------------------------------Begin-------------------------------------

                //先计算PictureBox2中图片的大小，并找到中心
                int picBox2Pic_wid = pictureBox2.Width;
                int picBox2Pic_hei = pictureBox2.Height;
                //获取用户输入的数据：长为textBox7.Text，宽为1.4米，计算比例，求一半的值

                //更新用户输入的数据（更新的是全局，后期在绘制时也可以用）
                Variable.user_hei = float.Parse(textBox7.Text);//string转float
                Variable.user_wid = float.Parse("1.4");//string转float
                Variable.user_scale = Variable.user_wid / Variable.user_hei;//比例为   宽度/高度

                //更新要显示的矩形的大小
                paint.Rec_height = Variable.user_hei;
                paint.Rec_width = Variable.user_wid;

                //判断这两个值谁大谁小，大的值放大到pictureBox2的二分之一
                if (Variable.user_wid > Variable.user_hei)//宽度大
                {
                    paint.Rec_width = picBox2Pic_wid / 2;//把宽度放大到1/2
                    paint.Rec_height = paint.Rec_width / Variable.user_scale;//高度等比例放大
                }
                else//高度大或相同
                {
                    paint.Rec_height = picBox2Pic_hei / 2;//把高度放大到1/2
                    paint.Rec_width = paint.Rec_height * Variable.user_scale;//高度等比例放大
                }
                //计算绘制的位置坐标（初始位置位于中心）---------------------------------------
                //----------------------------------End-----------------------------------

                //更新要显示的矩形的坐标------------Bagin-----------------------------------------
                paint.Rec_x = picBox2Pic_wid / 2 - paint.Rec_width / 2;
                paint.Rec_y = picBox2Pic_hei / 2 - paint.Rec_height / 2;
                //更新要显示的矩形的坐标------------End-------------------------------------------            

                //绘制
                PicturePaint.Paint(g);

                cluster.iscluster = true;//显示矩形，如果矩形未显示，则不能进行聚类
                cluster.clusterFinished = false;//图片已选择、重新选择了区域

                
            }
            else//提示选择图像
            {
                MessageBox.Show("图像未选择，此时选择矩形区域将无意义！", "提示", MessageBoxButtons.OK);
            }
            //更新聚类结果
            pictureBox3.BackColor = Color.White;
            pictureBox4.BackColor = Color.White;
            pictureBox5.BackColor = Color.White;
            pictureBox6.BackColor = Color.White;
            
        }


        /// <summary>
        /// 界面加载时记录原本pictureBox2的大小，界面还原时用，不用使得每次加载入图片后对界面造成改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Variable.picBox2_wid = pictureBox2.Width;
            Variable.picBox2_hei = pictureBox2.Height;

            //记录OpenGLControl1的大小
            Variable.openglcontrol_wid = openGLControl1.Width;
            Variable.openglcontrol_hei = openGLControl1.Height;

            pictureBox2.Location = new Point(5, 5);
            
            //同时记录panel1的大小
            panel1.Width = pictureBox2.Width + 10;
            panel1.Height = pictureBox2.Height + 10;

            //位置
            
    
            cluster.iscluster = false;//不聚类
            pic.isPic = false;//图片未选择
            cluster.clusterFinished = false;//未聚类
        }


        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            //获取鼠标现在所处的位置，看是否处于阈值内

            //计算右下角坐标的阈值框的四个值
            float leftX = paint.Pos[3].x - paint.threshold;
            float rightX = paint.Pos[3].x + paint.threshold;
            float topY = paint.Pos[3].y - paint.threshold;
            float bottomY = paint.Pos[3].y + paint.threshold;

            if ((e.X > leftX) && (e.X < rightX) && (e.Y > topY) && (e.Y < bottomY))//处于右下角阈值内（执行拖动），进行操作
            {
                //标志位为1，说明鼠标抬起时需要进行操作了
                paint.flag = 1;

                this.pictureBox2.Cursor = Cursors.SizeNWSE;
            }
            //鼠标位置处于矩形中间（执行移动)
            else if ((e.X > paint.Pos[0].x + 2 * paint.threshold) && (e.X < paint.Pos[2].x - 2 * paint.threshold) && (e.Y > paint.Pos[0].y + 2 * paint.threshold) && (e.Y < paint.Pos[1].y - 2 * paint.threshold))
            {
                paint.flag = 2;
                
                //记录现在鼠标的位置(全局)
                mouse.formX = e.X;
                mouse.formY = e.Y;

                this.pictureBox2.Cursor = Cursors.SizeAll;//改变鼠标样式为“移动”
            }
            //鼠标位置处于矩形下边界
            else if((e.Y > (paint.Pos[1].y - paint.threshold)) && (e.Y < (paint.Pos[1].y + paint.threshold)))
            {
                paint.flag = 3;
                this.pictureBox2.Cursor = Cursors.SizeNS;//南北图标
            }
            //鼠标处于矩形右边界
            else if ((e.X > (paint.Pos[2].x - paint.threshold)) && (e.X < (paint.Pos[2].x + paint.threshold)))
            {
                paint.flag = 4;
                this.pictureBox2.Cursor = Cursors.SizeWE;//西东图标
            }
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            //检测鼠标左键是否按下
            if(e.Button==MouseButtons.Left)
            {
                if((paint.flag == 1) || (paint.flag == 3))//标志位为1，说明鼠标按下时的位置有效（处于右下角），执行的是拖动矩形的大小。标志位为3，执行的是鼠标位于矩形下边界时对矩形进行操作
                {
                    //预判当此次操作过后是否矩形将会超出区域
                    float new_Height = e.Y - paint.Rec_y;
                    float new_Width = Variable.user_scale * new_Height;

                    //计算新的边界区域
                    float newRight = paint.Rec_x + new_Width;
                    float newBottom = paint.Rec_y + new_Height;

                    //判断鼠标的位置有没有到图片框外面（没有到外面才执行）
                    if((newRight > paint.Rec_x+ 2 * paint.threshold) && (newRight < (float)(pictureBox2.Width - paint.threshold))&&(newBottom > paint.Rec_y + 2 * paint.threshold) && (newBottom < (float)(pictureBox2.Height - 2 * paint.threshold)))
                    {
                        pictureBox2.Refresh();//清除之前绘制的图像

                        Graphics g = pictureBox2.CreateGraphics();//为了在鼠标移动过程中矩形都有重新绘制，故每次移动鼠标时矩形都重新绘制
                        
                        paint.Rec_height = new_Height;//对矩形的高度进行更新           
                        paint.Rec_width = new_Width;//新的高度对应新的宽度

                        PicturePaint.Paint(g);//重新绘制矩形
                    } 
                }
                else if (paint.flag == 2)//按下时鼠标处于矩形中间，则执行的是移动矩形
                {
                    Graphics g = pictureBox2.CreateGraphics();

                    //判断移动前鼠标与左上角的相对位置
                    float relativeX = mouse.formX - paint.Rec_x;
                    float relativeY = mouse.formY - paint.Rec_y;

                    //判断新位置坐标 是否超出边界
                    float newX = e.X - relativeX;
                    float newY = e.Y - relativeY;

                    if ((newX > paint.threshold) && (newX + paint.Rec_width) < (pictureBox2.Width - paint.threshold) && (newY > paint.threshold) && (newY + paint.Rec_height) < (pictureBox2.Height - paint.threshold))
                    {
                        pictureBox2.Refresh();//清除之前绘制的图像

                        //把全局变量中的鼠标位置更新为现在的鼠标位置
                        mouse.formX = e.X;
                        mouse.formY = e.Y;

                        //计算矩形的新位置坐标
                        paint.Rec_x = newX;
                        paint.Rec_y = newY;

                        PicturePaint.Paint(g);
                    }
 
                }
                //以x坐标为准进行矩形的放大缩小
                else if(paint.flag == 4)
                {
                    //计算改变后的矩形宽度和高度
                    float new_Width = e.X - paint.Rec_x;
                    float new_Height = new_Width / Variable.user_scale;

                    //计算新的边界区域
                    float newRight = paint.Rec_x + new_Width;
                    float newBottom = paint.Rec_y + new_Height;

                    //判断矩形边界的位置有没有到图片框外面（没有到外面才执行）
                    if ((newRight > paint.Rec_x + 2 * paint.threshold) && (newRight < (float)(pictureBox2.Width - paint.threshold)) && (newBottom > paint.Rec_y + 2 * paint.threshold) && (newBottom < (float)(pictureBox2.Height - 2 * paint.threshold)))
                    {
                        pictureBox2.Refresh();//清除之前绘制的图像

                        Graphics g = pictureBox2.CreateGraphics();//为了在鼠标移动过程中矩形都有重新绘制，故每次移动鼠标时矩形都重新绘制
                        
                        paint.Rec_width = e.X - paint.Rec_x;//对矩形的宽度进行更新
                                                
                        paint.Rec_height = paint.Rec_width / Variable.user_scale;//新的宽度对应新的高度

                        PicturePaint.Paint(g);//重新绘制矩形
                    } 
                }
            }
            else//鼠标没有按下(判断鼠标所处的位置，对鼠标指针进行变换）
            {
                //获取鼠标现在所处的位置，看是否处于阈值内

                //计算右下角坐标的阈值框的四个值
                float leftX = paint.Pos[3].x - paint.threshold;
                float rightX = paint.Pos[3].x + paint.threshold;
                float topY = paint.Pos[3].y - paint.threshold;
                float bottomY = paint.Pos[3].y + paint.threshold;

                if ((e.X > leftX) && (e.X < rightX) && (e.Y > topY) && (e.Y < bottomY))//处于右下角阈值内（执行拖动），进行操作
                {
                    this.pictureBox2.Cursor = Cursors.SizeNWSE;
                }
                //鼠标位置处于矩形中间（执行移动)
                else if ((e.X > paint.Pos[0].x + 2 * paint.threshold) && (e.X < paint.Pos[2].x - 2 * paint.threshold) && (e.Y > paint.Pos[0].y + 2 * paint.threshold) && (e.Y < paint.Pos[1].y - 2 * paint.threshold))
                {
                    this.pictureBox2.Cursor = Cursors.SizeAll;//改变鼠标样式为“移动”
                }
                //鼠标处于矩形下边界
                else if ((e.Y > (paint.Pos[1].y - paint.threshold)) && (e.Y < (paint.Pos[1].y + paint.threshold)))
                {
                    this.pictureBox2.Cursor = Cursors.SizeNS;//改变鼠标形状为“南北”移动
                }
                //鼠标处于矩形右边界
                else if ((e.X > (paint.Pos[2].x - paint.threshold)) && (e.X < (paint.Pos[2].x + paint.threshold)))
                {
                    this.pictureBox2.Cursor = Cursors.SizeWE;//西东图标
                }
                else
                {
                    this.pictureBox2.Cursor = Cursors.Default;//如果鼠标没有处于任何一个区域内，则鼠标为默认指针
                }
            }

        }

        /// <summary>
        /// 鼠标抬起（对矩形区域的操作停止）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            paint.flag = 0;

            //如果鼠标位置仍然处于区域内，则鼠标形状不变

            //计算右下角坐标的阈值框的四个值
            float leftX = paint.Pos[3].x - paint.threshold;
            float rightX = paint.Pos[3].x + paint.threshold;
            float topY = paint.Pos[3].y - paint.threshold;
            float bottomY = paint.Pos[3].y + paint.threshold;

            if ((e.X > leftX) && (e.X < rightX) && (e.Y > topY) && (e.Y < bottomY))//处于右下角阈值内（执行拖动），进行操作
            {
                this.pictureBox2.Cursor = Cursors.SizeNWSE;//改变鼠标样式为“改变大小”
            }
            //鼠标位置处于矩形中间（执行移动)
            else if ((e.X > paint.Pos[0].x + 2 * paint.threshold) && (e.X < paint.Pos[2].x - 2 * paint.threshold) && (e.Y > paint.Pos[0].y + 2 * paint.threshold) && (e.Y < paint.Pos[1].y - 2 * paint.threshold))
            {
                this.pictureBox2.Cursor = Cursors.SizeAll;//改变鼠标样式为“移动”
            }
            //鼠标处于矩形下边界
            else if ((e.Y > (paint.Pos[1].y - paint.threshold)) && (e.Y < (paint.Pos[1].y + paint.threshold)))
            {
                this.pictureBox2.Cursor = Cursors.SizeNS;//改变鼠标形状为“南北”移动
            }
            //鼠标处于矩形右边界
            else if ((e.X > (paint.Pos[2].x - paint.threshold)) && (e.X < (paint.Pos[2].x + paint.threshold)))
            {
                this.pictureBox2.Cursor = Cursors.SizeWE;//西东图标
            }
            else
            {
                this.pictureBox2.Cursor = Cursors.Default;//如果鼠标没有处于任何一个区域内，则鼠标为默认指针
            }
        }

        //用户设置矩形区域颜色（有的图片白色居多，可能需要别的颜色）
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            //修改背景颜色
            ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.ShowDialog();//显示颜色选取框

            //这里可以不选取颜色，因为默认为白色            
            pictureBox7.BackColor = cd.Color;//显示用户选取的颜色

            paint.pen = new Pen(cd.Color, 2);//定义画笔

            if(cluster.iscluster == true)//如果已经绘制矩形，则直接重新绘制，否则什么都不做等着绘制
            {
                Graphics g = pictureBox2.CreateGraphics();
                PicturePaint.Paint(g);
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox8.Text = trackBar1.Value.ToString(); 
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            trackBar1.Value = Convert.ToInt16(textBox8.Text);

            Variable.eyez = Convert.ToInt16(textBox8.Text);

            //更新
            OpenGL gl = openGLControl1.OpenGL;

            Circle.RefreshPic(gl);

            gl.LookAt(Variable.eyex, Variable.eyey, Variable.eyez, Variable.eyex, Variable.eyey, 0, 0, 1, 0);


            gl.MatrixMode(OpenGL.GL_MODELVIEW);//Set  the modelview matrix.设置模型视图矩阵
        }

        
            

    }
    
}
