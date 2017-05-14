using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Core2
{
    internal static class FastBitmapExtensions
    {
        public static void PerformLockingOperation(this FastBitmap bitmap, FastBitmap.LockingDataOperation operation)
        {
            FastBitmap.PerformLockingOperation(bitmap.Content, operation);
        }
    }
}
