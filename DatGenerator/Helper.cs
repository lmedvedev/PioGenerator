using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Xml;
using System.Drawing;

namespace DatGenerator
{
    public static class Helper
    {
        public static bool IsUnicode(string s)
        {
            return !(Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(s)) == s);
        }

        public static bool IsNumeric(string s)
        {
            foreach (char ch in s)
            {
                if (!char.IsDigit(ch))
                    return false;
            }
            return true;
        }
        public static string Capitalise(string s)
        {
            string ret = "";
            if (!string.IsNullOrEmpty(s))
                ret = s.Substring(0, 1).ToUpper() + s.Substring(1);
            return ret;
        }

        public static string GetMSDescription(ScriptNameObjectBase obj)
        {
            if (obj is TableViewTableTypeBase)
                return (((TableViewTableTypeBase)obj).ExtendedProperties["MS_Description"] == null) ? string.Format("{0}", obj.Name) : ((TableViewTableTypeBase)obj).ExtendedProperties["MS_Description"].Value.ToString();
            else if (obj is Column)
            {
                string desc = (((Column)obj).ExtendedProperties["MS_Description"] == null) ? "" : ((Column)obj).ExtendedProperties["MS_Description"].Value.ToString();

                if (string.IsNullOrEmpty(desc))
                {
                    switch (obj.Name.ToLower())
                    {
                        case "id":
                            desc = "#";
                            break;
                        case "fp":
                        case "fpn":
                            desc = "Полный путь";
                            break;
                        case "parent_fp":
                        case "parent_fpn":
                            desc = "Родитель";
                            break;
                        case "sysuser":
                            desc = "Кто";
                            break;
                        case "sysdate":
                            desc = "Когда";
                            break;
                        case "name":
                            desc = "Название";
                            break;
                        case "scode":
                            desc = "Код";
                            break;
                        default:
                            desc = string.Format("{0}", Helper.Capitalise(obj.Name));
                            break;
                    }
                }

                return desc;
            }
            else
                return null;
        }

        public static Type GetTypeFromDBType(SqlDataType dbtype)
        {
            Type type = null;
            switch (dbtype)
            {
                case SqlDataType.BigInt:
                case SqlDataType.Int:
                case SqlDataType.SmallInt:
                case SqlDataType.TinyInt:
                    type = typeof(Int32);
                    break;

                case SqlDataType.Timestamp:
                    //type = typeof(long);
                    type = typeof(Int32);
                    break;

                case SqlDataType.Bit:
                    type = typeof(Boolean);
                    break;

                case SqlDataType.DateTime:
                case SqlDataType.DateTime2:
                case SqlDataType.SmallDateTime:
                    type = typeof(DateTime);
                    break;

                case SqlDataType.Float:
                case SqlDataType.Decimal:
                case SqlDataType.Money:
                case SqlDataType.Numeric:
                case SqlDataType.Real:
                case SqlDataType.SmallMoney:
                    type = typeof(Decimal);
                    break;

                case SqlDataType.Char:
                case SqlDataType.NChar:
                case SqlDataType.Text:
                case SqlDataType.VarChar:
                case SqlDataType.VarCharMax:
                case SqlDataType.NText:
                case SqlDataType.NVarChar:
                case SqlDataType.NVarCharMax:
                    type = typeof(String);
                    break;

                case SqlDataType.Xml:
                    type = typeof(XmlDocument);
                    //type = typeof(String);
                    break;

                case SqlDataType.VarBinary:
                case SqlDataType.VarBinaryMax:
                case SqlDataType.Binary:
                    type = typeof(byte[]);
                    break;

                case SqlDataType.Image:
                    type = typeof(Bitmap);
                    break;


                case SqlDataType.UniqueIdentifier:
                    type = typeof(Guid);
                    break;
                case SqlDataType.None:
                case SqlDataType.SysName:
                case SqlDataType.UserDefinedDataType:
                case SqlDataType.UserDefinedType:
                case SqlDataType.Variant:
                case SqlDataType.Geography:
                    type = typeof(String);
                    break;

                default:
                    break;
            }
            return type;
        }
    }
}
