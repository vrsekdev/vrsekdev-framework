using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public void RedirectToLogin()
        {
            navigationManager.NavigateTo(loginUrl);
        }
    }
}
