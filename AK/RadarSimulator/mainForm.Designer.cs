﻿namespace RadarSimulator
{
    partial class mainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.editIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.editPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.editX = new System.Windows.Forms.TextBox();
            this.editY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.editZ = new System.Windows.Forms.TextBox();
            this.editVz = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.editVy = new System.Windows.Forms.TextBox();
            this.editVx = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnSendOne = new System.Windows.Forms.Button();
            this.btnSendLoop = new System.Windows.Forms.Button();
            this.editInterval = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "目的广播地址:";
            // 
            // editIp
            // 
            this.editIp.Location = new System.Drawing.Point(102, 10);
            this.editIp.Name = "editIp";
            this.editIp.Size = new System.Drawing.Size(100, 21);
            this.editIp.TabIndex = 1;
            this.editIp.Text = "239.255.255.250";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "目的广播端口:";
            // 
            // editPort
            // 
            this.editPort.Location = new System.Drawing.Point(313, 10);
            this.editPort.Name = "editPort";
            this.editPort.Size = new System.Drawing.Size(100, 21);
            this.editPort.TabIndex = 3;
            this.editPort.Text = "5000";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(290, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "Y:";
            // 
            // editX
            // 
            this.editX.Location = new System.Drawing.Point(102, 36);
            this.editX.Name = "editX";
            this.editX.Size = new System.Drawing.Size(100, 21);
            this.editX.TabIndex = 6;
            this.editX.Text = "111";
            // 
            // editY
            // 
            this.editY.Location = new System.Drawing.Point(313, 36);
            this.editY.Name = "editY";
            this.editY.Size = new System.Drawing.Size(100, 21);
            this.editY.TabIndex = 7;
            this.editY.Text = "222";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(430, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "Z:";
            // 
            // editZ
            // 
            this.editZ.Location = new System.Drawing.Point(453, 36);
            this.editZ.Name = "editZ";
            this.editZ.Size = new System.Drawing.Size(100, 21);
            this.editZ.TabIndex = 9;
            this.editZ.Text = "333";
            // 
            // editVz
            // 
            this.editVz.Location = new System.Drawing.Point(453, 63);
            this.editVz.Name = "editVz";
            this.editVz.Size = new System.Drawing.Size(100, 21);
            this.editVz.TabIndex = 15;
            this.editVz.Text = "666";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(430, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "VZ:";
            // 
            // editVy
            // 
            this.editVy.Location = new System.Drawing.Point(313, 63);
            this.editVy.Name = "editVy";
            this.editVy.Size = new System.Drawing.Size(100, 21);
            this.editVy.TabIndex = 13;
            this.editVy.Text = "555";
            // 
            // editVx
            // 
            this.editVx.Location = new System.Drawing.Point(102, 63);
            this.editVx.Name = "editVx";
            this.editVx.Size = new System.Drawing.Size(100, 21);
            this.editVx.TabIndex = 12;
            this.editVx.Text = "444";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(290, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "VY:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(79, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 12);
            this.label8.TabIndex = 10;
            this.label8.Text = "VX:";
            // 
            // btnSendOne
            // 
            this.btnSendOne.Location = new System.Drawing.Point(394, 104);
            this.btnSendOne.Name = "btnSendOne";
            this.btnSendOne.Size = new System.Drawing.Size(75, 23);
            this.btnSendOne.TabIndex = 16;
            this.btnSendOne.Text = "发送一次";
            this.btnSendOne.UseVisualStyleBackColor = true;
            this.btnSendOne.Click += new System.EventHandler(this.btnSendOne_Click);
            // 
            // btnSendLoop
            // 
            this.btnSendLoop.Location = new System.Drawing.Point(475, 104);
            this.btnSendLoop.Name = "btnSendLoop";
            this.btnSendLoop.Size = new System.Drawing.Size(75, 23);
            this.btnSendLoop.TabIndex = 17;
            this.btnSendLoop.Text = "循环发送";
            this.btnSendLoop.UseVisualStyleBackColor = true;
            this.btnSendLoop.Click += new System.EventHandler(this.btnSendLoop_Click);
            // 
            // editInterval
            // 
            this.editInterval.Location = new System.Drawing.Point(283, 106);
            this.editInterval.Name = "editInterval";
            this.editInterval.Size = new System.Drawing.Size(100, 21);
            this.editInterval.TabIndex = 19;
            this.editInterval.Text = "100";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(194, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 12);
            this.label9.TabIndex = 18;
            this.label9.Text = "发送间隔(ms):";
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(453, 8);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 20;
            this.btnOpen.Text = "打开";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 139);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.editInterval);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnSendLoop);
            this.Controls.Add(this.btnSendOne);
            this.Controls.Add(this.editVz);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.editVy);
            this.Controls.Add(this.editVx);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.editZ);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.editY);
            this.Controls.Add(this.editX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.editPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.editIp);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "雷测模拟器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox editIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox editPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox editX;
        private System.Windows.Forms.TextBox editY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox editZ;
        private System.Windows.Forms.TextBox editVz;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox editVy;
        private System.Windows.Forms.TextBox editVx;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSendOne;
        private System.Windows.Forms.Button btnSendLoop;
        private System.Windows.Forms.TextBox editInterval;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Timer timer;
    }
}
