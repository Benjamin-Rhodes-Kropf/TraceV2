using UnityEngine;
using SA.iOS.XCode;

namespace SA.iOS
{
    internal class ISN_AdSupportResolver : ISN_APIResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() 
        {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.AdSupport));
            return requirements;
        }


        protected override string LibFolder 
        {
            get 
            { 
                return "AdSupport/"; 
            } 
        }

        public override bool IsSettingsEnabled 
        {
            get 
            { 
                return ISN_Settings.Instance.AdSupport; 
            }
            set 
            { 
                ISN_Settings.Instance.AdSupport = value; 
            }

        }
        public override string DefineName 
        { 
            get 
            { 
                return "AS_SUPPORT_API_ENABLED"; 
            } 
        }
    }
}

