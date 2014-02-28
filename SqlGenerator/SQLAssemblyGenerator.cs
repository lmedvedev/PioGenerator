using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.CSharp;
using DatGenerator;

namespace Generator
{

    public class SQLAssemblyGenerator : DatGenerator.CustomToolBase
    {
        public SQLAssemblyGenerator() { }

        public override string GetDefaultExtension()
        {
            return ".Designer.cs";
        }

        public override string GenerateCode(string fileContent)
        {
            //DataSet ds = GetDataSetFromXsd(fileContent);
            //CodeNamespace ns = GetNamespaceFromDataSet(ds);
            //return GetCodeFromNamespace(ns);
            return "FakeNamespace";
        }

        public string GetCodeFromTables(SortedList tables, string DefaultNamespace)
        {
            string name = (CustomToolNamespace == null) ? DefaultNamespace : CustomToolNamespace.Contains(".") ? CustomToolNamespace.Substring(0, CustomToolNamespace.LastIndexOf(".")) : CustomToolNamespace;
            CodeNamespace ns = new CodeNamespace(name);
            ns.Comments.Add(new CodeCommentStatement("Generated - '" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "' by " + System.Security.Principal.WindowsIdentity.GetCurrent().Name));

            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            ns.Imports.Add(new CodeNamespaceImport("System.Xml")); 
            ns.Imports.Add(new CodeNamespaceImport("System.Text"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data.SqlTypes"));
            ns.Imports.Add(new CodeNamespaceImport("System.Data.SqlClient"));
            ns.Imports.Add(new CodeNamespaceImport("Microsoft.SqlServer.Server"));
            //ns.Imports.Add(new CodeNamespaceImport("System.Drawing"));

            foreach (DictionaryEntry obj in tables)
            {
                Table table = (Table)obj.Value;
                CodeTypeDeclaration tp = GenerateDatClass(table);
                ns.Types.Add(tp);
                //tp = GenerateColumnConsts(tp);
                //if (tp != null) ns.Types.Add(tp);
                //tp = GenerateSetClass(table);
                //ns.Types.Add(tp);
            }
            StringBuilder sb = new StringBuilder();
            new CSharpCodeProvider().GenerateCodeFromNamespace(ns, new StringWriter(sb), new CodeGeneratorOptions());
            return sb.ToString();
        }

        public string GetCodeFromNamespace(CodeNamespace ns)
        {
            StringBuilder sb = new StringBuilder();
            new CSharpCodeProvider().GenerateCodeFromNamespace(ns, new StringWriter(sb), new CodeGeneratorOptions());
            return sb.ToString();
        }
        
        //System.Data.SqlTypes. 
        public SqlDbType SQLType(SqlDataType dbtype)
        {
            SqlDbType type = SqlDbType.Variant;

            switch (dbtype)
            {
                case SqlDataType.BigInt:
                    type = SqlDbType.BigInt;
                    break;
                case SqlDataType.Int:
                    type = SqlDbType.Int;
                    break;
                case SqlDataType.SmallInt:
                    type = SqlDbType.SmallInt;
                    break;
                case SqlDataType.TinyInt:
                    type = SqlDbType.TinyInt;
                    break;
                case SqlDataType.Timestamp:
                    type = SqlDbType.Timestamp;
                    break;
                case SqlDataType.Bit:
                    type = SqlDbType.Bit;
                    break;
                case SqlDataType.DateTime:
                    type = SqlDbType.DateTime;
                    break;
                case SqlDataType.SmallDateTime:
                    type = SqlDbType.SmallDateTime;
                    break;
                case SqlDataType.Float:
                    type = SqlDbType.Float;
                    break;
                case SqlDataType.Decimal:
                    type = SqlDbType.Decimal;
                    break;
                case SqlDataType.Money:
                    type = SqlDbType.Money;
                    break;
                case SqlDataType.Real:
                    type = SqlDbType.Real;
                    break;
                case SqlDataType.SmallMoney:
                    type = SqlDbType.SmallMoney;
                    break;
                case SqlDataType.Char:
                    type = SqlDbType.Char;
                    break;
                case SqlDataType.NChar:
                    type = SqlDbType.NChar;
                    break;
                case SqlDataType.Text:
                    type = SqlDbType.Text;
                    break;
                case SqlDataType.VarChar:
                case SqlDataType.VarCharMax:
                    type = SqlDbType.VarChar;
                    break;
                case SqlDataType.NText:
                    type = SqlDbType.NText;
                    break;
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                    type = SqlDbType.NVarChar;
                    break;
                case SqlDataType.Xml:
                    type = SqlDbType.Xml;
                    break;
                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                    type = SqlDbType.VarBinary;
                    break;
                case SqlDataType.Binary:
                    type = SqlDbType.Binary;
                    break;
                case SqlDataType.Image:
                    type = SqlDbType.Image;
                    break;
                case SqlDataType.UniqueIdentifier:
                    type = SqlDbType.UniqueIdentifier;
                    break;
                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                    type = SqlDbType.Udt;
                    break;
                case SqlDataType.Variant:
                case SqlDataType.Numeric:
                case SqlDataType.SysName:
                case SqlDataType.None:
                    type = SqlDbType.Variant;
                    break;
                default:
                    type = SqlDbType.Variant;
                    break;
            }
            return type;

        }
        public Type GetSQLTypeFromDBType(SqlDataType sdt)
        {
            Type type = null;
            switch (sdt)
            {
                case SqlDataType.BigInt:
                    type = typeof(System.Data.SqlTypes.SqlInt64);
                    break;
                case SqlDataType.Binary:
                    type = typeof(System.Data.SqlTypes.SqlBinary);
                    break;
                case SqlDataType.Bit:
                    type = typeof(System.Data.SqlTypes.SqlBoolean);
                    break;
                case SqlDataType.Char:
                    type = typeof(System.Data.SqlTypes.SqlChars);
                    break;
                case SqlDataType.DateTime:
                    type = typeof(System.Data.SqlTypes.SqlDateTime);
                    break;
                case SqlDataType.Decimal:
                    type = typeof(System.Data.SqlTypes.SqlDecimal);
                    break;
                case SqlDataType.Float:
                    type = typeof(System.Data.SqlTypes.SqlDouble);
                    break;
                case SqlDataType.Image:
                    type = typeof(System.Data.SqlTypes.SqlBinary);
                    break;
                case SqlDataType.Int:
                    type = typeof(System.Data.SqlTypes.SqlInt32);
                    break;
                case SqlDataType.Money:
                    type = typeof(System.Data.SqlTypes.SqlMoney);
                    break;
                case SqlDataType.NChar:
                    type = typeof(System.Data.SqlTypes.SqlChars);
                    break;
                case SqlDataType.NText:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.NVarChar:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.NVarCharMax:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.Numeric:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.Real:
                    type = typeof(System.Data.SqlTypes.SqlDouble);
                    break;
                case SqlDataType.SmallDateTime:
                    type = typeof(System.Data.SqlTypes.SqlDateTime);
                    break;
                case SqlDataType.SmallInt:
                    type = typeof(System.Data.SqlTypes.SqlInt16);
                    break;
                case SqlDataType.SmallMoney:
                    type = typeof(System.Data.SqlTypes.SqlMoney);
                    break;
                case SqlDataType.SysName:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.Text:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.Timestamp:
                    type = typeof(System.Data.SqlTypes.SqlInt64);
                    break;
                case SqlDataType.TinyInt:
                    type = typeof(System.Data.SqlTypes.SqlByte);
                    break;
                case SqlDataType.UniqueIdentifier:
                    type = typeof(System.Data.SqlTypes.SqlGuid);
                    break;
                case SqlDataType.VarBinary:
                    type = typeof(System.Data.SqlTypes.SqlBinary);
                    break;
                case SqlDataType.VarBinaryMax:
                    type = typeof(System.Data.SqlTypes.SqlBinary);
                    break;
                case SqlDataType.VarChar:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.VarCharMax:
                    type = typeof(System.Data.SqlTypes.SqlString);
                    break;
                case SqlDataType.Xml:
                    type = typeof(System.Data.SqlTypes.SqlXml);
                    break;
                case SqlDataType.Variant:
                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                case SqlDataType.None:
                    break;
                default:
                    break;
            }
            return type;
        }

        public class DatClass
        {
            public string Name;
            public string Description;
            public List<string> BaseTypes = new List<string>();
            public List<DatMember> Members = new List<DatMember>();
            public List<DatPublicVariable> Variables = new List<DatPublicVariable>();
            public bool ContainsMember(string name)
            {
                foreach (DatMember dat in Members)
                {
                    if (dat.Name == name) return true;
                }
                return false;
            }
            public bool ContainsPublicVariable(string name)
            {
                foreach (DatPublicVariable dat in Variables)
                {
                    if (dat.Name == name) return true;
                }
                return false;
            }
        }

        public class DatMember
        {
            public string Name;
            public string Description;
            public string DataType;
            public string ColumnName;
            //public List<CodeAttributeArgument> Ordinals = new List<CodeAttributeArgument>();
        }

        public class DatPublicVariable
        {
            public string Name;
            public string Description;
            //public string DataType;
            public string SQLDataType;
            public string SQLDataType2;
            public string ColumnName;
            public long Length;
            public int Precision;
            public int Scale;
        }

        private CodeTypeDeclaration GenerateDatClass(Table table)
        {
            DatClass dat = new DatClass();
            dat.Name = table.Name;
            dat.BaseTypes.Add("BaseSQL<" + dat.Name + ">");
            //dat.BaseTypes.Add("BaseDat<" + dat.Name + "Dat>");
            dat.Description = (table.ExtendedProperties["MS_Description"] == null) ? "" : table.ExtendedProperties["MS_Description"].Value.ToString();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column col = table.Columns[i];
                DatPublicVariable mem = new DatPublicVariable();
                string name = col.Name.Substring(0, 1).ToUpper() + col.Name.Substring(1);
                if (name.ToUpper() == "ID") name = "ID";
                if (name.ToUpper() == "FP") name = "FP";
                mem.Name = mem.ColumnName = name;
                mem.Description = Helper.GetMSDescription(col);

                //mem.DataType = Helper.GetTypeFromDBType(col.DataType.SqlDataType).Name;
                mem.SQLDataType = GetSQLTypeFromDBType(col.DataType.SqlDataType).Name;
                mem.SQLDataType2 = SQLType(col.DataType.SqlDataType).ToString();
                if (col.DataType.SqlDataType == SqlDataType.VarChar) mem.Length = col.DataType.MaximumLength;
                if (col.DataType.SqlDataType == SqlDataType.Decimal)
                {
                    mem.Scale = col.DataType.NumericScale;
                    mem.Precision = col.DataType.NumericPrecision;
                }
                dat.Variables.Add(mem);
            }
            return CreateDatClass(dat);
        }

        private CodeTypeDeclaration CreateDatClass(DatClass dat)
        {

            string[] varNames = dat.Variables.ConvertAll<string>(delegate(DatPublicVariable dat1)
            {
                return dat1.ColumnName;
            }
            ).ToArray();

            string[] varNames2 = dat.Variables.ConvertAll<string>(delegate(DatPublicVariable dat1)
            {
                string s = "\t\t\t\tnew SqlMetaData(\"" + dat1.ColumnName + "\", SqlDbType." + dat1.SQLDataType2;
                if (dat1.SQLDataType2 == "VarChar")
                    s += "," + dat1.Length;
                else if (dat1.SQLDataType2 == "Decimal")
                    s += ", " + dat1.Precision + ", " + dat1.Scale;
                s += ")";
                return s;
            }
            ).ToArray();


            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = dat.Name;// +"Dat";
            ret.IsPartial = true;
            ret.Comments.Add(new CodeCommentStatement("<summary>" + dat.Description + "</summary>", true));
            foreach (string type in dat.BaseTypes)
            {
                ret.BaseTypes.Add(new CodeTypeReference(type));
            }

            CodeConstructor constr = new CodeConstructor();
            //constr.Attributes = MemberAttributes.Public;
            //ret.Members.Add(constr);

            constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            ret.Members.Add(constr);

            constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            constr.Parameters.Add(new CodeParameterDeclarationExpression("SqlDataReader", "reader"));
            constr.Statements.Add(new CodeSnippetExpression("LoadMembers(reader)"));
            ret.Members.Add(constr);

            CodeMemberMethod Method_LoadMembers = new CodeMemberMethod();
            Method_LoadMembers.Name = "LoadMembers";
            Method_LoadMembers.Parameters.Add(new CodeParameterDeclarationExpression("SqlDataReader", "reader"));
            Method_LoadMembers.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            Method_LoadMembers.Statements.Add(new CodeSnippetExpression("int index = 0"));             
            ret.Members.Add(Method_LoadMembers);

            CodeMemberMethod Method_GetSqlDataRecord = new CodeMemberMethod();
            Method_GetSqlDataRecord.Name = "GetSqlDataRecord";
            Method_GetSqlDataRecord.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            Method_GetSqlDataRecord.ReturnType = new CodeTypeReference(typeof(SqlDataRecord));
            Method_GetSqlDataRecord.Statements.Add(new CodeSnippetExpression("return GetSqlDataRecord_static()")); 
            ret.Members.Add(Method_GetSqlDataRecord);

            CodeMemberMethod Method_GetSqlDataRecord_static = new CodeMemberMethod();
            Method_GetSqlDataRecord_static.Name = "GetSqlDataRecord_static";
            Method_GetSqlDataRecord_static.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            Method_GetSqlDataRecord_static.ReturnType = new CodeTypeReference(typeof(SqlDataRecord));
            Method_GetSqlDataRecord_static.Statements.Add(new CodeSnippetExpression("SqlDataRecord record = new SqlDataRecord(\r\n" + string.Join(",\r\n", varNames2) + ")"));
            Method_GetSqlDataRecord_static.Statements.Add(new CodeSnippetExpression("return record"));
            ret.Members.Add(Method_GetSqlDataRecord_static);

            CodeMemberMethod Method_GetSelect = new CodeMemberMethod();
            Method_GetSelect.Name = "GetSelect";
            Method_GetSelect.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            Method_GetSelect.ReturnType = new CodeTypeReference(typeof(string));
            Method_GetSelect.Statements.Add(new CodeSnippetExpression("return GetSelect_static()"));
            ret.Members.Add(Method_GetSelect);

            CodeMemberMethod Method_GetSelect_static = new CodeMemberMethod();
            Method_GetSelect_static.Name = "GetSelect_static";
            Method_GetSelect_static.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            Method_GetSelect_static.ReturnType = new CodeTypeReference(typeof(string));
            Method_GetSelect_static.Statements.Add(new CodeSnippetExpression(string.Format("return \"select {0} from {1}\"", string.Join(", ", varNames), dat.Name)));
            ret.Members.Add(Method_GetSelect_static);

            CodeMemberMethod Method_FillSqlDataRecord = new CodeMemberMethod();
            Method_FillSqlDataRecord.Name = "FillSqlDataRecord";
            Method_FillSqlDataRecord.Parameters.Add(new CodeParameterDeclarationExpression("SqlDataRecord", "record"));
            Method_FillSqlDataRecord.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            Method_FillSqlDataRecord.Statements.Add(new CodeSnippetExpression("int index = 0"));
            ret.Members.Add(Method_FillSqlDataRecord);

            for (int i = 0; i < dat.Variables.Count; i++)
            {
                DatPublicVariable var = dat.Variables[i];
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Public; // | MemberAttributes.Final;
                field.Name = var.Name;
                field.Type = new CodeTypeReference(var.SQLDataType);
                ret.Members.Add(field);

                Method_LoadMembers.Statements.Add(new CodeSnippetExpression(var.Name + " = reader.Get" + var.SQLDataType + "(index++)"));
                Method_FillSqlDataRecord.Statements.Add(new CodeSnippetExpression("record.Set" + var.SQLDataType + "(index++, " + var.Name + ")"));
            }
         
            return ret;
        }
    }

}
