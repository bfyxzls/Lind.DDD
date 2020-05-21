using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
namespace Lind.DDD.HttpExtensions.HttpModules
{
    /// <summary>
    /// session共享sessionId
    /// </summary>
    public class SessionProviderHttpModule : IHttpModule
    {
        private string m_RootDomain = string.Empty;

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            m_RootDomain = System.Configuration.ConfigurationManager.AppSettings["SessionDomain"] ?? "mvvm.com";

            Type stateServerSessionProvider = typeof(HttpSessionState).Assembly.GetType("System.Web.SessionState.OutOfProcSessionStateStore");
            FieldInfo uriField = stateServerSessionProvider.GetField("s_uribase", BindingFlags.Static | BindingFlags.NonPublic);

            if (uriField == null)
                throw new ArgumentException("UriField was not found");

            uriField.SetValue(null, m_RootDomain);

            context.EndRequest += new System.EventHandler(context_EndRequest);
        }

        void context_EndRequest(object sender, System.EventArgs e)
        {

            HttpApplication app = sender as HttpApplication;
            for (int i = 0; i < app.Context.Response.Cookies.Count; i++)
            {
                if (app.Context.Response.Cookies[i].Name == "ASP.NET_SessionId")
                {
                    app.Context.Response.Cookies[i].Domain = m_RootDomain;
                }
            }
        }
    }
}
