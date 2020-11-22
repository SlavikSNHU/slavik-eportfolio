using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SNHU_CS499_SlavikPlamadyala
{
    public partial class ConfigurationWindow : Form
    {
        private SQLServer sqlServer = new SQLServer();
        public class Configuraiton
        {
            private byte[] data;        // Store configuration value
            private Control controlRef; // Reference to control that may use stored value
            private int dataLen;        // Size of configuraiton value
            private Type dataType;      // Type of configuraiton value
            private int offset;         // Offset inside external device configuration structure buffer
            private bool saveInsideDB;  // Indicate if this configuraiton value should be stored inside database
            public bool SaveInsideDB => saveInsideDB;

            /// <summary>
            /// Constructor for custom configuraiton data structure
            /// </summary>
            /// <param name="_controlRef">Reference to control that may use stored value</param>
            /// <param name="_dataLen">Size of configuraiton value</param>
            /// <param name="_dataName">Name used by sql database</param>
            /// <param name="_dataType">Type of configuraiton value</param>
            /// <param name="_offset">Offset inside external device configuration structure buffer</param>
            /// <param name="_saveInsideDB">Indicate if this configuraiton value should be stored inside database</param>
            public Configuraiton(Control _controlRef, int _dataLen, Type _dataType, int _offset, bool _saveInsideDB = true)
            {
                controlRef = _controlRef;
                dataLen = _dataLen;
                dataType = _dataType;
                offset = _offset;
                saveInsideDB = _saveInsideDB;
                data = new byte[dataLen];

                // Check if control assigned
                if (controlRef == null) return;

                // Based on control type use different trigger events
                if (controlRef is TextBox)
                    controlRef.Validated += UpdateValue;
                else if (controlRef is ComboBox)
                    ((ComboBox)controlRef).SelectedIndexChanged += UpdateValue;

                // Set text box max length based on data type
                if (dataType == typeof(string))
                    ((TextBox)controlRef).MaxLength = 32;
                else if (dataType == typeof(uint))
                    ((TextBox)controlRef).MaxLength = 11;
                else if (dataType == typeof(int))
                    ((TextBox)controlRef).MaxLength = 11;
                else if (dataType == typeof(ushort))
                    ((TextBox)controlRef).MaxLength = 6;
                if (dataType == typeof(float))
                    ((TextBox)controlRef).MaxLength = 6;
                if (dataType == typeof(sbyte))
                    ((TextBox)controlRef).MaxLength = 4;
            }

            /// <summary>
            /// Update configuraiton value
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void UpdateValue(object sender, EventArgs e)
            {
                if (data == null) return;

                // Convert text to data type
                if (dataType == typeof(string))
                {
                    byte[] tempData = Encoding.ASCII.GetBytes(controlRef.Text);
                    if (tempData.Length > dataLen)
                        goto err;

                    data = new byte[dataLen]; // Clear buffer.
                    Buffer.BlockCopy(tempData, 0, data, 0, tempData.Length);
                }
                else if (dataType == typeof(uint))
                {
                    if (!uint.TryParse(controlRef.Text, out uint val))
                        goto err;

                    data = BitConverter.GetBytes(val);
                }
                else if (dataType == typeof(int))
                {
                    if (!int.TryParse(controlRef.Text, out int val))
                        goto err;

                    data = BitConverter.GetBytes(val);
                }
                else if (dataType == typeof(ushort))
                {
                    if (!ushort.TryParse(controlRef.Text, out ushort val))
                        goto err;

                    data = BitConverter.GetBytes(val);
                }
                else if (dataType == typeof(float))
                {
                    if (!float.TryParse(controlRef.Text, out float val))
                        goto err;

                    data = BitConverter.GetBytes(val);
                }
                else if (dataType == typeof(sbyte))
                {
                    if (!sbyte.TryParse(controlRef.Text, out sbyte val))
                        goto err;

                    data[0] = (byte)val;
                }
                else if (dataType == typeof(byte))
                {
                    if (!byte.TryParse(controlRef.Text, out byte val))
                        goto err;

                    data[0] = val;
                }
                else if (dataType == typeof(bool))
                {
                    if (((ComboBox)controlRef).SelectedIndex == 1)
                        data[0] = 1;
                    else
                        data[0] = 0;
                }
                return;
err:
                // Set value back to previous value
                SetData();
            }
            /// <summary>
            /// Based on configuraiton type conver byte array to value
            /// </summary>
            /// <param name="_data"></param>
            public void SetData(byte[] _data)
            {
                data = new byte[dataLen];
                if (offset + dataLen > _data.Length)
                {
                    Debug.WriteLine("ERROR: SetData _data lenght smaller than offset + length");
                    return;
                }

                Buffer.BlockCopy(_data, offset, data, 0, dataLen);

                if (controlRef == null) return;

                if(dataType == typeof(string))
                {
                    controlRef.Text = Encoding.Default.GetString(data);
                }
                else if (dataType == typeof(uint))
                {
                    controlRef.Text = BitConverter.ToUInt32(data, 0).ToString();
                }
                else if (dataType == typeof(int))
                {
                    controlRef.Text = BitConverter.ToInt32(data, 0).ToString();
                }
                else if (dataType == typeof(ushort))
                {
                    controlRef.Text = BitConverter.ToUInt16(data, 0).ToString();
                }
                else if (dataType == typeof(float))
                {
                    controlRef.Text = BitConverter.ToSingle(data, 0).ToString();
                }
                else if (dataType == typeof(sbyte))
                {
                    controlRef.Text = ((sbyte)data[0]).ToString();
                }
                else if (dataType == typeof(byte))
                {
                    controlRef.Text = data[0].ToString();
                }
                else if (dataType == typeof(bool))
                {
                    if (data[0] == 1)
                        ((ComboBox)controlRef).SelectedIndex = 1;
                    else
                        ((ComboBox)controlRef).SelectedIndex = 0;
                }
            }

            /// <summary>
            /// Based on configuraiton type populate assigned control with value
            /// </summary>
            public void SetData()
            {
                if (controlRef == null || data == null) return;

                if (dataType == typeof(string))
                {
                    controlRef.Text = Encoding.Default.GetString(data);
                }
                else if (dataType == typeof(uint))
                {
                    controlRef.Text = BitConverter.ToUInt32(data, 0).ToString();
                }
                else if (dataType == typeof(int))
                {
                    controlRef.Text = BitConverter.ToInt32(data, 0).ToString();
                }
                else if (dataType == typeof(ushort))
                {
                    controlRef.Text = BitConverter.ToUInt16(data, 0).ToString();
                }
                else if (dataType == typeof(float))
                {
                    controlRef.Text = BitConverter.ToSingle(data, 0).ToString();
                }
                else if (dataType == typeof(sbyte))
                {
                    controlRef.Text = ((sbyte)data[0]).ToString();
                }
                else if (dataType == typeof(byte))
                {
                    controlRef.Text = data[0].ToString();
                }
                else if (dataType == typeof(bool))
                {
                    if (data[0] == 1)
                        ((ComboBox)controlRef).SelectedIndex = 1;
                    else
                        ((ComboBox)controlRef).SelectedIndex = 0;
                }
            }
            /// <summary>
            /// Update current configuration value with new one
            /// </summary>
            /// <param name="newData"></param>
            public void UpdateData(byte[] newData)
            {
                if(data != null)
                    Buffer.BlockCopy(newData, 0, data, 0, dataLen);
            }
            /// <summary>
            /// Based on configuration type convert string value to byte array
            /// </summary>
            /// <param name="value"></param>
            public void UpdateData(string value)
            {
                if (dataType == typeof(string))
                {
                    byte[] newData = Encoding.ASCII.GetBytes(value);
                    Buffer.BlockCopy(newData, 0, data, 0, newData.Length);
                }
                else if (dataType == typeof(uint))
                {
                    byte[] newData = BitConverter.GetBytes(uint.Parse(value));
                    Buffer.BlockCopy(newData, 0, data, 0, dataLen);
                }
                else if (dataType == typeof(int))
                {
                    byte[] newData = BitConverter.GetBytes(int.Parse(value));
                    Buffer.BlockCopy(newData, 0, data, 0, dataLen);
                }
                else if (dataType == typeof(ushort))
                {
                    byte[] newData = BitConverter.GetBytes(ushort.Parse(value));
                    Buffer.BlockCopy(newData, 0, data, 0, dataLen);
                }
                else if (dataType == typeof(float))
                {
                    byte[] newData = BitConverter.GetBytes(float.Parse(value));
                    Buffer.BlockCopy(newData, 0, data, 0, dataLen);
                }
                else if (dataType == typeof(sbyte))
                {
                    byte[] newData = BitConverter.GetBytes(sbyte.Parse(value));
                    Buffer.BlockCopy(newData, 0, data, 0, dataLen);
                }
                else if (dataType == typeof(byte))
                {
                    byte[] newData = BitConverter.GetBytes(byte.Parse(value));
                    Buffer.BlockCopy(newData, 0, data, 0, dataLen);
                }
                else if (dataType == typeof(bool))
                {
                    if (int.Parse(value) == 1)
                        data[0] = 1;
                    else
                        data[0] = 0;
                }
            }
            /// <summary>
            ///  Based type get string value of configuration value
            /// </summary>
            /// <returns>String value of data</returns>
            public string GetStringData()
            {
                string value = "";

                if (dataType == typeof(string))
                {
                    value = Encoding.ASCII.GetString(data).Trim(new char[] {'\0', ' '});
                }
                else if (dataType == typeof(uint))
                {
                    value = BitConverter.ToUInt32(data, 0).ToString();
                }
                else if (dataType == typeof(int))
                {
                    value = BitConverter.ToInt32(data, 0).ToString();
                }
                else if (dataType == typeof(ushort))
                {
                    value = BitConverter.ToUInt16(data, 0).ToString();
                }
                else if (dataType == typeof(float))
                {
                    value = BitConverter.ToSingle(data, 0).ToString();
                }
                else if (dataType == typeof(sbyte))
                {
                    value = ((sbyte)data[0]).ToString();
                }
                else if (dataType == typeof(byte))
                {
                    value = data[0].ToString();
                }
                else if (dataType == typeof(bool))
                {
                    value = data[0].ToString();
                }
                return value;
            }
            /// <summary>
            /// Based on type get objet of properly converted data
            /// </summary>
            /// <returns></returns>
            public object GetDataValue()
            {
                object value = null;

                if (dataType == typeof(string))
                {
                    value = Encoding.Default.GetString(data);
                }
                else if (dataType == typeof(uint))
                {
                    value = BitConverter.ToUInt32(data, 0);
                }
                else if (dataType == typeof(int))
                {
                    value = BitConverter.ToInt32(data, 0);
                }
                else if (dataType == typeof(ushort))
                {
                    value = BitConverter.ToUInt16(data, 0);
                }
                if (dataType == typeof(float))
                {
                    value = BitConverter.ToSingle(data, 0);
                }
                if (dataType == typeof(byte))
                {
                    value = data[0];
                }
                if (dataType == typeof(sbyte))
                {
                    value = data[0];
                }
                else if (dataType == typeof(bool))
                {
                    value = data[0];
                }

                return value;
            }
            /// <summary>
            /// Store byte array values inside array
            /// </summary>
            /// <param name="dataList">Data byte list</param>
            public void GetData(ref List<byte> dataList)
            {
                foreach (byte b in data)
                    dataList.Add(b);
            }

            public bool StoreInisdeDB()
            {
                return false;
            }
        }
        private Dictionary<string, Configuraiton> mcConfiguraiton = new Dictionary<string, Configuraiton>();

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
            mcConfiguraiton.Add("baud_rate", new Configuraiton(txtMCBaud, 4, typeof(uint), offset));
            mcConfiguraiton.Add("temp_scale", new Configuraiton(cbtxtMCTempScale, 1, typeof(bool), offset += 4));
            mcConfiguraiton.Add("max_current", new Configuraiton(txtMCMaxCur, 4, typeof(float), offset += 1));
            mcConfiguraiton.Add("min_current", new Configuraiton(txtMCMinCur, 4, typeof(float), offset += 4));
            mcConfiguraiton.Add("min_efl", new Configuraiton(txtMCMinEfl, 2, typeof(ushort), offset += 4));
            mcConfiguraiton.Add("max_efl", new Configuraiton(txtMCMaxEfl, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_speed", new Configuraiton(txtMCZoomSpeed, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_near_limit", new Configuraiton(txtMCZoomNearSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_far_limit", new Configuraiton(txtMCZoomFarSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_opto_hardstop", new Configuraiton(txtMCZoomNearHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_far_hardstop", new Configuraiton(txtMCZoomFarHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_home", new Configuraiton(txtMCZoomHome, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("zoom_pid_p", new Configuraiton(txtMCZoomP, 4, typeof(int), offset += 2));
            mcConfiguraiton.Add("zoom_pid_i", new Configuraiton(txtMCZoomI, 4, typeof(int), offset += 4));
            mcConfiguraiton.Add("zoom_pid_d", new Configuraiton(txtMCZoomD, 4, typeof(int), offset += 4));
            mcConfiguraiton.Add("focus_speed", new Configuraiton(txtMCFocusSpeed, 2, typeof(ushort), offset += 4));
            mcConfiguraiton.Add("focus_near_limit", new Configuraiton(txtMCFocusNearSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_far_limit", new Configuraiton(txtMCFocusFarSoft, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_opto_hardstop", new Configuraiton(txtMCFocusNearHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_far_hardstop", new Configuraiton(txtMCFocusFarHard, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_home", new Configuraiton(txtMCFocusHome, 2, typeof(ushort), offset += 2));
            mcConfiguraiton.Add("focus_pid_p", new Configuraiton(txtMCFocusP, 4, typeof(int), offset += 2));
            mcConfiguraiton.Add("focus_pid_i", new Configuraiton(txtMCFocusI, 4, typeof(int), offset += 4));
            mcConfiguraiton.Add("focus_pid_d", new Configuraiton(txtMCFocusD, 4, typeof(int), offset += 4));
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
