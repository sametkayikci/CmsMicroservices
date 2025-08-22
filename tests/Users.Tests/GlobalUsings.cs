// Global using directives

global using System;
global using System.Collections.Generic;
global using System.Threading;
global using System.Threading.Tasks;
global using Cms.Shared.Abstractions;
global using Cms.Shared.Caching;
global using Cms.Shared.Contracts;
global using Cms.Shared.RefitClients;
global using Cms.Shared.Security;
global using FluentAssertions;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Moq;
global using Users.Api.Extensions;
global using Users.Api.Features.Users.Controllers;
global using Users.Api.Features.Users.Data;
global using Users.Api.Features.Users.Entities;
global using Users.Api.Features.Users.Repositories;
global using Users.Api.Features.Users.Services;
global using Xunit;