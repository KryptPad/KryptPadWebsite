
using KryptPadWebApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KryptPadWebApp.Models
{
    public class HomeIndexViewModel
    {

        #region Properties
        public string FileSize { get; set; }
        public int NumDownloads { get; set; }
        #endregion


        public HomeIndexViewModel()
        {
            // Map path to the download file
            var pathName = HttpContext.Current.Server.MapPath("~/files/KryptPad.application");

            //set the filesize
            if (System.IO.File.Exists(pathName))
            {
                var size = new System.IO.FileInfo(pathName).Length;

                // Format the filesize
                FileSize = FormatSize(size);
            }

            Load();
        }

        private void Load()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var appSetting = ctx.AppSettings.Where((a) => a.Id == 1).SingleOrDefault();
                // Do we have any settings? If not, create one
                if (appSetting != null)
                {
                    // Set the download count
                    NumDownloads = appSetting.Downloads;
                }
            }
        }

        /// <summary>
        /// Increments the download counter
        /// </summary>
        /// <returns></returns>
        public async Task IncrementDownloadCount()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var appSetting = ctx.AppSettings.Where((a) => a.Id == 1).SingleOrDefault();
                // Do we have any settings? If not, create one
                if (appSetting == null)
                {
                    appSetting = new AppSettings();
                    // Add to context
                    ctx.AppSettings.Add(appSetting);
                }

                // Increment the count
                appSetting.Downloads++;

                // Save changes in the database
                await ctx.SaveChangesAsync();
            }
        }

        #region Helper methods
        /// <summary>
        /// Formats a files size depending on how big it is
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        private string FormatSize(long size)
        {
            decimal fileSize = (decimal)size;

            //determine how to format it
            string suffix = "";
            if (fileSize < 1024) //bytes
            {
                suffix = "Bytes";
            }
            else if (fileSize < 1048576) //kilobytes
            {
                suffix = "Kb";
                fileSize = fileSize / 1024;
            }
            else if (fileSize >= 1048576 && fileSize < 1073741824) //megabytes
            {
                suffix = "Mb";
                fileSize = fileSize / 1048576;
            }
            else if (fileSize >= 1073741824) //gigabytes
            {
                suffix = "Gb";
                fileSize = fileSize / 1073741824;
            }

            string formatted = string.Format("{0} {1}", Math.Round(fileSize, 2), suffix);
            //return the format
            return formatted;
        }
        #endregion

    }
}