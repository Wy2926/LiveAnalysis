namespace LivePopularity
{
    partial class FrmMain
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
            this.dgvRooms = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblIpsNums = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbMsg = new System.Windows.Forms.RichTextBox();
            this.txtIpNums = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRoomAdd = new System.Windows.Forms.Button();
            this.txtRoomId = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.rtbGift = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvRooms
            // 
            this.dgvRooms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRooms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dgvRooms.Location = new System.Drawing.Point(13, 139);
            this.dgvRooms.Margin = new System.Windows.Forms.Padding(4);
            this.dgvRooms.Name = "dgvRooms";
            this.dgvRooms.RowHeadersWidth = 51;
            this.dgvRooms.RowTemplate.Height = 23;
            this.dgvRooms.Size = new System.Drawing.Size(341, 688);
            this.dgvRooms.TabIndex = 18;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "RoomId";
            this.Column1.HeaderText = "房间ID";
            this.Column1.MinimumWidth = 6;
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "Total";
            this.Column2.HeaderText = "目标人数";
            this.Column2.MinimumWidth = 6;
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "OkCount";
            this.Column3.HeaderText = "成功进入";
            this.Column3.MinimumWidth = 6;
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "ErrorCount";
            this.Column4.HeaderText = "进入失败";
            this.Column4.MinimumWidth = 6;
            this.Column4.Name = "Column4";
            // 
            // lblIpsNums
            // 
            this.lblIpsNums.AutoSize = true;
            this.lblIpsNums.Location = new System.Drawing.Point(13, 40);
            this.lblIpsNums.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIpsNums.Name = "lblIpsNums";
            this.lblIpsNums.Size = new System.Drawing.Size(68, 15);
            this.lblIpsNums.TabIndex = 17;
            this.lblIpsNums.Text = "IP数量：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(535, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 15);
            this.label2.TabIndex = 15;
            this.label2.Text = "进入IP：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(296, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "房间ID：";
            // 
            // rtbMsg
            // 
            this.rtbMsg.Location = new System.Drawing.Point(904, 139);
            this.rtbMsg.Margin = new System.Windows.Forms.Padding(4);
            this.rtbMsg.Name = "rtbMsg";
            this.rtbMsg.Size = new System.Drawing.Size(492, 686);
            this.rtbMsg.TabIndex = 14;
            this.rtbMsg.Text = "";
            // 
            // txtIpNums
            // 
            this.txtIpNums.Location = new System.Drawing.Point(614, 34);
            this.txtIpNums.Margin = new System.Windows.Forms.Padding(4);
            this.txtIpNums.Name = "txtIpNums";
            this.txtIpNums.Size = new System.Drawing.Size(71, 25);
            this.txtIpNums.TabIndex = 10;
            this.txtIpNums.Text = "1";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(832, 34);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 29);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "退出";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRoomAdd
            // 
            this.btnRoomAdd.Location = new System.Drawing.Point(704, 34);
            this.btnRoomAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnRoomAdd.Name = "btnRoomAdd";
            this.btnRoomAdd.Size = new System.Drawing.Size(100, 29);
            this.btnRoomAdd.TabIndex = 13;
            this.btnRoomAdd.Text = "添加";
            this.btnRoomAdd.UseVisualStyleBackColor = true;
            this.btnRoomAdd.Click += new System.EventHandler(this.btnRoomAdd_Click);
            // 
            // txtRoomId
            // 
            this.txtRoomId.Location = new System.Drawing.Point(375, 33);
            this.txtRoomId.Margin = new System.Windows.Forms.Padding(4);
            this.txtRoomId.Name = "txtRoomId";
            this.txtRoomId.Size = new System.Drawing.Size(136, 25);
            this.txtRoomId.TabIndex = 11;
            this.txtRoomId.Text = "99999";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(136, 38);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(141, 23);
            this.comboBox1.TabIndex = 9;
            // 
            // rtbGift
            // 
            this.rtbGift.Location = new System.Drawing.Point(375, 139);
            this.rtbGift.Margin = new System.Windows.Forms.Padding(4);
            this.rtbGift.Name = "rtbGift";
            this.rtbGift.Size = new System.Drawing.Size(368, 686);
            this.rtbGift.TabIndex = 14;
            this.rtbGift.Text = "";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1409, 861);
            this.Controls.Add(this.dgvRooms);
            this.Controls.Add(this.lblIpsNums);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbGift);
            this.Controls.Add(this.rtbMsg);
            this.Controls.Add(this.txtIpNums);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRoomAdd);
            this.Controls.Add(this.txtRoomId);
            this.Controls.Add(this.comboBox1);
            this.Name = "FrmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvRooms;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Label lblIpsNums;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbMsg;
        private System.Windows.Forms.TextBox txtIpNums;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRoomAdd;
        private System.Windows.Forms.TextBox txtRoomId;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.RichTextBox rtbGift;
    }
}

