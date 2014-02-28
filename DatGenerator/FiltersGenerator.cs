using System;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.CSharp;
using Microsoft.SqlServer.Management.Smo;

namespace DatGenerator
{
    public static class FiltersGenerator
    {
        private static DatClassGenerator.DatClass GenerateFilterClass(Table table)
        {
            DatClassGenerator.DatClass dat = new DatClassGenerator.DatClass();
            dat.Name = Helper.Capitalise(table.Name) + "BaseFilter";
            dat.Description = "Базовый фильтр для " + Helper.GetMSDescription(table);
            dat.BaseTypes.Add("BaseLoadFilter");



            #region test
            //for (int i = 0; i < table.Columns.Count; i++)
            //{
            //    Column col = table.Columns[i];

            //    switch (col.DataType.SqlDataType)
            //    {
            //        case SqlDataType.BigInt:
            //        case SqlDataType.Int:
            //        case SqlDataType.SmallInt:
            //        case SqlDataType.TinyInt:
            //            break;

            //        case SqlDataType.Bit:
            //            break;

            //        case SqlDataType.Char:
            //        case SqlDataType.NChar:
            //        case SqlDataType.NText:
            //        case SqlDataType.NVarChar:
            //        case SqlDataType.NVarCharMax:
            //        case SqlDataType.Text:
            //        case SqlDataType.VarChar:
            //        case SqlDataType.VarCharMax:
            //            NeedSubstring = true;
            //            break;

            //        case SqlDataType.Date:
            //        case SqlDataType.DateTime:
            //        case SqlDataType.DateTime2:
            //        case SqlDataType.DateTimeOffset:
            //        case SqlDataType.Time:
            //        case SqlDataType.Timestamp:
            //            // fromto
            //            break;

            //        case SqlDataType.Decimal:
            //        case SqlDataType.Float:
            //        case SqlDataType.Money:
            //        case SqlDataType.Numeric:
            //        case SqlDataType.Real:
            //        case SqlDataType.SmallMoney:
            //            // fromto
            //            break;

            //        case SqlDataType.UniqueIdentifier:
            //            break;

            //        case SqlDataType.Xml:
            //            break;

            //        case SqlDataType.Geography:
            //        case SqlDataType.Geometry:
            //        case SqlDataType.HierarchyId:
            //        case SqlDataType.Image:
            //        case SqlDataType.Binary:
            //        case SqlDataType.None:
            //        case SqlDataType.SysName:
            //        case SqlDataType.UserDefinedDataType:
            //        case SqlDataType.UserDefinedTableType:
            //        case SqlDataType.UserDefinedType:
            //        case SqlDataType.VarBinary:
            //        case SqlDataType.VarBinaryMax:
            //        case SqlDataType.Variant:
            //            break;
            //        default:
            //            break;
            //    }
            //}
            #endregion

            return dat;
        }
        public static CodeTypeDeclaration CreateInheritedFilterClass(Table table)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(table.Name) + "Filter";
            ret.IsPartial = true;
            ret.BaseTypes.Add(new CodeTypeReference(Helper.Capitalise(table.Name) + "BaseFilter"));
            ret.Comments.Add(new CodeCommentStatement("<summary>" + "Фильтр для " + Helper.GetMSDescription(table) + "</summary>", true));
            return ret;
        }

