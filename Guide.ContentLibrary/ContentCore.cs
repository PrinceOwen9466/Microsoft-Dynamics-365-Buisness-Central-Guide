//using CefSharp;
//using CefSharp.SchemeHandler;
//using CefSharp.Wpf;
using CommonServiceLocator;
using Guide.Common.Infrastructure;
using Guide.Common.Infrastructure.Extensions;
using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Controls;
using Guide.Common.Infrastructure.Services.Interfaces;
using Guide.ContentLibrary.Views;
using Guide.ContentLibrary.Views.Company_Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Guide.ContentLibrary
{
    public static class ContentCore
    {
        #region Properties
        public static IPresentable Container { get; private set; }
        static bool Initialized { get; set; }

        #region Solids
        public static Assembly ContentAssembly { get => typeof(ContentCore).Assembly; }
        #endregion

        #endregion

        #region Methods

        public static void Initialize()
        {
            if (Initialized) return;
            Initialized = true;

            SettingUpANewCompany sd = new SettingUpANewCompany();

            foreach (var child in sd.FindAllRippleButtons())
                Core.Log.Debug(child);

            Presentation c = new Presentation();
            c.Initialize(typeof(Sections), typeof(SectionHome));
            Container = c;
        }

        /*
        static ContentContainer CompileContainer()
        {
            ContentContainer container = new ContentContainer();

            foreach (var section in typeof(Sections).GetFields(BindingFlags.Static | BindingFlags.Public))
                container.AddSection((Section)section.GetValue(null));

            container.Initialize(typeof(Views.SectionHome));
            //Container.CurrentPage = Container.Sections[0].
            //Skybound.Gecko.xm
            return container;
        }
        */
        #endregion
    }
}
