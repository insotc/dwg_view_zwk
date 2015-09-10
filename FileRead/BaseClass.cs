using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileRead
{
    class BaseClass
    {
        //在combox中显示“我的电脑”下的磁盘供选择
        public void listComboBoxMyComputer(ToolStripComboBox tscb)
        {
            string[] logicdrives = System.IO.Directory.GetLogicalDrives();
            for (int i = 0; i < logicdrives.Length; i++)
            {
                tscb.Items.Add(logicdrives[i]);
                tscb.SelectedIndex = 0;
            }
        }
        //根据指定路径path，在treeview1中显示路径下的所有文件和文件夹
        public void listFolders(string path,TreeView tv)
        {
            TreeNode tnA = tv.Nodes.Add("ALL");
            TreeNode tnx = new TreeNode();
            getMyDirectories(path, tnA);
        }
        private void getMyDirectories(string path,TreeNode tn)
        {
            string[] fileNames = Directory.GetFiles(path);
            string[] directories = Directory.GetDirectories(path);

            if(Directory.Exists(path)==false)
            {
                return;
            }
            //show Leavel 1 files and folders
            foreach(string fileName in fileNames)
            {
                TreeNode tnfile = tn.Nodes.Add(Path.GetFileName(fileName));
            }
            foreach(string directory in directories)
            {
                TreeNode tnfloder = tn.Nodes.Add(directory.Substring(directory.LastIndexOf("\\")+1));
                //search current directory
                getMyDirectories(directory, tnfloder);
            }
        }
    }
}
