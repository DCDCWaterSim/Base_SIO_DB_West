using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using UniDB;
using ConsumerResourceModelFramework;
using WaterSimDCDC.Documentation;
//using WaterSimDCDC.America;

namespace WaterSimDCDC
{
    public class RateDataClass
    {
        // DataTable Parameters
        DataTable TheData = null;
        string FDataDirectory = "";
        //string FFilename = "ElevenStateGrowthRates2.csv";
        string FFilename = "ElevenStateGrowthRates2.csv";
        string FScodeFieldStr = "CODE";
        string FSnameFieldStr = "NAME";

        string FPOPRateFieldStr = "POPGRATE";
        string FINDRateFieldStr = "INDGRATE";
        string FAGRateFieldStr  = "AGGRATE";

        // Data Array Parameters

        Dictionary<string, int> StateCodes = new Dictionary<string, int>();
        const double InvalidRate = -1;//double.NaN;

        double[] FAgRateArray = null;
        double[] FIndRateArray = null;
        double[] FPopRateArray = null;

        public RateDataClass(string DataDirectory, string Filename)
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
            FAgRateArray = new double[arraysize];
            FIndRateArray = new double[arraysize];
            FPopRateArray = new double[arraysize];

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

                    string agratestr = DR[FAGRateFieldStr].ToString();
                    string popratestr = DR[FPOPRateFieldStr].ToString();
                    string indratestr = DR[FINDRateFieldStr].ToString();
                    double TempAg = Tools.ConvertToDouble(agratestr, ref isErr, ref errMessage);
                    if (!isErr)
                    {
                        double TempPop = Tools.ConvertToDouble(popratestr, ref isErr, ref errMessage);
                        if (!isErr)
                        {
                            double TempInd = Tools.ConvertToDouble(indratestr, ref isErr, ref errMessage);
                            if (!isErr)
                            {
                                // OK Everything is GOOD let's do it
                                // add to dictionary 
                                StateCodes.Add(namestr, CodeI);
                                // Set Rate values
                                FAgRateArray[CodeI] = TempAg;
                                FPopRateArray[CodeI] = TempPop;
                                FIndRateArray[CodeI] = TempInd;
                                // increment index
                                CodeI++;

                            }

                        }
                    }
                }
            }

        }
        public double AGRate(int State)
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
                        string valstr = DR[FAGRateFieldStr].ToString();
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
        public double POPRate(int State)
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
                        string valstr = DR[FPOPRateFieldStr].ToString();
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
        public double INDRate(int State)
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
                        string valstr = DR[FINDRateFieldStr].ToString();
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

        public double FastINDRate(int State)
        {
            double temp = InvalidRate;
            if ((State < FIndRateArray.Length)&&(State>-1))
            {
                temp = FIndRateArray[State];
            }
            return temp;
        }

        public double FastAGRate(int State)
        {
            double temp = InvalidRate;
            if ((State < FAgRateArray.Length) && (State > -1))
            {
                temp = FAgRateArray[State];
            }
            return temp;
        }

        public double FastPopRate(int State)
        {
            double temp = InvalidRate;
            if ((State > FPopRateArray.Length)&&(State>-1))
            {
                temp = FPopRateArray[State];
            }
            return temp;
        }

    }


    public class RateDataProcess : AnnualFeedbackProcess
    {
        int FStateCode = 0;
        RateDataClass FRData = null;

        double FAGRate = 0.0;
        double FPOPRate = 0.0;
        double FINDRate = 0.0;

        //public const double AG_National_MAX_GPDD = 20.0;
        //public const double AG_National_Shift = 1.2;
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor </summary>
        ///
        /// <param name="aName"> The name of the process </param>
        /// <param name="WSim"> The WaterSimManager Using the Process. </param>
        ///  <remarks> It should not be assumed that the WaterSimManager value being passed is the WaterSimManager that will make'
        /// 		   pre and post process calls</remarks>
        ///-------------------------------------------------------------------------------------------------

        public RateDataProcess(string aName, WaterSimManager WSim, RateDataClass rData)
        {
            BuildDescStrings();
            Fname = aName;
            this.Name = this.GetType().Name;
            FWsim = WSim;
            FRData = rData;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Builds the description strings. </summary>
        ///
        /// <seealso cref="WaterSimDCDC.AnnualFeedbackProcess.BuildDescStrings()"/>
        ///-------------------------------------------------------------------------------------------------

        protected override void BuildDescStrings()
        {
            FProcessDescription = "All Rate Data for the Model";
            FProcessLongDescription = "This process keeps track of the annual changes in the rates ";
            FProcessCode = "RDATA";
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Process the started. </summary>
        ///
        /// <remarks>   Mcquay, 2/25/2016. </remarks>
        ///
        /// <param name="year"> The year about to be run. </param>
        /// <param name="WSim"> The WaterSimManager that is making call. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override bool ProcessStarted(int year, WaterSimManagerClass WSim)
        {
            // zero out cumulatives
            // get the state code for this run
            int statecode = WSim.ParamManager.Model_Parameter(eModelParam.epState).Value;
            FStateCode = statecode;
            // Get the indicator data
            // Growth Rate
            FAGRate = FRData.AGRate(FStateCode);
            //
            FPOPRate = FRData.POPRate(FStateCode);
            //
            FINDRate = FRData.INDRate(FStateCode);
            
            (WSim as WaterSimManager_SIO).WaterSimModel.AgricultureGrowthRate = FAGRate;
            (WSim as WaterSimManager_SIO).WaterSimModel.IndustrialGrowthRate = FINDRate;
            //(WSim as WaterSimManager_SIO).WaterSimAmericaModel.PopulationGrowthRate = FPOPRate;
            return true;
        }

         internal void buildIndicatorArray()
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Method that is called before each annual run. </summary>
        ///
        /// <param name="year"> The year about to be run. </param>
        /// <param name="WSim"> The WaterSimManager that is making call. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. Error should be placed in FErrorMessage. </returns>
        ///
        /// <seealso cref="WaterSimDCDC.AnnualFeedbackProcess.PreProcess(int,WaterSimManagerClass)"/>
        ///-------------------------------------------------------------------------------------------------

        public override bool PreProcess(int year, WaterSimManagerClass WSim)
        {

            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the rate of growth. </summary>
        ///
        /// <value> The rate of growth. </value>
        ///-------------------------------------------------------------------------------------------------

        public double AGRateOfGrowth
        {
            get { return FAGRate; }
        }
        ///
        public double POPRateOfGrowth
        {
            get { return FPOPRate; }
        }
        public double INDRateOfGrowth
        {
            get { return FINDRate; }
        }

  
  

    }


}
