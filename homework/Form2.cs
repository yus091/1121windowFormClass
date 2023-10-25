﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace homework
{
    public partial class Form2 : Form
    {

        static Graphics g;	  //繪圖裝置（一個就夠了）
        static int r = 10, r2 = 20;    //半徑，直徑
        static double fr = 0; //摩擦力
        BufferedGraphicsContext currentContext;
        BufferedGraphics gBuffer;
        static int width = 0, height = 0; // 球桌寬高
        class ball
        {     //球 class(類別)
            int id;                                      //球編號
            public double x = 0, y = 0;    //球心 坐標
            Color c;                                   //球顏色
            SolidBrush br;                        //刷子（畫球用）
            private double ang = 0; // 球 行進角度
            public double cosA, sinA; // coSine 行進角度， Sine 行進角度
            public double speed = 0; // 球行進速度
            public void move()
            {
                if(speed > 0) // 速度 > 0 才移動
                {
                    x += speed * cosA; // x 方向分量
                    y += speed * sinA; // y 方向分量
                    speed -= fr; // 速度依照摩擦力大小遞減
                }
                else
                {
                    speed = 0; // 避免 < 0 而反向移動
                }
            }

            public ball(int bx, int by, Color cc, int i)
            {  //建構者
                x = bx;                                 //球心 x 坐標
                y = by;                                 //球心 y 坐標
                c = cc;                                  //球顏色    
                br = new SolidBrush(cc);     //球顏色的刷子
                id = i;                                   //球編號
            }
            public void draw()
            {      //畫 球物件 自己
                g.FillEllipse(br, (int)(x - r), (int)(y - r), r2, r2);          //畫橢圓（球刷子，左上角 坐標，直徑寬，直徑高）
            }
            public void setAng(double _ang)  // 角度改變
            {
                ang = _ang;  // 存 新角度
                cosA = Math.Cos(ang); // 重算 coSine
                sinA = Math.Sin(ang); // 重算 Sine
            }
            public void drawStick()
            {
                double r12 = 12 * r;
                Pen skyBluePen = new Pen(Brushes.DeepSkyBlue);
                skyBluePen.Width = 3.0F;
                g.DrawLine(skyBluePen, 
                    (float)(x - r12 * cosA), (float)(y - r12 * sinA),
                    (float)(x - r *cosA), (float)(y - r * sinA)
                    );
            }

            public void rebound()
            {
                if(x < r || x > width - r)   // 出左右邊
                {
                    setAng(Math.PI - ang);
                    if(x < r)
                    {
                        x = r;  // 拉回桌面
                    }
                    else
                    {
                        x = width - r;
                    }
                }
                else if(y < r || y > height - r)  // 出上下邊
                {
                    setAng(-ang);
                    if(y < r)
                    {
                        y = r;  // 拉回桌面
                    }
                    else
                    {
                        y = height - r;
                    }
                }
            }

        }	//class ball 結束

        ball[] balls = new ball[10];    // 10 顆球的陣列    宣告，new 

        public Form2()
        {
            InitializeComponent();

            g = panel1.CreateGraphics();     //繪圖裝置 初始化
            for (int i = 1; i < 10; i++)            //new 每個球，ball 建構者參數 見 work_note3 說明
                balls[i] = new ball(200, i * (r2 + 10)+90, Color.FromArgb(255, (i * 100)%256, (i * 50)%256, (i * 25)%256), i);
　　　   // 0號球(母球)， 白色，放右邊中間
　　　       balls[0] = new ball(600, 230, Color.FromArgb(255, 255, 255, 255), 0);
            balls[0].setAng(Math.PI / 4);
            width = panel1.Width;
            height = panel1.Height;

        }

        


        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Owner.Show();
        }


        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < 10; i++)     //10 顆球
                balls[i].draw();     //每個球 畫自己

            // balls[0].drawStick(); // 畫指向 0號球（母球）的球桿
            if(balls[0].speed < 0.0001)
            {
                balls[0].drawStick(); //  0號球停止時 才畫指向 0號球(母球) 的球桿
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            double a = Math.Atan2(e.Y - balls[0].y, e.X - balls[0].x); // e：滑鼠點擊處座標
            balls[0].setAng(a); //存入母球行進角度
            panel1.Refresh(); // 重新繪畫轉動過的球桿
            g.DrawRectangle(Pens.HotPink, e.X - 2, e.Y - 2, 4, 4); //點擊點畫小方塊
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double sum_speed = 0;
            double spd_sum = 0;
            panel1.Refresh();
            for(int i = 0; i < 10; i++)
            {
                if (balls[i].speed > 0)
                {
                    balls[i].move();
                    balls[i].rebound();
                    sum_speed += balls[i].speed;
                }
            }
            if(sum_speed <= 0.001)
            {
                timer1.Stop();
                panel1.Refresh();
            }
        }

        private void HitButton_Click(object sender, EventArgs e)
        {
            balls[0].speed = vScrollBar1.Maximum - vScrollBar1.Value;
            fr = (vScrollBar2.Maximum - vScrollBar2.Value) / 50.0;
            timer1.Enabled = true;
        }

        private void HitPowerLB_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = "歡迎 － " + ((Form1)Owner).textBox1.Text;
        }
    }
}
