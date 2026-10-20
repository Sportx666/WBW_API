using System.Security.Cryptography.X509Certificates;

namespace WBM_API.Helpers
{
    public static class MtlsHelper
    {
        public static bool IsClientCertificateAllowed(X509Certificate2? clientCert, IConfiguration configuration, out string reason)
        {
            reason = "";

            if (clientCert == null)
            {
                reason = "Client certificate required";
                return false;
            }

            bool allowAny = configuration.GetValue<bool?>("mTLS:AllowAnyClientCertificate") ?? false;
            if (allowAny)
            {
                return true;
            }

            var allowed = configuration.GetSection("mTLS:AllowedThumbprints").Get<string[]>() ?? Array.Empty<string>();
            string thumb = NormalizeThumbprint(clientCert.Thumbprint);

            if (!allowed.Select(NormalizeThumbprint).Contains(thumb))
            {
                reason = "Client certificate not allowed";
                return false;
            }

            bool validateChain = configuration.GetValue<bool?>("mTLS:ValidateChain") ?? false;
            if (validateChain)
            {
                bool allowSelfSigned = configuration.GetValue<bool?>("mTLS:AllowSelfSigned") ?? true;
                using var chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;

                if (allowSelfSigned)
                {
                    // Permit private PKI / self-signed while still building a chain.
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                }

                if (!chain.Build(clientCert))
                {
                    reason = "Client certificate chain validation failed";
                    return false;
                }
            }

            return true;
        }

        private static string NormalizeThumbprint(string? thumb)
        {
            return (thumb ?? "").Replace(" ", "").Trim().ToUpperInvariant();
        }
    }
}
