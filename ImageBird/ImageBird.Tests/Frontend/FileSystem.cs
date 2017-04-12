using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SUT = ImageBird.Frontend.Shared;

namespace ImageBird.Tests.Frontend
{
    public class FileSystem
    {
        private static readonly string[] FileExtensions = new string[]
        {
            ".PNG",
            ".BMP",
            ".GIF",
            ".JPG",
            ".JPEG",
            ".WEBM",
            ".MKV",
            ".TXT",
            ".RTF",
            ".DOCX",
            ".XLSX",
            ".EXE",
            ".DLL"
        };

        [Fact]
        [Trait("Category", "Integration")]
        public void EnumerateDirectories_ValidDirectory_ShouldSucceed()
        {
            FileSystem.PerformTestOnFileRig((root, expectedDirectories, expectedFiles) =>
            {
                List<string> actualDirectories = SUT.FileSystem.EnumerateDirectories(root, true);

                Assert.True(FileSystem.EnumerableContentsEqual(expectedDirectories, actualDirectories));
            });
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void EnumerateFiles_ValidDirectory_ShouldSucceed()
        {
            FileSystem.PerformTestOnFileRig((root, expectedDirectories, expectedFiles) =>
            {
                List<string> actualFiles = SUT.FileSystem.EnumerateFiles(root, true);

                Assert.True(FileSystem.EnumerableContentsEqual(expectedFiles, actualFiles));
            });
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void EnumerateFiles_ValidDirectory_ExcludedFileTypes_ShouldNotContainExcludedTypes()
        {
            FileSystem.PerformTestOnFileRig((root, expectedDirectories, expectedFiles) =>
            {
                string excludedType = Path.GetExtension(expectedFiles.First());

                List<string> actual = SUT.FileSystem.EnumerateFiles(root, true, null, new string[] { excludedType });

                Assert.True(expectedFiles.Except(actual).All(x => Path.GetExtension(x) == excludedType));
                Assert.False(actual.Except(expectedFiles).Any());
            });
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void EnumerateFiles_ValidDirectory_ExcludedDirectories_ShouldNotContainExcludedDirectories()
        {
            FileSystem.PerformTestOnFileRig((root, expectedDirectories, expectedFiles) =>
            {
                string excludedDirectory = expectedDirectories.First();

                List<string> actual = SUT.FileSystem.EnumerateFiles(root, true, new string[] { excludedDirectory });

                Assert.True(expectedFiles.Except(actual).All(x => new FileInfo(x).DirectoryName == excludedDirectory));
                Assert.False(actual.Except(expectedFiles).Any());
            });
        }

        /// <summary>
        /// Cheap hack to determine if we have write access on a given folder under the current user context. Don't use this in production!.
        /// </summary>
        private static bool CheckWriteAccess(string folderPath)
        {
            try
            {
                Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static bool EnumerableContentsEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            return !expected.Except(actual).Any() && !actual.Except(expected).Any();
        }

        private static void PerformTestOnFileRig(Action<string, List<string>, List<string>> testAction)
        {
            Assert.True(CheckWriteAccess("."), "Need write permissions to perform FileSystem unit tests.");

            string root = Guid.NewGuid().ToString();
            Directory.CreateDirectory(root);

            List<string> oneToTen = Enumerable.Range(start: 1, count: 10).Select(x => x.ToString()).ToList();
            List<string> expectedDirectories = new List<string>();
            ConcurrentBag<string> expectedFiles = new ConcurrentBag<string>();
            foreach (string subDir in oneToTen.Select(x => Path.Combine(root, x)))
            {
                Directory.CreateDirectory(subDir);
                expectedDirectories.Add(Path.GetFullPath(subDir));

                Parallel.ForEach<string>(
                    oneToTen.Select(x => Path.Combine(subDir, x + FileSystem.RandomFileExtension())), 
                    file =>
                    {
                        using (StreamWriter writer = new StreamWriter(file))
                        {
                            writer.WriteAsync((string)null).Wait();
                        }

                        expectedFiles.Add(Path.GetFullPath(file));
                    });
            }

            try
            {
                testAction(root, expectedDirectories, expectedFiles.ToList());
            }
            finally
            {
                Directory.Delete(root, recursive: true);
            }
        }

        private static string RandomFileExtension()
        {
            return FileExtensions[StaticRandom.Rand(0, FileExtensions.Length)];
        }
        
        private static class StaticRandom
        {
            private static int seed = Environment.TickCount;

            private static readonly ThreadLocal<Random> Instance =
                new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

            public static int Rand(int inclusiveMin, int exclusiveMax)
            {
                return Instance.Value.Next(inclusiveMin, exclusiveMax);
            }
        }
    }
}
