#if UNITY_ANDROID
using System.Xml;
using VoxelBusters.CoreLibrary;

namespace VoxelBusters.EssentialKit.Editor.Android
{
    public class AndroidManifestGenerator
    {
#region Static fields

        private static string s_androidLibraryRootPackageName = "com.voxelbusters.android.essentialkit";

#endregion

#region Public methods

        public static void GenerateManifest(EssentialKitSettings settings, bool addQueries = false, string savePath = null)
        {
            Manifest manifest = new Manifest();
            manifest.AddAttribute("xmlns:android", "http://schemas.android.com/apk/res/android");
            manifest.AddAttribute("xmlns:tools", "http://schemas.android.com/tools");
            manifest.AddAttribute("package", s_androidLibraryRootPackageName + "plugin");
            manifest.AddAttribute("android:versionCode", "1");
            manifest.AddAttribute("android:versionName", "1.0");

            if (addQueries) //Required from Android 11
            {
                AddQueries(manifest, settings);
            }

            /*SDK sdk = new SDK();
            sdk.AddAttribute("android:minSdkVersion", "14");
            sdk.AddAttribute("android:targetSdkVersion", "30");

            // Add sdk
            manifest.Add(sdk);*/

            Application application = new Application();

            AddActivities(application, settings);
            AddProviders(application, settings);
            AddServices(application, settings);
            AddReceivers(application, settings);
            AddMetaData(application, settings);

            manifest.Add(application);

            AddPermissions(manifest, settings, addQueries);
            AddFeatures(manifest, settings);


            XmlDocument xmlDocument = new XmlDocument();
            XmlNode xmlNode = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            // Append xml node
            xmlDocument.AppendChild(xmlNode);

            // Get xml hierarchy
            XmlElement element = manifest.GenerateXml(xmlDocument);
            xmlDocument.AppendChild(element);

            // Save to androidmanifest.xml
            xmlDocument.Save(savePath == null ? IOServices.CombinePath(EssentialKitPackageLayout.AndroidProjectPath, "AndroidManifest.xml") : savePath);
        }

#endregion

#region Private methods

        private static void AddQueries(Manifest manifest, EssentialKitSettings settings)
        {
            Queries queries = new Queries();

            if (settings.SharingServicesSettings.IsEnabled)
            {
                Intent intent = new Intent();
                Action action = new Action();
                action.AddAttribute("android:name", "android.intent.action.SEND");
                intent.Add(action);
                queries.Add(intent);

                intent = new Intent();
                action = new Action();
                action.AddAttribute("android:name", "android.intent.action.SENDTO");
                intent.Add(action);
                queries.Add(intent);

                intent = new Intent();
                action = new Action();
                action.AddAttribute("android:name", "android.intent.action.SEND_MULTIPLE");
                intent.Add(action);
                queries.Add(intent);
            }

            

            if (settings.SharingServicesSettings.IsEnabled)
            {
                Package package = new Package();
                package.AddAttribute("android:name", "com.facebook.katana");
                queries.Add(package);

                package = new Package();
                package.AddAttribute("android:name", "com.twitter.android");
                queries.Add(package);

                package = new Package();
                package.AddAttribute("android:name", "com.instagram.android");
                queries.Add(package);

                package = new Package();
                package.AddAttribute("android:name", "com.whatsapp");
                queries.Add(package);

                Intent intent = new Intent();
                Action action = new Action();
                action.AddAttribute("android:name", "android.intent.action.VIEW");
                intent.Add(action);
                queries.Add(intent);
            }

            manifest.Add(queries);
        }

        private static void AddActivities(Application application, EssentialKitSettings settings)
        {
        }

        private static void AddProviders(Application application, EssentialKitSettings settings)
        {
            Provider provider = null;

            provider = new Provider();
            provider.AddAttribute("android:name", "com.voxelbusters.android.essentialkit.common.FileProviderExtended");
            provider.AddAttribute("android:authorities", string.Format("{0}.essentialkit.fileprovider", UnityEngine.Application.identifier));
            provider.AddAttribute("android:exported", "false");
            provider.AddAttribute("android:grantUriPermissions", "true");

            MetaData metaData = new MetaData();
            metaData.AddAttribute("android:name", "android.support.FILE_PROVIDER_PATHS");
            metaData.AddAttribute("android:resource", "@xml/essential_kit_file_paths");

            provider.Add(metaData);
            application.Add(provider);
        }

        private static void AddServices(Application application, EssentialKitSettings settings)
        {
            
        }

        private static void AddReceivers(Application application, EssentialKitSettings settings)
        {
        }

        private static void AddMetaData(Application application, EssentialKitSettings settings)
        {
        }

        private static void AddFeatures(Manifest manifest, EssentialKitSettings settings)
        {
        }

        private static void AddPermissions(Manifest manifest, EssentialKitSettings settings, bool addSupportForApi30 = false)
        {
            Permission permission;

            if (settings.AddressBookSettings.IsEnabled)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.READ_CONTACTS");
                manifest.Add(permission);
            }

            if (settings.NetworkServicesSettings.IsEnabled)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.ACCESS_NETWORK_STATE");
                manifest.Add(permission);

                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.INTERNET");
                manifest.Add(permission);
            }

            /*if(addSupportForApi30)
            {
                permission = new Permission();
                permission.AddAttribute("android:name", "android.permission.QUERY_ALL_PACKAGES");
                manifest.Add(permission);
            }*/
        }

        private static IntentFilter CreateIntentFilterForDeepLink(bool isUniversalLinkFilter, string label, string scheme, string host, string pathPrefix = null)
        {
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAttribute("android:label", label);

            if(isUniversalLinkFilter)
                intentFilter.AddAttribute("android:autoVerify", "true");

            Action action = new Action();
            action.AddAttribute("android:name", "android.intent.action.VIEW");
            intentFilter.Add(action);

            Category category = new Category();
            category.AddAttribute("android:name", "android.intent.category.DEFAULT");
            intentFilter.Add(category);

            category = new Category();
            category.AddAttribute("android:name", "android.intent.category.BROWSABLE");
            intentFilter.Add(category);

            Data data = new Data();
            data.AddAttribute("android:scheme", scheme);
            data.AddAttribute("android:host", host);
            if (pathPrefix != null)
            {
                data.AddAttribute("android:pathPrefix", pathPrefix.StartsWith("/") ? pathPrefix :  "/" + pathPrefix);
            }

            intentFilter.Add(data);

            return intentFilter;
        }

#endregion
    }
}
#endif