﻿
///==============================================================
///  CODE TO ADD FLUX PARAMETERS 
///  GENERATED BY  BUILDFLUXCODE APP Ver 1.0
///  Tuesday, March 15, 2016
///  DO NOT MODIFY THIS CODE!
///  It is machine generated and any changes made will not propogate to next version
///==============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ConsumerResourceModelFramework;
using WaterSimDCDC.Documentation;
//using WaterSimDCDC.America;

namespace WaterSimDCDC
{
    ///-----------------------------------------------------------
    ///  <summary>   eModelParm parameter codes. </summary>
    ///-----------------------------------------------------------
    public static partial class eModelParam
    {
        public const int ep_SUR_UD = 901;
        public const int ep_SUR_AD = 902;
        public const int ep_SUR_ID = 903;
        public const int ep_SUR_PD = 904;
        public const int ep_SURL_UD = 905;
        public const int ep_SURL_AD = 906;
        public const int ep_SURL_ID = 907;
        public const int ep_SURL_PD = 908;
        public const int ep_GW_UD = 909;
        public const int ep_GW_AD = 910;
        public const int ep_GW_ID = 911;
        public const int ep_GW_PD = 912;
        public const int ep_REC_UD = 913;
        public const int ep_REC_AD = 914;
        public const int ep_REC_ID = 915;
        public const int ep_REC_PD = 916;
        public const int ep_SAL_UD = 917;
        public const int ep_SAL_AD = 918;
        public const int ep_SAL_ID = 919;
        public const int ep_SAL_PD = 920;
    }

    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Manager for water simulations. </summary>
    ///-------------------------------------------------------------------------------------------------
    public partial class WaterSimManager : WaterSimManagerClass
    {
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

        public bool FluxParametersReady()
        {
            return FFluxParametersReady;
        }

