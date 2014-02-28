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
    public static class ControllersGenerator
    {
        public static CodeTypeDeclaration CreateFilterBaseForm(Table table, CodeTypeDeclaration flt)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(table.Name) + "FilterFormBase";
            ret.IsPartial = true;
            ret.BaseTypes.Add(new CodeTypeReference("FilterFormFlowBase"));
            ret.Comments.Add(new CodeCommentStatement("<summary>" + "Базовая форма фильтра для " + Helper.GetMSDescription(table) + "</summary>", true));

            CodeConstructor constr = new CodeConstructor();
            constr.Attributes = MemberAttributes.Public | MemberAttributes.Overloaded;
            //constr..BaseConstructorArgs
            //constr.ChainedConstructorArgs.Add(new CodeSnippetExpression("base()"));
            ret.Members.Add(constr);

            CodeMemberMethod GetFilter = new CodeMemberMethod();
            GetFilter.Name = "GetFilter";
            GetFilter.ReturnType = new CodeTypeReference("BaseLoadFilter");
            GetFilter.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("{0}Filter flt = new {0}Filter()", Helper.Capitalise(table.Name))));

            CodeMemberMethod Init = new CodeMemberMethod();
            Init.Name = "Init";
            //GetFilter.ReturnType = new CodeTypeReference("BaseLoadFilter");
            Init.Parameters.Add(new CodeParameterDeclarationExpression("BaseLoadFilter", "filter"));
            Init.Attributes = MemberAttributes.Public | MemberAttributes.Override;
            Init.Statements.Add(new CodeSnippetExpression(string.Format("{0}Filter flt = filter as {0}Filter", Helper.Capitalise(table.Name))));
            Init.Statements.Add(new CodeSnippetExpression(string.Format("if (flt == null){{flt = new {0}Filter(); flt.ResetToDefault();}}", Helper.Capitalise(table.Name))));

            foreach (CodeTypeMember item in flt.Members)
            {

                if (item is CodeMemberProperty)
                {

                    CodeMemberProperty pr = (CodeMemberProperty)item;
                    if (pr.Name != "FltID")
                    {

                        CodeMemberField field = new CodeMemberField();
                        field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                        field.Name = "ctl_" + Helper.Capitalise(pr.Name);

                        switch (pr.Type.BaseType)
                        {
                            case "FilterID":
                                field.Type = new CodeTypeReference("ComboBox");
                                field.InitExpression = new CodeSnippetExpression("null");

                                if (pr.UserData["ReferencedTable"] != null)
                                {
                                    Init.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper<{2}Set, {2}Dat>.SetComboValue(ctl_{0}, null, flt.{0}, \"{1}\")", Helper.Capitalise(pr.Name), pr.UserData["ReferencedTableMSDescription"].ToString().Replace("\"", "\\\""), Helper.Capitalise(pr.UserData["ReferencedTable"].ToString()))));
                                    GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("flt.{0}.AddIDRange(FilterHelper.GetComboValue(ctl_{0}))", Helper.Capitalise(pr.Name))));
                                    constr.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.AddComboBox(ref ctl_{0}, pnlFlow, \"{1}\")", Helper.Capitalise(pr.Name), pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""))));
                                }

                                break;

                            case "DateTime":
                                field.Type = new CodeTypeReference("CtlDateTime");
                                field.InitExpression = new CodeSnippetExpression("null");

                                Init.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.SetCtlDateTimeValue(ctl_{0}, flt.{0})", Helper.Capitalise(pr.Name))));
                                GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("flt.{0} = ctl_{0}.Value", Helper.Capitalise(pr.Name))));
                                constr.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.AddCtlDateTime(ref ctl_{0}, pnlFlow, \"{1}\")", Helper.Capitalise(pr.Name), pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""))));

                                break;

                            case "FilterDateFromTo":
                                if (field.Name.ToLower().Contains("date"))
                                {
                                    field.Type = new CodeTypeReference("CtlFilterDateFromTo");
                                    field.InitExpression = new CodeSnippetExpression("null");

                                    Init.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.SetCtlDateTimeValue(ctl_{0}, flt.{0}.DtFrom, flt.{0}.DtTo)", Helper.Capitalise(pr.Name))));
                                    GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("flt.{0}.DtFrom = ctl_{0}.DateFrom", Helper.Capitalise(pr.Name))));
                                    GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("flt.{0}.DtTo = ctl_{0}.DateTo", Helper.Capitalise(pr.Name))));
                                    constr.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.AddCtlFilterDateFromTo(ref ctl_{0}, pnlFlow, \"{1}\")", Helper.Capitalise(pr.Name), pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""))));

                                }
                                break;

                            case "FilterBool":
                                field.Type = new CodeTypeReference("CheckBox");
                                field.InitExpression = new CodeSnippetExpression("null");

                                Init.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.SetCheckBoxValue(ctl_{0}, flt.{0}.IsTrue)", Helper.Capitalise(pr.Name))));
                                GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("flt.{0}.IsTrue = FilterHelper.GetCheckBoxValue(ctl_{0})", Helper.Capitalise(pr.Name))));
                                constr.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.AddCheckBox(ref ctl_{0}, pnlFlow, \"{1}\")", Helper.Capitalise(pr.Name), pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""))));

                                break;

                            case "FilterString":
                                field.Type = new CodeTypeReference("TextBox");
                                field.InitExpression = new CodeSnippetExpression("null");

                                Init.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.SetTextBoxValue(ctl_{0}, flt.{0}.Value)", Helper.Capitalise(pr.Name))));
                                GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("flt.{0}.Value = ctl_{0}.Text", Helper.Capitalise(pr.Name))));
                                constr.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.AddTextBox(ref ctl_{0}, pnlFlow, \"{1}\")", Helper.Capitalise(pr.Name), pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""))));

                                break;

                            case "FilterGUID":
                                field.Type = new CodeTypeReference("TextBox");
                                field.InitExpression = new CodeSnippetExpression("null");

                                //Init.Statements.Add(new CodeSnippetExpression(string.Format("FilterHelper.AddTextBox(ref ctl_{0}, pnlFlow, flt.{0}.Value, \"{0}\")", pr.Name)));
                                //GetFilter.Statements.Add(new CodeSnippetExpression(string.Format("flt.{0}.AddID(new Guid(ctl_{0}.Text))", pr.Name)));

                                break;

                            default:
                                Init.Comments.Add(new CodeCommentStatement(string.Format("cannot generate field for filterform: {0}", pr.Type.BaseType)));
                                GetFilter.Comments.Add(new CodeCommentStatement(string.Format("cannot generate field for filterform: {0}", pr.Type.BaseType)));
                                constr.Comments.Add(new CodeCommentStatement(string.Format("cannot generate field for filterform: {0}", pr.Type.BaseType)));
                                break;

                        }

                        if (field.Type.BaseType != "System.Void")
                            ret.Members.Add(field);
                    }
                }
            }

            constr.Statements.Add(new CodeSnippetExpression("FilterHelper.SetFormHeight(pnlFlow, pnlFlowTop)"));

            GetFilter.Statements.Add(new CodeSnippetExpression("return flt"));
            ret.Members.Add(GetFilter);
            ret.Members.Add(Init);

            return ret;
        }
        public static CodeTypeDeclaration CreateFilterForm(Table table)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(table.Name) + "FilterForm";
            ret.IsPartial = true;
            ret.BaseTypes.Add(new CodeTypeReference(Helper.Capitalise(table.Name) + "FilterFormBase"));
            ret.Comments.Add(new CodeCommentStatement("<summary>" + "Форма фильтра для " + Helper.GetMSDescription(table) + "</summary>", true));

            return ret;
        }
        private static CodeMethodInvokeExpression ExMessageBox()
        {
            List<CodeExpression> pars = new List<CodeExpression>();
            pars.Add(new CodeSnippetExpression("Common.ExMessage(Ex)"));
            pars.Add(new CodePrimitiveExpression("Ошибка"));
            pars.Add(new CodeSnippetExpression("MessageBoxButtons.OK"));
            pars.Add(new CodeSnippetExpression("MessageBoxIcon.Error"));

            CodeMethodInvokeExpression ret = new CodeMethodInvokeExpression(
                            new CodeSnippetExpression("MessageBox")
                            , "Show"
                            , pars.ToArray()
                            );
            return ret;
        }
        private static CodeTryCatchFinallyStatement TryCatchMessageBox()
        {
            CodeTryCatchFinallyStatement ret = new CodeTryCatchFinallyStatement();
            CodeCatchClause catch2 = new CodeCatchClause("Ex");
            catch2.Statements.Add(ExMessageBox());
            ret.CatchClauses.Add(catch2);

            return ret;

        }
        private static CodeTryCatchFinallyStatement CreateEntityFormCall(bool isNew, string className)
        {
            CodeTryCatchFinallyStatement ret = TryCatchMessageBox();

            if (isNew)
            {
                ret.TryStatements.Add(new CodeSnippetExpression(string.Format("{0}Form frm = new {0}Form()", className)));
                ret.TryStatements.Add(new CodeSnippetExpression(string.Format("{0}Dat dat = new {0}Dat()", className)));
                ret.TryStatements.Add(new CodeSnippetExpression("frm.OldValue = dat"));
                ret.TryStatements.Add(new CodeSnippetExpression("if (frm.ShowDialog() == DialogResult.OK) Refresh()"));
            }
            else
            {
                ret.TryStatements.Add(new CodeSnippetExpression(string.Format("{0}Dat dat = e.DatEntity as {0}Dat", className)));
                ret.TryStatements.Add(new CodeSnippetExpression("if (dat == null) return"));
                ret.TryStatements.Add(new CodeSnippetExpression(string.Format("{0}Form frm = new {0}Form()", className)));
                ret.TryStatements.Add(new CodeSnippetExpression("frm.OldValue = dat"));
                ret.TryStatements.Add(new CodeSnippetExpression("if (frm.ShowDialog() == DialogResult.OK) Refresh()"));
            }

            return ret;
        }
        public static CodeTypeDeclaration CreateController(Table table, DatClassGenerator.DatClass dat)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(table.Name) + "Controller";
            ret.IsPartial = true;

            ret.Comments.Add(new CodeCommentStatement("<summary>" + "Контроллер для " + Helper.GetMSDescription(table) + "</summary>", true));

            if (dat.BaseTypes.Contains("ITreeDat"))
                ret.BaseTypes.Add(new CodeTypeReference(string.Format("TreeListFormController<{0}Set, {0}Dat, {1}Set, {1}Dat>", Helper.Capitalise(table.Name), Helper.Capitalise(table.Name).Replace("Tree", ""))));
            else
                ret.BaseTypes.Add(new CodeTypeReference(string.Format("ListFormController<{0}Set, {0}Dat, {0}FilterForm, {0}Filter>", Helper.Capitalise(table.Name))));


            CodeConstructor constr1 = new CodeConstructor();
            constr1.Attributes = MemberAttributes.Public;
            constr1.Parameters.Add(new CodeParameterDeclarationExpression("Form", "mdiParent"));
            constr1.ChainedConstructorArgs.Add(new CodeSnippetExpression("mdiParent, null"));

            ret.Members.Add(constr1);

            CodeConstructor constr2 = new CodeConstructor();
            constr2.Attributes = MemberAttributes.Public;
            constr2.Parameters.Add(new CodeParameterDeclarationExpression("Form", "mdiParent"));
            constr2.Parameters.Add(new CodeParameterDeclarationExpression("BaseLoadFilter", "loadfilter"));

            if (dat.BaseTypes.Contains("ITreeDat"))
                constr2.BaseConstructorArgs.Add(new CodeSnippetExpression(string.Format("mdiParent, \"{0}\", \"{1}\", BOF.Icons.Web_XML, new {2}Form(), new {0}Form()", Helper.Capitalise(table.Name), Helper.GetMSDescription(table), Helper.Capitalise(table.Name).Replace("Tree", ""))));
            else
                constr2.BaseConstructorArgs.Add(new CodeSnippetExpression(string.Format("mdiParent, \"{0}\", \"{1}\", BOF.Icons.Web_XML, true, false, loadfilter, true", Helper.Capitalise(table.Name), Helper.GetMSDescription(table))));

            //constr.ChainedConstructorArgs.Add();
            ret.Members.Add(constr2);

            if (!dat.BaseTypes.Contains("ITreeDat"))
            {
                CodeMemberMethod AddGridEvents = new CodeMemberMethod();
                AddGridEvents.Name = "AddGridEvents";
                AddGridEvents.Attributes = MemberAttributes.Family | MemberAttributes.Override;
                AddGridEvents.Statements.Add(new CodeSnippetExpression("gridMain.EntitySelected += new DatEventDelegate(Grid_ValueSelected)"));
                AddGridEvents.Statements.Add(new CodeSnippetExpression("gridMain.EntityNew += new DatEventDelegate(Grid_EntityNew)"));
                ret.Members.Add(AddGridEvents);

                CodeMemberMethod Grid_EntityNew = new CodeMemberMethod();
                Grid_EntityNew.Name = "Grid_EntityNew";
                Grid_EntityNew.Attributes = MemberAttributes.Private;
                Grid_EntityNew.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "sender"));
                Grid_EntityNew.Parameters.Add(new CodeParameterDeclarationExpression("DatEventArgs", "e"));
                Grid_EntityNew.Statements.Add(CreateEntityFormCall(true, Helper.Capitalise(table.Name)));
                ret.Members.Add(Grid_EntityNew);


                CodeMemberMethod Grid_ValueSelected = new CodeMemberMethod();
                Grid_ValueSelected.Name = "Grid_ValueSelected";
                Grid_ValueSelected.Attributes = MemberAttributes.Private;
                Grid_ValueSelected.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "sender"));
                Grid_ValueSelected.Parameters.Add(new CodeParameterDeclarationExpression("DatEventArgs", "e"));
                Grid_ValueSelected.Statements.Add(CreateEntityFormCall(false, Helper.Capitalise(table.Name)));
                ret.Members.Add(Grid_ValueSelected);

                CodeMemberMethod DoGroup = new CodeMemberMethod();
                DoGroup.Name = "DoGroup";
                DoGroup.Attributes = MemberAttributes.Family | MemberAttributes.Override;
                DoGroup.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "open"));

                DoGroup.Statements.Add(new CodeSnippetExpression("base.DoGroup(open)"));
                DoGroup.Statements.Add(new CodeSnippetExpression("if (open) {"));

                for (int i = 0; i < dat.Members.Count; i++)
                {
                    DatClassGenerator.DatMember m = dat.Members[i];
                    if (m.Name != "ID")
                    {
                        switch (m.DataType)
                        {
                            case "Boolean":
                            case "Decimal":
                                break;
                            case "Int32":
                                DoGroup.Statements.Add(new CodeSnippetExpression(string.Format("treeGroup.AddGroup<{0}Dat, {0}Set>({0}Columns.{1}, \"{2}\")", dat.Name, dat.Members[i].Name, dat.Members[i].Description.Replace("\"", "\\\""))));
                                break;
                            case "DateTime":
                                DoGroup.Statements.Add(new CodeSnippetExpression(string.Format("treeGroup.AddGroupDate<{0}Dat, {0}Set>({0}Columns.{1}, \"{2}\")", dat.Name, dat.Members[i].Name, dat.Members[i].Description.Replace("\"", "\\\""))));
                                break;
                            default:
                                DoGroup.Statements.Add(new CodeSnippetExpression(string.Format("treeGroup.AddGroup<{0}Dat, {0}Set>({0}Columns.{1}, \"{2}\")", dat.Name, dat.Members[i].Name, dat.Members[i].Description.Replace("\"", "\\\""))));
                                break;
                        }
                    }
                }

                DoGroup.Statements.Add(new CodeSnippetExpression("} else SetList.FilterReset()"));


                ret.Members.Add(DoGroup);
            }

            CodeMemberMethod AddGridColumns = new CodeMemberMethod();
            AddGridColumns.Name = "AddGridColumns";
            AddGridColumns.Attributes = MemberAttributes.Family | MemberAttributes.Override;

            AddGridColumns.Statements.Add(new CodeSnippetExpression("gridMain.Grid.AutoGenerateColumns = false"));
            for (int i = 0; i < dat.Members.Count; i++)
            {
                DatClassGenerator.DatMember m = dat.Members[i];
                int width;

                if (m.DataType == "Boolean")
                    AddGridColumns.Statements.Add(new CodeSnippetExpression(string.Format("gridMain.Grid.AddGridCheckColumn({0}Columns.{1}, \"{2}\")", dat.Name, m.Name, m.Description.Replace("\"", "\\\""))));
                else
                {
                    switch (m.DataType)
                    {
                        case "DateTime":
                            if (m.Name == "SysDate")
                                width = 110;
                            else
                                width = 80;
                            break;
                        case "Int32":
                            if (m.Name == "ID")
                                width = 20;
                            else
                                width = 80;
                            break;
                        case "Decimal":
                            width = 100;
                            break;
                        default:
                            width = 200;
                            break;
                    }



                    AddGridColumns.Statements.Add(new CodeSnippetExpression(string.Format("gridMain.Grid.AddGridColumn({0}Columns.{1}, \"{2}\", typeof({3}), {4})", dat.Name, m.Name, m.Description.Replace("\"", "\\\""), m.DataType, width)));
                }


            }
            ret.Members.Add(AddGridColumns);


            return ret;
        }

        public static CodeTypeDeclaration CreateEntityForm(CodeTypeDeclaration tp, Table table, DatGenerator.DatClassGenerator.DatClass dat)
        {
            CodeTypeDeclaration ret = new CodeTypeDeclaration();
            ret.Name = Helper.Capitalise(table.Name) + "Form";
            ret.IsPartial = true;

            if (dat.BaseTypes.Contains("ICardDat"))
                ret.BaseTypes.Add(new CodeTypeReference("CardFlowForm"));
            else if (dat.BaseTypes.Contains("ITreeDat"))
                ret.BaseTypes.Add(new CodeTypeReference("CardTreeFlowForm"));
            else
                ret.BaseTypes.Add(new CodeTypeReference("OKCancelDatFlowForm"));

            ret.Comments.Add(new CodeCommentStatement("<summary>" + "Форма для " + Helper.GetMSDescription(table) + "</summary>", true));

            CodeMemberMethod Init = new CodeMemberMethod();
            Init.Name = "Init";
            Init.Attributes = MemberAttributes.Family | MemberAttributes.Override;

            Init.Statements.Add(new CodeSnippetExpression("base.Init()"));
            foreach (CodeTypeMember item in tp.Members)
            {

                if (item is CodeMemberProperty && item.Name != "SysUser" && item.Name != "SysDate")
                {

                    CodeMemberProperty pr = (CodeMemberProperty)item;
                    CodeMemberField field = new CodeMemberField();
                    field.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                    field.Name = "ctl_" + pr.Name;

                    switch (pr.Type.BaseType)
                    {
                        case "IDataAccess":
                        case "PathTree":
                        case "PathCard":
                            break;

                        case "SysUser":
                        case "SysDate":


                        case "Int32":
                            if ((dat.BaseTypes.Contains("ICardDat") || dat.BaseTypes.Contains("ITreeDat")) && pr.Name == "Code")
                                break;

                            if (pr.Name == "ID")
                                break;

                            field.Type = new CodeTypeReference("CtlInt");
                            field.InitExpression = new CodeSnippetExpression("new CtlInt()");
                            Init.Statements.Add(new CodeSnippetExpression(string.Format("FormHelper.AddCtlInt(ctl_{0}, pnlFlow, \"{1}\", {2}Columns.{0})", pr.Name, pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""), tp.UserData["TableName"])));

                            break;

                        case "Decimal":
                            field.Type = new CodeTypeReference("CtlDecimal");
                            field.InitExpression = new CodeSnippetExpression("new CtlDecimal()");
                            Init.Statements.Add(new CodeSnippetExpression(string.Format("FormHelper.AddCtlDecimal(ctl_{0}, pnlFlow, \"{1}\", {2}Columns.{0})", pr.Name, pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""), tp.UserData["TableName"])));

                            break;

                        case "DateTime":
                            field.Type = new CodeTypeReference("CtlDateTime");
                            field.InitExpression = new CodeSnippetExpression("new CtlDateTime()");
                            Init.Statements.Add(new CodeSnippetExpression(string.Format("FormHelper.AddCtlDateTime(ctl_{0}, pnlFlow, \"{1}\", {2}Columns.{0})", pr.Name, pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""), tp.UserData["TableName"])));

                            break;

                        case "Boolean":
                            field.Type = new CodeTypeReference("CtlBool");
                            field.InitExpression = new CodeSnippetExpression("new CtlBool()");
                            Init.Statements.Add(new CodeSnippetExpression(string.Format("FormHelper.AddCtlBool(ctl_{0}, pnlFlow, \"{1}\", {2}Columns.{0})", pr.Name, pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""), tp.UserData["TableName"])));

                            break;

                        case "String":
                        case "Guid":
                            if ((dat.BaseTypes.Contains("ICardDat") || dat.BaseTypes.Contains("ITreeDat")) && pr.Name == "Name")
                                break;

                            field.Type = new CodeTypeReference("CtlString");
                            field.InitExpression = new CodeSnippetExpression("new CtlString()");
                            Init.Statements.Add(new CodeSnippetExpression(string.Format("FormHelper.AddCtlString(ctl_{0}, pnlFlow, \"{1}\", {2}Columns.{0})", pr.Name, pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""), tp.UserData["TableName"])));

                            break;

                        default:
                            if (pr.Type.BaseType.EndsWith("Dat"))
                            {
                                string SetClass = pr.Type.BaseType.Remove(pr.Type.BaseType.LastIndexOf("Dat")) + "Set";
                                field.Type = new CodeTypeReference("CtlChooser");
                                field.InitExpression = new CodeSnippetExpression("new CtlChooser()");
                                Init.Statements.Add(new CodeSnippetExpression(string.Format("ctl_{0}.SetClass = new {1}()", pr.Name, SetClass)));
                                Init.Statements.Add(new CodeSnippetExpression(string.Format("FormHelper.AddCtlChooser(ctl_{0}, pnlFlow, \"{1}\", {2}Columns.{0})", pr.Name, pr.UserData["MSDescription"].ToString().Replace("\"", "\\\""), tp.UserData["TableName"])));
                            }
                            else
                            {
                                field.Type = new CodeTypeReference(typeof(object));
                                field.InitExpression = new CodeSnippetExpression("new object()");
                                Init.Statements.Add(new CodeSnippetExpression(string.Format("throw new Exception(\"no control for field {0} type {1}\")", pr.Name, pr.Type.BaseType)));
                            }
                            break;

                    }

                    if (field.Type.BaseType != "System.Void")
                        ret.Members.Add(field);

                }
            }

            Init.Statements.Add(new CodeSnippetExpression("FormHelper.SetFormHeight(pnlFlow, pnlFlowTop)"));
            Init.Statements.Add(new CodeSnippetExpression("if (NewValue != null) BindControls(this)"));
            ret.Members.Add(Init);




            return ret;
        }

    }
}
