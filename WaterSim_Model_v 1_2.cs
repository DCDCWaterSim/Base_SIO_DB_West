using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace     WaterSimDCDC.Generic
{

#region MODEL ERRORS
    //==========================================================================================================================
    //  MODEL ERROR  
    //==========================================================================================================================
    /// <summary>   Model error. </summary>
    /// <remarks> This class is used to identify a model error </remarks>
    /// 
    public class ModelError
    {
        string FUnitName = "";
        int FUnitCode = 0;
        List<int> FErrorCodes = new List<int>();
        List<string> FErrorMsgs = new List<string>();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="aUnitName">    Name of the unit. </param>
        /// <param name="aUnitCode">    The unit code. </param>
        /// <param name="ErrorCode">    The error code. </param>
        /// <param name="ErrorMsg">     Message describing the error. </param>
        ///-------------------------------------------------------------------------------------------------

        public ModelError(string aUnitName, int aUnitCode, int ErrorCode, string ErrorMsg)
        {
            FUnitCode = aUnitCode;
            FUnitName = aUnitName;
            FErrorCodes.Add(ErrorCode);
            FErrorMsgs.Add(ErrorMsg);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds Error Code and Message. </summary>
        ///
        /// <param name="ErrorCode">    The error code. </param>
        /// <param name="ErrorMsg">     Message describing the error. </param>
        ///-------------------------------------------------------------------------------------------------

        public void Add(int ErrorCode, string ErrorMsg)
        {
            FErrorCodes.Add(ErrorCode);
            FErrorMsgs.Add(ErrorMsg);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the error codes. </summary>
        ///
        /// <value> The error codes. </value>
        ///-------------------------------------------------------------------------------------------------

        public List<int> ErrorCodes
        {
            get { return FErrorCodes; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the error messages. </summary>
        ///
        /// <value> The error messages. </value>
        ///-------------------------------------------------------------------------------------------------

        public List<string> ErrorMessages
        {
            get {return FErrorMsgs; }
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
        /// <summary>   Gets the unit code. </summary>
        ///
        /// <value> The unit code. </value>
        ///-------------------------------------------------------------------------------------------------

        public int UnitCode
        {
            get { return FUnitCode; }
        }

    }

    //==========================================================================================================================
    //  MODEL CALL BACK HANDLERS 
    //==========================================================================================================================

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   AddError Call Back method </summary>
    ///
    /// <param name="TheError"> the error. </param>
    ///<remarks> If Provided, this method is called by the ModelErrorList 
    ///          if the model should be stopped, then return a 1, if the model should continue, return a 0</remarks>
    /// <see cref="ModelErrorList"/>
    /// <returns> 0 or 1 </returns>
    ///-------------------------------------------------------------------------------------------------

    public delegate int OnErrorAddHandler(ModelError TheError);

    //==========================================================================================================================
    //  MODEL ERROR EXCEPTION 
    //==========================================================================================================================

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Critical model error. </summary>
    ///<remarks>  Thrown by ModelErrorList when a CallBack returns !=0 value</remarks>
    /// <seealso cref="System.Exception"/>
    /// <seealso cref="ModelErrorList"/>
    ///-------------------------------------------------------------------------------------------------

    public class CriticalModelError : Exception
    {
        public CriticalModelError(string Message)
            : base("Critical CRF Model Error: "+Message)
        {
        }
    }

    //==========================================================================================================================
    //  MODEL ERROR LIST 
    //==========================================================================================================================

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   List of model errors. </summary>
    /// <remarks> Used to keep track of model errors </remarks>
    ///
    /// <seealso cref="System.Collections.Generic.List<WaterSimDCDC.Generic.ModelError>"/>
    ///-------------------------------------------------------------------------------------------------

    public class ModelErrorList : List<ModelError>
    {
        OnErrorAddHandler FCallBack;

        /// <summary>   Default constructor. </summary>
        public ModelErrorList():base()
        {
            FCallBack = null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <param name="TheCallBack">  the AddError call back Handler. </param>
        ///-------------------------------------------------------------------------------------------------

        public ModelErrorList(OnErrorAddHandler TheCallBack):base()
        {
            FCallBack = TheCallBack;

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds an error. </summary>
        ///
        /// <exception cref="CriticalModelError">   Thrown when critical model error. </exception>
        ///
        /// <param name="aUnitName">    Name of the unit. </param>
        /// <param name="aUnitCode">    The unit code. </param>
        /// <param name="ErrorCode">    The error code. </param>
        /// <param name="ErrorMsg">     Message describing the error. </param>
        ///-------------------------------------------------------------------------------------------------

        public void AddError(string aUnitName, int aUnitCode, int ErrorCode, string ErrorMsg)
        {
            ModelError ME = Find(delegate(ModelError AnError){ return AnError.UnitName == aUnitName; });
            if (ME!=null)
            {
                ME.Add(ErrorCode, ErrorMsg);
            }
            else
            {
                ME = new ModelError(aUnitName,aUnitCode,ErrorCode,ErrorMsg);
            }
            if (FCallBack!=null)
            {
                if (FCallBack(ME)>0)
                {
                   throw new CriticalModelError("On Add Error Code: "+ErrorCode.ToString()+ " " + ErrorMsg); 
                }
            }
        }
    }

#endregion MODEL ERRORS

    //==========================================================================================================================
    //  WATERSIM MODEL MANAGER 
    //==========================================================================================================================

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A data Model for the water simulation crf. </summary>
    ///
    /// <remarks>   Mcquay, 2/6/2017. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class WaterSimModel
    {
        List<WaterSimCRFModel> FUnitModels = new List<WaterSimCRFModel>();
        ModelErrorList FModelErrors = new ModelErrorList();
        bool FisModelError = false;

        public int FstartYear = 0;
        public int FendYear = 0;
        public int FcurrentYear = 0;
        int  FPolicyStartYear = 0;
        int FStartDroughYear = 0;


        RateDataClass FRateData;
        UnitData FUnitData;
        ScenarioDataClass FScenarioData;



        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Mcquay, 2/6/2017. </remarks>
        ///
        /// <param name="DataDirectoryName">    Pathname of the data directory. </param>
        /// <param name="TempDirectoryName">    Pathname of the temporary directory. </param>
        ///-------------------------------------------------------------------------------------------------

        public WaterSimModel(string DataDirectoryName, string TempDirectoryName)
        {
 //           string UnitDataFielname = "Just11StatesLakeNoRuralPower.csv";// "Just5StatesLakeNoRural.csv"; //"Just5StatesLake.csv";// "JustSmithStates.csv";// "All_50_states.csv";
            string UnitDataFielname = "USGSSevenStateCountyWaterUse_031017.csv";//"BasinStatesSubRegionData.csv";"USGSBasinRegionCountyWaterUse_030317.csv";//
            //string rates = "ElevenStateGrowthRates.csv";
            string RateDataFilename = "SevenStateGrowthRatesUnit.csv";// "ElevenStateGrowthRates3.csv";
            string ScenarioDataFilename = "Scenarios.csv";// "ElevenStateGrowthRates3.csv";
            try
            {
                //StreamW(DataDirectoryName);
                FUnitData = new UnitData(DataDirectoryName + "//" + UnitDataFielname, UDI.UnitCodeField, UDI.UnitNameField);
                FRateData = new RateDataClass(DataDirectoryName, RateDataFilename);
                FScenarioData = new ScenarioDataClass(DataDirectoryName ,ScenarioDataFilename);

                foreach (string Name in FUnitData.UnitNames)
                {
                    //WaterSimCRFModel TempModel = new WaterSimCRFModel(FUnitData, FRateData, Name);
                    WaterSimCRFModel TempModel = new WaterSimCRFModel(FUnitData, FRateData,FScenarioData, Name);
                    FUnitModels.Add(TempModel);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Mcquay, 2/6/2017. </remarks>
        ///
        /// <param name="TheUnitData">  Information describing the unit. </param>
        /// <param name="TheUnitName">  Name of the unit. </param>
        /////-------------------------------------------------------------------------------------------------

        public WaterSimModel(UnitData TheUnitData, RateDataClass TheRateData, string TheUnitName)
        {
            FUnitData = TheUnitData;
            FRateData = TheRateData;
            foreach (string Name in TheUnitData.UnitNames)
            {
                WaterSimCRFModel TempModel = new WaterSimCRFModel(FUnitData, FRateData, Name);
                FUnitModels.Add(TempModel);
            }
        }

        public UnitData ModelUnitData
        {
            get { return FUnitData; }
        }

        public RateDataClass ModelRateData
        {
            get { return FRateData; }
        }

        public WaterSimCRFModel GetUnitModel(string aUnitName)
        {
            WaterSimCRFModel WSMod = FUnitModels.Find(delegate(WaterSimCRFModel WSCRF) { return WSCRF.UnitName == aUnitName; });
            return WSMod;

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets an unit model. </summary>
        ///
        /// <param name="aUnitCode">    The unit code. </param>
        ///
        /// <returns>   The unit model. </returns>
        ///-------------------------------------------------------------------------------------------------

        public WaterSimCRFModel GetUnitModel(int aUnitCode)
        {
            WaterSimCRFModel WSMod = FUnitModels.Find(delegate(WaterSimCRFModel WSCRF) { return WSCRF.unitCode == aUnitCode; });
            return WSMod;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   The crf network. </summary>

        /// <param name="aUnitName">    Name of the unit. </param>
        ///
        /// <returns>   A CRF_Unit_Network. </returns>
        ///-------------------------------------------------------------------------------------------------

        public CRF_Unit_Network TheCRFNetwork(string aUnitName)
        {
            WaterSimCRFModel WSMod = GetUnitModel(aUnitName);
            if (WSMod != null)
            {
                return WSMod.TheCRFNetwork;
            }
            else
            {
                return null;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a value indicating whether a model error has occurred. </summary>
        ///
        /// <value> true if model error, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

        public bool isModelError
        {
            get { return FisModelError; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the model errors. </summary>
        ///
        /// <value> The model errors. </value>
        ///-------------------------------------------------------------------------------------------------

        public ModelErrorList ModelErrors
        {
            get {return FModelErrors; }
        }


        /// <summary>   Resets the model errors. </summary>
        public void ResetModelErrors()
        {
            FisModelError = false;
            FModelErrors.Clear();
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the start year. </summary>
        ///
        /// <value> The start year. </value>
        ///-------------------------------------------------------------------------------------------------

        public int startYear
        {
            get { return FstartYear; }
            set 
            { 
                FstartYear = value;
                foreach (WaterSimCRFModel WSM in FUnitModels)
                {
                    WSM.startYear = FstartYear;
                }

            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the end year. </summary>
        ///
        /// <value> The end year. </value>
        ///-------------------------------------------------------------------------------------------------

        public int endYear
        {
            get { return FendYear; }
            set
            { 
                FendYear = value;
                foreach (WaterSimCRFModel WSM in FUnitModels)
                {
                    WSM.endYear = FendYear;
                }

            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the current year. </summary>
        ///
        /// <value> The current year. </value>
        ///-------------------------------------------------------------------------------------------------

        public int currentYear
        {
            get { return FcurrentYear; }
            set
            { 
                FcurrentYear = value;
                foreach (WaterSimCRFModel WSM in FUnitModels)
                {
                    WSM.currentYear = FcurrentYear;
                }
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the policy start year. </summary>
        ///
        /// <value> The policy start year. </value>
        ///-------------------------------------------------------------------------------------------------

        public int policyStartYear
        {
            set 
            {
                FPolicyStartYear = value;
                foreach (WaterSimCRFModel WSM in FUnitModels)
                {
                    WSM.policyStartYear = FPolicyStartYear;
                }
            }
            get { return FPolicyStartYear; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the start drought year. </summary>
        ///
        /// <value> The start drought year. </value>
        ///-------------------------------------------------------------------------------------------------

        public int StartDroughtYear
        {
            get
            {
                return FStartDroughYear;
            }
            set
            {
                FStartDroughYear = value;
                foreach (WaterSimCRFModel WSM in FUnitModels)
                {
                    WSM.startDroughtYear = FStartDroughYear;
                }
            }
        }
        // ========================================

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adjust population. </summary>
        /// <param name="aUnitName">    Name of the unit. </param>
        /// <returns>   A double. </returns>
        ///-------------------------------------------------------------------------------------------------

        public double AdjustPopulation(string aUnitName)
        {
            WaterSimCRFModel WS = GetUnitModel(aUnitName);
            if (WS != null)
            {
                return WS.AdjustPopulation;
            }
            else
            {
                return 0.0;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Ag conservation. </summary>
        ///
        /// <remarks>   2/6/2017. </remarks>
        ///
        /// <param name="aUnitName">    Name of the unit. </param>
        ///
        /// <returns>   A double. </returns>
        ///-------------------------------------------------------------------------------------------------

        public double AgConservation(string aUnitName)
        {
            WaterSimCRFModel WS = GetUnitModel(aUnitName);
            if (WS != null)
            {
                return WS.AgConservation;
            }
            else
            {
                return 0.0;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the state. </summary>
        ///
        /// <value> The state. </value>
        ///-------------------------------------------------------------------------------------------------

        //public string[] State
        //{
        //    get 
        //    {
        //        int count = FUnitModels.Count;
        //        string[] States = new string[count];
        //        for (int i = 0; i < count; i++)
        //        {
        //            States[i] = FUnitModels[i].UnitName;
        //        }
        //        return States; 
        //    }
        //}


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Executes the one year operation. </summary>
        ///
        /// <see cref="isError"/>
        /// <see cref="ModelErros"/>
        /// <param name="parameter1">   The first parameter. </param>
        /// <remarks>  if Callback on error returns value >0 then this routine will break on critical error and stop execution.
        ///            returns a 0 if no critical errors.  But non critical erros can occur and execution continues.  Check isError and ModelErros for non critical erros </remarks>
        /// <returns>  int, 0 if no critical erros occurred, >0 if a critical error occured  . </returns>
        ///-------------------------------------------------------------------------------------------------

        public virtual int runOneYear(int year)
        {
            int result = 0;
            foreach(WaterSimCRFModel WSModel in FUnitModels)
            {
                int tempresult = WSModel.runOneYear(year);
                if (tempresult>0) 
                {
                    try
                    {
                        FisModelError = true;
                        FModelErrors.AddError(WSModel.UnitName, 1, tempresult, "on runOneYear");
                    }
                    catch (Exception ex)
                    {
                        result = tempresult;
                        break;
                    }
 
                }
            }

            return result;
        }

        /// <summary>   Resets the network. </summary>
        /// <remarks>   Call ResetNetwork for each of the Models</remarks>
        public void ResetNetwork()
        {
            foreach(WaterSimCRFModel WSModel in FUnitModels)
            {
                WSModel.ResetNetwork();
            }
        }

        /// <summary>   Resets the variables. </summary>
        public void ResetVariables()
        {
            foreach (WaterSimCRFModel WSModel in FUnitModels)
            {
                WSModel.ResetVariables();
                WSModel.ResetNetwork();
            }
        }
        //======================================================================
        // PROVIDER PROPERTIES
        // ====================================================================
#region ProviderProperties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the Unit Models Data Array index. </summary>
        /// <param name="UnitCode"> The code. </param>
        /// <remarks>   This returns the index into ProviderintArrays for the unitCode </remarks>
        /// <returns>   The unit array index. </returns>
        ///-------------------------------------------------------------------------------------------------

        public int GetUnitIndex(int UnitCode)
        {
            return FUnitModels.FindIndex(delegate(WaterSimCRFModel WSCRF) { return WSCRF.unitCode == UnitCode; });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the Unit Models Data Array index. </summary>
        /// <param name="UnitName"> The code. </param>
        /// <remarks>   This returns the index into ProviderintArrays for the UnitName </remarks>
        /// <returns>   The unit array index. </returns>
        ///-------------------------------------------------------------------------------------------------
        public int GetUnitIndex(string UnitName)
        {
            return FUnitModels.FindIndex(delegate(WaterSimCRFModel WSCRF) { return WSCRF.UnitName == UnitName; });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets a value. </summary>
        /// <param name="PAP">          The ProviderArrayProperty. </param>
        /// <param name="ArrayIndex">   Zero-based index of the ProviderIntArray. </param>
        /// <param name="Value">        The value. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        protected bool SetProviderIntValue(providerArrayProperty PAP, int ArrayIndex, int Value)
        {
            bool result = false;
            try
            {
                ProviderIntArray PIA = PAP.getvalues();
                int[] NewValues = PIA.Values;
                NewValues[ArrayIndex] = Value;
                PIA.Values = NewValues;
                PAP.setvalues(PIA);
                result = true;
            }
            catch (Exception ex)
            {
                // Do nothing for now
            }
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets unit value. </summary>
        /// <param name="Code">         The code. </param>
        /// <param name="PropArray">    [in,out] Array of Unit Values. </param>
        /// <param name="Value">        The value. </param>
        /// <remarks> This places the Value into the proper location in the UnitValueArry based on the UnitCode passed</remarks>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------


        public bool SetUnitValue(int Code, ref int[] UnitValueArray, int Value)
        {
            bool result = false;
            int Index = GetUnitIndex(Code);
            if (Index > -1)
            {
                UnitValueArray[Index] = Value;
                result = true;
            }
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets unit value. </summary>
        /// <param name="Code">     The code. </param>
        /// <param name="PAP">      [in,out] The pap. </param>
        /// <param name="Value">    The value. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool SetUnitValue(int Code, ref providerArrayProperty PAP, int Value)
        {
            bool result = false;
            int Index = GetUnitIndex(Code);
            if (Index > -1)
            {
                result = SetProviderIntValue(PAP, Index, Value);
            }
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets unit value. </summary>
        /// <param name="Name">         The name. </param>
        /// <param name="PropArray">    [in,out] Array of Unit Values. </param>
        /// <param name="Value">        The value. </param>
        /// <remarks> This places the Value into the proper location in the UnitValueArry based on the UniTName passed</remarks>
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool SetUnitValue(string Name, ref int[] UnitValueArray, int Value)
        {
            bool result = false;
            int Index = GetUnitIndex(Name);
            if (Index > -1)
            {
                UnitValueArray[Index] = Value;
                result = true;
            }
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets unit value. </summary>
        /// <param name="Name">     The name. </param>
        /// <param name="PAP">      [in,out] The pap. </param>
        /// <param name="Value">    The value. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool SetUnitValue(string Name, ref providerArrayProperty PAP, int Value)
        {
            bool result = false;
            int Index = GetUnitIndex(Name);
            if (Index > -1)
            {
                result = SetProviderIntValue(PAP, Index, Value);
            }
            return result;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets unit value. </summary>
        /// <param name="Codes">            The codes. </param>
        /// <param name="UnitValueArray">   [in,out] Array of unit values. </param>
        /// <param name="Values">           The values. </param>
        /// <param name="BadCode">          [out] List of which codes were good (true) and bad (false). </param>
        /// <remarks> This places each value in Values into the proper location in the UnitValueArry based on the code value in Codes
        ///           This allows multiple parameters to be sent at once.  Not much faster than just doing each one, one at a time using
        ///           SetUnitValue with single code and value, k=just more convenient.
        ///           </remarks>
        /// 
        /// <returns>   true if it succeeds, false if one of the assignments failed. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool SetUnitValue(List<int> Codes, ref int[] UnitValueArray, List<int> Values, out List<bool> BadCode)
        {
            bool result = true;
            // OK all good
            BadCode = new List<bool>();
            // loop through each of the codes
            for (int i = 0; i < Codes.Count; i++)
            {
                // find this code in the unit list
                int Index = GetUnitIndex(Codes[i]);
                if (Index > -1)
                {
                    // found it, set the value and BadCode
                    UnitValueArray[Index] = Values[i];
                    BadCode.Add(false);
                }
                else
                {
                    // not found, this is an error state, set the BadCode
                    result = false;
                    BadCode.Add(true);
                }
            }
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets unit value. </summary>
        /// <param name="Names">            The names. </param>
        /// <param name="UnitValueArray">   [in,out] Array of unit values. </param>
        /// <param name="Values">           The values. </param>
        /// <param name="BadCode">          [out] List of which codes were good (true) and bad (false). </param>
        /// <remarks> This places each value in Values into the proper location in the UnitValueArry based on the code value in Names
        ///           This allows multiple parameters to be sent at once.  Not much faster than just doing each one, one at a time using
        ///           SetUnitValue with single code and value, k=just more convenient.
        ///           </remarks>
        /// <returns>   true if it succeeds, false if one of the assignments failed. </returns>
        ///-------------------------------------------------------------------------------------------------

        public bool SetUnitValue(List<string> Names, ref int[] UnitValueArray, List<int> Values, out List<bool> BadCode)
        {
            bool result = true;
            BadCode = new List<bool>();
            // loop through each of the codes
            for (int i = 0; i < Names.Count; i++)
            {
                // find this code in the unit list
                int Index = GetUnitIndex(Names[i]);
                if (Index > -1)
                {
                    // found it, set the value and BadCode
                    UnitValueArray[Index] = Values[i];
                    BadCode.Add(false);
                }
                else
                {
                    // not found, this is an error state, set the BadCode
                    result = false;
                    BadCode.Add(true);
                }
            }
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets a population. </summary>
        ///
        /// <param name="Values">   The values. </param>
        ///-------------------------------------------------------------------------------------------------

        public void Set_population(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].population = Values[i];
            }
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets a population. </summary>
        ///
        /// <param name="Values">   The values. </param>
        ///-------------------------------------------------------------------------------------------------

        //public void Set_population(int[] Values)
        //{
        //    int ArraySize = FUnitModels.Count;
        //    if (ArraySize > Values.Length)
        //    {
        //        ArraySize = Values.Length;
        //    }
        //    for (int i = 0; i < ArraySize; i++)
        //    {
        //        FUnitModels[i].population = Values[i];
        //    }
        //}


        //=======================================================
        //  Population
        //=======================================================
        // 08.03.17 

        public double _popGrowthRateModifier = 1.0;
        public double PopulationGrowthRateModifier
        {
            get { return _popGrowthRateModifier; }
            set { _popGrowthRateModifier = value; }
        }

        #region Population
        ///------------------------------------------------------
        /// <summary> The Population provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Population;

        ///------------------------------------------------------
        /// <summary> Gets the Population  </summary>
        ///<returns> the Population </returns>

        public int[] geti_Pop()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].population ;
            }
            return result;
        }

        #endregion Population
        //=======================================================
        //  GPCD_urban
        //=======================================================
        #region GPCD_urban
        ///------------------------------------------------------
        /// <summary> The GPCD_urban provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty GPCD_urban;

        ///------------------------------------------------------
        /// <summary> Gets the GPCD_urban  </summary>
        ///<returns> the GPCD_urban </returns>

        public int[] geti_gpcd()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_gpcd();
            }
            return result;
        }

        #endregion GPCD_urban
        //=======================================================
        //  GPCD_ag
        //=======================================================
        #region GPCD_ag
        ///------------------------------------------------------
        /// <summary> The GPCD_ag provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty GPCD_ag;

        ///------------------------------------------------------
        /// <summary> Gets the GPCD_ag  </summary>
        ///<returns> the GPCD_ag </returns>

        public int[] geti_gpcdAg()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_gpcdAg();
            }
            return result;
        }

        #endregion GPCD_ag
        //=======================================================
        //  GPCD_other
        //=======================================================
        #region GPCD_other
        ///------------------------------------------------------
        /// <summary> The GPCD_other provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty GPCD_other;

        ///------------------------------------------------------
        /// <summary> Gets the GPCD_other  </summary>
        ///<returns> the GPCD_other </returns>

        public int[] geti_gpcdOther()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_gpcdOther();
            }
            return result;
        }

        #endregion GPCD_other
        //=======================================================
        //  SurfaceFresh
        //=======================================================
        #region SurfaceFresh
        ///------------------------------------------------------
        /// <summary> The SurfaceFresh provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty SurfaceFresh;

        ///------------------------------------------------------
        /// <summary> Gets the SurfaceFresh  </summary>
        ///<returns> the SurfaceFresh </returns>

        public int[] geti_SurfaceWaterFresh()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SurfaceWaterFresh();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a SurfaceFresh  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SurfaceWaterFresh(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SurfaceWaterFresh(Values[i]);
            }
        }
        #endregion SurfaceFresh
        //=======================================================
        //  SurfaceFreshNet
        //=======================================================
        #region SurfaceFreshNet
        ///------------------------------------------------------
        /// <summary> The SurfaceFreshNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty SurfaceFreshNet;

        ///------------------------------------------------------
        /// <summary> Gets the SurfaceFreshNet  </summary>
        ///<returns> the SurfaceFreshNet </returns>

        public int[] geti_SurfaceWaterFreshNet()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SurfaceWaterFreshNet();
            }
            return result;
        }

        #endregion SurfaceFreshNet
        //=======================================================
        //  SurfaceSaline
        //=======================================================
        #region SurfaceSaline
        ///------------------------------------------------------
        /// <summary> The SurfaceSaline provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty SurfaceSaline;

        ///------------------------------------------------------
        /// <summary> Gets the SurfaceSaline  </summary>
        ///<returns> the SurfaceSaline </returns>

        public int[] geti_SurfaceWaterSaline()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SurfaceWaterSaline();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a SurfaceSaline  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SurfaceWaterSaline(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SurfaceWaterSaline(Values[i]);
            }
        }
        #endregion SurfaceSaline
        //=======================================================
        //  SurfaceSalineNet
        //=======================================================
        #region SurfaceSalineNet
        ///------------------------------------------------------
        /// <summary> The SurfaceSalineNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty SurfaceSalineNet;

        ///------------------------------------------------------
        /// <summary> Gets the SurfaceSalineNet  </summary>
        ///<returns> the SurfaceSalineNet </returns>

        public int[] geti_SurfaceWaterSalineNet()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SurfaceWaterSalineNet();
            }
            return result;
        }

        #endregion SurfaceSalineNet
        //=======================================================
        //  Groundwater
        //=======================================================
        #region Groundwater
        ///------------------------------------------------------
        /// <summary> The Groundwater provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Groundwater;

        ///------------------------------------------------------
        /// <summary> Gets the Groundwater  </summary>
        ///<returns> the Groundwater </returns>

        public int[] geti_Groundwater()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Groundwater();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a Groundwater  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_Groundwater(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_Groundwater(Values[i]);
            }
        }
        #endregion Groundwater
        //=======================================================
        //  GroundwaterNet
        //=======================================================
        #region GroundwaterNet
        ///------------------------------------------------------
        /// <summary> The GroundwaterNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty GroundwaterNet;

        ///------------------------------------------------------
        /// <summary> Gets the GroundwaterNet  </summary>
        ///<returns> the GroundwaterNet </returns>

        public int[] geti_GroundwaterNet()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_GroundwaterNet();
            }
            return result;
        }

        #endregion GroundwaterNet
        //=======================================================
        //  Effluent
        //=======================================================
        #region Effluent
        ///------------------------------------------------------
        /// <summary> The Effluent provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Effluent;

        ///------------------------------------------------------
        /// <summary> Gets the Effluent  </summary>
        ///<returns> the Effluent </returns>

        public int[] geti_Effluent()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Effluent();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a Effluent  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_Effluent(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_Effluent(Values[i]);
            }
        }
        #endregion Effluent
        //=======================================================
        //  EffluentNet
        //=======================================================
        #region EffluentNet
        ///------------------------------------------------------
        /// <summary> The EffluentNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty EffluentNet;

        ///------------------------------------------------------
        /// <summary> Gets the EffluentNet  </summary>
        ///<returns> the EffluentNet </returns>

        public int[] geti_EffluentNet()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_EffluentNet();
            }
            return result;
        }

        #endregion EffluentNet
        //=======================================================
        //  SurfaceLake
        //=======================================================
        #region SurfaceLake
        ///------------------------------------------------------
        /// <summary> The SurfaceLake provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty SurfaceLake;

        ///------------------------------------------------------
        /// <summary> Gets the SurfaceLake  </summary>
        ///<returns> the SurfaceLake </returns>

        public int[] geti_SurfaceLake()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SurfaceLake();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a SurfaceLake  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SurfaceLake(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SurfaceLake(Values[i]);
            }
        }
        #endregion SurfaceLake
        //=======================================================
        //  SurfaceLakeNet
        //=======================================================
        #region SurfaceLakeNet
        ///------------------------------------------------------
        /// <summary> The SurfaceLakeNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty SurfaceLakeNet;

        ///------------------------------------------------------
        /// <summary> Gets the SurfaceLakeNet  </summary>
        ///<returns> the SurfaceLakeNet </returns>

        public int[] geti_SurfaceLakeNet()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SurfaceLakeNet();
            }
            return result;
        }

        #endregion SurfaceLakeNet
        //=======================================================
        //  TotalSupplies
        //=======================================================
        #region TotalSupplies
        ///------------------------------------------------------
        /// <summary> The TotalSupplies provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty TotalSupplies;

        ///------------------------------------------------------
        /// <summary> Gets the TotalSupplies  </summary>
        ///<returns> the TotalSupplies </returns>

        public int[] geti_TotalSupplies()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_TotalSupplies();
            }
            return result;
        }

        #endregion TotalSupplies
        //=======================================================
        //  Urban
        //=======================================================
        #region Urban
        ///------------------------------------------------------
        /// <summary> The Urban provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Urban;

        ///------------------------------------------------------
        /// <summary> Gets the Urban  </summary>
        ///<returns> the Urban </returns>

        public int[] geti_Urban()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Urban();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a Urban  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_Urban(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_Urban(Values[i]);
            }
        }
        #endregion Urban
        //=======================================================
        //  UrbanNet
        //=======================================================
        #region UrbanNet
        ///------------------------------------------------------
        /// <summary> The UrbanNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty UrbanNet;

        ///------------------------------------------------------
        /// <summary> Gets the UrbanNet  </summary>
        ///<returns> the UrbanNet </returns>

        public int[] geti_Urban_Net()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Urban_Net();
            }
            return result;
        }

        #endregion UrbanNet
        //=======================================================
        //  Agriculture
        //=======================================================
        #region Agriculture
        ///------------------------------------------------------
        /// <summary> The Agriculture provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Agriculture;

        ///------------------------------------------------------
        /// <summary> Gets the Agriculture  </summary>
        ///<returns> the Agriculture </returns>

        public int[] geti_Agriculture()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Agriculture();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a Agriculture  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_Agriculture(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_Agriculture(Values[i]);
            }
        }
        #endregion Agriculture
        //=======================================================
        //  AgricultureNet
        //=======================================================
        #region AgricultureNet
        ///------------------------------------------------------
        /// <summary> The AgricultureNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty AgricultureNet;

        ///------------------------------------------------------
        /// <summary> Gets the AgricultureNet  </summary>
        ///<returns> the AgricultureNet </returns>

        public int[] geti_Agriculture_Net()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Agriculture_Net();
            }
            return result;
        }

        #endregion AgricultureNet
        //=======================================================
        //  Industrial
        //=======================================================
        #region Industrial
        ///------------------------------------------------------
        /// <summary> The Industrial provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Industrial;

        ///------------------------------------------------------
        /// <summary> Gets the Industrial  </summary>
        ///<returns> the Industrial </returns>

        public int[] geti_Industrial()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Industrial();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a Industrial  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_Industrial(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_Industrial(Values[i]);
            }
        }
        #endregion Industrial
        //=======================================================
        //  IndustrialNet
        //=======================================================
        #region IndustrialNet
        ///------------------------------------------------------
        /// <summary> The IndustrialNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty IndustrialNet;

        ///------------------------------------------------------
        /// <summary> Gets the IndustrialNet  </summary>
        ///<returns> the IndustrialNet </returns>

        public int[] geti_Industrial_Net()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Industrial_Net();
            }
            return result;
        }

        #endregion IndustrialNet
        //=======================================================
        //  Power
        //=======================================================
        #region Power
        ///------------------------------------------------------
        /// <summary> The Power provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Power;

        ///------------------------------------------------------
        /// <summary> Gets the Power  </summary>
        ///<returns> the Power </returns>

        public int[] geti_PowerWater()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_PowerWater();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a Power  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_PowerWater(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_PowerWater(Values[i]);
            }
        }
        #endregion Power
        //=======================================================
        //  PowerNet
        //=======================================================
        #region PowerNet
        ///------------------------------------------------------
        /// <summary> The PowerNet provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty PowerNet;

        ///------------------------------------------------------
        /// <summary> Gets the PowerNet  </summary>
        ///<returns> the PowerNet </returns>

        public int[] geti_PowerWater_Net()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_PowerWater_Net();
            }
            return result;
        }

        #endregion PowerNet
        //=======================================================
        //  PowerEnergy
        //=======================================================
        #region PowerEnergy
        ///------------------------------------------------------
        /// <summary> The PowerEnergy provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty PowerEnergy;

        ///------------------------------------------------------
        /// <summary> Gets the PowerEnergy  </summary>
        ///<returns> the PowerEnergy </returns>

        public int[] geti_PowerEnergy()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_PowerEnergy();
            }
            return result;
        }

        #endregion PowerEnergy
        //=======================================================
        //  NetDemandDifference
        //=======================================================
        #region NetDemandDifference
        ///------------------------------------------------------
        /// <summary> The NetDemandDifference provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty NetDemandDifference;

        ///------------------------------------------------------
        /// <summary> Gets the NetDemandDifference  </summary>
        ///<returns> the NetDemandDifference </returns>

        public int[] geti_NetDemandDifference()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_NetDemandDifference();
            }
            return result;
        }

        #endregion NetDemandDifference
        //=======================================================
        //  UrbanWaterConservation
        //=======================================================
        #region UrbanWaterConservation
        ///------------------------------------------------------
        /// <summary> The UrbanWaterConservation provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty UrbanWaterConservation;

        ///------------------------------------------------------
        /// <summary> Gets the UrbanWaterConservation  </summary>
        ///<returns> the UrbanWaterConservation </returns>

        public int[] geti_UrbanConservation()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_UrbanConservation();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a UrbanWaterConservation  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_UrbanConservation(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_UrbanConservation(Values[i]);
            }
        }
        #endregion UrbanWaterConservation
        //=======================================================
        //  AgWaterConservation
        //=======================================================
        #region AgWaterConservation
        ///------------------------------------------------------
        /// <summary> The AgWaterConservation provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty AgWaterConservation;

        ///------------------------------------------------------
        /// <summary> Gets the AgWaterConservation  </summary>
        ///<returns> the AgWaterConservation </returns>

        public int[] geti_AgConservation()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_AgConservation();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a AgWaterConservation  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_AgConservation(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_AgConservation(Values[i]);
            }
        }
        #endregion AgWaterConservation
        //=======================================================
        //  PowerWaterConservation
        //=======================================================
        #region PowerWaterConservation
        ///------------------------------------------------------
        /// <summary> The PowerWaterConservation provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty PowerWaterConservation;

        ///------------------------------------------------------
        /// <summary> Gets the PowerWaterConservation  </summary>
        ///<returns> the PowerWaterConservation </returns>

        public int[] geti_PowerConservation()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_PowerConservation();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a PowerWaterConservation  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_PowerConservation(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_PowerConservation(Values[i]);
            }
        }
        #endregion PowerWaterConservation
        //=======================================================
        //  IndustrialWaterConservation
        //=======================================================
        #region IndustrialWaterConservation
        ///------------------------------------------------------
        /// <summary> The IndustrialWaterConservation provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty IndustrialWaterConservation;

        ///------------------------------------------------------
        /// <summary> Gets the IndustrialWaterConservation  </summary>
        ///<returns> the IndustrialWaterConservation </returns>

        public int[] geti_IndustryConservation()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_IndustryConservation();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a IndustrialWaterConservation  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_IndustryConservation(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_IndustryConservation(Values[i]);
            }
        }
        #endregion IndustrialWaterConservation
        //=======================================================
        //  SurfaceWaterManagement
        //=======================================================
        #region SurfaceWaterManagement
        ///------------------------------------------------------
        /// <summary> The SurfaceWaterManagement provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty SurfaceWaterManagement;

        ///------------------------------------------------------
        /// <summary> Gets the SurfaceWaterManagement  </summary>
        ///<returns> the SurfaceWaterManagement </returns>

        public int[] geti_SurfaceWaterControl()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SurfaceWaterControl();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a SurfaceWaterManagement  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SurfaceWaterControl(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SurfaceWaterControl(Values[i]);
            }
        }
        #endregion SurfaceWaterManagement
        //=======================================================
        //  GroundwaterManagement
        //=======================================================
        #region GroundwaterManagement
        ///------------------------------------------------------
        /// <summary> The GroundwaterManagement provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty GroundwaterManagement;

        ///------------------------------------------------------
        /// <summary> Gets the GroundwaterManagement  </summary>
        ///<returns> the GroundwaterManagement </returns>

        public int[] geti_GroundwaterControl()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_GroundwaterControl();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a GroundwaterManagement  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_GroundwaterControl(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_GroundwaterControl(Values[i]);
            }
        }
        #endregion GroundwaterManagement
        //=======================================================
        //  ReclainedWaterUse
        //=======================================================
        #region ReclainedWaterUse
        ///------------------------------------------------------
        /// <summary> The ReclainedWaterUse provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty ReclainedWaterUse;

        ///------------------------------------------------------
        /// <summary> Gets the ReclainedWaterUse  </summary>
        ///<returns> the ReclainedWaterUse </returns>

        public int[] geti_ReclaimedWaterManagement()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_ReclaimedWaterManagement();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a ReclainedWaterUse  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_ReclaimedWaterManagement(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_ReclaimedWaterManagement(Values[i]);
            }
        }
        #endregion ReclainedWaterUse
        //=======================================================
        //  LakeWaterManagement
        //=======================================================
        #region LakeWaterManagement
        ///------------------------------------------------------
        /// <summary> The LakeWaterManagement provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty LakeWaterManagement;

        ///------------------------------------------------------
        /// <summary> Gets the LakeWaterManagement  </summary>
        ///<returns> the LakeWaterManagement </returns>

        public int[] geti_LakeWaterManagement()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_LakeWaterManagement();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a LakeWaterManagement  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_LakeWaterManagement(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_LakeWaterManagement(Values[i]);
            }
        }
        #endregion LakeWaterManagement
        //=======================================================
        //  Augmented
        //=======================================================
        #region Augmented
        ///------------------------------------------------------
        /// <summary> The Augmented provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty Augmented;

        ///------------------------------------------------------
        /// <summary> Gets the Augmented  </summary>
        ///<returns> the Augmented </returns>

        public int[] geti_Desalinization()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_Desalinization();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a Augmented  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_Desalinization(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_Desalinization(Values[i]);
            }
        }
        #endregion Augmented
        //=======================================================
        //  PopGrowthRate
        //=======================================================
        #region PopGrowthRate
        ///------------------------------------------------------
        /// <summary> The PopGrowthRate provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty PopGrowthRate;

        ///------------------------------------------------------
        /// <summary> Gets the PopGrowthRate  </summary>
        ///<returns> the PopGrowthRate </returns>

        public int[] geti_PopGrowthRate()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_PopGrowthRate();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a PopGrowthAdjustment  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_PopGrowthRate(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_PopGrowthRate(Values[i]);
            }
        }
        #endregion PopGrowthRate

        #region PopGrowthRate Modifyer

            public providerArrayProperty PopGrowthRateModifyer;

            ///------------------------------------------------------
            /// <summary> Gets the PopGrowthRate  </summary>
            ///<returns> the PopGrowthRate </returns>

            public int[] geti_PopGrowthRateMod()
            {
                int ArraySize = FUnitModels.Count;
                int[] result = new int[ArraySize];
                for (int i = 0; i < ArraySize; i++)
                {
                    result[i] = FUnitModels[i].geti_PopGRateModifier();
                }
                return result;
            }


            ///------------------------------------------------------
            /// <summary> Sets a PopGrowthAdjustment  </summary>
            /// <param name="Values">   The values. </param>

            public void seti_PopGrowthRateMod(int[] Values)
            {
                int ArraySize = FUnitModels.Count;
                if (ArraySize > Values.Length)
                {
                    ArraySize = Values.Length;
                }
                for (int i = 0; i < ArraySize; i++)
                {
                    FUnitModels[i].seti_PopGRateModifier(Values[i]);
                }
            }
        #endregion PopGrowthRate Modifyer

        //=======================================================
        //  ClimateDrought
        //=======================================================
        #region ClimateDrought
        ///------------------------------------------------------
        /// <summary> The ClimateDrought provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty ClimateDrought;

        ///------------------------------------------------------
        /// <summary> Gets the ClimateDrought  </summary>
        ///<returns> the ClimateDrought </returns>

        public int[] geti_DroughtImpacts()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_DroughtImpacts();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a ClimateDrought  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_DroughtImpacts(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_DroughtImpacts(Values[i]);
            }
        }
        #endregion ClimateDrought
        //=======================================================
        //  DroughtControl
        //=======================================================
        #region DroughtControl
        ///------------------------------------------------------
        /// <summary> The DroughtControl provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty DroughtControl;

        ///------------------------------------------------------
        /// <summary> Gets the DroughtControl  </summary>
        ///<returns> the DroughtControl </returns>

        public int[] geti_DroughtControl()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_DroughtControl();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a DroughtControl  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_DroughtControl(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_DroughtControl(Values[i]);
            }
        }
        #endregion DroughtControl
        //=======================================================
        //  AgricultureProduction
        //=======================================================
        #region AgricultureProduction
        ///------------------------------------------------------
        /// <summary> The AgricultureProduction provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty AgricultureProduction;

        ///------------------------------------------------------
        /// <summary> Gets the AgricultureProduction  </summary>
        ///<returns> the AgricultureProduction </returns>

        public int[] geti_AgricutureProduction()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_AgricutureProduction();
            }
            return result;
        }

        #endregion AgricultureProduction
        //=======================================================
        //  AgriculturalGrowth
        //=======================================================
        #region AgriculturalGrowth
        ///------------------------------------------------------
        /// <summary> The AgriculturalGrowth provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty AgriculturalGrowth;

        ///------------------------------------------------------
        /// <summary> Gets the AgriculturalGrowth  </summary>
        ///<returns> the AgriculturalGrowth </returns>

        public int[] geti_AgGrowthRate()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_AgGrowthRate();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a AgriculturalGrowth  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_AgGrowthRate(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_AgGrowthRate(Values[i]);
            }
        }
        #endregion AgriculturalGrowth

        //==================================
        // FLUXES
        //==================================

        //=======================================================
        //  _SUR_UD
        //=======================================================
        #region _SUR_UD
        ///------------------------------------------------------
        /// <summary> The _SUR_UD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SUR_UD;

        ///------------------------------------------------------
        /// <summary> Gets the _SUR_UD  </summary>
        ///<returns> the _SUR_UD </returns>

        public int[] geti_SUR_UD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SUR_UD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SUR_UD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SUR_UD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SUR_UD(Values[i]);
            }
        }
        #endregion _SUR_UD
        //=======================================================
        //  _SUR_AD
        //=======================================================
        #region _SUR_AD
        ///------------------------------------------------------
        /// <summary> The _SUR_AD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SUR_AD;

        ///------------------------------------------------------
        /// <summary> Gets the _SUR_AD  </summary>
        ///<returns> the _SUR_AD </returns>

        public int[] geti_SUR_AD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SUR_AD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SUR_AD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SUR_AD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SUR_AD(Values[i]);
            }
        }
        #endregion _SUR_AD
        //=======================================================
        //  _SUR_ID
        //=======================================================
        #region _SUR_ID
        ///------------------------------------------------------
        /// <summary> The _SUR_ID provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SUR_ID;

        ///------------------------------------------------------
        /// <summary> Gets the _SUR_ID  </summary>
        ///<returns> the _SUR_ID </returns>

        public int[] geti_SUR_ID()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SUR_ID();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SUR_ID  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SUR_ID(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SUR_ID(Values[i]);
            }
        }
        #endregion _SUR_ID
        //=======================================================
        //  _SUR_PD
        //=======================================================
        #region _SUR_PD
        ///------------------------------------------------------
        /// <summary> The _SUR_PD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SUR_PD;

        ///------------------------------------------------------
        /// <summary> Gets the _SUR_PD  </summary>
        ///<returns> the _SUR_PD </returns>

        public int[] geti_SUR_PD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SUR_PD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SUR_PD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SUR_PD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SUR_PD(Values[i]);
            }
        }
        #endregion _SUR_PD
        //=======================================================
        //  _SURL_UD
        //=======================================================
        #region _SURL_UD
        ///------------------------------------------------------
        /// <summary> The _SURL_UD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SURL_UD;

        ///------------------------------------------------------
        /// <summary> Gets the _SURL_UD  </summary>
        ///<returns> the _SURL_UD </returns>

        public int[] geti_SURL_UD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SURL_UD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SURL_UD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SURL_UD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SURL_UD(Values[i]);
            }
        }
        #endregion _SURL_UD
        //=======================================================
        //  _SURL_AD
        //=======================================================
        #region _SURL_AD
        ///------------------------------------------------------
        /// <summary> The _SURL_AD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SURL_AD;

        ///------------------------------------------------------
        /// <summary> Gets the _SURL_AD  </summary>
        ///<returns> the _SURL_AD </returns>

        public int[] geti_SURL_AD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SURL_AD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SURL_AD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SURL_AD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SURL_AD(Values[i]);
            }
        }
        #endregion _SURL_AD
        //=======================================================
        //  _SURL_ID
        //=======================================================
        #region _SURL_ID
        ///------------------------------------------------------
        /// <summary> The _SURL_ID provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SURL_ID;

        ///------------------------------------------------------
        /// <summary> Gets the _SURL_ID  </summary>
        ///<returns> the _SURL_ID </returns>

        public int[] geti_SURL_ID()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SURL_ID();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SURL_ID  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SURL_ID(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SURL_ID(Values[i]);
            }
        }
        #endregion _SURL_ID
        //=======================================================
        //  _SURL_PD
        //=======================================================
        #region _SURL_PD
        ///------------------------------------------------------
        /// <summary> The _SURL_PD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SURL_PD;

        ///------------------------------------------------------
        /// <summary> Gets the _SURL_PD  </summary>
        ///<returns> the _SURL_PD </returns>

        public int[] geti_SURL_PD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SURL_PD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SURL_PD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SURL_PD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SURL_PD(Values[i]);
            }
        }
        #endregion _SURL_PD
        //=======================================================
        //  _GW_UD
        //=======================================================
        #region _GW_UD
        ///------------------------------------------------------
        /// <summary> The _GW_UD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _GW_UD;

        ///------------------------------------------------------
        /// <summary> Gets the _GW_UD  </summary>
        ///<returns> the _GW_UD </returns>

        public int[] geti_GW_UD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_GW_UD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _GW_UD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_GW_UD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_GW_UD(Values[i]);
            }
        }
        #endregion _GW_UD
        //=======================================================
        //  _GW_AD
        //=======================================================
        #region _GW_AD
        ///------------------------------------------------------
        /// <summary> The _GW_AD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _GW_AD;

        ///------------------------------------------------------
        /// <summary> Gets the _GW_AD  </summary>
        ///<returns> the _GW_AD </returns>

        public int[] geti_GW_AD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_GW_AD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _GW_AD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_GW_AD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_GW_AD(Values[i]);
            }
        }
        #endregion _GW_AD
        //=======================================================
        //  _GW_ID
        //=======================================================
        #region _GW_ID
        ///------------------------------------------------------
        /// <summary> The _GW_ID provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _GW_ID;

        ///------------------------------------------------------
        /// <summary> Gets the _GW_ID  </summary>
        ///<returns> the _GW_ID </returns>

        public int[] geti_GW_ID()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_GW_ID();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _GW_ID  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_GW_ID(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_GW_ID(Values[i]);
            }
        }
        #endregion _GW_ID
        //=======================================================
        //  _GW_PD
        //=======================================================
        #region _GW_PD
        ///------------------------------------------------------
        /// <summary> The _GW_PD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _GW_PD;

        ///------------------------------------------------------
        /// <summary> Gets the _GW_PD  </summary>
        ///<returns> the _GW_PD </returns>

        public int[] geti_GW_PD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_GW_PD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _GW_PD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_GW_PD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_GW_PD(Values[i]);
            }
        }
        #endregion _GW_PD
        //=======================================================
        //  _REC_UD
        //=======================================================
        #region _REC_UD
        ///------------------------------------------------------
        /// <summary> The _REC_UD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _REC_UD;

        ///------------------------------------------------------
        /// <summary> Gets the _REC_UD  </summary>
        ///<returns> the _REC_UD </returns>

        public int[] geti_REC_UD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_REC_UD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _REC_UD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_REC_UD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_REC_UD(Values[i]);
            }
        }
        #endregion _REC_UD
        //=======================================================
        //  _REC_AD
        //=======================================================
        #region _REC_AD
        ///------------------------------------------------------
        /// <summary> The _REC_AD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _REC_AD;

        ///------------------------------------------------------
        /// <summary> Gets the _REC_AD  </summary>
        ///<returns> the _REC_AD </returns>

        public int[] geti_REC_AD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_REC_AD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _REC_AD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_REC_AD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_REC_AD(Values[i]);
            }
        }
        #endregion _REC_AD
        //=======================================================
        //  _REC_ID
        //=======================================================
        #region _REC_ID
        ///------------------------------------------------------
        /// <summary> The _REC_ID provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _REC_ID;

        ///------------------------------------------------------
        /// <summary> Gets the _REC_ID  </summary>
        ///<returns> the _REC_ID </returns>

        public int[] geti_REC_ID()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_REC_ID();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _REC_ID  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_REC_ID(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_REC_ID(Values[i]);
            }
        }
        #endregion _REC_ID
        //=======================================================
        //  _REC_PD
        //=======================================================
        #region _REC_PD
        ///------------------------------------------------------
        /// <summary> The _REC_PD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _REC_PD;

        ///------------------------------------------------------
        /// <summary> Gets the _REC_PD  </summary>
        ///<returns> the _REC_PD </returns>

        public int[] geti_REC_PD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_REC_PD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _REC_PD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_REC_PD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_REC_PD(Values[i]);
            }
        }
        #endregion _REC_PD
        //=======================================================
        //  _SAL_UD
        //=======================================================
        #region _SAL_UD
        ///------------------------------------------------------
        /// <summary> The _SAL_UD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SAL_UD;

        ///------------------------------------------------------
        /// <summary> Gets the _SAL_UD  </summary>
        ///<returns> the _SAL_UD </returns>

        public int[] geti_SAL_UD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SAL_UD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SAL_UD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SAL_UD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SAL_UD(Values[i]);
            }
        }
        #endregion _SAL_UD
        //=======================================================
        //  _SAL_AD
        //=======================================================
        #region _SAL_AD
        ///------------------------------------------------------
        /// <summary> The _SAL_AD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SAL_AD;

        ///------------------------------------------------------
        /// <summary> Gets the _SAL_AD  </summary>
        ///<returns> the _SAL_AD </returns>

        public int[] geti_SAL_AD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SAL_AD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SAL_AD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SAL_AD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SAL_AD(Values[i]);
            }
        }
        #endregion _SAL_AD
        //=======================================================
        //  _SAL_ID
        //=======================================================
        #region _SAL_ID
        ///------------------------------------------------------
        /// <summary> The _SAL_ID provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SAL_ID;

        ///------------------------------------------------------
        /// <summary> Gets the _SAL_ID  </summary>
        ///<returns> the _SAL_ID </returns>

        public int[] geti_SAL_ID()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SAL_ID();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SAL_ID  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SAL_ID(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SAL_ID(Values[i]);
            }
        }
        #endregion _SAL_ID
        //=======================================================
        //  _SAL_PD
        //=======================================================
        #region _SAL_PD
        ///------------------------------------------------------
        /// <summary> The _SAL_PD provider property  </summary>
        ///------------------------------------------------------
        public providerArrayProperty _SAL_PD;

        ///------------------------------------------------------
        /// <summary> Gets the _SAL_PD  </summary>
        ///<returns> the _SAL_PD </returns>

        public int[] geti_SAL_PD()
        {
            int ArraySize = FUnitModels.Count;
            int[] result = new int[ArraySize];
            for (int i = 0; i < ArraySize; i++)
            {
                result[i] = FUnitModels[i].geti_SAL_PD();
            }
            return result;
        }


        ///------------------------------------------------------
        /// <summary> Sets a _SAL_PD  </summary>
        /// <param name="Values">   The values. </param>

        public void seti_SAL_PD(int[] Values)
        {
            int ArraySize = FUnitModels.Count;
            if (ArraySize > Values.Length)
            {
                ArraySize = Values.Length;
            }
            for (int i = 0; i < ArraySize; i++)
            {
                FUnitModels[i].seti_SAL_PD(Values[i]);
            }
        }
        #endregion _SAL_PD

#endregion ProviderProperties
    }

}

