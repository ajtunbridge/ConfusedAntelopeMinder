using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ngen.Core.Security
{
    /// <summary>
    /// Allows you to impersonate a domain user for the duration of a using statement
    /// </summary>
    public class Impersonator : IDisposable
    {
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        private WindowsImpersonationContext _impersonationContext;
        
        public Impersonator(NetworkCredential credentials)
        {
            ImpersonateValidUser(credentials.UserName, credentials.Domain, credentials.Password);
        }

        public void Dispose()
        {
            UndoImpersonation();
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int LogonUser(
            string lpszUserName,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int DuplicateToken(
            IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool CloseHandle(
            IntPtr handle);

        /// <summary>
        ///     Does the actual impersonation.
        /// </summary>
        /// <param name="userName">The name of the user to act as.</param>
        /// <param name="domain">The domain name of the user to act as.</param>
        /// <param name="password">The password of the user to act as.</param>
        private void ImpersonateValidUser(string userName, string domain, string password)
        {
            var token = IntPtr.Zero;
            var tokenDuplicate = IntPtr.Zero;

            try
            {
                if (RevertToSelf())
                    if (LogonUser(
                            userName,
                            domain,
                            password,
                            LOGON32_LOGON_INTERACTIVE,
                            LOGON32_PROVIDER_DEFAULT,
                            ref token) != 0)
                        if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                        {
                            var tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                            _impersonationContext = tempWindowsIdentity.Impersonate();
                        }
                        else
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    else
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (token != IntPtr.Zero)
                    CloseHandle(token);
                if (tokenDuplicate != IntPtr.Zero)
                    CloseHandle(tokenDuplicate);
            }
        }

        /// <summary>
        ///     Reverts the impersonation.
        /// </summary>
        private void UndoImpersonation()
        {
            _impersonationContext?.Undo();
        }
    }
}