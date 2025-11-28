global using IAMService.Domain.Entities;
global using System.Security.Claims;
global using MediatR;
global using FluentValidation;
global using IAMService.Application.Models.Login;
global using IAMService.Application.Models.User;

global using IAMService.Domain.Interfaces;
global using IAMService.Application.Interfaces;
global using IAMService.Domain.Errors;
global using Microsoft.Extensions.Logging;
global using IAMService.Application.Common;
global using IAMService.Application.UseCases.Users.Commands;