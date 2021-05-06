using System;
using System.Collections.Generic;
using System.Text;

namespace MyDB.Infrastructure.Tools.Interfaces
{
    public interface IUtilityService
    {
        bool checkPrimitive(Type type);
        string toJson<T>(T value);
        T fromJson<T>(string value);
    }
}
