using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Method_Stub_Generator
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> _stubNames = new Dictionary<string, string>();
        public Form1()
        {
            InitializeComponent();
        }

        private SqlDbType GetSqlDbType(string inputStr)
        {
            var type = (SqlDbType)Enum.Parse(typeof(SqlDbType), inputStr, true);
            return type;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            codeOutput.Clear();
            _stubNames.Clear();
            if (string.IsNullOrEmpty(txtConnString.Text))
            {
                ShowErrMsg("DB Connection string should not be empty.");
            }
            var dt = ReadParameters(txtStoreProc.Text.Trim());
            StringBuilder strDataObj = new StringBuilder();

            var className = string.IsNullOrEmpty(txtControllerName.Text.Trim()) ? "YourClass" : txtControllerName.Text.Trim();
            var classNameAsParam = className[0].ToString().ToLower() +
                                   className.Substring(1, className.Length - 1);
            _stubNames.Add("classname", className);
            _stubNames.Add("classnameasparam", classNameAsParam);

            if (cgOption1.Checked)
            {
                strDataObj.Append(GenerateCodeForDALService(dt));
            }
            else
            {
                strDataObj.Append(GenerateCodeForBLService(dt));
            }

            codeOutput.Text = strDataObj.ToString();
        }

        private string GenerateCodeForBLService(DataTable dt)
        {
            StringBuilder strDataObj = new StringBuilder();

            if (dt.Rows.Count > 1)
            {
                strDataObj.AppendLine("Library.DataObjects Class:");
                strDataObj.AppendLine("----------------------------------");
                strDataObj.Append(GenerateDataObject(dt, txtStoreProc.Text.Trim()));

                var dataObjNameForMethod = _stubNames["dataobjname"];
                _stubNames.Add("sqllibmethodname", dataObjNameForMethod + "Async");

                var dataObjNameForMethodParam = dataObjNameForMethod[0].ToString().ToLower() +
                                                dataObjNameForMethod.Substring(1, dataObjNameForMethod.Length - 1);

                _stubNames.Add("dataobjnameasparam", dataObjNameForMethodParam);
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("BL Library Class Code:");
                strDataObj.AppendLine("------------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetBL_Lib_Method_FilePath()));
                strDataObj.Append(Environment.NewLine);

                GenerateDataObjForUtcUse(dt, dataObjNameForMethod, dataObjNameForMethodParam);

                string emptyDataObj = $"var {dataObjNameForMethodParam} = {dataObjNameForMethod}();";
                _stubNames.Add("emptydataobjforutc", emptyDataObj);

                strDataObj.AppendLine("BL Library Class UTC Code:");
                strDataObj.AppendLine("-----------------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetBL_LibMethod_UTC_DataObj_FilePath()));
                strDataObj.Append(Environment.NewLine);

                var inputDataMsg =
                    $"var {dataObjNameForMethodParam} = JsonConvert.DeserializeObject<{dataObjNameForMethod}>(reqDataMessage.Parameters[ParametersDicKey], JsonSerializeSettings());";

                _stubNames.Add("inputdata4mmessage", inputDataMsg);

                strDataObj.AppendLine("BL Controller Class Code:");
                strDataObj.AppendLine("-----------------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetBL_Controller_Method_FilePath()));
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("BL Controller Class UTC Code:");
                strDataObj.AppendLine("--------------------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetBL_Controller_UTC_FilePath()));
                strDataObj.Append(Environment.NewLine);
            }
            else if (dt.Rows.Count == 1)
            {
                var type = TypeConverter.ToClrType(GetSqlDbType(dt.Rows[0]["Data_Type"].ToString()), false);
                _stubNames.Add("dataobjname", type);
                var dataObjNameForMethod = ChangeVarName(txtStoreProc.Text.Trim()) + "Details";

                _stubNames.Add("sqllibmethodname", dataObjNameForMethod + "Async");

                var sqlParam = ChangeVarName(dt.Rows[0]["Parameter_Name"].ToString());
                var sqlParamForMethodParam = sqlParam[0].ToString().ToLower() +
                                             sqlParam.Substring(1, sqlParam.Length - 1);

                _stubNames.Add("dataobjnameasparam", sqlParamForMethodParam);

                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("BL Library Class Code:");
                strDataObj.AppendLine("-----------------------------------" + Environment.NewLine);
                strDataObj.Append(GenerateCode(GetBL_Lib_Method_1Param_FilePath()));
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("BL Library Class UTC Code:");
                strDataObj.AppendLine("-----------------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetBL_LibMethod_UTC_1Param_FilePath()));
                strDataObj.Append(Environment.NewLine);

                if (type == "string")
                {
                    var inputDataMsg =
                        $"var {sqlParamForMethodParam} = Convert.ToString(reqDataMessage.Parameters[ParametersDicKey])";

                    _stubNames.Add("inputdata4mmessage", inputDataMsg);
                }
                else if (type == "int")
                {
                    var inputDataMsg =
                        $"var {sqlParamForMethodParam} = JsonConvert.DeserializeObject<int>(reqDataMessage.Parameters[ParametersDicKey]);";
                    _stubNames.Add("inputdata4mmessage", inputDataMsg);
                }

                strDataObj.AppendLine("BL Controller Class Code:");
                strDataObj.AppendLine("-----------------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetBL_Controller_Method_FilePath()));
                strDataObj.Append(Environment.NewLine);

                var dataObjUtc = "reqMessage.Parameters.Add(ParametersDicKeyVal, \"7\");";
                _stubNames.Add("dataobjforutc", dataObjUtc);

                strDataObj.AppendLine("BL Controller Class UTC Code:");
                strDataObj.AppendLine("--------------------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetBL_Controller_UTC_FilePath()));
                strDataObj.Append(Environment.NewLine);
            }

            return strDataObj.ToString();
        }

        private string GenerateCodeForDALService(DataTable dt)
        {
            StringBuilder strDataObj = new StringBuilder();
            if (dt.Rows.Count > 1)
            {
                strDataObj.AppendLine("Library.DataObjects Class:");
                strDataObj.AppendLine("---------------------------");
                strDataObj.Append(GenerateDataObject(dt, txtStoreProc.Text.Trim()));

                var dataObjNameForMethod = _stubNames["dataobjname"];
                _stubNames.Add("sqllibmethodname", dataObjNameForMethod + "Async");

                var dataObjNameForMethodParam = dataObjNameForMethod[0].ToString().ToLower() +
                                                dataObjNameForMethod.Substring(1, dataObjNameForMethod.Length - 1);

                _stubNames.Add("dataobjnameasparam", dataObjNameForMethodParam);
                SqlParamContent(dt, dataObjNameForMethodParam);

                strDataObj.Append(Environment.NewLine);
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("Sql Storage Class Code:");
                strDataObj.AppendLine("------------------------" + Environment.NewLine);
                strDataObj.Append(GenerateCode(GetSqlLibMethodFilePath()));

                GenerateDataObjForUtcUse(dt, dataObjNameForMethod, dataObjNameForMethodParam);
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("Sql Storage Class UTC Code:");
                strDataObj.AppendLine("----------------------------" + Environment.NewLine);
                strDataObj.Append(GenerateCode(GetSqlLibMethodUTCFilePath()));
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("Sql Controller Class Code:");
                strDataObj.AppendLine("----------------------------" + Environment.NewLine);

                strDataObj.Append(GenerateCode(GetDAL_Controller_Method_FilePath()));
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("Sql Controller Class UTC Code:");
                strDataObj.AppendLine("-------------------------------" + Environment.NewLine);
                strDataObj.Append(GenerateCode(GetDAL_Controller_Utc_DataObj_FilePath()));
                strDataObj.Append(Environment.NewLine);
            }
            else if (dt.Rows.Count == 1)
            {
                var type = TypeConverter.ToClrType(GetSqlDbType(dt.Rows[0]["Data_Type"].ToString()), false);
                _stubNames.Add("dataobjname", type);
                var dataObjNameForMethod = ChangeVarName(txtStoreProc.Text.Trim()) + "Details";

                _stubNames.Add("sqllibmethodname", dataObjNameForMethod + "Async");

                var sqlParam = ChangeVarName(dt.Rows[0]["Parameter_Name"].ToString());
                var sqlParamForMethodParam = sqlParam[0].ToString().ToLower() +
                                                sqlParam.Substring(1, sqlParam.Length - 1);

                _stubNames.Add("dataobjnameasparam", sqlParamForMethodParam);
                SqlParamContent(dt, sqlParamForMethodParam);

                strDataObj.Append(Environment.NewLine);
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("Sql Storage Class Code:");
                strDataObj.AppendLine("------------------------" + Environment.NewLine);
                strDataObj.Append(GenerateCode(GetSqlLibMethodFilePath()));

                strDataObj.AppendLine("Sql Storage Class UTC Code:");
                strDataObj.AppendLine("----------------------------" + Environment.NewLine);

                if (type == "string")
                {
                    Generate_String_ForUtcUse(dt);
                    strDataObj.Append(GenerateCode(GetSqlLibMethodUTCFilePath()));
                }
                else if (type == "int")
                {
                    Generate_Int_ForUtcUse(dt);
                    strDataObj.Append(GenerateCode(GetSqlLibMethodUTC_Int_1Param_FilePath()));
                }
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("Sql Controller Class Code:");
                strDataObj.AppendLine("----------------------------" + Environment.NewLine);
                strDataObj.Append(GenerateCode(GetDAL_Controller_1Param_Method_FilePath()));
                strDataObj.Append(Environment.NewLine);

                strDataObj.AppendLine("Sql Controller Class UTC Code:");
                strDataObj.AppendLine("------------------------------" + Environment.NewLine);

                if (type == "string")
                {
                    strDataObj.Append(GenerateCode(GetDAL_Controller_Utc_DataObj_FilePath()));
                }
                else if (type == "int")
                {
                    strDataObj.Append(GenerateCode(GetDAL_Controller_Utc_Int_1Param_FilePath()));
                }
            }

            return strDataObj.ToString();
        }

        private void ShowErrMsg(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private string GetBL_Controller_Method_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\BL_Controller_Method.txt");
            return path;
        }

        private string GetBL_Controller_UTC_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\BL_Controller_Utc.txt");
            return path;
        }

        private string GetBL_LibMethod_UTC_1Param_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\BL_Library_Utc_1Param.txt");
            return path;
        }

        private string GetBL_LibMethod_UTC_DataObj_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\BL_Library_Utc_DataObj.txt");
            return path;
        }

        private string GetBL_Lib_Method_1Param_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\BL_LibMethod_1Param.txt");
            return path;
        }

        private string GetBL_Lib_Method_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\BL_LibMethod.txt");
            return path;
        }

        private string GetDAL_Controller_Utc_Int_1Param_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\DALController_Utc_Int_1Param.txt");
            return path;
        }

        private string GetDAL_Controller_Utc_DataObj_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\DALController_Utc_DataObj.txt");
            return path;
        }

        private string GetDAL_Controller_1Param_Method_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\DALController_Method_1Param.txt");
            return path;
        }

        private string GetSqlLibMethodFilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\SqlLibMethod.txt");
            return path;
        }

        private string GetSqlLibMethodUTCFilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\SqlLibMethod_Utc_DataObj.txt");
            return path;
        }

        private string GetSqlLibMethodUTC_Int_1Param_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\SqlLibMethod_Utc_int_1Param.txt");
            return path;
        }

        private string GetDAL_Controller_Method_FilePath()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Templates\DALController_Method.txt");
            return path;
        }

        private string GenerateCode(string path)
        {
            string[] fileLines = File.ReadAllLines(path);

            var sqlLibMethod = new StringBuilder();

            foreach (var singleLine in fileLines)
            {
                sqlLibMethod.AppendLine(FillPlaceHolders(singleLine));
            }

            return sqlLibMethod.ToString();
        }

        private string SqlParamContent(DataTable dt, string dataObjParamName)
        {
            StringBuilder paramContent = new StringBuilder();
            paramContent.Append("       var sqlParameters = new List<SqlParameter>" +
                               Environment.NewLine + "{" + Environment.NewLine);
            if (dt.Rows.Count > 1)
            {
                foreach (DataRow row in dt.Rows)
                {
                    paramContent.AppendFormat("         new SqlParameter(\"{0}\", {1}.{2}),",
                        row["Parameter_Name"].ToString(), dataObjParamName,
                        ChangeVarName(row["Parameter_Name"].ToString()));
                    paramContent.Append(Environment.NewLine);
                }
            }
            else if (dt.Rows.Count == 1)
            {
                paramContent.AppendFormat("         new SqlParameter(\"{0}\", {1}),",
                    dt.Rows[0]["Parameter_Name"].ToString(), dataObjParamName);
                paramContent.Append(Environment.NewLine);
            }

            StringBuilder daLibMethod = new StringBuilder();
            daLibMethod.Append(paramContent.ToString().Substring(0, paramContent.ToString().Length - 3));
            daLibMethod.Append(Environment.NewLine + "};" + Environment.NewLine);

            var content = daLibMethod.ToString();
            _stubNames.Add("sqlparamcontent", content);

            return content;
        }

        private string FillPlaceHolders(string line)
        {
            var lstLocations = AllIndexesOfHash(line);
            if (lstLocations.Count % 2 != 0)
                return line;
            string copiedLine = line;
            for (int location = 0; location < lstLocations.Count; location = location + 2)
            {
                var replaceKey = line.Substring(lstLocations[location],
                     lstLocations[location + 1] - lstLocations[location] + 1);

                copiedLine = copiedLine.Replace(replaceKey, _stubNames[replaceKey.Trim('#').ToLower()]);
            }

            return copiedLine;
        }

        private List<int> AllIndexesOfHash(string str)
        {
            string value = "#";
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        private string GenerateDataObject(DataTable dt, string spName)
        {
            var sb = new StringBuilder();

            var dataObjName = ChangeVarName(spName) + "Details";
            _stubNames.Add("dataobjname", dataObjName);

            sb.AppendFormat("public class {0}", dataObjName);
            sb.Append(Environment.NewLine + "{" + Environment.NewLine);
            foreach (DataRow row in dt.Rows)
            {
                var isNull = row["Character_Maximum_Length"].ToString() == string.Empty;
                var type = TypeConverter.ToClrType(GetSqlDbType(row["Data_Type"].ToString()), isNull);

                sb.AppendFormat("public {0} {1}", type, ChangeVarName(row["Parameter_Name"].ToString()));
                sb.Append(" { get; set; }");
                sb.Append(Environment.NewLine);
            }
            sb.Append("}" + Environment.NewLine);

            return sb.ToString();
        }

        private string ChangeVarName(string sqlParamName)
        {
            if (sqlParamName.StartsWith("@"))
            {
                sqlParamName = sqlParamName.Substring(1, sqlParamName.Length - 1);
            }
            string modifiedVar = string.Empty;
            var frags = sqlParamName.Split('_');
            for (int i = 0; i < frags.Length; i++)
            {
                modifiedVar += frags[i][0].ToString().ToUpper() + frags[i].Substring(1, frags[i].Length - 1).ToLower();
            }
            return modifiedVar;
        }

        #region old code
        private string GenerateDALibMethod(DataTable dt)
        {
            StringBuilder daLibMethod = new StringBuilder();

            var dataObjNameForMethod = _stubNames["DataObjName"];
            _stubNames.Add("SqlLibMethodName", dataObjNameForMethod + "Async");

            var dataObjNameForMethodParam = dataObjNameForMethod[0].ToString().ToLower() +
                                            dataObjNameForMethod.Substring(1, dataObjNameForMethod.Length - 1);
            daLibMethod.AppendFormat("public async Task<string> {0}Async({0} {1}, string connectionString, string sqlCommandText, string signalRClientId = \"\")", dataObjNameForMethod, dataObjNameForMethodParam);
            daLibMethod.Append(Environment.NewLine + "{" + Environment.NewLine);

            daLibMethod.AppendFormat("if ({0} == null) ", dataObjNameForMethodParam);
            daLibMethod.Append(Environment.NewLine + "{" + Environment.NewLine);
            daLibMethod.AppendFormat("throw new ArgumentException(nameof({0}));", dataObjNameForMethodParam);
            daLibMethod.Append(Environment.NewLine + "}" + Environment.NewLine);

            daLibMethod.Append("ValidateConnectionInputs(connectionString, sqlCommandText);" + Environment.NewLine);

            daLibMethod.Append("try" + Environment.NewLine + "{" + Environment.NewLine + "var sqlParameters = new List<SqlParameter>" +
                          Environment.NewLine + "{" + Environment.NewLine);

            StringBuilder paramContent = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                paramContent.AppendFormat("new SqlParameter(\"{0}\", {1}.{2}),", row["Parameter_Name"].ToString(), dataObjNameForMethodParam, ChangeVarName(row["Parameter_Name"].ToString()));
                paramContent.Append(Environment.NewLine);
            }

            daLibMethod.Append(paramContent.ToString().Substring(0, paramContent.ToString().Length - 3));
            daLibMethod.Append(Environment.NewLine);
            var newLine1 = @"};

                var jsonString = await _common.ExecuteReaderAsync(connectionString, sqlCommandText, sqlParameters);
                return jsonString;
            }
            catch (Exception ex)
            {
                _errorPublishLibrary.ReportError(ex, Assembly.GetExecutingAssembly().ToString(),
                    ex.GetType().ToString()," +

                   " \"Error\", Environment.UserName, signalRClientId);" +
                " throw; }" +
                           "}";
            daLibMethod.Append(newLine1);

            return daLibMethod.ToString();
        }

        private string GenerateDALibMethodUTCs(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtcs = new StringBuilder();

            //libMethodUtcs.Append(GenerateDaLibTestCase1Method(methodName, dt));
            //libMethodUtcs.Append(Environment.NewLine);
            //libMethodUtcs.Append(GenerateDaLibTestCase2Method(methodName, dt));
            //libMethodUtcs.Append(Environment.NewLine);
            //libMethodUtcs.Append(GenerateDaLibTestCase3Method(methodName, dt));
            //libMethodUtcs.Append(Environment.NewLine);
            //libMethodUtcs.Append(GenerateDaLibTestCase4Method(methodName, dt));
            //libMethodUtcs.Append(Environment.NewLine);



            return libMethodUtcs.ToString();
        }

        private string GenerateDaLibTestCase1Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_ConnectionString_NullTest()", methodName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Arrange" + Environment.NewLine);

            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append(GenerateDataObjForUtcUse(dt, dataObjClassName, dataObjClassDeclared));

            libMethodUtc.Append(Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(null, null, null);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}({1}, null, string.Empty, string.Empty);", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            if (result.Exception?.InnerException != null)
            {";
            libMethodUtc.Append(newLine1 + Environment.NewLine);
            libMethodUtc.Append("Assert.AreEqual(\"Value cannot be null.\r\nParameter name: connectionString\", ");

            libMethodUtc.Append("result.Exception.InnerException.Message);" + Environment.NewLine + "}");

            return libMethodUtc.ToString();
        }

        private string GenerateDaLibTestCase2Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_ConnectionString_EmptyTest()", methodName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Arrange" + Environment.NewLine);

            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append(GenerateDataObjForUtcUse(dt, dataObjClassName, dataObjClassDeclared));

            libMethodUtc.Append(Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(null, null, null);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}({1}, string.Empty, string.Empty, string.Empty);", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            if (result.Exception?.InnerException != null)
            {";
            libMethodUtc.Append(newLine1 + Environment.NewLine);
            libMethodUtc.Append("Assert.AreEqual(\"Value cannot be null.\r\nParameter name: connectionString\", ");

            libMethodUtc.Append("result.Exception.InnerException.Message);" + Environment.NewLine + "}");

            return libMethodUtc.ToString();
        }

        private string GenerateDaLibTestCase3Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_SqlCommand_NullTest()", methodName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Arrange" + Environment.NewLine);

            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append(GenerateDataObjForUtcUse(dt, dataObjClassName, dataObjClassDeclared));

            libMethodUtc.Append(Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(null, null, null);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}({1}, SqlTestConnectionString, null, string.Empty);", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            if (result.Exception?.InnerException != null)
            {";
            libMethodUtc.Append(newLine1 + Environment.NewLine);
            libMethodUtc.Append("Assert.AreEqual(\"Value cannot be null.\r\nParameter name: sqlCommandText\", ");

            libMethodUtc.Append("result.Exception.InnerException.Message);" + Environment.NewLine + "}");

            return libMethodUtc.ToString();
        }

        private string GenerateDaLibTestCase4Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_SqlCommand_EmptyTest()", methodName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Arrange" + Environment.NewLine);

            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append(GenerateDataObjForUtcUse(dt, dataObjClassName, dataObjClassDeclared));

            libMethodUtc.Append(Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(null, null, null);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}({1}, SqlTestConnectionString, string.Empty, string.Empty);", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            if (result.Exception?.InnerException != null)
            {";
            libMethodUtc.Append(newLine1 + Environment.NewLine);
            libMethodUtc.Append("Assert.AreEqual(\"Value cannot be null.\r\nParameter name: sqlCommandText\", ");

            libMethodUtc.Append("result.Exception.InnerException.Message);" + Environment.NewLine + "}");

            return libMethodUtc.ToString();
        }

        private string GenerateDaLibTestCase5Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();
            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_{1}_NullTest()", methodName, dataObjClassName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(null, null, null);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}(null, SqlTestConnectionString, SqlTestSpName, string.Empty);", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            if (result.Exception?.InnerException != null)
            {";
            libMethodUtc.Append(newLine1 + Environment.NewLine);
            libMethodUtc.AppendFormat("Assert.AreEqual(\"{0}\", ", dataObjClassDeclared);
            libMethodUtc.Append("result.Exception.InnerException.Message);" + Environment.NewLine + "}");

            return libMethodUtc.ToString();
        }

        private string GenerateDaLibTestCase6Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();
            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_{1}_EmptyResults()", methodName, dataObjClassName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Arrange" + Environment.NewLine);
            libMethodUtc.Append("var mockCommon = GetCommonMock_ExecuteReader(string.Empty);" + Environment.NewLine + Environment.NewLine);
            libMethodUtc.Append(GenerateDataObjForUtcUse(dt, dataObjClassName, dataObjClassDeclared));

            libMethodUtc.Append(Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(mockCommon.Object);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}({1}, SqlTestConnectionString, SqlTestSpName, string.Empty).Result;", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            Assert.IsTrue(result == string.Empty);";
            libMethodUtc.Append(newLine1 + Environment.NewLine + "}" + Environment.NewLine);

            return libMethodUtc.ToString();
        }

        private string GenerateDaLibTestCase7Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();
            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_{1}_ValidResults()", methodName, dataObjClassName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Arrange" + Environment.NewLine);
            libMethodUtc.Append("var sampleDasResponse = \"\"" + Environment.NewLine);
            libMethodUtc.Append("var mockCommon = GetCommonMock_ExecuteReader(sampleDasResponse);" + Environment.NewLine + Environment.NewLine);

            libMethodUtc.Append(GenerateDataObjForUtcUse(dt, dataObjClassName, dataObjClassDeclared));

            libMethodUtc.Append(Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(mockCommon.Object);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}({1}, SqlTestConnectionString, SqlTestSpName, string.Empty).Result;", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            Assert.IsTrue(result == string.Empty);";
            libMethodUtc.Append(newLine1 + Environment.NewLine + "}" + Environment.NewLine);

            return libMethodUtc.ToString();
        }

        private string GenerateDaLibTestCase8Method(string methodName, DataTable dt)
        {
            StringBuilder libMethodUtc = new StringBuilder();
            var dataObjClassName = _stubNames["DataObjName"];
            var dataObjClassDeclared = dataObjClassName[0].ToString().ToLower() +
                                       dataObjClassName.Substring(1, dataObjClassName.Length - 1);

            libMethodUtc.Append("[TestMethod]" + Environment.NewLine);
            libMethodUtc.AppendFormat("public void {0}_{1}_Exception()", methodName, dataObjClassName);
            libMethodUtc.Append(Environment.NewLine + "{" + Environment.NewLine + "//Arrange" + Environment.NewLine);
            libMethodUtc.Append("var sampleDasResponse = \"\"" + Environment.NewLine);
            libMethodUtc.Append("var mockCommon = GetCommonMock_ExecuteReader(string.Empty, sampleDasResponse);" + Environment.NewLine + Environment.NewLine);

            libMethodUtc.Append(GenerateDataObjForUtcUse(dt, dataObjClassName, dataObjClassDeclared));

            libMethodUtc.Append(Environment.NewLine + "//Act" + Environment.NewLine);
            libMethodUtc.Append("var yourController = new YourController(null, null, null);" + Environment.NewLine);
            libMethodUtc.AppendFormat("var result = yourController.{0}({1}, SqlTestConnectionString, SqlTestSpName, string.Empty).Result;", methodName, dataObjClassDeclared);
            libMethodUtc.Append(Environment.NewLine + Environment.NewLine + "//Assert" + Environment.NewLine);

            var newLine1 = @"Assert.IsNotNull(result);
            Assert.IsTrue(result == string.Empty);";
            libMethodUtc.Append(newLine1 + Environment.NewLine + "}" + Environment.NewLine);

            return libMethodUtc.ToString();
        }

        #endregion

        private string GenerateDataObjForUtcUse(DataTable dt, string dataObjClassName, string dataObjClassDeclared)
        {
            var dataObjUtc = new StringBuilder();
            dataObjUtc.AppendFormat("var {0} = {1}", dataObjClassDeclared, dataObjClassName);
            dataObjUtc.Append(Environment.NewLine + "{" + Environment.NewLine);

            var paramsList = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                paramsList.AppendFormat("{0} = \"\",", ChangeVarName(row["Parameter_Name"].ToString()));
                paramsList.Append(Environment.NewLine);
            }
            dataObjUtc.Append(paramsList.ToString().Substring(0, paramsList.ToString().Length - 3));
            dataObjUtc.Append(Environment.NewLine + "}" + Environment.NewLine);

            _stubNames.Add("dataobjforutc", dataObjUtc.ToString());
            return dataObjUtc.ToString();
        }

        private string Generate_String_ForUtcUse(DataTable dt)
        {
            var dataObjUtc = new StringBuilder();
            dataObjUtc.AppendFormat("string {0} = \"\";", ChangeVarName(dt.Rows[0]["Parameter_Name"].ToString()));
            dataObjUtc.Append(Environment.NewLine);

            _stubNames.Add("dataobjforutc", dataObjUtc.ToString());
            return dataObjUtc.ToString();
        }

        private string Generate_Int_ForUtcUse(DataTable dt)
        {
            var dataObjUtc = new StringBuilder();
            dataObjUtc.AppendFormat("int {0} = \"\";", ChangeVarName(dt.Rows[0]["Parameter_Name"].ToString()));
            dataObjUtc.Append(Environment.NewLine);

            _stubNames.Add("dataobjforutc", dataObjUtc.ToString());
            return dataObjUtc.ToString();
        }

        //Get all parameters for a specified stored procedure 
        private DataTable ReadParameters(string strName)
        {
            var con = new SqlConnection(txtConnString.Text.Trim());

            var strCmd = $"select Ordinal_Position, Parameter_Name, Data_Type, Character_Maximum_Length from information_schema.parameters where specific_name = \'{strName}\' and PARAMETER_MODE = \'IN\'";

            //Use a SqlDataAdapter to fill a datatable, 
            //using the above command 
            SqlDataAdapter adapter = new SqlDataAdapter(strCmd, con);
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                adapter.Fill(dt);
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                //Clean up the connection
                con.Close();
            }
            return dt;
        }

    }
}
