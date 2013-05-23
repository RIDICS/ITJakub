using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace IT_Jakub.Views.Controls.FlyoutControls {
    /// <summary>
    /// Flayout control that shows on click at userlist in Crossword app
    /// </summary>
    public sealed partial class CrosswordUserListChangedFlyout : UserControl {
        /// <summary>
        /// The clicked user in userlist
        /// </summary>
        private Classes.Models.User u;
        /// <summary>
        /// The flyout itself
        /// </summary>
        private Callisto.Controls.Flyout flyOut;
        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();
        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordUserListChangedFlyout"/> class.
        /// </summary>
        public CrosswordUserListChangedFlyout() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordUserListChangedFlyout"/> class.
        /// </summary>
        /// <param name="u">The u.</param>
        /// <param name="flyOut">The fly out.</param>
        internal CrosswordUserListChangedFlyout(Classes.Models.User u, Callisto.Controls.Flyout flyOut)
            : this() {
            this.u = u;
            this.flyOut = flyOut;
        }

        /// <summary>
        /// Handles the Click event of the solution control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void solution_Click(object sender, RoutedEventArgs e) {
            CommandTable ct = new CommandTable();
            Command c = await ct.getUsersSolutionCommand(ss.getSessionData(), u);
            if (c != null) {
                string xml;
                Regex r = new Regex("^.*Solution" + Regex.Escape("(") + ".+" + Regex.Escape(", "));
                xml = r.Replace(c.CommandText, "");
                xml.TrimEnd();
                r = new Regex(Regex.Escape(")") + "$");
                xml = r.Replace(xml, "");
                if (u.Id != lu.getUserData().Id) {
                    Views.EducationalApplications.Crosswords.CrosswordsApp.stopSendingUpdate();
                }
                Views.EducationalApplications.Crosswords.CrosswordsApp.openCrossword(xml, u);
                if (u.Id == lu.getUserData().Id) {
                    Views.EducationalApplications.Crosswords.CrosswordsApp.startSendinAutoUpdate();
                }
            }
        }
    }
}
