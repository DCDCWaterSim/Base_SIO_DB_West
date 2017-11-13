
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Drawing;
using UniDB;
using ConsumerResourceModelFramework;
using System.Windows.Forms;
namespace WaterSimDCDC.Generic
{
    //class ScenarioDataWest
    //{
        public static class SDI // 
        {
            // ID Fields
             public const string RegionUnitNameField = "RN";
            public const string RegionUnitCodeField = "RC";
            public const string StateUnitNameField = "SN";
            public const string StateUnitCodeField = "SC";
            public const string CountyUnitNameField = "CN";
            public const string CountyUnitCodeField = "CC";
            public const string UnitNameField = RegionUnitNameField;
            public const string UnitCodeField = RegionUnitCodeField;
 
            // Other ID Fields
            public const string LakeField = "LAKE";
            public const string YearField = "YR";
            //// Resources
            //public const string GroundWaterFld = "GW";
            //public const string SurfaceWaterFld = "SUR";
            //public const string SurfaceWaterLakeFld = "SURL";
            //public const string SurfaceWaterColoradoFld = "SURC";
            //public const string ReclaimedWaterFld = "REC";
            //public const string SaltWaterFld = "SAL";
            //public const string AugmentedFld = "AUG";
            //// Consumers
            //public const string UrbanDemandFld = "UTOT";
            //public const string RuralDemandFld = "RTOT";
            //public const string AgricultureDemandFld = "ATOT";
            //public const string IndustrialDemandFld = "ITOT";
            //public const string PowerDemandFld = "PTOT";

            //// Other
            //public const string PowerGenFld = "PGEN";
            //public const string PopulationFld = "POP";

            //public enum eResource { erSurfaceFresh, erSurfaceLake, erGroundwater, erReclained, erSurfaceSaline, erAugmented };
            //public enum eConsumer { ecUrban, ecRural, ecAg, ecInd, ecPower };
            //public enum eOther { eoPopulation, eoPowerGen };

            //static public string[] ResourceList = new string[] { SurfaceWaterFld, SurfaceWaterLakeFld, GroundWaterFld, ReclaimedWaterFld, SaltWaterFld, AugmentedFld };
            //static public string[] ResourceListLabel = new string[] { "Surface Water (Fresh)", "Surface Water Lake ", "Groundwater", "Reclaimed Water (effluent)", "Surface Water (Saline)", "Augmented (desal or other)" };
            //static public string[] ConsumerList = new string[] { UrbanDemandFld, RuralDemandFld, AgricultureDemandFld, IndustrialDemandFld, PowerDemandFld };
            //static public string[] ConsumerListLabel = new string[] { "Urban Public Supply Demand", "Non-Urban Residential Demand", "Agricultural Demand", "Industrial Demand", "Power Generation Demand" };
            //static public string[] OtherListLabel = new string[] { "Population", "Power Generation" };
            //static public string[] OtherList = new string[] { PopulationFld, PowerGenFld };

            //static public string[] IDFieldList = new string[] { RegionUnitNameField, RegionUnitCodeField, StateUnitNameField, StateUnitCodeField, CountyUnitNameField, CountyUnitCodeField, LakeField, YearField };

            //public const string USGSDataFilename = "Just11StatesLakeNoRuralPower.csv";

            //public const string DefaultUnitName = "Florida";

            public static int BadIntValue = int.MinValue;
            public static double BadDoubleValue = double.NaN;

        }
        //=============================================================================================================
        // Scenario DATA MANAGER
        // =============================================================================================================

        /// <summary>   Rate data. </summary>
        public struct ScenarioData
        {
            string FUnitName;
            string FUnitCodeStr;
            int FUnitCode;
            int FScenarioCode;
            double FUrbanCon;
            double FAgCon;
            double FOtherCon;
            double Ftemp;
            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Constructor. </summary>
            ///
            /// <param name="aUnitName">    Name of the unit. </param>
            /// <param name="aUnitCode">    The unit code. </param>
            /// <param name="anAgRate">     an ag rate. </param>
            /// <param name="anIndRate">    an ind rate. </param>
            /// <param name="aPopRate">     The pop rate. </param>
            /// <param name="anAgNet">      an ag net. </param>
            ///-------------------------------------------------------------------------------------------------

            //public ScenarioData(string aUnitName, string aUnitCode, double anUrban, double anAg, double anOther, double anAgNet)
            //{
            //    bool isErr = false;
            //    string errMsg = "";

