using System;

namespace com.ciswired.libraries.CISLogger
{
    interface ILogManager
    {
        ILogger GetLogger(Type type);

        ILogger GetLogger(string name);
    }
}
