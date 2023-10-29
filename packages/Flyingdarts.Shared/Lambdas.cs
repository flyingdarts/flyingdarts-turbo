using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Flyingdarts.Shared
{
    public static class Lambdas
    {
        public static class Functions
        {
            public static class Signalling
            {
                public static string CONNECT = "Flyingdarts.Backend.Signalling.OnConnect";
                public static string DEFAULT = "Flyingdarts.Backend.Signalling.OnDefault";
                public static string DISCONNECT = "Flyingdarts.Backend.Signalling.OnDisconnect";
            }
            public static class User    
            {
                public static class Profile
                {
                    public static string GET = "Flyingdarts.Backend.User.Profile.Get";
                    public static string CREATE = "Flyingdarts.Backend.User.Profile.Create";
                    public static string UPDATE = "Flyingdarts.Backend.User.Profile.Update";
                }

                public static class Games
                {
                    public static class X01
                    {
                        public static string JOIN = "Flyingdarts.Backend.Games.X01.Join";
                        public static string JOIN_QUEUE = "Flyingdarts.Backend.Games.X01.JoinQueue";
                        public static string SCORE = "Flyingdarts.Backend.Games.X01.Score";
                    }
                }
            }

            public static class Utilities
            {
                public static class Emails
                {
                    public static string VERIFY_USER_EMAIL = "Flyingdarts.Backend.User.Profile.VerifyEmail";
                    public static string VERIFY_USER_EMAIL_QUEUE = "Flyingdarts.Backend.User.Profile.VerifyEmail.Queue";
                    public static string VERIFY_USER_EMAIL_QUEUE_URL = "Flyingdarts.Backend.User.Profile.VerifyEmail.Queue.URL";
                    public static string VERIFY_USER_EMAIL_QUEUE_URL_PARAMETER = "Flyingdarts.Backend.User.Profile.VerifyEmail.Queue.URL.Parameter";
                    public static string VERIFY_USER_EMAIL_QUEUE_ARN = "Flyingdarts.Backend.User.Profile.VerifyEmail.Queue.ARN";
                    public static string VERIFY_USER_EMAIL_QUEUE_ARN_PARAMETER = "Flyingdarts.Backend.User.Profile.VerifyEmail.Queue.ARN.Parameter";
                }
            }
        }

        public static string[] GetAll()
        {
            var classType = typeof(Lambdas);
            var fields = classType.GetFields(BindingFlags.Public | BindingFlags.Static);
            return (from field in fields where field.FieldType == typeof(string) select (string)field.GetValue(null)!).ToArray();
        }
    }
}
