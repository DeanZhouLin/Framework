namespace WinTest
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.rTxtRes = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNodesXPath = new System.Windows.Forms.TextBox();
            this.txtSingleNodeXpath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGetHtml = new System.Windows.Forms.Button();
            this.txtParams = new System.Windows.Forms.TextBox();
            this.txtUrlTemplate = new System.Windows.Forms.TextBox();
            this.txtNodeIndex = new System.Windows.Forms.TextBox();
            this.btnSelectNodes = new System.Windows.Forms.Button();
            this.btnSelectNode = new System.Windows.Forms.Button();
            this.dgvRes = new System.Windows.Forms.DataGridView();
            this.txtAttributeName = new System.Windows.Forms.TextBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRes)).BeginInit();
            this.SuspendLayout();
            // 
            // rTxtRes
            // 
            this.rTxtRes.Location = new System.Drawing.Point(918, 24);
            this.rTxtRes.Name = "rTxtRes";
            this.rTxtRes.Size = new System.Drawing.Size(528, 554);
            this.rTxtRes.TabIndex = 0;
            this.rTxtRes.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "URL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "NodesXPath";
            // 
            // txtNodesXPath
            // 
            this.txtNodesXPath.Location = new System.Drawing.Point(105, 67);
            this.txtNodesXPath.Name = "txtNodesXPath";
            this.txtNodesXPath.Size = new System.Drawing.Size(692, 21);
            this.txtNodesXPath.TabIndex = 4;
            this.txtNodesXPath.Text = "//*[@id=\'resultList\']/tr[@class=\'tr0\']";
            // 
            // txtSingleNodeXpath
            // 
            this.txtSingleNodeXpath.Location = new System.Drawing.Point(105, 105);
            this.txtSingleNodeXpath.Name = "txtSingleNodeXpath";
            this.txtSingleNodeXpath.Size = new System.Drawing.Size(295, 21);
            this.txtSingleNodeXpath.TabIndex = 6;
            this.txtSingleNodeXpath.Text = "/td[@class=\'td1\']/a";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "SingleNodeXPath";
            // 
            // btnGetHtml
            // 
            this.btnGetHtml.Location = new System.Drawing.Point(815, 24);
            this.btnGetHtml.Name = "btnGetHtml";
            this.btnGetHtml.Size = new System.Drawing.Size(97, 23);
            this.btnGetHtml.TabIndex = 7;
            this.btnGetHtml.Text = "执行";
            this.btnGetHtml.UseVisualStyleBackColor = true;
            this.btnGetHtml.Click += new System.EventHandler(this.btnGetHtml_Click);
            // 
            // txtParams
            // 
            this.txtParams.Location = new System.Drawing.Point(683, 26);
            this.txtParams.Name = "txtParams";
            this.txtParams.Size = new System.Drawing.Size(114, 21);
            this.txtParams.TabIndex = 8;
            this.txtParams.Text = "c#|1";
            // 
            // txtUrlTemplate
            // 
            this.txtUrlTemplate.Location = new System.Drawing.Point(105, 26);
            this.txtUrlTemplate.Name = "txtUrlTemplate";
            this.txtUrlTemplate.Size = new System.Drawing.Size(572, 21);
            this.txtUrlTemplate.TabIndex = 1;
            this.txtUrlTemplate.Text = "http://search.51job.com/jobsearch/search_result.php?jobarea=070500&keyword={0}&cu" +
    "rr_page={1}";
            // 
            // txtNodeIndex
            // 
            this.txtNodeIndex.Location = new System.Drawing.Point(418, 105);
            this.txtNodeIndex.Name = "txtNodeIndex";
            this.txtNodeIndex.Size = new System.Drawing.Size(114, 21);
            this.txtNodeIndex.TabIndex = 9;
            this.txtNodeIndex.Text = "0";
            // 
            // btnSelectNodes
            // 
            this.btnSelectNodes.Location = new System.Drawing.Point(815, 65);
            this.btnSelectNodes.Name = "btnSelectNodes";
            this.btnSelectNodes.Size = new System.Drawing.Size(97, 23);
            this.btnSelectNodes.TabIndex = 10;
            this.btnSelectNodes.Text = "执行";
            this.btnSelectNodes.UseVisualStyleBackColor = true;
            this.btnSelectNodes.Click += new System.EventHandler(this.btnSelectNodes_Click);
            // 
            // btnSelectNode
            // 
            this.btnSelectNode.Location = new System.Drawing.Point(815, 103);
            this.btnSelectNode.Name = "btnSelectNode";
            this.btnSelectNode.Size = new System.Drawing.Size(97, 23);
            this.btnSelectNode.TabIndex = 11;
            this.btnSelectNode.Text = "执行";
            this.btnSelectNode.UseVisualStyleBackColor = true;
            this.btnSelectNode.Click += new System.EventHandler(this.btnSelectNode_Click);
            // 
            // dgvRes
            // 
            this.dgvRes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRes.Location = new System.Drawing.Point(3, 132);
            this.dgvRes.Name = "dgvRes";
            this.dgvRes.RowTemplate.Height = 23;
            this.dgvRes.Size = new System.Drawing.Size(909, 446);
            this.dgvRes.TabIndex = 12;
            // 
            // txtAttributeName
            // 
            this.txtAttributeName.Location = new System.Drawing.Point(563, 105);
            this.txtAttributeName.Name = "txtAttributeName";
            this.txtAttributeName.Size = new System.Drawing.Size(234, 21);
            this.txtAttributeName.TabIndex = 13;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(282, 252);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(250, 250);
            this.webBrowser1.TabIndex = 14;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1449, 581);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.txtAttributeName);
            this.Controls.Add(this.dgvRes);
            this.Controls.Add(this.btnSelectNode);
            this.Controls.Add(this.btnSelectNodes);
            this.Controls.Add(this.txtNodeIndex);
            this.Controls.Add(this.txtParams);
            this.Controls.Add(this.btnGetHtml);
            this.Controls.Add(this.txtSingleNodeXpath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNodesXPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUrlTemplate);
            this.Controls.Add(this.rTxtRes);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dgvRes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox rTxtRes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNodesXPath;
        private System.Windows.Forms.TextBox txtSingleNodeXpath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGetHtml;
        private System.Windows.Forms.TextBox txtParams;
        private System.Windows.Forms.TextBox txtUrlTemplate;
        private System.Windows.Forms.TextBox txtNodeIndex;
        private System.Windows.Forms.Button btnSelectNodes;
        private System.Windows.Forms.Button btnSelectNode;
        private System.Windows.Forms.DataGridView dgvRes;
        private System.Windows.Forms.TextBox txtAttributeName;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}

