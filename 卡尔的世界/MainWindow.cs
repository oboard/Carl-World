using carlworld;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 卡尔的世界
{
    public partial class MainWindow : Form
    {

        //绘制列表
        private List<RectangleF> list = new List<RectangleF>();
        private List<string> listn = new List<string>();

        //获取资源
        private ResourceManager rm = new ResourceManager("carlworld.block.block", Assembly.GetExecutingAssembly());

        //可见范围
        private int seew = 32, seeh = 16;

        //绘图偏移量
        private float xx, yy;

        //世界对象
        private World world = new World();
        
        //OpenGL对象
        private OpenGL gl;

        //行走加速
        private float toX, toY;

        //鼠标坐标
        private Point mouse = new Point(0, 0);
        private bool havemouse = false;
            
        //纹理
        private Dictionary<String, Texture> textrued = new Dictionary<String, Texture>();

        //窗口大小
        int w, h;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            
            //生成地图
            world.CreateMap();
            toX = world.person.X;
            toY = world.person.Y;

            gl = glc.OpenGL;

            gl.ClearColor(world.skycolorR, world.skycolorG, world.skycolorB, 0.5f);

            gl.Enable(OpenGL.GL_ALPHA_TEST); //透明部分测试
            gl.AlphaFunc(OpenGL.GL_GREATER, 0.1f);

            gl.Enable(OpenGL.GL_BLEND);//启用混合
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

            //gl.ShadeModel(OpenGL.GL_SMOOTH);//允许平滑着色
            
            //添加纹理到纹理数组

            Texture textrue = new Texture();
            textrue.Create(gl, new Bitmap((Image)rm.GetObject("p")));
            textrued.Add("p", textrue);

            textrue = new Texture();
            textrue.Create(gl, new Bitmap((Image)rm.GetObject("p2")));
            textrued.Add("p2", textrue);

            for (int i = 1; i < 15; i++)
            {
                textrue = new Texture();
                textrue.Create(gl, new Bitmap((Image)rm.GetObject("_" + i)));
                textrued.Add("" + i, textrue);
            }

            MainWindow_Resize(sender, e);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            w = ClientRectangle.Width;
            h = ClientRectangle.Height;

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            //gl.LookAt(0, 0, -5, 0, 0, 0, 0, 1, 0);// 计算窗口的纵横比（像素比）
            gl.Ortho2D(0, seew, seeh, 0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void glc_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.A:
                    //左←_←
                    world.personface = false;
                    toX = world.person.X - 1f;
                    break;
                case Keys.D:
                    //右→_→
                    world.personface = true;
                    toX = world.person.X + 1f;
                    break;
                case Keys.W:
                    //跳↑_↑
                    Jump();
                    break;
                case Keys.S:
                    //蹲↓_↓
                    //还没做好嘛。。
                    break;
            }
        }

        private void glc_Load(object sender, EventArgs e)
        {

        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            //X轴加速
            if (Math.Abs(world.person.X - toX) > 0.05f)
                world.person.X = world.person.X + (toX - world.person.X) / 10;
            else
                world.person.X = toX;

            //Y轴加速
            if (Math.Abs(world.person.Y - toY) > 0.05f)
                world.person.Y = world.person.Y + (toY - world.person.Y) / 5;
            else
                world.person.Y = toY;

            if (world.blocks[(int)(world.person.X + xx), (int)toY] == 0)
                toY++;

            if (world.blocks[(int)world.person.X, (int)toY] != 0 &&
                world.blocks[(int)(world.person.X), (int)toY - 1]!= 0)
                Jump();

            //在列表产生位置
            list.Clear();
            listn.Clear();
            
            //绘制范围、位置（我也是为了方便呀）
            RectangleF rect;
            
            for (int dx = 0; dx <= seew; dx++)
            {
                for (int dy = 0; dy <= seeh; dy++)
                {
                    int x = dx + (int)world.person.X - seew / 2,
                        y = dy + (int)world.person.Y - seeh / 2 - 2;

                    //绘图偏移量
                    xx = (int)world.person.X - world.person.X;
                    yy = (int)world.person.Y - world.person.Y;

                    //添加到渲染列表
                    if (world.blocks[x, y] != 0)
                    {
                        //如果不是空气,就绘制。。

                        rect = new RectangleF(dx + xx, dy + yy, 1f, 1.1f);
                        list.Add(rect);
                        listn.Add("" + world.blocks[x, y]);
                    }
                }
            }

            //人物
            rect = new RectangleF(seew / 2, seeh / 2, 1, 2);
            list.Add(rect);
            if (world.personface)
                listn.Add("p");
            else
                listn.Add("p2");

            //全部绘制
            Draw();
        }

        private void glc_MouseLeave(object sender, EventArgs e)
        {
            havemouse = false;
        }

        private void glc_MouseMove(object sender, MouseEventArgs e)
        {
            havemouse = true;
            mouse.X = (int)((float)e.X / w * seew - xx);
            mouse.Y = (int)((float)e.Y / h * seeh - yy);
        }

        private void glc_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void Jump()
        {
            if (toY == world.person.Y)
                //只有在地面才能跳
                toY = world.person.Y - 4f;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            havemouse = true;
        }

        void Draw()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.Color(1f, 1f, 1f, 1f);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            for (int i = 0; i < list.Count; i++)
            {
                //gl.Rotate(Rotation, 0.0f, 1.0f, 0.0f);
                
                textrued[listn[i]].Bind(gl);
                gl.Begin(OpenGL.GL_QUADS);//绘制四边形
                gl.TexCoord(0.0, 0.0); gl.Vertex(list[i].X, list[i].Y);
                gl.TexCoord(0.0, 1.0); gl.Vertex(list[i].X, list[i].Bottom);
                gl.TexCoord(1.0, 1.0); gl.Vertex(list[i].Right, list[i].Bottom);
                gl.TexCoord(1.0, 0.0); gl.Vertex(list[i].Right, list[i].Y);
                gl.End();

            }

            gl.Disable(OpenGL.GL_TEXTURE_2D);

            //绘制光标
            if (havemouse)
            {
                gl.Color(0f, 0f, 0f, 0.2f);
                gl.Rect(mouse.X + xx, mouse.Y + yy, mouse.X + xx + 1, mouse.Y + yy + 1);
            }

            gl.Flush();

        }
    }
}
