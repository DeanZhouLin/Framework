using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeanZhou.Framework;

namespace WinTest
{
    public partial class Form1 : Form
    {
        HttpCore hc = new HttpCore();

        private string html;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetHtml_Click(object sender, EventArgs e)
        {
            hc.SetUrl(txtUrlTemplate.Text, txtParams.Text.Split('|').Cast<object>().ToArray());
            Clipboard.SetText(hc.CurrentHttpItem.URL);
            html = rTxtRes.Text = hc.GetHtml();
        }

        private void btnSelectNodes_Click(object sender, EventArgs e)
        {
            rTxtRes.Text = hc.SelectNodesHtml(html, txtNodesXPath.Text);
        }

        private void btnSelectNode_Click(object sender, EventArgs e)
        {
            try
            {

                var nodes = hc.SelectNodes(html, txtNodesXPath.Text);
                int index = int.Parse(txtNodeIndex.Text.Trim());

                var res = nodes[index].SelectSingleNode(nodes[index].XPath + txtSingleNodeXpath.Text).InnerHtml;
                rTxtRes.Text = res;
            }
            catch (Exception)
            {

                rTxtRes.Text = "null";
            }
        }
    }
}
