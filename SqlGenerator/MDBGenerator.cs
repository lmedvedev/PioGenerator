using System;
using System.Text;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace Generator
{
    public class MDBFileGenerator
    {
        public enum ActionTypes
        {
            UPDATE = 0,
            INSERT = 1,
            DELETE = -1
        }

        private Table _pTable;
        public MDBFileGenerator(Table ProcessedTable)
        {
            _pTable = ProcessedTable;
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
            tr.TextBody = TriggerTextBody(trigType);
            //tr.Parent = _pTable;
            return tr;
        }
        public StoredProcedure GenerateProcedure(ActionTypes procType)
        {
            StoredProcedure spG = new StoredProcedure(_pTable.Parent, "sp_" + _pTable.Name + "_" + procType.ToString()); // + "_gen");
            spG.TextMode = false;
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
                if (fk.ReferencedTable == "RegDocs" && fk.Columns.Contains("regdoc_id") && (_pTable.Columns.Contains("docNum") || _pTable.Columns.Contains("docDate") || _pTable.Columns.Contains("client_id")))
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

        public string ProcedureTextBody(ActionTypes procType)
        {
            StringBuilder sBody = new StringBuilder();
            StringBuilder whereClause = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;

            sBody.AppendFormat("begin{0}", Environment.NewLine);
            sBody.AppendFormat("--Created by SPGenerator {0}, {1}{2}", DateTime.Now, System.Security.Principal.WindowsIdentity.GetCurrent().Name, Environment.NewLine);
            sBody.AppendFormat("--Для защиты процедуры от переписывания генератором напишите \"--protect--\" без кавычек, с новой строки, в любом месте между begin & end{0}", Environment.NewLine);
            sBody.AppendFormat("SET NOCOUNT ON{0}", Environment.NewLine);

            switch (procType)
            {
                case ActionTypes.INSERT:

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
                    sBody.AppendFormat("return IDENT_CURRENT('{0}')", _pTable.Name);

                    break;

                case ActionTypes.UPDATE:
                    Column idCol = null;
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
                        
                        if (col.InPrimaryKey)
                        {
                            whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                            idCol = col;
                        }
                        else if (col.Identity)
                        {
                            whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                            idCol = col;
                        }
                    }

                    sBody.Remove(sBody.Length - tail, tail);
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
                    sBody.Append("if @@rowcount = 1");
                    sBody.Append(Environment.NewLine);
                    if (idCol != null)
                        sBody.AppendFormat("    return @{0} else RAISERROR('Неудачная попытка сохранить запись с {0}=%d в таблице {1}, {2}', 16, 1, @{0})", idCol.Name, _pTable.Name, whereClause.ToString());
                    else
                        sBody.AppendFormat("    return 0 else RAISERROR('Неудачная попытка сохранить запись в таблице {0}, {1}', 16, 1)", _pTable.Name, whereClause.ToString());
                    sBody.Append(Environment.NewLine);
                    break;

                case ActionTypes.DELETE:
                    sBody.AppendFormat("DELETE {0}", _pTable.ToString());
                    sBody.Append(Environment.NewLine);
                    foreach (Column col in _pTable.Columns)
                    {
                        if (col.Identity)
                        {
                            whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                        }
                        else if (col.InPrimaryKey)
                        {
                            whereClause.AppendFormat("([{0}] = @{0})", col.Name);
                        }
                    }
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
            StringBuilder whereClause = new StringBuilder();
            int tail = Environment.NewLine.Length + 1;

            sBody.Append("begin");
            sBody.Append(Environment.NewLine);
            sBody.Append("SET NOCOUNT ON");
            sBody.Append(Environment.NewLine);

            sBody.AppendFormat("INSERT INTO SysAudit (Key_ServerName, Key_DataBaseName, Key_TableName, Key_Id, Operation, HostName, ApplicationName, dt, usr, OpContent){0}", Environment.NewLine);
            sBody.AppendFormat("select '{0}', '{1}', '{2}', t1.id, {3}, host_name(), app_name(), GetDate(), suser_sname(), ", _pTable.Parent.Parent.Name, _pTable.Parent.Name, _pTable.Name, (int)trigType);
            sBody.Append(Environment.NewLine);
            sBody.Append("'<Changes>'");
            sBody.Append(Environment.NewLine);
            
            switch (trigType)
            {
                case ActionTypes.INSERT:

                    foreach (Column col in _pTable.Columns)
                    {
                        sBody.AppendFormat("+ dbo.fn_Compare('{0}', t1.{0}, null)", col.Name);
                        sBody.Append(Environment.NewLine);
                    }
                    sBody.Append("+'</Changes>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from inserted t1");
                    break;

                case ActionTypes.UPDATE:
                    foreach (Column col in _pTable.Columns)
                    {
                        sBody.AppendFormat("+ dbo.fn_Compare('{0}', t1.{0}, t2.{0})", col.Name);
                        sBody.Append(Environment.NewLine);
                    }
                    sBody.Append("+'</Changes>'");
                    sBody.Append(Environment.NewLine);
                    sBody.Append("from inserted t1 inner join deleted t2 on t1.id = t2.id");
                    break;

                case ActionTypes.DELETE:
                    foreach (Column col in _pTable.Columns)
                    {
                        sBody.AppendFormat("+ dbo.fn_Compare('{0}', null, t1.{0})", col.Name);
                        sBody.Append(Environment.NewLine);
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

    }
}
