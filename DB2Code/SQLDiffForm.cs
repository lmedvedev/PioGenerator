using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Collections.Specialized;

namespace SPGeneratorForms
{
    public partial class SQLDiffForm : Form
    {
        List<string> serversList = new List<string>();

        SqlConnection sqlConn1;
        SqlConnection sqlConn2;
        ServerConnection serverConn1;
        ServerConnection serverConn2;
        Server smoServer1;
        Server smoServer2;

        Database db1;
        Database db2;

        public SQLDiffForm()
        {
            InitializeComponent();

            serversList.Add("TEAM");
            serversList.Add("PIF");
            serversList.Add("PF");
            serversList.Add("KMT");
            comboServers1.DataSource = serversList;
            comboServers2.DataSource = serversList;


        }
        private string ConnString(ComboBox ServerName, TextBox UserName, TextBox Password, CheckBox Trusted)
        {
            string ret = "";
            if (Trusted.Checked)
            {
                ret = string.Format("server={0};Integrated Security=SSPI", ServerName.Text);
            }
            else
            {
                ret = string.Format("server={0};user={1};password={2}", ServerName.Text, UserName.Text, Password.Text);
            }
            return ret;
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {

            this.Cursor = Cursors.WaitCursor;

            try
            {
                string connString1 = ConnString(comboServers1, textUser1, textUser2, checkBoxTrusted1);
                string connString2 = ConnString(comboServers1, textUser1, textUser2, checkBoxTrusted1);

                sqlConn1 = new SqlConnection(connString1);
                serverConn1 = new ServerConnection(sqlConn1);
                smoServer1 = new Server(serverConn1);

                sqlConn2 = new SqlConnection(connString2);
                serverConn2 = new ServerConnection(sqlConn2);
                smoServer2 = new Server(serverConn2);

                serverConn1.Connect();
                serverConn2.Connect();

                foreach (Database db in smoServer1.Databases)
                {
                    comboBases1.Items.Add(db.Name);
                }

                foreach (Database db in smoServer2.Databases)
                {
                    comboBases2.Items.Add(db.Name);
                }

                comboBases1.Text = "DU2";
                comboBases2.Text = "PifClients";

                this.Cursor = Cursors.Default;
            }
            catch (System.Exception Ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(Ex.ToString(), this.Text);
            }


        }
        private void DrawDiff(SqlSmoObject so1, SqlSmoObject so2)
        {
            StringCollection sc1 = new StringCollection();
            StringCollection sc2 = new StringCollection();

            //ScriptingOptions so = new ScriptingOptions();
            //so.Add(ScriptOption.
            //ScriptOption. s1 = 

            if (so1 is StoredProcedure)
            {
                sc1 = ((StoredProcedure)so1).Script();
                sc2 = ((StoredProcedure)so2).Script();
            }
            else if (so1 is Table)
            {
                sc1 = ((Table)so1).Script();
                sc2 = ((Table)so2).Script();
            }
            else if (so1 is Check)
            {
                sc1 = ((Check)so1).Script();
                sc2 = ((Check)so2).Script();
            }
            else if (so1 is Index)
            {
                sc1 = ((Index)so1).Script();
                sc2 = ((Index)so2).Script();
            }
            else if (so1 is ForeignKey)
            {
                sc1 = ((ForeignKey)so1).Script();
                sc2 = ((ForeignKey)so2).Script();
            }

            string script1 = "";
            string script2 = "";

            foreach (string s in sc1)
            {
                    script1 += s;
            }

            foreach (string s in sc2)
            {
                    script2 += s;
            }

            script1 = script1.Replace("\r\n", "");
            script2 = script2.Replace("\r\n", "");

            if (script1 != script2)
            {
                int i = listBoxDiffers.Items.Add(so1);
            }
        
        }
        private void StringsDiff(RichTextBox rtb, string s1, string s2)
        {
            string[] sa1 = s1.Split('\r', '\n');
            string[] sa2 = s2.Split('\r', '\n');
            
            foreach (string s01 in sa1)
            {
                bool contains = false;
                foreach (string s02 in sa2)
                {
                    if (s01 == s02)
                        contains = true;
                }
                
                if (!contains)
                    rtb.Text += s01 + "\r\n";
            }
        }
        private void buttonDiff_Click(object sender, EventArgs e)
        {
            listBoxDiffers.Items.Clear();
            listBoxNotExists.Items.Clear();
            
            db1 = smoServer1.Databases[comboBases1.Text];
            db2 = smoServer2.Databases[comboBases2.Text];

            if (checkBoxTables.Checked)
            {
                progressBar1.Maximum = db1.Tables.Count;
                progressBar1.Value = 0;
                foreach (Table tbl1 in db1.Tables)
                {
                    progressBar1.Value += 1;
                    labelProgress.Text = tbl1.Name;
                    Application.DoEvents();
                    if (!tbl1.IsSystemObject)
                    {
                        Table tbl2 = db2.Tables[tbl1.Name];
                        if (tbl2 != null)
                        {
                            DrawDiff(tbl1, tbl2);

                            if (checkBoxConstraints.Checked)
                            {
                                foreach (Check chk1 in tbl1.Checks)
                                {
                                    Check chk2 = tbl2.Checks[chk1.Name];
                                    if (chk2 != null)
                                        DrawDiff(chk1, chk2);
                                    else
                                        listBoxNotExists.Items.Add(chk1);

                                }
                            }

                            if (checkBoxFKeys.Checked)
                            {
                                foreach (ForeignKey chk1 in tbl1.ForeignKeys)
                                {
                                    ForeignKey chk2 = tbl2.ForeignKeys[chk1.Name];
                                    if (chk2 != null)
                                        DrawDiff(chk1, chk2);
                                    else
                                        listBoxNotExists.Items.Add(chk1);

                                }
                            }

                            if (checkBoxIndexes.Checked)
                            {
                                foreach (Index chk1 in tbl1.Indexes)
                                {
                                    Index chk2 = tbl2.Indexes[chk1.Name];
                                    if (chk2 != null)
                                        DrawDiff(chk1, chk2);
                                    else
                                        listBoxNotExists.Items.Add(chk1);

                                }
                            }
                        }
                        else
                            listBoxNotExists.Items.Add(tbl1); 
                    }
                }

            }

            if (checkBoxFunctions.Checked)
            {
                progressBar1.Maximum = db1.UserDefinedFunctions.Count;
                progressBar1.Value = 0;
                foreach (UserDefinedFunction tbl1 in db1.UserDefinedFunctions)
                {
                    progressBar1.Value += 1;
                    labelProgress.Text = tbl1.Name;
                    Application.DoEvents();
                    if (!tbl1.IsSystemObject)
                    {
                        UserDefinedFunction tbl2 = db2.UserDefinedFunctions[tbl1.Name];
                        if (tbl2 != null)
                        {
                            DrawDiff(tbl1, tbl2);
                        }
                        else
                            listBoxNotExists.Items.Add(tbl1);
                    }
                }
            }

            
            if (checkBoxProcedures.Checked)
            {
                progressBar1.Maximum = db1.StoredProcedures.Count;
                progressBar1.Value = 0;
                foreach (StoredProcedure sp1 in db1.StoredProcedures)
                {
                    progressBar1.Value += 1;
                    labelProgress.Text = sp1.Name;
                    Application.DoEvents();
                    if (!sp1.IsSystemObject)
                    {
                        StoredProcedure sp2 = db2.StoredProcedures[sp1.Name];
                        if (sp2 != null)
                        {
                            DrawDiff(sp1, sp2);
                        }
                        else
                            listBoxNotExists.Items.Add(sp1);
                    }
                }
            }
            labelProgress.Text = "Готово";
        }

        private void listBoxDiffers_SelectedValueChanged(object sender, EventArgs e)
        {
            SqlSmoObject si = (SqlSmoObject)((ListBox)sender).SelectedItem;
            f1(si);
        }

        private void listBoxNotExists_SelectedValueChanged(object sender, EventArgs e)
        {
            SqlSmoObject si = (SqlSmoObject)((ListBox)sender).SelectedItem;
            f1(si);
        }

        public void f1(SqlSmoObject si)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            if (si is StoredProcedure || si is Table || si is Check || si is ForeignKey || si is UserDefinedFunction || si is Index)
            {
                string script1 = "";
                string script2 = "";

                if (si is StoredProcedure)
                {
                    StoredProcedure sc1 = (StoredProcedure)si;
                    StoredProcedure sc2 = db2.StoredProcedures[sc1.Name];
                    foreach (string s in sc1.Script()) script1 += s + "\r\n";
                    if (sc2 != null) foreach (string s in sc2.Script()) script2 += s + "\r\n";
                }
                else if (si is UserDefinedFunction)
                {
                    UserDefinedFunction sc1 = (UserDefinedFunction)si;
                    UserDefinedFunction sc2 = db2.UserDefinedFunctions[sc1.Name];
                    foreach (string s in sc1.Script()) script1 += s + "\r\n";
                    if (sc2 != null) foreach (string s in sc2.Script()) script2 += s + "\r\n";
                }
                else if (si is Table)
                {
                    Table sc1 = (Table)si;
                    Table sc2 = db2.Tables[sc1.Name];
                    foreach (string s in sc1.Script()) script1 += s + "\r\n";
                    if (sc2 != null) foreach (string s in sc2.Script()) script2 += s + "\r\n";
                }
                else if (si is Check)
                {
                    Check sc1 = (Check)si;
                    foreach (string s in sc1.Script()) script1 += s + "\r\n";
                    try
                    {
                        Check sc2 = db2.Tables[sc1.Parent.ToString()].Checks[sc1.Name];
                        if (sc2 != null) foreach (string s in sc2.Script()) script2 += s + "\r\n";
                    }
                    catch { }
                }
                else if (si is ForeignKey)
                {
                    ForeignKey sc1 = (ForeignKey)si;
                    foreach (string s in sc1.Script()) script1 += s + "\r\n";
                    try
                    {
                        ForeignKey sc2 = db2.Tables[sc1.Parent.ToString()].ForeignKeys[sc1.Name];
                        if (sc2 != null) foreach (string s in sc2.Script()) script2 += s + "\r\n";
                    }
                    catch { }
                }
                else if (si is Index)
                {
                    Index sc1 = (Index)si;
                    foreach (string s in sc1.Script()) script1 += s + "\r\n";
                    try
                    {
                        Index sc2 = db2.Tables[sc1.Parent.ToString()].Indexes[sc1.Name];
                        if (sc2 != null) foreach (string s in sc2.Script()) script2 += s + "\r\n";
                    }
                    catch { }
                }

                if (script1 != script2)
                {
                    StringsDiff(richTextBox1, script1, script2);
                    StringsDiff(richTextBox2, script2, script1);
                }
            }
        }
    }
}