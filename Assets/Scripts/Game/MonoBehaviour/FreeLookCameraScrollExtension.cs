using Cinemachine;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class FreeLookCameraScrollExtension : MonoBehaviour
    {
        public AxisState scrollState = new AxisState(0.1f, 2f,false, false, 20, 0, 0, "", false);

        private CinemachineFreeLook.Orbit[] cacheOrbits;
        private CinemachineFreeLook freeLook;
        private float cacheValue = -1;

        void Awake()
        {
            freeLook = GetComponent<CinemachineFreeLook>();
            if (freeLook != null)
            {
                cacheOrbits = new CinemachineFreeLook.Orbit[3];
                for (int i = 0; i < 3; i++)
                {
                    cacheOrbits[i].m_Height = freeLook.m_Orbits[i].m_Height;
                    cacheOrbits[i].m_Radius = freeLook.m_Orbits[i].m_Radius;
                }
            }
            scrollState.Reset();
            scrollState.Validate();
        }

        void Update()
        {
            if (freeLook != null)
            {
                scrollState.Update(Time.deltaTime);

                if(cacheValue != scrollState.Value)
                {
                    cacheValue = scrollState.Value;
                    var scroll = scrollState.Value;
                    for (int i = 0; i < 3; i++)
                    {
                        freeLook.m_Orbits[i].m_Radius = cacheOrbits[i].m_Radius * scroll;
                    }
                    freeLook.m_Orbits[0].m_Height = (cacheOrbits[0].m_Height - freeLook.m_Orbits[1].m_Height) * scroll +
                                                    freeLook.m_Orbits[1].m_Height;
                    freeLook.m_Orbits[2].m_Height = (cacheOrbits[2].m_Height - freeLook.m_Orbits[1].m_Height) * scroll +
                                                    freeLook.m_Orbits[1].m_Height;
                }

                scrollState.m_InputAxisValue = 0;
            }
        }

    }
}