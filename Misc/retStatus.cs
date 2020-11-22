using System;

namespace SNHU_CS499_SlavikPlamadyala
{
    public class retStatus
    {
        public enum ReturnCodes
        {
            OK = 0,

            // General errors
            ERR_UNSUPORTED = -1,
            ERR_BUFFER_NON_NUMERIC = -2,
            ERR_BUFFER_SIZE = -3,
            ERR_UNKNOWN_TYPE = -4,
            ERR_UNSUPPORTED_SWITCH_STATEMENT = -5,
            ERR_PARSING_DATA = -6,
            ERR_COM_PROTOCOL_NOT_SELECTED = -7,
            ERR_NOT_CONNECTED = -8,
            ERR_UNSUPPORTED_DEVICE = -9,
            ERR_FUNCTION_NOT_IMPLEMENTED = -10,
            ERR_UNABLE_TO_CONVERT_TEXT = -11,
            ERR_BAD_FILEPATH = -12,
            ERR_UNABLE_CREATE_FILE = -13,
            ERR_FILEPATH_INVALID = -14,
            ERR_PARSE_FILE = -15,

            // RS232 Communicaiton
            ERR_RS232_UNABLE_TO_CONNECT = -400,
            ERR_RS232_PORT_NOT_OPEN = -401,
            ERR_RS232_WRONG_PARAMETERS = -402,
            ERR_RS232_WRITE_BYTES = -403,
            ERR_RS232_READ_BYTES = -404,
            ERR_RS232_READ_BYTES_SIZE = -405,
            ERR_RS232_COM_PORT_UNAVAILABLE = -406,

            // Ethernet
            ERR_ETHERNET_UNABLE_TO_CONNECT = -500,
            ERR_ETHERNET_TCP_SEND_FAILURE = -501,
            ERR_ETHERNET_TCP_READ_FAILURE = -502,
            ERR_ETHERNET_UNSUPPORTED_PROTOCOL = -503,
            ERR_ETHERNET_TCP_READ_BYTE_FAILURE = -504,
            ERR_ETHERNET_TCP_NOT_CONNECTED = -505,
            ERR_ETHERNET_READ_AMOUNT = -506,

            // Video Stream
            ERR_VIDEO_STREAM_UNABLE_TO_PLAY = -600,

            // XML
            ERR_XML_BAD_PARENT = -2000,
            ERR_XML_BAD_ATTRIBUTE = -2001,
            ERR_XML_BAD_ELEMENT = -2002,
            ERR_XML_NOT_LOADED = -2003,
            ERR_XML_XELEMENT_MISSING = -2004,

            // SQL Database
            ERR_SQL_DATABASE_CONNECTION = -2200,
            ERR_SQL_GET_DATA_TABLE = -2201,
            ERR_SQL_GET_LAST_ID = -2202,
            ERR_SQL_BAD_QUERY = -2203,
            ERR_SQL_MODEL_NOT_SELECTED = -2204,
            ERR_SQL_MODEL_DOES_NOT_EXIST = -2205,

            WARNING_SQL_UNABLE_TO_FIND_RECORD_ID = 2200,
        }

        private ReturnCodes retCode;
        private Exception exception;
        public ReturnCodes RetCode
        {
            get => retCode;
            set
            {
                retCode = value;
                SetText();
            }
        }
        public bool IsError => RetCode < 0;
        public Exception Exception { get => exception; }
        private string errorDescription { get; set; }
        public string ErrorDescription => errorDescription;

        public retStatus()
        {
            retCode = ReturnCodes.OK;
            exception = new Exception("No Error Description");
        }
        public retStatus(ReturnCodes _retCode)
        {
            retCode = _retCode;
            exception = new Exception("No Error Description");
        }
        public retStatus(ReturnCodes _retCode, Exception _exception)
        {
            retCode = _retCode;
            exception = _exception;
        }

        private void SetText()
        {
            switch (retCode)
            {
                case ReturnCodes.ERR_UNSUPORTED:
                    errorDescription = "Error Unknown";
                    break;

                // RS232 Error Description
                case ReturnCodes.ERR_RS232_UNABLE_TO_CONNECT:
                    errorDescription = "Unable to establish communication using RS232";
                    break;
                case ReturnCodes.ERR_RS232_PORT_NOT_OPEN:
                    errorDescription = "RS232 Communication port is not open";
                    break;
                case ReturnCodes.ERR_RS232_WRITE_BYTES:
                    errorDescription = "Unable to write bytes over RS232";
                    break;
                case ReturnCodes.ERR_RS232_WRONG_PARAMETERS:
                    errorDescription = "Wrong serial com or baur rate selected for RS232";
                    break;
                case ReturnCodes.ERR_RS232_READ_BYTES:
                    errorDescription = "Unable to write bytes over RS232";
                    break;
                case ReturnCodes.ERR_BUFFER_NON_NUMERIC:
                    errorDescription = "RS232 read buffer value is not numeric";
                    break;

                case ReturnCodes.ERR_VIDEO_STREAM_UNABLE_TO_PLAY:
                    errorDescription = "Video stream unable to play";
                    break;

                default:
                    if (retCode > 0) errorDescription = $"Undefined Status [{retCode}]";
                    else errorDescription = $"Undefined Error [{retCode}]";
                    break;
            }
        }
    }
}
