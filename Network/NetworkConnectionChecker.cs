using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SKTools.Network
{
    public class NetworkConnectionChecker
    {
        private static SynchronizationContext _synchronizationContext;

        //A DNS-style host name or IP address.
        private readonly string _hostName;
        private readonly TimeSpan _timeout;

        public NetworkConnectionChecker(string hostName, TimeSpan timeout)
        {
            _hostName = hostName;
            _timeout = timeout;
        }

        public enum NetworkState
        {
            NotReachable = 0,
            Timeout,
            Reachable,
            Undefined,
        }

        public async Task<Tuple<NetworkState, long, Exception>> HasConnection()
        {
            var time = new Stopwatch();
            time.Start();

            if (SynchronizationContext.Current == _synchronizationContext)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    time.Stop();
                    return new Tuple<NetworkState, long, Exception>(NetworkState.NotReachable, time.ElapsedMilliseconds, null);
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Call Application.internetReachability is not allowed on not main thread");
            }

            try
            {
                bool result;
                using (var cts = new CancellationTokenSource())
                {
                    cts.CancelAfter(_timeout);
                    result = await Task.Run(() => Check(cts.Token, _hostName), cts.Token).ConfigureAwait(false);
                }

                time.Stop();
                return new Tuple<NetworkState, long, Exception>(
                    result
                        ? NetworkState.Reachable
                        : NetworkState.NotReachable, time.ElapsedMilliseconds, null);
            }
            catch (OperationCanceledException e) //in this case lets try again
            {
                time.Stop();
                return new Tuple<NetworkState, long, Exception>(NetworkState.Timeout, time.ElapsedMilliseconds, e);
            }
            catch (SocketException e) //usually this means network issues and not reachable connection
            {
                time.Stop();
                return new Tuple<NetworkState, long, Exception>(NetworkState.NotReachable, time.ElapsedMilliseconds, e);
            }
            catch (ArgumentNullException e)
            {
                UnityEngine.Debug.LogError(nameof(NetworkConnectionChecker) + " hostNameOrAddress is null");
                time.Stop();
                return new Tuple<NetworkState, long, Exception>(NetworkState.Undefined, time.ElapsedMilliseconds, e);
            }
            catch (ArgumentException e)
            {
                UnityEngine.Debug.LogError(nameof(NetworkConnectionChecker) + " hostNameOrAddress is an invalid IP address.");
                time.Stop();
                return new Tuple<NetworkState, long, Exception>(NetworkState.Undefined, time.ElapsedMilliseconds, e);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(nameof(NetworkConnectionChecker) + " unknown Exception  " + e.Message);
                time.Stop();
                return new Tuple<NetworkState, long, Exception>(NetworkState.Undefined, time.ElapsedMilliseconds, e);
            }
        }

        private static bool Check(CancellationToken token, string hostName, int port = 443)
        {
            var ipHostEntry = Dns.GetHostEntry(hostName);

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                return false;
            }

            if (ipHostEntry.AddressList.Length == 0)
            {
                throw new ArgumentException("The specified host name could not be resolved.");
            }

            foreach (var address in ipHostEntry.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork ||
                    address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    var ipe = new IPEndPoint(address, port); //443 for https , 80 for http
                    var tempSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    var result = tempSocket.BeginConnect(ipe, null, null);
                    //Blocks the current thread until the current WaitHandle receives a signal.
                    var success = result.AsyncWaitHandle.WaitOne(5000, true);

                    if (success)
                    {
                        tempSocket.EndConnect(result);

                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                            return false;
                        }

                        return true;
                    }
                    else
                    {
                        tempSocket.Close();
                        throw new SocketException(10060); // Connection timed out.
                    }
                }
            }

            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CatchUnityContext()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }
    }
}
