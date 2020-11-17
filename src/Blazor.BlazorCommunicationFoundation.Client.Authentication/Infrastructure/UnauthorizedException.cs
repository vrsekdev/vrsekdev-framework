using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Infrastructure
{
    public class UnauthorizedException : Exception
    {
        private readonly NavigationManager navigationManager;
        private readonly string loginUrl;

        public UnauthorizedException(
            NavigationManager navigationManager,
            string loginUrl)
        {
            this.navigationManager = navigationManager;
            this.loginUrl = loginUrl;
        }

        public void RedirectToLogin(string returnUrl = null)
        {
            UriBuilder uriBuilder = new UriBuilder(navigationManager.ToAbsoluteUri(loginUrl));

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["returnUrl"] = returnUrl ?? navigationManager.Uri;
            uriBuilder.Query = query.ToString();

            navigationManager.NavigateTo(uriBuilder.ToString());
        }
    }
}
