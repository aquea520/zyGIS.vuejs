using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lilee.GeneratorCore
{
    [Serializable]
    public class Parameter
    {
        public string DatabaseName { get; set; }

        public string TableName { get; set; }

        public List<DbColumn> TableColumns { get; set; }

        public ListPage page  { get; set; }

        public string ProjName { get; set; }

        public string GetUpperName(string name)
        {
            if (name.Contains("adm_"))
            {
                name = TransferNameToUpper(name.Replace("adm_", ""));
            }
            if (name.Contains("_Info"))
            {
                name = TransferNameToUpper(name.Replace("_Info", ""));
            }
            if (name.Contains("_info"))
            {
                name = TransferNameToUpper(name.Replace("_info", ""));
            }
            if (name.Contains("_"))
            {
                var list = name.Split('_');
                name = string.Empty;
                foreach (var splitName in list)
                {
                    name += TransferNameToUpper(splitName);
                }
            }
            return name;
        }

        public string GetLowerName(string name)
        {
            if (name.Contains("adm_"))
            {
                name = name.Replace("adm_", "");
            }
            if (name.Contains("_Info"))
            {
                name = name.Replace("_Info", "");
            }
            if (name.Contains("_info"))
            {
                name = name.Replace("_info", "");
            }
            if (name.Contains("_"))
            {
                var list = name.Split('_');
                name = string.Empty;
                foreach (var splitName in list)
                {
                    if (!string.IsNullOrEmpty(name))
                        name += TransferNameToUpper(splitName);
                    else
                        name += splitName;
                }
            }
            return name;
        }

        public string TransferNameToUpper(string name)
        {
            return name.Substring(0, 1).ToUpper()
                   + name.Substring(1, name.Length - 1);
        }
    }
    [Serializable]
    public class DataGridColumn
    {
        public string CHName { get; set; }
        public string ColumnName { get; set; }
        public string CSharpType { get; set; }
        public Nullable<bool> IsNullable { get; set; }
        public Nullable<int> Width { get; set; }
        public string Align { get; set; }
        public bool IsFormat { get; set; }
        public string Formatter { get; set; }
        public string EndChar { get; set; }
        public string FKTable { get; set; }

        public string ColumnDtoName { get; set; }
    }
    [Serializable]
    public class FormColumn
    {
        public string CHName { get; set; }
        public string ColumnName { get; set; }
        public string HtmlType { get; set; }
        public int? LabelWidth { get; set; }
        public int? Width { get; set; }
        public int? Space { get; set; }
        public Nullable<bool> Newline { get; set; }
        public int? Sort { get; set; }
        public string CSharpType { get; set; }
        public Nullable<bool> IsNullable { get; set; }
        public string FKTable { get; set; }
    }
    [Serializable]
    public class SelectColumn
    {
        public string CHName { get; set; }
        public string ColumnName { get; set; }
        public string HtmlType { get; set; }
        public string Oper { get; set; }
        public string EndChar { get; set; }
        public string SelectCategory { get; set; }
        public Nullable<bool> Newline { get; set; }
    }
    [Serializable]
    public class ListPage
    {
        /// <summary>
        /// 列表名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 请求Action名称
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string MoudleName { get; set; }
        /// <summary>
        /// 查询表单
        /// </summary>
        public List<SelectColumn> SelectColumns { get; set; }
        /// <summary>
        /// datagrid列
        /// </summary>
        public List<DataGridColumn> DgColumns { get; set; }
        /// <summary>
        /// 新增表单
        /// </summary>
        public List<FormColumn> FormColumns { get; set; }

        public ListPage(string title, string actionName, string moudleName)
        {
            Title = title;
            ActionName = actionName;
            MoudleName = moudleName;
            FormColumns = new List<FormColumn>();
            SelectColumns = new List<SelectColumn>();
            DgColumns = new List<DataGridColumn>();
        }
    }
}
