﻿namespace Web.Services.Interfaces
{

    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expiration = null);
        void Remove(string key);
        void Clear();
        bool Exists(string key);
    }
}
