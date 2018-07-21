using UnityEngine;
using Vigilant.Interface.Commands;
using Vigilant.Interface.Weapons;

namespace Vigilant.Commands
{ 
    /// <summary>
    /// This class is a base weapon command, will be used as bridge between Unit and Weapon.
    /// </summary>
    public sealed class WeaponCommand : MonoBehaviour,ICommand,IWeapon
    {
        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Unity Events
        #endregion

        #region Methods
        #endregion

        public void Execute()
        {
        }

        public void Upgrade()
        {
        }

        public void Cancel()
        {   
        }
    }
}