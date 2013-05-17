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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IT_Jakub.Views.Controls.FlyoutControls {
    public sealed partial class CrosswordUserListChangedFlyout : UserControl {
        private Classes.Models.User u;
        private Callisto.Controls.Flyout flyOut;
        private static SignedSession ss = SignedSession.getInstance();
        private static LoggedUser lu = LoggedUser.getInstance();

        public CrosswordUserListChangedFlyout() {
            this.InitializeComponent();
        }

        internal CrosswordUserListChangedFlyout(Classes.Models.User u, Callisto.Controls.Flyout flyOut)
            : this() {
            this.u = u;
            this.flyOut = flyOut;
        }

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
