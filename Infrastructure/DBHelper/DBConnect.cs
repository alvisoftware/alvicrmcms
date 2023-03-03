using MongoDB.Driver;
namespace Infrastructure.DBHelper
{
    public class DBConnect
    {
        private static IMongoDatabase _db;
        private static String _sConnectionString = "mongodb+srv://saurabh:test123@cluster0.ra8snbg.mongodb.net/?retryWrites=true&w=majority/testcrm/AlviUsers";
        public IMongoDatabase db
        {
            get
            {
                return _db
                    ??
                  (_db = ConnectDatabase());
            }
            set
            {
                if (value == null)
                {
                    value = ConnectDatabase();
                }
                _db = value;
            }
        }

        public DBConnect()
        {
            GetConfiguration();
            this.db = ConnectDatabase();
        }
        internal protected void GetConfiguration()
        {
            _sConnectionString = "mongodb+srv://saurabh:test123@cluster0.ra8snbg.mongodb.net/?retryWrites=true&w=majority/testcrm/AlviUsers";
        }
        private static IMongoDatabase ConnectDatabase()
        {
            var _Settings = MongoClientSettings.FromConnectionString(_sConnectionString);
            var _Client = new MongoClient(_Settings);
            return _Client.GetDatabase("testcrm");
        }
    }


    public class DBTableNames
    {
        public const string MenuDB = "Menus";
        public const string AlviUsersDB = "AlviUsers";
        public const string SentMails = "CRMSentMails";
        public const string AlviContacts = "AlviWebContact";
        public const string TrackMails = "CRMTrackMail";
        public const string TestimonialDB = "Testimonials";
        public const string CommonDataDB = "CommonData";
        public const string CustomeDataDB = "CustomersData";
        public const string CMSPagesDB = "CMSPages";
        public const string PortfolioDB = "Portfolio";
        public const string PageDB = "Pages";
        //
        public const string CompanyDetails = "CompDet";
        public const string InvoiceDetails = "InvDet";
        public const string CompanyInfo = "CompInfo";
        public const string EmployeeDetails = "EmployeeMod";
        public const string WorkingDaysDetails = "WorkDetails";
        public const string HolidayDetails = "HoliDetails";
        public const string EmployeeLeave = "EmpLeave";
        public const string PaymentDetails = "PayDetails";
        public const string ExpenseTable = "ExpTable";
        public const string DashBoard = "DashBoard";
    }

    public class Constantdata
    {
        public const string PortfolioDirectoryName = "portfolio";
        public const string PageBackgroundImageDirectoryName = "background";
        public const string ProjectDirectoryName = "project";
    }
    public static class Extensions
    {
        public static string ToLowerStringWithoutSpace(this string value)
        {
            if (!string.IsNullOrEmpty(value))
                return value.Replace(" ", "_").ToLower();
            return string.Empty;
        }
    }
}
