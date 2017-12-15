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
        //地图数组
        public int[,] blocks;
        int blockslengh = 500;

        //人物位置
        public PointF person = new PointF(0, 0);
        //脸的方向
        public bool personface = false;

        public Color skycolor = Color.FromArgb(20, 100, 255);
        public float skycolorR = 0.1f;
        public float skycolorG = 0.5f;
        public float skycolorB = 1.0f;

        //随机数
        private Random random = new Random();

        public void CreateMap()
        {
            //清空地图
            blocks = new int[blockslengh, blockslengh];

            //生成随机地图
            long CreateW, CreateH, StoneHeight, SoilHeight;
            //循环添加下层泥土

            //石层
            StoneHeight = 50;
            SoilHeight = StoneHeight + 20;
            for (CreateW = 0; CreateW < blockslengh; CreateW++)
            {
                for (CreateH = SoilHeight; CreateH < blockslengh; CreateH++)
                {
                    if (CreateH < StoneHeight)
                        //石头
                        blocks[CreateW, CreateH] = 10;
                    else
                        //泥土
                        blocks[CreateW, CreateH] = 1;
                }
                //添加顶层草
                blocks[CreateW, SoilHeight] = 2;
            }


            //随机陡峭地形
            long MountainHeight = 0;
            //生成陡峭地形
            long MountainY = SoilHeight - 1;
            bool isUp = false;
            blocks[CreateW / 2, SoilHeight - 1] = 1;
            blocks[CreateW / 2 + 1, SoilHeight - 1] = 1;
            blocks[CreateW / 2 + 2, SoilHeight - 1] = 1;
            for (CreateW = 0; CreateW < blockslengh; CreateW++)
            {
                if (SoilHeight - MountainY >= MountainHeight && isUp == false)
                {
                    isUp = true;
                    MountainHeight = (int)(Rnd() * 10);
                }
                else if (SoilHeight - MountainY <= 0 && isUp == true)
                {
                    isUp = false;
                }
                
                if (isUp)
                    MountainY = MountainY + (long)(Rnd() * 3);
                else
                    MountainY = MountainY - (long)(Rnd() * 3);
                //            blocks[CreateW / 2, StoneHeight - 1] = 1;
                if (CreateW % 50 > 10)
                {
                    for (CreateH = MountainY; CreateH <= SoilHeight; CreateH++)
                    {
                        blocks[CreateW, CreateH] = 1;
                        if (CreateH == MountainY)
                            blocks[CreateW, CreateH] = 2;
                    }
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
        }
        private double Rnd()
        {
            return random.NextDouble();
        }

    }
}
