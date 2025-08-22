// Global using directives

global using System;
global using System.Collections.Generic;
global using System.Reflection;
global using System.Threading;
global using System.Threading.Tasks;
global using Cms.Shared.Abstractions;
global using Cms.Shared.Contracts;
global using Cms.Shared.RefitClients;
global using Contents.Api.Extensions;
global using Contents.Api.Features.Contents.Controllers;
global using Contents.Api.Features.Contents.Data;
global using Contents.Api.Features.Contents.Entities;
global using Contents.Api.Features.Contents.Repositories;
global using Contents.Api.Features.Contents.Services;
global using Contents.Api.Features.Contents.Validators;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Moq;
global using Xunit;