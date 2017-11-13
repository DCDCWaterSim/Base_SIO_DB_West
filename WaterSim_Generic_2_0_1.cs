using System;
using System.IO;
//using ReadWriteCsv;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsumerResourceModelFramework;
//using WaterSimDCDC.America;
using UniDB;
using System.Data;

namespace WaterSimDCDC.Generic
{

    //==========================================================================================================================
    //  MODEL CALL BACK HANDLERS 
    //==========================================================================================================================
        
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Executes the run model handler action. </summary>
    ///
    /// <param name="year"> The year. </param>
    ///-------------------------------------------------------------------------------------------------

    public delegate void OnRunModelHandler(int year);

    //==========================================================================================================================
    //  WaterSim CRF Model 
    //==========================================================================================================================
    
    /// <summary>   Water simulation crf model. </summary>
    public class WaterSimCRFModel
    {
        CRF_Unit_Network UnitNetwork;
        
        RateDataClass FRDC;
        UnitData FUnitData = null;
        ScenarioDataClass SDC;

        internal StreamWriter sw;
        DateTime now = DateTime.Now;

        string FUnitName = "";
        int FUnitCode = 0;
        string FComment = "";

        OnRunModelHandler FRunCallBack = null;
        //
        // http://waterdata.usgs.gov/fl/nwis/wu
        // Units
        // Million gallons per day (Mgd)--a rate of flow of water equal to 133,680.56 cubic feet per day, 
        // or 1.5472 cubic feet per second, or 3.0689 acre-feet per day. 
        // A flow of one million gallons per day for one year equals 1,120 acre-feet (365 million gallons).

        /// <summary>
        ///  Constructor for WaterSimAmerica
        /// </summary>
        #region Constructors
        
        static protected bool _isWaterForAmericaInstatiated = false;  // used to keep track if a WaterForAmerica object has been constructed
 
        
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   2/6/2017. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="DataDirectoryName">    Pathname of the data directory. </param>
        /// <param name="TempDirectoryName">    Pathname of the temporary directory. </param>
        ///-------------------------------------------------------------------------------------------------
        public void StreamW(string TempDirectoryName)
        {
            string filename = string.Concat(TempDirectoryName + "\\" + "Output" + "\\" + "Output" + "_" + now.Month.ToString() + "-"
                + now.Day.ToString() + "_" + now.Hour.ToString() + "." + now.Minute.ToString() + now.Second.ToString()
                + ".csv");
            sw = File.AppendText(filename);
        }


