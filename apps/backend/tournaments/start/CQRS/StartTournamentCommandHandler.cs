﻿using Amazon.Lambda.APIGatewayEvents;
using Flyingdarts.Backend.Shared.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Flyingdarts.Backend.Tournaments.Create.CQRS
{
    internal class StartTournamentCommandHandler : IRequestHandler<StartTournamentCommand, APIGatewayProxyResponse>
    {
        public async Task<APIGatewayProxyResponse> Handle(StartTournamentCommand request, CancellationToken cancellationToken)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "A yeet"
            };
        }
    }
}