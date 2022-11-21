using UnityEngine;

namespace SimpleMenu.Runtime
{
    /// <summary>
    /// stays alive & destroys duplicates
    /// </summary>
    [DisallowMultipleComponent]
    internal sealed class StayAlive : MonoBehaviour
    {
        private static StayAlive s_instance;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic()
        {
            s_instance = null;
        }
        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void OnDestroy()
        {
            {
                if (s_instance != this) return;
                ResetStatic();
            }
        }
    }
}
