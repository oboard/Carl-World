using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace carlworld
{
    class World
    {
        public int[,] blocks = new int[100, 100];
        int blockslengh = 100;

        public PointF person = new PointF(0, 0);

        public void CreateMap()
        {
            //清空地图
            blocks = new int[100, 100];

            //生成随机地图
            long CreateW, CreateH, CreateMaxH;
            //循环添加下层泥土

            //泥土层
            CreateMaxH = 30;

            for (CreateW = 0; CreateW < blockslengh; CreateW++)
            {
                for (CreateH = CreateMaxH; CreateH < 100; CreateH++)
                {
                    if (CreateH < 50)
                        blocks[CreateW, CreateH] = 1;
                    else
                        //石头
                        blocks[CreateW, CreateH] = 10;
                }
                //添加顶层草
                blocks[CreateW, CreateMaxH - 1] = 2;
            }

            //小人位置设定
            person.X = blockslengh / 2;
            person.Y = 1;
            while (blocks[(int)person.X, (int)person.Y + 1] == 0)
            {
                //确定小人坐标
                person.Y++;
            }

        }

        private double Rnd()
        {
            return new Random().NextDouble();
        }

    }
}
