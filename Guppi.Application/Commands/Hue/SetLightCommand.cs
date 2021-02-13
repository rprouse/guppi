using System;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Hue
{
    public sealed class SetLightCommand : IRequest
    {
        public string IpAddress { get; init; }

        public bool On { get; init; }

        public bool  Off { get; init; }

        public bool Alert { get; init; }

        public byte? Brightness { get; init; }

        public string Color { get; init; }

        public uint Light { get; init; }

        public Action<string> WaitForUserInput { get; set; }
    }

    internal sealed class SetLightCommandHandler : IRequestHandler<SetLightCommand>
    {
        private readonly IHueService _hueService;

        public SetLightCommandHandler(IHueService hueService)
        {
            _hueService = hueService;
        }

        public async Task<Unit> Handle(SetLightCommand request, CancellationToken cancellationToken)
        {
            _hueService.WaitForUserInput = request.WaitForUserInput;
            await _hueService.Set(request.IpAddress, request.On, request.Off, request.Alert, request.Brightness, request.Color, request.Light);
            return await Unit.Task;
        }
    }
}
