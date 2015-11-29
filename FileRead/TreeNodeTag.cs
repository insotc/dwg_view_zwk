using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRead
{
    class TreeNodeTag
    {
        public TreeNodeTag(TreeNodeType type, string text)
        {
            this.type = type;
            this.lasttimeText = text;
        }

        public TreeNodeType type;
        public string lasttimeText;
        public string fileNameWithType;
    }
}
