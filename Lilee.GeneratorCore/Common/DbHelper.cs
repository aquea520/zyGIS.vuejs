﻿using System;
using System.Xml;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;

namespace Lilee.GeneratorCore
{
    [Serializable]
    public class config
    {
        public static readonly string ConnectionString = "Server=.\\MSSQLSERVER12;Database=master;uid=sa;pwd=zyjs;";
        public static readonly string DbDatabase = "master";
    }

    [Serializable]
    public class DbHelper
    {
        #region GetDbNames
        public static List<string> GetDbNames(string connectionString)
        {
            #region SQL
            string sql = "Select Name FROM Master..SysDatabases where dbid > 4 orDER BY Name ";
            DataTable dt = GetDataTable(connectionString, sql);
            return dt.Rows.Cast<DataRow>().Select(row => row.Field<string>("Name")).ToList();
            #endregion
        }

        #endregion
        #region GetDbTables

        public static List<DbTable> GetDbTables(string connectionString, string database, string tables = null)
        {

            if (!string.IsNullOrEmpty(tables))
            {
                tables = string.Format(" and obj.name in ('{0}')", tables.Replace(",", "','"));
            }
            #region SQL
            string sql = string.Format(@"SELECT
									obj.name tablename,
									schem.name schemname,
									idx.rows,
									CAST
									(
										CASE 
											WHEN (SELECT COUNT(1) FROM sys.indexes WHERE object_id= obj.OBJECT_ID AND is_primary_key=1) >=1 THEN 1
											ELSE 0
										END 
									AS BIT) HasPrimaryKey                                         
									from {0}.sys.objects obj 
									inner join {0}.dbo.sysindexes idx on obj.object_id=idx.id and idx.indid<=1
									INNER JOIN {0}.sys.schemas schem ON obj.schema_id=schem.schema_id
									where type='U' {1}
									order by obj.name", database, tables);
            #endregion
            DataTable dt = GetDataTable(connectionString, sql);
            return dt.Rows.Cast<DataRow>().Select(row => new DbTable
            {
                TableName = row.Field<string>("tablename"),
                SchemaName = row.Field<string>("schemname"),
                Rows = row.Field<int>("rows"),
                HasPrimaryKey = row.Field<bool>("HasPrimaryKey")
            }).ToList();
        }
        #endregion

        #region GetDbColumns

        public static List<DbColumn> GetDbColumns(string connectionString, string database, string tableName, string schema = "dbo")
        {
            #region SQL
            string sql = string.Format(@"
                                    WITH indexCTE AS
                                    (
	                                    SELECT 
                                        ic.column_id,
                                        ic.index_column_id,
                                        ic.object_id    
                                        FROM {0}.sys.indexes idx
                                        INNER JOIN {0}.sys.index_columns ic ON idx.index_id = ic.index_id AND idx.object_id = ic.object_id
                                        WHERE  idx.object_id =OBJECT_ID(@tableName) AND idx.is_primary_key=1
                                    )
                                    select
									colm.column_id ColumnID,
                                    CAST(CASE WHEN indexCTE.column_id IS NULL THEN 0 ELSE 1 END AS BIT) IsPrimaryKey,
                                    colm.name ColumnName,
                                    systype.name ColumnType,
                                    colm.is_identity IsIdentity,
                                    colm.is_nullable IsNullable,
                                    cast(colm.max_length as int) ByteLength,
                                    (
                                        case 
                                            when systype.name='nvarchar' and colm.max_length>0 then colm.max_length/2 
                                            when systype.name='nchar' and colm.max_length>0 then colm.max_length/2
                                            when systype.name='ntext' and colm.max_length>0 then colm.max_length/2 
                                            else colm.max_length
                                        end
                                    ) CharLength,
                                    cast(colm.precision as int) Precision,
                                    cast(colm.scale as int) Scale,
                                    prop.value Remark
                                    from {0}.sys.columns colm
                                    inner join {0}.sys.types systype on colm.system_type_id=systype.system_type_id and colm.user_type_id=systype.user_type_id
                                    left join {0}.sys.extended_properties prop on colm.object_id=prop.major_id and colm.column_id=prop.minor_id
                                    LEFT JOIN indexCTE ON colm.column_id=indexCTE.column_id AND colm.object_id=indexCTE.object_id                                        
                                    where colm.object_id=OBJECT_ID(@tableName)
                                    order by colm.column_id", database);
            #endregion
            SqlParameter param = new SqlParameter("@tableName", SqlDbType.NVarChar, 100) { Value = string.Format("{0}.{1}.{2}", database, schema, tableName) };
            DataTable dt = GetDataTable(connectionString, sql, param);
            return dt.Rows.Cast<DataRow>().Select(row => new DbColumn()
            {
                ColumnID = row.Field<int>("ColumnID"),
                IsPrimaryKey = row.Field<bool>("IsPrimaryKey"),
                ColumnName = row.Field<string>("ColumnName"),
                ColumnType = row.Field<string>("ColumnType"),
                IsIdentity = row.Field<bool>("IsIdentity"),
                IsNullable = row.Field<bool>("IsNullable"),
                ByteLength = row.Field<int>("ByteLength"),
                CharLength = row.Field<int>("CharLength"),
                Scale = row.Field<int>("Scale"),
                Remark = row["Remark"].ToString()
            }).ToList();
        }

        #endregion


        #region GetDataTable

        public static DataTable GetDataTable(string connectionString, string commandText, params SqlParameter[] parms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = commandText;
                command.Parameters.AddRange(parms);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }

        #endregion

        #region GetTableFK
        public static List<DbTableFK> GetTableFK(string connectionString, string database, string tables = null)
        {
            if (!string.IsNullOrEmpty(tables))
            {
                tables = string.Format(" where ftable.name in ('{0}')", tables.Replace(",", "','"));
            }
            #region SQL
            string sql = string.Format(@"select fk.name fkname , ftable.name ptablename, cn.name fkcol, rtable.name ftablename from sysforeignkeys  
                            join sysobjects fk    
                            on sysforeignkeys.constid = fk.id  
                            join sysobjects ftable  
                            on sysforeignkeys.fkeyid = ftable.id  
                            join sysobjects rtable  
                            on sysforeignkeys.rkeyid = rtable.id  
                            join syscolumns cn  
                            on sysforeignkeys.fkeyid = cn.id and sysforeignkeys.fkey = cn.colid {0}", tables);
            #endregion
            DataTable dt = GetDataTable(connectionString, sql);
            return dt.Rows.Cast<DataRow>().Select(row => new DbTableFK
            {
                FkName = row.Field<string>("fkname"),
                TableName = row.Field<string>("ptablename"),
                Column = row.Field<string>("fkcol"),
                FkTableName = row.Field<string>("ftablename")
            }).ToList();
        }
        #endregion

        #region GetFKTable
        public static List<DbTableFK> GetFKTable(string connectionString, string database, string PKtables = null)
        {
            if (!string.IsNullOrEmpty(PKtables))
            {
                PKtables = string.Format(" where ftable.name in ('{0}')", PKtables.Replace(",", "','"));
            }
            #region SQL
            string sql = string.Format(@"select fk.name fkname , ftable.name ptablename, cn.name fkcol, rtable.name ftablename from {0}.sys.sysforeignkeys  
                            join {0}.sys.sysobjects fk    
                            on {0}.sys.sysforeignkeys.constid = fk.id  
                            join {0}.sys.sysobjects ftable  
                            on {0}.sys.sysforeignkeys.fkeyid = ftable.id  
                            join {0}.sys.sysobjects rtable  
                            on {0}.sys.sysforeignkeys.rkeyid = rtable.id  
                            join {0}.sys.syscolumns cn  
                            on {0}.sys.sysforeignkeys.fkeyid = cn.id and {0}.sys.sysforeignkeys.fkey = cn.colid {1}", database, PKtables);
            #endregion
            DataTable dt = GetDataTable(connectionString, sql);
            return dt.Rows.Cast<DataRow>().Select(row => new DbTableFK
            {
                FkName = row.Field<string>("fkname"),
                TableName = row.Field<string>("ptablename"),
                Column = row.Field<string>("fkcol"),
                FkTableName = row.Field<string>("ftablename")
            }).ToList();
        }
        #endregion
    }

    #region DbTable
    /// <summary>
    /// 表结构
    /// </summary>
    public sealed class DbTable
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表的架构
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 表的记录数
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// 是否含有主键
        /// </summary>
        public bool HasPrimaryKey { get; set; }
    }
    #endregion

    #region DbTableFK
    /// <summary>
    /// 表结构
    /// </summary>
    public sealed class DbTableFK
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 外键名称
        /// </summary>
        public string FkName { get; set; }
        /// <summary>
        /// 外键字段
        /// </summary>
        public string Column { get; set; }
        /// <summary>
        /// 外键表名称
        /// </summary>
        public string FkTableName { get; set; }
    }
    #endregion

    #region DbColumn
    /// <summary>
    /// 表字段结构
    /// </summary>
    [Serializable]
    public sealed class DbColumn
    {
        /// <summary>
        /// 字段ID
        /// </summary>
        public int ColumnID { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// 数据库类型对应的C#类型
        /// </summary>
        public string CSharpType
        {
            get
            {
                return SqlServerDbTypeMap.MapCsharpType(ColumnType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type CommonType
        {
            get
            {
                return SqlServerDbTypeMap.MapCommonType(ColumnType);
            }
        }

        /// <summary>
        /// 字节长度
        /// </summary>
        public int ByteLength { get; set; }

        /// <summary>
        /// 字符长度
        /// </summary>
        public int CharLength { get; set; }

        /// <summary>
        /// 小数位
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// 是否自增列
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 是否允许空
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }
    }
    #endregion

    #region SqlServerDbTypeMap

    public class SqlServerDbTypeMap
    {
        public static string MapCsharpType(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return dbtype;
            dbtype = dbtype.ToLower();
            string csharpType = "object";
            switch (dbtype)
            {
                case "bigint": csharpType = "long"; break;
                case "binary": csharpType = "byte[]"; break;
                case "bit": csharpType = "bool"; break;
                case "char": csharpType = "string"; break;
                case "date": csharpType = "DateTime"; break;
                case "datetime": csharpType = "DateTime"; break;
                case "datetime2": csharpType = "DateTime"; break;
                case "datetimeoffset": csharpType = "DateTimeOffset"; break;
                case "decimal": csharpType = "decimal"; break;
                case "float": csharpType = "double"; break;
                case "image": csharpType = "byte[]"; break;
                case "int": csharpType = "int"; break;
                case "money": csharpType = "decimal"; break;
                case "nchar": csharpType = "string"; break;
                case "ntext": csharpType = "string"; break;
                case "numeric": csharpType = "decimal"; break;
                case "nvarchar": csharpType = "string"; break;
                case "real": csharpType = "Single"; break;
                case "smalldatetime": csharpType = "DateTime"; break;
                case "smallint": csharpType = "short"; break;
                case "smallmoney": csharpType = "decimal"; break;
                case "sql_variant": csharpType = "object"; break;
                case "sysname": csharpType = "object"; break;
                case "text": csharpType = "string"; break;
                case "time": csharpType = "TimeSpan"; break;
                case "timestamp": csharpType = "byte[]"; break;
                case "tinyint": csharpType = "byte"; break;
                case "uniqueidentifier": csharpType = "Guid"; break;
                case "varbinary": csharpType = "byte[]"; break;
                case "varchar": csharpType = "string"; break;
                case "xml": csharpType = "string"; break;
                default: csharpType = "object"; break;
            }
            return csharpType;
        }

        public static Type MapCommonType(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return Type.Missing.GetType();
            dbtype = dbtype.ToLower();
            Type commonType = typeof(object);
            switch (dbtype)
            {
                case "bigint": commonType = typeof(long); break;
                case "binary": commonType = typeof(byte[]); break;
                case "bit": commonType = typeof(bool); break;
                case "char": commonType = typeof(string); break;
                case "date": commonType = typeof(DateTime); break;
                case "datetime": commonType = typeof(DateTime); break;
                case "datetime2": commonType = typeof(DateTime); break;
                case "datetimeoffset": commonType = typeof(DateTimeOffset); break;
                case "decimal": commonType = typeof(decimal); break;
                case "float": commonType = typeof(double); break;
                case "image": commonType = typeof(byte[]); break;
                case "int": commonType = typeof(int); break;
                case "money": commonType = typeof(decimal); break;
                case "nchar": commonType = typeof(string); break;
                case "ntext": commonType = typeof(string); break;
                case "numeric": commonType = typeof(decimal); break;
                case "nvarchar": commonType = typeof(string); break;
                case "real": commonType = typeof(Single); break;
                case "smalldatetime": commonType = typeof(DateTime); break;
                case "smallint": commonType = typeof(short); break;
                case "smallmoney": commonType = typeof(decimal); break;
                case "sql_variant": commonType = typeof(object); break;
                case "sysname": commonType = typeof(object); break;
                case "text": commonType = typeof(string); break;
                case "time": commonType = typeof(TimeSpan); break;
                case "timestamp": commonType = typeof(byte[]); break;
                case "tinyint": commonType = typeof(byte); break;
                case "uniqueidentifier": commonType = typeof(Guid); break;
                case "varbinary": commonType = typeof(byte[]); break;
                case "varchar": commonType = typeof(string); break;
                case "xml": commonType = typeof(string); break;
                default: commonType = typeof(object); break;
            }
            return commonType;
        }
    }
    #endregion
}
