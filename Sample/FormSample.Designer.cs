namespace Sample
{
    partial class FormSample
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
            this.tgvSample = new TreeGridView.TreeGridView();
            ((System.ComponentModel.ISupportInitialize)(this.tgvSample)).BeginInit();
            this.SuspendLayout();
            // 
            // tgvSample
            // 
            this.tgvSample.AllowUserToAddRows = false;
            this.tgvSample.AllowUserToDeleteRows = false;
            this.tgvSample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tgvSample.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.tgvSample.ImageList = null;
            this.tgvSample.Location = new System.Drawing.Point(12, 12);
            this.tgvSample.MultiSelect = false;
            this.tgvSample.Name = "tgvSample";
            this.tgvSample.RowHeadersVisible = false;
            this.tgvSample.RowTemplate.Height = 23;
            this.tgvSample.Size = new System.Drawing.Size(776, 426);
            this.tgvSample.TabIndex = 0;
            // 
            // FormSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tgvSample);
            this.Name = "FormSample";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sample";
            this.Load += new System.EventHandler(this.FormSample_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tgvSample)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TreeGridView.TreeGridView tgvSample;
    }
}

