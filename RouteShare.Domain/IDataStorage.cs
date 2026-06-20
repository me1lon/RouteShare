using System;

namespace RouteShare.Domain
{
    public interface IDataStorage
    {
        void SaveToJson(string filePath);
        void LoadFromJson(string filePath);
    }
}