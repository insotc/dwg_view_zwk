using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRead
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        BaseClass bc = new BaseClass();
        private void Form1_Load(object sender, EventArgs e)
        {
            //初始化treeView为默认路径下
            string defaultParh = Application.StartupPath;
            toolStripLabel1.Text = defaultParh;
            bc.listFolders(defaultParh, treeView1);

            
            //bc.listComboBoxMyComputer(toolStripComboBox1);
            axMxDrawX1.EnableToolBarButton("新建",false);
            //axMxDrawX1.HideToolBarControl("新建", "新建",false,false);
        }


        //选择指定路径
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripLabel1.Text = folderBrowserDialog1.SelectedPath;
                treeView1.Nodes.Clear();
                bc.listFolders(toolStripLabel1.Text, treeView1);
            }
            
        }
        //刷新树结构
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            bc.listFolders(toolStripLabel1.Text, treeView1);
        }
        //点击叶节点时，显示dwg文件或图片文件
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Node.Nodes.Count != 0)
            {
                return;
            }
            else
            {

                //显示路径
                treeView1.PathSeparator = "\\";
                string openfile = (toolStripLabel1.Text + e.Node.FullPath.Substring(3)).Replace("\\", "/");
                axMxDrawX1.OpenDwgFile(openfile);
                //测试图片
                //string file = "C:/Users/ceedi/Desktop/图纸目录.dwg";
                //axMxDrawX1.OpenDwgFile(file);
            }
        }
        //鼠标右击倒数第二级（文件夹）节点时，选择：添加下级、重命名、删除
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Nodes.Count == 0)
            {
                return;
            }
            if(e.Button == MouseButtons.Right)
            {
                TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);
                if (tn != null)
                    treeView1.SelectedNode = tn;
                ContextMenu popMenu = new ContextMenu();
                popMenu.MenuItems.Add("添加下级", new EventHandler(AddNextLevel_Click));
                popMenu.MenuItems.Add("重命名", new EventHandler(ReName_Click));
                popMenu.MenuItems.Add("删除", new EventHandler(Delete_Click));
                tn.ContextMenu = popMenu;
            }
        }

        private void ReName_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            treeView1.LabelEdit = true;
            treeView1.SelectedNode.BeginEdit();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            treeView1.Nodes.Remove(treeView1.SelectedNode);
        }

        private void AddNextLevel_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            treeView1.LabelEdit = true;
            treeView1.SelectedNode.Expand();
            TreeNode tnNew = new TreeNode("新建类别");
            treeView1.SelectedNode.Nodes.Add(tnNew);
            tnNew.BeginEdit();
        }
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {

            e.Node.EndEdit(true);
            treeView1.LabelEdit = false;
            string nodePath = toolStripLabel1.Text + e.Node.Parent.FullPath.Substring(3) + "\\" + e.Label;
            if (Directory.Exists(nodePath))
            {
                MessageBox.Show("类别名已存在，请重新命名！");
                //treeView1.SelectedNode.BeginEdit();
            }
            else
            {
                //Directory.CreateDirectory(nodePath);
                
            }
        }


       

        
    }
}
