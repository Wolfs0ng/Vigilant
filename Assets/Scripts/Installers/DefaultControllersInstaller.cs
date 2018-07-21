using System;
using UnityEngine;
using Vigilant.Controllers.Camera;
using Vigilant.Interface.Managers;
using Vigilant.Managers.InputManagers;
using Zenject;

public class DefaultControllersInstaller : MonoInstaller<DefaultControllersInstaller>
{
    [SerializeField] private GameObject inputManagerGameObject; 
    
    public override void InstallBindings()
    {
        if (inputManagerGameObject == null)
        {
            throw new NullReferenceException("Input manager gameobject is null in installer");
        }

        Container.Bind<IInputManager>().FromComponentInNewPrefab(inputManagerGameObject)
            .WithGameObjectName(inputManagerGameObject.name).UnderTransform(transform.parent).AsSingle();

        Container.Bind<CameraController>().FromComponentInHierarchy(null,true).AsSingle();
    }
}