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
    [Guid("11337B6C-D4DF-46ce-8A69-118F1F79417A")]
    public class DatClassGenerator : CustomToolBase
    {
        public DatClassGenerator() { }

        public override string GetDefaultExtension()
        {
            return ".Designer.cs";
        }

        public override string GenerateCode(string fileContent)
        {
            DataSet ds = GetDataSetFromXsd(fileContent);
            CodeNamespace ns = GetNamespaceFromDataSet(ds);
            return GetCodeFromNamespace(ns);
        }

        public DataSet GetDataSetFromXsd(string xsd)
        {
            DataSet ds = new DataSet();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xsd);
            ds.DataSetName = xml.DocumentElement.Attributes["id"].Value;
            XmlNodeList tables = xml.DocumentElement.SelectNodes("/*/*[local-name()='element']/*/*/*[local-name()='element']");//"*/*[count(*[local-name()='keyref'])=0]")[0];
            XmlNodeList rels = xml.DocumentElement.SelectNodes("/*/*[local-name()='annotation']/*/*[local-name()='Relationship']");
            Hashtable cols = new Hashtable();
            foreach (XmlNode table in tables)
            {
                DataTable tbl = new DataTable(table.Attributes["name"].Value);
                ds.Tables.Add(tbl);
                XmlNodeList columns = table.SelectNodes("*/*/*[local-name()='element']");
                foreach (XmlNode col in columns)
                {
                    string type = (col.Attributes["type"] == null) ? col.SelectSingleNode("*/*").Attributes["base"].Value : col.Attributes["type"].Value;
                    DataColumn cl = new DataColumn(col.Attributes["name"].Value, GetTypeFromXsd(type));
                    tbl.Columns.Add(cl);
                    cols.Add(tbl.TableName + "." + cl.ColumnName, cl);
                }
            }
            foreach (XmlNode rel in rels)
            {
                string name = rel.Attributes["name"].Value;
                DataColumn parent = (DataColumn)cols[rel.Attributes["msdata:parent"].Value + "." + rel.Attributes["msdata:parentkey"].Value];
                DataColumn child = (DataColumn)cols[rel.Attributes["msdata:child"].Value + "." + rel.Attributes["msdata:childkey"].Value];
                ds.Relations.Add(name, parent, child);
            }
            return ds;
        }

        public CodeNamespace GetNamespaceFromDataSet(DataSet ds)
        {
            string name = (CustomToolNamespace == null) ? "AppD" : CustomToolNamespace.Contains(".") ? CustomToolNamespace.Substring(0, CustomToolNamespace.LastIndexOf(".")) : CustomToolNamespace;
            CodeNamespace ns = new CodeNamespace(name);
            ns.Imports.Add(new CodeNamespaceImport("BO"));
            ns.Imports.Add(new CodeNamespaceImport("System"));
            CodeTypeDeclarationCollection types = new CodeTypeDeclarationCollection();
            CodeTypeDeclaration main = new CodeTypeDeclaration();
            main.Name = ds.DataSetName;
            main.BaseTypes.Add(typeof(DataSet));
            main.IsPartial = true;
            CodeConstructor constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            foreach (DataTable table in ds.Tables)
            {
                //constr.Statements.Add(new CodeSnippetExpression("Tables.Add(new " + table.TableName + "Table())"));
                types.Add(GenerateDatClass(table));
                types.Add(GenerateSetClass(table));
            }
            foreach (DataRelation rel in ds.Relations)
            {
                //constr.Statements.Add(new CodeSnippetExpression("Relations.Add(new DataRelation(\"" + rel.RelationName + "\", Tables[\"" + rel.ParentTable.TableName + "\"].Columns[\"" + rel.ParentColumns[0].ColumnName + "\"], Tables[\"" + rel.ChildTable.TableName + "\"].Columns[\"" + rel.ChildColumns[0].ColumnName + "\"]))"));
            }
            //main.Members.Add(constr);
            //ns.Types.Add(main);
            ns.Types.AddRange(types);
            return ns;
        }
        public CodeNamespace GetFormsNamespaceFromDB(Database db)
        {
            string name = (CustomToolNamespace == null) ? db.Name.Replace("-", "_") : CustomToolNamespace.Contains(".") ? CustomToolNamespace.Substring(0, CustomToolNamespace.LastIndexOf(".")) : CustomToolNamespace;
            name = name.Replace("_DEBUG", "");
            CodeNamespace ns = new CodeNamespace(name);
            ns.Comments.Add(new CodeCommentStatement("Generated - '" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "' by " + System.Security.Principal.WindowsIdentity.GetCurrent().Name));
            ns.Imports.Add(new CodeNamespaceImport("BOF"));
            ns.Imports.Add(new CodeNamespaceImport("BO"));
            ns.Imports.Add(new CodeNamespaceImport("DA"));
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Xml"));
            ns.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
            ns.Imports.Add(new CodeNamespaceImport("System.Drawing"));
            ns.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));
            return ns;
        }

        public CodeNamespace GetDatNamespaceFromDB(Database db)
        {
            string name = (CustomToolNamespace == null) ? db.Name.Replace("-", "_") : CustomToolNamespace.Contains(".") ? CustomToolNamespace.Substring(0, CustomToolNamespace.LastIndexOf(".")) : CustomToolNamespace;
            name = name.Replace("_DEBUG","");
            CodeNamespace ns = new CodeNamespace(name);
            ns.Comments.Add(new CodeCommentStatement("Generated - '" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "' by " + System.Security.Principal.WindowsIdentity.GetCurrent().Name));
            ns.Imports.Add(new CodeNamespaceImport("BO"));
            ns.Imports.Add(new CodeNamespaceImport("DA"));
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.Xml"));
            ns.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
            ns.Imports.Add(new CodeNamespaceImport("System.Drawing"));
            return ns;
        }

        //public void GetFormsFromTables(SortedList tables, Database db, ref CodeNamespace ns, bool createControllers, bool createFilterForms, bool createEntityForms, bool createMenu)
        //{
        //    foreach (DictionaryEntry obj in tables)
        //    {
        //        Table table = (Table)obj.Value;
        //        if (table.Name == "SysAudit") continue;

        //        CodeTypeDeclaration tp = null;

        //        string headerTable = "";
        //        DatClass helper = GenerateOurDatClass(table, out headerTable);

        //        if (createControllers)
        //        {
        //            tp = ControllersGenerator.CreateController(table, helper);
        //            ns.Types.Add(tp);
        //        }

        //        if (createFilterForms)
        //        {
        //            tp = ControllersGenerator.CreateFilterForm(table);
        //            ns.Types.Add(tp);

        //            tp = FiltersGenerator.CreateBaseFilterClass(table);
        //            FiltersGenerator.BaseFilterClassAddProperties(table, ref tp);
        //            //FiltersGenerator.BaseFilterClassAddMethods(table, ref tp);

        //            tp = ControllersGenerator.CreateFilterBaseForm(table, tp);
        //            ns.Types.Add(tp);
        //        }

        //        if (createEntityForms)
        //        {
        //            tp = ControllersGenerator.CreateEntityForm(table);
        //            ns.Types.Add(tp);
        //        }
        //    }

        //    if (createMenu)
        //        foreach (DictionaryEntry obj in tables)
        //        {
        //            Table table = (Table)obj.Value;
        //            ns.Comments.Add(new CodeCommentStatement(string.Format("ToolStripMenuItem {0} = MFController.AddMenuSubItem<{0}Controller>(Dict, \"{0}\");", table.Name)));
        //        }

        //    //StringBuilder sb = new StringBuilder();
        //    //new CSharpCodeProvider().GenerateCodeFromNamespace(ns, new StringWriter(sb), new CodeGeneratorOptions());
        //    //return sb.ToString();

        //}
        //public string GetCodeFromTables(SortedList tables, Database db, bool trusted, string login, string password, bool dbc, bool dat, bool set, bool filter, bool columns)
        //{
        //    string name = (CustomToolNamespace == null) ? db.Name.Replace("-", "_") : CustomToolNamespace.Contains(".") ? CustomToolNamespace.Substring(0, CustomToolNamespace.LastIndexOf(".")) : CustomToolNamespace;
        //    CodeNamespace ns = new CodeNamespace(name);
        //    ns.Comments.Add(new CodeCommentStatement("Generated - '" + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "' by " + System.Security.Principal.WindowsIdentity.GetCurrent().Name));
        //    ns.Imports.Add(new CodeNamespaceImport("BO"));
        //    ns.Imports.Add(new CodeNamespaceImport("DA"));
        //    ns.Imports.Add(new CodeNamespaceImport("System"));
        //    ns.Imports.Add(new CodeNamespaceImport("System.Xml"));
        //    ns.Imports.Add(new CodeNamespaceImport("System.Xml.Serialization"));
        //    ns.Imports.Add(new CodeNamespaceImport("System.Drawing"));

        //    if (dbc)
        //    {
        //        CodeTypeDeclaration dbclass = GenerateDBClass(db, trusted, login, password);
        //        ns.Types.Add(dbclass);
        //    }

        //    foreach (DictionaryEntry obj in tables)
        //    {
        //        Table table = (Table)obj.Value;
        //        if (table.Name == "SysAudit") continue;
        //        CodeTypeDeclaration tp = null;

        //        if (dat)
        //        {
        //            string headerTab = null;
        //            tp = GenerateDatClass(table, out headerTab);
        //            ns.Types.Add(tp);

        //            if (columns)
        //            {
        //                //tp = GenerateColumnConsts(tp);
        //                tp = GenerateColumnConsts(table, tp);
        //                if (tp != null) ns.Types.Add(tp);
        //            }

        //            if (!string.IsNullOrEmpty(headerTab))
        //            {
        //                tp = GenerateHeaderDatClass(db.Tables[headerTab].Name, table.Name);
        //                ns.Types.Add(tp);

        //                tp = GenerateHeaderSetClass(db.Tables[headerTab].Name, table.Name);
        //                ns.Types.Add(tp);

        //                tp = GenerateHeaderDetailsSetClass(db.Tables[headerTab].Name, table.Name);
        //                ns.Types.Add(tp);
        //            }

        //        }

        //        if (filter)
        //        {
        //            //DatClass fltclass = GenerateFilterClass(table);
        //            //ns.Types.Add(tp);

        //            tp = FiltersGenerator.CreateBaseFilterClass(table);
        //            FiltersGenerator.BaseFilterClassAddProperties(table, ref tp);
        //            FiltersGenerator.BaseFilterClassAddMethods(table, ref tp);
        //            ns.Types.Add(tp);

        //            tp = FiltersGenerator.CreateInheritedFilterClass(table);
        //            ns.Types.Add(tp);
        //        }


        //        if (set)
        //        {
        //            tp = GenerateSetClass(table);
        //            ns.Types.Add(tp);
        //        }
        //    }

        //    StringBuilder sb = new StringBuilder();
        //    new CSharpCodeProvider().GenerateCodeFromNamespace(ns, new StringWriter(sb), new CodeGeneratorOptions());
        //    return sb.ToString();
        //}

        public string GetCodeFromNamespace(CodeNamespace ns)
        {
            StringBuilder sb = new StringBuilder();
            new CSharpCodeProvider().GenerateCodeFromNamespace(ns, new StringWriter(sb), new CodeGeneratorOptions());
            return sb.ToString();
        }

        public Type GetTypeFromXsd(string name)
        {
            Type type = null;
            switch (name)
            {
                case "xs:int": type = typeof(int); break;
                case "xs:string": type = typeof(string); break;
                case "xs:dateTime": type = typeof(DateTime); break;
                case "xs:decimal": type = typeof(decimal); break;
                case "xs:boolean": type = typeof(bool); break;
            }
            return type;
        }



        public class DatClass
        {
            public string Name;
            public string Description;
            public List<string> BaseTypes = new List<string>();
            public List<DatMember> Members = new List<DatMember>();
            public bool Contains(string name)
            {
                foreach (DatMember dat in Members)
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
            public bool InPrimaryKey;
            public bool IsForeignKey;
            public List<CodeAttributeArgument> Ordinals = new List<CodeAttributeArgument>();
        }

        //private CodeTypeDeclaration GenerateColumnConsts(Table table)
        //{
        //    CodeTypeDeclaration ret = new CodeTypeDeclaration(Helper.Capitalise(table.Name) + "Columns");
        //    ret.IsPartial = true;
        //    ret.TypeAttributes = TypeAttributes.Public;
        //    for (int i = 0; i < table.Columns.Count; i++)
        //    {
        //        Column col = table.Columns[i];
        //        string name = col.Name; // col.Name.Substring(0, 1).ToUpper() + col.Name.Substring(1);
        //        if (name.ToUpper() == "ID") name = "ID";
        //        CodeMemberField mem = new CodeMemberField();
        //        mem.Type = new CodeTypeReference(typeof(string));
        //        mem.Attributes = MemberAttributes.Const | MemberAttributes.Public;
        //        mem.Comments.Add(new CodeCommentStatement("<summary>" + Helper.GetMSDescription(col) + "</summary>", true));
        //        mem.InitExpression = new CodeSnippetExpression("\"" + name + "\"");
        //        ret.Members.Add(mem);
        //    }
        //    return ret;
        //}

        public CodeTypeDeclaration GenerateColumnConsts(Table table, CodeTypeDeclaration dat)
        {
            string name = dat.Name.Substring(0, dat.Name.Length - 3) + "Columns";
            CodeTypeDeclaration ret = new CodeTypeDeclaration(name);
            ret.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThroughAttribute"));
            ret.IsPartial = true;
            ret.TypeAttributes = TypeAttributes.Public;

            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column col = table.Columns[i];
                string cname = Helper.Capitalise(col.Name);
                if (cname.ToUpper() == "ID") cname = "ID";

                CodeMemberField mem = new CodeMemberField();
                mem.Name = cname;
                mem.Type = new CodeTypeReference(typeof(string));
                mem.Attributes = MemberAttributes.Const | MemberAttributes.Public;
                mem.Comments.Add(new CodeCommentStatement("<summary>" + Helper.GetMSDescription(col) + "</summary>", true));
                mem.InitExpression = new CodeSnippetExpression("\"" + col.Name + "\"");
                ret.Members.Add(mem);
            }

            foreach (CodeTypeMember prop in dat.Members)
            {
                if (!(prop is CodeMemberProperty)) continue;
                //if (prop.CustomAttributes.Count > 0) prop.CustomAttributes[0].Arguments.Add(new CodeAttributeArgument(new CodeSnippetExpression(name + "." + prop.Name)));

                string cname = Helper.Capitalise(prop.Name);
                if (cname.ToUpper() == "ID") cname = "ID";

                if (!table.Columns.Contains(cname))
                {
                    CodeMemberField mem = new CodeMemberField();
                    mem.Name = Helper.Capitalise(prop.Name);
                    mem.Type = new CodeTypeReference(typeof(string));
                    mem.Attributes = MemberAttributes.Const | MemberAttributes.Public;
                    if (prop.Comments.Count > 0) mem.Comments.Add(new CodeCommentStatement(prop.Comments[0].Comment.Text, true));
                    mem.InitExpression = new CodeSnippetExpression("\"" + prop.Name + "\"");
                    ret.Members.Add(mem);
                }
            }


            return (ret.Members.Count > 1) ? ret : null;
        }
        public CodeTypeDeclaration GenerateDBClass(Database db, bool trusted, string login, string password)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = "DB_" + db.Name.Replace("_DEBUG", "").Replace("-", "_");
            ret.IsPartial = false;
            ret.IsClass = true;
            ret.Attributes = MemberAttributes.Public | MemberAttributes.Static;

            CodeMemberField fieldConnString = new CodeMemberField();
            fieldConnString.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;
            fieldConnString.Name = "_ConnectionString";
            fieldConnString.Type = new CodeTypeReference(typeof(string));

            StringBuilder csBuilder = new StringBuilder();
            csBuilder.AppendFormat("Server={0};DataBase={1};", db.Parent.Name, db.Name);
            if (trusted)
                csBuilder.Append("Integrated Security=SSPI;");
            else
                csBuilder.AppendFormat("User ID={0};Password={1};", login, password);

            fieldConnString.InitExpression = new CodeSnippetExpression("\"" + csBuilder.ToString() + "\"");

            CodeMemberProperty propConnString = new CodeMemberProperty();
            propConnString.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            propConnString.Name = "ConnectionString";
            propConnString.Type = fieldConnString.Type;
            propConnString.SetStatements.Add(new CodeSnippetExpression(string.Format("{0} = value", fieldConnString.Name)));
            propConnString.GetStatements.Add(new CodeSnippetExpression(string.Format("return {0}", fieldConnString.Name)));


            CodeMemberField fieldDataAccessor = new CodeMemberField();
            fieldDataAccessor.Attributes = MemberAttributes.Private | MemberAttributes.Final | MemberAttributes.Static;
            fieldDataAccessor.Name = "_DbDataAccessor";
            fieldDataAccessor.Type = new CodeTypeReference("IDataAccess");
            fieldDataAccessor.InitExpression = new CodeSnippetExpression("null");

            CodeMemberProperty propDataAccessor = new CodeMemberProperty();
            propDataAccessor.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            propDataAccessor.Name = "DbDataAccessor";
            propDataAccessor.Type = fieldDataAccessor.Type;
            propDataAccessor.SetStatements.Add(new CodeSnippetExpression(string.Format("{0} = value", fieldDataAccessor.Name)));

            CodeConditionStatement if1 = new CodeConditionStatement(new CodeSnippetExpression(string.Format("{0} == null", fieldDataAccessor.Name)));
            CodeConditionStatement if2 = new CodeConditionStatement(new CodeSnippetExpression(string.Format("string.IsNullOrEmpty({0})", fieldConnString.Name)));
            if1.TrueStatements.Add(if2);
            if2.TrueStatements.Add(new CodeSnippetExpression(string.Format("{0} = DA.Global.DefaultConnection", fieldDataAccessor.Name)));
            if2.FalseStatements.Add(new CodeSnippetExpression(string.Format("{0} = new SQLDataAccess({1})", fieldDataAccessor.Name, fieldConnString.Name)));

            propDataAccessor.GetStatements.Add(if1);
            propDataAccessor.GetStatements.Add(new CodeSnippetExpression(string.Format("return {0}", fieldDataAccessor.Name)));


            //propDataAccessor.GetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(string.Format("{} == null", fieldDataAccessor.Name)),
            //        new CodeConditionStatement(new CodeSnippetExpression(string.Format("string.IsNullOrEmpty({0})", fieldConnString.Name)),
            //        new CodeSnippetStatement[] 
            //            { 
            //            new CodeSnippetStatement[] {new CodeSnippetStatement("_DbDataAccessor = DA.Global.DefaultConnection")},
            //            new CodeSnippetStatement[] {new CodeSnippetStatement("_DbDataAccessor = new SQLDataAccess(_ConnectionString)")}
            //            }
            //            )));


            ret.Members.Add(fieldConnString);
            ret.Members.Add(fieldDataAccessor);
            ret.Members.Add(propConnString);
            ret.Members.Add(propDataAccessor);

            //CodeMemberMethod mset = new CodeMemberMethod();
            //mset.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            //mset.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
            //mset.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "value"));



            return ret;
        }

        public static CodeMemberProperty DataAccessorProperty(Database db)
        {
            CodeMemberProperty ret = new CodeMemberProperty();
            ret.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            ret.Name = "DataAccessor";
            ret.Type = new CodeTypeReference("IDataAccess");
            ret.GetStatements.Add(new CodeSnippetExpression(string.Format("return DataAccessorIsNull() ? DB_{0}.DbDataAccessor : base.DataAccessor", db.Name.Replace("_DEBUG","").Replace("-", "_"))));

            return ret;

            //public override IDataAccess DataAccessor
            //{
            //    get { return DataAccessorIsNull() ? DB_PifDocs.DbDataAccessor : base.DataAccessor; }
            //}

        }


        public CodeTypeDeclaration GenerateHeaderDatClass(string headerTableName, string detailsTableName)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(headerTableName) + "Dat";
            ret.IsPartial = true;
            string ty = string.Format("DetailsWrapper<{1}Dat, {1}Set, {0}Dat>", headerTableName, detailsTableName);

            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            field.Name = "_Details";
            field.Type = new CodeTypeReference(ty);
            field.InitExpression = new CodeSnippetExpression("null");
            ret.Members.Add(field);

            CodeMemberProperty prop = new CodeMemberProperty();
            prop.CustomAttributes.Add(new CodeAttributeDeclaration("XmlIgnore"));
            prop.Attributes = MemberAttributes.Public;
            prop.Name = "Details";
            prop.Type = new CodeTypeReference(ty);
            prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
            prop.SetStatements.Add(new CodeSnippetExpression(field.Name + " = value"));
            ret.Members.Add(prop);

            CodeMemberMethod init = new CodeMemberMethod();
            init.Name = "Init";
            init.Attributes = MemberAttributes.Family | MemberAttributes.Override;
            init.Statements.Add(new CodeSnippetExpression("base.Init()"));
            init.Statements.Add(new CodeSnippetExpression(string.Format("_Details = new DetailsWrapper<{0}Dat, {0}Set, {1}Dat>(this)", detailsTableName, headerTableName)));
            ret.Members.Add(init);

            return ret;
        }
        public CodeTypeDeclaration GenerateHeaderDetailsSetClass(string headerTableName, string detailsTableName)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(detailsTableName) + "Set";
            ret.IsPartial = true;

            CodeTypeConstructor constr = new CodeTypeConstructor();
            //constr.Attributes = MemberAttributes.Static | MemberAttributes.Final;
            constr.Statements.Add(new CodeSnippetExpression(string.Format("BaseSet<{0}Dat, {0}Set>.HeaderOrdinal = 1", detailsTableName)));
            ret.Members.Add(constr);

            CodeMemberMethod load = new CodeMemberMethod();
            load.Name = "Load";
            load.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            load.Statements.Add(new CodeSnippetExpression(string.Format("throw new Exception(\"{0}Set.Load: Ќельз€ загружать напр€мую, можно только через {1}Dat.Load\")", detailsTableName, headerTableName)));
            ret.Members.Add(load);

            return ret;

        }
        public CodeTypeDeclaration GenerateHeaderSetClass(string headerTableName, string detailsTableName)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(headerTableName) + "Set";
            ret.IsPartial = true;
            string ty = string.Format("{0}Set", detailsTableName);

            CodeMemberField field = new CodeMemberField();
            field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            field.Name = "_DetailsSet";
            field.Type = new CodeTypeReference(ty);
            field.InitExpression = new CodeSnippetExpression("null");

            CodeMemberProperty prop = new CodeMemberProperty();
            prop.CustomAttributes.Add(new CodeAttributeDeclaration("XmlIgnore"));
            prop.Attributes = MemberAttributes.Public;
            prop.Name = "DetailsSet";
            prop.Type = new CodeTypeReference(ty);
            prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
            prop.SetStatements.Add(new CodeSnippetExpression(field.Name + " = value"));

            ret.Members.Add(field);
            ret.Members.Add(prop);

            return ret;
        }
        public static CodeTypeDeclaration GenerateDatClass(Table table, out string headerTable)
        {
            return CreateDatClass(GenerateOurDatClass(table, out headerTable), table.Parent);
        }
        public static DatClass GenerateOurDatClass(Table table, out string headerTable)
        {
            headerTable = null;
            DatClass dat = new DatClass();
            dat.Name = Helper.Capitalise(table.Name);
            dat.BaseTypes.Add("BaseDat<" + dat.Name + "Dat>");
            dat.Description = Helper.GetMSDescription(table);
            for (int i = 0; i < table.Columns.Count; i++)
            {
                Column col = table.Columns[i];
                DatMember mem = new DatMember();
                string name = Helper.Capitalise(col.Name);
                if (name.ToUpper() == "ID") name = "ID";
                if (name.ToUpper() == "FP") name = "FP";
                mem.Name = mem.ColumnName = name;
                mem.Description = Helper.GetMSDescription(col);

                mem.DataType = Helper.GetTypeFromDBType(col.DataType.SqlDataType).Name;
                mem.InPrimaryKey = col.InPrimaryKey;
                mem.IsForeignKey = col.IsForeignKey;
                //dat.

                foreach (ForeignKey fk in table.ForeignKeys)
                {
                    if (fk.Columns.Contains(col.Name))
                    {
                        if (mem.Name.ToUpper() != "FP" && mem.Name.ToUpper() != "PARENT_FP" && mem.Name.ToUpper() != "ID" && fk.Columns[0].Name == col.Name)
                        {

                            if (mem.DataType.ToLower().StartsWith("int") && !mem.Name.ToLower().EndsWith("_ref"))
                            {
                                int ind = 0;

                                if (mem.Name.ToLower().EndsWith("_id"))
                                {
                                    ind = mem.Name.ToLower().IndexOf("_id");
                                }
                                else if (mem.Name.ToLower().EndsWith("id"))
                                {
                                    ind = mem.Name.ToLower().IndexOf("id");
                                }

                                if (ind > 0)
                                {
                                    mem.Name = mem.Name.Substring(0, ind);


                                }

                                mem.DataType = fk.ReferencedTable + "Dat";
                            }
                        }

                        if (mem.Name.ToUpper() != "FPN" && mem.Name.ToUpper() != "PARENT_FPN" && mem.Name.ToUpper() != "ID" && fk.Columns[0].Name == col.Name)
                        {

                            if (mem.DataType.ToLower().StartsWith("int") && !mem.Name.ToLower().EndsWith("_ref"))
                            {
                                int ind = 0;

                                if (mem.Name.ToLower().EndsWith("_id"))
                                {
                                    ind = mem.Name.ToLower().IndexOf("_id");
                                }
                                else if (mem.Name.ToLower().EndsWith("id"))
                                {
                                    ind = mem.Name.ToLower().IndexOf("id");
                                }

                                if (ind > 0)
                                {
                                    mem.Name = mem.Name.Substring(0, ind);


                                }

                                mem.DataType = fk.ReferencedTable + "Dat";
                            }
                        }
                        if (mem.Name.ToUpper() == "HEADER" && mem.IsForeignKey)
                        {
                            string iface = "IDetailDat<" + mem.DataType + ">";
                            if (!dat.BaseTypes.Contains(iface)) dat.BaseTypes.Add(iface);
                            headerTable = fk.ReferencedTable;
                        }

                        if (mem.Name.ToUpper() == "ID" && fk.Columns[0].Name == col.Name)
                        {
                            mem.Name = "Root";
                            mem.DataType = fk.ReferencedTable + "Dat";
                            mem.DataType = Helper.Capitalise(mem.DataType);
                            string iface = "IExpPropDat<" + mem.DataType + ">";
                            if (!dat.BaseTypes.Contains(iface)) dat.BaseTypes.Add(iface);
                        }
                    }
                }
                mem.DataType = Helper.Capitalise(mem.DataType);

                DatMember dm = dat.Members.Find(delegate(DatMember dat1)
                {
                    return (dat1.Name == mem.Name);
                }
                );

                if (dm != null)
                    mem.Name = mem.Name + dat.Members.Count.ToString();

                dat.Members.Add(mem);
            }

            if (dat.Contains("Parent_FP") && dat.Contains("ID") && dat.Contains("Code") && (dat.Contains("Name") || dat.Name == "Employer"))
            {
                dat.BaseTypes.Add("ICardDat");
            }
            if (dat.Contains("Parent_FPn") && dat.Contains("ID") && dat.Contains("CodeN") && (dat.Contains("Name") || dat.Name == "Employer"))
            {
                dat.BaseTypes.Add("ICardNDat");
            }
            else if (dat.Contains("Parent_FP") && dat.Contains("ID") && dat.Contains("FP") && dat.Contains("Name"))
            {
                dat.BaseTypes.Add("ITreeDat");
            }
            else if (dat.Contains("Parent_FPn") && dat.Contains("ID") && dat.Contains("FPn") && dat.Contains("Name"))
            {
                dat.BaseTypes.Add("ITreeNDat");
            }

            if (dat.Contains("Name"))
            {
                dat.BaseTypes.Add("IHasName");
            }

            if (dat.Contains("ValueDate"))
            {
                dat.BaseTypes.Add("IHasVersion");
            }

            if (dat.Contains("IsDeleted"))
            {
                dat.BaseTypes.Add("IHasIsDeleted");
            }

            if (dat.Contains("SCode") && dat.Contains("Name"))
            {
                dat.BaseTypes.Add("IDictDat");
            }

            if (dat.Contains("BackColor"))
            {
                dat.BaseTypes.Add("IHasBackColor");
            }

            if (dat.Contains("ForeColor"))
            {
                dat.BaseTypes.Add("IHasForeColor");
            }


            if (dat.Members[0].DataType.ToLower().StartsWith("int"))
            {
                if (dat.Members[0].Name.ToLower() == "id")
                {
                    if (dat.BaseTypes.Count == 1) dat.BaseTypes.Add("IDat");
                }
                else
                {
                    if (dat.BaseTypes.Count == 1) dat.BaseTypes.Add("IDatNoID");
                }

            }
            else if (dat.Members[0].DataType.ToLower() == "guid")
            {
                if (dat.BaseTypes.Count == 1) dat.BaseTypes.Add("IDatGuid");
            }

            return dat;
            //return CreateDatClass(dat, table.Parent);
        }

        public CodeTypeDeclaration GenerateSetClass(Table table)
        {
            DatClass dat = new DatClass();
            dat.Name = Helper.Capitalise(table.Name);
            foreach (ForeignKey fk in table.ForeignKeys)
            {
                string name = fk.ReferencedTable;
                Column FKColumn = table.Columns[fk.Columns[0].Name];


                name = Helper.Capitalise(name);

                if (FKColumn.Name.ToLower().EndsWith("_ref")) continue;
                if (FKColumn.DataType.Name != DataType.Int.Name && FKColumn.DataType.Name != DataType.BigInt.Name) continue;

                if (dat.Name.ToLower() == name.ToLower() && fk.Columns[0].Name.ToLower() != "id") continue;
                if (fk.Columns[0].Name.ToLower() == "parent_fp") continue;
                if (fk.Columns[0].Name.ToLower() == "parent_fpn") continue;
                //if (fk.Columns[0].Name.ToLower() != "id") continue;
                //if (fk.Columns[0].Name.ToUpper() != "ID" && (name == table.Name || fk.Columns[0].Name.ToLower().EndsWith("_ref"))) continue;

                DatMember oldmem = null;
                foreach (DatMember mem in dat.Members)
                {
                    if (mem.Name == name) oldmem = mem;
                }
                if (oldmem != null)
                {
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (fk.Columns[0].Name == table.Columns[i].Name) oldmem.Ordinals.Add(new CodeAttributeArgument(new CodeSnippetExpression(i.ToString())));
                    }
                }
                else
                {
                    DatMember mem = new DatMember();
                    mem.Name = (fk.Columns[0].Name.ToUpper() == "ID") ? "Header" : name;
                    mem.DataType = name + "Set";
                    List<CodeAttributeArgument> args = new List<CodeAttributeArgument>();
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (fk.Columns[0].Name == table.Columns[i].Name) mem.Ordinals.Add(new CodeAttributeArgument(new CodeSnippetExpression(i.ToString())));
                    }
                    dat.Members.Add(mem);
                }

            }
            return CreateSetClass(dat, table.Parent);
        }
        public static CodeTypeDeclaration CreateDatClass(DatClass dat, Database db)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.UserData["TableName"] = dat.Name;
            ret.Name = dat.Name + "Dat";
            ret.IsPartial = true;
            ret.Comments.Add(new CodeCommentStatement("<summary>" + dat.Description + "</summary>", true));
            //ret.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThroughAttribute"));
            foreach (string type in dat.BaseTypes)
            {
                ret.BaseTypes.Add(new CodeTypeReference(type));
            }
            ret.Members.Add(DataAccessorProperty(db));
            CodeConstructor constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            ret.Members.Add(constr);
            constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            constr.Parameters.Add(new CodeParameterDeclarationExpression("params object[]", "args"));
            constr.BaseConstructorArgs.Add(new CodeSnippetExpression("args"));
            ret.Members.Add(constr);
            CodeMemberProperty m_pfp = null;
            CodeMemberProperty m_fp = null;
            CodeMemberProperty m_codeS = null;

            for (int i = 0; i < dat.Members.Count; i++)
            {
                DatMember mem = dat.Members[i];
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Attributes = MemberAttributes.Public;
                prop.CustomAttributes.Add(new CodeAttributeDeclaration("FieldInfoDat", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodeSnippetExpression(i.ToString())), new CodeAttributeArgument(new CodeSnippetExpression("\"" + mem.ColumnName + "\"")) }));
                //prop.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThroughAttribute"));
                //[System.Diagnostics.DebuggerStepThroughAttribute()]
                prop.Comments.Add(new CodeCommentStatement("<summary>" + mem.Description + "</summary>", true));
                prop.UserData["MSDescription"] = mem.Description;
                prop.Name = mem.Name;
                prop.Type = new CodeTypeReference(mem.DataType);
                CodeMemberMethod mget = new CodeMemberMethod();
                //mget.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerHiddenAttribute"));
                mget.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mget.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mget.ReturnType = new CodeTypeReference(typeof(object));
                CodeMemberMethod mset = new CodeMemberMethod();
                mset.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mset.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mset.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "value"));
                if (prop.Name.ToUpper() == "ROOT" && !dat.Contains("ID"))
                {
                    field.InitExpression = new CodeSnippetExpression("new " + mem.DataType + "()");
                    CodeMemberProperty pr = new CodeMemberProperty();
                    pr.Attributes = MemberAttributes.Public;
                    pr.Name = "ID";
                    pr.Type = new CodeTypeReference("Int32");
                    pr.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("_Root.ID != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t_Root.ID = value;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(\"ID\");")}));
                    pr.GetStatements.Add(new CodeSnippetExpression("return _Root.ID"));
                    ret.Members.Add(pr);
                }
                else if (prop.Name.ToUpper() == "PARENT_FP")
                {
                    m_pfp = prop;
                    prop.Name = "Parent_FP";
                    prop.Type = new CodeTypeReference("PathTree");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= O2PathTree(value)"));
                }
                else if (prop.Name.ToUpper() == "PARENT_FPN")
                {
                    m_pfp = prop;
                    prop.Name = "Parent_FPn";
                    prop.Type = new CodeTypeReference("PathTreeN");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= O2PathTreeN(value)"));
                }
                else if (prop.Name.ToUpper() == "FP")
                {
                    m_fp = prop;
                    prop.Name = "FP";
                    prop.Type = new CodeTypeReference("PathTree");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= O2PathTree(value)"));
                }
                else if (prop.Name.ToUpper() == "FPN")
                {
                    m_fp = prop;
                    prop.Name = "FPn";
                    prop.Type = new CodeTypeReference("PathTreeN");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= O2PathTreeN(value)"));

                    m_codeS = new CodeMemberProperty();
                    m_codeS.Attributes = MemberAttributes.Public;
                    m_codeS.Name = "CodeS";
                    m_codeS.Type = new CodeTypeReference(typeof(string));
                }
                prop.Name = Helper.Capitalise(prop.Name);
                mget.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerHiddenAttribute"));
                mget.Statements.Add(new CodeSnippetExpression("return ((" + ret.Name + ")dat)." + prop.Name));
                if (mset.Statements.Count == 0)
                {
                    if (mem.DataType.EndsWith("Dat"))
                        mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= value as " + mem.DataType));
                    else
                        mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= O2" + mem.DataType.Replace("[]", "Array") + "(value)"));
                }
                field.Name = "_" + prop.Name;
                field.Type = prop.Type;
                prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression(field.Name + " != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t" + field.Name + " = value;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns." + prop.Name + ");"),
                                        }));

                if (dat.BaseTypes.Contains("ICardDat") && (prop.Name.ToUpper() == "PARENT_FP" || prop.Name.ToUpper() == "CODE")) prop.SetStatements.Add(new CodeSnippetExpression("NotifyPropertyChanged(\"FP\");"));
                if (dat.BaseTypes.Contains("ICardNDat") && (prop.Name.ToUpper() == "PARENT_FPN" || prop.Name.ToUpper() == "CODEN")) prop.SetStatements.Add(new CodeSnippetExpression("NotifyPropertyChanged(\"FPn\");"));

                prop.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerHiddenAttribute"));
                prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                ret.Members.Add(field);
                ret.Members.Add(prop);
                mget.Name = "Get" + field.Name;
                ret.Members.Add(mget);
                mset.Name = "Set" + field.Name;
                ret.Members.Add(mset);
            }

            if (dat.BaseTypes.Contains("ICardDat"))
            {
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                prop.Name = "FP";
                prop.Type = new CodeTypeReference("PathCard");
                prop.GetStatements.Add(new CodeSnippetExpression("return (Code > 0) ? new PathCard(Parent_FP, Code):null"));
                prop.SetStatements.Add(new CodeSnippetExpression("PathTree pfp = (value == null) ? null : value.Parent"));
                prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("Parent_FP != pfp"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\tParent_FP = pfp;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.Parent_FP);")}));
                prop.SetStatements.Add(new CodeSnippetExpression("int c = (value == null) ? 0 : value.Code;"));
                prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("Code != c"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\tCode = c;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(\"" + dat.Name + "Columns.Code\");")}));
                prop.SetStatements.Add(new CodeSnippetExpression("NotifyPropertyChanged(" + dat.Name + "Columns.FP)"));
                ret.Members.Add(prop);
                CodeMemberMethod mget = new CodeMemberMethod();
                mget.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mget.Name = "Get_FP";
                mget.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mget.ReturnType = new CodeTypeReference(typeof(object));
                mget.Statements.Add(new CodeSnippetExpression("return ((" + ret.Name + ")dat).FP"));
                ret.Members.Add(mget);
                CodeMemberMethod mset = new CodeMemberMethod();
                mset.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mset.Name = "Set_FP";
                mset.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mset.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "value"));
                mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= value as PathCard"));
                ret.Members.Add(mset);
            }

            if (dat.BaseTypes.Contains("ICardNDat"))
            {
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                prop.Name = "FPn";
                prop.Type = new CodeTypeReference("PathCardN");
                prop.GetStatements.Add(new CodeSnippetExpression("return (!string.IsNullOrEmpty(CodeN)) ? new PathCardN(Parent_FPn, CodeN) : null"));
                prop.SetStatements.Add(new CodeSnippetExpression("PathTreeN pfp = (value == null) ? null : value.Parent"));
                prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("Parent_FPn != pfp"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\tParent_FPn = pfp;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.Parent_FPn);")}));
                prop.SetStatements.Add(new CodeSnippetExpression("string c = (value == null) ? null : value.CodeN;"));
                prop.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("CodeN != c"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\tCodeN = c;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(\"" + dat.Name + "Columns.CodeN\");")}));
                prop.SetStatements.Add(new CodeSnippetExpression("NotifyPropertyChanged(" + dat.Name + "Columns.FPn)"));
                ret.Members.Add(prop);
                CodeMemberMethod mget = new CodeMemberMethod();
                mget.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mget.Name = "Get_FPn";
                mget.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mget.ReturnType = new CodeTypeReference(typeof(object));
                mget.Statements.Add(new CodeSnippetExpression("return ((" + ret.Name + ")dat).FPn"));
                ret.Members.Add(mget);
                CodeMemberMethod mset = new CodeMemberMethod();
                mset.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mset.Name = "Set_FPn";
                mset.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mset.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "value"));
                mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= value as PathCardN"));
                ret.Members.Add(mset);
            }

            if (dat.BaseTypes.Contains("ITreeDat"))
            {
                m_fp.SetStatements.Add(new CodeSnippetExpression("PathTree pfp = (value == null) ? null : value.Parent"));
                m_fp.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("_Parent_FP != pfp"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t_Parent_FP = pfp;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.Parent_FP);")}));
                m_pfp.SetStatements.Clear();
                m_pfp.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("_Parent_FP != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t_Parent_FP = (_FP == null)?null:value;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.Parent_FP);")}));

                //m_fp.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("_FP != null"), new CodeStatement[]{ 
                //                        new CodeSnippetStatement("\t\t\t\t\t_FP.Parent = value;"),
                //                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.FP);")}));
            }

            if (dat.BaseTypes.Contains("ITreeNDat"))
            {
                m_fp.SetStatements.Add(new CodeSnippetExpression("PathTreeN pfp = (value == null) ? null : value.Parent"));
                m_fp.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("_Parent_FPn != pfp"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t_Parent_FPn = pfp;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.Parent_FPn);")}));
                m_pfp.SetStatements.Clear();
                m_pfp.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("_Parent_FPn != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\t_Parent_FPn = (_FPn == null)?null:value;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.Parent_FPn);")}));

                m_codeS.Comments.Add(new CodeCommentStatement("<summary>„иним биндинг, сломанный в .NET 4 дл€ WinXP</summary>", true));
                m_codeS.SetStatements.Clear();
                m_codeS.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("FPn.CodeS != value"), new CodeStatement[]{ 
                                        new CodeSnippetStatement("\t\t\t\t\tFPn.CodeS = value;"),
                                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(\"CodeS\");")}));

                m_codeS.GetStatements.Add(new CodeSnippetExpression("return FPn.CodeS"));

                
                ret.Members.Add(m_codeS);

                //m_fp.SetStatements.Add(new CodeConditionStatement(new CodeSnippetExpression("_FP != null"), new CodeStatement[]{ 
                //                        new CodeSnippetStatement("\t\t\t\t\t_FP.Parent = value;"),
                //                        new CodeSnippetStatement("\t\t\t\t\tNotifyPropertyChanged(" + dat.Name + "Columns.FP);")}));
            }
            return ret;
        }

        private CodeTypeDeclaration CreateSetClass(DatClass dat, Database db)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = dat.Name + "Set";
            //ret.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThroughAttribute"));
            ret.BaseTypes.Add(new CodeTypeReference("BaseSet<" + dat.Name + "Dat, " + ret.Name + ">"));
            ret.BaseTypes.Add("ISet");
            ret.IsPartial = true;
            ret.Members.Add(DataAccessorProperty(db));

            //public UserOrderSet() : base() { }
            CodeConstructor constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            //constr.
            //constr.BaseConstructorArgs.Add(new CodeExpression());
            ret.Members.Add(constr);

            //public UserOrderSet(UserOrderFilter filter) : base(filter) { }
            CodeConstructor constrf = new CodeConstructor();
            constrf.Attributes = MemberAttributes.Public;
            constrf.Parameters.Add(new CodeParameterDeclarationExpression("BaseLoadFilter", "filter"));
            constrf.BaseConstructorArgs.Add(new CodeSnippetExpression("filter"));
            ret.Members.Add(constrf);

            CodeConstructor constrf2 = new CodeConstructor();
            constrf2.Attributes = MemberAttributes.Public;
            constrf2.Parameters.Add(new CodeParameterDeclarationExpression("IDAOFilter", "filter"));
            constrf2.BaseConstructorArgs.Add(new CodeSnippetExpression("filter"));
            ret.Members.Add(constrf2);

            for (int i = 0; i < dat.Members.Count; i++)
            {
                DatMember mem = dat.Members[i];
                CodeMemberField field = new CodeMemberField(mem.DataType, "_" + mem.Name);
                field.InitExpression = new CodeSnippetExpression("new " + mem.DataType + "()");
                ret.Members.Add(field);
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.CustomAttributes.Add(new CodeAttributeDeclaration("FieldInfoOrdinals", mem.Ordinals.ToArray()));
                prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                prop.Name = mem.Name;
                prop.Type = new CodeTypeReference(mem.DataType);
                prop.SetStatements.Add(new CodeSnippetExpression("_" + mem.Name + " = value"));
                prop.GetStatements.Add(new CodeSnippetExpression("return _" + mem.Name));
                ret.Members.Add(prop);
            }
            return ret;
        }

        private CodeTypeDeclaration GenerateDatClass(DataTable table)
        {
            int count = table.Columns.Count;
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.BaseTypes.Add(new CodeTypeReference("BaseDat<" + table.TableName + "Dat>"));
            ret.Name = table.TableName + "Dat";
            ret.IsPartial = true;
            bool HasID = false;
            bool HasCode = false;
            bool HasCodeN = false;
            bool HasName = false;
            bool HasParent_FP = false;
            bool HasFP = false;
            bool HasParent_FPn = false;
            bool HasFPn = false;
            CodeConstructor constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            ret.Members.Add(constr);
            constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            constr.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object[]), "args"));
            constr.BaseConstructorArgs.Add(new CodeSnippetExpression("args"));
            ret.Members.Add(constr);
            for (int i = 0; i < count; i++)
            {
                DataColumn col = table.Columns[i];
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.CustomAttributes.Add(new CodeAttributeDeclaration("FieldInfoDat", new CodeAttributeArgument[] { new CodeAttributeArgument(new CodeSnippetExpression(i.ToString())), new CodeAttributeArgument(new CodeSnippetExpression("\"" + col.ColumnName + "\"")) }));
                //prop.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerStepThroughAttribute"));
                //[System.Diagnostics.DebuggerStepThroughAttribute()]
                prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                prop.Name = col.Caption;
                prop.Type = new CodeTypeReference(col.DataType);
                CodeMemberMethod mget = new CodeMemberMethod();
                //mget.CustomAttributes.Add(new CodeAttributeDeclaration("System.Diagnostics.DebuggerHiddenAttribute"));
                mget.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mget.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mget.ReturnType = new CodeTypeReference(typeof(object));
                CodeMemberMethod mset = new CodeMemberMethod();
                mset.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mset.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mset.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "value"));
                if (prop.Name.ToUpper() == "ID")
                {
                    prop.Name = "ID";
                    HasID = true;
                }
                if (prop.Name.ToUpper() == "CODE")
                {
                    prop.Name = "Code";
                    HasCode = true;
                }
                if (prop.Name.ToUpper() == "CODEN")
                {
                    prop.Name = "CodeN";
                    HasCodeN = true;
                }
                if (prop.Name.ToUpper() == "NAME")
                {
                    prop.Name = "Name";
                    HasName = true;
                }
                if (prop.Name.ToUpper() == "PARENT_FP")
                {
                    prop.Name = "Parent_FP";
                    prop.Type = new CodeTypeReference("PathTree");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= ToPathTree(value)"));
                    HasParent_FP = true;
                }
                if (prop.Name.ToUpper() == "PARENT_FPN")
                {
                    prop.Name = "Parent_FPn";
                    prop.Type = new CodeTypeReference("PathTreeN");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= ToPathTreeN(value)"));
                    HasParent_FP = true;
                }
                if (prop.Name.ToUpper() == "FP")
                {
                    prop.Name = "FP";
                    prop.Type = new CodeTypeReference("PathTree");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= ToPathTree(value)"));
                    HasFP = true;
                }
                if (prop.Name.ToUpper() == "FPN")
                {
                    prop.Name = "FPn";
                    prop.Type = new CodeTypeReference("PathTreeN");
                    mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= ToPathTreeN(value)"));
                    HasFP = true;
                }
                prop.Name = Helper.Capitalise(prop.Name);
                foreach (DataRelation rel in table.ParentRelations)
                {
                    if (
                        prop.Name.ToUpper() != "FP" &&
                        prop.Name.ToUpper() != "PARENT_FP" &&
                        prop.Name.ToUpper() != "FPN" &&
                        prop.Name.ToUpper() != "PARENT_FPN" &&
                        prop.Name.ToUpper() != "CODEN" &&
                        prop.Name.ToUpper() != "ID"
                        && rel.ChildTable == table &&
                        rel.ChildColumns[0] == col)
                    {
                        int ind = prop.Name.IndexOf("_id");
                        if (ind > 0)
                            prop.Name = prop.Name.Substring(0, ind);
                        string typename = rel.ParentTable.TableName + "Dat";
                        prop.Type = new CodeTypeReference(typename);
                        mget.ReturnType = prop.Type;
                        mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= value as " + typename));
                    }
                }
                mget.Statements.Add(new CodeSnippetExpression("return ((" + ret.Name + ")dat)." + prop.Name));
                if (mset.Statements.Count == 0) mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= To" + col.DataType.Name + "(value)"));
                field.Name = "_" + prop.Name;
                field.Type = prop.Type;
                prop.SetStatements.Add(new CodeSnippetExpression(field.Name + " = value"));
                prop.GetStatements.Add(new CodeSnippetExpression("return " + field.Name));
                ret.Members.Add(field);
                ret.Members.Add(prop);
                mget.Name = "Get" + field.Name;
                ret.Members.Add(mget);
                mset.Name = "Set" + field.Name;
                ret.Members.Add(mset);
            }
            if (HasID && HasParent_FP && HasCode && HasName)
            {
                ret.BaseTypes.Add("ICardDat");
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                prop.Name = "FP";
                prop.Type = new CodeTypeReference("PathCard");
                prop.GetStatements.Add(new CodeSnippetExpression("return (Code > 0)?new PathCard(Parent_FP, Code):null"));
                prop.SetStatements.Add(new CodeSnippetExpression("Parent_FP = value.Parent"));
                prop.SetStatements.Add(new CodeSnippetExpression("Code = value.Code"));
                ret.Members.Add(prop);
                CodeMemberMethod mget = new CodeMemberMethod();
                mget.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mget.Name = "Get_FP";
                mget.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mget.ReturnType = new CodeTypeReference(typeof(object));
                mget.Statements.Add(new CodeSnippetExpression("return ((" + ret.Name + ")dat).FP"));
                ret.Members.Add(mget);
                CodeMemberMethod mset = new CodeMemberMethod();
                mset.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mset.Name = "Set_FP";
                mset.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mset.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "value"));
                mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= value as PathCard"));
                ret.Members.Add(mset);
            }
            else if (HasID && HasParent_FPn && HasCodeN && HasName)
            {
                ret.BaseTypes.Add("ICardNDat");
                CodeMemberProperty prop = new CodeMemberProperty();
                prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                prop.Name = "FP";
                prop.Type = new CodeTypeReference("PathCardN");
                prop.GetStatements.Add(new CodeSnippetExpression("return (Code > 0) ? new PathCardN(Parent_FPn, CodeN):null"));
                prop.SetStatements.Add(new CodeSnippetExpression("Parent_FPn = value.Parent"));
                prop.SetStatements.Add(new CodeSnippetExpression("CodeN = value.CodeN"));
                ret.Members.Add(prop);
                CodeMemberMethod mget = new CodeMemberMethod();
                mget.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mget.Name = "Get_FPn";
                mget.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mget.ReturnType = new CodeTypeReference(typeof(object));
                mget.Statements.Add(new CodeSnippetExpression("return ((" + ret.Name + ")dat).FPn"));
                ret.Members.Add(mget);
                CodeMemberMethod mset = new CodeMemberMethod();
                mset.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                mset.Name = "Set_FPn";
                mset.Parameters.Add(new CodeParameterDeclarationExpression("BaseDat", "dat"));
                mset.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "value"));
                mset.Statements.Add(new CodeSnippetExpression("((" + ret.Name + ")dat)." + prop.Name + "= value as PathCardN"));
                ret.Members.Add(mset);
            }
            else if (HasID && HasParent_FP && HasFP && HasName)
            {
                ret.BaseTypes.Add("ITreeDat");
            }
            else if (HasID && HasParent_FPn && HasFPn && HasName)
            {
                ret.BaseTypes.Add("ITreeDatN");
            }
            else if (HasID)
                ret.BaseTypes.Add("IDat");
            return ret;
        }

        private CodeTypeDeclaration GenerateSetClass(DataTable table)
        {
            int count = table.Columns.Count;
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = table.TableName + "Set";
            ret.BaseTypes.Add(new CodeTypeReference("BaseSet<" + table.TableName + "Dat, " + ret.Name + ">"));
            ret.BaseTypes.Add("ISet");
            ret.IsPartial = true;

            //public UserOrderSet() : base() { }
            CodeConstructor constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public;
            constr.ChainedConstructorArgs.Add(new CodeSnippetExpression("base()"));
            ret.Members.Add(constr);

            //public UserOrderSet(UserOrderFilter filter) : base(filter) { }
            CodeConstructor constrf = new CodeConstructor();
            constrf.Attributes = MemberAttributes.Public;
            constrf.Parameters.Add(new CodeParameterDeclarationExpression("BaseLoadFilter", "filter"));
            constrf.ChainedConstructorArgs.Add(new CodeSnippetExpression("base(filter)"));
            ret.Members.Add(constrf);

            CodeConstructor constrf2 = new CodeConstructor();
            constrf2.Attributes = MemberAttributes.Public;
            constrf2.Parameters.Add(new CodeParameterDeclarationExpression("IDAOFilter", "filter"));
            constrf2.ChainedConstructorArgs.Add(new CodeSnippetExpression("base(filter)"));
            ret.Members.Add(constrf2);


            for (int i = 0; i < table.ParentRelations.Count; i++)
            {
                DataRelation rel = table.ParentRelations[i];
                string name = rel.ParentTable.TableName;
                if (name == table.TableName || rel.ParentColumns[0].ColumnName.ToUpper() != "ID" || rel.ChildColumns[0].ColumnName.ToUpper() == "ID") continue;
                CodeMemberProperty oldprop = null;
                foreach (CodeTypeMember pr in ret.Members)
                {
                    if (pr is CodeMemberProperty && pr.Name == name) oldprop = (CodeMemberProperty)pr;
                }
                if (oldprop == null)
                {
                    string type = rel.ParentTable.TableName + "Set";
                    CodeMemberField field = new CodeMemberField(type, "_" + name);
                    field.InitExpression = new CodeSnippetExpression("new " + type + "()");
                    ret.Members.Add(field);
                    CodeMemberProperty prop = new CodeMemberProperty();
                    string col = rel.ChildColumns[0].ColumnName;
                    List<CodeAttributeArgument> args = new List<CodeAttributeArgument>();
                    foreach (DataRelation dr in table.ParentRelations)
                    {
                        if (dr.ParentColumns[0] == rel.ParentColumns[0]) args.Add(new CodeAttributeArgument(new CodeSnippetExpression(dr.ChildColumns[0].Ordinal.ToString())));
                    }
                    prop.CustomAttributes.Add(new CodeAttributeDeclaration("FieldInfoOrdinals", args.ToArray()));
                    prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                    prop.Name = name;
                    prop.Type = new CodeTypeReference(type);
                    prop.SetStatements.Add(new CodeSnippetExpression("_" + name + " = value"));
                    prop.GetStatements.Add(new CodeSnippetExpression("return _" + name));
                    ret.Members.Add(prop);
                }
            }
            return ret;
        }
    }

    #region CustomToolBase
    public abstract class CustomToolBase : IVsSingleFileGenerator
    {
        abstract public string GetDefaultExtension();
        abstract public string GenerateCode(string fileContent);
        public string CustomToolNamespace
        {
            get { return customToolNamespace; }
            set { customToolNamespace = value; }
        }
        public string InputFilePath
        {
            get { return inputFilePath; }
            set { inputFilePath = value; }
        }
        protected void SetProgress(int complete, int total)
        {
            if (codeGeneratorProgress == null) return;

            codeGeneratorProgress.Progress(complete, total);
        }
        protected void ReportError(bool isWarning, int level,
            string error, int line, int column)
        {
            if (codeGeneratorProgress == null) return;

            codeGeneratorProgress.GeneratorError(
                isWarning, level, error, line, column);
        }

        private IVsGeneratorProgress codeGeneratorProgress;
        private string customToolNamespace;
        private string inputFilePath;

        public void Generate(
            string wszInputFilePath,
            string bstrInputFileContents,
            string wszDefaultNamespace,
            out System.IntPtr rgbOutputFileContents,
            out int pcbOutput,
            IVsGeneratorProgress pGenerateProgress)
        {
            inputFilePath = wszInputFilePath;
            customToolNamespace = wszDefaultNamespace;
            codeGeneratorProgress = pGenerateProgress;

            string code = GenerateCode(bstrInputFileContents);

            if (code == null)
                throw new NullReferenceException(
                    "Generated code string is null.");

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(code);
            pcbOutput = bytes.Length;
            rgbOutputFileContents = Marshal.AllocCoTaskMem(pcbOutput);
            Marshal.Copy(bytes, 0, rgbOutputFileContents, pcbOutput);
        }
    }


    [ComImport,
    Guid("3634494C-492F-4F91-8009-4541234E4E99"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVsSingleFileGenerator
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetDefaultExtension();

        void Generate(
            [MarshalAs(UnmanagedType.LPWStr)] string wszInputFilePath,
            [MarshalAs(UnmanagedType.BStr)]   string bstrInputFileContents,
            [MarshalAs(UnmanagedType.LPWStr)] string wszDefaultNamespace,
            out IntPtr rgbOutputFileContents,
            [MarshalAs(UnmanagedType.U4)] out int pcbOutput,
            IVsGeneratorProgress pGenerateProgress);
    }


    [ComImport,
    Guid("BED89B98-6EC9-43CB-B0A8-41D6E2D6669D"),
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVsGeneratorProgress
    {
        void GeneratorError(bool isWarning,
            [MarshalAs(UnmanagedType.U4)]   int dwLevel,
            [MarshalAs(UnmanagedType.BStr)] string bstrError,
            [MarshalAs(UnmanagedType.U4)]   int dwLine,
            [MarshalAs(UnmanagedType.U4)]   int dwColumn);
        void Progress(
            [MarshalAs(UnmanagedType.U4)] int nComplete,
            [MarshalAs(UnmanagedType.U4)] int nTotal);
    }
    #endregion
}
