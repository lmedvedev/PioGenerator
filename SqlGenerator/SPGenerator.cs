using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace Generator
{
    public class SQLTextGenerator
    {
        public enum ActionTypes
        {
            UPDATE = 0,
            INSERT = 1,
            DELETE = -1
        }

        public enum Template
        {
            DICTIONARY,
            TREE_CARDS,
            HEADER_DETAILS
        }

        private Table _pTable;
        public SQLTextGenerator(Table ProcessedTable)
        {
            _pTable = ProcessedTable;
        }

        public UserDefinedFunction testF()
        {

            UserDefinedFunction udf = new UserDefinedFunction(_pTable.Parent, "function1", "dbo");
            udf.TextMode = false;//by default this is true if it is true you can directly provide text header and body
            udf.FunctionType = UserDefinedFunctionType.Table;
            udf.TableVariableName = "@t";
            udf.TextBody = "begin \n INSERT @t \n Select @opened, @high, @low, @closed, @volume, @dateTimed \n return \n end";

            UserDefinedFunctionParameter par1 = new UserDefinedFunctionParameter(udf, "@opened");

            par1.DataType = DataType.Float;

            par1.DefaultValue = "0.0";

            UserDefinedFunctionParameter par2 = new UserDefinedFunctionParameter(udf, "@high");

            par2.DataType = DataType.Float;

            par2.DefaultValue = "0.0";

            UserDefinedFunctionParameter par3 = new UserDefinedFunctionParameter(udf, "@low");

            par3.DataType = DataType.Float;

            par3.DefaultValue = "0.0";

            UserDefinedFunctionParameter par4 = new UserDefinedFunctionParameter(udf, "@closed");

            par4.DataType = DataType.Float;

            par4.DefaultValue = "0.0";

            UserDefinedFunctionParameter par5 = new UserDefinedFunctionParameter(udf, "@volume");

            par5.DataType = DataType.BigInt;

            par5.DefaultValue = "0";


            UserDefinedFunctionParameter par6 = new UserDefinedFunctionParameter(udf, "@datetimed");

            par6.DataType = DataType.DateTime;

            par6.DefaultValue = "\"" + DateTime.MinValue.ToString() + "\"";

            udf.Parameters.Add(par1);
            udf.Parameters.Add(par2);
            udf.Parameters.Add(par3);
            udf.Parameters.Add(par4);
            udf.Parameters.Add(par5);
            udf.Parameters.Add(par6);

            udf.Columns.Add(new Column(udf, "opened", DataType.Float));
            udf.Columns.Add(new Column(udf, "high", DataType.Float));
            udf.Columns.Add(new Column(udf, "low", DataType.Float));
            udf.Columns.Add(new Column(udf, "closed", DataType.Float));
            udf.Columns.Add(new Column(udf, "volume", DataType.BigInt));
            udf.Columns.Add(new Column(udf, "datetimed", DataType.DateTime));

            return udf;
            //udf.Create();
        }

        public UserDefinedFunction GenerateAsOfFunction()
        {
            bool ContainsValueDate = false;
            UserDefinedFunction udf = null;
            foreach (Column col in _pTable.Columns)
                if (col.Name == "ValueDate")
                    ContainsValueDate = true;

            if (ContainsValueDate)
            {
                udf = new UserDefinedFunction(_pTable.Parent, string.Format("fn_AsOf_{0}", _pTable.Name), _pTable.Schema);
                udf.TextMode = false;
                udf.FunctionType = UserDefinedFunctionType.Inline;


                List<string> selectColumns = new List<string>();
                List<string> groupColumns = new List<string>();
                List<string> joinColumns = new List<string>();

                foreach (Column col in _pTable.Columns)
                {
                    if (col.Name.Contains("Value") || col.InPrimaryKey || col.Name == "SysDate" || col.Name == "SysUser")
                        continue;

                    if (col.Name == "ValueDate")
                    {
                        selectColumns.Add(string.Format("max({0}) as {0}", col.Name));
                        joinColumns.Add(string.Format("{0}.{1} = a.{1}", _pTable.Name, col.Name));
                    }
                    else
                    {
                        if (!col.Name.Contains("Value"))
                        {
                            selectColumns.Add(col.Name);
                            groupColumns.Add(col.Name);

                            if (col.Nullable )
                                // тут надо проверять тип колонки для правильного isnull
                                joinColumns.Add(string.Format("isnull({0}.{1},-1) = isnull(a.{1},-1)", _pTable.Name, col.Name));
                            else
                                joinColumns.Add(string.Format("{0}.{1} = a.{1}", _pTable.Name, col.Name));
                        }
                    }

                }

                StringBuilder fBody = new StringBuilder();

                fBody.AppendLine("RETURN");

                fBody.AppendFormat("--Created by SPGenerator {0}, {1}{2}", DateTime.Now, System.Security.Principal.WindowsIdentity.GetCurrent().Name, Environment.NewLine);
                fBody.AppendFormat("--Для защиты функции от переписывания генератором напишите \"--protect--\" без кавычек, с новой строки, в любом месте после RETURN{0}", Environment.NewLine);

                fBody.AppendFormat("(\r\nselect {0}.id from {0}\r\ninner join (select \r\n{1}\r\nfrom {0} where ValueDate <= @AsOfDate\r\ngroup by\r\n{2}) a on {3})"
                    , _pTable.Name
                    , string.Join("\r\n,", selectColumns.ToArray())
                    , string.Join("\r\n,", groupColumns.ToArray())
                    , string.Join("\r\nand ", joinColumns.ToArray())
                    );

                udf.TextBody = fBody.ToString();

                udf.Columns.Add(new Column(udf, "id", DataType.Int));


                UserDefinedFunctionParameter p = new UserDefinedFunctionParameter();
                p.Parent = udf;
                p.Name = "@AsOfDate";
                p.DataType = DataType.DateTime;
                udf.Parameters.Add(p);

            }

            return udf;
        }

        public Trigger GenerateTrigger(ActionTypes trigType)
        {
            Trigger tr = new Trigger(_pTable, "tr_" + _pTable.Name + "_" + trigType.ToString());
            tr.TextMode = false;
            switch (trigType)
            {
                case ActionTypes.INSERT:
                    tr.Insert = true;
                    break;
                case ActionTypes.UPDATE:
                    tr.Update = true;
                    break;
                case ActionTypes.DELETE:
                    tr.Delete = true;
                    break;
            }
            tr.TextBody = TriggerTextBodyOld1(trigType);
            //tr.Parent = _pTable;
            //tr.IsEnabled = false;
            tr.IsEnabled = true;
            return tr;
        }
        public StoredProcedure GenerateProcedure(ActionTypes procType)
        {
            StoredProcedure spG = new StoredProcedure(_pTable.Parent, "sp_" + _pTable.Name + "_" + procType.ToString()); // + "_gen");
            spG.TextMode = false;
            spG.Schema = _pTable.Schema;
            switch (procType)
            {
                case ActionTypes.INSERT:
                case ActionTypes.UPDATE:
                    foreach (Column col in _pTable.Columns)
                    {
                        StoredProcedureParameter p = new StoredProcedureParameter();
                        p.Parent = spG;
                        p.Name = "@" + col.Name;
                        if (col.DataType.SqlDataType == SqlDataType.VarCharMax)
                        {
                            p.DataType = DataType.VarChar(8000);
                        }
                        else
                            p.DataType = col.DataType;
                        p.IsOutputParameter = false;
                        p.DefaultValue = "null";
                        spG.Parameters.Add(p);
                    }
                    break;
                case ActionTypes.DELETE:
                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.InPrimaryKey)
                        {
                            StoredProcedureParameter p = new StoredProcedureParameter();
                            p.Parent = spG;
                            p.Name = "@" + col.Name;
                            p.DataType = col.DataType;
                            p.IsOutputParameter = false;
                            p.DefaultValue = "null";
                            spG.Parameters.Add(p);
                        }
                    }
                    break;
            }
            spG.TextBody = ProcedureTextBody(procType);
            return spG;
        }
        private string ProcessExclusions()
        {
            StringBuilder sBody = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;

            if (_pTable.Columns.Contains("sysuser"))
                sBody.AppendFormat("set @{0} = suser_sname(){1}", "sysuser", Environment.NewLine);
            if (_pTable.Columns.Contains("sysdate"))
                sBody.AppendFormat("set @{0} = GetDate(){1}", "sysdate", Environment.NewLine);

            foreach (ForeignKey fk in _pTable.ForeignKeys)
            {
                if (fk.ReferencedTable == "RegDocs" && fk.Columns.Contains("regdoc_id") && (_pTable.Name.ToLower() != "transactions") && (_pTable.Columns.Contains("docNum") || _pTable.Columns.Contains("docDate") || _pTable.Columns.Contains("client_id")))
                {
                    Table RegDocTable = _pTable.Parent.Tables["RegDocs"];

                    sBody.AppendFormat("select{0}", Environment.NewLine);

                    if (_pTable.Columns.Contains("docNum") && RegDocTable.Columns.Contains("docNum"))
                        sBody.AppendFormat("@docNum = docNum,{0}", Environment.NewLine);

                    if (_pTable.Columns.Contains("docDate") && RegDocTable.Columns.Contains("docDate"))
                        sBody.AppendFormat("@docDate = docDate,{0}", Environment.NewLine);

                    if (_pTable.Columns.Contains("Client_id") && RegDocTable.Columns.Contains("Client_id"))
                        sBody.AppendFormat("@Client_id = Client_id,{0}", Environment.NewLine);

                    sBody.Remove(sBody.Length - tail, tail);
                    sBody.Append(Environment.NewLine);

                    sBody.AppendFormat("from RegDocs where id = @RegDoc_id{0}", Environment.NewLine);
                    break;
                }
            }
            return sBody.ToString();

        }
        public string InsertStatement()
        {
            StringBuilder sBody = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;

            sBody.Append(ProcessExclusions());

            sBody.AppendFormat("INSERT INTO {0} (", _pTable.ToString());
            sBody.Append(Environment.NewLine);

            foreach (Column col in _pTable.Columns)
            {
                if (!col.Identity)
                {
                    sBody.AppendFormat("    [{0}],", col.Name);
                    sBody.Append(Environment.NewLine);
                }
            }
            sBody.Remove(sBody.Length - tail, tail);
            sBody.Append(")");
            sBody.Append(Environment.NewLine);
            sBody.Append("VALUES (");
            sBody.Append(Environment.NewLine);

            foreach (Column col in _pTable.Columns)
            {
                if (!col.Identity)
                {
                    sBody.AppendFormat("    @{0},", col.Name);
                    sBody.Append(Environment.NewLine);
                }
            }
            sBody.Remove(sBody.Length - tail, tail);
            sBody.Append(")");
            sBody.Append(Environment.NewLine);
            return sBody.ToString();
        }
        public string UpdateStatement()
        {
            StringBuilder sBody = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;
            sBody.Append(ProcessExclusions());

            sBody.AppendFormat("UPDATE {0}", _pTable.ToString());
            sBody.Append(Environment.NewLine);
            sBody.Append("SET");
            sBody.Append(Environment.NewLine);
            foreach (Column col in _pTable.Columns)
            {
                if (!col.Identity)
                {
                    sBody.AppendFormat("    [{0}] = @{0},", col.Name);
                    sBody.Append(Environment.NewLine);
                }
            }
            sBody.Remove(sBody.Length - tail, tail);
            return sBody.ToString();
        }

        public string ProcedureTextBody(ActionTypes procType)
        {
            StringBuilder sBody = new StringBuilder();
            StringBuilder sBody1 = new StringBuilder();
            StringBuilder whereClause = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;

            sBody.AppendFormat("begin{0}", Environment.NewLine);
            sBody.AppendFormat("--Created by SPGenerator {0}, {1}{2}", DateTime.Now, System.Security.Principal.WindowsIdentity.GetCurrent().Name, Environment.NewLine);
            sBody.AppendFormat("--Для защиты процедуры от переписывания генератором напишите \"--protect--\" без кавычек, с новой строки, в любом месте между begin & end{0}", Environment.NewLine);
            sBody.AppendFormat("SET NOCOUNT ON{0}", Environment.NewLine);

            Column idCol = null;
            List<Column> prKey = new List<Column>();
            foreach (Column col in _pTable.Columns)
            {

                //if (col.Identity)
                //{
                //    //whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                //    idCol = col;
                //}

                if (col.InPrimaryKey)
                {
                    whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                    prKey.Add(col);

                    if (idCol == null)
                        idCol = col;
                    else if (!idCol.Identity && col.Identity)
                        idCol = col;
                    //idCol = col;

                }

            }


            switch (procType)
            {
                case ActionTypes.INSERT:

                    sBody.AppendLine(InsertStatement());

                    sBody.AppendFormat("IF @@ERROR <> 0{0}", Environment.NewLine);
                    sBody.AppendFormat("\tBEGIN{0} ", Environment.NewLine);
                    sBody.AppendFormat("\t\tRAISERROR('Error inserting new row in table {0}', 16, 1){1}", _pTable.Name, Environment.NewLine);
                    sBody.AppendFormat("\t\tRETURN -1 {0}", Environment.NewLine);
                    sBody.AppendFormat("\tEND{0}", Environment.NewLine);

                    if (idCol != null)
                        switch (idCol.DataType.SqlDataType)
                        {
                            case SqlDataType.BigInt:
                            case SqlDataType.Int:
                            case SqlDataType.SmallInt:
                            case SqlDataType.TinyInt:
                                if (idCol.Identity)
                                    sBody.AppendFormat("return IDENT_CURRENT('{0}')", _pTable.Name);
                                else
                                    sBody.Append("return 0");
                                //sBody.AppendFormat("return @{0}", idCol.Name);
                                break;

                            case SqlDataType.UniqueIdentifier:
                                sBody.Append("return 0");
                                //sBody.AppendFormat("return @{0}", idCol.Name);
                                break;

                            case SqlDataType.Binary:
                            case SqlDataType.Bit:
                            case SqlDataType.Char:
                            case SqlDataType.Date:
                            case SqlDataType.DateTime:
                            case SqlDataType.DateTime2:
                            case SqlDataType.DateTimeOffset:
                            case SqlDataType.Decimal:
                            case SqlDataType.Float:
                            case SqlDataType.Geography:
                            case SqlDataType.Geometry:
                            case SqlDataType.HierarchyId:
                            case SqlDataType.Image:
                            case SqlDataType.Money:
                            case SqlDataType.NChar:
                            case SqlDataType.NText:
                            case SqlDataType.NVarChar:
                            case SqlDataType.NVarCharMax:
                            case SqlDataType.None:
                            case SqlDataType.Numeric:
                            case SqlDataType.Real:
                            case SqlDataType.SmallDateTime:
                            case SqlDataType.SmallMoney:
                            case SqlDataType.SysName:
                            case SqlDataType.Text:
                            case SqlDataType.Time:
                            case SqlDataType.Timestamp:
                            case SqlDataType.UserDefinedDataType:
                            case SqlDataType.UserDefinedTableType:
                            case SqlDataType.UserDefinedType:
                            case SqlDataType.VarBinary:
                            case SqlDataType.VarBinaryMax:
                            case SqlDataType.VarChar:
                            case SqlDataType.VarCharMax:
                            case SqlDataType.Variant:
                            case SqlDataType.Xml:
                                sBody.Append("return 0");
                                break;
                            default:
                                sBody.Append("return 0");
                                break;
                        }
                    else
                        sBody.Append("return 0");

                    break;

                case ActionTypes.UPDATE:
                    //sBody.Append(ProcessExclusions());

                    //if (idCol == null || !idCol.Identity)
                    //{
                    //    sBody.AppendFormat("if exists (select {0} from {1} where {2}) begin\r\n", _pTable.Columns[0].Name, _pTable.Name, whereClause.ToString().Replace(")(", ") and ("));
                    //    sBody.AppendLine();
                    //}

                    sBody.Append(UpdateStatement());

                    if (whereClause.ToString().Length > 0)
                    {
                        sBody.Append(Environment.NewLine);
                        sBody.AppendFormat("where {0}", whereClause.ToString().Replace(")(", ") and ("));
                        sBody.Append(Environment.NewLine);
                    }
                    else
                    {
                        throw new Exception(string.Format("Не задано условие where! Действие: {0}, таблица: {1}", procType, _pTable.ToString()));
                    }

                    //if (idCol == null || !idCol.Identity)
                    //{

                    //    sBody.AppendLine("end");
                    //    sBody.AppendLine("else begin");
                    //    sBody.AppendLine(InsertStatement());
                    //    sBody.AppendLine("end");
                    //}

                    sBody.Append("if @@rowcount = 1");
                    sBody.Append(Environment.NewLine);
                    if (idCol != null)
                    {
                        switch (idCol.DataType.SqlDataType)
                        {
                            case SqlDataType.BigInt:
                            case SqlDataType.Int:
                            case SqlDataType.SmallInt:
                            case SqlDataType.TinyInt:
                                sBody.AppendFormat("\treturn @{0}\r\n", idCol.Name);
                                break;
                            default:
                                sBody.AppendLine("\treturn 0");
                                break;
                        }
                        sBody.AppendLine("else");
                        sBody.AppendLine("\tbegin");
                        sBody.AppendLine("\tdeclare @s as nvarchar(max)");
                        sBody.AppendFormat("\tset @s = convert(nvarchar(max),@{0})\r\n", idCol.Name);
                        sBody.AppendFormat("\tRAISERROR('Неудачная попытка сохранить запись с {0}=%s в таблице {1}, {2}', 16, 1, @s)", idCol.Name, _pTable.Name, whereClause.ToString());
                        sBody.AppendLine("end");
                    }
                    else
                        sBody.AppendFormat("    return 0 else RAISERROR('Неудачная попытка сохранить запись в таблице {0}, {1}', 16, 1)", _pTable.Name, whereClause.ToString());
                    sBody.Append(Environment.NewLine);
                    break;

                case ActionTypes.DELETE:
                    sBody.AppendFormat("DELETE {0}", _pTable.ToString());
                    sBody.Append(Environment.NewLine);
                    //foreach (Column col in _pTable.Columns)
                    //{
                    //    //if (col.Identity)
                    //    //{
                    //    //    whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                    //    //}
                    //    //else 
                    //    if (col.InPrimaryKey)
                    //    {
                    //        whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                    //    }
                    //}
                    if (whereClause.ToString().Length > 0)
                    {
                        sBody.Append(Environment.NewLine);
                        sBody.AppendFormat("where {0}", whereClause.ToString().Replace(")(", ") and ("));
                        sBody.Append(Environment.NewLine);
                    }
                    else
                    {
                        throw new Exception(string.Format("Не задано условие where! Действие: {0}, таблица: {1}", procType, _pTable.ToString()));
                    }
                    break;
            }

            sBody.Append(Environment.NewLine);
            sBody.Append("end");
            sBody.Append(Environment.NewLine);

            return sBody.ToString();
        }
        public string TriggerTextBody(ActionTypes trigType)
        {
            StringBuilder sBody = new StringBuilder();

            sBody.AppendFormat("begin{0}", Environment.NewLine);
            sBody.AppendFormat("SET NOCOUNT ON{0}", Environment.NewLine);

            sBody.AppendFormat("declare @ins as xml{0}", Environment.NewLine);
            sBody.AppendFormat("declare @del as xml{0}", Environment.NewLine);
            sBody.AppendFormat("declare @all as xml{0}", Environment.NewLine);

            string ColumnsList = "";
            bool UseAsterick = true;
            foreach (Column col in _pTable.Columns)
            {
                if (col.DataType.Name != DataType.Image.Name && col.DataType.Name != DataType.Text.Name && col.DataType.Name != DataType.NText.Name)
                {
                    ColumnsList += ", " + col.Name;
                }
                else
                {
                    UseAsterick = false;
                }
            }

            if (UseAsterick)
                ColumnsList = ", *";

            sBody.AppendFormat("set @ins = (select 'insert' as act{1} from inserted FOR XML raw ('row')){0}", Environment.NewLine, ColumnsList);
            sBody.AppendFormat("set @del = (select 'delete' as act{1} from deleted FOR XML raw ('row')){0}", Environment.NewLine, ColumnsList);
            sBody.AppendFormat("set @all = (select @del, @ins for XML raw ('{1}')){0}", Environment.NewLine, _pTable.Name);
            sBody.AppendFormat("exec dbo.sp_SysAudit_INSERT '{0}', @all, {1}{2}", _pTable.Name, (int)trigType, Environment.NewLine);
            sBody.AppendFormat("end{0}", Environment.NewLine);

            return sBody.ToString();
        }
        public string TriggerTextBodyOld1(ActionTypes trigType)
        {
            StringBuilder sBody = new StringBuilder();
            StringBuilder whereClause = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;

            sBody.Append("begin");
            sBody.Append(Environment.NewLine);
            sBody.Append("SET NOCOUNT ON");
            sBody.Append(Environment.NewLine);

            sBody.AppendFormat("INSERT SysAudit (Srv, Db, Tbl, Operation, ClientHost, ClientApplication, dt, usr, OpContent) select{0}", Environment.NewLine);
            sBody.AppendFormat("convert(varchar(max), SERVERPROPERTY('MachineName')), DB_NAME(), '{0}', {1},  host_name(), app_name(), GetDate(), suser_sname(), ", _pTable.Name, (int)trigType);
            sBody.Append(Environment.NewLine);
            sBody.AppendFormat("'<t:Audit xmlns:t=\"http://BO/SysAudit.xsd\" OperType=\"{0}\" id=\"' + convert(varchar(max), i.id) + '\" table=\"{1}\">' +", trigType, _pTable.Name);
            sBody.Append(Environment.NewLine);

            switch (trigType)
            {
                case ActionTypes.INSERT:

                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.DataType.Name != DataType.Image.Name && col.DataType.Name != DataType.Text.Name && col.DataType.Name != DataType.NText.Name)
                        {
                            if (col.DataType.Name == DataType.DateTime.Name || col.DataType.Name == DataType.SmallDateTime.Name)
                                sBody.AppendFormat("+ dbo.fn_Compare('{0}', convert(varchar(MAX), i.{0}, 121), null)", col.Name);
                            else if (col.DataType.SqlDataType == SqlDataType.Xml)
                                sBody.AppendFormat("+ dbo.fn_CompareXML('{0}', convert(varchar(MAX), i.{0}), null)", col.Name);
                            else
                                sBody.AppendFormat("+ dbo.fn_Compare('{0}', convert(varchar(MAX), i.{0}), null)", col.Name);
                            sBody.Append(Environment.NewLine);
                        }
                    }
                    sBody.Append("+'</t:Audit>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from inserted i");
                    break;

                case ActionTypes.UPDATE:
                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.DataType.Name != DataType.Image.Name && col.DataType.Name != DataType.Text.Name && col.DataType.Name != DataType.NText.Name)
                        {
                            if (col.DataType.Name == DataType.DateTime.Name || col.DataType.Name == DataType.SmallDateTime.Name)
                                sBody.AppendFormat("+ dbo.fn_Compare('{0}', convert(varchar(MAX), i.{0}, 121), convert(varchar(MAX), d.{0}, 121))", col.Name);
                            else if (col.DataType.SqlDataType == SqlDataType.Xml)
                                sBody.AppendFormat("+ dbo.fn_CompareXML('{0}', convert(varchar(MAX), i.{0}), convert(varchar(MAX), d.{0}))", col.Name);
                            else
                                sBody.AppendFormat("+ dbo.fn_Compare('{0}', convert(varchar(MAX), i.{0}), convert(varchar(MAX), d.{0}))", col.Name);
                            sBody.Append(Environment.NewLine);
                        }
                    }
                    sBody.Append("+'</t:Audit>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from inserted i inner join deleted d on i.id = d.id");
                    break;

                case ActionTypes.DELETE:
                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.DataType.Name != DataType.Image.Name && col.DataType.Name != DataType.Text.Name && col.DataType.Name != DataType.NText.Name)
                        {
                            if (col.DataType.Name == DataType.DateTime.Name || col.DataType.Name == DataType.SmallDateTime.Name)
                                sBody.AppendFormat("+ dbo.fn_Compare('{0}', null, convert(varchar(MAX), i.{0}, 121))", col.Name);
                            else if (col.DataType.SqlDataType == SqlDataType.Xml)
                                sBody.AppendFormat("+ dbo.fn_CompareXML('{0}', null, convert(varchar(MAX), i.{0}))", col.Name);
                            else
                                sBody.AppendFormat("+ dbo.fn_Compare('{0}', null, convert(varchar(MAX), i.{0}))", col.Name);
                            sBody.Append(Environment.NewLine);
                        }
                    }
                    sBody.Append("+'</t:Audit>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from deleted i");
                    break;
            }

            sBody.Append(Environment.NewLine);
            sBody.Append("end");
            sBody.Append(Environment.NewLine);

            return sBody.ToString();
        }
        public string TriggerTextBodyOld(ActionTypes trigType)
        {
            StringBuilder sBody = new StringBuilder();
            StringBuilder whereClause = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;

            sBody.Append("begin");
            sBody.Append(Environment.NewLine);
            sBody.Append("SET NOCOUNT ON");
            sBody.Append(Environment.NewLine);

            sBody.AppendFormat("INSERT INTO SysAudit (Key_ServerName, Key_DataBaseName, Key_TableName, Key_Id, Operation, HostName, ApplicationName, dt, usr, OpContent){0}", Environment.NewLine);
            //sBody.AppendFormat("INSERT INTO SysAudit (ServerName, DataBaseName, TableName, Id, Operation, HostName, ApplicationName, dt, usr, OpContent){0}", Environment.NewLine);
            sBody.AppendFormat("select '{0}', '{1}', '{2}', t1.id, {3}, host_name(), app_name(), GetDate(), suser_sname(), ", _pTable.Parent.Parent.Name, _pTable.Parent.Name, _pTable.Name, (int)trigType);
            sBody.Append(Environment.NewLine);
            sBody.Append("'<Changes>'");
            sBody.Append(Environment.NewLine);

            switch (trigType)
            {
                case ActionTypes.INSERT:


                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.DataType.Name != DataType.Image.Name && col.DataType.Name != DataType.Text.Name && col.DataType.Name != DataType.NText.Name)
                        {
                            sBody.AppendFormat("+ dbo.fn_Compare('{0}', convert(varchar(MAX), t1.{0}, 121), null)", col.Name);
                            sBody.Append(Environment.NewLine);
                        }
                    }
                    sBody.Append("+'</Changes>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from inserted t1");
                    break;

                case ActionTypes.UPDATE:
                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.DataType.Name != DataType.Image.Name && col.DataType.Name != DataType.Text.Name && col.DataType.Name != DataType.NText.Name)
                        {
                            sBody.AppendFormat("+ dbo.fn_Compare('{0}', convert(varchar(MAX), t1.{0}, 121), convert(varchar(MAX), t2.{0}, 121))", col.Name);
                            sBody.Append(Environment.NewLine);
                        }
                    }
                    sBody.Append("+'</Changes>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from inserted t1 inner join deleted t2 on t1.id = t2.id");
                    break;

                case ActionTypes.DELETE:
                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.DataType.Name != DataType.Image.Name && col.DataType.Name != DataType.Text.Name && col.DataType.Name != DataType.NText.Name)
                        {
                            sBody.AppendFormat("+ dbo.fn_Compare('{0}', null, convert(varchar(MAX), t1.{0}, 121))", col.Name);
                            sBody.Append(Environment.NewLine);
                        }
                    }
                    sBody.Append("+'</Changes>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from deleted t1");
                    break;
            }

            sBody.Append(Environment.NewLine);
            sBody.Append("end");
            sBody.Append(Environment.NewLine);

            return sBody.ToString();
        }

        public static List<string> TemplateText(Template t, string NewName, bool addSysUserSysDate)
        {
            List<string> ret = new List<string>();

            //DECLARE @v sql_variant 
            //SET @v = N'Код'
            //EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'ContractType', N'COLUMN', N'SCode'
            //GO

            //DECLARE @v sql_variant 
            //SET @v = N'Наименование'
            //EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'ContractType', N'COLUMN', N'Name'
            //GO


            ret.Add("SET ANSI_NULLS ON");
            ret.Add("SET QUOTED_IDENTIFIER ON");
            ret.Add("SET ANSI_PADDING ON");

            string SysUserSysDate = "";
            string SysUserSysDate2 = "";
            if (addSysUserSysDate)
            {
                SysUserSysDate = string.Format("[SysUser] [nvarchar](255) NOT NULL CONSTRAINT [DF_{0}_SysUser]  DEFAULT (suser_sname()),[SysDate] [datetime] NOT NULL CONSTRAINT [DF_{0}_SysDate]  DEFAULT (getdate()),", NewName);
                switch (t)
                {
                    case Template.DICTIONARY:
                        break;
                    case Template.TREE_CARDS:
                        SysUserSysDate2 = string.Format("[SysUser] [nvarchar](255) NOT NULL CONSTRAINT [DF_{1}{0}_SysUser]  DEFAULT (suser_sname()),[SysDate] [datetime] NOT NULL CONSTRAINT [DF_{1}{0}_SysDate]  DEFAULT (getdate()),", NewName, "tree");
                        break;
                    case Template.HEADER_DETAILS:
                        SysUserSysDate2 = string.Format("[SysUser] [nvarchar](255) NOT NULL CONSTRAINT [DF_{0}{1}_SysUser]  DEFAULT (suser_sname()),[SysDate] [datetime] NOT NULL CONSTRAINT [DF_{0}{1}_SysDate]  DEFAULT (getdate()),", NewName, "Details");
                        break;
                    default:
                        break;
                }

                
            }

            switch (t)
            {
                case Template.DICTIONARY:
                    ret.Add(string.Format(@"CREATE TABLE [dbo].[{0}]([id] [int] IDENTITY(1,1) NOT NULL, [SCode] [varchar](255) NOT NULL,	[Name] [varchar](255) NOT NULL, {1} CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([id] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]", NewName, SysUserSysDate));
                    ret.Add("SET ANSI_PADDING OFF");
                    ret.Add(string.Format("CREATE UNIQUE NONCLUSTERED INDEX [IX_{0}_SCode] ON [dbo].[{0}] ([SCode] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]", NewName));
                    break;
                case Template.TREE_CARDS:
                    ret.Add("SET ANSI_PADDING OFF");
                    ret.Add(string.Format("CREATE TABLE [dbo].[tree{0}]([id] [int] IDENTITY(1,1) NOT NULL, [FP] [varchar](255) NOT NULL,	[Parent_FP] [varchar](255) NULL, [Name] [varchar](255) NOT NULL, {1} CONSTRAINT [PK_tree{0}] PRIMARY KEY NONCLUSTERED ([id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY], CONSTRAINT [IX_tree{0}] UNIQUE NONCLUSTERED ([FP] ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]", NewName, SysUserSysDate2));
                    
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] WITH NOCHECK ADD CONSTRAINT [FK_tree{0}_PARENT] FOREIGN KEY([Parent_FP]) REFERENCES [dbo].[tree{0}] ([FP])", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] CHECK CONSTRAINT [FK_tree{0}_PARENT]", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] WITH NOCHECK ADD CONSTRAINT [CH_tree{0}_INTEGER] CHECK ((((not([dbo].[fn_LCodeS]([fp]) like '%[-., ]%')))))", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] CHECK CONSTRAINT [CH_tree{0}_INTEGER]", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] WITH NOCHECK ADD CONSTRAINT [CH_tree{0}_NOTRIM] CHECK (([dbo].[fn_LCodeS]([fp]) like '[^0]%'))", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] CHECK CONSTRAINT [CH_tree{0}_NOTRIM]", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] WITH NOCHECK ADD CONSTRAINT [CH_tree{0}_NUMBER] CHECK ((isnumeric([dbo].[fn_LCodeS]([fp])) = 1))", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] CHECK CONSTRAINT [CH_tree{0}_NUMBER]", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] WITH NOCHECK ADD CONSTRAINT [CH_tree{0}_PARENT] CHECK ((isnull([Parent_FP],'') = isnull([dbo].[fn_ParCodeS]([fp]),'')))", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] CHECK CONSTRAINT [CH_tree{0}_PARENT]", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[tree{0}] ADD CONSTRAINT [DF_tree{0}_Name_Empty] DEFAULT ('') FOR [Name]", NewName));

                    ret.Add(string.Format("CREATE TABLE [dbo].[{0}]([id] [int] IDENTITY(1,1) NOT NULL, [Parent_FP] [varchar](255) NOT NULL, [Code] [int] NOT NULL,	[Name] [varchar](255) NOT NULL, {1} CONSTRAINT [PK_{0}] PRIMARY KEY NONCLUSTERED ([id] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]) ON [PRIMARY]", NewName, SysUserSysDate));
                    ret.Add(string.Format("ALTER TABLE [dbo].[{0}] WITH NOCHECK ADD CONSTRAINT [FK_{0}_PARENT] FOREIGN KEY([Parent_FP]) REFERENCES [dbo].[tree{0}] ([FP])", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_PARENT]", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[{0}] ADD CONSTRAINT [DF_{0}_Name_Empty] DEFAULT ('') FOR [Name]", NewName));
                    ret.Add(string.Format("CREATE UNIQUE NONCLUSTERED INDEX [IX_{0}_Parent_Code] ON [dbo].[{0}] ([Parent_FP] ASC, [Code] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]", NewName));
                    break;
                case Template.HEADER_DETAILS:
                    ret.Add(string.Format("CREATE TABLE [dbo].[{0}]([id] [int] IDENTITY(1,1) NOT NULL,[SomeHeaderColumn] [varchar](255) NULL, {1} CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED ([id] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]", NewName, SysUserSysDate));
                    ret.Add(string.Format("CREATE TABLE [dbo].[{0}Details]([id] [int] IDENTITY(1,1) NOT NULL, [Header_id] [int] NOT NULL, [SomeDetailsColumn] [varchar](255) NULL, {1} CONSTRAINT [PK_{0}Details] PRIMARY KEY CLUSTERED ([id] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]", NewName, SysUserSysDate2));
                    ret.Add("SET ANSI_PADDING OFF");
                    ret.Add(string.Format("ALTER TABLE [dbo].[{0}Details]  WITH CHECK ADD  CONSTRAINT [FK_{0}Details_{0}] FOREIGN KEY([Header_id]) REFERENCES [dbo].[{0}] ([id])", NewName));
                    ret.Add(string.Format("ALTER TABLE [dbo].[{0}Details] CHECK CONSTRAINT [FK_{0}Details_{0}]", NewName));
                    ret.Add(string.Format("CREATE NONCLUSTERED INDEX [IX_{0}Details_Header] ON [dbo].[{0}Details] ([Header_id] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]", NewName));
                    break;
                default:
                    break;
            }
            return ret;
        }

    }
}
