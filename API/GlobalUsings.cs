global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Security.Cryptography;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text;
global using System.Net;
global using System.Text.Json;
global using API.Errors;
global using AutoMapper;
global using AutoMapper.QueryableExtensions;
global using CloudinaryDotNet.Actions;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Mvc.Filters;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



global using API.Entities;
global using API.Data;
global using API.DTOs;
global using API.Services;
global using API.Interfaces;
global using API.Extentions;
global using API.Middleware;
global using API.Helpers;