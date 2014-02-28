using System;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;
using System.IO;
using System.Drawing;
using Microsoft.SqlServer.Management.Smo;

namespace Generator
{
    public class DataSetGenerator
    {
        private Database _pDatabase;
        private DataSet _ds;
        public DataSetGenerator(Database db)
        {
            _pDatabase = db;
        }

        //public DataSet CreateGlobalDS(ArrayList tabColl)
        //{
        //    _ds = new DataSet("Global");
        //    foreach (object o in tabColl)
        //    {
        //        Table tbl = (Table)o;
        //        DataTable t = new DataTable(tbl.Name);
        //
        //        foreach (Column col in tbl.Columns)
        //        {
        //            DataColumn c = new DataColumn(col.Name, col.DataType.GetType());
        //            c.AllowDBNull = col.Nullable;
        //            if (col.Identity)
        //            {
        //                c.AutoIncrement = col.Identity;
        //                c.AutoIncrementSeed = col.IdentitySeed;
        //                c.AutoIncrementStep = col.IdentityIncrement;
        //            }
        //            t.Columns.Add(c);
        //        }
        //
        //        _ds.Tables.Add(t);
        //
        //    }
        //    return _ds;
        //}
    
        public DataSet CreateGlobalDS(SortedList tabColl, bool AddLinkedTables)
        {
            _ds = new DataSet("Global");

            if (AddLinkedTables)
            {
                SortedList tabCollLinked = new SortedList();
                
                foreach (DictionaryEntry o in tabColl)
                {
                    Table tbl = (Table)o.Value;
                    tabCollLinked[tbl.Name] = tbl;
                    foreach (ForeignKey k in tbl.ForeignKeys)
                    {
                        if (tabCollLinked[k.ReferencedTable] == null)
                            tabCollLinked[k.ReferencedTable] = _pDatabase.Tables[k.ReferencedTable];
                    }
                }
                tabColl = tabCollLinked;
            }
            foreach (DictionaryEntry o in tabColl)
            {
                Table tbl = (Table)o.Value;
                DataTable t = new DataTable(tbl.Name);

                foreach (Column col in tbl.Columns)
                {
                    DataColumn c = new DataColumn();
                    c.ColumnName = col.Name;
                    if (col.Identity)
                    {
                        c.AutoIncrement = col.Identity;
                        c.AutoIncrementSeed = col.IdentitySeed;
                        c.AutoIncrementStep = col.IdentityIncrement;
                    }

                    switch (col.DataType.SqlDataType)
                    {
                        case SqlDataType.BigInt:
                        case SqlDataType.Int:
                        case SqlDataType.SmallInt:
                        case SqlDataType.TinyInt:
                            c.DataType = typeof(Int32);
                            break;

                        case SqlDataType.Timestamp:
                            //c.DataType = typeof(long);
                            c.DataType = typeof(Int32);
                            break;

                        case SqlDataType.Bit:
                            c.DataType = typeof(Boolean);
                            break;

                        case SqlDataType.DateTime:
                        case SqlDataType.SmallDateTime:
                            c.DataType = typeof(DateTime);
                            break;
                        
                        case SqlDataType.Float:
                        case SqlDataType.Decimal:
                        case SqlDataType.Money:
                        case SqlDataType.Numeric:
                        case SqlDataType.Real:
                        case SqlDataType.SmallMoney:
                            c.DataType = typeof(Decimal);
                            break;

                        case SqlDataType.Char:
                        case SqlDataType.NChar:
                        case SqlDataType.Text:
                        case SqlDataType.VarChar:
                        case SqlDataType.VarCharMax:
                        case SqlDataType.NText:
                        case SqlDataType.NVarChar:
                        case SqlDataType.NVarCharMax:
                            c.DataType = typeof(String);
                            break;

                        case SqlDataType.Xml:
                            //c.DataType = typeof(XmlDocument);
                            c.DataType = typeof(String);
                            break;

                        case SqlDataType.Image:
                            c.DataType = typeof(Image);
                            break;

                        case SqlDataType.VarBinary:
                        case SqlDataType.VarBinaryMax:
                        case SqlDataType.Binary:
                            c.DataType = typeof(Stream);
                            break;


                        case SqlDataType.None:
                        case SqlDataType.SysName:
                        case SqlDataType.UniqueIdentifier:
                        case SqlDataType.UserDefinedDataType:
                        case SqlDataType.UserDefinedType:
                        case SqlDataType.Variant:
                            c.DataType = typeof(string);
                            break;

                        default:
                            break;
                    }
                    c.AllowDBNull = col.Nullable;
                    t.Columns.Add(c);
                }

                _ds.Tables.Add(t);

            }

            foreach (DictionaryEntry o in tabColl)
            {
                Table tbl = (Table)o.Value;
                DataTable dsTable1 = _ds.Tables[tbl.Name];
                foreach (ForeignKey fk in tbl.ForeignKeys)
                {
                    if (_ds.Tables.Contains(fk.ReferencedTable))
                    {
                        DataTable dsTable2 = _ds.Tables[fk.ReferencedTable];
                        string pkcolumn = PrimaryKeyColumn(tabColl, fk);
                        if (pkcolumn != null)
                        {
                            DataColumn dcParent = dsTable2.Columns[pkcolumn];
                            DataColumn dcChild = dsTable1.Columns[fk.Columns.ItemById(1).Name];
                            if (dcParent.DataType == dcChild.DataType)
                            {
                                DataRelation rel = new DataRelation(fk.Name, dcParent, dcChild);
                                _ds.Relations.Add(rel);
                            }
                        }
                    }
                }

            }
            
            return _ds;
        }
        private string RemoveBrackets(string s)
        {
            s.Replace("[", "");
            s.Replace("]", "");
            return s;
        }

        private string PrimaryKeyColumn(SortedList tabColl, ForeignKey fk)
        {
            string ret = null;
            Table t = (Table)tabColl[fk.ReferencedTable];
            if (t != null)
            {
                foreach (Column c in t.Columns)
                {
                    DataTable dtfk = c.EnumForeignKeys();
                    if (dtfk.Rows.Count > 0 && dtfk.Select("Name = '" + fk.Name + "'").Length > 0)
                    {
                        ret = c.Name;
                        return ret;
                    }
                }
            }
            return ret;
        }
    }
}
