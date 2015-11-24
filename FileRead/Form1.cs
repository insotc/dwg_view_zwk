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
            //打开欢迎界面
            WelcomeForm welcomeForm = new WelcomeForm();
            welcomeForm.ShowDialog();

            //初始化treeView为默认路径下
            string defaultParh = Application.StartupPath;
            if(Directory.Exists(defaultParh + "\\" + "Data"))
            {
                defaultParh += "\\" + "Data";
            }
            toolStripLabel1.Text = defaultParh;
            bc.listFolders(defaultParh, treeView1);


            //bc.listComboBoxMyComputer(toolStripComboBox1);
            //axMxDrawX1.EnableToolBarButton("新建",false);

            //axMxDrawX1.HideToolBarControl("常用工具", "新建,重作", true, true);
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
            if(((TreeNodeTag)e.Node.Tag).type == TreeNodeType.FILE)
            {
                if (isCADFileFormat( getFileFormatName( e.Node.Text)))
                {
                    pictureBox1.Hide();
                    axMxDrawX1.Show();
                    //显示路径
                    string openfilepath = getNodeFullPath(e.Node);
                    axMxDrawX1.OpenDwgFile(openfilepath);
                }

                else if (isImageFileFormat(getFileFormatName(e.Node.Text)))
                {
                    pictureBox1.Show();
                    axMxDrawX1.Hide();
                    //显示路径
                    string openfilepath = getNodeFullPath(e.Node);
                    pictureBox1.Image = Image.FromFile(openfilepath);
                    axMxDrawX1.OpenDwgFile(openfilepath);
                }
            }
        }
        //鼠标右击倒数第二级（文件夹）节点时，选择：添加下级、重命名、删除
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if ( ((TreeNodeTag)e.Node.Tag).type == TreeNodeType.DIRECTORY)
            {
                if (e.Button == MouseButtons.Right)
                {
                    TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);
                    if (tn != null)
                    {
                        treeView1.SelectedNode = tn;
                        ContextMenu popMenu = new ContextMenu();
                        popMenu.MenuItems.Add("添加子分类", new EventHandler(AddChildDir_Click));
                        popMenu.MenuItems.Add("添加文件", new EventHandler(AddChildFile_Click));
                        popMenu.MenuItems.Add("重命名", new EventHandler(ReName_Click));
                        popMenu.MenuItems.Add("删除", new EventHandler(DeleteDir_Click));
                        tn.ContextMenu = popMenu;
                    }
                }
            }
            else if (((TreeNodeTag)e.Node.Tag).type == TreeNodeType.FILE)
            {
                if(e.Button == MouseButtons.Right)
                {
                    TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);
                    if (tn != null)
                    {
                        treeView1.SelectedNode = tn;
                        ContextMenu popMenu = new ContextMenu();
                        popMenu.MenuItems.Add("重命名", new EventHandler(ReName_Click));
                        popMenu.MenuItems.Add("删除", new EventHandler(DeleteFile_Click));
                        tn.ContextMenu = popMenu;
                    }
                       
                }
            }
            else if (((TreeNodeTag)e.Node.Tag).type == TreeNodeType.ROOTNODE)
            {
                if (e.Button == MouseButtons.Right)
                {
                    TreeNode tn = treeView1.GetNodeAt(e.X, e.Y);
                    if (tn != null)
                    {
                        treeView1.SelectedNode = tn;
                        ContextMenu popMenu = new ContextMenu();
                        popMenu.MenuItems.Add("添加子分类", new EventHandler(AddChildDir_Click));
                        popMenu.MenuItems.Add("添加文件", new EventHandler(AddChildFile_Click));
                        tn.ContextMenu = popMenu;
                    }
                }
            }

        }

        private void ReName_Click(object sender, EventArgs e)
        {
            treeView1.LabelEdit = true;
            treeView1.SelectedNode.BeginEdit();
        }

        private void DeleteDir_Click(object sender, EventArgs e)
        {
            int cnt = calAffectedFileCnt(treeView1.SelectedNode);
            if (MessageBox.Show("确定删除目录：" + treeView1.SelectedNode.Text + " ? \n将会删除 "+cnt+" 个文件", "删除文件", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string path = getNodeFullPath(treeView1.SelectedNode);
                Directory.Delete(path, true);
                treeView1.Nodes.Remove(treeView1.SelectedNode);
            }

        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定删除文件："+ treeView1.SelectedNode.Text+" ?", "删除文件" , MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string path = getNodeFullPath(treeView1.SelectedNode);
                File.Delete(path);
                treeView1.Nodes.Remove(treeView1.SelectedNode);
            }
          
        }

        private void AddChildDir_Click(object sender, EventArgs e)
        {
            treeView1.SelectedNode.Expand();
            TreeNode tnNew = new TreeNode();
            tnNew.Tag = new TreeNodeTag(TreeNodeType.DIRECTORY, "");
            treeView1.SelectedNode.Nodes.Add(tnNew);
            setNodeUniqueName(tnNew);
            Directory.CreateDirectory(getNodeFullPath(tnNew));
        }

        private void AddChildFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CAD 文件 (*.dwg)|*.dwg|图片文件(*.jpg,*.gif,*.bmp,*.jpeg,*.png)|*.jpg;*.gif;*.bmp,*.jpeg,*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

                string fileSrcPath = openFileDialog.FileName;
                TreeNode curNode = treeView1.SelectedNode;
                string fileDesPath = getNodeFullPath(curNode);
                string desFileName = openFileDialog.SafeFileName;

                if (nodeHasChildWithSameName(curNode, desFileName))
                {
                    int startIdx = 1;
                    while (nodeHasChildWithSameName(curNode, desFileName + "(" + startIdx + ")"))
                    {
                        startIdx++;
                    }
                    desFileName += "(" + startIdx + ")";
                }
                fileDesPath += "\\" + desFileName;
                System.IO.File.Copy(fileSrcPath, fileDesPath);

                treeView1.SelectedNode.Expand();
                TreeNode tnNew = new TreeNode(desFileName);
                tnNew.Tag = new TreeNodeTag(TreeNodeType.FILE, "");
                treeView1.SelectedNode.Nodes.Add(tnNew);
            }
           
           
        }
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            
            e.Node.EndEdit(true);
            treeView1.LabelEdit = false;
           
            if (nodeHasSibWithName(e.Node, e.Label))
            {
                MessageBox.Show("该名称已存在，请重新命名！");
                e.CancelEdit = true;
            }
            else
            {
                string desPath = getNodeFullPath(e.Node.Parent) + "\\" + e.Label;
                string srcPath = getNodeFullPath(e.Node.Parent) + "\\" + e.Node.Text;
                TreeNodeType type = ((TreeNodeTag)e.Node.Tag).type;
                if(type == TreeNodeType.DIRECTORY)
                {
                    if(Directory.Exists(srcPath) == true && Directory.Exists(desPath) == false)
                    {
                        Directory.Move(srcPath, desPath);
                    }
                }
                else if (type == TreeNodeType.FILE)
                {
                    if (File.Exists(srcPath) == true && File.Exists(desPath) == false)
                    {
                        Directory.Move(srcPath, desPath);
                    }
                }
            }

            string nodePath = toolStripLabel1.Text + e.Node.Parent.FullPath.Substring(3) + "\\" + e.Label;
            //if (Directory.Exists(nodePath))
            //{
            //    MessageBox.Show("类别名已存在，请重新命名！");
            //    //treeView1.SelectedNode.BeginEdit();
            //}
            //else
            //{
            //    //Directory.CreateDirectory(nodePath);
                
            //}
        }

        private bool nodeHasSibWithName(TreeNode node, string name)
        {
            TreeNode tmpNode = node.PrevNode;
            while(tmpNode != null)
            {
                if (tmpNode.Text == name) return true;
                tmpNode = tmpNode.PrevNode;
            }
            tmpNode = node.NextNode;
            while (tmpNode != null)
            {
                if (tmpNode.Text == name) return true;
                tmpNode = tmpNode.NextNode;
            }
            return false;
        }

        private bool nodeHasChildWithSameName(TreeNode node, string name)
        {
            if (node.FirstNode == null) return false;
            if (node.FirstNode.Text == name) return true;
            return nodeHasSibWithName(node.FirstNode, name);
        }

        private void setNodeUniqueName(TreeNode node)
        {
            string baseText = "empty";
            if(((TreeNodeTag)node.Tag).type == TreeNodeType.FILE)
            {
                baseText = "新建文件";
            } else
            {
                baseText = "新建类别";
            }

            int startIdx = 1;
            while (true)
            {
                string newText = baseText + startIdx;
                if (nodeHasSibWithName(node, newText))
                {
                    startIdx++;
                }
                else
                {
                    node.Text = newText;
                    return;
                }
            }
        }

        private bool isStrEmpty(string str)
        {
            if(str == null || str.Length == 0){ return true; }
            return false;
        }

        private string getNodeFullPath(TreeNode node)
        {
            int startIdx = node.FullPath.IndexOf("\\");
            if(startIdx == -1)
            {
                startIdx = node.FullPath.Length;
            }
            string fpath = toolStripLabel1.Text + node.FullPath.Substring(startIdx);
            return fpath;
        }

        private int calAffectedFileCnt(TreeNode node)
        {
            int cnt = 0;
            if(((TreeNodeTag)node.Tag).type == TreeNodeType.FILE)
            {
                cnt++;
            }
            TreeNode child = node.FirstNode;
            while(child != null)
            {
                cnt += calAffectedFileCnt(child);
                child = child.NextNode;
            }
            return cnt;
        }

        private bool isCADFileFormat(string fileFormatName)
        {
            if (fileFormatName == "dwg") return true;
            return false;
        }

        private bool isImageFileFormat(string fileFormatName)
        {
            if (fileFormatName == "bmp" || fileFormatName == "jpg" || fileFormatName == "jpeg" || fileFormatName == "gif" || fileFormatName == "png") return true;
            return false;
        }

        private string getFileFormatName(string str)
        {
            int idx = str.LastIndexOf(".");
            if (idx == -1) return "";
            string formatStr = str.Substring(idx + 1);
            return formatStr;
        }

    }
}
