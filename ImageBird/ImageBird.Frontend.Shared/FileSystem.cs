﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ImageBird.Frontend.Shared
{
    /// <summary>
    /// Contains methods pertaining to file discovery and manipulation.
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Wraps <see cref="Directory.EnumerateDirectories(string, string, SearchOption)"/> with logic described by accepted parameters.
        /// </summary>
        /// <param name="root">
        /// The root directory to enumerate from.
        /// </param>
        /// <param name="includeSubDirectories">
        /// True if subdirectories should be included in the enumeration.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}"/> of type <see cref="string"/> containing the full file path to all appropriate directories.
        /// </returns>
        public static List<string> EnumerateDirectories(string root, bool includeSubDirectories)
        {
            return 
                Directory.EnumerateDirectories(
                    root, 
                    "*",
                    includeSubDirectories
                        ? SearchOption.AllDirectories 
                        : SearchOption.TopDirectoryOnly)
                .Select(x => Path.GetFullPath(x))
                .ToList();
        }

        /// <summary>
        /// Gets the full path of all files which satisfy the constraints supplied as parameters.
        /// </summary>
        /// <param name="root">
        /// The root directory to enumerate from.
        /// </param>
        /// <param name="includeSubDirectories">
        /// True if subdirectories should be included in the enumeration.
        /// </param>
        /// <param name="excludedDirectories">
        /// A case-sensitive <see cref="IEnumerable{T}"/> of type <see cref="string"/> which contains any directories which should be excluded, or null if there are no excluded directories.
        /// </param>
        /// <param name="excludedFileTypes">
        /// A case-insensitive <see cref="IEnumerable{T}"/> of type <see cref="string"/> which contains any filetypes which should be excluded, or null if there are no excluded filetypes.
        /// </param>
        /// <returns>
        /// All files which specify the constraints supplied as parameters.
        /// </returns>
        public static List<string> EnumerateFiles(
            string root, 
            bool includeSubDirectories, 
            IEnumerable<string> excludedDirectories = null, 
            IEnumerable<string> excludedFileTypes = null)
        {
            IEnumerable<string> directories =
                FileSystem.EnumerateDirectories(root, includeSubDirectories);

            if (excludedDirectories != null)
            {
                directories = directories.Except(excludedDirectories);
            }

            List<string> files = new List<string>();
            foreach (string directory in directories)
            {
                files.AddRange(Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly));
            }

            if (excludedFileTypes != null)
            {
                List<string> actualExcluded = excludedFileTypes.Select(x => x.ToUpperInvariant()).ToList();
                files = files.Where(x => !actualExcluded.Contains(Path.GetExtension(x).ToUpperInvariant())).ToList();
            }

            return files.Select(x => Path.GetFullPath(x)).ToList();
        }

        /// <summary>
        /// Computes the MD5 hash of a file.
        /// </summary>
        /// <param name="file">
        /// The file to compute the MD5 hash of.
        /// </param>
        /// <returns>
        /// The computed MD5 hash of the file.
        /// </returns>
        public static string FileHash(string file)
        {
            MD5 md5 = MD5.Create();

            byte[] contentBytes = File.ReadAllBytes(file);
            md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);

            return BitConverter.ToString(md5.Hash).Replace("-", string.Empty);
        }

        /// <summary>
        /// Computes the MD5 hash of a directory <paramref name="path"/>'s contents.
        /// </summary>
        /// <param name="path">
        /// The directory to compute the hash of.
        /// </param>
        /// <returns>
        /// The MD5 hash of the specified <paramref name="path"/>.
        /// </returns>
        public static string FolderHash(string path)
        {
            List<string> files = FileSystem.EnumerateFiles(path, true);

            MD5 md5 = MD5.Create();

            for (int counter = 0; counter < files.Count; counter++)
            {
                string file = files[counter];
                
                byte[] pathBytes = Encoding.UTF8.GetBytes(file);
                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                
                byte[] contentBytes = File.ReadAllBytes(file);
                if (counter == files.Count - 1)
                {
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                }
                else
                {
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }
            }

            return BitConverter.ToString(md5.Hash).Replace("-", string.Empty);
        }
    }
}
