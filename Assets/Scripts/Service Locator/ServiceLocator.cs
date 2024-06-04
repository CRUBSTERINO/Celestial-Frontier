using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServiceLocator
{
    private static ServiceLocator _instance;
    private static bool _isInitialized;

    // Service Locator is a Singleton
    public static ServiceLocator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ServiceLocator();
            }

            return _instance;
        }
    }
    public static bool IsInitialized { get => _isInitialized; set => _isInitialized = value; }

    private ServiceLocator()
    {
        _services = new Dictionary<Type, IService>();
    }

    private Dictionary<Type, IService> _services;

    public void RegisterService(IService service)
    {
        _services.Add(service.GetType(), service);
    }

    public T GetService<T>() where T : IService
    {
        T serviceInstance = (T)_services[typeof(T)];

        if (serviceInstance == null)
        {
            Debug.LogWarning($"Service of type {typeof(T)} is not registered in Service Locator");
        }

        return serviceInstance;
    }

    public List<IService> GetAllServices()
    {
        return _services.Values.ToList();
    }
}
