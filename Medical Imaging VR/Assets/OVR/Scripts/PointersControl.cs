/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PointersControl : MonoBehaviour {
    
    bool vrMode = false;
    
    private OVRInputModule _inputModule;
    private OVRInputModule inputModule
    {
        get 
        {
            if (_inputModule == null)
            {
                _inputModule = EventSystem.current.currentInputModule as OVRInputModule;
            }
            return _inputModule;
        }
    }

    void Awake()
    {
        if (GameObject.Find("OVRCameraRig"))
        {
            vrMode = true;
            // In VR we should lock the cursor to the this window.
            Screen.lockCursor = true;
        }
    }

    void Update()
    {
        // lock cursor when user clicks on window
        if (vrMode && Input.GetMouseButtonDown(0))
            Screen.lockCursor = true;
    }

    public void SetUseSphereTest(bool on)
    {
        inputModule.performSphereCastForGazepointer = on;
    }

    public void SetMatchNormal(bool on)
    {
        FindObjectOfType<OVRInputModule>().matchNormalOnPhysicsColliders = on;
    }
    public void SetHideGazepointerByDefault(bool hide)
    {
        OVRGazePointer.instance.hideByDefault = hide;
    }
    public void SetOnlyDimCursorWhenMouseActive(bool dim)
    {
        OVRGazePointer.instance.dimOnHideRequest = dim;
    }

	// Use this for initialization
	void Start () {
        OVRPlayerController playerController = FindObjectOfType<OVRPlayerController>();
        if (playerController)
        {
            playerController.SetSkipMouseRotation(true);
        }
	}

}
