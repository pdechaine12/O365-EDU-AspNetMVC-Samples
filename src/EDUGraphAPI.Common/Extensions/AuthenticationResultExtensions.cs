using EDUGraphAPI.Utils;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EDUGraphAPI
{
    public static class AuthenticationResultExtensions
    {
        public static ActiveDirectoryClient CreateActiveDirectoryClient(this AuthenticationResult authenticationResult)
        {
            return AuthenticationHelper.GetActiveDirectoryClient(authenticationResult);
        }

        public static GraphServiceClient CreateGraphServiceClient(this AuthenticationResult authenticationResult)
        {
            return AuthenticationHelper.GetGraphServiceClient(authenticationResult);
        }
    }
}