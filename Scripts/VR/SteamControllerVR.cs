/// Created by Justin Kasowski

using UnityEngine;
using Valve.VR;

namespace VR
{
    public class SteamControllerVR : MonoBehaviour
    {

        [SerializeField] SteamVR_Action_Boolean
            TriggerOnOff,
            SideButtonOnOff,
            TrackPadRightOnOff,
            TrackPadLeftOnOff,
            TrackPadUpOnOff,
            TrackPadDownOnOff;

        public bool LH_TriggerPressed,
            LH_SideButtonPressed,
            LH_TrackPadRightPressed,
            LH_TrackPadLeftPressed,
            LH_TrackPadUpPressed,
            LH_TrackPadDownPressed,

            RH_TriggerPressed,
            RH_SideButtonPressed,
            RH_TrackPadRightPressed,
            RH_TrackPadLeftPressed,
            RH_TrackPadUpPressed,
            RH_TrackPadDownPressed,

            TriggerPressed,
            SideButtonPressed,
            TrackPadRightPressed,
            TrackPadLeftPressed,
            TrackPadUpPressed,
            TrackPadDownPressed;


        // * Left Hand * //
        void LH_TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TriggerPressed = true;
        }

        void LH_SideButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_SideButtonPressed = true;
        }

        void LH_TrackPadUpDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadUpPressed = true;
        }

        void LH_TrackPadLeftDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadLeftPressed = true;
        }

        void LH_TrackPadRightDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadRightPressed = true;
        }

        void LH_TrackPadDownDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadDownPressed = true;
        }

        void LH_TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TriggerPressed = false;
        }

        void LH_SideButtonUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_SideButtonPressed = false;
        }

        void LH_TrackPadUpUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadUpPressed = false;
        }

        void LH_TrackPadLeftUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadLeftPressed = false;
        }

        void LH_TrackPadRightUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadRightPressed = false;
        }

        void LH_TrackPadDownUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.LH_TrackPadDownPressed = false;
        }

        // * Right Hand * //
        void RH_TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            RH_TriggerPressed = true;
        }

        void RH_SideButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_SideButtonPressed = true;
        }

        void RH_TrackPadUpDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadUpPressed = true;
        }

        void RH_TrackPadLeftDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadLeftPressed = true;
        }

        void RH_TrackPadRightDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadRightPressed = true;
        }

        void RH_TrackPadDownDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadDownPressed = true;
        }

        void RH_TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TriggerPressed = false;
        }

        void RH_SideButtonUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_SideButtonPressed = false;
        }

        void RH_TrackPadUpUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadUpPressed = false;
        }

        void RH_TrackPadLeftUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadLeftPressed = false;
        }

        void RH_TrackPadRightUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadRightPressed = false;
        }

        void RH_TrackPadDownUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Instance.RH_TrackPadDownPressed = false;
        }


        void Start()
        {
            TriggerOnOff.AddOnStateDownListener(LH_TriggerDown, SteamVR_Input_Sources.LeftHand);
            SideButtonOnOff.AddOnStateDownListener(LH_SideButtonDown, SteamVR_Input_Sources.LeftHand);
            TrackPadLeftOnOff.AddOnStateDownListener(LH_TrackPadLeftDown, SteamVR_Input_Sources.LeftHand);
            TrackPadRightOnOff.AddOnStateDownListener(LH_TrackPadRightDown, SteamVR_Input_Sources.LeftHand);
            TrackPadUpOnOff.AddOnStateDownListener(LH_TrackPadUpDown, SteamVR_Input_Sources.LeftHand);
            TrackPadDownOnOff.AddOnStateDownListener(LH_TrackPadDownDown, SteamVR_Input_Sources.LeftHand);

            TriggerOnOff.AddOnStateUpListener(LH_TriggerUp, SteamVR_Input_Sources.LeftHand);
            SideButtonOnOff.AddOnStateUpListener(LH_SideButtonUp, SteamVR_Input_Sources.LeftHand);
            TrackPadLeftOnOff.AddOnStateUpListener(LH_TrackPadLeftUp, SteamVR_Input_Sources.LeftHand);
            TrackPadRightOnOff.AddOnStateUpListener(LH_TrackPadRightUp, SteamVR_Input_Sources.LeftHand);
            TrackPadUpOnOff.AddOnStateUpListener(LH_TrackPadUpUp, SteamVR_Input_Sources.LeftHand);
            TrackPadDownOnOff.AddOnStateUpListener(LH_TrackPadDownUp, SteamVR_Input_Sources.LeftHand);

            TriggerOnOff.AddOnStateDownListener(RH_TriggerDown, SteamVR_Input_Sources.RightHand);
            SideButtonOnOff.AddOnStateDownListener(RH_SideButtonDown, SteamVR_Input_Sources.RightHand);
            TrackPadLeftOnOff.AddOnStateDownListener(RH_TrackPadLeftDown, SteamVR_Input_Sources.RightHand);
            TrackPadRightOnOff.AddOnStateDownListener(RH_TrackPadRightDown, SteamVR_Input_Sources.RightHand);
            TrackPadUpOnOff.AddOnStateDownListener(RH_TrackPadUpDown, SteamVR_Input_Sources.RightHand);
            TrackPadDownOnOff.AddOnStateDownListener(RH_TrackPadDownDown, SteamVR_Input_Sources.RightHand);

            TriggerOnOff.AddOnStateUpListener(RH_TriggerUp, SteamVR_Input_Sources.RightHand);
            SideButtonOnOff.AddOnStateUpListener(RH_SideButtonUp, SteamVR_Input_Sources.RightHand);
            TrackPadLeftOnOff.AddOnStateUpListener(RH_TrackPadLeftUp, SteamVR_Input_Sources.RightHand);
            TrackPadRightOnOff.AddOnStateUpListener(RH_TrackPadRightUp, SteamVR_Input_Sources.RightHand);
            TrackPadUpOnOff.AddOnStateUpListener(RH_TrackPadUpUp, SteamVR_Input_Sources.RightHand);
            TrackPadDownOnOff.AddOnStateUpListener(RH_TrackPadDownUp, SteamVR_Input_Sources.RightHand);
        }

        void Update()
        {
            TriggerPressed = LH_TriggerPressed || RH_TriggerPressed;
            SideButtonPressed = LH_SideButtonPressed || RH_SideButtonPressed;
            TrackPadRightPressed = LH_TrackPadRightPressed || RH_TrackPadRightPressed;
            TrackPadLeftPressed = LH_TrackPadLeftPressed || RH_TrackPadLeftPressed;
            TrackPadUpPressed = LH_TrackPadUpPressed || RH_TrackPadUpPressed;
            TrackPadDownPressed = LH_TrackPadDownPressed || RH_TrackPadDownPressed;
        }

        // Singleton initiated on Awake()
        public static SteamControllerVR Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
