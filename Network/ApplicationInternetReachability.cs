using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SKTools.Network
{
    public class ApplicationInternetReachability : MonoBehaviour
    {
        private readonly NetworkConnectionChecker _networkConnectionChecker = 
            new NetworkConnectionChecker("api.boardkingsgame.com", TimeSpan.FromMilliseconds(30000));

        private async void Start()
        {
            while (true)
            {
                Debug.Log("__start checking...");
                var result = await _networkConnectionChecker.HasConnection();
                Debug.Log("NetworkConnectionStatus " + result.Item1 + " takes: " + result.Item2 +" ms");
                await Task.Delay(2000);
            }
        }
    }
}
