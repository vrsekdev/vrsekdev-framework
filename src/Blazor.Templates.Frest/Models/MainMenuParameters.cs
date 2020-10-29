using System;
using System.Collections.Generic;
using System.Text;

namespace VrsekDev.Blazor.Templates.Frest.Models
{
    public class MainMenuParameters
    {
        public event EventHandler MouseEnterEvent;
        public event EventHandler MouseLeaveEvent;

        /// <summary>
        /// Menu is collapsed
        /// </summary>
        public bool IsCollapsed { get; set; }

        /// <summary>
        /// Mouse is over the menu
        /// </summary>
        public bool IsExpanded { get; set; }

        internal void HandleMainMenuOnMouseenter()
        {
            if (IsCollapsed && !IsExpanded)
            {
                IsExpanded = true;
                MouseEnterEvent?.Invoke(this, null);
            }
        }

        internal void HandleMainMenuOnMouseleave()
        {
            if (IsCollapsed && IsExpanded)
            {
                IsExpanded = false;
                MouseLeaveEvent?.Invoke(this, null);
            }
        }
    }
}
