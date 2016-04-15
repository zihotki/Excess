﻿using System.Web.Optimization;

namespace Excess.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            //VENDOR RESOURCES

            //~/Bundles/App/vendor/css
            bundles.Add(
                new StyleBundle("~/Bundles/App/vendor/css")
                    .Include("~/Content/font-awesome.min.css", new CssRewriteUrlTransform())
                    .Include(
                        "~/Content/themes/base/all.css",
                        "~/Content/bootstrap.min.css",
                        "~/Content/toastr.min.css",
                        "~/Content/flags/famfamfam-flags.css",
                        "~/Content/jquery.mmenu.all.css",
                        "~/Content/jquery.mmenu.css",
                        "~/Content/addons/jquery.mmenu.dragopen.css",
                        "~/Content/extensions/jquery.mmenu.themes.css",
                        "~/Content/jquery.ui.layout.css",
                        "~/Scripts/codemirror-4.8/lib/codemirror.css",
                        "~/Scripts/codemirror-4.8/theme/eclipse.css",
                        "~/Scripts/codemirror-4.8/theme/neat.css"
                    )
                );

            //~/Bundles/App/vendor/js
            bundles.Add(
                new ScriptBundle("~/Bundles/App/vendor/js")
                    .Include(
                        "~/Scripts/json2.min.js",
                        "~/Scripts/modernizr-2.8.3.js",
                        "~/Scripts/jquery-2.1.1.min.js",
                        "~/Scripts/jquery-ui-1.11.2.min.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/Scripts/moment-with-locales.min.js",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/others/spinjs/spin.js",
                        "~/Scripts/others/spinjs/jquery.spin.js",
                        "~/Scripts/angular.min.js",
                        "~/Scripts/angular-animate.min.js",
                        "~/Scripts/angular-sanitize.min.js",
                        "~/Scripts/angular-ui-router.min.js",
                        "~/Scripts/angular-ui/ui-bootstrap.min.js",
                        "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                        "~/Scripts/angular-ui/ui-utils.min.js",
                        "~/Scripts/jquery.mmenu.min.all.js",
                        "~/Scripts/addons/jquery.mmenu.dragopen.min.js",
                        "~/Scripts/addons/jquery.mmenu.fixedelements.min.js",
                        "~/Scripts/jquery.layout.js",
                        "~/Scripts/dialogs.min.js",
                        "~/Scripts/dialogs-default-translations.min.js",
                        "~/Scripts/codemirror-4.8/lib/codemirror.js",
                        "~/Scripts/codemirror-4.8/mode/clike/clike.js",
                        "~/Scripts/jquery.bootstrap.wizard.js",
                        "~/Scripts/rcWizard.js",
                        "~/Scripts/hotkeys.js",
                        "~/Scripts/jquery.signalR-2.2.0.min.js",
                        "~/signalr/hubs"
                    )
                );

            //APPLICATION RESOURCES

            //~/Bundles/App/Main/css
            bundles.Add(
                new StyleBundle("~/Bundles/App/Main/css")
                    .IncludeDirectory("~/App/Main", "*.css", true)
                );

            //~/Bundles/App/Main/js
            bundles.Add(
                new ScriptBundle("~/Bundles/App/Main/js")
                    .IncludeDirectory("~/App/Main", "*.js", true)
                );
        }
    }
}