        public bool initializeFluxParameters()
        {
            bool result = true;
            try
            {
                Extended_Parameter_Documentation ExtendDoc = ParamManager.Extended;
                //----  SUR_UD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SUR_UD, "SUR to UTOT Allocation", "SUR_UD", rangeChecktype.rctCheckRange, 0, 50000, geti_SUR_UD, seti_SUR_UD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SUR_UD, "SUR Water Supply allocated to UTOT water consumption", "MGD", "Million Gallons Per Day", "SUR to UTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SUR_AD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SUR_AD, "SUR to ATOT Allocation", "SUR_AD", rangeChecktype.rctCheckRange, 0, 50000, geti_SUR_AD, seti_SUR_AD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SUR_AD, "SUR Water Supply allocated to ATOT water consumption", "MGD", "Million Gallons Per Day", "SUR to ATOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SUR_ID Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SUR_ID, "SUR to ITOT Allocation", "SUR_ID", rangeChecktype.rctCheckRange, 0, 50000, geti_SUR_ID, seti_SUR_ID, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SUR_ID, "SUR Water Supply allocated to ITOT water consumption", "MGD", "Million Gallons Per Day", "SUR to ITOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SUR_PD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SUR_PD, "SUR to PTOT Allocation", "SUR_PD", rangeChecktype.rctCheckRange, 0, 50000, geti_SUR_PD, seti_SUR_PD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SUR_PD, "SUR Water Supply allocated to PTOT water consumption", "MGD", "Million Gallons Per Day", "SUR to PTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SURL_UD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SURL_UD, "SURL to UTOT Allocation", "SURL_UD", rangeChecktype.rctCheckRange, 0, 50000, geti_SURL_UD, seti_SURL_UD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SURL_UD, "SURL Water Supply allocated to UTOT water consumption", "MGD", "Million Gallons Per Day", "SURL to UTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SURL_AD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SURL_AD, "SURL to ATOT Allocation", "SURL_AD", rangeChecktype.rctCheckRange, 0, 50000, geti_SURL_AD, seti_SURL_AD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SURL_AD, "SURL Water Supply allocated to ATOT water consumption", "MGD", "Million Gallons Per Day", "SURL to ATOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SURL_ID Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SURL_ID, "SURL to ITOT Allocation", "SURL_ID", rangeChecktype.rctCheckRange, 0, 50000, geti_SURL_ID, seti_SURL_ID, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SURL_ID, "SURL Water Supply allocated to ITOT water consumption", "MGD", "Million Gallons Per Day", "SURL to ITOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SURL_PD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SURL_PD, "SURL to PTOT Allocation", "SURL_PD", rangeChecktype.rctCheckRange, 0, 50000, geti_SURL_PD, seti_SURL_PD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SURL_PD, "SURL Water Supply allocated to PTOT water consumption", "MGD", "Million Gallons Per Day", "SURL to PTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  GW_UD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_GW_UD, "GW to UTOT Allocation", "GW_UD", rangeChecktype.rctCheckRange, 0, 50000, geti_GW_UD, seti_GW_UD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_GW_UD, "GW Water Supply allocated to UTOT water consumption", "MGD", "Million Gallons Per Day", "GW to UTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  GW_AD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_GW_AD, "GW to ATOT Allocation", "GW_AD", rangeChecktype.rctCheckRange, 0, 50000, geti_GW_AD, seti_GW_AD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_GW_AD, "GW Water Supply allocated to ATOT water consumption", "MGD", "Million Gallons Per Day", "GW to ATOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  GW_ID Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_GW_ID, "GW to ITOT Allocation", "GW_ID", rangeChecktype.rctCheckRange, 0, 50000, geti_GW_ID, seti_GW_ID, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_GW_ID, "GW Water Supply allocated to ITOT water consumption", "MGD", "Million Gallons Per Day", "GW to ITOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  GW_PD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_GW_PD, "GW to PTOT Allocation", "GW_PD", rangeChecktype.rctCheckRange, 0, 50000, geti_GW_PD, seti_GW_PD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_GW_PD, "GW Water Supply allocated to PTOT water consumption", "MGD", "Million Gallons Per Day", "GW to PTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  REC_UD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_REC_UD, "REC to UTOT Allocation", "REC_UD", rangeChecktype.rctCheckRange, 0, 50000, geti_REC_UD, seti_REC_UD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_REC_UD, "REC Water Supply allocated to UTOT water consumption", "MGD", "Million Gallons Per Day", "REC to UTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  REC_AD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_REC_AD, "REC to ATOT Allocation", "REC_AD", rangeChecktype.rctCheckRange, 0, 50000, geti_REC_AD, seti_REC_AD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_REC_AD, "REC Water Supply allocated to ATOT water consumption", "MGD", "Million Gallons Per Day", "REC to ATOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  REC_ID Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_REC_ID, "REC to ITOT Allocation", "REC_ID", rangeChecktype.rctCheckRange, 0, 50000, geti_REC_ID, seti_REC_ID, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_REC_ID, "REC Water Supply allocated to ITOT water consumption", "MGD", "Million Gallons Per Day", "REC to ITOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  REC_PD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_REC_PD, "REC to PTOT Allocation", "REC_PD", rangeChecktype.rctCheckRange, 0, 50000, geti_REC_PD, seti_REC_PD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_REC_PD, "REC Water Supply allocated to PTOT water consumption", "MGD", "Million Gallons Per Day", "REC to PTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SAL_UD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SAL_UD, "SAL to UTOT Allocation", "SAL_UD", rangeChecktype.rctCheckRange, 0, 50000, geti_SAL_UD, seti_SAL_UD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SAL_UD, "SAL Water Supply allocated to UTOT water consumption", "MGD", "Million Gallons Per Day", "SAL to UTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SAL_AD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SAL_AD, "SAL to ATOT Allocation", "SAL_AD", rangeChecktype.rctCheckRange, 0, 50000, geti_SAL_AD, seti_SAL_AD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SAL_AD, "SAL Water Supply allocated to ATOT water consumption", "MGD", "Million Gallons Per Day", "SAL to ATOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SAL_ID Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SAL_ID, "SAL to ITOT Allocation", "SAL_ID", rangeChecktype.rctCheckRange, 0, 50000, geti_SAL_ID, seti_SAL_ID, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SAL_ID, "SAL Water Supply allocated to ITOT water consumption", "MGD", "Million Gallons Per Day", "SAL to ITOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));
                //----  SAL_PD Parameter  ----------------------------
                ParamManager.AddParameter(new ModelParameterClass(eModelParam.ep_SAL_PD, "SAL to PTOT Allocation", "SAL_PD", rangeChecktype.rctCheckRange, 0, 50000, geti_SAL_PD, seti_SAL_PD, RangeCheck.NoSpecialBase));
                ExtendDoc.Add(new WaterSimDescripItem(eModelParam.ep_SAL_PD, "SAL Water Supply allocated to PTOT water consumption", "MGD", "Million Gallons Per Day", "SAL to PTOT", new string[] { }, new int[] { }, new ModelParameterGroupClass[] { }));




                FFluxParametersReady = true;
            } // try
            catch (Exception ex)
            {
                // ouch
                result = false;
            }
            return result;
        } // InitiallizeFluxParameters
    } // WaterSim
} // NameSpace