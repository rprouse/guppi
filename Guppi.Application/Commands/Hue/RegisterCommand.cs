using System;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Hue
{
    public sealed class RegisterCommand : IRequest<bool>
    {
        public string IpAddress { get; set; }

        public Action<string> WaitForUserInput { get; set; }
    }

    internal sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly IHueService _hueService;

        public RegisterCommandHandler(IHueService hueService)
        {
            _hueService = hueService;
        }

        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            _hueService.WaitForUserInput = request.WaitForUserInput;
            return await _hueService.Register(request.IpAddress);
        }
    }
}
