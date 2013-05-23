using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Utils {
    /// <summary>
    /// Class which properly ends running tasks.
    /// </summary>
    class TaskKiller {

        /// <summary>
        /// Ends the educational application tasks.
        /// </summary>
        public static void killEducationalApplicationTasks() {
            IT_Jakub.Views.EducationalApplications.SynchronizedReading.SyncReadingApp.killAutoUpdateTask();
            IT_Jakub.Views.EducationalApplications.Crosswords.CrosswordsApp.killAutoUpdateTask();
        }

    }
}
