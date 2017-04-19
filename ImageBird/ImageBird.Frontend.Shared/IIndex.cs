using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    public interface IIndex
    {
        IReadOnlyCollection<string> Directories { get; }

        IReadOnlyCollection<string> Files { get; }

        IReadOnlyCollection<string> Categories { get; }

        void AddDirectory(string path);

        IReadOnlyCollection<IndexObject> AddFile(string path);

        void AddCategory(string category);

        void RemoveDirectory(string path);

        void RemoveFile(string path);

        void RemoveCategory(string category);

        IndexObject GetByPath(string path);

        IndexObject GetByPerceptualHash(string hash);

        IReadOnlyCollection<IndexObject> GetByCategory(string category);

        void Load(string path);

        void Save(string path);
    }
}