        public static void BaseFilterClassAddMethods(Table table, ref CodeTypeDeclaration flt)
        {
            flt.UserData.Add("TableName", table.Name);

            CodeConstructor constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            flt.Members.Add(constr);

            CodeMemberMethod resetToDefault = new CodeMemberMethod();
            resetToDefault.Name = "ResetToDefault";
            resetToDefault.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            CodeMemberMethod getfilter = new CodeMemberMethod();
            getfilter.Name = "GetFilter";
            getfilter.ReturnType = new CodeTypeReference("IDAOFilter");
            getfilter.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            getfilter.Statements.Add(new CodeSnippetExpression("IDataAccess da = DB_" + table.Parent.Name.Replace("_DEBUG","").Replace("-", "_") + ".DbDataAccessor"));
            getfilter.Statements.Add(new CodeSnippetExpression("IDAOFilter filter = da.NewFilter()"));

            foreach (CodeTypeMember item in flt.Members)
            {
                if (item is CodeMemberProperty)
                {
                    CodeMemberProperty prop = (CodeMemberProperty)item;

                    object o = prop.UserData["InPrimaryKey"];

                    switch (prop.Type.BaseType)
                    {
                        case "FilterID":
                            resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0}.IDList.Clear()", prop.Name)));
                            getfilter.Statements.Add(new CodeSnippetExpression(string.Format("filter.AddWhere({0})", prop.Name)));
                            break;
                        case "FilterDateFromTo":
                            resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0}.Reset()", prop.Name)));
                            getfilter.Statements.Add(new CodeSnippetExpression(string.Format("filter.AddWhere({0})", prop.Name)));
                            break;
                        case "DateTime":
                            resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0} = DateTime.MinValue", prop.Name)));
                            if (prop.Name == "FltValueDateAsOf")
                            {
                                StringBuilder filterExp = new StringBuilder();
                                filterExp.AppendLine("if (FltValueDateAsOf > DateTime.MinValue)");
                                filterExp.AppendLine("\t{");
                                filterExp.AppendLine("\tstring DateFormat = (FltValueDateAsOf.TimeOfDay.TotalSeconds > 0) ? \"yyyyMMdd HH:mm:ss\" : \"yyyyMMdd 23:59:59:999\";");
                                filterExp.AppendFormat("\tfilter.AddJoin(new ColumnJoin(JoinType.INNER, \"f\", string.Format(\"dbo.fn_AsOf_{2}('{1}')\", {0}Dat.STableName, FltValueDateAsOf.ToString(DateFormat)), \"id\", {0}Dat.STableName, {0}Columns.ID));\r\n", flt.UserData["TableName"], "{1}", "{0}");
                                filterExp.AppendLine("\t}");

                                getfilter.Statements.Add(new CodeSnippetExpression(filterExp.ToString()));
                            }
                            break;
                        case "FilterGUID":
                            resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0}.IDList.Clear()", prop.Name)));
                            getfilter.Statements.Add(new CodeSnippetExpression(string.Format("filter.AddWhere({0})", prop.Name)));
                            break;
                        case "FilterBool":
                            resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0}.Reset()", prop.Name)));
                            getfilter.Statements.Add(new CodeSnippetExpression(string.Format("filter.AddWhere({0})", prop.Name)));
                            break;
                        case "FilterString":
                            resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0}.Reset()", prop.Name)));
                            getfilter.Statements.Add(new CodeSnippetExpression(string.Format("filter.AddWhere({0})", prop.Name)));
                            break;
                        //case "FilterTree":
                        //    resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0} = null", prop.Name)));
                        //    getfilter.Statements.Add(new CodeSnippetExpression(string.Format("filter.AddWhere({0})", prop.Name)));
                        //    break;
                        default:
                            resetToDefault.Statements.Add(new CodeSnippetExpression(string.Format("{0} = null", prop.Name)));
                            getfilter.Statements.Add(new CodeSnippetExpression(string.Format("filter.AddWhere({0})", prop.Name)));
                            break;
                    }

                }
            }

            //for (int i = 0; i < dat.Members.Count; i++)
            //{
            //    string code = null;
            //    DatMember mem = dat.Members[i];
            //    switch (mem.DataType)
            //    {
            //        case "Int32":
            //            code = string.Format("{0}.IDList.Clear()", mem.Name);
            //            break;
            //        case "bool?":
            //        case "Guid?":
            //            code = string.Format("{0} = null", mem.Name);
            //            break;
            //        default:
            //            break;
            //    }


            //for (int i = 0; i < dat.Members.Count; i++)
            //{
            //    string code = null;
            //    DatMember mem = dat.Members[i];
            //    switch (mem.DataType)
            //    {
            //        case "Int32":
            //            code = string.Format("filter.AddWhere({0})", mem.Name);
            //            break;
            //        case "bool?":
            //            code = string.Format("if ({1}.HasValue)\r\n\tfilter.AddWhere(new FilterBool({2}Dat.STableName, {2}Columns.{0}, {1}.Value))", Helper.Capitalise(mem.ColumnName), mem.Name, table.Name);
            //            break;
            //        case "Guid?":
            //            code = string.Format("if ({1}.HasValue)\r\n\tfilter.AddWhere(new FilterGUID({2}Columns.{0}, {1}.Value))", Helper.Capitalise(mem.ColumnName), mem.Name, table.Name);
            //            break;
            //        default:
            //            break;
            //    }

            //    if (mem.Name == "FltPeriod_From")
            //    {
            //        code = string.Format("if (FltPeriod_From != DateTime.MinValue || FltPeriod_To != DateTime.MinValue)\r\n\t");
            //        code += "filter.AddWhere(new FilterOR(\r\n";

            //        List<string> fCode = new List<string>();

            //        for (int j = 0; j < table.Columns.Count; j++)
            //        {
            //            Column col = table.Columns[j];
            //            if (col.DataType.ToString().StartsWith("date"))
            //            {
            //                fCode.Add(string.Format("new FilterDateFromTo({0}Columns.{1}, FltPeriod_From, FltPeriod_To)", table.Name, Helper.Capitalise(col.Name)));
            //            }
            //        }
            //        code += string.Join(",\r\n", fCode.ToArray());
            //        code += "))";
            //    }
            //    if (mem.Name == "FltSubstring")
            //    {
            //        code = string.Format("if (!string.IsNullOrEmpty(FltSubstring))\r\n\t");
            //        code += "filter.AddWhere(new FilterOR(\r\n";

            //        List<string> fCode = new List<string>();

            //        for (int j = 0; j < table.Columns.Count; j++)
            //        {
            //            Column col = table.Columns[j];
            //            if (col.DataType.ToString().Contains("char"))
            //            {
            //                fCode.Add(string.Format("new FilterString({0}Columns.{1}, \"%\" + FltSubstring + \"%\", true)", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name)));
            //            }
            //        }
            //        code += string.Join(",\r\n", fCode.ToArray());
            //        code += "))";
            //    }

            //    if (!string.IsNullOrEmpty(code))
            //        getfilter.Statements.Add(new CodeSnippetExpression(code));

            //}



            //if (DateFrom != DateTime.MinValue || DateTo != DateTime.MinValue)
            //    f.AddWhere(new FilterDateFromTo(Napi_OrdersColumns.Orderdate, DateFrom, DateTo));


            //if (Product != null && Product.ID != 0)
            //    f.AddWhere(new FilterID(Napi_OrdersColumns.Product + "_id", Product.ID));

            //if (Status != null && Status.ID != 0)
            //    f.AddWhere(new FilterID(Napi_OrdersColumns.Order_status, Status.ID));

            ////if (Substring != "")
            ////    f.AddWhere(new FilterString("Name", "%" + Substring + "%", true));

            getfilter.Statements.Add(new CodeSnippetExpression("return filter"));
            flt.Members.Add(getfilter);
            flt.Members.Add(resetToDefault);


        }

        public static void BaseFilterClassAddProperties(Table table, ref CodeTypeDeclaration flt)
        {
            //bool NeedSubstring = false;
            bool NeedValueDateAsOf = false;

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column col = table.Columns[i];

                switch (col.DataType.SqlDataType)
                {
                    case SqlDataType.BigInt:
                    case SqlDataType.Int:
                    case SqlDataType.SmallInt:
                    case SqlDataType.TinyInt:
                        {
                            CodeMemberField field = new CodeMemberField();
                            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                            field.Name = "_Flt" + Helper.Capitalise(col.Name);
                            field.Type = new CodeTypeReference("FilterID");
                            field.InitExpression = new CodeSnippetExpression(string.Format(" new FilterID({0}Columns.{2})", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name), (col.Name.ToUpper() == "ID") ? "ID" : Helper.Capitalise(col.Name)));

                            CodeMemberProperty prop = new CodeMemberProperty();
                            prop.Attributes = MemberAttributes.Public;
                            //prop.Comments.Add(new CodeCommentStatement("<summary>" + mem.Description + "</summary>", true));
                            prop.Name = "Flt" + Helper.Capitalise(col.Name);
                            prop.Type = field.Type;
                            prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                            prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
                                        }));

                            prop.UserData.Add("IsForeignKey", col.IsForeignKey);
                            prop.UserData.Add("InPrimaryKey", col.InPrimaryKey);

                            if (col.IsForeignKey)
                            {
                                foreach (ForeignKey fk in table.ForeignKeys)
                                {
                                    if (fk.Columns[0].Name == col.Name)
                                    {
                                        Table reft = table.Parent.Tables[fk.ReferencedTable];
                                        prop.UserData.Add("ReferencedTable", reft.Name);
                                        prop.UserData.Add("ReferencedTableMSDescription", Helper.GetMSDescription(reft));
                                        break;
                                    }
                                }


                            }

                            prop.UserData.Add("MSDescription", Helper.GetMSDescription(col));
                            flt.Members.Add(field);
                            flt.Members.Add(prop);
                        }
                        break;
                    case SqlDataType.UniqueIdentifier:
                        {
                            CodeMemberField field = new CodeMemberField();
                            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                            field.Name = "_Flt" + Helper.Capitalise(col.Name);
                            field.Type = new CodeTypeReference("FilterGUID");
                            field.InitExpression = new CodeSnippetExpression(string.Format(" new FilterGUID({0}Columns.{1})", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name)));

                            CodeMemberProperty prop = new CodeMemberProperty();
                            prop.Attributes = MemberAttributes.Public;
                            prop.Name = "Flt" + Helper.Capitalise(col.Name);
                            prop.Type = field.Type;
                            prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                            prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
                                        }));

                            prop.UserData.Add("MSDescription", Helper.GetMSDescription(col));
                            flt.Members.Add(field);
                            flt.Members.Add(prop);
                        }
                        break;
                    case SqlDataType.Bit:
                        {
                            CodeMemberField field = new CodeMemberField();
                            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                            field.Name = "_Flt" + Helper.Capitalise(col.Name);
                            field.Type = new CodeTypeReference("FilterBool");
                            field.InitExpression = new CodeSnippetExpression(string.Format(" new FilterBool({0}Dat.STableName, {0}Columns.{1})", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name)));

                            CodeMemberProperty prop = new CodeMemberProperty();
                            prop.Attributes = MemberAttributes.Public;
                            prop.Name = "Flt" + Helper.Capitalise(col.Name);
                            prop.Type = field.Type;
                            prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                            prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
                                        }));

                            prop.UserData.Add("MSDescription", Helper.GetMSDescription(col));
                            flt.Members.Add(field);
                            flt.Members.Add(prop);
                        }
                        break;

                    case SqlDataType.Char:
                    case SqlDataType.NChar:
                    case SqlDataType.NText:
                    case SqlDataType.NVarChar:
                    case SqlDataType.NVarCharMax:
                    case SqlDataType.Text:
                    case SqlDataType.VarChar:
                    case SqlDataType.VarCharMax:
                        {
                            //NeedSubstring = true;
                            CodeMemberField field = new CodeMemberField();
                            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                            field.Name = string.Format("_Flt{0}", Helper.Capitalise(col.Name));

                            if (/*col.Name.ToLower() == "fp" || */col.Name.ToLower() == "parent_fp")
                            {
                                field.Type = new CodeTypeReference("FilterTree");
                                field.InitExpression = new CodeSnippetExpression(string.Format(" new FilterTree({0}Dat.STableName, {0}Columns.{1}, null as PathTree, false)", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name)));
                            }
                            else if (col.Name.ToLower() == "fpn")
                            {
                                field.Type = new CodeTypeReference("FilterTreeN");
                                field.InitExpression = new CodeSnippetExpression(string.Format(" new FilterTreeN({0}Dat.STableName, {0}Columns.{1}, null as PathTreeN, false)", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name)));
                            }
                            else
                            {
                                field.Type = new CodeTypeReference("FilterString");
                                field.InitExpression = new CodeSnippetExpression(string.Format(" new FilterString({0}Dat.STableName, {0}Columns.{1}, String.Empty, true)", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name)));
                            }

                            CodeMemberProperty prop = new CodeMemberProperty();
                            prop.Attributes = MemberAttributes.Public;
                            prop.Name = string.Format("Flt{0}", Helper.Capitalise(col.Name));
                            prop.Type = field.Type;
                            prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                            prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
                                        }));

                            prop.UserData.Add("MSDescription", Helper.GetMSDescription(col));
                            flt.Members.Add(field);
                            flt.Members.Add(prop);

                        }
                        break;

                    case SqlDataType.Date:
                    case SqlDataType.DateTime:
                    case SqlDataType.SmallDateTime:
                    case SqlDataType.DateTime2:
                    case SqlDataType.DateTimeOffset:
                    case SqlDataType.Time:
                    case SqlDataType.Timestamp:
                        {
                            if (col.Name == "ValueDate")
                                NeedValueDateAsOf = true;

                            CodeMemberField field = new CodeMemberField();
                            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                            field.Name = "_Flt" + Helper.Capitalise(col.Name);
                            field.Type = new CodeTypeReference("FilterDateFromTo");
                            field.InitExpression = new CodeSnippetExpression(string.Format(" new FilterDateFromTo({0}Columns.{1}, DateTime.MinValue, DateTime.MinValue)", Helper.Capitalise(table.Name), Helper.Capitalise(col.Name)));

                            CodeMemberProperty prop = new CodeMemberProperty();
                            prop.Attributes = MemberAttributes.Public;
                            //prop.Comments.Add(new CodeCommentStatement("<summary>" + mem.Description + "</summary>", true));
                            prop.Name = "Flt" + Helper.Capitalise(col.Name);
                            prop.Type = field.Type;
                            prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                            prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
                                        }));
                            prop.UserData.Add("MSDescription", Helper.GetMSDescription(col));
                            flt.Members.Add(field);
                            flt.Members.Add(prop);

                        }
                        break;

                    case SqlDataType.Decimal:
                    case SqlDataType.Float:
                    case SqlDataType.Money:
                    case SqlDataType.Numeric:
                    case SqlDataType.Real:
                    case SqlDataType.SmallMoney:
                    case SqlDataType.Xml:
                    case SqlDataType.Geography:
                    case SqlDataType.Geometry:
                    case SqlDataType.HierarchyId:
                    case SqlDataType.Image:
                    case SqlDataType.Binary:
                    case SqlDataType.None:
                    case SqlDataType.SysName:
                    case SqlDataType.UserDefinedDataType:
                    case SqlDataType.UserDefinedTableType:
                    case SqlDataType.UserDefinedType:
                    case SqlDataType.VarBinary:
                    case SqlDataType.VarBinaryMax:
                    case SqlDataType.Variant:
                        break;
                    default:
                        break;
                }
            }

            //if (NeedSubstring)
            //{
            //    CodeMemberField field = new CodeMemberField();
            //    field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            //    field.Name = "_FltSubString";
            //    field.Type = new CodeTypeReference("string");

            //    CodeMemberProperty prop = new CodeMemberProperty();
            //    prop.Attributes = MemberAttributes.Public;
            //    prop.Name = "FltSubString";
            //    prop.Type = field.Type;
            //    prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
            //    prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
            //                            new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
            //                            }));

            //    flt.Members.Add(field);
            //    flt.Members.Add(prop);
            //}

            if (NeedValueDateAsOf)
            {
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                field.Name = "_FltValueDateAsOf";
                field.Type = new CodeTypeReference("DateTime");

                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Attributes = MemberAttributes.Public;
                prop.Name = "FltValueDateAsOf";
                prop.Type = field.Type;
                prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
                                        }));

                prop.UserData.Add("MSDescription", "Актуально на дату");
                flt.Members.Add(field);
                flt.Members.Add(prop);

            }


        }
        public static CodeTypeDeclaration CreateBaseFilterClass(Table table)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();

            ret.Name = Helper.Capitalise(table.Name) + "BaseFilter";
            ret.IsPartial = true;

            string summary = "";
            string descr = Helper.GetMSDescription(table);

            if (string.IsNullOrEmpty(descr))
                summary = string.Format("Базовый фильтр для {0}", table.Name);
            else
                summary = string.Format("Базовый фильтр для {0}", descr);

            ret.Comments.Add(new CodeCommentStatement("<summary>" + summary + "</summary>", true));
            ret.BaseTypes.Add(new CodeTypeReference("BaseLoadFilter"));

            return ret;
        }
    }
}