        public WaterSimCRFModel(string DataDirectoryName, string TempDirectoryName)
        {

            FUnitName = UDI.DefaultUnitName;

            //StateData = "Just11StatesLakeNoRuralPower.csv";// "Just5StatesLakeNoRural.csv"; //"Just5StatesLake.csv";// "JustSmithStates.csv";// "All_50_states.csv";
            //  String IndustryData = "ElevenStateAnnualIndGrowthRates.csv";
            string rates = "ElevenStateGrowthRates3.csv";

            try
            {
                FRDC = new RateDataClass(DataDirectoryName, rates);
                string StateDataPath = DataDirectoryName + "//" + UDI.USGSDataFilename;
                FUnitData = new UnitData(StateDataPath);
                UnitNetwork = new CRF_Unit_Network(FUnitData, FUnitName);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   2/6/2017. </remarks>
        ///
        /// <param name="TheUnitData">  Information describing the unit. </param>
        /// <param name="TheRateData">  Information describing the rate. </param>
        /// <param name="TheUnitName">  Name of the unit. </param>
        ///-------------------------------------------------------------------------------------------------

        public WaterSimCRFModel(UnitData TheUnitData, RateDataClass TheRateData, string TheUnitName)
        {
            FUnitName = TheUnitName;
            string errMsg = "";
            FRDC = TheRateData;
            FUnitData = TheUnitData;
            //StreamW(DataDirectoryName);

            if (FUnitData.GetValue(TheUnitName, UDI.UnitCodeField, out FUnitCode, out errMsg))
            {
                // All good
            }
            else
            {
                // Not So Good 
                FComment = "Unit Code : "+errMsg;
            }
            UnitNetwork = new CRF_Unit_Network(TheUnitData, TheUnitName);

            initialize_FirstRun();
          //  isInitialized = true;
        }
        public WaterSimCRFModel(UnitData TheUnitData, RateDataClass TheRateData, ScenarioDataClass TheScenarioData, string TheUnitName)
        {
            FUnitName = TheUnitName;
            string errMsg = "";
            FRDC = TheRateData;
            FUnitData = TheUnitData;
            SDC = TheScenarioData;
            //StreamW(DataDirectoryName);

            if (FUnitData.GetValue(TheUnitName, UDI.UnitCodeField, out FUnitCode, out errMsg))
            {
                // All good
            }
            else
            {
                // Not So Good 
                FComment = "Unit Code : " + errMsg;
            }
            //

            //
            UnitNetwork = new CRF_Unit_Network(TheUnitData, TheUnitName);

            initialize_FirstRun();
            //  isInitialized = true;
            alterManagement();
        }
        protected void alterManagement()
        {
           
           // double Check = SDC.FastUrbanCon(4);
            double check = SDC.FastUrbanCon(10);
           
        }
        protected bool WaterAmerica
        {
            get { return _isWaterForAmericaInstatiated; }
            set { _isWaterForAmericaInstatiated = value; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Dispose of this object, cleaning up any resources it uses. </summary>
        ///
        /// <param name="disposing">    true if resources should be disposed, false if not. </param>
        ///-------------------------------------------------------------------------------------------------

        protected virtual void Dispose(bool disposing)
        {
            _isWaterForAmericaInstatiated = false;
            if (disposing)
            {
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Performs application-defined tasks associated with freeing, releasing, or
        ///             resetting unmanaged resources. </summary>
        ///
        /// <seealso cref="System.IDisposable.Dispose()"/>
        ///-------------------------------------------------------------------------------------------------

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the crf network. </summary>
        ///
        /// <value> the crf network. </value>
        ///-------------------------------------------------------------------------------------------------

        public  CRF_Unit_Network  TheCRFNetwork
        {
            get { return UnitNetwork; }
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

        public int unitCode
        {
            get { return FUnitCode; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets the on run handler. </summary>
        ///
        /// <value> The on run handler. </value>
        ///-------------------------------------------------------------------------------------------------

        public OnRunModelHandler OnRunHandler
        {
            set { FRunCallBack = value; }
        }        

        #endregion
        // --------------------------------------------
        // ====================================================================================
        // ====================================================================================

        internal void initialize_FirstRun()
        {
            demand_total = 0;
            GPCD = 0;
            initProportions();
            //
            OldPopulation = population;
            maxReclaimed = 25;
            // 02.01.17 added reset below
            ResetVariables();
            populationRatio = 1;
            Desalinization = 1;
            AgricultureTargetGPDD = 100; // 03.12.17 das added because the dampener was not working.
        }
 
        // ====================================================================================
        // New Access Fields
        /// <summary>
        /// State Indexing.This code grabs the string and the index for the State being examined.
        /// And, it resets the Network
        /// </summary>
        #region State Examined
        //public const int FNumberOfStates = 5;
        //string[] FStateNames = new string[FNumberOfStates] { "Florida", "Idaho", "Illinois", "Minnesota", "Wyoming" };

        //public int FStateIndex = 0;
        //public string FStateName = "Florida";
        //public void seti_StateIndex(int value)
        //{
        //    if ((value > -1) && (value < WaterSimManager.FNumberOfStates))
        //    {
        //        FStateIndex = value;
        //        FStateName = WaterSimManager.FStateNames[value];
        //        UnitNetwork.unitName = FStateName;
        //        //WSA.StateName = FStateName;
        //    }
        //}
        //public int geti_StateIndex()
        //{
        //    return FStateIndex;
        //}


        #endregion
        //
        public int FYearIndex = 0;
        public void seti_YearIndex(int value)
        {
            if ((value > -1) && (value < 51))
            {
                FYearIndex = value;
            }
        }
        public int geti_YearIndex()
        {
            return FYearIndex;
        }
        // -------------------------
        //
        #region Reset Network and variables
        public void ResetNetwork()
        {
            UnitNetwork.ChangeUnit(FUnitName);
            //seti_StateIndex(FStateIndex);
            ResetVariables();
            if (FRunCallBack != null)
            {
                FRunCallBack(-1);


            }
        }
        public void ResetVariables()
        {
            // 
            int temp = 100;
            int zero = 0;
            //
            seti_PowerConservation(temp);
            seti_AgConservation(temp);
            seti_UrbanConservation(temp);
            seti_PopGrowthRate(temp);
            //
            seti_SurfaceWaterControl(temp);
            seti_GroundwaterControl(temp);
            seti_Effluent(zero);
            seti_ReclaimedWaterManagement(zero);
            seti_LakeWaterManagement(temp);
            seti_Desalinization(temp);
            //
            // 08.03.17 das
            //OldPopulation = 0;
            InitialPower(FUnitName);
            seti_PowerEnergy(initialPower);
            invokeEffluent = true;
            // QUAY 2/6/17
            SetInitialRates();
            SetInitialDemandFactors();

        }
        #endregion
        //
        // ====================================================================================
        // MODEL
        //
        /// <summary>   Sets the initial rates. </summary>
        /// <remarks> Quay 2/6/17  This gets the initial rates from the RateData class</remarks>
        void SetInitialRates()
        {
            FAgRate = FRDC.FastAgRate(FUnitName);
            FIndRate = FRDC.FastIndRate(FUnitName);
            FPopRate = FRDC.FastPopRate(FUnitName);
        }

        void SetInitialDemandFactors()
        {
            // Set up Agriculture Production
            // Get Production - only called for 2016 ....????? das
            FAgNet = FRDC.FastAgNet(FUnitName);
            // Get Water Demand
            double AgWater = TheCRFNetwork.Agriculture.Demand;
            // Calculate Production Efficiency Coeficient
            AgricultureInitialGPDD = AgWater / FAgNet;  


        //    double FBaseAgDemand =  TheCRFNetwork.Agriculture;
        //    // Calcualte Base $ per Gallon
        //    double FBaseAGGallonPerDollar = FBaseAgDemand / FBaseAGNet;

        //    FBasePercent = (FBaseAGGallonPerDollar / AG_National_MAX_GPDD) * AG_National_Shift;

        //    (WSim as WaterSimManager_SIO).WaterSimAmericaModel.AgricultureGrowthRate = FAGRate;
        //    (WSim as WaterSimManager_SIO).WaterSimAmericaModel.AgricultureInitialGPDD = FBaseAGGallonPerDollar;
        //    (WSim as WaterSimManager_SIO).WaterSimAmericaModel.AgricultureNet = FBaseAGNet;
        }


        //-------------------------------------------------

        
        
        public int runOneYear(int year)
        {
            currentYear = year;
            startUp(year);
            Model(year);
            cleanUp(year);
            // CallBack if set
            FRunCallBack?.Invoke(year);
            return 0;
        }
        internal void startUp(int year)
        {
            // 09.12.16
            CalculateNewPopulation(year, StartYear);
        }
        internal void cleanUp(int year)
        {
            if (year == endYear)
            {
                int temp = geti_NetDemandDifference();
                tearDown(year);
            }
        }
        internal void tearDown(int year)
        {
            //sw.Flush();
            // sw.Close();
        }
        // ===========================
        //
        #region Model kernel Calls
        //
        int baseyear = 2015;
        internal void Model(int year)
        {
            seti_YearIndex(year - baseyear);
            //
            currentYear = year;
            initializeRun();
            preProcess(year);
            annual_delta();
            initialize_run_network();
            analyze_results();
            postProces(year);
            //            
        }
        void initializeRun()
        {
            populationRatio = 0;
            demand_total = 0;
            initProportions();
        }
        internal void annual_delta()
        {
            surfaceFresh();
            surfaceSaline();
            surfaceLake();
            groundwater();
            effluent();
        }
        internal void initialize_run_network()
        {
            Urban();
            Agriculture();
            PowerWater();
            PowerEnergy();
            Industrial();
        }
        // 12.19.16 das following code
        // ---------------------------
        void preProcess(int yr)
        {
            // 12.19.16 added
            if(yr == policyStartYear)
            invokePolicies = true;

        }
        // ===========================
        void postProces(int yr)
        {
            int temp = 0;
            temp = population;
            if (yr == startYear) startPop = temp;
            OldPopulation = temp;
            reset_Drivers(yr);
            //invokeEffluent = false; // One year - 2015, where the value of the added effluent is set

        }
        internal void analyze_results()
        {
            int GPCDurban = urbanGPCD;
            int GPCDindustry = industrialGPCD;
            int GPCDag = agriculturalGPCD;
            int GPCDpower = powerGPCD;
        }
        internal void reset_Drivers(int yr)
        {
            if (yr == policyStartYear)
            {
                int one = 100;
                int zero = 0;
                seti_SurfaceWaterControl(one);
                seti_GroundwaterControl(one);
                seti_Effluent(zero);
                seti_LakeWaterManagement(one);
                seti_Desalinization(one);
            }
        }

        #endregion
        //
        // =============================================================================================
        //
        // ---------------------------------------------------------------------------------------------
        // Resources - State definitions and management actions on resources
        // ==================================================================
        #region Resources
        void surfaceFresh()
        {
            int one = 100;
            double temp = 0;
            int result = 0;
            temp = geti_SurfaceWaterFresh() * d_surfaceWaterControl;
            if (startDroughtYear <= currentYear)
                temp = geti_SurfaceWaterFresh() * d_surfaceWaterControl * d_drought;

            result = Convert.ToInt32(temp);
            seti_SurfaceWaterFresh(result);
            if (startDroughtYear <= currentYear) seti_DroughtControl(one);
        }
        void surfaceSaline()
        {
            double temp = 0;
            int result = 0;
            //temp = geti_SurfaceWaterSaline() * _desalinization;
            temp = geti_SurfaceWaterSaline() * Desalinization;
            result = Convert.ToInt32(temp);
            result = (int)temp;
            seti_SurfaceWaterSaline(result);
        }
        void surfaceLake()
        {
            double temp = 0;
            int result = 0;
            //temp = geti_SurfaceLake() * LWManagement;
            temp = geti_SurfaceLake() * d_lakeWaterManagement;
            result = Convert.ToInt32(temp);
            seti_SurfaceLake(result);
        }
        void groundwater()
        {
            double temp = 0;
            int result = 0;
            temp = geti_Groundwater() * d_groundwaterControl;
            result = (int)temp;
            seti_Groundwater(result);
        }
        void effluent()
        {
            double temp = 0;
            int result = 0;
            temp = geti_Effluent();
            result = (int)temp;
            seti_Effluent(result);
        }
        //
        #endregion
        // ---------------------------------------------------------------------------------------------
        //
        // ---------------------------------------------------------------------------------------------
        // Consumers - State definitions and Management Actions on the consumers
        // =====================================================================
        //
        const double convertDemand = 1000000;

        #region Water Demand
        // Community Demand UrbanConservation
        #region Community Demand
        // -------------------------
        void Urban()
        {
            double temp = 0;
            int result = geti_Urban();
            temp = weightedGrowthPopulation(result) * modifyDemandCF();
            if (invokePolicies)
            {
                temp = 0;
                try
                {
                    temp = alterGrowthConservation(geti_Urban(), UrbanConservation) * modifyDemandCF();
                }
                catch (Exception ex)
                {
                    // Ouch
                }
            }
            result = Convert.ToInt32(temp);
            seti_Urban(result);
            i_demand_urban = result;
        }
        int urbanGPCD
        {
            get
            {
                //double temp = WSA.Urban.Demand * convertDemand;
                double temp = UnitNetwork.Urban.Demand * convertDemand;
                double pop = population;
                double gpcd = 0;
                try
                {
                    if (0 < pop) gpcd = temp / pop;
                    // comparison estimates
                }
                catch (Exception ex) { }
                return Convert.ToInt32(gpcd);
            }
        }
        #endregion
        // ----------------

        // AGRICULTURE PRODUCTION AND DEMAND
        // ----------------
        # region Agriculture Demand
        //  
        //  Added Quay 3/3/16
        //#########################################################

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Dampen rate of growth. </summary>
        /// <remarks>   This function reduces the annual rate of growth based on the period from start 
        ///             of growth to current year.  The dampening of the rate increases as the period increases</remarks>
        /// <param name="rate">     The base annual rate. </param>
        /// <param name="damper">   A Factor for the strength of the dampening.  1 is no dampening, 1.0001 is slight dampening
        ///                         1.1 is really fast, anything larger is insane. </param>
        /// <param name="period">   The period. </param>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        double DampenRate(double rate, double damper, double period)
        {
            double NewRate = 0;
            NewRate = rate * Math.Pow(damper, -1 * period);
            return NewRate;
        }


        // need some default values here
        double FAgNet = 0;
        double FAgRate = 0;
        double FAgBaseGPDD = 0;
        double FAgTargetGPDDReduction = 0;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the agriculture net. </summary>
        /// <remarks> This is the Base net Form income for the Should not vary from year to year</remarks>
        /// <value> The agriculture net. </value>
        ///-------------------------------------------------------------------------------------------------

        public double AgricultureNet
        {
            get { return FAgNet; }
            set { FAgNet = value; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the agriculture growth rate. </summary>
        /// <remarks>  This is the annual rate of growth projected for agricultural net farm inccome.
        ///            This is as a percent ie 2 = 2% or a rate of .02</remarks>
        /// <value> The agriculture growth rate (percent). </value>
        ///-------------------------------------------------------------------------------------------------
        // AGCON is the policy control for agriculture, scaled from 50 to 100 (100 is no change)
        // 03.04.2016 DAS
        //
        public double AgricultureGrowthRate
        {
            get { return FAgRate; }
            set { FAgRate = value; }
        }
        // 03.04.26 DAS
        // need a separate rate controller that uses this variable, but cannot alter AgricultureGrowthRate
        // directly. Can be initialized and used in the model
        //
        double _agGrowthRate = 0.5;
        public int geti_AgGrowthRate()
        {
            int TempInt = Convert.ToInt32(Math.Round(_agGrowthRate * 100));
            return TempInt;
        }
        public void seti_AgGrowthRate(int value)
        {
            _agGrowthRate = (Double)value / 100;
        }
        //
        double AgGrowthRate
        {
            get { return _agGrowthRate; }

        }
        // -------------------------------------------------------------------------

        double _agProductionRate = 1.0;
        public int geti_AgProductionRate()
        {
            int TempInt = Convert.ToInt32(Math.Round(_agProductionRate * 100));
            return TempInt;
        }
        public void seti_AgProductionRate(int value)
        {
            _agProductionRate = (Double)value / 100;
        }
        double AgProductionRate
        {
            get { return _agProductionRate; }

        }
        //
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the agriculture gpdd. </summary>
        /// <remarks>  This is the initial base GPDD and should not change over time.
        ///            
        ///            </remarks>
        /// <value> The agriculture Gallons per One $ of Net Farm Income per Day (gpdd). </value>
        ///-------------------------------------------------------------------------------------------------

        public double AgricultureInitialGPDD
        {
            get { return FAgBaseGPDD; }

            set { FAgBaseGPDD = value; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the agriculture target reduction. </summary>
        /// <remarks>  This is the Target annual GPDD and should not change over time.
        ///            This is a Percent, 100 is 100% or no change in GPDD            </remarks>
        ///
        /// <value> The agriculture target Percent reduction in Gallons per One $ of Net Farm Income per Day (gpdd) </value>
        ///-------------------------------------------------------------------------------------------------

        public double AgricultureTargetGPDD
        {
            get { return FAgTargetGPDDReduction; }
            set { FAgTargetGPDDReduction = value; }
        }


        const double Damper = 1.01;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Calculates the ag production. </summary>
        ///
        /// <param name="theCurrentYear">   the current year. </param>
        /// <param name="theStartYear">     the start year. </param>
        ///
        /// <returns>   The calculated ag production. </returns>
        ///-------------------------------------------------------------------------------------------------

        public int Calc_AgProduction(int theCurrentYear, int theStartYear)
        {
            int result = 0;
            // calculate term
            double period = (theCurrentYear - theStartYear) + 1;
            // get adjusted growth rate
            //// dampen the rate
            double rate = AgricultureGrowthRate;
            // calculate the new $ production
            double AgNet = AgricultureNet;

            int temp = 0;
            try
            {
                double CurrentRate = DampenRate(AgricultureGrowthRate / 100, Damper, period);
                double NewProduction = Math.Round(AgNet * Math.Pow(1 + (CurrentRate * AgProductionRate), period));
                temp = Convert.ToInt32(NewProduction);
                result = temp;
            }
            catch (Exception ex)
            {
                // Ouch
            }
            return result;
        }

        // holder for GPDD
        double FAdjAGPCD = 0;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti ag demand. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------
        double _retainAg = 0;
        double RetainAgDemand
        {
            get { return _retainAg; }
            set { _retainAg = value; }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Calculates the ag demand. </summary>
        ///
        /// <param name="theCurrentYear">   the current year. </param>
        /// <param name="theStartYear">     the start year. </param>
        /// <param name="NewProduction">    The new production, ie production from last year. </param>
        /// <param name="modDemand">        The modifier demand. Not currently used! </param>
        ///
        /// <returns>   The calculated ag demand. </returns>
        ///-------------------------------------------------------------------------------------------------

        public int Calc_AgDemand(int theCurrentYear, int theStartYear, double NewProduction, double modDemand)
        {
            // QUAY 2 8 17
            // The original code was not working, in order to debug rebuilt the code to be simple annual increase 
            // THis code will have to be later tweaked to allow for efficiency changes and to dampen the growth curve
            // 
            //int result = 0;
            //double InitialAGPD = AgricultureInitialGPDD;
            //double AdjustedProduction = 0;
            //double NewAgDemand = 0;
            //double AgGrowthFactor = 1 + (FAgRate / 100);
            //if (theCurrentYear == theStartYear)
            //{
            //    AdjustedProduction = NewProduction;
            //}
            //else
            //{
            //    try
            //    {
            //        double period = (theCurrentYear - theStartYear) + 1;

            //        AdjustedProduction = NewProduction * AgGrowthFactor;

            //    }
            //    catch (Exception ex)
            //    {
            //    }
            //}

            //NewAgDemand = AdjustedProduction * AgricultureInitialGPDD;

            //result = Convert.ToInt32(NewAgDemand);
            //==============================================================

            int result = 0;
            double final = 0;
            double temp = 0;
            // get the Initial Gallons per Dollar (ie assume it does not change; should insert some code to change this
            double InitialAGPD = AgricultureInitialGPDD;
            // Calculate new demand based on new production and AGPD
            // Adjust AGPCD
            double period = (theCurrentYear - theStartYear) + 1;
            if (theCurrentYear <= theStartYear) { RetainAgDemand = (UnitNetwork.Agriculture.Demand); }
            //if (theCurrentYear <= theStartYear) { RetainAgDemand = (WSA.Agriculture.Demand); }
            try
            {
                double CurDamper = 1 + ((100.0 - (double)FAgTargetGPDDReduction) / 4000);
                FAdjAGPCD = DampenRate(InitialAGPD, CurDamper, period);
                // das
                //double ttemp = WSA.Agriculture.Demand;
                double ttemp = UnitNetwork.Agriculture.Demand;

                double NewAgDemand = NewProduction * FAdjAGPCD;
                double CurrentRate = DampenRate(FAgRate / 100, Damper, period);
                double newAgproduction = Math.Round(ttemp * Math.Pow(1 + (CurrentRate * AgGrowthRate), period));
                // NOTE I see on 01.03.17 that AgGrowthRate has no current effect on Agriculture
                // DAS- and newAgproduction is NOT being called - AgProductionRate would need to be modified
                // to have any growth effects on Agriculture (at this time)
                //
                double Difference = NewAgDemand - RetainAgDemand;// ttemp;
                //if (modDemand < 1) modDemand = correctMod(modDemand);
                //double temp = ttemp + (Difference * Math.Min(1.0, Math.Max(minMod, modDemand)));
                temp = ttemp + (Difference * modDemand);
                RetainAgDemand = NewAgDemand;
                if (theCurrentYear <= theStartYear) temp = NewAgDemand;

            }
            catch (Exception ex) { }
            //
            final = (temp);
            result = Convert.ToInt32(final);

            //sw.WriteLine(currentYear
            //+ ","

            //+ result
            //);
            return result;
        }

        // holder for ag production
        int FAgProduction = 0;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the agricuture production. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_AgricutureProduction()
        {
            return FAgProduction;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the agriculture gpdd. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Agriculture_GPDD()
        {
            int tempGPCD = 200;
            int result = 0;
            try
            {
                // alert 02.20.17 das this MUST BE corrected
                //int tempi = Convert.ToInt32(FAdjAGPCD);
                int tempi = Convert.ToInt32(tempGPCD);
                result = tempi;
            }
            catch (Exception ex)
            {
                // ouch
            }
            return result;
        }

        /// <summary>   Estimate Agriculture demand </summary>
        void Agriculture()
        {
            int result = 0;
            double temp = 0;
            // get Ag production
            FAgProduction = Calc_AgProduction(currentYear, startYear);
            // now estmate demand based on production
            temp = Calc_AgDemand(currentYear, startYear, FAgProduction, AgConservation) * modifyDemandCF();
            result = (int)temp;
            // set the parameter for AgDemand
            seti_Agriculture(result);
            // Set the parameter for AgGPDD
            //i_demand_ag = result;
            //
            //if (this.FUnitName == "CA_Urban")
            //{
            //    sw.WriteLine(currentYear
            //    + ","
            //    + FAgProduction
            //    + ","
            //    + FAdjAGPCD
            //    + ","

            //    + result
            //    );
            //}
        }


        int agriculturalGPCD
        {
            get
            {
                double temp = UnitNetwork.Agriculture.Demand * convertDemand;
                //double temp = WSA.Agriculture.Demand * convertDemand;
                double pop = population;
                double gpcd = 0;
                try
                {
                    if (0 < pop) gpcd = temp / pop;
                    // For comparison
                    int t = Convert.ToInt32(Convert.ToDouble(GPCD) * Proportion_Waterdemand_Ag);
                }
                catch (Exception ex) { }
                return Convert.ToInt32(gpcd);
            }
        }
        #endregion
        // ----------------

        // ----------------
        // Power Water
        # region Power Demand
        void PowerWater()
        {
            double temp = 0;
            int result = 0;
            temp = weightedGrowthPopulation(geti_PowerWater()) * modifyDemandCF();
            //
            if (invokePolicies)
            {
                temp = 0;
                try
                {
                    temp = alterGrowthConservation(geti_PowerWater(), PowerConservation) * modifyDemandCF();
                }
                catch (Exception ex)
                {
                    // opps
                }
            }
            result = (int)temp;
            seti_PowerWater(result);
            i_demand_power = result;
        }
        // das march
        int powerGPCD
        {
            get
            {
                double temp = UnitNetwork.Power.Demand * convertDemand;
                //double temp = WSA.Power.Demand * convertDemand;
                double pop = population;
                double gpcd = 0;
                if (0 < pop)
                {
                    gpcd = temp / pop;
                }

                int t = Convert.ToInt32(Convert.ToDouble(GPCD) * Proportion_Waterdemand_Power);
                return Convert.ToInt32(gpcd);
            }
        }
        // ----------------
        // Power Energy
        void PowerEnergy()
        {
            double temp = 0;
            int result = 0;
            temp = weightedGrowthPopulation(geti_PowerEnergy()) * modifyDemandCF();
            result = Convert.ToInt32(temp);
            seti_PowerEnergy(result);
        }
        #endregion
        // ----------------

        // ----------------
        // Industry
        #region Industry Demand

        // NEW CODE AS OS 09.06.16 das 
        double _indProduction;
        double IndProduction
        {
            get { return _indProduction; }
            set { _indProduction = value; }
        }
        //
        // need some default values here
        double FIndRate = 0;
        // double IndDamper = 1.2;
        const double DamperF = 0.9;
        public double IndustrialGrowthRate
        {
            //get { return RDC.INDRate(FStateIndex); }
            get { return FRDC.FastIndRate(FUnitName); }
            set { FIndRate = value; }

        }
        internal double correctMod(double dataIn)
        {
            double temp = 0;
            double result = 0;
            const double slope = -0.267;
            const double intercept = 1.265;
            temp = slope * dataIn + intercept;
            result = temp * dataIn;
            return result;

        }
        const double minMod = 0.5;
        public int Calc_IndDemand(int theCurrentYear, int theStartYear, double modDemand)
        {
            int result = 0;
            int Industry2015 = 0;

            double final = 0;
            double IndustrialDamper = 1;
            double DampenTheRate = 1;
            double useValue = 1.2;
            double modValue = 0.53;
            double minTheRate = 1.15;
            double temp = 0;
            try
            {
                IndustrialDamper = Math.Max(IndustrialGrowthRate * modValue, useValue);
                DampenTheRate = Math.Min(IndustrialGrowthRate / 2, minTheRate);
                if (theCurrentYear <= theStartYear) { Industry2015 = geti_Industrial(); }

                double period = (theCurrentYear - theStartYear) + 1;
                double dampedRate = DampenRate(IndustrialGrowthRate / 100, IndustrialDamper, period);
                double ttemp = Convert.ToInt32(Math.Round(UnitNetwork.Industrial.Demand));
//                double ttemp = Convert.ToInt32(Math.Round(WSA.Industrial.Demand));
                // Generic code from here down
                double newDemand = Math.Round(ttemp * Math.Pow(1 + (dampedRate * DampenTheRate), period));
                double Difference = newDemand - ttemp;
                if (modDemand < 1) modDemand = correctMod(modDemand);
                temp = ttemp + (Difference * Math.Min(1.0, Math.Max(minMod, modDemand)));
                if (theCurrentYear <= theStartYear) temp = newDemand;
            }
            catch (Exception ex) { }
            final = (temp);
            result = Convert.ToInt32(final);
            //

            //sw.WriteLine(currentYear
            // + ","
            // + ttemp
            // + ","
            // + newDemand
            // + ","
            // + indDifference
            // + ","
            // + indDifference * modDemand
            // + ","
            // + result
            // );
            return result;
        }
        // End Of New Code 09.06.16

        void Industrial()
        {
            double temp = 0;
            int result = 0;
            double preInvoke = 1.0;
            temp = Calc_IndDemand(currentYear, startYear, preInvoke);
            result = (int)temp;
            if (invokePolicies)
            {
                temp = Calc_IndDemand(currentYear, startYear, IndustryConservation);
            }
            result = Convert.ToInt32(temp);
            seti_Industrial(result);
            i_demand_industry = result;
        }
        int industrialGPCD
        {
            get
            {
                double temp = UnitNetwork.Industrial.Demand * convertDemand;
                //double temp = WSA.Industrial.Demand * convertDemand;
                double pop = population;
                double gpcd = 0;
                if (0 < pop) gpcd = temp / pop;
                // for comparison only
                int t = Convert.ToInt32(Convert.ToDouble(GPCD) * Proportion_Waterdemand_Industry);
                return Convert.ToInt32(gpcd);
            }
        }
        #endregion
        // ----------------

        // ----------------
        #region GPCD and modify Demand
        internal int geti_gpcd()
        {
            return urbanGPCD;
        }
        internal int geti_gpcdAg()
        {
            return agriculturalGPCD;
        }
        internal int geti_gpcdOther()
        {
            return powerGPCD + industrialGPCD;
        }
        // -------------------------------------------------------------
        int ModifyDemand(double demand, double parm)
        {
            int result = Convert.ToInt32(demand);
            const double b = 0.98;
            if (0.5 <= parm)
            {
                double DifYear = (endYear - startYear);
                double temp = 0;
                double a = 1 / Math.Sqrt(parm);
                if (0 < DifYear)
                {
                    double touch = a * b * ((1 - parm) / DifYear);
                    temp = demand - (demand * touch);
                    result = Convert.ToInt32(Math.Round(temp));
                }
            }
            return result;
        }
        // -------------------------------------------

        // -------------------------------------------------------------
        int ModifyDemandIndustry(double demand, double parm)
        {
            int result = 0;




            return result;
        }
        // -------------------------------------------
        //internal struct MODdemand
        //{
        //    public string urban;
        //    public string power;
        //    public string agriculture;
        //    public string industry;
        //}
        //internal static void myConsumers(MODdemand m)
        //{
        //    m.urban = "Urban";
        //    m.power = "Power";
        //    m.agriculture = "Ag";
        //    m.industry = "Industry";
        //}

        int _startYear = 0;
        public int startYear
        {
            set { _startYear = value; }
            get { return _startYear; }
        }
        //
        int _Sim_CurrentYear = 0;
        public int currentYear
        {
            set { _Sim_CurrentYear = value; }
            get { return _Sim_CurrentYear; }
        }
        int _endYear = 0;
        public int endYear
        {
            set { _endYear = value; }
            get { return _endYear; }
        }
        int _policyStartYear = 2015;
        public int policyStartYear
        {
            set { _policyStartYear = value; }
            get { return _policyStartYear; }
        }
        //

        // =========================================
        // -------------------------------------------------------------
        double modifyDemandCF()
        {
            double result = 1;
            double cf = geti_DroughtControl();
            if (cf * 0.01 < 1)
            {
                result = utilities.hyperbola(cf);
            }
            return result;
        }
        #endregion
        // ----------------

        //
        // ------------------------
        // use lower case
        // derived
        // send to WaterSimith Manager
        public int i_demand_urban;
        public int i_demand_rural;
        public int i_demand_ag;
        public int i_demand_power;
        public int i_demand_industry;
        //
        #endregion
        // 
        //public int sustainability_surface_water;
        //public int sustainability_groundwater;
        //public int sustainability_economic;
        // -------------------------------------------------
        // proportions
        // -------------------------------------------------
        #region Proportional demand
        //
        double _proportion_waterdemand_urban;
        double _proportion_waterdemand_ag;
        double _proportion_waterdemand_industry;
        double _proportion_waterdemand_power;
        //
        // ======================================================================================
        // May want to use the parameter that Ray created...
        //  ParamManager.AddParameter(new ModelParameterClass(eModelParam.epTotalDemand, "Total Demand", "TD", geti_TotalDemand));
        double d_demand_total = 0;
        double demand_total
        {
            get
            {
                return d_demand_total;
            }
            set
            {
                double temp = value;
                temp = (geti_Urban() + geti_Agriculture() + geti_PowerWater() + geti_Industrial());
                d_demand_total = temp;
            }
        }

        int _gpcd = 0;
        const double MGDtogal = 1000000;
        public int GPCD
        {
            set
            {
                double temp = value;
                if (0 < population)
                    temp = (d_demand_total * MGDtogal) / Convert.ToDouble(population);
                _gpcd = Convert.ToInt32(temp);
            }
            get { return _gpcd; }
        }

        // ======================================================================================
        //
        void initProportions()
        {
            Proportion_Waterdemand_Urban = 0;
            //Proportion_Waterdemand_Rural = 0;
            Proportion_Waterdemand_Ag = 0;
            Proportion_Waterdemand_Power = 0;
            Proportion_Waterdemand_Industry = 0;
        }
        //
        internal double Proportion_Waterdemand_Urban
        {
            get { return _proportion_waterdemand_urban; }
            set
            {
                double temp = value;
                if (0 < d_demand_total)
                {
                    temp = Convert.ToDouble(geti_Urban()) / Convert.ToDouble(d_demand_total);
                    temp = utilities.RoundToSignificantDigits(temp, 3);
                }
                //value = temp;
                _proportion_waterdemand_urban = temp;
            }
        }
        internal double Proportion_Waterdemand_Ag
        {
            get { return _proportion_waterdemand_ag; }
            set
            {
                double temp = value;
                if (0 < d_demand_total)
                {
                    temp = Convert.ToDouble(geti_Agriculture()) / Convert.ToDouble(d_demand_total);
                    temp = utilities.RoundToSignificantDigits(temp, 3);
                }
                _proportion_waterdemand_ag = temp;
            }
        }
        internal double Proportion_Waterdemand_Industry
        {
            get { return _proportion_waterdemand_industry; }
            set
            {
                double temp = value;
                //temp = 1 - (Proportion_Waterdemand_Urban + Proportion_Waterdemand_Rural + Proportion_Waterdemand_Ag + Proportion_Waterdemand_Power);
                temp = 1 - (Proportion_Waterdemand_Urban + Proportion_Waterdemand_Ag + Proportion_Waterdemand_Power);
                _proportion_waterdemand_industry = temp;
            }
        }
        internal double Proportion_Waterdemand_Power
        {
            get { return _proportion_waterdemand_power; }
            set
            {
                double temp = value;
                if (0 < d_demand_total)
                {
                    temp = Convert.ToDouble(geti_PowerWater()) / Convert.ToDouble(d_demand_total);
                    temp = utilities.RoundToSignificantDigits(temp, 3);
                }
                _proportion_waterdemand_power = temp;
            }
        }
        #endregion
        // -------------------------------------------------
        // Directory Control
        // -------------------------------------------------
        #region Website directory faking
        private static string DataDirectoryName
        {
            get
            {
                return @"App_Data\";
            }
        }

        private static string TempDirectoryName
        {
            set
            {
                string dir = value;
                string.Concat(@"WaterSmith_Output\", dir);
            }
            get
            {
                // Make a common for testing
                return @"WaterSmith_Output\";
                // Make the temp directory name unique for each access to avoid client clashes
                //return +System.Guid.NewGuid().ToString() + @"\";
            }
        }
        private static void CreateDirectory(string directoryName)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(directoryName);
            if (!dir.Exists)
            {
                dir.Create();
            }
        }
        #endregion

        // -------------------------------------------------
        // Population
        // -------------------------------------------------
        #region Population
        // ------------
        //


        // NEW CODE AS OF 09.08.16 das 
        //
        double PopAdj = 1.4;
        double PopDamper = 1.01;
        double FPopRate = 0;
        public double PopulationGrowthRate
        {
            get { return FRDC.FastPopRate(FUnitName); }
            //get { return RDC.POPRate(FStateIndex); }
            set { FPopRate = value; }
        }
        //
        //
        public int population
        {
            get { return Convert.ToInt32(Math.Round(UnitNetwork.Population.CurrentState)); }
            //get { return Convert.ToInt32(Math.Round(WSA.Population.CurrentState)); }
            set { UnitNetwork.Population.CurrentState = value; }
            //set { WSA.Population.CurrentState = value; }
        }
        int _initialPopulation = 0;
        int Pop2015
        {
            set { _initialPopulation = value; }
            get { return _initialPopulation; }

        }
        double _holdPopulation = 0;
        double PopRunning
        {
            set { _holdPopulation = value; }
            get { return _holdPopulation; }

        }

        public int CalculateNewPopulation(int theCurrentYear, int theStartYear)
        {
            double final = 0;
            int result = 0;
            //
            if (theCurrentYear <= theStartYear) { Pop2015 = population; PopRunning = Pop2015; }
            double period = (theCurrentYear - theStartYear) + 1;
            try
            {
                double dampedRate = DampenRate((PopulationGrowthRate / 100), PopDamper, period);
                double newPop = Math.Round(Pop2015 * Math.Pow(1 + (dampedRate * PopAdj), period));
                double popDifference = newPop - PopRunning;
                PopRunning = newPop;
                //double temp = population + (popDifference * AdjustPopulation);
                // 08.07.2017 das
                double temp = population + (popDifference * AdjustPopulations);
                if (theCurrentYear <= theStartYear) temp = newPop;
                final = (temp);
            }
            catch (Exception ex) { }
            //
            result = Convert.ToInt32(final);
            population = result;
            return result;
        }
        int _popOld = 0;
        public int OldPopulation
        {
            get { return _popOld; }
            set { _popOld = value; }
        }

        //
        // End New CODE As Of 09.08.16 das
        double _populationRatio = 0;
        double _startPop = 0;
        double startPop
        {
            set { _startPop = value; }
            get { return _startPop; }
        }
        double weightedGrowthPopulation(int institution)
        {
            double temp = 0;
            double result = institution;
            try
            {
                //temp = instituion + Convert.ToInt32(Convert.ToDouble(institution) * populationRatio);
                // 08.03.2017 das
                temp = institution + Convert.ToInt32(Convert.ToDouble(institution) * (populationRatio ));
                result = temp;
            }
            catch (Exception ex) { throw ex; }
            return result;
        }
        double alterGrowthConservation(double institution, double ModifyGrowth)
        {
            double temp = 0;
            double result = institution;
            try
            {
                 temp = institution + Convert.ToInt32(Convert.ToDouble(institution) * populationRatio * ModifyGrowth);
                result = temp;
            }
            catch (Exception ex) { }
            return result;
        }
        double populationRatio
        {
            set
            {
                double temp;
                temp = 0.0;
                if (0 < OldPopulation)
                {
                    double pop = population;
                    double old = OldPopulation;// _oldPop;
                    // if(startPop < old)
                    try
                    {
                        temp = ((pop - old) / old);
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    temp = 0;
                }
                _populationRatio = temp;
            }
            get { return _populationRatio; }
        }
        //public int Get_PopYear(int year)
        //// END QUAY EDIT
        //{
        //    int result = 0;
        //    result = population;
        //    return result;
        //}
        #endregion
        // -------------------------------------------------
        // Gallon Per Capita Per Day
        // -------------------------------------------------
        #region _GPCD
        int _urban_gpcd = 0;
        public virtual int gpcd
        {
            get { return geti_gpcd(); }
            set { _urban_gpcd = value; }
        }
        const int StartYear = 2015;
        const int EndYear = 2050;

        const int RawGPCDDataInc = 5; //Years
        const int NumberGPCDYears = ((EndYear - StartYear) / 5) + 1;

        int[][] GPCDYearData = new int[WaterSimManager.FNumberOfStates][];

        //internal int Get_GPCDYear(int year)
        //{
        //    int TempGPCD = 0;
        //    if (year == 0) year = StartYear;
        //    int ModYear = year % RawGPCDDataInc; ;
        //    if (ModYear == 0)
        //    {
        //        int yearIndex = (year - StartYear) / RawGPCDDataInc;
        //        TempGPCD = GPCDYearData[FStateIndex][yearIndex];
        //    }
        //    else
        //    {
        //        int lowyearindex = ((year - StartYear) - ModYear) / RawGPCDDataInc;
        //        int hiyearindex = lowyearindex + 1;
        //        int lowgpcd = GPCDYearData[FStateIndex][lowyearindex];
        //        int higpcd = GPCDYearData[FStateIndex][hiyearindex];
        //        int GPCDChangeByYear = (higpcd - lowgpcd) / RawPopDataInc;
        //        TempGPCD = lowgpcd + (GPCDChangeByYear * ModYear);


        //    }
        //    return Convert.ToInt32(TempGPCD * d_urbanConservation);
        //}
        const int RawPopDataInc = 5;
        // 02.09.16
        internal int geti_gpcdTotal()
        {
            return GPCD;
        }
        #endregion
        //
        // =================================================
        // Network Parameters

        // CRF_NETWORK PARAMETERS
        // =================================================
        // Population for 2010 adjusted to 2015
        //--------------------------------------------------
        #region UnitNetwork Population
        // This is the method being called by The WaterSim parameter manager
        public int geti_NewPopulation()
        {
            int TempInt = 0;
            TempInt = Convert.ToInt32(Math.Round(UnitNetwork.Population.CurrentState));
            //TempInt = Convert.ToInt32(Math.Round(WSA.Population.CurrentState));

            return TempInt;
        }
        public int seti_NewPopulation
        {
            set { UnitNetwork.Population.CurrentState = value; }
            //set { WSA.Population.CurrentState = value; }
        }
        #endregion

        // Resources
        // -------------------------------------------------
        #region Resources
        //----------------------------------------------
        //  SUrface Water Fresh 
        //-----------------------------------------------

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti surface water fresh. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_SurfaceWaterFresh()
        {
            int TempInt = Convert.ToInt32(Math.Round(UnitNetwork.SurfaceFresh.Limit));
            //int TempInt = Convert.ToInt32(Math.Round(WSA.SurfaceFresh.Limit));
            return TempInt;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti surface water fresh. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_SurfaceWaterFresh(int value)
        {
            UnitNetwork.SurfaceFresh.Limit = value;
            //WSA.SurfaceFresh.Limit = value;
        }
        //

        public int geti_SurfaceWaterFreshNet()
        {
            int TempInt = Convert.ToInt32(Math.Abs(Math.Round(UnitNetwork.SurfaceFresh.Net)) + 0);
            //int TempInt = Convert.ToInt32(Math.Abs(Math.Round(WSA.SurfaceFresh.Net)) + 0);
            return TempInt;
        }


        // -------------------------------------------------------------------------------------------------
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti surface lake From CRF_Network. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_SurfaceLake()
        {
            int result = 0;
            double temp = TheCRFNetwork.SurfaceLake.Limit;
            try
            {
                int tempint = Convert.ToInt32(Math.Round(temp));
                result = tempint;
            }
            catch (Exception ex)
            {
                // ouch
            }
            return result;
        }
        public void seti_SurfaceLake(int value)
        {
            UnitNetwork.SurfaceLake.Value = value;
            //WSA.SurfaceLake.Value = value;
        }
        public int geti_SurfaceLakeNet()
        {
            int result = 0;
            //double temp = WSmith.TheCRFNetwork.SurfaceLake.Limit;
            double temp = TheCRFNetwork.SurfaceLake.Net;
            double temp2 = TheCRFNetwork.SurfaceLake.Limit;
            //
            try
            {
                int tempint = Convert.ToInt32(Math.Abs(Math.Round(temp)) + 0);
                result = tempint;
            }
            catch (Exception ex)
            {
                // ouch
            }
            return result;
        }

        //----------------------------------------------
        // Surface Water Saline
        //-----------------------------------------------

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti surface water saline. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------
        // Changed to mimic Surface Lake on 03.10.16 DAS
        //
        public int geti_SurfaceWaterSaline()
        {
            int result = 0;
            double temp = UnitNetwork.SurfaceSaline.Limit;
            //double temp = WSA.SurfaceSaline.Limit;
            try
            {
                int tempint = Convert.ToInt32(Math.Round(temp));
                result = tempint;
            }
            catch (Exception ex)
            {
                // ouch
            }
            //return result;
            //int TempInt = Convert.ToInt32(Math.Round(WSA.SurfaceSaline.Limit));
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti surface water saline. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_SurfaceWaterSaline(int value)
        {
            UnitNetwork.SurfaceSaline.Limit = value;
            //WSA.SurfaceSaline.Limit = value;
        }
        //
        public int geti_SurfaceWaterSalineNet()
        {

            int result = 0;
            double temp = UnitNetwork.SurfaceSaline.Net;
            double temp2 = UnitNetwork.SurfaceSaline.Limit;
            //double temp = WSA.SurfaceSaline.Net;
            //double temp2 = WSA.SurfaceSaline.Limit;
            try
            {
                int TempInt = Convert.ToInt32(Math.Abs(Math.Round(temp)) + 0);
                result = TempInt;
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        //----------------------------------------------
        // Groundwater
        //-----------------------------------------------

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti groundwater. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Groundwater()
        {
            int TempInt = Convert.ToInt32(Math.Round(UnitNetwork.Groundwater.Limit));
            //int TempInt = Convert.ToInt32(Math.Round(WSA.Groundwater.Limit));
            return TempInt;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti groundwater. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_Groundwater(int value)
        {
            UnitNetwork.Groundwater.Limit = value;
            //WSA.Groundwater.Limit = value;
        }
        //
        public int geti_GroundwaterNet()
        {
            int TempInt = Convert.ToInt32(Math.Abs(Math.Round(UnitNetwork.Groundwater.Net)) + 0);
            //int TempInt = Convert.ToInt32(Math.Abs(Math.Round(WSA.Groundwater.Net)) + 0);
            return TempInt;
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti effluent. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        //----------------------------------------------
        // Effluent
        //-----------------------------------------------

        public int geti_Effluent()
        {
            int TempInt = Convert.ToInt32(Math.Round(UnitNetwork.Effluent.Limit));
            //int TempInt = Convert.ToInt32(Math.Round(WSA.Effluent.Limit));

            return TempInt;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti effluent. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_Effluent(int value)
        {
            UnitNetwork.Effluent.Limit = value;
            //WSA.Effluent.Limit = value;
            if (invokePolicies)
            {
                maxReclaimed = MaxReclaimed();
                // "value" is the default amount for each state. Added is the user-defined
                // request, balanced by the reasonable amount as defined by indoor water use,
                // allowing for some lost due to leaks and efficiencies of production and use
                //WSA.Effluent.Limit = Math.Min(maxReclaimed, (double)value + effluentToAdd);

                UnitNetwork.Effluent.Limit = Math.Min(maxReclaimed, effluentToAdd);
                //WSA.Effluent.Limit = Math.Min(maxReclaimed, effluentToAdd);
                // Added here from elsewhere on 12.14.16 DAS
                // At present "staticEffluentAdd" is not used- 12.19.16
                staticEffluentAdd = effluentToAdd;
            }
        }
        public int geti_EffluentNet()
        {
            int TempInt = Convert.ToInt32(Math.Abs(Math.Round(UnitNetwork.Effluent.Net)) + 0);
            //int TempInt = Convert.ToInt32(Math.Abs(Math.Round(WSA.Effluent.Net)) + 0);
            return TempInt;
        }

        double _maxReclaimed = 0;
        const double maxReclaimedRatio = 0.95;
        //const double consumptive = 0.86; // leaks http://www3.epa.gov/watersense/pubs/indoor.html
        // Need more flexibility- it is a GAME, and we need a response for reclaimed (recycled) water
        const double consumptive = 0.97; // leaks 
        //const double indoor = 0.45;
        const double indoor = 0.65;
        double maxReclaimed
        {
            set { _maxReclaimed = value; }
            get { return _maxReclaimed; }
        }
        double staticEffluent = 0;
        double staticEffluentAdd
        {
            get
            {
                return staticEffluent;
            }
            set
            {
                staticEffluent = value;
            }
        }


        double effluentToAdd
        {
            get
            {
                double temp = 0;
                temp = (d_reclaimedWaterUse) * UnitNetwork.Urban.Demand;
                //temp = (d_reclaimedWaterUse) * WSA.Urban.Demand;
                return Math.Min(maxReclaimed, temp);
            }
        }
        public double MaxReclaimed()
        {
            double temp = 0;
            temp = maxReclaimedRatio * consumptive * ((UnitNetwork.Urban.Demand) * indoor);
            //temp = maxReclaimedRatio * consumptive * ((WSA.Urban.Demand) * indoor);
            return temp;
        }
        // ============================================
        // Gets ans Sets to the AReclaimed Water Management   
        //--------------------------------
        /// <summary>
        /// Seti ReclaimedWaterUse
        /// </summary>
        /// <returns>an int from zero to 100   . </returns>
        double d_reclaimedWaterUse = 0.00;
        public int geti_ReclaimedWaterManagement()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_reclaimedWaterUse * 100));
            return TempInt;
        }
        ///------------------------------------------------------------------
        /// <summary>   Seti ReclaimedWaterUse.</summary>
        ///
        /// <param name="value"> The value (zero to 1.00). </param>
        ///------------------------------------------------------------------
        public void seti_ReclaimedWaterManagement(int value)
        {
            d_reclaimedWaterUse = (Double)value / 100;
        }
        // ============================================
        public int geti_TotalSupplies()
        {
            int result = 0;
            double temp = TheCRFNetwork.SurfaceLake.Limit;
            temp += TheCRFNetwork.SurfaceFresh.Limit;
            temp += TheCRFNetwork.SurfaceSaline.Limit;
            temp += TheCRFNetwork.Groundwater.Limit;
            temp += TheCRFNetwork.Effluent.Limit;
            try
            {
                int tempint = Convert.ToInt32(Math.Round(temp));
                result = tempint;
            }
            catch (Exception ex)
            {
                // ouch
            }
            return result;
        }

        // ========================================================================================
        #endregion Resources
        //--------------------------------------------------
        // Consumers
        // -------------------------------------------------
        # region Consumers
        /// <summary>
        /// Total Demand
        /// </summary>
        /// <returns></returns>
        double getd_TotalDemand()
        {
            double Temp = UnitNetwork.Urban.Demand + UnitNetwork.Industrial.Demand + UnitNetwork.Agriculture.Demand + UnitNetwork.Power.Demand;
            //double Temp = WSA.Urban.Demand + WSA.Industrial.Demand + WSA.Agriculture.Demand + WSA.Power.Demand;
            return Temp;
        }
        double getd_TotalNet()
        {
            double Temp = UnitNetwork.Urban.Net + UnitNetwork.Industrial.Net + UnitNetwork.Agriculture.Net + UnitNetwork.Power.Net;
            //double Temp = WSA.Urban.Net + WSA.Industrial.Net + WSA.Agriculture.Net + WSA.Power.Net;
            return Temp;
        }
        public int geti_NetDemandDifference()
        {
            //int TotalSupplies = geti_TotalSupplies();
            int tempInt = 0;
            double temp = 0;
            //
            double demand = getd_TotalDemand();
            double net = getd_TotalNet();
            if (0 < demand)
            {
                temp = Math.Min(100, Math.Max(0, (net / demand) * 100));
            }
            return tempInt = Convert.ToInt32(Math.Round(temp));

        }
        // =================================================================================================

        //----------------------------------------------
        // Urban
        //-----------------------------------------------

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti urban. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Urban()
        {
            int TempInt = Convert.ToInt32(Math.Round(UnitNetwork.Urban.Demand));
//            int TempInt = Convert.ToInt32(Math.Round(WSA.Urban.De//mand));
            return TempInt;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti urban. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_Urban(int value)
        {
            UnitNetwork.Urban.Demand = value;
            //WSA.Urban.Demand = value;
            i_demand_urban = value;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti urban net. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Urban_Net()
        {
            int TempInt = Convert.ToInt32(Math.Round(Math.Abs(UnitNetwork.Urban.Net)));
            //int TempInt = Convert.ToInt32(Math.Round(Math.Abs(WSA.Urban.Net)));
            return TempInt;

        }


        //----------------------------------------------
        // Agriculture
        //-----------------------------------------------

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti agriculture. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Agriculture()
        {
            int TempInt = Convert.ToInt32(Math.Round(UnitNetwork.Agriculture.Demand));
            //int TempInt = Convert.ToInt32(Math.Round(WSA.Agriculture.Demand));
            return TempInt;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti agriculture. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_Agriculture(int value)
        {
            UnitNetwork.Agriculture.Demand = value;
            //WSA.Agriculture.Demand = value;
            i_demand_ag = value;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti agriculture net. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Agriculture_Net()
        {
            int TempInt = Convert.ToInt32(Math.Abs(Math.Round(UnitNetwork.Agriculture.Net)));
            //int TempInt = Convert.ToInt32(Math.Abs(Math.Round(WSA.Agriculture.Net)));
            return TempInt;

        }
        // -------------------------------------------------------------------------------------------------



        // =================================================================================================

        //----------------------------------------------
        // Industrial
        //-----------------------------------------------

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti industrial. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Industrial()
        {
            int TempInt = Convert.ToInt32(Math.Round(UnitNetwork.Industrial.Demand));
            //int TempInt = Convert.ToInt32(Math.Round(WSA.Industrial.Demand));
            return TempInt;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti insustrial. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_Industrial(int value)
        {
            UnitNetwork.Industrial.Demand = value;
            //WSA.Industrial.Demand = value;
            i_demand_industry = value;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti industrial net. </summary>
        ///
        /// 
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_Industrial_Net()
        {
            int TempInt = Convert.ToInt32(Math.Round(Math.Abs(UnitNetwork.Industrial.Net)));
            //int TempInt = Convert.ToInt32(Math.Round(Math.Abs(WSA.Industrial.Net)));
            return TempInt;

        }

        //----------------------------------------------
        // Power
        //-----------------------------------------------

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the geti power. </summary>
        ///
        /// <returns>   . </returns>
        ///-------------------------------------------------------------------------------------------------

        public int geti_PowerWater()
        {
            int TempInt = Convert.ToInt32(Math.Round(UnitNetwork.Power.Demand));
            //int TempInt = Convert.ToInt32(Math.Round(WSA.Power.Demand));
            return TempInt;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti power. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public void seti_PowerWater(int value)
        {
            UnitNetwork.Power.Demand = value;
            //WSA.Power.Demand = value;
            i_demand_power = value;
        }

        public int geti_PowerWater_Net()
        {
            int TempInt = Convert.ToInt32(Math.Round(Math.Abs(UnitNetwork.Power.Net)));
//            int TempInt = Convert.ToInt32(Math.Round(Math.Abs(WSA.Power.Net)));
            return TempInt;

        }
        // ------------------------------------------------------------------
        /// <summary>
        /// The Power production
        /// </summary>
        /// <returns></returns>

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Seti power. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------
        //
        int _initialPower = 0;

        void InitialPower(string state)
        {

            //int temp = UnitNetwork.InitialPowerGenerated(state);
            int temp = UnitNetwork.InitialPowerGenerated;
            //int temp = WSA.InitialPowerGenerated(state);
            initialPower = temp;
        }
        internal int initialPower
        {
            get { return _initialPower; }
            set { _initialPower = value; }
        }
        int _powerEnergy = 0;
        public int geti_PowerEnergy()
        {
            int temp = _powerEnergy;
            return temp;
        }
        public void seti_PowerEnergy(int value)
        {
            _powerEnergy = value;
        }



        //----------------------------------------------------------------------------------------------------------------------------
        #endregion Consumers
        // -------------------------------------------------
        // Policies to Implement
        // -------------------------------------------------
        #region Policy Controls
        //
        int i_droughtYear = 2015;
        public int startDroughtYear
        {
            get
            {
                return i_droughtYear;
            }
            set
            {
                i_droughtYear = value;
            }
        }
        ///// <summary>

        // External Drivers 
        // =================================================================
        // Population Growth Rate 
        //-----------------------------------------------

        ///-----------------------------------------------------------------
        /// <summary>   Gets the geti population growth rate. </summary>
        ///
        /// <returns>an int from zero to 150 </returns>
        ///-----------------------------------------------------------------
        double d_popGrowthRateMod = 1.00;
        public int geti_PopGRateModifier()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_popGrowthRateMod * 100));
            return TempInt;
        }

        ///----------------------------------------------------------------
        /// <summary>   Seti population growth rate. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///----------------------------------------------------------------
        //public void seti_PopGrowthRate(int value)
        //{
        //    d_popGrowthRate = (Double)value / 100;
        //}
        public void seti_PopGRateModifier(int value)
        {
            d_popGrowthRateMod = ((Double)value / 100);
        }
        // ========================================
        public double AdjustPopulations
        {
            get { return d_popGrowthRateMod; }
        }

        //--------------------------------------------------------------------------------------------
        // =================================================================
        // Population Growth Rate 
        //-----------------------------------------------

        ///-----------------------------------------------------------------
        /// <summary>   Gets the geti population growth rate. </summary>
        ///
        /// <returns>an int from zero to 150 </returns>
        ///-----------------------------------------------------------------
        double d_popGrowthRate = 1.00;
        public int geti_PopGrowthRate()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_popGrowthRate * 100));
            return TempInt;
        }

        ///----------------------------------------------------------------
        /// <summary>   Seti population growth rate. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///----------------------------------------------------------------
        //public void seti_PopGrowthRate(int value)
        //{
        //    d_popGrowthRate = (Double)value / 100;
        //}
        public void seti_PopGrowthRate(int value)
        {
            d_popGrowthRate = ((Double)value / 100);
        }
        // ========================================
        public double AdjustPopulation
        {
            get { return d_popGrowthRate; }
        }
        // ========================================
        // Drought Impacts on Rivers/Lakes
        //--------------------------------

        ///--------------------------------------------------------------
        /// <summary>   Gets the geti DroughtImpacts. </summary>
        ///
        /// <returns>an int from zero to 150   . </returns>
        ///--------------------------------------------------------------
        //double d_drought = 1.00;
        double d_droughtManagement = 0.00;
        public int geti_DroughtImpacts()
        {
            int TempInt = Convert.ToInt32(d_droughtManagement);
            return TempInt;
        }
        ///------------------------------------------------------------------
        /// <summary>   Seti DroughtImpacts. </summary>
        ///
        /// <param name="value">    The value. </param>
        ///------------------------------------------------------------------
        public void seti_DroughtImpacts(int value)
        {
            d_droughtManagement = value;
        }
        // =======================================================================
        //
        // User Policies - 
        // =======================================================================
        // Urban Water Conservation
        //--------------------------------

        ///--------------------------------------------------------------
        /// <summary>   Gets the geti UrbanConservation. </summary>
        ///
        /// <returns>an int from zero to 100   . </returns>
        ///--------------------------------------------------------------
        double d_urbanConservation = 1.00;
        //double return_urban = 100;
        public int geti_UrbanConservation()
        {
            int temp = Convert.ToInt32(Math.Round(d_urbanConservation * 100));
            return temp;
        }
        ///------------------------------------------------------------------
        /// <summary>   Seti UrbanConservation.</summary>
        ///
        /// <param name="value">    The value. </param>
        ///------------------------------------------------------------------
        public void seti_UrbanConservation(int value)
        {
            d_urbanConservation = (Double)value / 100;
        }
        public double UrbanConservation
        {
            get { return d_urbanConservation; }
        }
        //
        // Desalinaiton
        double _desalinization = 1.0;
        public int geti_Desalinization()
        {
            int TempInt = Convert.ToInt32(Math.Round(_desalinization * 100));
            return TempInt;
        }
        public void seti_Desalinization(int value)
        {
            _desalinization = (Double)value / 100;
        }
        public double Desalinization
        {
            set { _desalinization = value; }
            get { return _desalinization; }
        }

        //
        //public double Desal
        //{
        //    get
        //    {

        //        double temp = 1;

        //        if (invokePolicies)
        //        {
        //            temp = 1 + _desalinization;
        //        }
        //        return temp;

        //    }
        //}



        // =====================================================================
        // ============================================
        // Agricultural Water Conservation
        //--------------------------------

        ///--------------------------------------------------------------
        /// <summary>   Gets the geti AgConservation.</summary>
        ///
        /// <returns>an int from zero to 100   . </returns>
        ///--------------------------------------------------------------
        double d_agConservation = 1.00;
        public int geti_AgConservation()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_agConservation * 100));
            return TempInt;
        }
        ///------------------------------------------------------------------
        /// <summary>   Seti UrbanConservation.</summary>
        ///
        /// <param name="value">    The value. </param>
        ///------------------------------------------------------------------
        public void seti_AgConservation(int value)
        {
            d_agConservation = (Double)value / 100;
        }
        public double AgConservation
        {
            get { return d_agConservation; }
        }
        // =====================================================================
        // ============================================
        // Power (utilities) Water Conservation
        //--------------------------------

        ///--------------------------------------------------------------
        /// <summary>   Gets the geti AgConservation.</summary>
        ///
        /// <returns>an int from zero to 100   . </returns>
        ///--------------------------------------------------------------
        double d_powerConservation = 1.00;
        public int geti_PowerConservation()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_powerConservation * 100));
            return TempInt;
        }
        ///------------------------------------------------------------------
        /// <summary>   Seti UrbanConservation.</summary>
        ///
        /// <param name="value">    The value (0 to 1.00). </param>
        ///------------------------------------------------------------------
        public void seti_PowerConservation(int value)
        {
            d_powerConservation = (Double)value / 100;
        }
        public double PowerConservation
        {
            get { return d_powerConservation; }
        }
        // =====================================================================
        // ============================================
        double d_industryConservation = 1.00;
        public int geti_IndustryConservation()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_industryConservation * 100));
            return TempInt;
        }
        double IndustryConservation
        {
            get { return d_industryConservation; }
        }
        ///------------------------------------------------------------------
        /// <summary>   Seti UrbanConservation.</summary>
        ///
        /// <param name="value">    The value (0 to 1.00). </param>
        ///------------------------------------------------------------------
        public void seti_IndustryConservation(int value)
        {
            d_industryConservation = (Double)value / 100;
        }

        // Groundwater Management 
        //--------------------------------
        /// <summary>
        /// Seti GroundwaterManagement
        /// </summary>
        /// <returns>an int from zero to 100   . </returns>
        //int d_groundwaterManagement = 1;
        //public int geti_GroundwaterManagement()
        //{
        //    int TempInt = d_groundwaterManagement;
        //    return TempInt;
        //}
        public int geti_GroundwaterControl()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_groundwaterControl*100));
            return TempInt;
        }

        ///------------------------------------------------------------------
        /// <summary>   Seti GroundwaterManagement.</summary>
        ///
        /// <param name="value"> The value (zero to 1.00). </param>
        ///------------------------------------------------------------------
        //public void seti_GroundwaterManagement(int value)
        //{

        //    //CheckBaseValueRange(eModelParam.epGroundwaterManagement, value);
        //    d_groundwaterManagement = value;
        //}
        double d_groundwaterControl = 1.0;
        public void seti_GroundwaterControl(int value)
        {
            d_groundwaterControl = ((double)value) / 100;
        }
        //
        // =====================================================================
        // ============================================
        // SurfaceWater Management 
        //--------------------------------
        /// <summary>
        /// Seti SurfaceWaterManagement
        /// </summary>
        /// <returns>an int from zero to 100   . </returns>
        //int i_surfaceWaterManagement = 1;
        //public int geti_SurfaceWaterManagement()
        //{
        //    int TempInt = i_surfaceWaterManagement;
        //    return TempInt;
        //}
        public int geti_SurfaceWaterControl()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_surfaceWaterControl* 100));
            return TempInt;
        }
        ///------------------------------------------------------------------
        /// <summary>   Seti SurfaceWaterManagement. ONLY called 
        /// durring initialization</summary>
        ///
        /// <param name="value"> The value (zero to 1.00). </param>
        ///------------------------------------------------------------------
        //public void seti_SurfaceWaterManagement(int value)
        //{
        //    i_surfaceWaterManagement = value;
        //}
        double d_surfaceWaterControl = 1.0;
        public void seti_SurfaceWaterControl(int value)
        {
            d_surfaceWaterControl = ((double)value) / 100;
        }
        // =====================================================================
        // ===================================
        // Lake Water Management
        double d_lakeWaterManagement = 0.00;
        public int geti_LakeWaterManagement()
        {
            int TempInt = Convert.ToInt32(Math.Round(d_lakeWaterManagement * 100));
            return TempInt;
        }
        // ------------------------------------------------
        public void seti_LakeWaterManagement(int value)
        {
            d_lakeWaterManagement = (double)value / 100;

        }
        //public double LWManagement
        //{
        //    get
        //    {
        //        double temp = 1;

        //        if (invokePolicies)
        //      //  if(invokeLakeWaterManagement)
        //        {
        //            //temp = 1 + d_lakeWaterManagement;
        //            temp = d_lakeWaterManagement;
        //        }
        //        return temp;
        //    }
        //}
        bool _invokePolicies = false;
        public bool invokePolicies
        {
            set { _invokePolicies = value; }
            get { return _invokePolicies; }
        }
        bool _invokeEffluent = false;
        public bool invokeEffluent
        {
            set { _invokeEffluent = value; }
            get { return _invokeEffluent; }
        }
        bool _invokeLakeWaterManagement;
        public bool invokeLakeWaterManagement
        {
            set { _invokeLakeWaterManagement = value; }
            get { return _invokeLakeWaterManagement; }
        }


        // =====================================================================
        //
        double d_drought = 1.0;
        public int geti_DroughtControl()
        {
            int TempInt = Convert.ToInt32(d_drought * 100);
            return TempInt;
        }
        public void seti_DroughtControl(int value)
        {
            d_drought = ((double)value) / 100;
        }
        // ================================================================================
        // Sustainability
        //
        // ================================================================================

        /// <summary>
        /// Retain a memory of what the flow modifyer was, so we can revert it back
        /// after the endYear is reached
        /// </summary>
        double _initialFlowSurface = 0;
        double initialFlowSurface
        {
            get { return _initialFlowSurface; }
            set { _initialFlowSurface = value; }
        }
        double _initialFlowModGW = 0;
        double initialFlowModGW
        {
            get { return _initialFlowModGW; }
            set { _initialFlowModGW = value; }
        }


        #endregion
        //    
        // FLUXES
        #region fluxes
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets flux allocated value. </summary>
        /// <param name="ResField">     The resource field. </param>
        /// <param name="ConsField">    The cons field. </param>
        /// <returns>   The flux allocated, 0 if not found. </returns>
        ///-------------------------------------------------------------------------------------------------
        public int GetFluxAllocated(string ResField, string ConsField)
        {
            int result = 0;
            CRF_Flux theFlux = TheCRFNetwork.FindFlux(ResField, ConsField);
            if (theFlux != null)
            {
                double value = theFlux.Allocated();
                try
                {
                    int tempint = Convert.ToInt32(value);
                    result = tempint;
                }
                catch (Exception ex)
                {
                    // ouch
                }
            }
            return result;
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Sets flux allocated value. </summary>
        /// <param name="ResField">     The resource field. </param>
        /// <param name="ConsField">    The cons field. </param>
        /// <param name="aValue">       The value. </param>
        ///-------------------------------------------------------------------------------------------------
        public void SetFluxAllocation(string ResField, string ConsField, double aValue)
        {
            CRF_Flux theFlux = TheCRFNetwork.FindFlux(ResField, ConsField);
            if (theFlux != null)
            {
                theFlux.SetAllocation(aValue);
            }
        }

        //======================================
        // Gets and Sets for SUR_UD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SUR_UD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SUR_UD()
        {
            int result = 0;
            result = GetFluxAllocated("SUR", "UTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SUR_UD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SUR_UD(int value)
        {
            SetFluxAllocation("SUR", "UTOT", value);
        }

        //======================================
        // Gets and Sets for SUR_AD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SUR_AD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SUR_AD()
        {
            int result = 0;
            result = GetFluxAllocated("SUR", "ATOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SUR_AD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SUR_AD(int value)
        {
            SetFluxAllocation("SUR", "ATOT", value);
        }

        //======================================
        // Gets and Sets for SUR_ID
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SUR_ID value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SUR_ID()
        {
            int result = 0;
            result = GetFluxAllocated("SUR", "ITOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SUR_ID value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SUR_ID(int value)
        {
            SetFluxAllocation("SUR", "ITOT", value);
        }

        //======================================
        // Gets and Sets for SUR_PD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SUR_PD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SUR_PD()
        {
            int result = 0;
            result = GetFluxAllocated("SUR", "PTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SUR_PD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SUR_PD(int value)
        {
            SetFluxAllocation("SUR", "PTOT", value);
        }

        //======================================
        // Gets and Sets for SURL_UD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SURL_UD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SURL_UD()
        {
            int result = 0;
            result = GetFluxAllocated("SURL", "UTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SURL_UD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SURL_UD(int value)
        {
            SetFluxAllocation("SURL", "UTOT", value);
        }

        //======================================
        // Gets and Sets for SURL_AD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SURL_AD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SURL_AD()
        {
            int result = 0;
            result = GetFluxAllocated("SURL", "ATOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SURL_AD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SURL_AD(int value)
        {
            SetFluxAllocation("SURL", "ATOT", value);
        }

        //======================================
        // Gets and Sets for SURL_ID
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SURL_ID value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SURL_ID()
        {
            int result = 0;
            result = GetFluxAllocated("SURL", "ITOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SURL_ID value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SURL_ID(int value)
        {
            SetFluxAllocation("SURL", "ITOT", value);
        }

        //======================================
        // Gets and Sets for SURL_PD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SURL_PD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SURL_PD()
        {
            int result = 0;
            result = GetFluxAllocated("SURL", "PTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SURL_PD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SURL_PD(int value)
        {
            SetFluxAllocation("SURL", "PTOT", value);
        }

        //======================================
        // Gets and Sets for GW_UD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get GW_UD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_GW_UD()
        {
            int result = 0;
            result = GetFluxAllocated("GW", "UTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set GW_UD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_GW_UD(int value)
        {
            SetFluxAllocation("GW", "UTOT", value);
        }

        //======================================
        // Gets and Sets for GW_AD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get GW_AD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_GW_AD()
        {
            int result = 0;
            result = GetFluxAllocated("GW", "ATOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set GW_AD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_GW_AD(int value)
        {
            SetFluxAllocation("GW", "ATOT", value);
        }

        //======================================
        // Gets and Sets for GW_ID
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get GW_ID value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_GW_ID()
        {
            int result = 0;
            result = GetFluxAllocated("GW", "ITOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set GW_ID value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_GW_ID(int value)
        {
            SetFluxAllocation("GW", "ITOT", value);
        }

        //======================================
        // Gets and Sets for GW_PD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get GW_PD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_GW_PD()
        {
            int result = 0;
            result = GetFluxAllocated("GW", "PTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set GW_PD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_GW_PD(int value)
        {
            SetFluxAllocation("GW", "PTOT", value);
        }

        //======================================
        // Gets and Sets for REC_UD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get REC_UD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_REC_UD()
        {
            int result = 0;
            result = GetFluxAllocated("REC", "UTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set REC_UD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_REC_UD(int value)
        {
            SetFluxAllocation("REC", "UTOT", value);
        }

        //======================================
        // Gets and Sets for REC_AD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get REC_AD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_REC_AD()
        {
            int result = 0;
            result = GetFluxAllocated("REC", "ATOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set REC_AD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_REC_AD(int value)
        {
            SetFluxAllocation("REC", "ATOT", value);
        }

        //======================================
        // Gets and Sets for REC_ID
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get REC_ID value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_REC_ID()
        {
            int result = 0;
            result = GetFluxAllocated("REC", "ITOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set REC_ID value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_REC_ID(int value)
        {
            SetFluxAllocation("REC", "ITOT", value);
        }

        //======================================
        // Gets and Sets for REC_PD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get REC_PD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_REC_PD()
        {
            int result = 0;
            result = GetFluxAllocated("REC", "PTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set REC_PD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_REC_PD(int value)
        {
            SetFluxAllocation("REC", "PTOT", value);
        }

        //======================================
        // Gets and Sets for SAL_UD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SAL_UD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SAL_UD()
        {
            int result = 0;
            result = GetFluxAllocated("SAL", "UTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SAL_UD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SAL_UD(int value)
        {
            SetFluxAllocation("SAL", "UTOT", value);
        }

        //======================================
        // Gets and Sets for SAL_AD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SAL_AD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SAL_AD()
        {
            int result = 0;
            result = GetFluxAllocated("SAL", "ATOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SAL_AD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SAL_AD(int value)
        {
            SetFluxAllocation("SAL", "ATOT", value);
        }

        //======================================
        // Gets and Sets for SAL_ID
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SAL_ID value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SAL_ID()
        {
            int result = 0;
            result = GetFluxAllocated("SAL", "ITOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SAL_ID value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SAL_ID(int value)
        {
            SetFluxAllocation("SAL", "ITOT", value);
        }

        //======================================
        // Gets and Sets for SAL_PD
        //======================================

        ///-----------------------------------------------------------
        ///  <summary>  get SAL_PD value </summary>
        /// <returns>  the value allocated for this flux. </returns>
        ///-----------------------------------------------------------
        public int geti_SAL_PD()
        {
            int result = 0;
            result = GetFluxAllocated("SAL", "PTOT");
            return result;
        }

        ///-----------------------------------------------------------
        ///  <summary>  set SAL_PD value </summary>
        /// <param name="value">    The value to Allocate to the Flux Parameter. </param>
        ///-----------------------------------------------------------
        public void seti_SAL_PD(int value)
        {
            SetFluxAllocation("SAL", "PTOT", value);
        }

        // Field used to test if parameters loaded
        private bool FFluxParametersReady = false;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the flux parameters ready. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------
        #endregion fluxes

    }
    #region Utilities
    static class utilities
    {
        public static double RoundToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }

        //If, as in your example, you really want to truncate, then you want:
        static double TruncateToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1 - digits);
            return scale * Math.Truncate(d / scale);
        }
        public static double DoubleBack(double In)
        {
            int Temp = 0;
            double temp = 0;
            Temp = Convert.ToInt32(In * 100);
            temp = (double)Temp / 100;

            return temp;
        }
        public static double hyperbola(double droughtFactor)
        {
            double b = 1.18;
            double a = -17;
            double temp = 1;
            const double minDF = 50;
            if (0 < droughtFactor)
            {
                if (50 <= droughtFactor)
                {
                    if (droughtFactor <= 100)
                        temp = droughtFactor / (a + b * droughtFactor);
                }
                else
                {
                    temp = droughtFactor / (a + b * minDF);
                }
            }
            return temp;
        }
    }

    #endregion
}
