using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Web.Services.Http
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly NavigationManager _navigation;
        private readonly IConfiguration _configuration;

        public AuthorizationMessageHandler(
            IAccessTokenProvider tokenProvider,
            NavigationManager navigation,
            IConfiguration configuration)
        {
            _tokenProvider = tokenProvider;
            _navigation = navigation;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Intentar obtener el token de acceso
            var tokenResult = await _tokenProvider.RequestAccessToken();

            if (tokenResult.TryGetToken(out var token))
            {
                // Agregar el token al header de autorización
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Value);
            }
            else
            {
                // Si no hay token, redirigir al login
                _navigation.NavigateTo("/authentication/login");
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Si la respuesta es 401, el token puede haber expirado
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Intentar refrescar el token
                var refreshResult = await _tokenProvider.RequestAccessToken();

                if (!refreshResult.TryGetToken(out _))
                {
                    // Si no se puede refrescar, redirigir al login
                    _navigation.NavigateTo("/authentication/login");
                }
            }

            return response;
        }
    }
}