
global using System;
global using System.Collections.Generic;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Threading.Tasks;
global using IAMService.Application.Interfaces;
global using IAMService.Domain.Entities;
global using IAMService.Domain.Interfaces;
global using IAMService.Infrastructure.Identity;
global using IAMService.Infrastructure.Shared;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Infrastructure;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

global using Shared.Security;