using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilee.GeneratorCore
{
    public class CodeGenerator
    {
        private List<KeyValuePair<string, object>> _arg;
        /// <summary>
        /// 需要传递到模板的参数
        /// </summary>
        /// <param name="arg"></param>
        public CodeGenerator(List<KeyValuePair<string, object>> arg)
        {
            _arg = arg;
        }

        /// <summary>
        /// 根据模板名称及传递进去的参数，返回生成的代码
        /// </summary>
        /// <param name="templateName">模板名称</param>
        /// <returns>生成的代码</returns>
        public string GetCode(string templateName)
        {
            string input = GetTemplate(templateName);
            CustomTextTemplatingEngineHost host = new CustomTextTemplatingEngineHost();
            host.TemplateFileValue = "test.tt";
            host.Session = new TextTemplatingSession();
            foreach (KeyValuePair<string, object> item in _arg)
            {
                host.Session.Add(item);
            }

            Engine engine = new Engine();
            string codeString = engine.ProcessTemplate(input, host);

            string msg = string.Empty;
            foreach (CompilerError error in host.Errors)
            {
                msg += error.ToString() + "/br";
            }

            return codeString;
        }

        /// <summary>
        /// 根据模板名称获取模板内容
        /// </summary>
        /// <param name="name">模板名称</param>
        /// <returns>模板字符串</returns>
        public string GetTemplate(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new Exception("文件名不能为空!"); }

            string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            if (!System.IO.File.Exists(path + @"..\..\Template\" + name + ".txt"))
                throw new Exception("文件不存在!");

            return System.IO.File.ReadAllText(path + @"..\..\Template\" + name + ".txt");
        }
    }
}
