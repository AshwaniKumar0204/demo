using System.Web;
using System.Web.Optimization;

namespace SchoolERP
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //Admin Bundle
            const string AngularAdminAppRoot = "~/Angular/Admin/";
            const string VirtualAdminBundlePath = AngularAdminAppRoot + "main2.js";

            var ScriptBundle = new ScriptBundle(VirtualAdminBundlePath)
                .Include(AngularAdminAppRoot + "AdminModule.js")
                .Include(AngularAdminAppRoot + "AdminRoute.js")
                .Include("~/Angular/Shared/AdminService.js")
                .Include("~/Angular/Shared/ConstantData.js")
                .Include("~/Angular/Shared/LoadDataService.js")
                .Include("~/Angular/Shared/Factory.js")
                .Include("~/Angular/Shared/sceDelegateProvider.js")
                .IncludeDirectory(
                AngularAdminAppRoot,
                searchPattern: "*.js",
                searchSubdirectories: true
                );
            bundles.Add(ScriptBundle);


            //SuperAdmin Bundle
            const string AngularSuperAdminAppRoot = "~/Angular/SuperAdmin/";
            const string VirtualSuperAdminBundlePath = AngularSuperAdminAppRoot + "main1.js";

            var SuperAdminScriptBundle = new ScriptBundle(VirtualSuperAdminBundlePath)
                .Include(AngularSuperAdminAppRoot + "SuperAdminModule.js")
                .Include(AngularSuperAdminAppRoot + "SuperAdminRoute.js")
                .Include("~/Angular/Shared/SuperAdminService.js")
                .Include("~/Angular/Shared/ConstantData.js")
                .Include("~/Angular/Shared/LoadDataService.js")
                .Include("~/Angular/Shared/Factory.js")
                .IncludeDirectory(
                AngularSuperAdminAppRoot,
                searchPattern: "*.js",
                searchSubdirectories: true
                );
            bundles.Add(SuperAdminScriptBundle);

          BundleTable.EnableOptimizations = true;
        }
    }
}
