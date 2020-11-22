using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SNHU_CS499_SlavikPlamadyala
{
    public partial class ConfigurationWindow : Form
    {
        private SQLServer sqlServer = new SQLServer();

        private Dictionary<string, Configuration> mcConfiguraiton = new Dictionary<string, Configuration>();

        private const string dbConnection = @"user id=na;server=na;Trusted_Connection=yes;database=na;connection timeout=1";
        private const string dbtModel = "dbo.Model";
        private const string dbtConfig = "dbo.Configuration";

        private const string dbColModelID = "modelID";
        private const string dbColModelNumber = "modelNumber";
        private const string dbColDeviceName = "deviceName";
        private const string dbColConfigName = "configurationName";
        private const string dbColConfigValue = "configurationValue";

        private const string deviceMC = "mtrController";

        private bool isDatabaseAvialable = true;

        public ConfigurationWindow()
        {
            InitializeComponent();
            InitializeMCControls();

            // Attempt to establish connection with SQL database
            isDatabaseAvialable = sqlServer.Connect(dbConnection);

            // Load list of all available model numbers from SQL database
            GetModelList();
            pnlMain.Focus();
        }

        #region Misc
        private void ClearTextBoxText(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if(child is Panel)
                    ClearTextBoxText(child);

                if (child is TextBox)
                    child.Text = "";
            }
        }
        private void ClearComboBox(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is Panel)
                    ClearComboBox(child);

                if (child is ComboBox)
                {
                    ComboBox cb = (ComboBox)child;
                    cb.SelectedIndex = -1;
                }
            }
        }
        private void SetTextBoxReadOnlyState(Control parent, bool state)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is Panel)
                    SetTextBoxReadOnlyState(child, state);

                if (child is TextBox)
                {
                    ((TextBox)child).ReadOnly = state;
                    if (state)
                        ((TextBox)child).BackColor = Color.FromArgb(47, 47, 47);
                    else
                        ((TextBox)child).BackColor = Color.SlateGray;
                }
            }
        }
        private void SetComboBoxColorState(Control parent, bool state)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is Panel)
                    SetComboBoxColorState(child, state);

                if (child is ComboBox)
                {
                    if (state)
                        ((ComboBox)child).BackColor = Color.FromArgb(47, 47, 47);
                    else
                        ((ComboBox)child).BackColor = Color.SlateGray;
                }
            }
        }
        private void PreventCBChange(object sender, MouseEventArgs e)
        {
            if (btnEdit.Text.Equals("EDIT"))
                pnlMain.Focus();
        }
        #endregion Misc

        #region Config Controls
        private void InitializeMCControls()
        {
            int offset = 0;
            // Initialize motor configuraiton data structure
            /*Arguments: 
                1) UI Control associated with data
                2) Data size in bytes
                3) Type of data
                4) Offset at which to read/write from/to external device */
            mcConfiguraiton.Add("baud_rate", new Configuration(txtMCBaud, 4, typeof(uint), offset));
            mcConfiguraiton.Add("temp_scale", new Configuration(cbtxtMCTempScale, 1, typeof(bool), offset += 4));
            mcConfiguraiton.Add("max_current", new Configuration(txtMCMaxCur, 4, typeof(float), offset += 1));
            mcConfiguraiton.Add("min_current", new Configuration(txtMCMinCur, 4, typeof(float), offset += 4));
            mcConfiguraiton.Add("min_efl", new Configuration(txtMCMinEfl, 2, typeof(ushort), offset += 4));
            mcConfiguraiton.Add("max_efl", new Configuration(txtMCMaxEfl, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_speed", new Configuration(txtMCZoomSpeed, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_near_limit", new Configuration(txtMCZoomNearSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_far_limit", new Configuration(txtMCZoomFarSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_opto_hardstop", new Configuration(txtMCZoomNearHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_far_hardstop", new Configuration(txtMCZoomFarHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_home", new Configuration(txtMCZoomHome, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_pid_p", new Configuration(txtMCZoomP, 4, typeof(int), offset += 2));
            mcConfiguraiton.Add("zoom_pid_i", new Configuration(txtMCZoomI, 4, typeof(int), offset += 4));
            mcConfiguraiton.Add("zoom_pid_d", new Configuration(txtMCZoomD, 4, typeof(int), offset += 4));
            mcConfiguraiton.Add("focus_speed", new Configuration(txtMCFocusSpeed, 2, typeof(ushort), offset += 4));
            mcConfiguraiton.Add("focus_near_limit", new Configuration(txtMCFocusNearSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_far_limit", new Configuration(txtMCFocusFarSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_opto_hardstop", new Configuration(txtMCFocusNearHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_far_hardstop", new Configuration(txtMCFocusFarHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_home", new Configuration(txtMCFocusHome, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_pid_p", new Configuration(txtMCFocusP, 4, typeof(int), offset += 2));
            mcConfiguraiton.Add("focus_pid_i", new Configuration(txtMCFocusI, 4, typeof(int), offset += 4));
            mcConfiguraiton.Add("focus_pid_d", new Configuration(txtMCFocusD, 4, typeof(int), offset += 4));
        }
        #endregion Config Controls 

        #region Database

        /// <summary>
        /// Get list of all available model numbers and place them inside combo box
        /// </summary>
        /// <returns>Load model list status</returns>
        private retStatus GetModelList()
        {
            // Check if connection to SQL server is available
            if (!isDatabaseAvialable || !sqlServer.IsConnected) return new retStatus(retStatus.ReturnCodes.ERR_SQL_DATABASE_CONNECTION);

            // Clear combo box with model numbers
            cbModel.Items.Clear();

            // Get table from sql server from model table
            retStatus status = sqlServer.GetDataTable(dbtModel, out DataTable dataTable);
            if (status.IsError) return status;

            // Add all model numbers inside combo box
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                // Go trough each row of the table and only input mode numbers for selected device
                if (dataTable.Rows[i][2].ToString().Equals(deviceMC))
                    cbModel.Items.Add(dataTable.Rows[i][1].ToString().Trim()); // Remove any spaces from number
            }

            // Check if any model numbers avaialbe
            if (cbModel.Items.Count > 0)
            {
                // Select first model number inside list
                cbModel.SelectedIndex = 0;
                LoadModelConfig();
            }

            return new retStatus();
        }

        /// <summary>
        /// Load all configuraiton information from SQL database for selected model number
        /// </summary>
        /// <returns>Load status</returns>
        private retStatus LoadModelConfig()
        {
            DataRow[] selectedConfig;

            // Check if connection to SQL server is available
            if (!isDatabaseAvialable || !sqlServer.IsConnected) return new retStatus(retStatus.ReturnCodes.ERR_SQL_DATABASE_CONNECTION);

            // Check if model number is selected. Less than zero index is set when nothing is available
            if (cbModel.SelectedIndex < 0) return new retStatus(retStatus.ReturnCodes.ERR_SQL_MODEL_NOT_SELECTED);

            // Get ID from sql server of selected model number
            retStatus status = sqlServer.GetRecordId(dbtModel, 1, cbModel.Text, out int id);
            if (status.RetCode != 0) return status; // Warnings are possible

            // Pull configuration table data from sql server for selcted ID number
            status = sqlServer.GetDataTableWithID(dbtConfig, dbColModelID, id, out DataTable dataTable);
            if (status.IsError) return status;

            // Store configuration data that is matching model ID
            selectedConfig = dataTable.Select($"{dbColModelID} = {id}");

            // Clear configuration controls
            ClearTextBoxText(pnlMotorConfig);
            ClearComboBox(pnlMotorConfig);

            // Go trough all configuraiton values and store them inside configruation data structure
            for (int i = 0; i < selectedConfig.Length; i++)
            {
                // Get configuration name at current index
                string configName = selectedConfig[i][1].ToString().Trim();

                // Check if configuraiton name from sql server exist inside configuration data structure
                if (!mcConfiguraiton.ContainsKey(configName)) continue;

                mcConfiguraiton[configName].UpdateData(selectedConfig[i][2].ToString().Trim());
                mcConfiguraiton[configName].SetData(); // Update UI
            }
            return status;
        }

        private void btnMCAddNew_Click(object sender, EventArgs e)
        {
            retStatus status = new retStatus();
            // Check if connection to SQL server is available
            if (!isDatabaseAvialable || !sqlServer.IsConnected) return;

            // make sure that current model does not exist already inside database
            status = sqlServer.GetRecordId(dbtModel, 1, cbModel.Text, out int id);
            if (status.RetCode == 0) return; // Already exist

            // create new model
            List<string> colName = new List<string> { dbColModelNumber, dbColDeviceName };
            List<string> colValue = new List<string> { cbModel.Text, deviceMC };
            status = sqlServer.AddNewRecord(dbtModel, colName, colValue);
            if (status.IsError)
            {
                MessageBox.Show($"Unable to add new record inside database. Error: {status.RetCode}");
                return;
            }

            status = sqlServer.GetLastId(dbtModel, out id);
            if (status.IsError)
            {
                MessageBox.Show($"Unable to get newly created ID from database. Error: {status.RetCode}");
                return;
            }

            // set configuraiton table column names
            colName = new List<string> { dbColModelID, dbColConfigName, dbColConfigValue };

            // Go through all configurations inside dictionary
            foreach(string configName in mcConfiguraiton.Keys)
            {
                // Check if configuraiton should be saved inside database
                if (!mcConfiguraiton[configName].SaveInsideDB) continue;

                // Store config name
                colValue = new List<string> { id.ToString(), configName, mcConfiguraiton[configName].GetStringData() };
                status = sqlServer.AddNewRecord(dbtConfig, colName, colValue);
                if (status.IsError)
                {
                    MessageBox.Show($"Unable to add new record inside database. Error: {status.RetCode}");
                    return;
                }
            }
            MessageBox.Show($"Added new configuraiton for model: {cbModel.Text} inside database");
        }
        private void btnMCRemove_Click(object sender, EventArgs e)
        {
            // Check if connection to SQL server is available
            if (!isDatabaseAvialable || !sqlServer.IsConnected) return;

            DialogResult result = MessageBox.Show($"Remove {cbModel.Text} From Database?", "Remove Model", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No) return;

            retStatus status = sqlServer.GetRecordId(dbtModel, 1, cbModel.Text, out int id);
            if (status.RetCode == retStatus.ReturnCodes.WARNING_SQL_UNABLE_TO_FIND_RECORD_ID)
            {
                MessageBox.Show($"Selected model number is not inside database.");
                return;
            }
            else if (status.IsError)
                return;

            status = sqlServer.DeleteRecord(dbtConfig, dbColModelID, id);
            if (status.IsError)
            {
                MessageBox.Show($"Unable remove record from database. Error: {status.RetCode}");
                return;
            }

            status = sqlServer.DeleteRecord(dbtModel, dbColModelID, id);
            if (status.IsError)
            {
                MessageBox.Show($"Unable remove record from database. Error: {status.RetCode}");
                return;
            }

            MessageBox.Show($"Removed model: {cbModel.Text} from database");

            cbModel.Items.RemoveAt(cbModel.Items.IndexOf(cbModel.Text));
            ClearTextBoxText(pnlMotorConfig);
            ClearComboBox(pnlMotorConfig);
        }
        private void btnMCUpdate_Click(object sender, EventArgs e)
        {
            // Check if connection to SQL server is available
            if (!isDatabaseAvialable || !sqlServer.IsConnected) return;

            retStatus status = sqlServer.GetRecordId(dbtModel, 1, cbModel.Text, out int id);
            if (status.RetCode == retStatus.ReturnCodes.WARNING_SQL_UNABLE_TO_FIND_RECORD_ID)
            {
                MessageBox.Show($"Selected model number is not inside database.");
                return;
            }
            else if (status.IsError)
                return;

            List<string> colName = new List<string> { dbColConfigValue };
            List<string> colValue;

            // Go through all configurations inside dictionary
            foreach (string configName in mcConfiguraiton.Keys)
            {
                // Check if configuraiton should be saved inside database
                if (!mcConfiguraiton[configName].SaveInsideDB) continue;

                // Update record
                status = sqlServer.UpdateRecord(dbtConfig, colName, new List<string> { mcConfiguraiton[configName].GetStringData() }, dbColModelID, id, dbColConfigName, configName);
                if (status.IsError)
                {
                    MessageBox.Show($"Unable to update values inside database. Error: {status.RetCode}");
                    return;
                }
            }

            MessageBox.Show($"Updated configuraiton for model: {cbModel.Text} inside database");
        }
        #endregion Database

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if(btnEdit.Text.Equals("EDIT"))
            {
                btnEdit.Text = "DONE";
                SetTextBoxReadOnlyState(pnlMotorConfig, false);
                SetComboBoxColorState(pnlMotorConfig, false);
                pnlMCEdit.Visible = true;

                cbModel.DropDownStyle = ComboBoxStyle.DropDown;
                cbModel.BackColor = Color.SlateGray;
            }
            else
            {
                btnEdit.Text = "EDIT";
                SetTextBoxReadOnlyState(pnlMotorConfig, true);
                SetComboBoxColorState(pnlMotorConfig, true);
                pnlMCEdit.Visible = false;
                
                string model = cbModel.Text;
                cbModel.DropDownStyle = ComboBoxStyle.DropDownList;
                cbModel.SelectedIndex = cbModel.Items.IndexOf(model);
                cbModel.BackColor = Color.FromArgb(47,47,47);
            }
            pnlMain.Focus();
        }
        private void cbModel_Validated(object sender, EventArgs e)
        {
            if (!cbModel.Items.Contains(cbModel.Text))
                cbModel.Items.Add(cbModel.Text);

            if (!mcConfiguraiton.ContainsKey("model")) return;
            mcConfiguraiton["model"].UpdateData(cbModel.Text);
        }
        private void cbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load selected model number configuraiton
            retStatus status = LoadModelConfig();
            if(status.RetCode != 0)
                MessageBox.Show("Unable to load selected model configuraiton from database");
            else
                MessageBox.Show($"Loaded configuraiton for model: {cbModel.Text}");
        }
    }
}
