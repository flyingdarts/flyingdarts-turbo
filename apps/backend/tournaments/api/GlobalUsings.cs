// Global using directives

global using System.Text.Json;
global using System.Threading.Tasks;
global using Amazon.ApiGatewayManagementApi;
global using Amazon.DynamoDBv2;
global using Amazon.DynamoDBv2.DataModel;
global using Amazon.Lambda.APIGatewayEvents;
global using Amazon.Lambda.Core;
global using Amazon.Lambda.RuntimeSupport;
global using Amazon.Lambda.Serialization.SystemTextJson;
global using FluentValidation;
global using Flyingdarts.Backend.Games.X01.Services.Connection;
global using Flyingdarts.Backend.Tournaments.Api.Requests.Create;
global using Flyingdarts.Backend.Tournaments.Api.Requests.Participants.Add;
global using Flyingdarts.Backend.Tournaments.Api.Requests.Participants.Matches.Update;
global using Flyingdarts.Backend.Tournaments.Api.Requests.Start;
global using Flyingdarts.Persistence;
global using MediatR;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;