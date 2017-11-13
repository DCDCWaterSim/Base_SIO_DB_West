using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaterSimDCDC;


namespace WaterSim_Base
{
    public class WaterSim_ParameterProcessFromIO
    {
        protected string Fname = "";
        internal const string ShortDescription = "Base: Does nothing!";
        internal const string LongDescription = "Base: Does nothing for ProcessInitialized(), ProcessedStarted(), PreProcess() or PostProcess()";
        internal const string CodeDescription = "BASE";
        protected string FProcessDescription = "";
        /// <summary> More Information describing the process. </summary>
        protected string FProcessLongDescription = "";
        /// <summary> The process code. </summary>
        protected string FProcessCode = "";

        protected WaterSimManager FWsim;

        public WaterSim_ParameterProcessFromIO(string aName)
        {
            Fname = aName;

        }
        public WaterSim_ParameterProcessFromIO(string aName, WaterSimManager WSim)
        {
            BuildDescStrings();
            Fname = aName;
            this.Name = this.GetType().Name;
            FWsim = WSim;
        }
        protected virtual void BuildDescStrings()
        {
            FProcessDescription = ShortDescription;
            FProcessLongDescription = LongDescription;
            FProcessCode = CodeDescription;
        }
        public string Name
        {
            get { return Fname; }
            set { Fname = value; }
        }
        public WaterSimManager WSM
        {
            get{return FWsim;}
        }
    }
}
namespace WaterSimDCDC.Processes
{
    public class ModifyPoliciesFeedbackProcess : WaterSimDCDC.AnnualFeedbackProcess
    {
        string Name = "";
        public ModifyPoliciesFeedbackProcess(string aName)
             : base (aName)
        {
            Name = aName;

        }
        public ModifyPoliciesFeedbackProcess(string aName, WaterSimManager WSim, bool Quiet)
        {
            if (Quiet)
            {
                bool truth = true;
                // WaterSim_Base.WaterSim_ParameterProcessFromIO WSP = new WaterSim_Base.WaterSim_ParameterProcessFromIO(aName, WSim);
            //    if (WaterSimDCDC. TreeViewInputs.TreeNodes == true) { }
            }
            else
            {
            }

        }

    }
}
