using carlworld;
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
        private int seew = 16, seeh = 9;

        //世界对象
        private World world = new World();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            //创建画布
            graphics = this.CreateGraphics();

            FreshGfx();

            //生成地图
            world.CreateMap();
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //一刀捅死双缓冲
            DisposeGfx();
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            //重置双缓冲

            DisposeGfx();

            FreshGfx();
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            //在列表产生位置
            list.Clear();
            listn.Clear();

            int w = ClientRectangle.Width, h = ClientRectangle.Height;

            //绘制范围、位置（我也是为了方便呀）
            RectangleF rect;
            
            for (int dx = 0; dx < seew; dx++)
            {
                for (int dy = 0; dy < seeh; dy++)
                {
                    int x = dx + (int) world.person.X,
                        y = dy + (int) world.person.Y;

                    //添加到渲染列表
                    if (world.blocks[x, y] != 0)
                    {
                        //如果不是空气,就绘制。。

                        rect = new RectangleF(dx, dy, 1, 1);
                        list.Add(rect);
                        listn.Add("_" + world.blocks[x, y]);
                    }
                }
            }

            //人物
            rect = new RectangleF(seew / 2, seeh / 2 - 2, 1, 2);
            list.Add(rect);
            listn.Add("p");

            //全部绘制
            Draw();

            Application.DoEvents();
        }

        void FreshGfx()
        {
            //新的双缓冲
            bufferGfx = BufferedGraphicsManager.Current.Allocate(graphics, ClientRectangle);
            gfx = bufferGfx.Graphics;

            gfx.ScaleTransform(ClientRectangle.Width / seew, ClientRectangle.Height / seeh);
            gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        }
        
        void DisposeGfx()
        {
            //摧毁
            gfx.Dispose();
            bufferGfx.Dispose();
        }

        void Draw()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Image img = (Image)rm.GetObject(listn[i]);

                //绘制列表的内容
                gfx.DrawImage(img, list[i]);
            }

            //双缓冲渲染
            bufferGfx.Render(graphics);
        }
    }
}
