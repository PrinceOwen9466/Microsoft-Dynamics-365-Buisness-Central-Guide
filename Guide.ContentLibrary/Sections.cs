using Guide.Common.Infrastructure.Models;
using Guide.ContentLibrary.Properties;
using Guide.ContentLibrary.Views.Introduction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.ContentLibrary
{
    public static class Sections
    {
        #region Properties

        #region Introduction
        public static Section Introduction = new Section();

        public static Section GettingStarted = new Section();

        public static Section BusinessCentralTour = new Section();

        public static Section CompanySetup = new Section();

        /*
        public static Section Finance = new Section();

        public static Section Banking = new Section();

        public static Section Sales = new Section();

        public static Section Purchasing = new Section();

        public static Section Inventory = new Section();
        */
        #endregion

        #endregion

        #region Constructors
        static Sections()
        {                                   
            Introduction.Index = 1;
            Introduction.Name = "Introduction";
            Introduction.Description = "An overview of Microsoft Dynamics 365 Business Central and it's benefits to your company";
            Introduction.PageMap = new List<Reference>()
            {
                new Reference(Introduction, typeof(WelcomeToBusinessCentral), "Welcome To Business Central"),
                new Reference(Introduction, typeof(WhyBusinessCentral), "Why Business Central"),
                new Reference(Introduction, typeof(BusinessCentralOverview), "Business Central Overview"),
            };

            GettingStarted.Index = 2;
            GettingStarted.Name = "Getting Started";
            GettingStarted.Description = "Steps on how to register for business central";
            GettingStarted.PageMap = new List<Reference>()
            {
                new Reference(GettingStarted, typeof(Views.Getting_Started.Registeration), "Registeration"),
                new Reference(GettingStarted, typeof(Views.Getting_Started.Accessing_Business_Central), "Accessing Business Central")
            };

            BusinessCentralTour.Index = 3;
            BusinessCentralTour.Name = "Business Central Tour";
            BusinessCentralTour.Description = "An Interactive Tour around Microsoft Business Central.";
            BusinessCentralTour.PageMap = new List<Reference>()
            {
                new Reference(BusinessCentralTour, typeof(Views.Business_Central_Overview.Tour_Introduction), "Tour Introduction"),
                new Reference(BusinessCentralTour, typeof(Views.Business_Central_Overview.HomePage), "Home Page"),
                new Reference(BusinessCentralTour, typeof(Views.Business_Central_Overview.TipsAndTricks), "Key Symbols")
            };

            CompanySetup.Index = 4;
            CompanySetup.Name = "Company Setup";
            CompanySetup.Description = "Steps and directions to setup your company in business central.";
            CompanySetup.PageMap = new List<Reference>()
            {
                new Reference(CompanySetup, typeof(Views.Company_Setup.SettingUpANewCompany), "Setting Up A Company"),
                new Reference(CompanySetup, typeof(Views.Company_Setup.SettingUpFinance), "Setting Up Finance"),
                new Reference(CompanySetup, typeof(Views.Company_Setup.SettingUpInventory), "Setting Up Inventory"),
                new Reference(CompanySetup, typeof(Views.Company_Setup.SettingUpSales), "Setting Up Sales"),
                new Reference(CompanySetup, typeof(Views.Company_Setup.SettingUpPurchasing), "Setting Up Purchasing")
            };

            /*
            Finance.Index = 5;
            Finance.Name = "Finance";
            Finance.Description = "Managing Money in business central";
            Finance.PageMap = new List<Reference>();

            Banking.Index = 6;
            Banking.Name = "Banking";
            Banking.Description = "Managing Banking with business central";
            Banking.PageMap = new List<Reference>();

            Sales.Index = 7;
            Sales.Name = "Sales";
            Sales.Description = "Managing Sales with Business Central";
            Sales.PageMap = new List<Reference>();

            Purchasing.Index = 8;
            Purchasing.Name = "Purchasing";
            Purchasing.Description = "Managing Purchases and invoicing with Business Central";
            Purchasing.PageMap = new List<Reference>();

            Inventory.Index = 9;
            Inventory.Name = "Inventory";
            Inventory.Description = "Managing Stock and Inventory with Business Central";
            Inventory.PageMap = new List<Reference>();
            */
        }
        #endregion
    }
}
