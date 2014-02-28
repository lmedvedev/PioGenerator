using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;
using System.Text;
using System.Data;
//using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Data.OleDb;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

using Generator;
using DatGenerator;

using System.Runtime.InteropServices;
using System.IO;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace SPGeneratorForms
{
    /// <summary>
    /// Main UI for SPGen
    /// </summary>
    /// 


    public class frmMain : Form
    {
        //protected SQLDMOHelper dmoMain = new SQLDMOHelper();
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.ComboBox smbServers;
        private System.Windows.Forms.Splitter spltrMain;
        private System.Windows.Forms.Panel pnlConnectTo;
        private System.Windows.Forms.TreeView treeServerExplorer;
        private System.Windows.Forms.TextBox txtGeneratedCode;
        private System.Windows.Forms.ImageList imglstMain;
        private System.ComponentModel.IContainer components;

        string connString;
        private SqlConnection sqlConn;
        private ServerConnection serverConn;
        private CheckBox checkBoxTrusted;
        private CheckBox checkBoxUpdate;
        private CheckBox checkBoxInsert;
        private Button buttonGenerate;
        private CheckBox checkBoxDelete;
        private GroupBox groupProcedures;
        private CheckBox checkBoxInsertT;
        private CheckBox checkBoxDeleteT;
        private CheckBox checkBoxUpdateT;
        private CheckBox checkBoxExecute;
        private CheckBox checkBoxDataSet;
        private Server smoServer;
        private DataSetGenerator dsGenerator;
        private ArrayList dsTables;
        private CheckBox checkBoxPermissions;
        private CheckBox checkBoxMDB;
        private Button buttonSQLDiff;
        private CheckBox checkBoxSQLClasses;
        private TextBox textBoxTemplateName;
        private GroupBox groupBoxCreate;
        private RadioButton radioButtonHeaderDetails;
        private RadioButton radioButtonDict;
        private RadioButton radioButtonTree;
        private Button buttonCreateByTemplate;
        private CheckBox checkBox1;
        private SortedList dsTablesDict;

        const string ServerName = "ServerName";
        const string UserLogin = "UserLogin";
        const string UserPassword = "UserPassword";
        private RadioButton radioSQLCE;
        private RadioButton radioSQL;
        private Button buttonBrowse;
        private TextBox textBoxSQLCEFileName;
        private CheckBox checkBoxControllers;
        private CheckBox checkBoxColumns;
        private CheckBox checkBoxSetClasses;
        private CheckBox checkBoxDBClass;
        private CheckBox checkBoxFilters;
        private CheckBox checkBoxDatClasses;
        private CheckBox checkBoxAsOfFunction;
        private GroupBox groupBox2;
        private CheckBox checkBoxEntityForms;
        private CheckBox checkBoxFilterForms;
        private CheckBox checkBoxMenu;
        private Button buttonControllersPath;
        private CheckBox checkBoxUseControllersPath;
        private Button buttonEntityFormsPath;
        private CheckBox checkBoxUseEntityFormsPath;
        private Button buttonFilterFormsPath;
        private CheckBox checkBoxUseFilterFormsPath;

        const string IsTrusterConnection = "IsTrusterConnection";
        const string ControllersPath = "ControllersPath";
        const string FilterFormsPath = "FilterFormsPath";
        const string EntityFormsPath = "EntityFormsPath";
        const string UseControllersPath = "UseControllersPath";
        const string UseFilterFormsPath = "UseFilterFormsPath";
        const string UseEntityFormsPath = "UseEntityFormsPath";
        private FolderBrowserDialog fbd = new FolderBrowserDialog();
        private Button buttonDatClassesPath;
        private Button buttonDBClassPath;
        private Button buttonColumnsPath;
        private Button buttonFiltersPath;
        private Button buttonSetClassesPath;
        private CheckBox checkBoxUseDatClassesPath;
        private CheckBox checkBoxUseDBClassPath;
        private CheckBox checkBoxUseColumnsPath;
        private CheckBox checkBoxUseFiltersPath;
        private CheckBox checkBoxUseSetClassesPath;

        ServerList config = new ServerList();

        public frmMain()
        {
            InitializeComponent();

            //smbServers.Items.Add("TEAM");
            //smbServers.Items.Add("PIF");
            //smbServers.Items.Add("PF");
            //smbServers.Items.Add("KMT");

            //Microsoft.SqlServer.Management.Smo.Server CurrentServer; //= new Microsoft.SqlServer.Management.Smo.Server();
            //Microsoft.SqlServer.Management.Common.ServerConnection CurrentConnection; //= new Microsoft.SqlServer.Management.Common.ServerConnection(new SqlConnection());

            //ServerGroupCollection scoll = new ServerGroupCollection();
            //RegisteredServerCollection servers = RegisteredServerCollection();
            //Microsoft.SqlServer.Management.Smo.RegisteredServers.ServerGroupCollection a;

            //object[] objServers = (object[])dmoMain.RegisteredServers;
            //object[] objServers = (object[])s;
            //selServers.Items.AddRange(objServers);
            //foreach (RegisteredServer regserv in servers)
            //{
            //}
            //selServers.Items.AddRange((object[])s);


        }

        //private static System.Configuration.Configuration _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //private static string GetConfigValue(string name, string def_value)
        //{
        //    return (_config.AppSettings.Settings[name] != null && !string.IsNullOrEmpty(_config.AppSettings.Settings[name].Value)) ? _config.AppSettings.Settings[name].Value : def_value;
        //}
        //private static void SetConfigValue(string name, string value)
        //{
        //    if (_config.AppSettings.Settings[name] == null)
        //        _config.AppSettings.Settings.Add(name, value);
        //    else
        //        _config.AppSettings.Settings[name].Value = value;
        //}

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlConnectTo = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonDatClassesPath = new System.Windows.Forms.Button();
            this.buttonDBClassPath = new System.Windows.Forms.Button();
            this.buttonColumnsPath = new System.Windows.Forms.Button();
            this.buttonFiltersPath = new System.Windows.Forms.Button();
            this.buttonSetClassesPath = new System.Windows.Forms.Button();
            this.checkBoxUseDatClassesPath = new System.Windows.Forms.CheckBox();
            this.checkBoxUseDBClassPath = new System.Windows.Forms.CheckBox();
            this.checkBoxUseColumnsPath = new System.Windows.Forms.CheckBox();
            this.checkBoxUseFiltersPath = new System.Windows.Forms.CheckBox();
            this.checkBoxUseSetClassesPath = new System.Windows.Forms.CheckBox();
            this.checkBoxColumns = new System.Windows.Forms.CheckBox();
            this.checkBoxSetClasses = new System.Windows.Forms.CheckBox();
            this.buttonEntityFormsPath = new System.Windows.Forms.Button();
            this.checkBoxDBClass = new System.Windows.Forms.CheckBox();
            this.checkBoxUseEntityFormsPath = new System.Windows.Forms.CheckBox();
            this.checkBoxFilters = new System.Windows.Forms.CheckBox();
            this.buttonFilterFormsPath = new System.Windows.Forms.Button();
            this.checkBoxDatClasses = new System.Windows.Forms.CheckBox();
            this.checkBoxUseFilterFormsPath = new System.Windows.Forms.CheckBox();
            this.buttonControllersPath = new System.Windows.Forms.Button();
            this.checkBoxUseControllersPath = new System.Windows.Forms.CheckBox();
            this.checkBoxEntityForms = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterForms = new System.Windows.Forms.CheckBox();
            this.checkBoxControllers = new System.Windows.Forms.CheckBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxSQLCEFileName = new System.Windows.Forms.TextBox();
            this.radioSQLCE = new System.Windows.Forms.RadioButton();
            this.radioSQL = new System.Windows.Forms.RadioButton();
            this.checkBoxMenu = new System.Windows.Forms.CheckBox();
            this.groupBoxCreate = new System.Windows.Forms.GroupBox();
            this.buttonCreateByTemplate = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.radioButtonHeaderDetails = new System.Windows.Forms.RadioButton();
            this.radioButtonDict = new System.Windows.Forms.RadioButton();
            this.radioButtonTree = new System.Windows.Forms.RadioButton();
            this.textBoxTemplateName = new System.Windows.Forms.TextBox();
            this.checkBoxSQLClasses = new System.Windows.Forms.CheckBox();
            this.buttonSQLDiff = new System.Windows.Forms.Button();
            this.checkBoxMDB = new System.Windows.Forms.CheckBox();
            this.checkBoxPermissions = new System.Windows.Forms.CheckBox();
            this.checkBoxDataSet = new System.Windows.Forms.CheckBox();
            this.checkBoxExecute = new System.Windows.Forms.CheckBox();
            this.checkBoxInsertT = new System.Windows.Forms.CheckBox();
            this.checkBoxDeleteT = new System.Windows.Forms.CheckBox();
            this.checkBoxUpdateT = new System.Windows.Forms.CheckBox();
            this.groupProcedures = new System.Windows.Forms.GroupBox();
            this.checkBoxAsOfFunction = new System.Windows.Forms.CheckBox();
            this.checkBoxInsert = new System.Windows.Forms.CheckBox();
            this.checkBoxDelete = new System.Windows.Forms.CheckBox();
            this.checkBoxUpdate = new System.Windows.Forms.CheckBox();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.checkBoxTrusted = new System.Windows.Forms.CheckBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.smbServers = new System.Windows.Forms.ComboBox();
            this.treeServerExplorer = new System.Windows.Forms.TreeView();
            this.imglstMain = new System.Windows.Forms.ImageList(this.components);
            this.spltrMain = new System.Windows.Forms.Splitter();
            this.txtGeneratedCode = new System.Windows.Forms.TextBox();
            this.pnlConnectTo.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxCreate.SuspendLayout();
            this.groupProcedures.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlConnectTo
            // 
            this.pnlConnectTo.Controls.Add(this.groupBox2);
            this.pnlConnectTo.Controls.Add(this.buttonBrowse);
            this.pnlConnectTo.Controls.Add(this.textBoxSQLCEFileName);
            this.pnlConnectTo.Controls.Add(this.radioSQLCE);
            this.pnlConnectTo.Controls.Add(this.radioSQL);
            this.pnlConnectTo.Controls.Add(this.checkBoxMenu);
            this.pnlConnectTo.Controls.Add(this.groupBoxCreate);
            this.pnlConnectTo.Controls.Add(this.checkBoxSQLClasses);
            this.pnlConnectTo.Controls.Add(this.buttonSQLDiff);
            this.pnlConnectTo.Controls.Add(this.checkBoxMDB);
            this.pnlConnectTo.Controls.Add(this.checkBoxPermissions);
            this.pnlConnectTo.Controls.Add(this.checkBoxDataSet);
            this.pnlConnectTo.Controls.Add(this.checkBoxExecute);
            this.pnlConnectTo.Controls.Add(this.groupProcedures);
            this.pnlConnectTo.Controls.Add(this.buttonGenerate);
            this.pnlConnectTo.Controls.Add(this.checkBoxTrusted);
            this.pnlConnectTo.Controls.Add(this.buttonConnect);
            this.pnlConnectTo.Controls.Add(this.txtPassword);
            this.pnlConnectTo.Controls.Add(this.txtUser);
            this.pnlConnectTo.Controls.Add(this.smbServers);
            this.pnlConnectTo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlConnectTo.Location = new System.Drawing.Point(0, 0);
            this.pnlConnectTo.Name = "pnlConnectTo";
            this.pnlConnectTo.Size = new System.Drawing.Size(854, 357);
            this.pnlConnectTo.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.buttonDatClassesPath);
            this.groupBox2.Controls.Add(this.buttonDBClassPath);
            this.groupBox2.Controls.Add(this.buttonColumnsPath);
            this.groupBox2.Controls.Add(this.buttonFiltersPath);
            this.groupBox2.Controls.Add(this.buttonSetClassesPath);
            this.groupBox2.Controls.Add(this.checkBoxUseDatClassesPath);
            this.groupBox2.Controls.Add(this.checkBoxUseDBClassPath);
            this.groupBox2.Controls.Add(this.checkBoxUseColumnsPath);
            this.groupBox2.Controls.Add(this.checkBoxUseFiltersPath);
            this.groupBox2.Controls.Add(this.checkBoxUseSetClassesPath);
            this.groupBox2.Controls.Add(this.checkBoxColumns);
            this.groupBox2.Controls.Add(this.checkBoxSetClasses);
            this.groupBox2.Controls.Add(this.buttonEntityFormsPath);
            this.groupBox2.Controls.Add(this.checkBoxDBClass);
            this.groupBox2.Controls.Add(this.checkBoxUseEntityFormsPath);
            this.groupBox2.Controls.Add(this.checkBoxFilters);
            this.groupBox2.Controls.Add(this.buttonFilterFormsPath);
            this.groupBox2.Controls.Add(this.checkBoxDatClasses);
            this.groupBox2.Controls.Add(this.checkBoxUseFilterFormsPath);
            this.groupBox2.Controls.Add(this.buttonControllersPath);
            this.groupBox2.Controls.Add(this.checkBoxUseControllersPath);
            this.groupBox2.Controls.Add(this.checkBoxEntityForms);
            this.groupBox2.Controls.Add(this.checkBoxFilterForms);
            this.groupBox2.Controls.Add(this.checkBoxControllers);
            this.groupBox2.Location = new System.Drawing.Point(176, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(585, 205);
            this.groupBox2.TabIndex = 31;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CS-code";
            // 
            // buttonDatClassesPath
            // 
            this.buttonDatClassesPath.Location = new System.Drawing.Point(97, 40);
            this.buttonDatClassesPath.Name = "buttonDatClassesPath";
            this.buttonDatClassesPath.Size = new System.Drawing.Size(22, 20);
            this.buttonDatClassesPath.TabIndex = 45;
            this.buttonDatClassesPath.UseVisualStyleBackColor = true;
            // 
            // buttonDBClassPath
            // 
            this.buttonDBClassPath.Location = new System.Drawing.Point(97, 18);
            this.buttonDBClassPath.Name = "buttonDBClassPath";
            this.buttonDBClassPath.Size = new System.Drawing.Size(22, 20);
            this.buttonDBClassPath.TabIndex = 44;
            this.buttonDBClassPath.UseVisualStyleBackColor = true;
            // 
            // buttonColumnsPath
            // 
            this.buttonColumnsPath.Location = new System.Drawing.Point(97, 106);
            this.buttonColumnsPath.Name = "buttonColumnsPath";
            this.buttonColumnsPath.Size = new System.Drawing.Size(22, 20);
            this.buttonColumnsPath.TabIndex = 43;
            this.buttonColumnsPath.UseVisualStyleBackColor = true;
            // 
            // buttonFiltersPath
            // 
            this.buttonFiltersPath.Location = new System.Drawing.Point(97, 84);
            this.buttonFiltersPath.Name = "buttonFiltersPath";
            this.buttonFiltersPath.Size = new System.Drawing.Size(22, 20);
            this.buttonFiltersPath.TabIndex = 42;
            this.buttonFiltersPath.UseVisualStyleBackColor = true;
            // 
            // buttonSetClassesPath
            // 
            this.buttonSetClassesPath.Location = new System.Drawing.Point(97, 62);
            this.buttonSetClassesPath.Name = "buttonSetClassesPath";
            this.buttonSetClassesPath.Size = new System.Drawing.Size(22, 20);
            this.buttonSetClassesPath.TabIndex = 41;
            this.buttonSetClassesPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseDatClassesPath
            // 
            this.checkBoxUseDatClassesPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseDatClassesPath.Location = new System.Drawing.Point(127, 41);
            this.checkBoxUseDatClassesPath.Name = "checkBoxUseDatClassesPath";
            this.checkBoxUseDatClassesPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseDatClassesPath.TabIndex = 40;
            this.checkBoxUseDatClassesPath.Text = "DatClassesPath";
            this.checkBoxUseDatClassesPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseDBClassPath
            // 
            this.checkBoxUseDBClassPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseDBClassPath.Location = new System.Drawing.Point(127, 19);
            this.checkBoxUseDBClassPath.Name = "checkBoxUseDBClassPath";
            this.checkBoxUseDBClassPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseDBClassPath.TabIndex = 39;
            this.checkBoxUseDBClassPath.Text = "DBClassPath";
            this.checkBoxUseDBClassPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseColumnsPath
            // 
            this.checkBoxUseColumnsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseColumnsPath.Location = new System.Drawing.Point(127, 106);
            this.checkBoxUseColumnsPath.Name = "checkBoxUseColumnsPath";
            this.checkBoxUseColumnsPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseColumnsPath.TabIndex = 38;
            this.checkBoxUseColumnsPath.Text = "ColumnsPath";
            this.checkBoxUseColumnsPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseFiltersPath
            // 
            this.checkBoxUseFiltersPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseFiltersPath.Location = new System.Drawing.Point(127, 84);
            this.checkBoxUseFiltersPath.Name = "checkBoxUseFiltersPath";
            this.checkBoxUseFiltersPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseFiltersPath.TabIndex = 37;
            this.checkBoxUseFiltersPath.Text = "FiltersPath";
            this.checkBoxUseFiltersPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseSetClassesPath
            // 
            this.checkBoxUseSetClassesPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseSetClassesPath.Location = new System.Drawing.Point(127, 62);
            this.checkBoxUseSetClassesPath.Name = "checkBoxUseSetClassesPath";
            this.checkBoxUseSetClassesPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseSetClassesPath.TabIndex = 36;
            this.checkBoxUseSetClassesPath.Text = "SetClassesPath";
            this.checkBoxUseSetClassesPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxColumns
            // 
            this.checkBoxColumns.Location = new System.Drawing.Point(6, 106);
            this.checkBoxColumns.Name = "checkBoxColumns";
            this.checkBoxColumns.Size = new System.Drawing.Size(85, 22);
            this.checkBoxColumns.TabIndex = 34;
            this.checkBoxColumns.Text = "Columns";
            this.checkBoxColumns.UseVisualStyleBackColor = true;
            // 
            // checkBoxSetClasses
            // 
            this.checkBoxSetClasses.Location = new System.Drawing.Point(6, 62);
            this.checkBoxSetClasses.Name = "checkBoxSetClasses";
            this.checkBoxSetClasses.Size = new System.Drawing.Size(85, 22);
            this.checkBoxSetClasses.TabIndex = 33;
            this.checkBoxSetClasses.Text = "SetClasses";
            this.checkBoxSetClasses.UseVisualStyleBackColor = true;
            // 
            // buttonEntityFormsPath
            // 
            this.buttonEntityFormsPath.Location = new System.Drawing.Point(97, 172);
            this.buttonEntityFormsPath.Name = "buttonEntityFormsPath";
            this.buttonEntityFormsPath.Size = new System.Drawing.Size(22, 20);
            this.buttonEntityFormsPath.TabIndex = 35;
            this.buttonEntityFormsPath.UseVisualStyleBackColor = true;
            this.buttonEntityFormsPath.Click += new System.EventHandler(this.buttonEntityFormsPath_Click);
            // 
            // checkBoxDBClass
            // 
            this.checkBoxDBClass.Location = new System.Drawing.Point(6, 18);
            this.checkBoxDBClass.Name = "checkBoxDBClass";
            this.checkBoxDBClass.Size = new System.Drawing.Size(85, 22);
            this.checkBoxDBClass.TabIndex = 32;
            this.checkBoxDBClass.Text = "DBClass";
            this.checkBoxDBClass.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseEntityFormsPath
            // 
            this.checkBoxUseEntityFormsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseEntityFormsPath.Location = new System.Drawing.Point(127, 172);
            this.checkBoxUseEntityFormsPath.Name = "checkBoxUseEntityFormsPath";
            this.checkBoxUseEntityFormsPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseEntityFormsPath.TabIndex = 34;
            this.checkBoxUseEntityFormsPath.Text = "EntityFormsPath";
            this.checkBoxUseEntityFormsPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxFilters
            // 
            this.checkBoxFilters.Location = new System.Drawing.Point(6, 84);
            this.checkBoxFilters.Name = "checkBoxFilters";
            this.checkBoxFilters.Size = new System.Drawing.Size(85, 22);
            this.checkBoxFilters.TabIndex = 31;
            this.checkBoxFilters.Text = "Filters";
            this.checkBoxFilters.UseVisualStyleBackColor = true;
            // 
            // buttonFilterFormsPath
            // 
            this.buttonFilterFormsPath.Location = new System.Drawing.Point(97, 150);
            this.buttonFilterFormsPath.Name = "buttonFilterFormsPath";
            this.buttonFilterFormsPath.Size = new System.Drawing.Size(22, 20);
            this.buttonFilterFormsPath.TabIndex = 33;
            this.buttonFilterFormsPath.UseVisualStyleBackColor = true;
            this.buttonFilterFormsPath.Click += new System.EventHandler(this.buttonFilterFormsPath_Click);
            // 
            // checkBoxDatClasses
            // 
            this.checkBoxDatClasses.Location = new System.Drawing.Point(6, 40);
            this.checkBoxDatClasses.Name = "checkBoxDatClasses";
            this.checkBoxDatClasses.Size = new System.Drawing.Size(85, 22);
            this.checkBoxDatClasses.TabIndex = 30;
            this.checkBoxDatClasses.Text = "DatClasses";
            this.checkBoxDatClasses.UseVisualStyleBackColor = true;
            // 
            // checkBoxUseFilterFormsPath
            // 
            this.checkBoxUseFilterFormsPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseFilterFormsPath.Location = new System.Drawing.Point(127, 150);
            this.checkBoxUseFilterFormsPath.Name = "checkBoxUseFilterFormsPath";
            this.checkBoxUseFilterFormsPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseFilterFormsPath.TabIndex = 32;
            this.checkBoxUseFilterFormsPath.Text = "FilterFormsPath";
            this.checkBoxUseFilterFormsPath.UseVisualStyleBackColor = true;
            // 
            // buttonControllersPath
            // 
            this.buttonControllersPath.Location = new System.Drawing.Point(97, 128);
            this.buttonControllersPath.Name = "buttonControllersPath";
            this.buttonControllersPath.Size = new System.Drawing.Size(22, 20);
            this.buttonControllersPath.TabIndex = 31;
            this.buttonControllersPath.UseVisualStyleBackColor = true;
            this.buttonControllersPath.Click += new System.EventHandler(this.buttonControllersPath_Click);
            // 
            // checkBoxUseControllersPath
            // 
            this.checkBoxUseControllersPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxUseControllersPath.Location = new System.Drawing.Point(127, 128);
            this.checkBoxUseControllersPath.Name = "checkBoxUseControllersPath";
            this.checkBoxUseControllersPath.Size = new System.Drawing.Size(452, 22);
            this.checkBoxUseControllersPath.TabIndex = 30;
            this.checkBoxUseControllersPath.Text = "ControllersPath";
            this.checkBoxUseControllersPath.UseVisualStyleBackColor = true;
            // 
            // checkBoxEntityForms
            // 
            this.checkBoxEntityForms.Location = new System.Drawing.Point(6, 172);
            this.checkBoxEntityForms.Name = "checkBoxEntityForms";
            this.checkBoxEntityForms.Size = new System.Drawing.Size(85, 22);
            this.checkBoxEntityForms.TabIndex = 28;
            this.checkBoxEntityForms.Text = "EntityForms";
            this.checkBoxEntityForms.UseVisualStyleBackColor = true;
            // 
            // checkBoxFilterForms
            // 
            this.checkBoxFilterForms.Location = new System.Drawing.Point(6, 150);
            this.checkBoxFilterForms.Name = "checkBoxFilterForms";
            this.checkBoxFilterForms.Size = new System.Drawing.Size(85, 22);
            this.checkBoxFilterForms.TabIndex = 27;
            this.checkBoxFilterForms.Text = "FilterForms";
            this.checkBoxFilterForms.UseVisualStyleBackColor = true;
            // 
            // checkBoxControllers
            // 
            this.checkBoxControllers.Location = new System.Drawing.Point(6, 128);
            this.checkBoxControllers.Name = "checkBoxControllers";
            this.checkBoxControllers.Size = new System.Drawing.Size(85, 22);
            this.checkBoxControllers.TabIndex = 26;
            this.checkBoxControllers.Text = "Controllers";
            this.checkBoxControllers.UseVisualStyleBackColor = true;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowse.Location = new System.Drawing.Point(730, 29);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(31, 21);
            this.buttonBrowse.TabIndex = 24;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxSQLCEFileName
            // 
            this.textBoxSQLCEFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSQLCEFileName.Enabled = false;
            this.textBoxSQLCEFileName.Location = new System.Drawing.Point(84, 29);
            this.textBoxSQLCEFileName.Name = "textBoxSQLCEFileName";
            this.textBoxSQLCEFileName.ReadOnly = true;
            this.textBoxSQLCEFileName.Size = new System.Drawing.Size(640, 20);
            this.textBoxSQLCEFileName.TabIndex = 23;
            // 
            // radioSQLCE
            // 
            this.radioSQLCE.AutoSize = true;
            this.radioSQLCE.Enabled = false;
            this.radioSQLCE.Location = new System.Drawing.Point(12, 30);
            this.radioSQLCE.Name = "radioSQLCE";
            this.radioSQLCE.Size = new System.Drawing.Size(60, 17);
            this.radioSQLCE.TabIndex = 22;
            this.radioSQLCE.Text = "SQLCE";
            this.radioSQLCE.UseVisualStyleBackColor = true;
            // 
            // radioSQL
            // 
            this.radioSQL.AutoSize = true;
            this.radioSQL.Checked = true;
            this.radioSQL.Location = new System.Drawing.Point(12, 4);
            this.radioSQL.Name = "radioSQL";
            this.radioSQL.Size = new System.Drawing.Size(46, 17);
            this.radioSQL.TabIndex = 21;
            this.radioSQL.TabStop = true;
            this.radioSQL.Text = "SQL";
            this.radioSQL.UseVisualStyleBackColor = true;
            // 
            // checkBoxMenu
            // 
            this.checkBoxMenu.AutoSize = true;
            this.checkBoxMenu.Location = new System.Drawing.Point(182, 264);
            this.checkBoxMenu.Name = "checkBoxMenu";
            this.checkBoxMenu.Size = new System.Drawing.Size(55, 17);
            this.checkBoxMenu.TabIndex = 29;
            this.checkBoxMenu.Text = "Меню";
            this.checkBoxMenu.UseVisualStyleBackColor = true;
            // 
            // groupBoxCreate
            // 
            this.groupBoxCreate.Controls.Add(this.buttonCreateByTemplate);
            this.groupBoxCreate.Controls.Add(this.checkBox1);
            this.groupBoxCreate.Controls.Add(this.radioButtonHeaderDetails);
            this.groupBoxCreate.Controls.Add(this.radioButtonDict);
            this.groupBoxCreate.Controls.Add(this.radioButtonTree);
            this.groupBoxCreate.Controls.Add(this.textBoxTemplateName);
            this.groupBoxCreate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxCreate.Location = new System.Drawing.Point(0, 292);
            this.groupBoxCreate.Name = "groupBoxCreate";
            this.groupBoxCreate.Size = new System.Drawing.Size(854, 65);
            this.groupBoxCreate.TabIndex = 20;
            this.groupBoxCreate.TabStop = false;
            this.groupBoxCreate.Text = "Create";
            // 
            // buttonCreateByTemplate
            // 
            this.buttonCreateByTemplate.Location = new System.Drawing.Point(338, 11);
            this.buttonCreateByTemplate.Name = "buttonCreateByTemplate";
            this.buttonCreateByTemplate.Size = new System.Drawing.Size(72, 23);
            this.buttonCreateByTemplate.TabIndex = 24;
            this.buttonCreateByTemplate.Text = "Create";
            this.buttonCreateByTemplate.UseVisualStyleBackColor = true;
            this.buttonCreateByTemplate.Click += new System.EventHandler(this.buttonCreateByTemplate_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(338, 43);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(71, 17);
            this.checkBox1.TabIndex = 23;
            this.checkBox1.Text = "C# partial";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // radioButtonHeaderDetails
            // 
            this.radioButtonHeaderDetails.AutoSize = true;
            this.radioButtonHeaderDetails.Location = new System.Drawing.Point(233, 43);
            this.radioButtonHeaderDetails.Name = "radioButtonHeaderDetails";
            this.radioButtonHeaderDetails.Size = new System.Drawing.Size(98, 17);
            this.radioButtonHeaderDetails.TabIndex = 22;
            this.radioButtonHeaderDetails.Text = "Header+Details";
            this.radioButtonHeaderDetails.UseVisualStyleBackColor = true;
            // 
            // radioButtonDict
            // 
            this.radioButtonDict.AutoSize = true;
            this.radioButtonDict.Checked = true;
            this.radioButtonDict.Location = new System.Drawing.Point(233, 11);
            this.radioButtonDict.Name = "radioButtonDict";
            this.radioButtonDict.Size = new System.Drawing.Size(72, 17);
            this.radioButtonDict.TabIndex = 21;
            this.radioButtonDict.TabStop = true;
            this.radioButtonDict.Text = "Dictionary";
            this.radioButtonDict.UseVisualStyleBackColor = true;
            // 
            // radioButtonTree
            // 
            this.radioButtonTree.AutoSize = true;
            this.radioButtonTree.Location = new System.Drawing.Point(233, 27);
            this.radioButtonTree.Name = "radioButtonTree";
            this.radioButtonTree.Size = new System.Drawing.Size(80, 17);
            this.radioButtonTree.TabIndex = 20;
            this.radioButtonTree.Text = "Tree+Cards";
            this.radioButtonTree.UseVisualStyleBackColor = true;
            // 
            // textBoxTemplateName
            // 
            this.textBoxTemplateName.Location = new System.Drawing.Point(6, 19);
            this.textBoxTemplateName.Name = "textBoxTemplateName";
            this.textBoxTemplateName.Size = new System.Drawing.Size(205, 20);
            this.textBoxTemplateName.TabIndex = 19;
            // 
            // checkBoxSQLClasses
            // 
            this.checkBoxSQLClasses.AutoSize = true;
            this.checkBoxSQLClasses.Location = new System.Drawing.Point(65, 253);
            this.checkBoxSQLClasses.Name = "checkBoxSQLClasses";
            this.checkBoxSQLClasses.Size = new System.Drawing.Size(83, 17);
            this.checkBoxSQLClasses.TabIndex = 18;
            this.checkBoxSQLClasses.Text = "SQLClasses";
            this.checkBoxSQLClasses.UseVisualStyleBackColor = true;
            // 
            // buttonSQLDiff
            // 
            this.buttonSQLDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSQLDiff.Location = new System.Drawing.Point(767, 84);
            this.buttonSQLDiff.Name = "buttonSQLDiff";
            this.buttonSQLDiff.Size = new System.Drawing.Size(75, 23);
            this.buttonSQLDiff.TabIndex = 17;
            this.buttonSQLDiff.Text = "SQL Diff";
            this.buttonSQLDiff.UseVisualStyleBackColor = true;
            this.buttonSQLDiff.Click += new System.EventHandler(this.buttonSQLDiff_Click);
            // 
            // checkBoxMDB
            // 
            this.checkBoxMDB.AutoSize = true;
            this.checkBoxMDB.Location = new System.Drawing.Point(18, 253);
            this.checkBoxMDB.Name = "checkBoxMDB";
            this.checkBoxMDB.Size = new System.Drawing.Size(50, 17);
            this.checkBoxMDB.TabIndex = 16;
            this.checkBoxMDB.Text = "MDB";
            this.checkBoxMDB.UseVisualStyleBackColor = true;
            // 
            // checkBoxPermissions
            // 
            this.checkBoxPermissions.AutoSize = true;
            this.checkBoxPermissions.Location = new System.Drawing.Point(18, 228);
            this.checkBoxPermissions.Name = "checkBoxPermissions";
            this.checkBoxPermissions.Size = new System.Drawing.Size(81, 17);
            this.checkBoxPermissions.TabIndex = 15;
            this.checkBoxPermissions.Text = "Permissions";
            this.checkBoxPermissions.UseVisualStyleBackColor = true;
            // 
            // checkBoxDataSet
            // 
            this.checkBoxDataSet.AutoSize = true;
            this.checkBoxDataSet.Location = new System.Drawing.Point(18, 276);
            this.checkBoxDataSet.Name = "checkBoxDataSet";
            this.checkBoxDataSet.Size = new System.Drawing.Size(65, 17);
            this.checkBoxDataSet.TabIndex = 15;
            this.checkBoxDataSet.Text = "DataSet";
            this.checkBoxDataSet.UseVisualStyleBackColor = true;
            // 
            // checkBoxExecute
            // 
            this.checkBoxExecute.AutoSize = true;
            this.checkBoxExecute.Location = new System.Drawing.Point(18, 205);
            this.checkBoxExecute.Name = "checkBoxExecute";
            this.checkBoxExecute.Size = new System.Drawing.Size(65, 17);
            this.checkBoxExecute.TabIndex = 15;
            this.checkBoxExecute.Text = "Execute";
            this.checkBoxExecute.UseVisualStyleBackColor = true;
            // 
            // checkBoxInsertT
            // 
            this.checkBoxInsertT.AutoSize = true;
            this.checkBoxInsertT.Location = new System.Drawing.Point(6, 93);
            this.checkBoxInsertT.Name = "checkBoxInsertT";
            this.checkBoxInsertT.Size = new System.Drawing.Size(66, 17);
            this.checkBoxInsertT.TabIndex = 13;
            this.checkBoxInsertT.Text = "on insert";
            this.checkBoxInsertT.UseVisualStyleBackColor = true;
            // 
            // checkBoxDeleteT
            // 
            this.checkBoxDeleteT.AutoSize = true;
            this.checkBoxDeleteT.Location = new System.Drawing.Point(6, 129);
            this.checkBoxDeleteT.Name = "checkBoxDeleteT";
            this.checkBoxDeleteT.Size = new System.Drawing.Size(70, 17);
            this.checkBoxDeleteT.TabIndex = 15;
            this.checkBoxDeleteT.Text = "on delete";
            this.checkBoxDeleteT.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdateT
            // 
            this.checkBoxUpdateT.AutoSize = true;
            this.checkBoxUpdateT.Location = new System.Drawing.Point(6, 111);
            this.checkBoxUpdateT.Name = "checkBoxUpdateT";
            this.checkBoxUpdateT.Size = new System.Drawing.Size(74, 17);
            this.checkBoxUpdateT.TabIndex = 14;
            this.checkBoxUpdateT.Text = "on update";
            this.checkBoxUpdateT.UseVisualStyleBackColor = true;
            // 
            // groupProcedures
            // 
            this.groupProcedures.Controls.Add(this.checkBoxInsertT);
            this.groupProcedures.Controls.Add(this.checkBoxDeleteT);
            this.groupProcedures.Controls.Add(this.checkBoxAsOfFunction);
            this.groupProcedures.Controls.Add(this.checkBoxUpdateT);
            this.groupProcedures.Controls.Add(this.checkBoxInsert);
            this.groupProcedures.Controls.Add(this.checkBoxDelete);
            this.groupProcedures.Controls.Add(this.checkBoxUpdate);
            this.groupProcedures.Location = new System.Drawing.Point(12, 53);
            this.groupProcedures.Name = "groupProcedures";
            this.groupProcedures.Size = new System.Drawing.Size(152, 151);
            this.groupProcedures.TabIndex = 13;
            this.groupProcedures.TabStop = false;
            this.groupProcedures.Text = "SQL-code";
            // 
            // checkBoxAsOfFunction
            // 
            this.checkBoxAsOfFunction.AutoSize = true;
            this.checkBoxAsOfFunction.Location = new System.Drawing.Point(6, 70);
            this.checkBoxAsOfFunction.Name = "checkBoxAsOfFunction";
            this.checkBoxAsOfFunction.Size = new System.Drawing.Size(79, 17);
            this.checkBoxAsOfFunction.TabIndex = 31;
            this.checkBoxAsOfFunction.Text = "fn_AsOf_...";
            this.checkBoxAsOfFunction.UseVisualStyleBackColor = true;
            // 
            // checkBoxInsert
            // 
            this.checkBoxInsert.AutoSize = true;
            this.checkBoxInsert.Location = new System.Drawing.Point(6, 17);
            this.checkBoxInsert.Name = "checkBoxInsert";
            this.checkBoxInsert.Size = new System.Drawing.Size(69, 17);
            this.checkBoxInsert.TabIndex = 9;
            this.checkBoxInsert.Text = "sp_Insert";
            this.checkBoxInsert.UseVisualStyleBackColor = true;
            // 
            // checkBoxDelete
            // 
            this.checkBoxDelete.AutoSize = true;
            this.checkBoxDelete.Location = new System.Drawing.Point(6, 53);
            this.checkBoxDelete.Name = "checkBoxDelete";
            this.checkBoxDelete.Size = new System.Drawing.Size(74, 17);
            this.checkBoxDelete.TabIndex = 12;
            this.checkBoxDelete.Text = "sp_Delete";
            this.checkBoxDelete.UseVisualStyleBackColor = true;
            // 
            // checkBoxUpdate
            // 
            this.checkBoxUpdate.AutoSize = true;
            this.checkBoxUpdate.Location = new System.Drawing.Point(6, 35);
            this.checkBoxUpdate.Name = "checkBoxUpdate";
            this.checkBoxUpdate.Size = new System.Drawing.Size(78, 17);
            this.checkBoxUpdate.TabIndex = 10;
            this.checkBoxUpdate.Text = "sp_Update";
            this.checkBoxUpdate.UseVisualStyleBackColor = true;
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGenerate.Location = new System.Drawing.Point(767, 56);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(75, 23);
            this.buttonGenerate.TabIndex = 11;
            this.buttonGenerate.Text = "Generate";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click);
            // 
            // checkBoxTrusted
            // 
            this.checkBoxTrusted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxTrusted.AutoSize = true;
            this.checkBoxTrusted.Checked = true;
            this.checkBoxTrusted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTrusted.Location = new System.Drawing.Point(701, 7);
            this.checkBoxTrusted.Name = "checkBoxTrusted";
            this.checkBoxTrusted.Size = new System.Drawing.Size(62, 17);
            this.checkBoxTrusted.TabIndex = 8;
            this.checkBoxTrusted.Text = "Trusted";
            this.checkBoxTrusted.UseVisualStyleBackColor = true;
            this.checkBoxTrusted.CheckedChanged += new System.EventHandler(this.checkBoxTrusted_CheckedChanged);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnect.Location = new System.Drawing.Point(767, 4);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(75, 46);
            this.buttonConnect.TabIndex = 7;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.Click += new System.EventHandler(this.cmdConnect_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(615, 3);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.ReadOnly = true;
            this.txtPassword.Size = new System.Drawing.Size(80, 20);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.Text = "Password";
            // 
            // txtUser
            // 
            this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUser.Enabled = false;
            this.txtUser.Location = new System.Drawing.Point(528, 3);
            this.txtUser.Name = "txtUser";
            this.txtUser.ReadOnly = true;
            this.txtUser.Size = new System.Drawing.Size(81, 20);
            this.txtUser.TabIndex = 5;
            this.txtUser.Text = "User";
            // 
            // smbServers
            // 
            this.smbServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.smbServers.Location = new System.Drawing.Point(84, 3);
            this.smbServers.Name = "smbServers";
            this.smbServers.Size = new System.Drawing.Size(438, 21);
            this.smbServers.TabIndex = 4;
            this.smbServers.SelectedValueChanged += new System.EventHandler(this.smbServers_SelectedValueChanged);
            this.smbServers.TextChanged += new System.EventHandler(this.smbServers_TextChanged);
            // 
            // treeServerExplorer
            // 
            this.treeServerExplorer.CheckBoxes = true;
            this.treeServerExplorer.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeServerExplorer.FullRowSelect = true;
            this.treeServerExplorer.ImageIndex = 0;
            this.treeServerExplorer.ImageList = this.imglstMain;
            this.treeServerExplorer.Location = new System.Drawing.Point(0, 357);
            this.treeServerExplorer.Name = "treeServerExplorer";
            this.treeServerExplorer.SelectedImageIndex = 0;
            this.treeServerExplorer.Size = new System.Drawing.Size(176, 243);
            this.treeServerExplorer.TabIndex = 10;
            this.treeServerExplorer.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeServerExplorer_AfterCheck);
            this.treeServerExplorer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvwServerExplorer_BeforeExpand);
            // 
            // imglstMain
            // 
            this.imglstMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglstMain.ImageStream")));
            this.imglstMain.TransparentColor = System.Drawing.Color.Transparent;
            this.imglstMain.Images.SetKeyName(0, "");
            this.imglstMain.Images.SetKeyName(1, "");
            this.imglstMain.Images.SetKeyName(2, "");
            // 
            // spltrMain
            // 
            this.spltrMain.Location = new System.Drawing.Point(176, 357);
            this.spltrMain.Name = "spltrMain";
            this.spltrMain.Size = new System.Drawing.Size(3, 243);
            this.spltrMain.TabIndex = 11;
            this.spltrMain.TabStop = false;
            // 
            // txtGeneratedCode
            // 
            this.txtGeneratedCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGeneratedCode.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtGeneratedCode.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtGeneratedCode.HideSelection = false;
            this.txtGeneratedCode.Location = new System.Drawing.Point(179, 357);
            this.txtGeneratedCode.Multiline = true;
            this.txtGeneratedCode.Name = "txtGeneratedCode";
            this.txtGeneratedCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGeneratedCode.Size = new System.Drawing.Size(675, 243);
            this.txtGeneratedCode.TabIndex = 12;
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(854, 600);
            this.Controls.Add(this.txtGeneratedCode);
            this.Controls.Add(this.spltrMain);
            this.Controls.Add(this.treeServerExplorer);
            this.Controls.Add(this.pnlConnectTo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(503, 503);
            this.Name = "frmMain";
            this.Text = "Stored Procedure Generator";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.pnlConnectTo.ResumeLayout(false);
            this.pnlConnectTo.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBoxCreate.ResumeLayout(false);
            this.groupBoxCreate.PerformLayout();
            this.groupProcedures.ResumeLayout(false);
            this.groupProcedures.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        [STAThread]
        static void Main()
        {
            Application.Run(new frmMain());
        }

        private void cmdConnect_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            //Valid connection details				
            treeServerExplorer.Nodes.Clear();

            // List Databases
            try
            {

                if (radioSQL.Checked)
                {

                    if (checkBoxTrusted.Checked)
                    {
                        connString = string.Format("server={0};Integrated Security=SSPI", smbServers.Text);
                    }
                    else
                    {
                        connString = string.Format("server={0};user={1};password={2}", smbServers.Text, txtUser.Text, txtPassword.Text);
                    }
                    sqlConn = new SqlConnection(connString);
                    serverConn = new ServerConnection(sqlConn);
                    smoServer = new Server(serverConn);

                    serverConn.Connect();

                    if (smoServer.Databases != null)
                    {
                        smoServer.Databases.Refresh(true);
                        foreach (Database db in smoServer.Databases)
                        {
                            if (!db.IsSystemObject)
                            {
                                TreeNode treenodeDatabase = new TreeNode(db.Name, 0, 0);
                                treenodeDatabase.Tag = db;
                                treenodeDatabase.Nodes.Add("");
                                treeServerExplorer.Nodes.Add(treenodeDatabase);
                            }

                            //switch (db.Name)
                            //{
                            //    case "master":
                            //    case "model":
                            //    case "msdb":
                            //    case "tempdb":
                            //        break;
                            //    default:
                            //        TreeNode treenodeDatabase = new TreeNode(db.Name, 0, 0);
                            //        treenodeDatabase.Tag = db;
                            //        treenodeDatabase.Nodes.Add("");
                            //        treeServerExplorer.Nodes.Add(treenodeDatabase);
                            //        break;
                            //}
                        }
                    }

                    serverConn.Disconnect();
                    //sqlConn.Close();
                    sqlConn = null;
                    serverConn = null;
                    smoServer = null;

                }
                else if (radioSQLCE.Checked)
                {
                    //string ConnString = @"Data Source='" + textBoxSQLCEFileName.Text + "'";

                    //SqlCeConnection _dbConnection = new SqlCeConnection(ConnString);
                    //_dbConnection.Open();

                    //TreeNode treenodeDatabase = new TreeNode(_dbConnection.Database, 0, 0);
                    //treenodeDatabase.Tag = _dbConnection;
                    //treenodeDatabase.Nodes.Add("");
                    //treeServerExplorer.Nodes.Add(treenodeDatabase);

                }

                this.Cursor = Cursors.Default;
            }
            catch (System.Exception Ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(Ex.ToString(), this.Text);
            }
        }

        private void tvwServerExplorer_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            e.Node.Nodes.Clear();
            try
            {

                if (e.Node.Tag is Database)
                {

                    if (checkBoxTrusted.Checked)
                    {
                        connString = string.Format("server={0};Integrated Security=SSPI;", smbServers.Text);
                    }
                    else
                    {
                        connString = string.Format("server={0};user={1};password={2};", smbServers.Text, txtUser.Text, txtPassword.Text);
                    }

                    //connString += string.Format("database={0};", e.Node.Text);

                    sqlConn = new SqlConnection(connString);
                    serverConn = new ServerConnection(sqlConn);
                    smoServer = new Server(serverConn);

                    serverConn.Connect();

                    Database currentDataBase = smoServer.Databases[e.Node.Text];
                    //Database currentDataBase = new Database(smoServer, e.Node.Text);

                    //                    currentDataBase.Initialize(true);

                    TableCollection tables = currentDataBase.Tables;
                    tables.Refresh();


                    foreach (Table tbl in tables)
                    {
                        bool containsBadCols = false;
                        if (tbl.IsSystemObject) continue;

                        if (tbl.Name.StartsWith("_"))
                            continue;

                        if (tbl.Name.ToLower().StartsWith("temp"))
                            continue;

                        if (tbl.Name.ToLower().StartsWith("tmp"))
                            continue;

                        if (tbl.Name.ToLower().StartsWith("xxx"))
                            continue;

                        if (tbl.Name.ToLower().Contains("_tmp"))
                            continue;

                        if (tbl.Name.ToLower().Contains("_temp"))
                            continue;

                        if (tbl.Name.Contains("-"))
                            continue;

                        if (tbl.Name.Contains(" "))
                            continue;

                        if (tbl.Name.Contains("#"))
                            continue;

                        if (Helper.IsUnicode(tbl.Name))
                            continue;

                        foreach (Column col in tbl.Columns)
                        {
                            containsBadCols = true;

                            if (col.Name.Contains("-"))
                                continue;

                            if (col.Name.Contains(" "))
                                continue;

                            if (col.Name.Contains("#"))
                                continue;

                            if (Helper.IsUnicode(col.Name))
                                continue;

                            containsBadCols = false;
                        }

                        if (!containsBadCols)
                        {

                            TreeNode treenodeTable = new TreeNode(tbl.Name, 1, 1);
                            treenodeTable.Tag = tbl;
                            e.Node.Nodes.Add(treenodeTable);
                        }
                    }

                }
                //else if (e.Node.Tag is SqlCeConnection)
                //{
                //    Database currentDataBase = smoServer.Databases[e.Node.Text];
                //}
            }
            catch (System.Exception Ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(Ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Cursor = Cursors.Default;
        }

        private void checkBoxTrusted_CheckedChanged(object sender, EventArgs e)
        {
            txtUser.Enabled = !checkBoxTrusted.Checked;
            txtUser.ReadOnly = checkBoxTrusted.Checked;
            txtPassword.Enabled = !checkBoxTrusted.Checked;
            txtPassword.ReadOnly = checkBoxTrusted.Checked;
        }

        private string AccessTableCreationScript(Table t)
        {
            StringBuilder TableCreationScript = new StringBuilder();
            TableCreationScript.AppendFormat("CREATE TABLE {0}\r\n(", t.Name);
            bool needIndex = false;

            foreach (Column c in t.Columns)
            {

                if (c.Name.ToLower() == "scode" && c.DataType.Name == "varchar")
                    needIndex = true;

                if (c.InPrimaryKey)
                {
                    string AccType = (c.Identity) ? string.Format("AUTOINCREMENT({0}, {1})", c.IdentitySeed, c.IdentityIncrement) : c.DataType.Name;
                    TableCreationScript.AppendFormat("{0} {1} CONSTRAINT PK_{2} PRIMARY KEY", c.Name, AccType, t.Name);
                }
                else
                    if (c.DataType.Name == "int")
                        TableCreationScript.AppendFormat("{0} {1} {2}", c.Name, c.DataType.Name, (c.Nullable) ? "" : "not null");
                    else if (c.DataType.Name == "bigint")
                        TableCreationScript.AppendFormat("{0} {1} {2}", c.Name, "number", (c.Nullable) ? "" : "not null");
                    else if (c.DataType.Name == "bit")
                        TableCreationScript.AppendFormat("{0} {1} {2}", c.Name, "bit", (c.Nullable) ? "" : "not null");
                    else if (c.DataType.Name == "decimal")
                        TableCreationScript.AppendFormat("{0} {1} {2}", c.Name, "number", (c.Nullable) ? "" : "not null");
                    else if (c.DataType.Name == "datetime")
                        TableCreationScript.AppendFormat("{0} {1} {2}", c.Name, "datetime", (c.Nullable) ? "" : "not null");
                    else if (c.DataType.SqlDataType.ToString() == "Xml")
                        TableCreationScript.AppendFormat("{0} {1} {2}", c.Name, "memo", (c.Nullable) ? "" : "not null");
                    else if (c.DataType.MaximumLength > 255)
                        TableCreationScript.AppendFormat("{0} {1} {2}", c.Name, "memo", (c.Nullable) ? "" : "not null");
                    else
                        TableCreationScript.AppendFormat("{0} {1}({2}) {3}", c.Name, c.DataType.Name, c.DataType.MaximumLength, (c.Nullable) ? "" : "not null");

                if (c == t.Columns[t.Columns.Count - 1])
                    TableCreationScript.Append("\r\n");
                else
                    TableCreationScript.Append(",\r\n");
            }
            TableCreationScript.Append(");\r\n");

            if (needIndex)
            {
                //CREATE [ UNIQUE ] INDEX index ON table (field [ASC|DEC][, field [ASC|DESC], ...]) [WITH { PRIMARY | DISALLOW NULL | IGNORE NULL }]

                TableCreationScript.AppendFormat("CREATE UNIQUE INDEX IX_{0}_sCode ON {0} (sCode ASC) WITH DISALLOW NULL;\r\n", t.Name);
            }

            return TableCreationScript.ToString();

        }
        //private string AccessIndexCreationScript(string tName, string cName)
        //{

        //}
        private void ProcessSubNodes(TreeNode ParentNode)
        {
            if (ParentNode.Nodes != null)
            {
                foreach (TreeNode n in ParentNode.Nodes)
                {
                    if (n.Tag is Table && n.Checked)
                    {
                        Table tbl = (Table)n.Tag;
                        dsTables.Add(tbl);
                        dsTablesDict[tbl.Name] = tbl;
                        SQLTextGenerator tgen = new SQLTextGenerator(tbl);
                        StringBuilder textResult = new StringBuilder();
                        StringCollection tr = new StringCollection();
                        StoredProcedure spI = null;
                        StoredProcedure spU = null;
                        StoredProcedure spD = null;
                        UserDefinedFunction fnAsOf = null;
                        Trigger trI = null;
                        Trigger trU = null;
                        Trigger trD = null;

                        ObjectPermissionSet permExecute = new ObjectPermissionSet(ObjectPermission.Execute);
                        ObjectPermissionSet permSelect = new ObjectPermissionSet(ObjectPermission.Select);

                        //Database currentDb = tbl.Parent;
                        if (checkBoxAsOfFunction.Checked)
                        {
                            fnAsOf = tgen.GenerateAsOfFunction();
                            if (fnAsOf != null)
                            {
                                foreach (string s in fnAsOf.Script())
                                    textResult.AppendFormat("{0}{1}", s, Environment.NewLine);
                                textResult.AppendFormat("GO{0}", Environment.NewLine);
                            }
                        }

                        if (checkBoxInsert.Checked)
                        {
                            spI = tgen.GenerateProcedure(SQLTextGenerator.ActionTypes.INSERT);
                            foreach (string s in spI.Script())
                                textResult.AppendFormat("{0}{1}", s, Environment.NewLine);
                            textResult.AppendFormat("GO{0}", Environment.NewLine);

                            if (checkBoxPermissions.Checked)
                            {
                                spI.Grant(ObjectPermission.Execute, "qa_Accounting");

                                textResult.AppendFormat("grant execute on {0} to qa_Accounting{1}", spI.Name, Environment.NewLine);
                                textResult.AppendFormat("GO{0}", Environment.NewLine);
                            }
                        }

                        if (checkBoxUpdate.Checked)
                        {
                            spU = tgen.GenerateProcedure(SQLTextGenerator.ActionTypes.UPDATE);
                            foreach (string s in spU.Script())
                                textResult.AppendFormat("{0}{1}", s, Environment.NewLine);
                            textResult.AppendFormat("GO{0}", Environment.NewLine);
                            if (checkBoxPermissions.Checked)
                            {
                                spU.Grant(ObjectPermission.Execute, "qa_Accounting");
                                textResult.AppendFormat("grant execute on {0} to qa_Accounting{1}", spU.Name, Environment.NewLine);
                                textResult.AppendFormat("GO{0}", Environment.NewLine);
                            }
                        }

                        if (checkBoxDelete.Checked)
                        {
                            spD = tgen.GenerateProcedure(SQLTextGenerator.ActionTypes.DELETE);
                            foreach (string s in spD.Script())
                                textResult.AppendFormat("{0}{1}", s, Environment.NewLine);
                            textResult.AppendFormat("GO{0}", Environment.NewLine);
                            if (checkBoxPermissions.Checked)
                            {
                                spD.Grant(ObjectPermission.Execute, "qa_Accounting");
                                textResult.AppendFormat("grant execute on {0} to qa_Accounting{1}", spD.Name, Environment.NewLine);
                                textResult.AppendFormat("GO{0}", Environment.NewLine);
                            }

                        }

                        if (tbl.Name.ToLower() != "sysaudit")
                        {
                            if (checkBoxInsertT.Checked)
                            {
                                trI = tgen.GenerateTrigger(SQLTextGenerator.ActionTypes.INSERT);
                                foreach (string s in trI.Script())
                                    textResult.AppendFormat("{0}{1}", s, Environment.NewLine);
                                textResult.AppendFormat("GO{0}", Environment.NewLine);
                            }

                            if (checkBoxUpdateT.Checked)
                            {
                                trU = tgen.GenerateTrigger(SQLTextGenerator.ActionTypes.UPDATE);
                                foreach (string s in trU.Script())
                                    textResult.AppendFormat("{0}{1}", s, Environment.NewLine);
                                textResult.AppendFormat("GO{0}", Environment.NewLine);
                            }

                            if (checkBoxDeleteT.Checked)
                            {
                                trD = tgen.GenerateTrigger(SQLTextGenerator.ActionTypes.DELETE);
                                foreach (string s in trD.Script())
                                    textResult.AppendFormat("{0}{1}", s, Environment.NewLine);
                                textResult.AppendFormat("GO{0}", Environment.NewLine);
                            }
                        }

                        if (checkBoxPermissions.Checked)
                        {
                            textResult.AppendFormat("grant select on {0} to qa_BackOffice{1}", tbl.Name, Environment.NewLine);
                            textResult.AppendFormat("grant select on {0} to qa_Accounting{1}", tbl.Name, Environment.NewLine);
                        }

                        txtGeneratedCode.Text += textResult.ToString() + Environment.NewLine;

                        if (checkBoxExecute.Checked)
                        {
                            if (spI != null)
                            {
                                SaveStoredProcedure(tbl.Parent, spI);
                                //ObjectPermissionInfo[] permInfo = spI.EnumObjectPermissions(ObjectPermission.Execute);
                            }
                            if (spU != null) SaveStoredProcedure(tbl.Parent, spU);
                            if (spD != null) SaveStoredProcedure(tbl.Parent, spD);

                            if (trI != null) SaveTrigger(tbl, trI);
                            if (trU != null) SaveTrigger(tbl, trU);
                            if (trD != null) SaveTrigger(tbl, trD);
                            if (fnAsOf != null) SaveUDF(tbl.Parent, fnAsOf);
                        }

                    }
                }
            }
        }
        private bool SaveUDF(Database db, UserDefinedFunction udf)
        {
            bool ret = false;
            if (db.UserDefinedFunctions.Contains(udf.Name, udf.Schema))
            {
                UserDefinedFunction tmp = db.UserDefinedFunctions[udf.Name, udf.Schema];
                if (!tmp.TextBody.Contains("\r\n--protect--"))
                {
                    tmp.TextMode = true;
                    tmp.TextHeader = udf.TextHeader;
                    tmp.TextBody = udf.TextBody;
                    tmp.Alter();
                    ret = true;
                }
            }
            else
            {
                udf.Create();
                ret = true;
            }
            return ret;
        }
        private bool SaveStoredProcedure(Database db, StoredProcedure sp)
        {
            bool ret = false;
            if (db.StoredProcedures.Contains(sp.Name, sp.Schema))
            {
                StoredProcedure tmp = db.StoredProcedures[sp.Name, sp.Schema];
                //ObjectPermissionInfo[] p = tmp.EnumObjectPermissions();
                //ObjectPermissionSet s = p[0].
                if (!tmp.TextBody.Contains("\r\n--protect--"))
                {
                    tmp.TextMode = true;
                    tmp.TextHeader = sp.TextHeader;
                    tmp.TextBody = sp.TextBody;
                    tmp.Alter();
                    ret = true;
                }
            }
            else
            {
                sp.Create();
                ret = true;
            }
            return ret;
        }

        private void SaveTrigger(Table tbl, Trigger tr)
        {
            if (tbl.Triggers.Contains(tr.Name))
            {
                Trigger tmp = tbl.Triggers[tr.Name];
                //ObjectPermissionInfo[] p = tmp.EnumObjectPermissions();
                //ObjectPermissionSet s = p[0].
                tmp.TextMode = true;
                tmp.TextHeader = tr.TextHeader;
                tmp.TextBody = tr.TextBody;
                //tmp.IsEnabled = tr.IsEnabled;
                //tmp.IsEnabled = false;
                tmp.Alter();
            }
            else
                tr.Create();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            //try
            //{
            this.Cursor = Cursors.WaitCursor;
            txtGeneratedCode.Text = "";
            dsTables = new ArrayList();
            dsTablesDict = new SortedList();
            foreach (TreeNode nFirst in treeServerExplorer.Nodes)
            {
                ProcessSubNodes(nFirst);
            }

            if (dsTablesDict.Count > 0)
            {

                Database db = ((Table)dsTablesDict.GetByIndex(0)).Parent;
                StringBuilder sb = new StringBuilder();

                if (checkBoxDataSet.Checked)
                {
                    dsGenerator = new DataSetGenerator(db);
                    DataSet ds = dsGenerator.CreateGlobalDS(dsTablesDict, true);
                    ds.WriteXmlSchema("c:\\" + db.Name + ".xsd");
                }

                if (checkBoxMenu.Checked)
                {
                    foreach (DictionaryEntry obj in dsTablesDict)
                    {
                        Table table = (Table)obj.Value;
                        sb.AppendFormat("//ToolStripMenuItem {0} = MFController.AddMenuSubItem<{0}Controller>(Dict, \"{1}\");\r\n", table.Name, Helper.GetMSDescription(table));
                    }
                }

                DatGenerator.DatClassGenerator dg = new DatGenerator.DatClassGenerator();
                CSharpCodeProvider CodeProvider = new CSharpCodeProvider();

                CodeNamespace nsForms = dg.GetFormsNamespaceFromDB(db);
                CodeNamespace nsDat = dg.GetDatNamespaceFromDB(db);
                bool onceIncluded = false;

                foreach (DictionaryEntry obj in dsTablesDict)
                {
                    Table table = (Table)obj.Value;
                    if (table.Name == "SysAudit") continue;

                    string headerTable = null;
                    CodeTypeDeclaration datclass = null;

                    DatGenerator.DatClassGenerator.DatClass helper = DatClassGenerator.GenerateOurDatClass(table, out headerTable);


                    #region Controllers
                    if (checkBoxControllers.Checked)
                    {
                        if (!helper.BaseTypes.Contains("ICardDat"))
                        {
                            CodeTypeDeclaration tpControllers = ControllersGenerator.CreateController(table, helper);
                            if (checkBoxUseControllersPath.Checked)
                            {
                                CodeNamespace nscontroller = dg.GetFormsNamespaceFromDB(table.Parent);
                                DirectoryInfo dir = new DirectoryInfo(checkBoxUseControllersPath.Text);
                                WriteCSFile(dir, nscontroller, CodeProvider, tpControllers);
                            }
                            else
                            {
                                nsForms.Types.Add(tpControllers);
                            }
                        }
                    }

                    #endregion
                    #region FilterForms
                    if (checkBoxFilterForms.Checked)
                    {
                        if (!helper.BaseTypes.Contains("ICardDat") && !helper.BaseTypes.Contains("ITreeDat"))
                        {

                            CodeTypeDeclaration tpFilterForm = ControllersGenerator.CreateFilterForm(table);
                            CodeTypeDeclaration tpFilterClassBase = FiltersGenerator.CreateBaseFilterClass(table);
                            FiltersGenerator.BaseFilterClassAddProperties(table, ref tpFilterClassBase);
                            CodeTypeDeclaration tpFilterFormBase = ControllersGenerator.CreateFilterBaseForm(table, tpFilterClassBase);
                            if (checkBoxUseFilterFormsPath.Checked)
                            {
                                CodeNamespace nscontroller = dg.GetFormsNamespaceFromDB(table.Parent);
                                DirectoryInfo dir = new DirectoryInfo(checkBoxUseFilterFormsPath.Text);

                                WriteCSFile(dir, nscontroller, CodeProvider, tpFilterForm);
                                WriteCSFile(dir, nscontroller, CodeProvider, tpFilterFormBase);
                            }
                            else
                            {
                                nsForms.Types.Add(tpFilterForm);
                                nsForms.Types.Add(tpFilterFormBase);
                            }
                        }
                    }

                    #endregion
                    #region EntityForms
                    if (checkBoxEntityForms.Checked)
                    {
                        if (datclass == null)
                            datclass = DatClassGenerator.GenerateDatClass(table, out headerTable);

                        CodeTypeDeclaration tpEntityForm = ControllersGenerator.CreateEntityForm(datclass, table, helper);
                        if (checkBoxUseEntityFormsPath.Checked)
                        {
                            CodeNamespace nscontroller = dg.GetFormsNamespaceFromDB(db);
                            DirectoryInfo dir = new DirectoryInfo(checkBoxUseEntityFormsPath.Text);
                            WriteCSFile(dir, nscontroller, CodeProvider, tpEntityForm);
                        }
                        else
                            nsForms.Types.Add(tpEntityForm);
                    }
                    #endregion
                    #region DbClass
                    if (checkBoxDBClass.Checked)
                    {
                        CodeTypeDeclaration dbclass = dg.GenerateDBClass(db, checkBoxTrusted.Checked, txtUser.Text, txtPassword.Text);
                        if (checkBoxUseDBClassPath.Checked)
                        {
                            CodeNamespace nscontroller = dg.GetDatNamespaceFromDB(db);
                            DirectoryInfo dir = new DirectoryInfo(checkBoxUseDBClassPath.Text);
                            WriteCSFile(dir, nscontroller, CodeProvider, dbclass);
                        }
                        else
                        {
                            if (!onceIncluded)
                            {
                                nsDat.Types.Add(dbclass);
                                onceIncluded = true;
                            }
                        }

                    }

                    #endregion
                    #region DatClasses
                    if (checkBoxDatClasses.Checked)
                    {
                        if (datclass == null)
                            datclass = DatClassGenerator.GenerateDatClass(table, out headerTable);

                        CodeTypeDeclaration headerdat = null;
                        CodeTypeDeclaration headerset = null;
                        CodeTypeDeclaration detailset = null;

                        if (!string.IsNullOrEmpty(headerTable))
                        {
                            headerdat = dg.GenerateHeaderDatClass(db.Tables[headerTable].Name, table.Name);
                            headerset = dg.GenerateHeaderSetClass(db.Tables[headerTable].Name, table.Name);
                            detailset = dg.GenerateHeaderDetailsSetClass(db.Tables[headerTable].Name, table.Name);
                        }

                        if (checkBoxUseDatClassesPath.Checked)
                        {
                            CodeNamespace nscontroller = dg.GetDatNamespaceFromDB(db);
                            DirectoryInfo dir = new DirectoryInfo(checkBoxUseDatClassesPath.Text);

                            WriteCSFile(dir, nscontroller, CodeProvider, datclass);
                            if (headerdat != null)
                                WriteCSFile(dir, nscontroller, CodeProvider, headerdat);
                            if (headerset != null)
                                WriteCSFile(dir, nscontroller, CodeProvider, headerset);
                            if (detailset != null)
                                WriteCSFile(dir, nscontroller, CodeProvider, detailset);

                        }
                        else
                        {
                            nsDat.Types.Add(datclass);

                            if (headerdat != null)
                                nsDat.Types.Add(headerdat);
                            if (headerset != null)
                                nsDat.Types.Add(headerset);
                            if (detailset != null)
                                nsDat.Types.Add(detailset);
                        }
                    }
                    #endregion
                    #region Filters
                    if (checkBoxFilters.Checked)
                    {
                        CodeTypeDeclaration baseFilter = FiltersGenerator.CreateBaseFilterClass(table);
                        FiltersGenerator.BaseFilterClassAddProperties(table, ref baseFilter);
                        FiltersGenerator.BaseFilterClassAddMethods(table, ref baseFilter);
                        CodeTypeDeclaration filter = FiltersGenerator.CreateInheritedFilterClass(table);

                        if (checkBoxUseFiltersPath.Checked)
                        {
                            CodeNamespace nscontroller = dg.GetDatNamespaceFromDB(db);
                            DirectoryInfo dir = new DirectoryInfo(checkBoxUseFiltersPath.Text);
                            WriteCSFile(dir, nscontroller, CodeProvider, filter);
                            WriteCSFile(dir, nscontroller, CodeProvider, baseFilter);
                        }
                        else
                        {
                            nsDat.Types.Add(filter);
                            nsDat.Types.Add(baseFilter);
                        }
                    }
                    #endregion
                    #region SetClasse
                    if (checkBoxSetClasses.Checked)
                    {
                        CodeTypeDeclaration setclass = dg.GenerateSetClass(table);
                        if (checkBoxUseSetClassesPath.Checked)
                        {
                            CodeNamespace nscontroller = dg.GetDatNamespaceFromDB(db);
                            DirectoryInfo dir = new DirectoryInfo(checkBoxUseSetClassesPath.Text);
                            WriteCSFile(dir, nscontroller, CodeProvider, setclass);
                            if (!string.IsNullOrEmpty(headerTable))
                            {
                                CodeTypeDeclaration headerset = dg.GenerateHeaderSetClass(db.Tables[headerTable].Name, table.Name);
                                WriteCSFile(dir, nscontroller, CodeProvider, headerset);

                                CodeTypeDeclaration detailsset = dg.GenerateHeaderDetailsSetClass(db.Tables[headerTable].Name, table.Name);
                                WriteCSFile(dir, nscontroller, CodeProvider, detailsset);
                            }

                        }
                        nsDat.Types.Add(setclass);
                    }

                    #endregion
                    #region Columns
                    if (checkBoxColumns.Checked)
                    {
                        if (datclass == null)
                            datclass = DatClassGenerator.GenerateDatClass(table, out headerTable);
                        CodeTypeDeclaration columns = dg.GenerateColumnConsts(table, datclass);
                        if (checkBoxUseColumnsPath.Checked)
                        {
                            CodeNamespace nscontroller = dg.GetDatNamespaceFromDB(db);
                            DirectoryInfo dir = new DirectoryInfo(checkBoxUseColumnsPath.Text);
                            WriteCSFile(dir, nscontroller, CodeProvider, columns);
                        }
                        nsDat.Types.Add(columns);
                    }
                    #endregion


                    //if (checkBoxDatClasses.Checked || checkBoxFilters.Checked)
                    //{
                    //    txtGeneratedCode.Text = dg.GetCodeFromTables(dsTablesDict, db, checkBoxTrusted.Checked, txtUser.Text, txtPassword.Text, checkBoxDBClass.Checked, checkBoxDatClasses.Checked, checkBoxSetClasses.Checked, checkBoxFilters.Checked, checkBoxColumns.Checked);
                    //}


                }

                //dg.GetFormsFromTables(dsTablesDict, tbl.Parent, ref ns, checkBoxControllers.Checked, checkBoxFilterForms.Checked, checkBoxEntityForms.Checked, checkBoxMenu.Checked);




                if (nsDat.Types.Count > 0)
                    CodeProvider.GenerateCodeFromNamespace(nsDat, new StringWriter(sb), new CodeGeneratorOptions());

                if (nsForms.Types.Count > 0)
                    CodeProvider.GenerateCodeFromNamespace(nsForms, new StringWriter(sb), new CodeGeneratorOptions());

                txtGeneratedCode.Text = sb.ToString();



                if (checkBoxSQLClasses.Checked)
                {
                    Generator.SQLAssemblyGenerator dgsql = new Generator.SQLAssemblyGenerator();
                    //txtGeneratedCode.Text = dg.GetCodeFromNamespace(dg.GetNamespaceFromDataSet(ds));
                    txtGeneratedCode.Text = dgsql.GetCodeFromTables(dsTablesDict, "MSSQL." + smoServer.Name + "." + db.Name);
                }

                if (checkBoxMDB.Checked)
                {

                    dsGenerator = new DataSetGenerator(db);
                    DataSet ds = dsGenerator.CreateGlobalDS(dsTablesDict, true);

                    OleDbConnection _dbConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Projects\Pio\VS2006\PioAgentF\Resources\Data.template");
                    //OleDbConnection _dbConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Projects\VS2006\PioAgentF\Resources\Data.template");
                    _dbConnection.Open();

                    foreach (DataTable dt in ds.Tables)
                    {
                        Table tbs = (Table)dsTablesDict[dt.TableName];
                        string s = AccessTableCreationScript(tbs);
                        txtGeneratedCode.Text += "\r\n" + s;
                        try
                        {
                            OleDbCommand dropTable = new OleDbCommand("drop table " + dt.TableName, _dbConnection);
                            //dropTable.ExecuteNonQuery();
                        }
                        catch { }

                        foreach (string cPart in s.Split(';'))
                        {
                            if (cPart.Trim().Length > 0)
                            {
                                OleDbCommand createTable = new OleDbCommand(cPart, _dbConnection);
                                //createTable.ExecuteNonQuery();
                            }
                        }
                    }

                    foreach (DataRelation dr in ds.Relations)
                    {
                        //alter TABLE Exchanges alter column Operation_id int constraint FK_Ex_Inv references Operations
                        StringBuilder rel = new StringBuilder();
                        rel.AppendFormat("ALTER TABLE {0} ALTER COLUMN {1} int CONSTRAINT {2} REFERENCES {3}", dr.ChildTable.TableName, dr.ChildColumns[0].ColumnName, dr.RelationName, dr.ParentTable);

                        OleDbCommand createRelation = new OleDbCommand(rel.ToString(), _dbConnection);
                        txtGeneratedCode.Text += "\r\n" + rel.ToString();

                        //createRelation.ExecuteNonQuery();
                    }
                    _dbConnection.Close();
                }
            }


            this.Cursor = Cursors.Default;
            //}
            //catch (System.Exception Ex)
            //{
            //    this.Cursor = Cursors.Default;
            //    MessageBox.Show(Ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            txtGeneratedCode.Focus();
            txtGeneratedCode.SelectAll();
        }
        private void ClearCSFile(DirectoryInfo dir, CodeNamespace nscontroller, CodeTypeDeclaration classCode)
        {
            string filename = nscontroller.Name + "." + classCode.Name + ".cs";
            if (!dir.Exists)
                return;

            FileInfo fi = new FileInfo(dir.FullName + "\\" + filename);
            if (!fi.Exists)
                return;

            FileStream fs = new FileStream(dir.FullName + "\\" + filename, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            bool protectedCode = false;
            while (!sr.EndOfStream)
            {
                string s = sr.ReadToEnd();
                if (s.StartsWith("//--protect--"))
                    protectedCode = true;
            }

            if (!protectedCode)
            {
                StreamWriter sw = new StreamWriter(fs);
                fs.SetLength(0);
                sw.Close();
                fs.Close();
            }
        }
        private void WriteCSFile(DirectoryInfo dir, CodeNamespace nscontroller, CSharpCodeProvider CodeProvider, CodeTypeDeclaration classCode)
        {
            string filename = nscontroller.Name + "." + classCode.Name + ".cs";
            if (!dir.Exists)
                dir.Create();

            FileStream fs = new FileStream(dir.FullName + "\\" + filename, FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs);
            bool protectedCode = false;
            while (!sr.EndOfStream)
            {
                string s = sr.ReadToEnd();
                if (s.StartsWith("//--protect--"))
                    protectedCode = true;
            }

            if (!protectedCode)
            {
                StreamWriter sw = new StreamWriter(fs);

                nscontroller.Types.Add(classCode);
                fs.SetLength(0);
                CodeProvider.GenerateCodeFromNamespace(nscontroller, sw, new CodeGeneratorOptions());
                sw.Close();
                fs.Close();
            }
        }

        private void treeServerExplorer_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode tnodeSelected = (TreeNode)e.Node;

            Source ser = config.Servers.Find(delegate(Source s)
            {
                return s.ServerName.ToLower() == smbServers.Text.ToLower() && s.Database.ToLower() == tnodeSelected.Text.ToLower();
            });

            if (ser != null)
            {

                checkBoxUseControllersPath.Checked = ser.Controllers.Use;
                checkBoxUseFilterFormsPath.Checked = ser.FilterForms.Use;
                checkBoxUseEntityFormsPath.Checked = ser.EntityForms.Use;
                checkBoxUseDBClassPath.Checked = ser.DBClass.Use;
                checkBoxUseDatClassesPath.Checked = ser.DatClasses.Use;
                checkBoxUseSetClassesPath.Checked = ser.SetClasses.Use;
                checkBoxUseColumnsPath.Checked = ser.ColumnsClasses.Use;
                checkBoxUseFiltersPath.Checked = ser.FilterClasses.Use;

                checkBoxUseControllersPath.Text = ser.Controllers.Path;
                checkBoxUseFilterFormsPath.Text = ser.FilterForms.Path;
                checkBoxUseEntityFormsPath.Text = ser.EntityForms.Path;
                checkBoxUseDBClassPath.Text = ser.DBClass.Path;
                checkBoxUseDatClassesPath.Text = ser.DatClasses.Path;
                checkBoxUseSetClassesPath.Text = ser.SetClasses.Path;
                checkBoxUseColumnsPath.Text = ser.ColumnsClasses.Path;
                checkBoxUseFiltersPath.Text = ser.FilterClasses.Path;
            }


            //tnodeSelected.Name

            foreach (TreeNode tn in tnodeSelected.Nodes)
            {
                tn.Checked = tnodeSelected.Checked;
            }
        }

        private void buttonSQLDiff_Click(object sender, EventArgs e)
        {
            SQLDiffForm frm = new SQLDiffForm();
            frm.Show();
        }

        private void buttonCreateByTemplate_Click(object sender, EventArgs e)
        {
            //OleDbCommand comm;
            List<string> commands = null;

            if (radioButtonDict.Checked)
                commands = Generator.SQLTextGenerator.TemplateText(SQLTextGenerator.Template.DICTIONARY, textBoxTemplateName.Text, true);

            if (radioButtonTree.Checked)
                commands = Generator.SQLTextGenerator.TemplateText(SQLTextGenerator.Template.TREE_CARDS, textBoxTemplateName.Text, true);

            if (radioButtonHeaderDetails.Checked)
                commands = Generator.SQLTextGenerator.TemplateText(SQLTextGenerator.Template.HEADER_DETAILS, textBoxTemplateName.Text, true);


            if (commands != null)
                foreach (string s in commands)
                {

                    txtGeneratedCode.Text += s + "\r\nGO\r\n";
                    //comm = new OleDbCommand(s, _dbConnection);
                    //comm.ExecuteNonQuery();
                }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SetConfigValue(ServerName, smbServers.Text);
            //SetConfigValue(UserLogin, txtUser.Text);
            //SetConfigValue(UserPassword, txtPassword.Text);
            //SetConfigValue(IsTrusterConnection, checkBoxTrusted.Checked.ToString());
            //_config.Save();

        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = false;
            fd.DefaultExt = "sdf";
            if (fd.ShowDialog() == DialogResult.OK)
                textBoxSQLCEFileName.Text = fd.FileName;
        }

        private void buttonControllersPath_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(checkBoxUseControllersPath.Text);
            if (dir.Exists)
                fbd.SelectedPath = dir.FullName;
            else
                fbd.SelectedPath = "c:/";

            if (fbd.ShowDialog() == DialogResult.OK)
                checkBoxUseControllersPath.Text = fbd.SelectedPath;
        }

        private void buttonFilterFormsPath_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(checkBoxUseFilterFormsPath.Text);
            if (dir.Exists)
                fbd.SelectedPath = dir.FullName;
            else
                fbd.SelectedPath = "c:/";

            if (fbd.ShowDialog() == DialogResult.OK)
                checkBoxUseFilterFormsPath.Text = fbd.SelectedPath;
        }

        private void buttonEntityFormsPath_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(checkBoxUseEntityFormsPath.Text);
            if (dir.Exists)
                fbd.SelectedPath = dir.FullName;
            else
                fbd.SelectedPath = "c:/";

            if (fbd.ShowDialog() == DialogResult.OK)
                checkBoxUseEntityFormsPath.Text = fbd.SelectedPath;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //ServerList slist = new ServerList();
            //Source s = new Source();
            //s.ServerName = "aaa";
            //slist.Servers.Add(s);
            //slist.Servers.Add(s);
            //slist.Save(Application.StartupPath + @"\s.xml");

            config = ServerList.Load(Application.StartupPath + @"\servers.xml");
            if (config.Servers.Count > 0)
            {
                foreach (Source item in config.Servers)
                {
                    smbServers.Items.Add(item.ServerName);
                }
                smbServers.SelectedIndex = 0;
            }
        }

        private void smbServers_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void smbServers_TextChanged(object sender, EventArgs e)
        {
            Source ser = config.Servers.Find(delegate(Source s)
            {
                return s.ServerName.ToLower() == smbServers.Text.ToLower();
            });

            if (ser != null)
            {

                txtUser.Text = ser.UserLogin;

                txtPassword.Text = ser.UserPassword;
                checkBoxTrusted.Checked = ser.IsTrusterConnection;


            }
        }


    }

}
