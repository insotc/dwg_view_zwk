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
        public const string DIR_PREFIX = "DIR#_";
        public const string ENT_PREFIX = "ENT#_";
        private const int PREFIX_LENGTH = 5;

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
            TreeNode tnA = tv.Nodes.Add("全部类别");
            tnA.Tag =new TreeNodeTag(TreeNodeType.ROOTNODE, "");
            getMyDirectories(path, tnA);
        }
        private void getMyDirectories(string path,TreeNode tn)
        {
            string[] directories = Directory.GetDirectories(path);
           /* foreach(string dir in directories)
            {
                MessageBox.Show(dir);
            }*/
            

            if(Directory.Exists(path)==false)
            {
                return;
            }
            //show Leavel 1 files and folders
            //foreach(string fileName in fileNames)
            //{

            //    TreeNode tnfile = tn.Nodes.Add(Path.GetFileName(fileName));
            //    tnfile.Tag = new TreeNodeTag(TreeNodeType.FILE, "");
            //}
            foreach(string directory in directories)
            {
                string dirName = Path.GetFileName(directory);
                //MessageBox.Show("dirname:"+dirName);
                if (is_dir_prefix(dirName))
                {
                    string text = remove_prefix(dirName);
                    TreeNode tnfloder = tn.Nodes.Add(text);
                    tnfloder.Tag = new TreeNodeTag(TreeNodeType.DIRECTORY, "");
                    //search current directory
                    getMyDirectories(directory, tnfloder);
                }
                else if (is_ent_prefix(dirName))
                {
                    string text = remove_prefix(dirName);
                    TreeNode tnfile = tn.Nodes.Add(text);
                    tnfile.Tag = new TreeNodeTag(TreeNodeType.FILE, "");
                }
               
            }
        }

        public static string remove_prefix(string str)
        {
            if(str.Length >= PREFIX_LENGTH)
            {
                if (str.Substring(0, PREFIX_LENGTH).Equals(DIR_PREFIX) || str.Substring(0, PREFIX_LENGTH).Equals(ENT_PREFIX))
                {
                    return str.Substring(PREFIX_LENGTH);
                }
            }
            return str;
        }

        public bool is_dir_prefix(string str)
        {
            if (str.Length >= PREFIX_LENGTH && str.Substring(0, PREFIX_LENGTH).Equals(DIR_PREFIX))
            {
                return true;
            }
            return false;
        }

        public bool is_ent_prefix(string str)
        {
            if (str.Length >= PREFIX_LENGTH && str.Substring(0, PREFIX_LENGTH).Equals(ENT_PREFIX))
            {
                return true;
            }
            return false;
        }
    }
}
