using UnityEngine;
using Vigilant.Enums;
using Vigilant.Interface.Managers;
using Vigilant.Managers.InputManagers;

namespace Vigilant.Controllers.Player
{ 
    /// <summary>
    /// Main player controller, contains all weapons,engine and send actions to them.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        //Full of shit
        
        #region Fields

        [SerializeField] private IInputManager InputController;
        [SerializeField] private CarController CarController;
        
        #endregion

        #region Properties
        #endregion

        #region Unity Events

        private void OnEnable()
        {
            InputEventsSubscription(true);
        }

        private void OnDisable()
        {
            InputEventsSubscription(false);
        }

        #endregion

        #region Methods
        
        protected virtual void InputEventsSubscription(bool subscription)
        {
            if (InputController == null) return;

            if (subscription)
            {
                InputController.MoveDirectionChanged += OnMoveDirectionChanged;
                InputController.WeaponTowerDirectionChanged += TempDirectionMethod;
                InputController.Button += TempClickMethod;
//                inputController.SpecialUse += TempDirectionMethod;
            }
            else
            {
                InputController.MoveDirectionChanged -= OnMoveDirectionChanged;
                InputController.WeaponTowerDirectionChanged -= TempDirectionMethod;
                InputController.Button -= TempClickMethod;
//                inputController.SpecialUse -= TempDirectionMethod;
            }
        }

        private void OnMoveDirectionChanged(Vector2 direction)
        {
            Debug.LogError(direction);
        }

        private void TempClickMethod(ButtonCode code,ButtonState state)
        {
            
        }

        private void TempDirectionMethod(Vector2 direction)
        {
            
        }
        
        #endregion
    }
}