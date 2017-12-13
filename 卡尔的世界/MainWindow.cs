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
        //双缓冲
        BufferedGraphics bufferGfx;

        //图像
        private Graphics graphics, gfx;

        //绘制列表
        private List<RectangleF> list = new List<RectangleF>();
        private List<string> listn = new List<string>();

        //获取资源
        private ResourceManager rm = new ResourceManager("carlworld.block.block", Assembly.GetExecutingAssembly());

        //方格
        private int seew = 32, seeh = 24;

        //世界对象
        private World world = new World();
        
        private OpenGL gl;

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
            //创建画布
            graphics = this.CreateGraphics();
            
            //生成地图
            world.CreateMap();

            gl = glc.OpenGL;

            gl.ClearColor(world.skycolorR, world.skycolorG, world.skycolorB, 1.0f);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_NORMALIZE);

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
                    world.personface = false;
                    world.person.X -= 0.2f;
                    break;
                case Keys.D:
                    world.personface = true;
                    world.person.X += 0.2f;
                    break;
            }
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            //在列表产生位置
            list.Clear();
            listn.Clear();
            
            //绘制范围、位置（我也是为了方便呀）
            RectangleF rect;
            
            for (int dx = 0; dx <= seew; dx++)
            {
                for (int dy = 0; dy <= seeh; dy++)
                {
                    int x = dx + (int) world.person.X,
                        y = dy + (int) world.person.Y - seeh / 2 - 1;

                    //绘图偏移量
                    float xx = dx + ((int) world.person.X - world.person.X),
                          yy = dy + ((int) world.person.Y - world.person.Y);

                    //添加到渲染列表
                    if (world.blocks[x, y] != 0)
                    {
                        //如果不是空气,就绘制。。

                        rect = new RectangleF(xx, yy, 1f, 1f);
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
        
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
        }

        void Draw()
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            
            gl.LoadIdentity();

            for (int i = 0; i < list.Count; i++)
            {
                //gl.Rotate(Rotation, 0.0f, 1.0f, 0.0f);
                gl.Color(1f, 0f, 0f, 0f);

                textrued[listn[i]].Bind(gl);
                gl.Begin(OpenGL.GL_QUADS);//绘制四边形
                gl.TexCoord(0.0, 0.0); gl.Vertex(list[i].X, list[i].Y);
                gl.TexCoord(0.0, 1.0); gl.Vertex(list[i].X, list[i].Bottom);
                gl.TexCoord(1.0, 1.0); gl.Vertex(list[i].Right, list[i].Bottom);
                gl.TexCoord(1.0, 0.0); gl.Vertex(list[i].Right, list[i].Y);
                gl.End();
            }

            gl.Flush();

        }
    }
}
