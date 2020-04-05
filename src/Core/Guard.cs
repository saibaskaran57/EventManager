using System;

namespace Core
{
    public static class Guard
    {
        public static void EnsureNotNull(object parameter, string name)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
}
