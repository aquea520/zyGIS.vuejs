using Lilee.GeneratorCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCodeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //123
            abpGenCode.GenCode();
        }

        private static void NewMethod()
        {
            Console.WriteLine("请输入要生成的表名：");
            string tableName = Console.ReadLine();
            string tableUpperName = tableName;
            string filePath = string.Empty;
            string csprojPath = @"F:\Portal\FixDomain\FixDomain.csproj";
            string path = csprojPath + "\\..\\";
            string databaseName = "zyGISDb";
            ListPage page = new ListPage("", "", "");//GreateListPageFromUI(rows, tableName);
            List<KeyValuePair<string, object>> arg = new List<KeyValuePair<string, object>>();
            arg.Add(new KeyValuePair<string, object>(
                            "parameter1", new Parameter() { page = page, TableName = tableName, DatabaseName = databaseName }
                        )
                    );

            CreateModelFile(tableName, filePath, path, tableUpperName, arg);
            CreateDomainMapFile(tableName, filePath, path, tableUpperName, arg);

            EditCsproj.GenCsproj(csprojPath, "");
            EditCsproj.GenCsproj(csprojPath, "");

            //执行编译命令
            Cmd cmd = new Cmd();
            cmd.RunCmd(@"F:\GenCodeConsole\1.bat");
        }

        /// <summary>
        /// 生成or更新实体层文件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filePath"></param>
        /// <param name="path"></param>
        /// <param name="tableUpperName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static string CreateModelFile(string tableName, string filePath, string path, string tableUpperName, List<KeyValuePair<string, object>> arg)
        {
            //实体类
            filePath = string.Format(path + @"{0}.cs", tableName);
            CreateFileFromTemp("Model1", arg, filePath);
            return filePath;
        }

        /// <summary>
        /// 生成DomainMap
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filePath"></param>
        /// <param name="path"></param>
        /// <param name="tableUpperName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static string CreateDomainMapFile(string tableName, string filePath, string path, string tableUpperName, List<KeyValuePair<string, object>> arg)
        {
            //实体类
            filePath = string.Format(path + @"{0}Map.cs", tableName);
            CreateFileFromTemp("Data\\MapConfig1", arg, filePath);
            return filePath;
        }

        /// <summary>
        /// 根据模板自动创建查询页代码文件，如果代码文件存在则覆盖
        /// </summary>
        /// <param name="page">UI数据源</param>
        /// <param name="tempName">模板名称</param>
        /// <param name="tableName">数据库表名</param>
        /// <param name="tableUpperName">数据库表简称</param>
        /// <param name="arg">模板参数</param>
        /// <param name="path">生成文件的路径</param>
        private static void CreateFileFromTemp(string tempName, List<KeyValuePair<string, object>> arg, string path)
        {
            CodeGenerator codeGenerator = new CodeGenerator(arg);

            string CodeString = codeGenerator.GetCode(tempName);

            string directoryName = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.File.Exists(directoryName))
                System.IO.Directory.CreateDirectory(directoryName);

            System.IO.File.WriteAllText(path, CodeString, System.Text.Encoding.UTF8);
        }
    }
}
