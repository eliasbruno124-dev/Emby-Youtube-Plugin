using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    internal static class YouTubeHttpClientFactory
    {
        private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan AddressAttemptTimeout = TimeSpan.FromSeconds(2);

        internal static SocketsHttpHandler CreateHandler(
            bool allowAutoRedirect,
            DecompressionMethods automaticDecompression)
        {
            return new SocketsHttpHandler
            {
                AllowAutoRedirect = allowAutoRedirect,
                AutomaticDecompression = automaticDecompression,
                ConnectTimeout = ConnectTimeout,
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
                MaxConnectionsPerServer = 16,
                ConnectCallback = ConnectIpv4FirstAsync
            };
        }

        private static async ValueTask<Stream> ConnectIpv4FirstAsync(
            SocketsHttpConnectionContext context,
            CancellationToken cancellationToken)
        {
            var endpoint = context.DnsEndPoint;
            IPAddress[] addresses;
            try
            {
                addresses = await Dns.GetHostAddressesAsync(endpoint.Host)
                    .WaitAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(
                    "DNS lookup was canceled.",
                    ex,
                    cancellationToken);
            }

            // Prefer working IPv4 on hosts whose advertised IPv6 route is a
            // black hole, while retaining IPv6 as a real fallback for v6-only
            // networks. Keep DNS order within each address family.
            var ipv4 = addresses
                .Where(address => address.AddressFamily == AddressFamily.InterNetwork)
                .Distinct()
                .ToList();
            var ipv6 = addresses
                .Where(address => address.AddressFamily == AddressFamily.InterNetworkV6)
                .Distinct()
                .ToList();
            var ordered = ipv4.Take(2)
                .Concat(ipv6.Take(1))
                .Concat(ipv4.Skip(2))
                .Concat(ipv6.Skip(1))
                .ToList();

            Exception? lastError = null;
            foreach (var address in ordered)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true
                };

                try
                {
                    using var attempt = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    attempt.CancelAfter(AddressAttemptTimeout);
                    await socket.ConnectAsync(
                            new IPEndPoint(address, endpoint.Port),
                            attempt.Token)
                        .ConfigureAwait(false);
                    return new NetworkStream(socket, ownsSocket: true);
                }
                catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
                {
                    socket.Dispose();
                    throw new OperationCanceledException(
                        "Connection attempt was canceled.",
                        ex,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    socket.Dispose();
                }
            }

            throw new HttpRequestException(
                $"Could not connect to {endpoint.Host}:{endpoint.Port} over IPv4 or IPv6.",
                lastError);
        }
    }
}
