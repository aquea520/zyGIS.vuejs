using Lilee.GeneratorCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenCodeConsole
{
    public static class abpGenCode
    {
        public static string SLN_Path = @"E:\03实现\zyGIS\zyGIS\";
        public static string Application_Path = @"src\zyGIS.Application\";
        public static string Domain_Path = @"src\zyGIS.Core\";
        public static string EF_Path = @"src\zyGIS.EntityFrameworkCore\";
        public static string Proj_Name = "zyGIS";
        public static string DBSET_Path = string.Format(@"EntityFrameworkCore\{0}DbContext.cs",Proj_Name);
        static abpGenCode() {

        }

        public static void GenCode() {
            Console.WriteLine("请输入要生成的表名：");
            string tableName = Console.ReadLine();
            string databaseName = "zyGISDb";
            ListPage page = new ListPage("", "", "");//GreateListPageFromUI(rows, tableName);
            List<KeyValuePair<string, object>> arg = new List<KeyValuePair<string, object>>();
            arg.Add(new KeyValuePair<string, object>(
                            "parameter1", new Parameter() { page = page, TableName = tableName, DatabaseName = databaseName ,ProjName = Proj_Name}
                        )
                    );
            Console.WriteLine("正在生成Model文件...");
            CreateModelFile(tableName, arg);
            Console.WriteLine("正在生成DTO文件...");
            CreateDtoFile(tableName, arg);
            Console.WriteLine("正在生成Service文件...");
            CreateServiceFile(tableName, arg);
            Console.WriteLine("正在替换dbset文件...");
            ReplaceDbContext_AddDbSetForTable(SLN_Path + EF_Path+ DBSET_Path, tableName);
            Console.WriteLine("正在执行批处理文件...");

            //执行编译命令
            Cmd cmd = new Cmd();
            string output = cmd.RunCmd(SLN_Path+"1.bat");

            Console.WriteLine(output);
            Console.WriteLine("按回车退出");
            ConsoleKeyInfo kinfo = Console.ReadKey();
            Environment.Exit(0);
        }

        public static void ReplaceDbContext_AddDbSetForTable(string path,string tableName) {
            string str = System.IO.File.ReadAllText(path,System.Text.Encoding.UTF8);
            str = str.Replace(" \r\n\t\t" + string.Format("public virtual DbSet<{1}.{0}s.{0}> {0}s ", tableName,Proj_Name) + "{ get; set; }","");
            str = str.Replace("*/", @"*/" +" \r\n\t\t" + string.Format("public virtual DbSet<{1}.{0}s.{0}> {0}s ",tableName, Proj_Name) + "{ get; set; }");
            System.IO.File.WriteAllText(path, str,System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 生成DTO
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filePath"></param>
        /// <param name="path"></param>
        /// <param name="tableUpperName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static void CreateDtoFile(string tableName, List<KeyValuePair<string, object>> arg)
        {
            //实体类
            string filePath = string.Format(SLN_Path + Application_Path + @"{0}s\Dto\",tableName);
            string fileName = string.Format(@"Create{0}Dto.cs", tableName);

            CreateFileFromTemp("abpTemplate\\application\\Dto\\CreateModelDto", arg,  filePath+ fileName);

            fileName = string.Format(@"{0}Dto.cs", tableName);
            CreateFileFromTemp("abpTemplate\\application\\Dto\\ModelDto", arg, filePath + fileName);

            fileName = string.Format(@"{0}MapProfile.cs", tableName);
            CreateFileFromTemp("abpTemplate\\application\\Dto\\ModelMapProfile", arg, filePath + fileName);

            //.net core 不需要手动加，文件都是自动添加
            //string csprojPath = path + @"src\zyGIS.Application\zyGIS.Application.csproj";
            //EditCsproj.GenCsproj(csprojPath, string.Format(@"{0}\Dto", tableName));
            
        }

        /// <summary>
        /// 生成Service
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filePath"></param>
        /// <param name="path"></param>
        /// <param name="tableUpperName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static void CreateServiceFile(string tableName, List<KeyValuePair<string, object>> arg)
        {
            string filePath = string.Format(SLN_Path + Application_Path + @"{0}s\",tableName);
            string fileName = string.Format("{0}Service.cs", tableName);
            CreateFileFromTemp("abpTemplate\\application\\Service", arg, filePath+ fileName);

            fileName = string.Format("I{0}Service.cs", tableName);
            CreateFileFromTemp("abpTemplate\\application\\IService", arg, filePath + fileName);

            //.net core 不需要手动加，文件都是自动添加
            //string csprojPath = path + @"src\zyGIS.Application\zyGIS.Application.csproj"; ;
            //EditCsproj.GenCsproj(csprojPath, tableName);
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
        private static void CreateModelFile(string tableName, List<KeyValuePair<string, object>> arg)
        {
            string filePath = string.Format(SLN_Path + Domain_Path + @"{0}s\", tableName);
            string fileName = string.Format(@"{0}.cs", tableName);

            CreateFileFromTemp("abpTemplate\\domain\\Model", arg, filePath+ fileName);

            //.net core 不需要手动加，文件都是自动添加
            //string csprojPath = path + @"src\zyGIS.Core\zyGIS.Core.csproj"; ;
            //EditCsproj.GenCsproj(csprojPath, tableName);
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
