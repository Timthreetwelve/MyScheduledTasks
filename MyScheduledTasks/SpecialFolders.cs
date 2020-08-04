// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

// Other folders are available. See documentation for Environment.SpecialFolder Enum

using System;

namespace TKUtils
{
    /// <summary>
    /// Class with methods for getting "Special" folder names
    /// </summary>
    public static class SpecialFolders
    {
        /// <summary>
        /// Returns the user's logical desktop
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        /// <summary>
        /// Returns the user's desktop folder
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        /// <summary>
        /// Returns the user's %localappdata% folder
        /// </summary>
        /// <returns></returns>
        public static string GetLocalAppdataFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        /// <summary>
        /// returns the user's %appdata% folder
        /// </summary>
        /// <returns></returns>
        public static string GetAppdataFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        /// Returns the user's Startup folder
        /// </summary>
        /// <returns></returns>
        public static string GetStartupFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        }

        /// <summary>
        /// Returns the user's Documents folder
        /// </summary>
        /// <returns></returns>
        public static string GetDocumentsFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// Returns the user's Pictures folder
        /// </summary>
        /// <returns></returns>
        public static string GetPicturesFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        }

        /// <summary>
        /// Returns the user's Music folder
        /// </summary>
        /// <returns></returns>
        public static string GetMusicFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        /// <summary>
        /// Returns the user's Videos folder
        /// </summary>
        /// <returns></returns>
        public static string GetVideoFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        }

        /// <summary>
        /// Returns the user's Program Files (x86) folder
        /// </summary>
        /// <returns></returns>
        public static string GetProgramFilesX86Folder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        }

        /// <summary>
        /// Returns the user's Program Files folder
        /// </summary>
        /// <returns></returns>
        public static string GetProgramFilesFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        }

        /// <summary>
        /// Returns the user's Programs folder  (..\Start Menu\Programs)
        /// This probably isn't the one your looking for.
        /// </summary>
        /// <returns></returns>
        public static string GetProgramsFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        }

        /// <summary>
        /// Returns the user's Start Menu folder
        /// </summary>
        /// <returns></returns>
        public static string GetStartMenuFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
        }

        /// <summary>
        /// Returns the user's UserProfile folder. Don't create files or folders in the UserProfile folder.
        /// </summary>
        /// <returns></returns>
        public static string GetUserProfileFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
}
