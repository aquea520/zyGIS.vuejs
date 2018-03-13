using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GenCodeConsole
{
    public class EditCsproj
    {

        //在.csproj文件中导入新文件
        public static void GenCsproj(string CsprojFile,string filePath)
        {
            string COMMONROTOCOLTEST_DIR = CsprojFile.Substring(0,CsprojFile.LastIndexOf(@"\") + 1);
            //ProtocolModel目前的cs文件列表
            var files = Directory.GetFiles(COMMONROTOCOLTEST_DIR+ filePath, "*.cs");
            List<String> currFiles = new List<String>();

            foreach (var file in files)
            {
                String path = file.ToString();
                StringBuilder sb = new StringBuilder();
                sb.Append(path.Substring(path.LastIndexOf(@"\") + 1));
                currFiles.Add(sb.ToString());
            }

            //CsprojFile = @"E:\03实现\Portal\Portal\Portal.csproj";

            XmlDocument doc = new XmlDocument();
            doc.Load(CsprojFile);
            //Project节点
            XmlNodeList xnl = doc.ChildNodes[0].ChildNodes;
            if (doc.ChildNodes[0].Name.ToLower() != "project")
            {
                xnl = doc.ChildNodes[1].ChildNodes;
            }
            bool isExist = true;
            foreach (XmlNode xn in xnl)
            {
                //找到包含compile的节点
                if (xn.ChildNodes.Count > 0 && xn.ChildNodes[0].Name.ToLower() == "compile")
                {
                    isExist = false;
                    //将剩下的文件加入csproj中
                    foreach (var file in currFiles)
                    {
                        XmlElement xelKey = doc.CreateElement("Compile", doc.DocumentElement.NamespaceURI);
                        XmlAttribute xelType = doc.CreateAttribute("Include");
                        xelType.InnerText = filePath+"\\"+file;
                        xelKey.SetAttributeNode(xelType);
                        xn.AppendChild(xelKey);
                    }
                }
            }

            if (isExist) {
                XmlElement xelGroup = doc.CreateElement("ItemGroup", doc.DocumentElement.NamespaceURI);
                //将剩下的文件加入csproj中
                foreach (var file in currFiles)
                {
                    XmlElement xelKey = doc.CreateElement("Compile", doc.DocumentElement.NamespaceURI);
                    XmlAttribute xelType = doc.CreateAttribute("Include");
                    xelType.InnerText = filePath + "\\" + file;
                    xelKey.SetAttributeNode(xelType);
                    xelGroup.AppendChild(xelKey);
                }
                doc.ChildNodes[0].AppendChild(xelGroup);
            }
            doc.Save(CsprojFile);
        }
    }
}
