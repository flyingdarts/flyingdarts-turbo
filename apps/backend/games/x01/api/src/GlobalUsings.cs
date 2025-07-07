// Global using directives

global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Amazon.ApiGatewayManagementApi;
global using Amazon.ApiGatewayManagementApi.Model;
global using Amazon.DynamoDBv2;
global using Amazon.DynamoDBv2.DataModel;
global using Amazon.Lambda.APIGatewayEvents;
global using Amazon.Lambda.Core;
global using Amazon.Lambda.RuntimeSupport;
global using Amazon.Lambda.Serialization.SystemTextJson;
global using Flyingdarts.Backend.Games.X01.Api;
global using Flyingdarts.Backend.Games.X01.Api.Models;
global using Flyingdarts.Backend.Games.X01.Api.Requests.Create;
global using Flyingdarts.Backend.Games.X01.Api.Requests.Join;
global using Flyingdarts.Backend.Games.X01.Api.Requests.JoinQueue;
global using Flyingdarts.Backend.Games.X01.Api.Requests.Score;
global using Flyingdarts.Meetings.Service;
global using Flyingdarts.Metadata.Services;
global using Flyingdarts.Persistence;
global using MediatR;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
