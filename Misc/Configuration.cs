using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SNHU_CS499_SlavikPlamadyala
{
    public class Configuration
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
        public Configuration(Control _controlRef, int _dataLen, Type _dataType, int _offset, bool _saveInsideDB = true)
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
                return;
            }

            Buffer.BlockCopy(_data, offset, data, 0, dataLen);

            if (controlRef == null) return;

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
            if (data != null)
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
                value = Encoding.ASCII.GetString(data).Trim(new char[] { '\0', ' ' });
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
    }
}
