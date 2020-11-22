using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SNHU_CS499_SlavikPlamadyala
{
    class SQLServer
    {
        private SqlConnection sqlDatabase;

        public bool Connect(string connString)
        {
            try
            {
                sqlDatabase = new SqlConnection(connString);
                sqlDatabase.Open();
                sqlDatabase.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsConnected => sqlDatabase.State == ConnectionState.Open;

        /// <summary>
        /// Send Non Query SQL command.
        /// </summary>
        /// <param name="command">SQL command</param>
        /// <returns>SQL Server connection status.</returns>
        public retStatus SendNonQCommand(string command)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(command, sqlDatabase))
                {
                    sqlDatabase.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlDatabase.Close();
                };
                return new retStatus();
            }
            catch (Exception ex)
            {
                sqlDatabase.Close();
                Debug.WriteLine($"SQL ERROR: {ex.Message}");
                return new retStatus(retStatus.ReturnCodes.ERR_SQL_BAD_QUERY);
            }
        }
        /// <summary>
        /// Send Non Query SQL command with unallowable"'/@" characters.
        /// </summary>
        /// <param name="command">SQL command.</param>
        /// <param name="add_with_format">String value to search for and replace with column values.</param>
        /// <param name="col_values">Column values</param>
        /// <returns></returns>
        public retStatus SendNonQCommand(string command, string add_with_format, List<string> col_values)
        {
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(command, sqlDatabase))
                {
                    for (int i = 0; i < col_values.Count; i++)
                        sqlCommand.Parameters.AddWithValue(add_with_format + i, col_values[i]);

                    sqlDatabase.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlDatabase.Close();
                };
                return new retStatus();
            }
            catch (Exception ex)
            {
                sqlDatabase.Close();
                Debug.WriteLine($"SQL ERROR: {ex.Message}");
                return new retStatus(retStatus.ReturnCodes.ERR_SQL_BAD_QUERY);
            }
        }

        /// <summary>
        /// Create new record inside selected database table and columns.
        /// </summary>
        /// <param name="table_name">SQL Database table name.</param>
        /// <param name="col_names">Selected table column names.</param>
        /// <param name="col_values">Selected column values.</param>
        /// <returns>New record creation status.</returns>
        public retStatus AddNewRecord(string table_name, List<string> col_names, List<string> col_values)
        {
            string full_col_names = "(";
            string full_col_values = "(";
            string sql_command = "";
            int i = 0;

            //combine column names and values in to proper sql command.
            for (i = 0; i < col_values.Count; i++)
            {
                //Check if array value is at the end of array
                if (i == (col_values.Count - 1))
                {
                    //last string structure
                    full_col_names += col_names[i] + ")";
                    full_col_values += "@t" + i + ")";
                }
                else
                {
                    //normal string structure
                    full_col_names += col_names[i] + ",";
                    full_col_values += "@t" + i + ",";
                }
            }

            sql_command = "INSERT INTO " + table_name + " " + full_col_names + " VALUES " + full_col_values;

            return SendNonQCommand(sql_command, "@t", col_values);
        }
        /// <summary>
        /// Update selected table record.
        /// </summary>
        /// <param name="table_name">SQL Database table name.</param>
        /// <param name="col_names">Selected table column names.</param>
        /// <param name="col_values">Selected column values.</param>
        /// <param name="id">ID of selected table record.</param>
        /// <returns>Record update status.</returns>
        public retStatus UpdateRecord(string table_name, List<string> col_names, List<string> col_values, string id_name, int id)
        {
            string sql_command = "";
            string command = "";
            int i = 0;

            //create string with values connected to columns.
            for (i = 0; i < col_values.Count; i++)
            {
                if (i == (col_values.Count - 1))
                    command += col_names[i] + " = @t" + i;
                else
                    command += col_names[i] + " = @t" + i + ",";
            }

            sql_command = "UPDATE " + table_name + " SET " + command + " WHERE " + id_name + " = " + id;

            return SendNonQCommand(sql_command, "@t", col_values);
        }
        /// <summary>
        /// Update selected table record.
        /// </summary>
        /// <param name="table_name">SQL Database table name.</param>
        /// <param name="col_names">Selected table column names.</param>
        /// <param name="col_values">Selected column values.</param>
        /// <param name="id">ID of selected table record.</param>
        /// <param name="col_name">second condition column name.</param>
        /// <param name="col_value">second condition value.</param>
        /// <returns>Record update status.</returns>
        public retStatus UpdateRecord(string table_name, List<string> col_names, List<string> col_values, string id_name, int id, string col_name, string col_value)
        {
            string sql_command = "";
            string command = "";
            int i = 0;

            //create string with values connected to columns.
            for (i = 0; i < col_values.Count; i++)
            {
                if (i == (col_values.Count - 1))
                    command += col_names[i] + " = @t" + i;
                else
                    command += col_names[i] + " = @t" + i + ",";
            }

            sql_command = "UPDATE " + table_name + " SET " + command + " WHERE " + id_name + " = " + id + " AND " + col_name + " = '" + col_value + "'";

            return SendNonQCommand(sql_command, "@t", col_values);
        }

        /// <summary>
        /// Delete selected ID record.
        /// </summary>
        /// <param name="table_name">SQL table name.</param>
        /// <param name="id">SQL record ID.</param>
        /// <returns>Record deletion status.</returns>
        public retStatus DeleteRecord(string table_name, string id_name, int id)
        {
            string sql_command = "DELETE FROM " + table_name + " WHERE " + id_name + " = " + id;
            return SendNonQCommand(sql_command);
        }

        /// <summary>
        /// Extract all columns and rows of selected table.
        /// </summary>
        /// <param name="table_name">SQL table name.</param>
        /// <param name="dataTable">Store selected SQL Server Table data inside this object.</param>
        /// <returns>SQL data transfer status</returns>
        public retStatus GetDataTable(string table_name, out DataTable dataTable)
        {
            dataTable = new DataTable();
            string sql_command = "SELECT * FROM " + table_name;
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(sql_command, sqlDatabase))
                {
                    sqlDatabase.Open();
                    var dataAdapter = new SqlDataAdapter(sqlCommand);
                    dataAdapter.Fill(dataTable);
                    sqlDatabase.Close();
                }

                return new retStatus();
            }
            catch
            {
                sqlDatabase.Close();
                return new retStatus(retStatus.ReturnCodes.ERR_SQL_GET_DATA_TABLE);
            }
        }
        /// <summary>
        /// Gather data table for SQL databse of selected table based on column name and id value
        /// </summary>
        /// <param name="table_name">Table name</param>
        /// <param name="idColName">ID column name</param>
        /// <param name="id">ID search for</param>
        /// <param name="dataTable">Store selected SQL Server Table data inside this object</param>
        /// <returns>SQL data transfer status</returns>
        public retStatus GetDataTableWithID(string table_name, string idColName, int id, out DataTable dataTable)
        {
            dataTable = new DataTable();
            string sql_command = $"SELECT * FROM {table_name} WHERE {idColName}={id}";
            try
            {
                using (SqlCommand sqlCommand = new SqlCommand(sql_command, sqlDatabase))
                {
                    sqlDatabase.Open();
                    var dataAdapter = new SqlDataAdapter(sqlCommand);
                    dataAdapter.Fill(dataTable);
                    sqlDatabase.Close();
                }

                return new retStatus();
            }
            catch
            {
                sqlDatabase.Close();
                return new retStatus(retStatus.ReturnCodes.ERR_SQL_GET_DATA_TABLE);
            }
        }
        /// <summary>
        /// Get last ID inside selected table.
        /// </summary>
        /// <param name="table_name">Selected table name.</param>
        /// <param name="Id">Selected record ID.</param>
        /// <returns>SQL Query status</returns>
        public retStatus GetLastId(string table_name, out int Id)
        {
            Id = -1;

            retStatus status = GetDataTable(table_name, out DataTable dataTable);
            if (status.IsError) return status;

            if (dataTable.Rows.Count != 0)
                Id = (int)dataTable.Rows[dataTable.Rows.Count - 1][0];
            else
                return new retStatus(retStatus.ReturnCodes.ERR_SQL_BAD_QUERY);

            return status;
        }

        /// <summary>
        /// Get selected record ID.
        /// </summary>
        public retStatus GetRecordId(string table_name, int column, string value, out int id)
        {
            id = -1;
            string dbValue;

            retStatus status = GetDataTable(table_name, out DataTable dataTable);
            if (status.IsError) return status;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dbValue = Regex.Replace(dataTable.Rows[i][1].ToString(), @"\s+", "");
                if (dbValue.Equals(value))
                {
                    if (!int.TryParse(dataTable.Rows[i][0].ToString(), out id)) return new retStatus(retStatus.ReturnCodes.ERR_PARSING_DATA);
                    return status;
                }
            }

            return new retStatus(retStatus.ReturnCodes.WARNING_SQL_UNABLE_TO_FIND_RECORD_ID);
        }

        public bool IdExists(string table_name, int id)
        {
            string dbValue;

            retStatus status = GetDataTable(table_name, out DataTable dataTable);
            if (status.IsError) return false;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dbValue = Regex.Replace(dataTable.Rows[i][0].ToString(), @"\s+", "");
                if (dbValue.Equals(id.ToString()))
                    return true;
            }
            return false;
        }
    }
}