            //    FUnitName = aUnitName;
            //    FUnitCodeStr = aUnitCode;
            //    //FScenarioCode = scenario;
            //    int temp = Tools.ConvertToInt32(FUnitCodeStr, ref isErr, ref errMsg);
            //    if (!isErr)
            //    {
            //        FUnitCode = temp;
            //    }
            //    else
            //    {
            //        FUnitCode = SDI.BadIntValue;
            //    }
            //    FUrbanCon = anUrban;
            //    FAgCon = anAg;
            //    FOtherCon = anOther;
            //    Ftemp = anAgNet;
            //}
            public ScenarioData(string aUnitName, string aUnitCode, int scenario, double anUrban, double anAg, double anOther, double anAgNet)
            {
                bool isErr = false;
                string errMsg = "";

                FUnitName = aUnitName;
                FUnitCodeStr = aUnitCode;
                FScenarioCode = scenario;

                int temp = Tools.ConvertToInt32(FUnitCodeStr, ref isErr, ref errMsg);
                if (!isErr)
                {
                    FUnitCode = temp;
                }
                else
                {
                    FUnitCode = SDI.BadIntValue;
                }
                FUrbanCon = anUrban;
                FAgCon = anAg;
                FOtherCon = anOther;
                Ftemp = anAgNet;
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Gets the name of the unit. </summary>
            ///
            /// <value> The name of the unit. </value>
            ///-------------------------------------------------------------------------------------------------

            public string UnitName
            {
                get { return FUnitName; }
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Gets the unit code string. </summary>
            ///
            /// <value> The f unit code string. </value>
            ///-------------------------------------------------------------------------------------------------

            public string UnitCodeStr
            {
                get { return FUnitCodeStr; }
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Gets the code. </summary>
            ///
            /// <value> The code. </value>
            ///-------------------------------------------------------------------------------------------------

            public int Code
            {
                get { return FUnitCode; }
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Gets the ag rate. </summary>
            ///
            /// <value> The ag rate. </value>
            ///-------------------------------------------------------------------------------------------------

            public double UrbanConservation
            {
                get { return FUrbanCon; }
                 
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Gets the ind rate. </summary>
            ///
            /// <value> The ind rate. </value>
            ///-------------------------------------------------------------------------------------------------

            public double AgConservation
            {
                get { return FAgCon; }
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Gets the pop rate. </summary>
            ///
            /// <value> The pop rate. </value>
            ///-------------------------------------------------------------------------------------------------

            public double OtherConservation
            {
                get { return FOtherCon; }
            }

          
           


        }
        //


        //
        public class ScenarioDataClass
        {
            // DataTable Parameters
            DataTable TheData = null;
            string FDataDirectory = "";
            string FFilename = "SevenStateScenarios.csv";
            string FScodeFieldStr = SDI.UnitCodeField; //"SC";
            string FSnameFieldStr = SDI.UnitNameField; //"SN";

            string FControlFieldStr = "Scenario";
            string FControl1FieldStr = "UrbanWaterConservation";
            string FControl2FieldStr = "Colorado";
            string FControl3FieldStr = "AgWaterConservation";
            string FControl4FieldStr = "Other";
            // Data Array Parameters

            Dictionary<string, int> StateCodes = new Dictionary<string, int>();
            const double InvalidRate = -1;//double.NaN;

            double[] FValueArray = null;
 
            List<ScenarioData> FScenarioDataList = new List<ScenarioData>();

            public ScenarioDataClass(string DataDirectory, string Filename)
            {
                string errMessage = "";
                bool isErr = false;
                FDataDirectory = DataDirectory;
                FFilename = Filename;
                UniDbConnection DbCon = new UniDbConnection(SQLServer.stText, "", FDataDirectory, "", "", "");
                DbCon.UseFieldHeaders = true;
                DbCon.Open();
                TheData = Tools.LoadTable(DbCon, FFilename, ref isErr, ref errMessage);
                if (isErr)
                {
                    throw new Exception("Error loading Rate Data. " + errMessage);
                }
                // build data arrays
                int arraysize = TheData.Rows.Count;
                FValueArray = new double[arraysize];

                int CodeI = 0;
                foreach (DataRow DR in TheData.Rows)
                {
                    // Get name and code
                    string namestr = DR[FSnameFieldStr].ToString();
                    string codestr = DR[FScodeFieldStr].ToString();
                    // Decided not to use code in DataTable
                    // int codeint = Tools.ConvertToInt32(codestr,ref isErr,ref errMessage);



                    if (!isErr)
                    {
                        string value_scenario = DR[FControlFieldStr].ToString();
                        string value_urbanCon = DR[FControl1FieldStr].ToString();
                        string value_COpurturbation = DR[FControl2FieldStr].ToString();
                        string value_AgCon = DR[FControl3FieldStr].ToString();
                        string value_Other = DR[FControl4FieldStr].ToString();

                        if (!isErr)
                        {
                            int TempScenario = Tools.ConvertToInt32(value_scenario, ref isErr, ref errMessage);
                            if (!isErr)
                            {

                                double TempUrbanCon = Tools.ConvertToDouble(value_COpurturbation, ref isErr, ref errMessage);
                                if (!isErr)
                                {
                                    double TempCO = Tools.ConvertToDouble(value_COpurturbation, ref isErr, ref errMessage);
                                    if (!isErr)
                                    {
                                        double TempAgCon = Tools.ConvertToDouble(value_AgCon, ref isErr, ref errMessage);
                                        if (!isErr)
                                        {
                                            double TempOther = Tools.ConvertToDouble(value_Other, ref isErr, ref errMessage);
                                            if (!isErr)
                                            {

                                                // OK Everything is GOOD let's do it
                                                ScenarioData SD = new ScenarioData(namestr, codestr, TempScenario, TempUrbanCon, TempCO, TempAgCon, TempOther);
                                                FScenarioDataList.Add(SD);
                                                //// add to dictionary 
                                                //StateCodes.Add(namestr, CodeI);
                                                //// Set Rate values
                                                //FUrbanCon[CodeI] = TempUrbanCon;
                                                //FColorado[CodeI] = TempCO;
                                                //FAgCony[CodeI] = TempAgCon;
                                                //FOtherCon[CodeI] = TempOther;

                                                //// increment index
                                                //CodeI++;
                                            }

                                        }
                                    }

                                }
                                else
                                {
                                    errMessage = "Scenario Data Error- Urban Conservation";
                                    throw new MyException(errMessage);
                                }
                            }
                        }
                    }
                }

            }
            public double Urban(int State)
            {
                double result = -1;
                bool iserr = true;
                string errMessage = "";
                foreach (DataRow DR in TheData.Rows)
                {
                    string statecode = DR[FScodeFieldStr].ToString();
                    int temp = Tools.ConvertToInt32(statecode, ref iserr, ref errMessage);
                    if (!iserr)
                    {
                        if (temp == State)
                        {
                            string valstr = DR[FControl1FieldStr].ToString();
                            double tempDbl = Tools.ConvertToDouble(valstr, ref iserr, ref errMessage);
                            if (!iserr)
                            {
                                result = tempDbl;
                                break;
                            }
                        }
                    }
                }
                return result;
            }
            //public double POPRate(int State)
            //{
            //    double result = -1;
            //    bool iserr = true;
            //    string errMessage = "";
            //    foreach (DataRow DR in TheData.Rows)
            //    {
            //        string statecode = DR[FScodeFieldStr].ToString();
            //        int temp = Tools.ConvertToInt32(statecode, ref iserr, ref errMessage);
            //        if (!iserr)
            //        {
            //            if (temp == State)
            //            {
            //                string valstr = DR[FPOPRateFieldStr].ToString();
            //                double tempDbl = Tools.ConvertToDouble(valstr, ref iserr, ref errMessage);
            //                if (!iserr)
            //                {
            //                    result = tempDbl;
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    return result;
            //}
            //public double INDRate(int State)
            //{
            //    double result = -1;
            //    bool iserr = true;
            //    string errMessage = "";
            //    foreach (DataRow DR in TheData.Rows)
            //    {
            //        string statecode = DR[FScodeFieldStr].ToString();
            //        int temp = Tools.ConvertToInt32(statecode, ref iserr, ref errMessage);
            //        if (!iserr)
            //        {
            //            if (temp == State)
            //            {
            //                string valstr = DR[FINDRateFieldStr].ToString();
            //                double tempDbl = Tools.ConvertToDouble(valstr, ref iserr, ref errMessage);
            //                if (!iserr)
            //                {
            //                    result = tempDbl;
            //                    break;
            //                }
            //            }
            //        }
            //    }
            //    return result;
            //}

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast ag net. </summary>
            ///
            /// <param name="Code"> The code. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------

            public double FastUrbanCon(int Code)
            {
                double temp = InvalidRate;
                ScenarioData TheData = FScenarioDataList.Find(delegate(ScenarioData SD) { return SD.Code == Code; });
                if (TheData.Code == Code)
                {
                    temp = TheData.UrbanConservation;
                }
                return temp;
            }
            public double FastUrbanCon(string UnitName)
            {
                double temp = InvalidRate;
                ScenarioData TheData = FScenarioDataList.Find(delegate(ScenarioData SD) { return SD.UnitName == UnitName; });
                if (TheData.UnitName == UnitName)
                {
                    temp = TheData.UrbanConservation;
                }
                return temp;
            }


            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast ag net. </summary>
            ///
            /// <param name="UnitName"> Name of the unit. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------
            public double FastAgCon(int Code)
            {
                double temp = InvalidRate;
                ScenarioData TheData = FScenarioDataList.Find(delegate(ScenarioData SD) { return SD.Code == Code; });
                if (TheData.Code == Code)
                {
                    temp = TheData.AgConservation;
                }
                return temp;
            }


            public double FastAgCon(string UnitName)
            {
                double temp = InvalidRate;
                ScenarioData TheData = FScenarioDataList.Find(delegate(ScenarioData SD) { return SD.UnitName == UnitName; });
                if (TheData.UnitName == UnitName)
                {
                    temp = TheData.AgConservation;
                }
                return temp;
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast ag rate. </summary>
            ///
            /// <param name="Code"> The code. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------

            public double FastOtherCon(int Code)
            {
                double temp = InvalidRate;
                ScenarioData TheData = FScenarioDataList.Find(delegate(ScenarioData SD) { return SD.Code == Code; });
                if (TheData.Code == Code)
                {
                    temp = TheData.OtherConservation;
                }
                return temp;
            }
            public double FastOtherCon(string UnitName)
            {
                double temp = InvalidRate;
                ScenarioData TheData = FScenarioDataList.Find(delegate(ScenarioData SD) { return SD.UnitName == UnitName; });
                if (TheData.UnitName == UnitName)
                {
                    temp = TheData.OtherConservation;
                }
                return temp;
            }

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast ag rate. </summary>
            ///
            /// <param name="UnitName"> Name of the unit. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------

            //public double FastAgRate(string UnitName)
            //{
            //    double temp = InvalidRate;
            //    ScenarioData TheData = FRateDataList.Find(delegate(ScenarioData SD)
            //    {
            //        return SD.UnitName == UnitName;
            //    });
            //    if (TheData.UnitName == UnitName)
            //    {
            //        temp = TheData.AgRate;
            //    }
            //    return temp;
            //}

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast ind rate. </summary>
            ///
            /// <param name="Code"> The code. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------

            //public double FastIndRate(int Code)
            //{
            //    double temp = InvalidRate;
            //    ScenarioData TheData = FRateDataList.Find(delegate(ScenarioData SD) { return SD.Code == Code; });
            //    if (TheData.Code == Code)
            //    {
            //        temp = TheData.IndRate;
            //    }
            //    return temp;
            //}

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast ind rate. </summary>
            ///
            /// <param name="UnitName"> Name of the unit. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------

            //public double FastIndRate(string UnitName)
            //{
            //    double temp = InvalidRate;
            //    ScenarioData TheData = FRateDataList.Find(delegate(ScenarioData SD) { return SD.UnitName == UnitName; });
            //    if (TheData.UnitName == UnitName)
            //    {
            //        temp = TheData.IndRate;
            //    }
            //    return temp;
            //}

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast pop rate. </summary>
            ///
            /// <param name="Code"> The code. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------

            //public double FastPopRate(int Code)
            //{
            //    double temp = InvalidRate;
            //    ScenarioData TheData = FRateDataList.Find(delegate(ScenarioData SD) { return SD.Code == Code; });
            //    if (TheData.Code == Code)
            //    {
            //        temp = TheData.PopRate;
            //    }
            //    return temp;
            //}

            ///-------------------------------------------------------------------------------------------------
            /// <summary>   Fast pop rate. </summary>
            ///
            /// <param name="UnitNam e"> Name of the unit. </param>
            ///
            /// <returns>   . </returns>
            ///-------------------------------------------------------------------------------------------------

            //public double FastPopRate(string UnitName)
            //{
            //    double temp = InvalidRate;
            //    ScenarioData TheData = FRateDataList.Find(delegate(ScenarioData SD) { return SD.UnitName == UnitName; });
            //    if (TheData.UnitName == UnitName)
            //    {
            //        temp = TheData.PopRate;
            //    }
            //    return temp;
            //}

        }

    //}

        public class MyException : System.Exception
        {
            string myerror = "";
            public MyException(string myerrorMessage)
            {
                 myerror = myerrorMessage;
                MessageBox.Show(myerror);
            }

        }

}
