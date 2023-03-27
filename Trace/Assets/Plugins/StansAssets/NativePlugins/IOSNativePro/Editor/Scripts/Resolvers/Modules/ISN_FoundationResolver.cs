using SA.iOS.XCode;

namespace SA.iOS 
{
    public class ISN_FoundationResolver : ISN_APIResolver
    {
        protected override ISN_XcodeRequirements GenerateRequirements() 
        {
            var requirements = new ISN_XcodeRequirements();

            var property = new ISD_BuildProperty("GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            requirements.AddBuildProperty(property);

            if (ISD_API.Capability.iCloud.Enabled && ISD_API.Capability.iCloud.keyValueStorage) {
                 requirements.Capabilities.Add("iCloud");
            }

            return requirements;
        }

        public override bool IsSettingsEnabled { get { return true; } set { } }
        protected override string LibFolder { get { return string.Empty; } }
        public override string DefineName { get { return string.Empty; } }
    }
}

