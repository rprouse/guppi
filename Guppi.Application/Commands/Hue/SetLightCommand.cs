using System;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Hue
{
    public sealed class SetLightCommand : IRequest
    {
        public string IpAddress { get; set; }

        public bool On { get; set; }

        public bool  Off { get; set; }

        public bool Alert { get; set; }

        public byte? Brightness { get; set; }

        public string Color { get; set; }

        public uint Light { get; set; }

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
